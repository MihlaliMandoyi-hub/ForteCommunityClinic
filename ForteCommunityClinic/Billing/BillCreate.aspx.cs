using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Billing
{
    public partial class BillCreate : System.Web.UI.Page
    {
        private DataTable DraftItems
        {
            get
            {
                if (ViewState["DraftBillItems"] == null)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("Description", typeof(string));
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("UnitPrice", typeof(decimal));
                    dt.Columns.Add("LineTotal", typeof(decimal));
                    ViewState["DraftBillItems"] = dt;
                }
                return (DataTable)ViewState["DraftBillItems"];
            }
            set { ViewState["DraftBillItems"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");
        }

        protected void btnFindPatient_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT p.PatientID, p.PatientNumber + ' - ' + u.FirstName + ' ' + u.Surname AS PatientDisplay
                    FROM Patients p JOIN Users u ON p.UserID = u.UserID
                    WHERE p.IsActive = 1
                      AND (p.PatientNumber LIKE '%' + @Search + '%' OR u.Surname LIKE '%' + @Search + '%')", conn);
                cmd.Parameters.AddWithValue("@Search", txtPatientSearch.Text.Trim());

                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlPatient.DataSource = dt;
                ddlPatient.DataTextField = "PatientDisplay";
                ddlPatient.DataValueField = "PatientID";
                ddlPatient.DataBind();
            }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescription.Text) || string.IsNullOrWhiteSpace(txtUnitPrice.Text))
            {
                lblMessage.Text = "Description and unit price are required.";
                lblMessage.Visible = true;
                return;
            }

            int quantity = string.IsNullOrWhiteSpace(txtQuantity.Text) ? 1 : int.Parse(txtQuantity.Text);
            decimal unitPrice = decimal.Parse(txtUnitPrice.Text);

            var items = DraftItems;
            var row = items.NewRow();
            row["Description"] = txtDescription.Text.Trim();
            row["Quantity"] = quantity;
            row["UnitPrice"] = unitPrice;
            row["LineTotal"] = quantity * unitPrice;
            items.Rows.Add(row);
            DraftItems = items;

            gvDraftItems.DataSource = items;
            gvDraftItems.DataBind();

            decimal grandTotal = 0;
            foreach (DataRow r in items.Rows) grandTotal += (decimal)r["LineTotal"];
            lblGrandTotal.Text = grandTotal.ToString("C2");

            txtDescription.Text = "";
            txtQuantity.Text = "1";
            txtUnitPrice.Text = "";
            lblMessage.Visible = false;
        }

        protected void btnSaveBill_Click(object sender, EventArgs e)
        {
            if (ddlPatient.Items.Count == 0)
            {
                lblMessage.Text = "Please find and select a patient.";
                lblMessage.Visible = true;
                return;
            }
            if (DraftItems.Rows.Count == 0)
            {
                lblMessage.Text = "Add at least one line item before saving.";
                lblMessage.Visible = true;
                return;
            }

            int patientId = int.Parse(ddlPatient.SelectedValue);
            decimal totalAmount = 0;
            foreach (DataRow r in DraftItems.Rows) totalAmount += (decimal)r["LineTotal"];

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    var insertBillCmd = new SqlCommand(@"
                        INSERT INTO Bills (PatientID, TotalAmount)
                        OUTPUT INSERTED.BillID
                        VALUES (@PatientID, @TotalAmount)", conn, transaction);
                    insertBillCmd.Parameters.AddWithValue("@PatientID", patientId);
                    insertBillCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                    int billId = (int)insertBillCmd.ExecuteScalar();

                    foreach (DataRow row in DraftItems.Rows)
                    {
                        var insertItemCmd = new SqlCommand(@"
                            INSERT INTO BillItems (BillID, Description, Quantity, UnitPrice)
                            VALUES (@BillID, @Description, @Quantity, @UnitPrice)", conn, transaction);
                        insertItemCmd.Parameters.AddWithValue("@BillID", billId);
                        insertItemCmd.Parameters.AddWithValue("@Description", row["Description"]);
                        insertItemCmd.Parameters.AddWithValue("@Quantity", row["Quantity"]);
                        insertItemCmd.Parameters.AddWithValue("@UnitPrice", row["UnitPrice"]);
                        insertItemCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    using (var auditConn = DatabaseHelper.GetConnection())
                    {
                        auditConn.Open();
                        AuditHelper.Log(auditConn, (int?)Session["UserID"], "CREATE BILL", "Bill", billId, "Bill created.");
                    }

                    Response.Redirect("BillDetails.aspx?id=" + billId);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    lblMessage.Text = "An error occurred while saving the bill. Please try again.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
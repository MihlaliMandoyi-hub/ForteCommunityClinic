using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Billing
{
    public partial class Payments : System.Web.UI.Page
    {
        private int billId;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

            if (!int.TryParse(Request.QueryString["billId"], out billId))
            {
                Response.Redirect("~/Billing/BillCreate.aspx");
                return;
            }

            lnkBack.HRef = "BillDetails.aspx?id=" + billId;

            if (!IsPostBack)
            {
                LoadBillInfo();
                LoadPaymentMethods();
            }
        }

        private void LoadBillInfo()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT u.FirstName + ' ' + u.Surname AS PatientName, b.OutstandingAmount
                    FROM Bills b
                    JOIN Patients p ON b.PatientID = p.PatientID
                    JOIN Users u ON p.UserID = u.UserID
                    WHERE b.BillID = @BillID", conn);
                cmd.Parameters.AddWithValue("@BillID", billId);

                conn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    Response.Redirect("~/Billing/BillCreate.aspx");
                    return;
                }

                lblBillInfo.Text = $"Patient: {reader["PatientName"]} — Outstanding: {Convert.ToDecimal(reader["OutstandingAmount"]):C2}";
            }
        }

        private void LoadPaymentMethods()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand("SELECT PaymentMethodID, MethodName FROM PaymentMethods", conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlPaymentMethod.DataSource = dt;
                ddlPaymentMethod.DataTextField = "MethodName";
                ddlPaymentMethod.DataValueField = "PaymentMethodID";
                ddlPaymentMethod.DataBind();
            }
        }

        protected void btnRecordPayment_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            decimal amount = decimal.Parse(txtAmount.Text);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Check payment won't overpay the bill (matches the CHK_AmountPaid constraint from Phase 3)
                    var checkCmd = new SqlCommand(
                        "SELECT TotalAmount, AmountPaid FROM Bills WHERE BillID = @BillID", conn, transaction);
                    checkCmd.Parameters.AddWithValue("@BillID", billId);
                    var reader = checkCmd.ExecuteReader();
                    reader.Read();
                    decimal totalAmount = (decimal)reader["TotalAmount"];
                    decimal currentPaid = (decimal)reader["AmountPaid"];
                    reader.Close();

                    if (currentPaid + amount > totalAmount)
                    {
                        transaction.Rollback();
                        lblMessage.Text = $"This payment would exceed the bill total. Maximum you can record is {(totalAmount - currentPaid):C2}.";
                        lblMessage.Visible = true;
                        return;
                    }

                    string referenceNumber = string.IsNullOrWhiteSpace(txtReference.Text)
                        ? "RCPT-" + DateTime.Now.ToString("yyyyMMddHHmmss")
                        : txtReference.Text.Trim();

                    var insertPaymentCmd = new SqlCommand(@"
                        INSERT INTO Payments (BillID, PaymentMethodID, AmountPaid, ReceivedBy, ReferenceNumber)
                        OUTPUT INSERTED.PaymentID
                        VALUES (@BillID, @PaymentMethodID, @AmountPaid, @ReceivedBy, @ReferenceNumber)",
                        conn, transaction);
                    insertPaymentCmd.Parameters.AddWithValue("@BillID", billId);
                    insertPaymentCmd.Parameters.AddWithValue("@PaymentMethodID", int.Parse(ddlPaymentMethod.SelectedValue));
                    insertPaymentCmd.Parameters.AddWithValue("@AmountPaid", amount);
                    insertPaymentCmd.Parameters.AddWithValue("@ReceivedBy", Session["UserID"]);
                    insertPaymentCmd.Parameters.AddWithValue("@ReferenceNumber", referenceNumber);
                    int paymentId = (int)insertPaymentCmd.ExecuteScalar();

                    decimal newAmountPaid = currentPaid + amount;
                    string newStatus = newAmountPaid >= totalAmount ? "Paid" : "PartiallyPaid";

                    var updateBillCmd = new SqlCommand(
                        "UPDATE Bills SET AmountPaid = @AmountPaid, Status = @Status WHERE BillID = @BillID",
                        conn, transaction);
                    updateBillCmd.Parameters.AddWithValue("@AmountPaid", newAmountPaid);
                    updateBillCmd.Parameters.AddWithValue("@Status", newStatus);
                    updateBillCmd.Parameters.AddWithValue("@BillID", billId);
                    updateBillCmd.ExecuteNonQuery();

                    transaction.Commit();

                    using (var auditConn = DatabaseHelper.GetConnection())
                    {
                        auditConn.Open();
                        AuditHelper.Log(auditConn, (int?)Session["UserID"], "RECORD PAYMENT", "Payment", paymentId, "Payment recorded.");
                    }

                    Response.Redirect("Receipt.aspx?paymentId=" + paymentId);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    lblMessage.Text = "An error occurred while recording the payment. Please try again.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
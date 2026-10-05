using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Billing
{
    public partial class MyBillDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Patient");

            int billId;
            if (!int.TryParse(Request.QueryString["id"], out billId))
            {
                Response.Redirect("~/Billing/MyBills.aspx");
                return;
            }

            LoadBill(billId);
        }

        private void LoadBill(int billId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                // Ownership check baked directly into the query — a patient can only ever
                // load a bill that belongs to THEIR OWN PatientID, never anyone else's
                var cmd = new SqlCommand(@"
                    SELECT b.BillDate, b.Status, b.TotalAmount, b.AmountPaid, b.OutstandingAmount
                    FROM Bills b
                    WHERE b.BillID = @BillID
                      AND b.PatientID = (SELECT PatientID FROM Patients WHERE UserID = @UserID)", conn);
                cmd.Parameters.AddWithValue("@BillID", billId);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    lblNotFound.Visible = true;
                    pnlDetails.Visible = false;
                    return;
                }

                litDate.Text = Convert.ToDateTime(reader["BillDate"]).ToString("yyyy-MM-dd HH:mm");
                litStatus.Text = reader["Status"].ToString();
                litTotal.Text = Convert.ToDecimal(reader["TotalAmount"]).ToString("C2");
                litPaid.Text = Convert.ToDecimal(reader["AmountPaid"]).ToString("C2");
                litOutstanding.Text = Convert.ToDecimal(reader["OutstandingAmount"]).ToString("C2");
                reader.Close();

                lnkMakePayment.HRef = "MyBillPayment.aspx?billId=" + billId;
                lnkMakePayment.Visible = reader["Status"].ToString() != "Paid";

                LoadBillItems(conn, billId);
                LoadPayments(conn, billId);
            }
        }

        private void LoadBillItems(SqlConnection conn, int billId)
        {
            var cmd = new SqlCommand(
                "SELECT Description, Quantity, UnitPrice, LineTotal FROM BillItems WHERE BillID = @BillID", conn);
            cmd.Parameters.AddWithValue("@BillID", billId);

            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            gvBillItems.DataSource = dt;
            gvBillItems.DataBind();
        }

        private void LoadPayments(SqlConnection conn, int billId)
        {
            var cmd = new SqlCommand(@"
                SELECT pay.PaymentDate, pm.MethodName, pay.AmountPaid, pay.ReferenceNumber
                FROM Payments pay
                JOIN PaymentMethods pm ON pay.PaymentMethodID = pm.PaymentMethodID
                WHERE pay.BillID = @BillID
                ORDER BY pay.PaymentDate DESC", conn);
            cmd.Parameters.AddWithValue("@BillID", billId);

            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            gvPayments.DataSource = dt;
            gvPayments.DataBind();
        }
    }
}
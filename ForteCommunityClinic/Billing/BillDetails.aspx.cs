using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Billing
{
    public partial class BillDetails : System.Web.UI.Page
    {
        private int billId;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

            if (!int.TryParse(Request.QueryString["id"], out billId))
            {
                Response.Redirect("~/Billing/BillCreate.aspx");
                return;
            }

            lnkRecordPayment.HRef = "Payments.aspx?billId=" + billId;

            LoadBill();
            LoadBillItems();
            LoadPayments();
        }

        private void LoadBill()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT u.FirstName + ' ' + u.Surname + ' (' + p.PatientNumber + ')' AS PatientInfo,
                           b.BillDate, b.Status, b.TotalAmount, b.AmountPaid, b.OutstandingAmount
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

                litPatient.Text = reader["PatientInfo"].ToString();
                litDate.Text = Convert.ToDateTime(reader["BillDate"]).ToString("yyyy-MM-dd HH:mm");
                litStatus.Text = reader["Status"].ToString();
                litTotal.Text = Convert.ToDecimal(reader["TotalAmount"]).ToString("C2");
                litPaid.Text = Convert.ToDecimal(reader["AmountPaid"]).ToString("C2");
                litOutstanding.Text = Convert.ToDecimal(reader["OutstandingAmount"]).ToString("C2");
            }
        }

        private void LoadBillItems()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(
                    "SELECT Description, Quantity, UnitPrice, LineTotal FROM BillItems WHERE BillID = @BillID", conn);
                cmd.Parameters.AddWithValue("@BillID", billId);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvBillItems.DataSource = dt;
                gvBillItems.DataBind();
            }
        }

        private void LoadPayments()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT pay.PaymentDate, pm.MethodName, pay.AmountPaid, pay.ReferenceNumber
                    FROM Payments pay
                    JOIN PaymentMethods pm ON pay.PaymentMethodID = pm.PaymentMethodID
                    WHERE pay.BillID = @BillID
                    ORDER BY pay.PaymentDate DESC", conn);
                cmd.Parameters.AddWithValue("@BillID", billId);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvPayments.DataSource = dt;
                gvPayments.DataBind();
            }
        }
    }
}
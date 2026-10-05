using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Billing
{
    public partial class Receipt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

            int paymentId;
            if (!int.TryParse(Request.QueryString["paymentId"], out paymentId))
            {
                Response.Redirect("~/Billing/BillCreate.aspx");
                return;
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT pay.ReferenceNumber, u1.FirstName + ' ' + u1.Surname AS PatientName,
                           pay.AmountPaid, pm.MethodName, pay.PaymentDate,
                           u2.FirstName + ' ' + u2.Surname AS ReceivedByName, pay.BillID
                    FROM Payments pay
                    JOIN Bills b ON pay.BillID = b.BillID
                    JOIN Patients p ON b.PatientID = p.PatientID
                    JOIN Users u1 ON p.UserID = u1.UserID
                    JOIN PaymentMethods pm ON pay.PaymentMethodID = pm.PaymentMethodID
                    JOIN Users u2 ON pay.ReceivedBy = u2.UserID
                    WHERE pay.PaymentID = @PaymentID", conn);
                cmd.Parameters.AddWithValue("@PaymentID", paymentId);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    Response.Redirect("~/Billing/BillCreate.aspx");
                    return;
                }

                litReference.Text = reader["ReferenceNumber"].ToString();
                litPatient.Text = reader["PatientName"].ToString();
                litAmount.Text = Convert.ToDecimal(reader["AmountPaid"]).ToString("C2");
                litMethod.Text = reader["MethodName"].ToString();
                litDate.Text = Convert.ToDateTime(reader["PaymentDate"]).ToString("yyyy-MM-dd HH:mm");
                litReceivedBy.Text = reader["ReceivedByName"].ToString();

                lnkBackToBill.HRef = "BillDetails.aspx?id=" + reader["BillID"];
            }
        }
    }
}
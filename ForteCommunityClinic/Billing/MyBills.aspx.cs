using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Billing
{
    public partial class MyBills : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Patient");

            if (!IsPostBack)
            {
                BindBills();
            }
        }

        private void BindBills()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT BillID, BillDate, TotalAmount, AmountPaid, OutstandingAmount, Status
                    FROM Bills
                    WHERE PatientID = (SELECT PatientID FROM Patients WHERE UserID = @UserID)
                    ORDER BY BillDate DESC", conn);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvMyBills.DataSource = dt;
                gvMyBills.DataBind();
            }
        }

        protected string GetStatusBadgeClass(string status)
        {
            switch (status)
            {
                case "Paid": return "bg-success";
                case "PartiallyPaid": return "bg-warning text-dark";
                default: return "bg-danger"; // Unpaid
            }
        }
    }
}
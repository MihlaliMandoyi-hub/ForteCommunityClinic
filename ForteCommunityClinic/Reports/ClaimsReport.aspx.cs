using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Reports
{
    public partial class ClaimsReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "ClinicManager", "Admin");

            if (!IsPostBack)
            {
                RunReport();
            }
        }

        private void RunReport()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                var byStatusCmd = new SqlCommand(@"
                    SELECT s.StatusName, COUNT(*) AS ClaimCount, SUM(c.ClaimAmount) AS TotalAmount
                    FROM MedicalAidClaims c
                    JOIN ClaimStatuses s ON c.ClaimStatusID = s.ClaimStatusID
                    GROUP BY s.StatusName
                    ORDER BY ClaimCount DESC", conn);
                var byStatusAdapter = new SqlDataAdapter(byStatusCmd);
                var byStatusTable = new DataTable();
                byStatusAdapter.Fill(byStatusTable);
                gvByStatus.DataSource = byStatusTable;
                gvByStatus.DataBind();

                var outstandingCmd = new SqlCommand(@"
                    SELECT u.FirstName + ' ' + u.Surname AS PatientName, b.BillDate, b.TotalAmount, b.OutstandingAmount, b.Status
                    FROM Bills b
                    JOIN Patients p ON b.PatientID = p.PatientID
                    JOIN Users u ON p.UserID = u.UserID
                    WHERE b.OutstandingAmount > 0
                    ORDER BY b.OutstandingAmount DESC", conn);
                var outstandingAdapter = new SqlDataAdapter(outstandingCmd);
                var outstandingTable = new DataTable();
                outstandingAdapter.Fill(outstandingTable);
                gvOutstanding.DataSource = outstandingTable;
                gvOutstanding.DataBind();
            }
        }
    }
}
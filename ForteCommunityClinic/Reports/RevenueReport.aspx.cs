using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Reports
{
    public partial class RevenueReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "ClinicManager", "Admin");

            if (!IsPostBack)
            {
                txtFromDate.Text = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
                txtToDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                RunReport();
            }
        }

        protected void btnRun_Click(object sender, EventArgs e) => RunReport();

        private void RunReport()
        {
            DateTime fromDate = string.IsNullOrWhiteSpace(txtFromDate.Text) ? DateTime.Today.AddDays(-30) : DateTime.Parse(txtFromDate.Text);
            DateTime toDate = string.IsNullOrWhiteSpace(txtToDate.Text) ? DateTime.Today : DateTime.Parse(txtToDate.Text);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                var periodCmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(AmountPaid), 0) FROM Payments
                    WHERE CAST(PaymentDate AS DATE) BETWEEN @FromDate AND @ToDate", conn);
                periodCmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                periodCmd.Parameters.AddWithValue("@ToDate", toDate.Date);
                lblPeriodRevenue.Text = Convert.ToDecimal(periodCmd.ExecuteScalar()).ToString("C2");

                var todayCmd = new SqlCommand(
                    "SELECT ISNULL(SUM(AmountPaid), 0) FROM Payments WHERE CAST(PaymentDate AS DATE) = CAST(GETDATE() AS DATE)", conn);
                lblTodayRevenue.Text = Convert.ToDecimal(todayCmd.ExecuteScalar()).ToString("C2");

                var monthCmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(AmountPaid), 0) FROM Payments
                    WHERE YEAR(PaymentDate) = YEAR(GETDATE()) AND MONTH(PaymentDate) = MONTH(GETDATE())", conn);
                lblMonthRevenue.Text = Convert.ToDecimal(monthCmd.ExecuteScalar()).ToString("C2");

                var byMethodCmd = new SqlCommand(@"
                    SELECT pm.MethodName, SUM(pay.AmountPaid) AS TotalAmount, COUNT(*) AS PaymentCount
                    FROM Payments pay
                    JOIN PaymentMethods pm ON pay.PaymentMethodID = pm.PaymentMethodID
                    WHERE CAST(pay.PaymentDate AS DATE) BETWEEN @FromDate AND @ToDate
                    GROUP BY pm.MethodName
                    ORDER BY TotalAmount DESC", conn);
                byMethodCmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                byMethodCmd.Parameters.AddWithValue("@ToDate", toDate.Date);
                var byMethodAdapter = new SqlDataAdapter(byMethodCmd);
                var byMethodTable = new DataTable();
                byMethodAdapter.Fill(byMethodTable);
                gvByMethod.DataSource = byMethodTable;
                gvByMethod.DataBind();

                var detailCmd = new SqlCommand(@"
                    SELECT pay.PaymentDate, u.FirstName + ' ' + u.Surname AS PatientName, pm.MethodName, pay.AmountPaid
                    FROM Payments pay
                    JOIN Bills b ON pay.BillID = b.BillID
                    JOIN Patients p ON b.PatientID = p.PatientID
                    JOIN Users u ON p.UserID = u.UserID
                    JOIN PaymentMethods pm ON pay.PaymentMethodID = pm.PaymentMethodID
                    WHERE CAST(pay.PaymentDate AS DATE) BETWEEN @FromDate AND @ToDate
                    ORDER BY pay.PaymentDate DESC", conn);
                detailCmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                detailCmd.Parameters.AddWithValue("@ToDate", toDate.Date);
                var detailAdapter = new SqlDataAdapter(detailCmd);
                var detailTable = new DataTable();
                detailAdapter.Fill(detailTable);
                gvPayments.DataSource = detailTable;
                gvPayments.DataBind();
            }
        }
    }
}
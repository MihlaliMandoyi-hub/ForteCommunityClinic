using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Reports
{
    public partial class DailyAppointments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "ClinicManager", "Admin");

            if (!IsPostBack)
            {
                txtReportDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                RunReport();
            }
        }

        protected void btnRun_Click(object sender, EventArgs e) => RunReport();

        private void RunReport()
        {
            DateTime reportDate = string.IsNullOrWhiteSpace(txtReportDate.Text)
                ? DateTime.Today : DateTime.Parse(txtReportDate.Text);

            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT a.AppointmentTime, u1.FirstName + ' ' + u1.Surname AS PatientName,
                           u2.FirstName + ' ' + u2.Surname AS DoctorName, s.StatusName
                    FROM Appointments a
                    JOIN Patients p ON a.PatientID = p.PatientID
                    JOIN Users u1 ON p.UserID = u1.UserID
                    JOIN Doctors d ON a.DoctorID = d.DoctorID
                    JOIN Users u2 ON d.UserID = u2.UserID
                    JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                    WHERE a.AppointmentDate = @ReportDate
                    ORDER BY a.AppointmentTime", conn);
                cmd.Parameters.AddWithValue("@ReportDate", reportDate.Date);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvAppointments.DataSource = dt;
                gvAppointments.DataBind();

                lblTotal.Text = dt.Rows.Count.ToString();

                int completed = 0, cancelled = 0, missed = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string status = row["StatusName"].ToString();
                    if (status == "Completed") completed++;
                    else if (status == "Cancelled") cancelled++;
                    else if (status == "Missed") missed++;
                }
                lblCompleted.Text = completed.ToString();
                lblCancelled.Text = cancelled.ToString();
                lblMissed.Text = missed.ToString();
            }
        }
    }
}
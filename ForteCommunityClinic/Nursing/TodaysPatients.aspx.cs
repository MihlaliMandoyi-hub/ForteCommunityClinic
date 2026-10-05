using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Nursing
{
    public partial class TodaysPatients : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Nurse", "Admin");

            if (!IsPostBack)
            {
                BindTodaysPatients();
            }
        }

        private void BindTodaysPatients()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT a.AppointmentID, u1.FirstName + ' ' + u1.Surname AS PatientName,
                           u2.FirstName + ' ' + u2.Surname AS DoctorName,
                           a.AppointmentTime, s.StatusName
                    FROM Appointments a
                    JOIN Patients p ON a.PatientID = p.PatientID
                    JOIN Users u1 ON p.UserID = u1.UserID
                    JOIN Doctors d ON a.DoctorID = d.DoctorID
                    JOIN Users u2 ON d.UserID = u2.UserID
                    JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                    WHERE a.AppointmentDate = CAST(GETDATE() AS DATE)
                      AND s.StatusName IN ('Scheduled', 'Checked In')
                    ORDER BY a.AppointmentTime", conn);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvTodaysPatients.DataSource = dt;
                gvTodaysPatients.DataBind();
            }
        }
    }
}
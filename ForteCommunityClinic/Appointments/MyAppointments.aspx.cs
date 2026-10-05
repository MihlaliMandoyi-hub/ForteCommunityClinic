using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Appointments
{
    public partial class MyAppointments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Patient");

            if (!IsPostBack)
            {
                BindMyAppointments();
            }
        }

        private void BindMyAppointments()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT a.AppointmentID, u.FirstName + ' ' + u.Surname AS DoctorName,
                           a.AppointmentDate, a.AppointmentTime, s.StatusName
                    FROM Appointments a
                    JOIN Doctors d ON a.DoctorID = d.DoctorID
                    JOIN Users u ON d.UserID = u.UserID
                    JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                    WHERE a.PatientID = (SELECT PatientID FROM Patients WHERE UserID = @UserID)
                    ORDER BY a.AppointmentDate DESC, a.AppointmentTime", conn);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvMyAppointments.DataSource = dt;
                gvMyAppointments.DataBind();
            }
        }

        protected void gvMyAppointments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Cancel") return;

            int appointmentId = int.Parse(e.CommandArgument.ToString());

            using (var conn = DatabaseHelper.GetConnection())
            {
                // Ownership check — a patient can only cancel their OWN appointment
                var cmd = new SqlCommand(@"
                    UPDATE Appointments
                    SET AppointmentStatusID = (SELECT AppointmentStatusID FROM AppointmentStatuses WHERE StatusName = 'Cancelled'),
                        CancelledReason = 'Cancelled by patient'
                    WHERE AppointmentID = @AppointmentID
                      AND PatientID = (SELECT PatientID FROM Patients WHERE UserID = @UserID)", conn);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            BindMyAppointments();
        }
    }
}
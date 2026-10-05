using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Appointments
{
    public partial class AppointmentReschedule : System.Web.UI.Page
    {
        private int appointmentId;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

            if (!int.TryParse(Request.QueryString["id"], out appointmentId))
            {
                Response.Redirect("~/Appointments/AppointmentList.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadAppointmentInfo();
            }
        }

        private void LoadAppointmentInfo()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT u1.FirstName + ' ' + u1.Surname AS PatientName,
                           u2.FirstName + ' ' + u2.Surname AS DoctorName,
                           a.AppointmentDate, a.AppointmentTime, s.StatusName
                    FROM Appointments a
                    JOIN Patients p ON a.PatientID = p.PatientID
                    JOIN Users u1 ON p.UserID = u1.UserID
                    JOIN Doctors d ON a.DoctorID = d.DoctorID
                    JOIN Users u2 ON d.UserID = u2.UserID
                    JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                    WHERE a.AppointmentID = @AppointmentID", conn);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    Response.Redirect("~/Appointments/AppointmentList.aspx");
                    return;
                }

                string status = reader["StatusName"].ToString();
                if (status != "Scheduled" && status != "Checked In")
                {
                    reader.Close();
                    Response.Redirect("~/Appointments/AppointmentList.aspx");
                    return;
                }

                lblAppointmentInfo.Text = $"{reader["PatientName"]} with {reader["DoctorName"]} — currently " +
                    $"{Convert.ToDateTime(reader["AppointmentDate"]):yyyy-MM-dd} at {(TimeSpan)reader["AppointmentTime"]:hh\\:mm}";

                txtDate.Text = Convert.ToDateTime(reader["AppointmentDate"]).ToString("yyyy-MM-dd");
                txtTime.Text = ((TimeSpan)reader["AppointmentTime"]).ToString(@"hh\:mm");
            }
        }

        protected void btnReschedule_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            DateTime newDate = DateTime.Parse(txtDate.Text);
            TimeSpan newTime = TimeSpan.Parse(txtTime.Text);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Same double-booking check as AppointmentCreate.aspx (Phase 9), but excluding
                // THIS appointment itself — otherwise it would always conflict with its own current slot
                var checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Appointments a
                    JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                    WHERE a.DoctorID = (SELECT DoctorID FROM Appointments WHERE AppointmentID = @AppointmentID)
                      AND a.AppointmentDate = @NewDate
                      AND a.AppointmentTime = @NewTime
                      AND s.StatusName NOT IN ('Cancelled', 'Missed')
                      AND a.AppointmentID <> @AppointmentID", conn);
                checkCmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                checkCmd.Parameters.AddWithValue("@NewDate", newDate.Date);
                checkCmd.Parameters.AddWithValue("@NewTime", newTime);

                if ((int)checkCmd.ExecuteScalar() > 0)
                {
                    lblMessage.Text = "This doctor already has an appointment at that date and time. Please choose a different slot.";
                    lblMessage.Visible = true;
                    return;
                }

                var updateCmd = new SqlCommand(@"
                    UPDATE Appointments
                    SET AppointmentDate = @NewDate, AppointmentTime = @NewTime
                    WHERE AppointmentID = @AppointmentID", conn);
                updateCmd.Parameters.AddWithValue("@NewDate", newDate.Date);
                updateCmd.Parameters.AddWithValue("@NewTime", newTime);
                updateCmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                try
                {
                    updateCmd.ExecuteNonQuery();
                    AuditHelper.Log(conn, (int?)Session["UserID"], "RESCHEDULE APPOINTMENT", "Appointment", appointmentId,
                        $"Rescheduled to {newDate:yyyy-MM-dd} {newTime:hh\\:mm}.");
                    Response.Redirect("~/Appointments/AppointmentList.aspx");
                }
                catch (SqlException)
                {
                    lblMessage.Text = "This slot was just booked by someone else. Please choose a different time.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
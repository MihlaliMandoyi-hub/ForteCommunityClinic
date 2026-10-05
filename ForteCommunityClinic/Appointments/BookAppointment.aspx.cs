using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Appointments
{
    public partial class BookAppointment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Patient");

            if (!IsPostBack)
            {
                LoadDoctors();
            }
        }

        private void LoadDoctors()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT d.DoctorID, u.FirstName + ' ' + u.Surname +
                           ISNULL(' - ' + d.Specialization, '') AS DoctorName
                    FROM Doctors d JOIN Users u ON d.UserID = u.UserID
                    WHERE d.IsActive = 1", conn);

                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlDoctor.DataSource = dt;
                ddlDoctor.DataTextField = "DoctorName";
                ddlDoctor.DataValueField = "DoctorID";
                ddlDoctor.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            if (ddlDoctor.Items.Count == 0)
            {
                lblMessage.Text = "No doctors are currently available.";
                lblMessage.Visible = true;
                return;
            }

            int doctorId = int.Parse(ddlDoctor.SelectedValue);
            DateTime appointmentDate = DateTime.Parse(txtDate.Text);
            TimeSpan appointmentTime = TimeSpan.Parse(txtTime.Text);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Same double-booking check used in staff booking (Phase 9)
                var checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Appointments a
                    JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                    WHERE a.DoctorID = @DoctorID
                      AND a.AppointmentDate = @AppointmentDate
                      AND a.AppointmentTime = @AppointmentTime
                      AND s.StatusName NOT IN ('Cancelled', 'Missed')", conn);
                checkCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                checkCmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate.Date);
                checkCmd.Parameters.AddWithValue("@AppointmentTime", appointmentTime);

                if ((int)checkCmd.ExecuteScalar() > 0)
                {
                    lblMessage.Text = "That doctor already has an appointment at that date and time. Please choose a different slot.";
                    lblMessage.Visible = true;
                    return;
                }

                var insertCmd = new SqlCommand(@"
                    INSERT INTO Appointments (PatientID, DoctorID, AppointmentDate, AppointmentTime, Reason, CreatedBy)
                    VALUES ((SELECT PatientID FROM Patients WHERE UserID = @UserID),
                             @DoctorID, @AppointmentDate, @AppointmentTime, @Reason, @UserID)", conn);
                insertCmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                insertCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                insertCmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate.Date);
                insertCmd.Parameters.AddWithValue("@AppointmentTime", appointmentTime);
                insertCmd.Parameters.AddWithValue("@Reason",
                    string.IsNullOrWhiteSpace(txtReason.Text) ? (object)DBNull.Value : txtReason.Text.Trim());

                try
                {
                    insertCmd.ExecuteNonQuery();
                    Response.Redirect("~/Appointments/MyAppointments.aspx");
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
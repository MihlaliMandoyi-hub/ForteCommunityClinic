using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Appointments
{
    public partial class AppointmentCreate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

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

        protected void btnFindPatient_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT p.PatientID, p.PatientNumber + ' - ' + u.FirstName + ' ' + u.Surname AS PatientDisplay
                    FROM Patients p JOIN Users u ON p.UserID = u.UserID
                    WHERE p.IsActive = 1
                      AND (p.PatientNumber LIKE '%' + @Search + '%' OR u.Surname LIKE '%' + @Search + '%')", conn);
                cmd.Parameters.AddWithValue("@Search", txtPatientSearch.Text.Trim());

                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlPatient.DataSource = dt;
                ddlPatient.DataTextField = "PatientDisplay";
                ddlPatient.DataValueField = "PatientID";
                ddlPatient.DataBind();
            }
        }

        protected void cvPatient_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = ddlPatient.Items.Count > 0;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int doctorId = int.Parse(ddlDoctor.SelectedValue);
            int patientId = int.Parse(ddlPatient.SelectedValue);
            DateTime appointmentDate = DateTime.Parse(txtDate.Text);
            TimeSpan appointmentTime = TimeSpan.Parse(txtTime.Text);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // ===== THE DOUBLE-BOOKING CHECK (server-side, runs before any insert) =====
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

                int conflictCount = (int)checkCmd.ExecuteScalar();

                if (conflictCount > 0)
                {
                    lblMessage.Text = "This doctor already has an appointment at that date and time. Please choose a different slot.";
                    lblMessage.Visible = true;
                    return;
                }
                // ===== END CHECK =====

                var insertCmd = new SqlCommand(@"
                    INSERT INTO Appointments (PatientID, DoctorID, AppointmentDate, AppointmentTime, Reason, CreatedBy)
                    VALUES (@PatientID, @DoctorID, @AppointmentDate, @AppointmentTime, @Reason, @CreatedBy)", conn);
                insertCmd.Parameters.AddWithValue("@PatientID", patientId);
                insertCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                insertCmd.Parameters.AddWithValue("@AppointmentDate", appointmentDate.Date);
                insertCmd.Parameters.AddWithValue("@AppointmentTime", appointmentTime);
                insertCmd.Parameters.AddWithValue("@Reason",
                    string.IsNullOrWhiteSpace(txtReason.Text) ? DBNull.Value : (object)txtReason.Text.Trim());
                insertCmd.Parameters.AddWithValue("@CreatedBy", Session["UserID"]);

                try
                {
                    insertCmd.ExecuteNonQuery();
                    AuditHelper.Log(conn, (int?)Session["UserID"], "CREATE APPOINTMENT", "Appointment", null, "Appointment booked.");
                    Response.Redirect("~/Appointments/AppointmentList.aspx");
                }
                catch (SqlException)
                {
                    // Backstop: if two receptionists somehow submit the exact same slot
                    // at the exact same instant, the filtered unique index (Phase 3) catches it here.
                    lblMessage.Text = "This slot was just booked by someone else. Please choose a different time.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
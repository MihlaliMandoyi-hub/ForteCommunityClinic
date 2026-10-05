using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Consultations
{
    public partial class ConsultationCreate : System.Web.UI.Page
    {
        private int appointmentId;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Doctor", "Admin");

            if (!int.TryParse(Request.QueryString["appointmentId"], out appointmentId))
            {
                Response.Redirect("~/Appointments/AppointmentList.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPatientInfo();
            }
        }

        private void LoadPatientInfo()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT u.FirstName + ' ' + u.Surname + ' (' + p.PatientNumber + ')' AS PatientInfo
                    FROM Appointments a
                    JOIN Patients p ON a.PatientID = p.PatientID
                    JOIN Users u ON p.UserID = u.UserID
                    WHERE a.AppointmentID = @AppointmentID", conn);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    Response.Redirect("~/Appointments/AppointmentList.aspx");
                    return;
                }
                lblPatientInfo.Text = "Patient: " + result;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    var getPatientDoctorCmd = new SqlCommand(
                        "SELECT PatientID, DoctorID FROM Appointments WHERE AppointmentID = @AppointmentID",
                        conn, transaction);
                    getPatientDoctorCmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                    var reader = getPatientDoctorCmd.ExecuteReader();
                    reader.Read();
                    int patientId = (int)reader["PatientID"];
                    int doctorId = (int)reader["DoctorID"];
                    reader.Close();

                    var insertConsultCmd = new SqlCommand(@"
                        INSERT INTO Consultations (AppointmentID, DoctorID, PatientID, Symptoms, ClinicalNotes)
                        OUTPUT INSERTED.ConsultationID
                        VALUES (@AppointmentID, @DoctorID, @PatientID, @Symptoms, @ClinicalNotes)",
                        conn, transaction);
                    insertConsultCmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                    insertConsultCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    insertConsultCmd.Parameters.AddWithValue("@PatientID", patientId);
                    insertConsultCmd.Parameters.AddWithValue("@Symptoms", txtSymptoms.Text.Trim());
                    insertConsultCmd.Parameters.AddWithValue("@ClinicalNotes",
                        string.IsNullOrWhiteSpace(txtClinicalNotes.Text) ? (object)DBNull.Value : txtClinicalNotes.Text.Trim());

                    int consultationId = (int)insertConsultCmd.ExecuteScalar();

                    string[] diagnosisLines = txtDiagnoses.Text.Split(
                        new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string diagnosisText in diagnosisLines)
                    {
                        var insertDiagCmd = new SqlCommand(@"
                            INSERT INTO Diagnoses (ConsultationID, DiagnosisDescription)
                            VALUES (@ConsultationID, @DiagnosisDescription)", conn, transaction);
                        insertDiagCmd.Parameters.AddWithValue("@ConsultationID", consultationId);
                        insertDiagCmd.Parameters.AddWithValue("@DiagnosisDescription", diagnosisText.Trim());
                        insertDiagCmd.ExecuteNonQuery();
                    }

                    var updateApptCmd = new SqlCommand(@"
                        UPDATE Appointments
                        SET AppointmentStatusID = (SELECT AppointmentStatusID FROM AppointmentStatuses WHERE StatusName = 'In Consultation')
                        WHERE AppointmentID = @AppointmentID", conn, transaction);
                    updateApptCmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                    updateApptCmd.ExecuteNonQuery();

                    transaction.Commit();

                    using (var auditConn = DatabaseHelper.GetConnection())
                    {
                        auditConn.Open();
                        AuditHelper.Log(auditConn, (int?)Session["UserID"], "CREATE CONSULTATION", "Consultation", consultationId, "Consultation recorded.");
                    }

                    Response.Redirect("ConsultationDetails.aspx?id=" + consultationId);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    lblMessage.Text = "An error occurred while saving the consultation. Please try again.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
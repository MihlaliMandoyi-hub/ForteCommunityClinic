using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Consultations
{
    public partial class ConsultationDetails : System.Web.UI.Page
    {
        private int consultationId;
        private int appointmentId;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Doctor", "Admin");

            if (!int.TryParse(Request.QueryString["id"], out consultationId))
            {
                Response.Redirect("~/Appointments/AppointmentList.aspx");
                return;
            }

            lnkAddPrescription.HRef = "PrescriptionCreate.aspx?consultationId=" + consultationId;

            if (!IsPostBack)
            {
                LoadConsultation();
                LoadDiagnoses();
            }
        }

        private void LoadConsultation()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT c.AppointmentID, u1.FirstName + ' ' + u1.Surname AS PatientName,
                           u2.FirstName + ' ' + u2.Surname AS DoctorName,
                           c.ConsultationDate, c.Symptoms, c.ClinicalNotes, c.Status
                    FROM Consultations c
                    JOIN Patients p ON c.PatientID = p.PatientID
                    JOIN Users u1 ON p.UserID = u1.UserID
                    JOIN Doctors d ON c.DoctorID = d.DoctorID
                    JOIN Users u2 ON d.UserID = u2.UserID
                    WHERE c.ConsultationID = @ConsultationID", conn);
                cmd.Parameters.AddWithValue("@ConsultationID", consultationId);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    Response.Redirect("~/Appointments/AppointmentList.aspx");
                    return;
                }

                appointmentId = (int)reader["AppointmentID"];
                litPatient.Text = reader["PatientName"].ToString();
                litDoctor.Text = reader["DoctorName"].ToString();
                litDate.Text = Convert.ToDateTime(reader["ConsultationDate"]).ToString("yyyy-MM-dd HH:mm");
                litSymptoms.Text = reader["Symptoms"].ToString();
                litNotes.Text = reader["ClinicalNotes"] == DBNull.Value ? "-" : reader["ClinicalNotes"].ToString();
                litStatus.Text = reader["Status"].ToString();

                btnComplete.Visible = reader["Status"].ToString() != "Completed";
            }
        }

        private void LoadDiagnoses()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(
                    "SELECT DiagnosisDescription FROM Diagnoses WHERE ConsultationID = @ConsultationID", conn);
                cmd.Parameters.AddWithValue("@ConsultationID", consultationId);

                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new System.Data.DataTable();
                dt.Load(reader);

                rptDiagnoses.DataSource = dt;
                rptDiagnoses.DataBind();
            }
        }

        protected void btnComplete_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    var updateConsultCmd = new SqlCommand(
                        "UPDATE Consultations SET Status = 'Completed' WHERE ConsultationID = @ConsultationID",
                        conn, transaction);
                    updateConsultCmd.Parameters.AddWithValue("@ConsultationID", consultationId);
                    updateConsultCmd.ExecuteNonQuery();

                    var updateApptCmd = new SqlCommand(@"
                        UPDATE Appointments
                        SET AppointmentStatusID = (SELECT AppointmentStatusID FROM AppointmentStatuses WHERE StatusName = 'Completed')
                        WHERE AppointmentID = @AppointmentID", conn, transaction);
                    updateApptCmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                    updateApptCmd.ExecuteNonQuery();

                    transaction.Commit();
                    LoadConsultation();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
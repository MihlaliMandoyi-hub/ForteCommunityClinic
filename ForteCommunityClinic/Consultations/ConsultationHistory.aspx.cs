using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Consultations
{
    public partial class ConsultationHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Doctor", "Admin");

            int patientId;
            if (!int.TryParse(Request.QueryString["patientId"], out patientId))
            {
                Response.Redirect("~/Patients/PatientList.aspx");
                return;
            }

            LoadHistory(patientId);
        }

        private void LoadHistory(int patientId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var infoCmd = new SqlCommand(@"
                    SELECT u.FirstName + ' ' + u.Surname + ' (' + p.PatientNumber + ')' AS PatientInfo
                    FROM Patients p JOIN Users u ON p.UserID = u.UserID
                    WHERE p.PatientID = @PatientID", conn);
                infoCmd.Parameters.AddWithValue("@PatientID", patientId);

                conn.Open();
                object info = infoCmd.ExecuteScalar();
                lblPatientInfo.Text = "Patient: " + (info ?? "Unknown");

                var cmd = new SqlCommand(@"
                    SELECT c.ConsultationID, c.ConsultationDate, u.FirstName + ' ' + u.Surname AS DoctorName,
                           c.Symptoms, c.Status
                    FROM Consultations c
                    JOIN Doctors d ON c.DoctorID = d.DoctorID
                    JOIN Users u ON d.UserID = u.UserID
                    WHERE c.PatientID = @PatientID
                    ORDER BY c.ConsultationDate DESC", conn);
                cmd.Parameters.AddWithValue("@PatientID", patientId);

                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvHistory.DataSource = dt;
                gvHistory.DataBind();
            }
        }
    }
}
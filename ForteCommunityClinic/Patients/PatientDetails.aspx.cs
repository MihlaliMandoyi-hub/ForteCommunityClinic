using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Patients
{
    public partial class PatientDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Doctor", "Nurse", "Admin");

            int patientId;
            if (!int.TryParse(Request.QueryString["id"], out patientId))
            {
                ShowNotFound();
                return;
            }

            LoadPatient(patientId);
        }

        private void LoadPatient(int patientId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT
                        p.PatientNumber, u.FirstName, u.Surname, p.DateOfBirth, g.GenderName,
                        u.Email, COALESCE(u.PhoneNumber, p.EmergencyContactPhone) AS Phone,
                        p.IDNumber, p.Address, pr.ProvinceName,
                        p.EmergencyContactName, p.EmergencyContactPhone, p.RegisteredDate
                    FROM Patients p
                    LEFT JOIN Users u ON p.UserID = u.UserID
                    LEFT JOIN Gender g ON p.GenderID = g.GenderID
                    LEFT JOIN Provinces pr ON p.ProvinceID = pr.ProvinceID
                    WHERE p.PatientID = @PatientID", conn);
                cmd.Parameters.AddWithValue("@PatientID", patientId);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    ShowNotFound();
                    return;
                }

                litPatientNumber.Text = reader["PatientNumber"].ToString();
                litFullName.Text = reader["FirstName"] + " " + reader["Surname"];
                litDOB.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd");
                litGender.Text = reader["GenderName"].ToString();
                litEmail.Text = reader["Email"] == DBNull.Value ? "-" : reader["Email"].ToString();
                litPhone.Text = reader["Phone"] == DBNull.Value ? "-" : reader["Phone"].ToString();
                litIDNumber.Text = reader["IDNumber"] == DBNull.Value ? "-" : reader["IDNumber"].ToString();
                litAddress.Text = reader["Address"] == DBNull.Value ? "-" : reader["Address"].ToString();
                litProvince.Text = reader["ProvinceName"] == DBNull.Value ? "-" : reader["ProvinceName"].ToString();
                litEmergency.Text = (reader["EmergencyContactName"] == DBNull.Value ? "-" : reader["EmergencyContactName"].ToString())
                                     + " (" + (reader["EmergencyContactPhone"] == DBNull.Value ? "-" : reader["EmergencyContactPhone"].ToString()) + ")";
                litRegistered.Text = Convert.ToDateTime(reader["RegisteredDate"]).ToString("yyyy-MM-dd");

                lnkEdit.NavigateUrl = "PatientEdit.aspx?id=" + patientId;
            }
        }

        private void ShowNotFound()
        {
            lblNotFound.Visible = true;
            pnlDetails.Visible = false;
        }
    }
}
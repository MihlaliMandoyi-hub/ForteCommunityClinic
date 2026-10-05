using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Patients
{
    public partial class PatientEdit : System.Web.UI.Page
    {
        private int patientId;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

            if (!int.TryParse(Request.QueryString["id"], out patientId))
            {
                Response.Redirect("~/Patients/PatientList.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPatient();
            }
        }

        private void LoadPatient()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT u.FirstName, u.Surname, u.Email, u.PhoneNumber, p.Address,
                           p.EmergencyContactName, p.EmergencyContactPhone
                    FROM Patients p
                    LEFT JOIN Users u ON p.UserID = u.UserID
                    WHERE p.PatientID = @PatientID", conn);
                cmd.Parameters.AddWithValue("@PatientID", patientId);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    Response.Redirect("~/Patients/PatientList.aspx");
                    return;
                }

                txtFirstName.Text = reader["FirstName"].ToString();
                txtSurname.Text = reader["Surname"].ToString();
                txtEmail.Text = reader["Email"] == DBNull.Value ? "" : reader["Email"].ToString();
                txtPhone.Text = reader["PhoneNumber"] == DBNull.Value ? "" : reader["PhoneNumber"].ToString();
                txtAddress.Text = reader["Address"] == DBNull.Value ? "" : reader["Address"].ToString();
                txtEmergencyName.Text = reader["EmergencyContactName"] == DBNull.Value ? "" : reader["EmergencyContactName"].ToString();
                txtEmergencyPhone.Text = reader["EmergencyContactPhone"] == DBNull.Value ? "" : reader["EmergencyContactPhone"].ToString();
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
                    var updateUserCmd = new SqlCommand(@"
                        UPDATE Users SET FirstName = @FirstName, Surname = @Surname, Email = @Email, PhoneNumber = @PhoneNumber
                        WHERE UserID = (SELECT UserID FROM Patients WHERE PatientID = @PatientID)",
                        conn, transaction);
                    updateUserCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                    updateUserCmd.Parameters.AddWithValue("@Surname", txtSurname.Text.Trim());
                    updateUserCmd.Parameters.AddWithValue("@Email",
                        string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : (object)txtEmail.Text.Trim());
                    updateUserCmd.Parameters.AddWithValue("@PhoneNumber",
                        string.IsNullOrWhiteSpace(txtPhone.Text) ? DBNull.Value : (object)txtPhone.Text.Trim());
                    updateUserCmd.Parameters.AddWithValue("@PatientID", patientId);
                    updateUserCmd.ExecuteNonQuery();

                    var updatePatientCmd = new SqlCommand(@"
                        UPDATE Patients SET Address = @Address, EmergencyContactName = @EmergencyContactName,
                               EmergencyContactPhone = @EmergencyContactPhone
                        WHERE PatientID = @PatientID",
                        conn, transaction);
                    updatePatientCmd.Parameters.AddWithValue("@Address",
                        string.IsNullOrWhiteSpace(txtAddress.Text) ? DBNull.Value : (object)txtAddress.Text.Trim());
                    updatePatientCmd.Parameters.AddWithValue("@EmergencyContactName",
                        string.IsNullOrWhiteSpace(txtEmergencyName.Text) ? DBNull.Value : (object)txtEmergencyName.Text.Trim());
                    updatePatientCmd.Parameters.AddWithValue("@EmergencyContactPhone",
                        string.IsNullOrWhiteSpace(txtEmergencyPhone.Text) ? DBNull.Value : (object)txtEmergencyPhone.Text.Trim());
                    updatePatientCmd.Parameters.AddWithValue("@PatientID", patientId);
                    updatePatientCmd.ExecuteNonQuery();

                    transaction.Commit();

                    using (var auditConn = DatabaseHelper.GetConnection())
                    {
                        auditConn.Open();
                        AuditHelper.Log(auditConn, (int?)Session["UserID"], "UPDATE PATIENT", "Patient", patientId, "Patient details updated.");
                    }

                    Response.Redirect("PatientDetails.aspx?id=" + patientId);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    lblMessage.Text = "An error occurred while saving changes. Please try again.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
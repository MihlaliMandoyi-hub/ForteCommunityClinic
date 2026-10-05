using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Account
{
    public partial class MyProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Patient");

            if (!IsPostBack)
            {
                LoadProfile();
            }
        }

        private void LoadProfile()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT u.FirstName, u.Surname, u.Email, u.PhoneNumber, p.Address,
                           p.EmergencyContactName, p.EmergencyContactPhone
                    FROM Patients p
                    JOIN Users u ON p.UserID = u.UserID
                    WHERE p.UserID = @UserID", conn);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                conn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read()) return;

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
                        WHERE UserID = @UserID", conn, transaction);
                    updateUserCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                    updateUserCmd.Parameters.AddWithValue("@Surname", txtSurname.Text.Trim());
                    updateUserCmd.Parameters.AddWithValue("@Email",
                        string.IsNullOrWhiteSpace(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text.Trim());
                    updateUserCmd.Parameters.AddWithValue("@PhoneNumber",
                        string.IsNullOrWhiteSpace(txtPhone.Text) ? (object)DBNull.Value : txtPhone.Text.Trim());
                    updateUserCmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                    updateUserCmd.ExecuteNonQuery();

                    var updatePatientCmd = new SqlCommand(@"
                        UPDATE Patients SET Address = @Address, EmergencyContactName = @EmergencyContactName,
                               EmergencyContactPhone = @EmergencyContactPhone
                        WHERE UserID = @UserID", conn, transaction);
                    updatePatientCmd.Parameters.AddWithValue("@Address",
                        string.IsNullOrWhiteSpace(txtAddress.Text) ? (object)DBNull.Value : txtAddress.Text.Trim());
                    updatePatientCmd.Parameters.AddWithValue("@EmergencyContactName",
                        string.IsNullOrWhiteSpace(txtEmergencyName.Text) ? (object)DBNull.Value : txtEmergencyName.Text.Trim());
                    updatePatientCmd.Parameters.AddWithValue("@EmergencyContactPhone",
                        string.IsNullOrWhiteSpace(txtEmergencyPhone.Text) ? (object)DBNull.Value : txtEmergencyPhone.Text.Trim());
                    updatePatientCmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                    updatePatientCmd.ExecuteNonQuery();

                    AuditHelper.Log(conn, transaction, (int?)Session["UserID"], "UPDATE PATIENT", "Patient", null, "Patient updated their own contact information.");

                    transaction.Commit();
                    lblMessage.Visible = true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
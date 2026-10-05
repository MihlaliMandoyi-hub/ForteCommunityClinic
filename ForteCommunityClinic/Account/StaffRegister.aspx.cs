using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Account
{
    public partial class StaffRegister : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Public page — no login required to reach it
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string role = ddlRole.SelectedValue; // "Doctor" or "Nurse"

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    var checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email = @Email",
                        conn, transaction);
                    checkCmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        lblMessage.Text = "That username or email is already registered.";
                        lblMessage.Visible = true;
                        transaction.Rollback();
                        return;
                    }

                    string salt = PasswordHelper.GenerateSalt();
                    string hash = PasswordHelper.HashPassword(txtPassword.Text, salt);

                    // IsActive = 0 — account exists but cannot log in until a receptionist approves it
                    var insertUserCmd = new SqlCommand(@"
                        INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, FirstName, Surname, IsActive)
                        OUTPUT INSERTED.UserID
                        VALUES (@Username, @Email, @PasswordHash, @PasswordSalt, @FirstName, @Surname, 0)",
                        conn, transaction);
                    insertUserCmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    insertUserCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    insertUserCmd.Parameters.AddWithValue("@PasswordHash", hash);
                    insertUserCmd.Parameters.AddWithValue("@PasswordSalt", salt);
                    insertUserCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                    insertUserCmd.Parameters.AddWithValue("@Surname", txtSurname.Text.Trim());

                    int newUserId = (int)insertUserCmd.ExecuteScalar();

                    var roleCmd = new SqlCommand(
                        "INSERT INTO UserRoles (UserID, RoleID) SELECT @UserID, RoleID FROM Roles WHERE RoleName = @RoleName",
                        conn, transaction);
                    roleCmd.Parameters.AddWithValue("@UserID", newUserId);
                    roleCmd.Parameters.AddWithValue("@RoleName", role);
                    roleCmd.ExecuteNonQuery();

                    if (role == "Doctor")
                    {
                        var insertDoctorCmd = new SqlCommand(@"
                            INSERT INTO Doctors (UserID, Specialization, IsActive)
                            VALUES (@UserID, @Specialization, 0)", conn, transaction);
                        insertDoctorCmd.Parameters.AddWithValue("@UserID", newUserId);
                        insertDoctorCmd.Parameters.AddWithValue("@Specialization",
                            string.IsNullOrWhiteSpace(txtSpecialization.Text) ? (object)DBNull.Value : txtSpecialization.Text.Trim());
                        insertDoctorCmd.ExecuteNonQuery();
                    }
                    else // Nurse
                    {
                        var insertNurseCmd = new SqlCommand(
                            "INSERT INTO Nurses (UserID, IsActive) VALUES (@UserID, 0)", conn, transaction);
                        insertNurseCmd.Parameters.AddWithValue("@UserID", newUserId);
                        insertNurseCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    using (var auditConn = DatabaseHelper.GetConnection())
                    {
                        auditConn.Open();
                        AuditHelper.Log(auditConn, newUserId, "CREATE USER", "User", newUserId, $"{role} self-registered, pending approval.");
                    }


                    pnlForm.Visible = false;
                    pnlSuccess.Visible = true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    lblMessage.Text = "An error occurred while registering. Please try again.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
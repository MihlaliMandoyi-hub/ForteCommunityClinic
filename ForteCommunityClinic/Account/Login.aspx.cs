using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["registered"] == "1")
            {
                lblRegistered.Visible = true;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT UserID, PasswordHash, PasswordSalt, FirstName, Surname, IsActive
                    FROM Users
                    WHERE Username = @Username",
                    conn);
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    ShowError();
                    return;
                }

                bool isActive = (bool)reader["IsActive"];
                string storedHash = reader["PasswordHash"].ToString();
                string storedSalt = reader["PasswordSalt"].ToString();
                int userId = (int)reader["UserID"];
                string fullName = reader["FirstName"] + " " + reader["Surname"];
                reader.Close();

                if (!isActive || !PasswordHelper.VerifyPassword(txtPassword.Text, storedHash, storedSalt))
                {
                    ShowError();
                    return;
                }

                // Get the user's primary role (first one found — most users will only have one)
                var roleCmd = new SqlCommand(@"
                    SELECT TOP 1 r.RoleName
                    FROM UserRoles ur
                    JOIN Roles r ON ur.RoleID = r.RoleID
                    WHERE ur.UserID = @UserID",
                    conn);
                roleCmd.Parameters.AddWithValue("@UserID", userId);
                string role = (string)roleCmd.ExecuteScalar();

                Session["UserID"] = userId;
                Session["FullName"] = fullName;
                Session["Role"] = role;
                AuditHelper.Log(conn, userId, "LOGIN", "User", userId, "User logged in.");

                // Update LastLoginDate
                var updateCmd = new SqlCommand(
                    "UPDATE Users SET LastLoginDate = GETDATE() WHERE UserID = @UserID", conn);
                updateCmd.Parameters.AddWithValue("@UserID", userId);
                updateCmd.ExecuteNonQuery();

                Response.Redirect("~/Dashboard.aspx");
            }
        }

        private void ShowError()
        {
            lblMessage.Text = "Invalid username or password.";
            lblMessage.Visible = true;
        }
    }
}
using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Account
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGenders();
            }
        }

        private void LoadGenders()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand("SELECT GenderID, GenderName FROM Gender", conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlGender.DataSource = dt;
                ddlGender.DataTextField = "GenderName";
                ddlGender.DataValueField = "GenderID";
                ddlGender.DataBind();
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Check username/email aren't already taken
                    var checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email = @Email",
                        conn, transaction);
                    checkCmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    checkCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

                    int existing = (int)checkCmd.ExecuteScalar();
                    if (existing > 0)
                    {
                        lblMessage.Text = "That username or email is already registered.";
                        lblMessage.Visible = true;
                        transaction.Rollback();
                        return;
                    }

                    // 2. Create the Users row with a hashed password
                    string salt = PasswordHelper.GenerateSalt();
                    string hash = PasswordHelper.HashPassword(txtPassword.Text, salt);

                    var insertUserCmd = new SqlCommand(@"
                        INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, FirstName, Surname)
                        OUTPUT INSERTED.UserID
                        VALUES (@Username, @Email, @PasswordHash, @PasswordSalt, @FirstName, @Surname)",
                        conn, transaction);
                    insertUserCmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    insertUserCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    insertUserCmd.Parameters.AddWithValue("@PasswordHash", hash);
                    insertUserCmd.Parameters.AddWithValue("@PasswordSalt", salt);
                    insertUserCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                    insertUserCmd.Parameters.AddWithValue("@Surname", txtSurname.Text.Trim());

                    int newUserId = (int)insertUserCmd.ExecuteScalar();

                    // 3. Assign the "Patient" role
                    var roleCmd = new SqlCommand(
                        "INSERT INTO UserRoles (UserID, RoleID) SELECT @UserID, RoleID FROM Roles WHERE RoleName = 'Patient'",
                        conn, transaction);
                    roleCmd.Parameters.AddWithValue("@UserID", newUserId);
                    roleCmd.ExecuteNonQuery();

                    // 4. Create the Patients row, generating a simple patient number
                    string patientNumber = "P" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    var insertPatientCmd = new SqlCommand(@"
                        INSERT INTO Patients (UserID, PatientNumber, DateOfBirth, GenderID)
                        VALUES (@UserID, @PatientNumber, @DateOfBirth, @GenderID)",
                        conn, transaction);
                    insertPatientCmd.Parameters.AddWithValue("@UserID", newUserId);
                    insertPatientCmd.Parameters.AddWithValue("@PatientNumber", patientNumber);
                    insertPatientCmd.Parameters.AddWithValue("@DateOfBirth", DateTime.Parse(txtDateOfBirth.Text));
                    insertPatientCmd.Parameters.AddWithValue("@GenderID", int.Parse(ddlGender.SelectedValue));
                    insertPatientCmd.ExecuteNonQuery();

                    transaction.Commit();

                    using (var auditConn = DatabaseHelper.GetConnection())
                    {
                        auditConn.Open();
                        AuditHelper.Log(auditConn, newUserId, "CREATE PATIENT", "Patient", newUserId, "Patient self-registered.");
                    }

                    Response.Redirect("~/Account/Login.aspx?registered=1");
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
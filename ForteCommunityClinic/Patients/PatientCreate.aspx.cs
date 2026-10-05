using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Patients
{
    public partial class PatientCreate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

            if (!IsPostBack)
            {
                LoadLookups();
            }
        }

        private void LoadLookups()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                BindDropDown(conn, "SELECT GenderID, GenderName FROM Gender", ddlGender, "GenderName", "GenderID");
                BindDropDown(conn, "SELECT ProvinceID, ProvinceName FROM Provinces", ddlProvince, "ProvinceName", "ProvinceID");
            }
        }

        private void BindDropDown(SqlConnection conn, string sql, System.Web.UI.WebControls.DropDownList ddl, string textField, string valueField)
        {
            var cmd = new SqlCommand(sql, conn);
            var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            ddl.DataSource = dt;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
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
                    // Generate a system username/password the patient can't actually log in with yet
                    // (an Admin/Receptionist can reset it later if the patient wants online access)
                    string generatedUsername = "patient_" + DateTime.Now.Ticks;
                    string salt = PasswordHelper.GenerateSalt();
                    string randomPassword = Guid.NewGuid().ToString();
                    string hash = PasswordHelper.HashPassword(randomPassword, salt);

                    var insertUserCmd = new SqlCommand(@"
                        INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, FirstName, Surname, PhoneNumber, IsActive)
                        OUTPUT INSERTED.UserID
                        VALUES (@Username, @Email, @PasswordHash, @PasswordSalt, @FirstName, @Surname, @PhoneNumber, 0)",
                        conn, transaction);
                    insertUserCmd.Parameters.AddWithValue("@Username", generatedUsername);
                    insertUserCmd.Parameters.AddWithValue("@Email",
                        string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : (object)txtEmail.Text.Trim());
                    insertUserCmd.Parameters.AddWithValue("@PasswordHash", hash);
                    insertUserCmd.Parameters.AddWithValue("@PasswordSalt", salt);
                    insertUserCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                    insertUserCmd.Parameters.AddWithValue("@Surname", txtSurname.Text.Trim());
                    insertUserCmd.Parameters.AddWithValue("@PhoneNumber",
                        string.IsNullOrWhiteSpace(txtPhone.Text) ? DBNull.Value : (object)txtPhone.Text.Trim());

                    int newUserId = (int)insertUserCmd.ExecuteScalar();

                    var roleCmd = new SqlCommand(
                        "INSERT INTO UserRoles (UserID, RoleID) SELECT @UserID, RoleID FROM Roles WHERE RoleName = 'Patient'",
                        conn, transaction);
                    roleCmd.Parameters.AddWithValue("@UserID", newUserId);
                    roleCmd.ExecuteNonQuery();

                    string patientNumber = "P" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    var insertPatientCmd = new SqlCommand(@"
                        INSERT INTO Patients
                            (UserID, PatientNumber, DateOfBirth, GenderID, IDNumber, Address, ProvinceID,
                             EmergencyContactName, EmergencyContactPhone)
                        VALUES
                            (@UserID, @PatientNumber, @DateOfBirth, @GenderID, @IDNumber, @Address, @ProvinceID,
                             @EmergencyContactName, @EmergencyContactPhone)",
                        conn, transaction);
                    insertPatientCmd.Parameters.AddWithValue("@UserID", newUserId);
                    insertPatientCmd.Parameters.AddWithValue("@PatientNumber", patientNumber);
                    insertPatientCmd.Parameters.AddWithValue("@DateOfBirth", DateTime.Parse(txtDateOfBirth.Text));
                    insertPatientCmd.Parameters.AddWithValue("@GenderID", int.Parse(ddlGender.SelectedValue));
                    insertPatientCmd.Parameters.AddWithValue("@IDNumber",
                        string.IsNullOrWhiteSpace(txtIDNumber.Text) ? DBNull.Value : (object)txtIDNumber.Text.Trim());
                    insertPatientCmd.Parameters.AddWithValue("@Address",
                        string.IsNullOrWhiteSpace(txtAddress.Text) ? DBNull.Value : (object)txtAddress.Text.Trim());
                    insertPatientCmd.Parameters.AddWithValue("@ProvinceID",
                        ddlProvince.SelectedValue == "" ? DBNull.Value : (object)int.Parse(ddlProvince.SelectedValue));
                    insertPatientCmd.Parameters.AddWithValue("@EmergencyContactName",
                        string.IsNullOrWhiteSpace(txtEmergencyName.Text) ? DBNull.Value : (object)txtEmergencyName.Text.Trim());
                    insertPatientCmd.Parameters.AddWithValue("@EmergencyContactPhone",
                        string.IsNullOrWhiteSpace(txtEmergencyPhone.Text) ? DBNull.Value : (object)txtEmergencyPhone.Text.Trim());

                    insertPatientCmd.ExecuteNonQuery();

                    transaction.Commit();

                    using (var auditConn = DatabaseHelper.GetConnection())
                    {
                        auditConn.Open();
                        AuditHelper.Log(auditConn, (int?)Session["UserID"], "CREATE PATIENT", "Patient", newUserId, "Receptionist registered a walk-in patient.");
                    }

                    Response.Redirect("~/Patients/PatientList.aspx");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    lblMessage.Text = "An error occurred while saving the patient. Please try again.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
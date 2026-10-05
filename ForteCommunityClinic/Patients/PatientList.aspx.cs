using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Patients
{
    public partial class PatientList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Doctor", "Admin");

            if (!IsPostBack)
            {
                BindPatients(null, null, null, null);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindPatients(
                txtSearchPatientNumber.Text.Trim(),
                txtSearchFirstName.Text.Trim(),
                txtSearchSurname.Text.Trim(),
                txtSearchPhone.Text.Trim());
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearchPatientNumber.Text = "";
            txtSearchFirstName.Text = "";
            txtSearchSurname.Text = "";
            txtSearchPhone.Text = "";
            BindPatients(null, null, null, null);
        }

        private void BindPatients(string patientNumber, string firstName, string surname, string phone)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT
                        p.PatientID,
                        p.PatientNumber,
                        u.FirstName + ' ' + u.Surname AS FullName,
                        p.DateOfBirth,
                        COALESCE(u.PhoneNumber, p.EmergencyContactPhone) AS PhoneNumber
                    FROM Patients p
                    LEFT JOIN Users u ON p.UserID = u.UserID
                    WHERE p.IsActive = 1
                      AND (@PatientNumber = '' OR p.PatientNumber LIKE '%' + @PatientNumber + '%')
                      AND (@FirstName = '' OR u.FirstName LIKE '%' + @FirstName + '%')
                      AND (@Surname = '' OR u.Surname LIKE '%' + @Surname + '%')
                      AND (@Phone = '' OR COALESCE(u.PhoneNumber, p.EmergencyContactPhone) LIKE '%' + @Phone + '%')
                    ORDER BY p.RegisteredDate DESC", conn);

                cmd.Parameters.AddWithValue("@PatientNumber", patientNumber ?? "");
                cmd.Parameters.AddWithValue("@FirstName", firstName ?? "");
                cmd.Parameters.AddWithValue("@Surname", surname ?? "");
                cmd.Parameters.AddWithValue("@Phone", phone ?? "");

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvPatients.DataSource = dt;
                gvPatients.DataBind();
            }
        }
    }
}
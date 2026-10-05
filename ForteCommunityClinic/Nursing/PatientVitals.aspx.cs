using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Nursing
{
    public partial class PatientVitals : System.Web.UI.Page
    {
        private int appointmentId;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Nurse", "Admin");

            if (!int.TryParse(Request.QueryString["appointmentId"], out appointmentId))
            {
                Response.Redirect("~/Nursing/TodaysPatients.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPatientInfo();
            }
        }

        private void LoadPatientInfo()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT u.FirstName + ' ' + u.Surname + ' (' + p.PatientNumber + ')' AS PatientInfo
                    FROM Appointments a
                    JOIN Patients p ON a.PatientID = p.PatientID
                    JOIN Users u ON p.UserID = u.UserID
                    WHERE a.AppointmentID = @AppointmentID", conn);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    Response.Redirect("~/Nursing/TodaysPatients.aspx");
                    return;
                }

                lblPatientInfo.Text = "Patient: " + result;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO PatientVitals
                        (AppointmentID, NurseID, Temperature, Weight, Height, BloodPressure, PulseRate, OxygenSaturation)
                    VALUES
                        (@AppointmentID, (SELECT NurseID FROM Nurses WHERE UserID = @UserID),
                         @Temperature, @Weight, @Height, @BloodPressure, @PulseRate, @OxygenSaturation)", conn);

                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                cmd.Parameters.AddWithValue("@Temperature",
                    string.IsNullOrWhiteSpace(txtTemperature.Text) ? (object)DBNull.Value : decimal.Parse(txtTemperature.Text));
                cmd.Parameters.AddWithValue("@Weight",
                    string.IsNullOrWhiteSpace(txtWeight.Text) ? (object)DBNull.Value : decimal.Parse(txtWeight.Text));
                cmd.Parameters.AddWithValue("@Height",
                    string.IsNullOrWhiteSpace(txtHeight.Text) ? (object)DBNull.Value : decimal.Parse(txtHeight.Text));
                cmd.Parameters.AddWithValue("@BloodPressure",
                    string.IsNullOrWhiteSpace(txtBloodPressure.Text) ? (object)DBNull.Value : (object)txtBloodPressure.Text.Trim());
                cmd.Parameters.AddWithValue("@PulseRate",
                    string.IsNullOrWhiteSpace(txtPulseRate.Text) ? (object)DBNull.Value : int.Parse(txtPulseRate.Text));
                cmd.Parameters.AddWithValue("@OxygenSaturation",
                    string.IsNullOrWhiteSpace(txtOxygenSaturation.Text) ? (object)DBNull.Value : int.Parse(txtOxygenSaturation.Text));

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Visible = true;
        }
    }
}
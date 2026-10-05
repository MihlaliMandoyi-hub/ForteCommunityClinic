using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Appointments
{
    public partial class AppointmentList : System.Web.UI.Page
    {
        protected bool IsDoctorView { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Doctor", "Nurse", "Admin");
            IsDoctorView = Session["Role"]?.ToString() == "Doctor";

            if (!IsPostBack)
            {
                LoadFilters();
                BindAppointments();
            }
        }

        private void LoadFilters()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                var doctorCmd = new SqlCommand(
                    "SELECT d.DoctorID, u.FirstName + ' ' + u.Surname AS DoctorName FROM Doctors d JOIN Users u ON d.UserID = u.UserID", conn);
                var doctorReader = doctorCmd.ExecuteReader();
                var doctorTable = new DataTable();
                doctorTable.Load(doctorReader);
                ddlFilterDoctor.DataSource = doctorTable;
                ddlFilterDoctor.DataTextField = "DoctorName";
                ddlFilterDoctor.DataValueField = "DoctorID";
                ddlFilterDoctor.DataBind();
                ddlFilterDoctor.Items.Insert(0, new ListItem("All Doctors", ""));

                var statusCmd = new SqlCommand("SELECT AppointmentStatusID, StatusName FROM AppointmentStatuses", conn);
                var statusReader = statusCmd.ExecuteReader();
                var statusTable = new DataTable();
                statusTable.Load(statusReader);
                ddlFilterStatus.DataSource = statusTable;
                ddlFilterStatus.DataTextField = "StatusName";
                ddlFilterStatus.DataValueField = "AppointmentStatusID";
                ddlFilterStatus.DataBind();
                ddlFilterStatus.Items.Insert(0, new ListItem("All Statuses", ""));
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e) => BindAppointments();

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtFilterDate.Text = "";
            ddlFilterDoctor.SelectedIndex = 0;
            ddlFilterStatus.SelectedIndex = 0;
            txtFilterPatient.Text = "";
            BindAppointments();
        }

        private void BindAppointments()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT a.AppointmentID, u1.FirstName + ' ' + u1.Surname AS PatientName,
                           u2.FirstName + ' ' + u2.Surname AS DoctorName,
                           a.AppointmentDate, a.AppointmentTime, s.StatusName
                    FROM Appointments a
                    JOIN Patients p ON a.PatientID = p.PatientID
                    JOIN Users u1 ON p.UserID = u1.UserID
                    JOIN Doctors d ON a.DoctorID = d.DoctorID
                    JOIN Users u2 ON d.UserID = u2.UserID
                    JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                    WHERE (@FilterDate = '' OR a.AppointmentDate = @FilterDateValue)
                      AND (@FilterDoctor = '' OR a.DoctorID = @FilterDoctorValue)
                      AND (@FilterStatus = '' OR a.AppointmentStatusID = @FilterStatusValue)
                      AND (@FilterPatient = '' OR u1.Surname LIKE '%' + @FilterPatient + '%' OR p.PatientNumber LIKE '%' + @FilterPatient + '%')
                      AND (@IsDoctorView = 0 OR d.UserID = @CurrentUserID)
                    ORDER BY a.AppointmentDate DESC, a.AppointmentTime", conn);

                cmd.Parameters.AddWithValue("@FilterDate", txtFilterDate.Text.Trim());
                cmd.Parameters.AddWithValue("@FilterDateValue", string.IsNullOrEmpty(txtFilterDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtFilterDate.Text));
                cmd.Parameters.AddWithValue("@FilterDoctor", ddlFilterDoctor.SelectedValue);
                cmd.Parameters.AddWithValue("@FilterDoctorValue", ddlFilterDoctor.SelectedValue == "" ? (object)DBNull.Value : int.Parse(ddlFilterDoctor.SelectedValue));
                cmd.Parameters.AddWithValue("@FilterStatus", ddlFilterStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@FilterStatusValue", ddlFilterStatus.SelectedValue == "" ? (object)DBNull.Value : int.Parse(ddlFilterStatus.SelectedValue));
                cmd.Parameters.AddWithValue("@FilterPatient", txtFilterPatient.Text.Trim());
                cmd.Parameters.AddWithValue("@IsDoctorView", IsDoctorView ? 1 : 0);
                cmd.Parameters.AddWithValue("@CurrentUserID", Session["UserID"] ?? DBNull.Value);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvAppointments.DataSource = dt;
                gvAppointments.DataBind();
            }
        }

        protected void gvAppointments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int appointmentId = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "StartConsultation")
            {
                Response.Redirect("~/Consultations/ConsultationCreate.aspx?appointmentId=" + appointmentId);
                return;
            }

            string newStatus = e.CommandName == "CheckIn" ? "Checked In" : "Cancelled";

            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    UPDATE Appointments
                    SET AppointmentStatusID = (SELECT AppointmentStatusID FROM AppointmentStatuses WHERE StatusName = @StatusName)
                    WHERE AppointmentID = @AppointmentID", conn);
                cmd.Parameters.AddWithValue("@StatusName", newStatus);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                conn.Open();
                cmd.ExecuteNonQuery();

                if (newStatus == "Cancelled")
                {
                    AuditHelper.Log(conn, (int?)Session["UserID"], "CANCEL APPOINTMENT", "Appointment", appointmentId, "Appointment cancelled by staff.");
                }
            }

            BindAppointments();
        }
    }
}
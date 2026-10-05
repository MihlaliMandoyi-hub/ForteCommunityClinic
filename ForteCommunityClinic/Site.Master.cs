using System;
using ForteCommunityClinic.DAL;
using System.Data.SqlClient;

namespace ForteCommunityClinic
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                lblUserInfo.Text = "Not logged in";
                return;
            }

            lblUserInfo.Text = Session["FullName"] + " (" + Session["Role"] + ")";

            string role = Session["Role"]?.ToString();

            phMyProfileNav.Visible = role == "Patient";
            phPatientsNav.Visible = role == "Receptionist" || role == "Doctor" || role == "Nurse" || role == "Admin";
            phAppointmentsNav.Visible = true;
            phNursingNav.Visible = role == "Nurse";
            phConsultationsNav.Visible = role == "Doctor";
            phPharmacyNav.Visible = role == "Pharmacist";
            phBillingNav.Visible = role == "Receptionist" || role == "Patient";
            phClaimsNav.Visible = role == "Receptionist";
            phReportsNav.Visible = role == "ClinicManager" || role == "Admin";
            phStaffApprovalsNav.Visible = role == "Receptionist" || role == "Admin";
            phAdminNav.Visible = role == "Admin";

            lnkAppointments.HRef = role == "Patient" ? "~/Appointments/MyAppointments.aspx" : "~/Appointments/AppointmentList.aspx";
            lnkBilling.HRef = role == "Patient" ? "~/Billing/MyBills.aspx" : "~/Billing/BillCreate.aspx";

            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Notifications WHERE UserID = @UserID AND IsRead = 0", conn);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                conn.Open();
                int unreadCount = (int)cmd.ExecuteScalar();
                lblUnreadCount.Text = unreadCount > 0 ? unreadCount.ToString() : "";
                lblUnreadCount.Visible = unreadCount > 0;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Account/Login.aspx");
        }
    }
}
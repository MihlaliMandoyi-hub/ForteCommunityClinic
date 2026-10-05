using System;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;

namespace ForteCommunityClinic
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool loggedIn = Session["UserID"] != null;

            // Login / Register pages: no sidebar, no user bar
            phSidebar.Visible = loggedIn;
            phUserBar.Visible = loggedIn;
            if (!loggedIn) return;

            string role = Session["Role"]?.ToString();

            // HtmlEncode so a name containing HTML can't inject markup into the page
            lblUserInfo.Text = Server.HtmlEncode(Session["FullName"] + " (" + role + ")");

            phMyProfileNav.Visible = role == "Patient";
            phPatientsNav.Visible = role == "Receptionist" || role == "Doctor" || role == "Admin";
            phAppointmentsNav.Visible = true;
            phTodaysPatientsNav.Visible = role == "Doctor";
            phPharmacyNav.Visible = role == "Pharmacist";
            phBillingNav.Visible = role == "Receptionist" || role == "Patient";
            phClaimsNav.Visible = role == "Receptionist";
            phReportsNav.Visible = role == "ClinicManager" || role == "Admin";
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
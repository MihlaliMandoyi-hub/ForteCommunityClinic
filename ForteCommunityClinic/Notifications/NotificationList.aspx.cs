using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Notifications
{
    public partial class NotificationList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireLogin(this);

            if (!IsPostBack)
            {
                BindNotifications();
            }
        }

        private void BindNotifications()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT NotificationID, Title, Message, IsRead, CreatedDate
                    FROM Notifications
                    WHERE UserID = @UserID
                    ORDER BY CreatedDate DESC", conn);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvNotifications.DataSource = dt;
                gvNotifications.DataBind();
            }
        }

        protected void gvNotifications_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "MarkRead") return;

            int notificationId = int.Parse(e.CommandArgument.ToString());

            using (var conn = DatabaseHelper.GetConnection())
            {
                // Ownership check — a user can only mark their OWN notifications as read
                var cmd = new SqlCommand(
                    "UPDATE Notifications SET IsRead = 1 WHERE NotificationID = @NotificationID AND UserID = @UserID", conn);
                cmd.Parameters.AddWithValue("@NotificationID", notificationId);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            BindNotifications();
        }
    }
}
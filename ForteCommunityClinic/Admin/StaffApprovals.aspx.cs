using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Admin
{
    public partial class StaffApprovals : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

            if (!IsPostBack)
            {
                BindPending();
            }
        }

        private void BindPending()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT u.UserID, u.FirstName + ' ' + u.Surname AS FullName, u.Email, r.RoleName, u.CreatedDate
                    FROM Users u
                    JOIN UserRoles ur ON u.UserID = ur.UserID
                    JOIN Roles r ON ur.RoleID = r.RoleID
                    WHERE u.IsActive = 0 AND r.RoleName IN ('Doctor', 'Nurse')
                    ORDER BY u.CreatedDate", conn);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvPending.DataSource = dt;
                gvPending.DataBind();
            }
        }

        protected void gvPending_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Approve") return;

            int userId = int.Parse(e.CommandArgument.ToString());

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    var activateUserCmd = new SqlCommand(
                        "UPDATE Users SET IsActive = 1 WHERE UserID = @UserID", conn, transaction);
                    activateUserCmd.Parameters.AddWithValue("@UserID", userId);
                    activateUserCmd.ExecuteNonQuery();

                    var activateDoctorCmd = new SqlCommand(
                        "UPDATE Doctors SET IsActive = 1 WHERE UserID = @UserID", conn, transaction);
                    activateDoctorCmd.Parameters.AddWithValue("@UserID", userId);
                    activateDoctorCmd.ExecuteNonQuery();

                    var activateNurseCmd = new SqlCommand(
                        "UPDATE Nurses SET IsActive = 1 WHERE UserID = @UserID", conn, transaction);
                    activateNurseCmd.Parameters.AddWithValue("@UserID", userId);
                    activateNurseCmd.ExecuteNonQuery();

                    NotificationHelper.Create(conn, transaction, userId,
                        "Account Approved", "Your staff account has been approved. You can now log in.", "AccountApproval");
                    AuditHelper.Log(conn, transaction, (int?)Session["UserID"], "UPDATE USER", "User", userId, "Staff account approved and activated.");
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }

            BindPending();
        }
    }
}
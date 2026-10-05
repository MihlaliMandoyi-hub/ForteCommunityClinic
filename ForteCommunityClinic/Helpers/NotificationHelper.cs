using System.Data.SqlClient;

namespace ForteCommunityClinic.Helpers
{
    public static class NotificationHelper
    {
        // Overload for use inside an existing transaction
        public static void Create(SqlConnection conn, SqlTransaction transaction, int userId, string title, string message, string type)
        {
            var cmd = new SqlCommand(@"
                INSERT INTO Notifications (UserID, Title, Message, NotificationType)
                VALUES (@UserID, @Title, @Message, @NotificationType)", conn, transaction);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Message", message);
            cmd.Parameters.AddWithValue("@NotificationType", type);
            cmd.ExecuteNonQuery();
        }

        // Overload for standalone use (no existing transaction)
        public static void Create(SqlConnection conn, int userId, string title, string message, string type)
        {
            var cmd = new SqlCommand(@"
                INSERT INTO Notifications (UserID, Title, Message, NotificationType)
                VALUES (@UserID, @Title, @Message, @NotificationType)", conn);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Message", message);
            cmd.Parameters.AddWithValue("@NotificationType", type);
            cmd.ExecuteNonQuery();
        }
    }
}
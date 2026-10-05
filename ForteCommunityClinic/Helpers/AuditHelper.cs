using System.Data.SqlClient;
using System.Web;

namespace ForteCommunityClinic.Helpers
{
    public static class AuditHelper
    {
        // Overload for use inside an existing transaction
        public static void Log(SqlConnection conn, SqlTransaction transaction, int? userId, string action, string entityType, int? entityId, string description)
        {
            var cmd = new SqlCommand(@"
                INSERT INTO AuditLogs (UserID, Action, EntityType, EntityID, Description, IPAddress)
                VALUES (@UserID, @Action, @EntityType, @EntityID, @Description, @IPAddress)", conn, transaction);
            AddParameters(cmd, userId, action, entityType, entityId, description);
            cmd.ExecuteNonQuery();
        }

        // Overload for standalone use (no existing transaction)
        public static void Log(SqlConnection conn, int? userId, string action, string entityType, int? entityId, string description)
        {
            var cmd = new SqlCommand(@"
                INSERT INTO AuditLogs (UserID, Action, EntityType, EntityID, Description, IPAddress)
                VALUES (@UserID, @Action, @EntityType, @EntityID, @Description, @IPAddress)", conn);
            AddParameters(cmd, userId, action, entityType, entityId, description);
            cmd.ExecuteNonQuery();
        }

        private static void AddParameters(SqlCommand cmd, int? userId, string action, string entityType, int? entityId, string description)
        {
            cmd.Parameters.AddWithValue("@UserID", (object)userId ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@EntityType", (object)entityType ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@EntityID", (object)entityId ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", (object)description ?? System.DBNull.Value);

            string ip = HttpContext.Current?.Request?.UserHostAddress;
            cmd.Parameters.AddWithValue("@IPAddress", (object)ip ?? System.DBNull.Value);
        }
    }
}
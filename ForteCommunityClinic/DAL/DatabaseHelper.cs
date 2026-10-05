using System.Configuration;
using System.Data.SqlClient;

namespace ForteCommunityClinic.DAL
{
    public static class DatabaseHelper
    {
        // Reads the connection string from Web.config instead of hard-coding it anywhere
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["ForteClinicDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
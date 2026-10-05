using System;
using ForteCommunityClinic.DAL;

namespace ForteCommunityClinic
{
    public partial class TestConnection : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    Response.Write("Connected successfully to: " + conn.Database);
                }
            }
            catch (Exception ex)
            {
                Response.Write("Connection failed: " + ex.Message);
            }
        }
    }
}
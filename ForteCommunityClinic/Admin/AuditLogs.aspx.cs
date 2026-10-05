using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI.WebControls;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Admin
{
    public partial class AuditLogs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Admin");

            if (!IsPostBack)
            {
                LoadActionFilter();
                BindLogs();
            }
        }

        private void LoadActionFilter()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand("SELECT DISTINCT Action FROM AuditLogs ORDER BY Action", conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlFilterAction.DataSource = dt;
                ddlFilterAction.DataTextField = "Action";
                ddlFilterAction.DataValueField = "Action";
                ddlFilterAction.DataBind();
                ddlFilterAction.Items.Insert(0, new ListItem("All Actions", ""));
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e) => BindLogs();

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtFilterUser.Text = "";
            ddlFilterAction.SelectedIndex = 0;
            txtFilterDate.Text = "";
            BindLogs();
        }

        // Shared by both the on-screen grid and the CSV export, so they always match
        private DataTable GetFilteredLogs()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT al.CreatedDate, u.Username, al.Action, al.EntityType, al.EntityID, al.Description, al.IPAddress
                    FROM AuditLogs al
                    LEFT JOIN Users u ON al.UserID = u.UserID
                    WHERE (@FilterUser = '' OR u.Username LIKE '%' + @FilterUser + '%')
                      AND (@FilterAction = '' OR al.Action = @FilterAction)
                      AND (@FilterDate = '' OR CAST(al.CreatedDate AS DATE) = @FilterDateValue)
                    ORDER BY al.CreatedDate DESC", conn);

                cmd.Parameters.AddWithValue("@FilterUser", txtFilterUser.Text.Trim());
                cmd.Parameters.AddWithValue("@FilterAction", ddlFilterAction.SelectedValue);
                cmd.Parameters.AddWithValue("@FilterDate", txtFilterDate.Text.Trim());
                cmd.Parameters.AddWithValue("@FilterDateValue",
                    string.IsNullOrEmpty(txtFilterDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtFilterDate.Text));

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        private void BindLogs()
        {
            var dt = GetFilteredLogs();
            gvAuditLogs.DataSource = dt;
            gvAuditLogs.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var dt = GetFilteredLogs();
            var csv = new StringBuilder();

            // Header row
            csv.AppendLine("Date/Time,User,Action,Entity,Entity ID,Description,IP Address");

            foreach (DataRow row in dt.Rows)
            {
                csv.AppendLine(string.Join(",",
                    CsvEscape(Convert.ToDateTime(row["CreatedDate"]).ToString("yyyy-MM-dd HH:mm:ss")),
                    CsvEscape(row["Username"]?.ToString()),
                    CsvEscape(row["Action"]?.ToString()),
                    CsvEscape(row["EntityType"]?.ToString()),
                    CsvEscape(row["EntityID"]?.ToString()),
                    CsvEscape(row["Description"]?.ToString()),
                    CsvEscape(row["IPAddress"]?.ToString())
                ));
            }

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", $"attachment; filename=AuditLogs_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            Response.Write(csv.ToString());
            Response.End();
        }

        // Wraps a field in quotes and escapes any embedded quotes/commas, so
        // descriptions containing commas (e.g. "Claim status changed to Approved, notified patient")
        // don't break the CSV column alignment when opened in Excel
        private string CsvEscape(string value)
        {
            if (string.IsNullOrEmpty(value)) return "\"\"";
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
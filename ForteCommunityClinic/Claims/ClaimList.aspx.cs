using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Claims
{
    public partial class ClaimList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

            if (!IsPostBack)
            {
                LoadStatusFilter();
                BindClaims();
            }
        }

        private void LoadStatusFilter()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand("SELECT ClaimStatusID, StatusName FROM ClaimStatuses", conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlFilterStatus.DataSource = dt;
                ddlFilterStatus.DataTextField = "StatusName";
                ddlFilterStatus.DataValueField = "ClaimStatusID";
                ddlFilterStatus.DataBind();
                ddlFilterStatus.Items.Insert(0, new ListItem("All Statuses", ""));
            }
        }

        protected void ddlFilterStatus_SelectedIndexChanged(object sender, EventArgs e) => BindClaims();

        private void BindClaims()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT c.ClaimID, u.FirstName + ' ' + u.Surname AS PatientName,
                           mp.ProviderName, c.ClaimAmount, c.SubmittedDate, s.StatusName
                    FROM MedicalAidClaims c
                    JOIN Patients p ON c.PatientID = p.PatientID
                    JOIN Users u ON p.UserID = u.UserID
                    JOIN MedicalAidProviders mp ON c.MedicalAidProviderID = mp.MedicalAidProviderID
                    JOIN ClaimStatuses s ON c.ClaimStatusID = s.ClaimStatusID
                    WHERE (@FilterStatus = '' OR c.ClaimStatusID = @FilterStatusValue)
                    ORDER BY c.SubmittedDate DESC", conn);

                cmd.Parameters.AddWithValue("@FilterStatus", ddlFilterStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@FilterStatusValue",
                    ddlFilterStatus.SelectedValue == "" ? (object)DBNull.Value : int.Parse(ddlFilterStatus.SelectedValue));

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvClaims.DataSource = dt;
                gvClaims.DataBind();
            }
        }

        protected string GetStatusBadgeClass(string status)
        {
            switch (status)
            {
                case "Approved":
                case "Paid":
                    return "bg-success";
                case "Rejected":
                    return "bg-danger";
                case "Pending":
                    return "bg-warning text-dark";
                default:
                    return "bg-primary"; // Submitted, Under Review
            }
        }
    }
}
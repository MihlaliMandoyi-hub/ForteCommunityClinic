using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Claims
{
    public partial class ClaimDetails : System.Web.UI.Page
    {
        private int claimId;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

            if (!int.TryParse(Request.QueryString["id"], out claimId))
            {
                Response.Redirect("~/Claims/ClaimList.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadClaim();
                LoadStatusDropdown();
            }
        }

        private void LoadClaim()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT u.FirstName + ' ' + u.Surname AS PatientName, c.BillID, mp.ProviderName,
                           c.MembershipNumber, c.ClaimAmount, c.SubmittedDate, s.StatusName
                    FROM MedicalAidClaims c
                    JOIN Patients p ON c.PatientID = p.PatientID
                    JOIN Users u ON p.UserID = u.UserID
                    JOIN MedicalAidProviders mp ON c.MedicalAidProviderID = mp.MedicalAidProviderID
                    JOIN ClaimStatuses s ON c.ClaimStatusID = s.ClaimStatusID
                    WHERE c.ClaimID = @ClaimID", conn);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);

                conn.Open();
                var reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    Response.Redirect("~/Claims/ClaimList.aspx");
                    return;
                }

                litPatient.Text = reader["PatientName"].ToString();
                litBill.Text = "Bill #" + reader["BillID"];
                litProvider.Text = reader["ProviderName"].ToString();
                litMembership.Text = reader["MembershipNumber"].ToString();
                litAmount.Text = Convert.ToDecimal(reader["ClaimAmount"]).ToString("C2");
                litSubmitted.Text = reader["SubmittedDate"] == DBNull.Value
                    ? "-" : Convert.ToDateTime(reader["SubmittedDate"]).ToString("yyyy-MM-dd");
                litStatus.Text = reader["StatusName"].ToString();
            }
        }

        private void LoadStatusDropdown()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand("SELECT ClaimStatusID, StatusName FROM ClaimStatuses", conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlNewStatus.DataSource = dt;
                ddlNewStatus.DataTextField = "StatusName";
                ddlNewStatus.DataValueField = "ClaimStatusID";
                ddlNewStatus.DataBind();
            }
        }

        protected void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            string newStatusName = ddlNewStatus.SelectedItem.Text;

            if (newStatusName == "Rejected" && string.IsNullOrWhiteSpace(txtRejectionReason.Text))
            {
                lblMessage.Text = "A rejection reason is required when rejecting a claim.";
                lblMessage.Visible = true;
                pnlRejectionReason.Visible = true;
                return;
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                bool isFinalStatus = newStatusName == "Approved" || newStatusName == "Rejected" || newStatusName == "Paid";

                var cmd = new SqlCommand(@"
                    UPDATE MedicalAidClaims
                    SET ClaimStatusID = @ClaimStatusID,
                        RejectionReason = @RejectionReason,
                        ProcessedDate = CASE WHEN @IsFinalStatus = 1 THEN GETDATE() ELSE ProcessedDate END
                    WHERE ClaimID = @ClaimID", conn);

                cmd.Parameters.AddWithValue("@ClaimStatusID", int.Parse(ddlNewStatus.SelectedValue));
                cmd.Parameters.AddWithValue("@RejectionReason",
                    newStatusName == "Rejected" ? (object)txtRejectionReason.Text.Trim() : DBNull.Value);
                cmd.Parameters.AddWithValue("@IsFinalStatus", isFinalStatus ? 1 : 0);
                cmd.Parameters.AddWithValue("@ClaimID", claimId);

                conn.Open();
                cmd.ExecuteNonQuery();
                var getPatientUserCmd = new SqlCommand(@"
    SELECT p.UserID FROM MedicalAidClaims c
    JOIN Patients p ON c.PatientID = p.PatientID
    WHERE c.ClaimID = @ClaimID", conn);
                getPatientUserCmd.Parameters.AddWithValue("@ClaimID", claimId);
                object patientUserId = getPatientUserCmd.ExecuteScalar();

                if (patientUserId != null && patientUserId != DBNull.Value)
                {
                    NotificationHelper.Create(conn, (int)patientUserId,
                        "Medical Aid Claim Update", $"Your claim status has changed to: {newStatusName}.", "ClaimUpdate");
                }

            }

            LoadClaim();
            lblMessage.Visible = false;
        }
    }
}
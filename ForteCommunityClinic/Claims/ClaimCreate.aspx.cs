using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Claims
{
    public partial class ClaimCreate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Receptionist", "Admin");

            if (!IsPostBack)
            {
                LoadProviders();
            }
        }

        private void LoadProviders()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand("SELECT MedicalAidProviderID, ProviderName FROM MedicalAidProviders", conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlProvider.DataSource = dt;
                ddlProvider.DataTextField = "ProviderName";
                ddlProvider.DataValueField = "MedicalAidProviderID";
                ddlProvider.DataBind();
            }
        }

        protected void btnFindPatient_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT p.PatientID, p.PatientNumber + ' - ' + u.FirstName + ' ' + u.Surname AS PatientDisplay
                    FROM Patients p JOIN Users u ON p.UserID = u.UserID
                    WHERE p.IsActive = 1
                      AND (p.PatientNumber LIKE '%' + @Search + '%' OR u.Surname LIKE '%' + @Search + '%')", conn);
                cmd.Parameters.AddWithValue("@Search", txtPatientSearch.Text.Trim());

                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlPatient.DataSource = dt;
                ddlPatient.DataTextField = "PatientDisplay";
                ddlPatient.DataValueField = "PatientID";
                ddlPatient.DataBind();
            }
            LoadBillsForSelectedPatient();
        }

        protected void ddlPatient_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBillsForSelectedPatient();
        }

        private void LoadBillsForSelectedPatient()
        {
            ddlBill.Items.Clear();
            if (ddlPatient.Items.Count == 0) return;

            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT BillID, 'Bill #' + CAST(BillID AS VARCHAR) + ' - ' + CONVERT(VARCHAR, BillDate, 23)
                           + ' - Outstanding: R' + CAST(OutstandingAmount AS VARCHAR) AS BillDisplay
                    FROM Bills
                    WHERE PatientID = @PatientID AND Status IN ('Unpaid', 'PartiallyPaid')
                    ORDER BY BillDate DESC", conn);
                cmd.Parameters.AddWithValue("@PatientID", int.Parse(ddlPatient.SelectedValue));

                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlBill.DataSource = dt;
                ddlBill.DataTextField = "BillDisplay";
                ddlBill.DataValueField = "BillID";
                ddlBill.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (ddlPatient.Items.Count == 0)
            {
                lblMessage.Text = "Please find and select a patient.";
                lblMessage.Visible = true;
                return;
            }
            if (ddlBill.Items.Count == 0)
            {
                lblMessage.Text = "This patient has no unpaid or partially paid bills to claim against.";
                lblMessage.Visible = true;
                return;
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO MedicalAidClaims
                        (PatientID, BillID, MedicalAidProviderID, MembershipNumber, ClaimAmount, SubmittedDate)
                    VALUES
                        (@PatientID, @BillID, @MedicalAidProviderID, @MembershipNumber, @ClaimAmount, GETDATE())", conn);

                cmd.Parameters.AddWithValue("@PatientID", int.Parse(ddlPatient.SelectedValue));
                cmd.Parameters.AddWithValue("@BillID", int.Parse(ddlBill.SelectedValue));
                cmd.Parameters.AddWithValue("@MedicalAidProviderID", int.Parse(ddlProvider.SelectedValue));
                cmd.Parameters.AddWithValue("@MembershipNumber", txtMembershipNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@ClaimAmount", decimal.Parse(txtClaimAmount.Text));

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    AuditHelper.Log(conn, (int?)Session["UserID"], "CREATE CLAIM", "MedicalAidClaim", null, "Medical aid claim submitted.");
                    Response.Redirect("~/Claims/ClaimList.aspx");
                }
                catch (Exception)
                {
                    lblMessage.Text = "An error occurred while saving the claim. Please try again.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
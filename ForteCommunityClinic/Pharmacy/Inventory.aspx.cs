using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Pharmacy
{
    public partial class Inventory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Pharmacist", "Admin");

            if (!IsPostBack)
            {
                LoadMedications();
            }
        }

        private void LoadMedications()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(
                    "SELECT MedicationID, MedicationName FROM Medications WHERE IsActive = 1 ORDER BY MedicationName", conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlMedication.DataSource = dt;
                ddlMedication.DataTextField = "MedicationName";
                ddlMedication.DataValueField = "MedicationID";
                ddlMedication.DataBind();
            }
        }

        protected void btnReceive_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int medicationId = int.Parse(ddlMedication.SelectedValue);
            int quantity = int.Parse(txtQuantity.Text);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    var updateStockCmd = new SqlCommand(
                        "UPDATE Medications SET QuantityInStock = QuantityInStock + @Quantity WHERE MedicationID = @MedicationID",
                        conn, transaction);
                    updateStockCmd.Parameters.AddWithValue("@Quantity", quantity);
                    updateStockCmd.Parameters.AddWithValue("@MedicationID", medicationId);
                    updateStockCmd.ExecuteNonQuery();

                    var insertTransactionCmd = new SqlCommand(@"
                        INSERT INTO MedicationStockTransactions
                            (MedicationID, TransactionType, QuantityChange, PharmacistID, Notes)
                        VALUES
                            (@MedicationID, 'Receive', @Quantity,
                             (SELECT PharmacistID FROM Pharmacists WHERE UserID = @UserID), @Notes)",
                        conn, transaction);
                    insertTransactionCmd.Parameters.AddWithValue("@MedicationID", medicationId);
                    insertTransactionCmd.Parameters.AddWithValue("@Quantity", quantity);
                    insertTransactionCmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                    insertTransactionCmd.Parameters.AddWithValue("@Notes",
                        string.IsNullOrWhiteSpace(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim());
                    insertTransactionCmd.ExecuteNonQuery();

                    transaction.Commit();
                    lblMessage.Visible = true;
                    txtQuantity.Text = "";
                    txtNotes.Text = "";
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
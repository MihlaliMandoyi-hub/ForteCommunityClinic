using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Pharmacy
{
    public partial class DispenseMedication : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Pharmacist", "Admin");

            if (!IsPostBack)
            {
                BindPendingItems();
            }
        }

        private void BindPendingItems()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT pi.PrescriptionItemID, u.FirstName + ' ' + u.Surname AS PatientName,
                           m.MedicationName, pi.Dosage, pi.Quantity, m.QuantityInStock, p.PrescriptionDate
                    FROM PrescriptionItems pi
                    JOIN Prescriptions p ON pi.PrescriptionID = p.PrescriptionID
                    JOIN Consultations c ON p.ConsultationID = c.ConsultationID
                    JOIN Patients pt ON c.PatientID = pt.PatientID
                    JOIN Users u ON pt.UserID = u.UserID
                    JOIN Medications m ON pi.MedicationID = m.MedicationID
                    WHERE pi.IsDispensed = 0
                    ORDER BY p.PrescriptionDate", conn);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvPendingItems.DataSource = dt;
                gvPendingItems.DataBind();
            }
        }

        protected void gvPendingItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Dispense") return;

            int prescriptionItemId = int.Parse(e.CommandArgument.ToString());

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Lock in the medication + quantity needed, and check stock BEFORE making any changes
                    var getItemCmd = new SqlCommand(@"
                        SELECT pi.MedicationID, pi.Quantity, m.QuantityInStock, m.MedicationName
                        FROM PrescriptionItems pi
                        JOIN Medications m ON pi.MedicationID = m.MedicationID
                        WHERE pi.PrescriptionItemID = @PrescriptionItemID AND pi.IsDispensed = 0",
                        conn, transaction);
                    getItemCmd.Parameters.AddWithValue("@PrescriptionItemID", prescriptionItemId);

                    var reader = getItemCmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        transaction.Rollback();
                        lblMessage.Text = "This item was already dispensed by someone else.";
                        lblMessage.Visible = true;
                        BindPendingItems();
                        return;
                    }

                    int medicationId = (int)reader["MedicationID"];
                    int neededQuantity = (int)reader["Quantity"];
                    int availableStock = (int)reader["QuantityInStock"];
                    string medicationName = reader["MedicationName"].ToString();
                    reader.Close();

                    // ===== INSUFFICIENT STOCK CHECK — blocks dispensing before any update happens =====
                    if (availableStock < neededQuantity)
                    {
                        transaction.Rollback();
                        lblMessage.Text = $"Cannot dispense: only {availableStock} units of {medicationName} in stock, but {neededQuantity} are needed.";
                        lblMessage.Visible = true;
                        BindPendingItems();
                        return;
                    }
                    // ===== END CHECK =====

                    var updateStockCmd = new SqlCommand(
                        "UPDATE Medications SET QuantityInStock = QuantityInStock - @Quantity WHERE MedicationID = @MedicationID",
                        conn, transaction);
                    updateStockCmd.Parameters.AddWithValue("@Quantity", neededQuantity);
                    updateStockCmd.Parameters.AddWithValue("@MedicationID", medicationId);
                    updateStockCmd.ExecuteNonQuery();

                    var insertTransactionCmd = new SqlCommand(@"
                        INSERT INTO MedicationStockTransactions
                            (MedicationID, TransactionType, QuantityChange, PrescriptionItemID, PharmacistID)
                        VALUES
                            (@MedicationID, 'Dispense', @QuantityChange, @PrescriptionItemID,
                             (SELECT PharmacistID FROM Pharmacists WHERE UserID = @UserID))",
                        conn, transaction);
                    insertTransactionCmd.Parameters.AddWithValue("@MedicationID", medicationId);
                    insertTransactionCmd.Parameters.AddWithValue("@QuantityChange", -neededQuantity);
                    insertTransactionCmd.Parameters.AddWithValue("@PrescriptionItemID", prescriptionItemId);
                    insertTransactionCmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                    insertTransactionCmd.ExecuteNonQuery();

                    var markDispensedCmd = new SqlCommand(
                        "UPDATE PrescriptionItems SET IsDispensed = 1 WHERE PrescriptionItemID = @PrescriptionItemID",
                        conn, transaction);
                    markDispensedCmd.Parameters.AddWithValue("@PrescriptionItemID", prescriptionItemId);
                    markDispensedCmd.ExecuteNonQuery();
                    AuditHelper.Log(conn, transaction, (int?)Session["UserID"], "DISPENSE MEDICATION", "PrescriptionItem", prescriptionItemId, "Medication dispensed.");
                    var checkStockCmd = new SqlCommand(
    "SELECT QuantityInStock, ReorderLevel, MedicationName FROM Medications WHERE MedicationID = @MedicationID",
    conn, transaction);
                    checkStockCmd.Parameters.AddWithValue("@MedicationID", medicationId);
                    var stockReader = checkStockCmd.ExecuteReader();
                    stockReader.Read();
                    int stockNow = (int)stockReader["QuantityInStock"];
                    int reorderLevel = (int)stockReader["ReorderLevel"];
                    string medName = stockReader["MedicationName"].ToString();
                    stockReader.Close();

                    if (stockNow <= reorderLevel)
                    {
                        var pharmacistsCmd = new SqlCommand(
                            "SELECT UserID FROM Pharmacists WHERE IsActive = 1", conn, transaction);
                        var pharmacistsReader = pharmacistsCmd.ExecuteReader();
                        var pharmacistUserIds = new System.Collections.Generic.List<int>();
                        while (pharmacistsReader.Read()) pharmacistUserIds.Add((int)pharmacistsReader["UserID"]);
                        pharmacistsReader.Close();

                        foreach (int pharmacistUserId in pharmacistUserIds)
                        {
                            NotificationHelper.Create(conn, transaction, pharmacistUserId,
                                "Low Stock Alert", $"{medName} has dropped to {stockNow} units (reorder level {reorderLevel}).", "LowStock");
                        }
                    }

                    transaction.Commit();
                    lblMessage.Visible = false;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    lblMessage.Text = "An error occurred while dispensing. Please try again.";
                    lblMessage.Visible = true;
                }
            }

            BindPendingItems();
        }
    }
}
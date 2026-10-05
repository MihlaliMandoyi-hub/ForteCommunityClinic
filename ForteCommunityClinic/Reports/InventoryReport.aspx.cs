using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Reports
{
    public partial class InventoryReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "ClinicManager", "Admin", "Pharmacist");

            if (!IsPostBack)
            {
                RunReport();
            }
        }

        private void RunReport()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT m.MedicationName, c.CategoryName, m.QuantityInStock, m.ReorderLevel,
                           m.UnitPrice, (m.QuantityInStock * m.UnitPrice) AS StockValue
                    FROM Medications m
                    JOIN MedicationCategories c ON m.MedicationCategoryID = c.MedicationCategoryID
                    WHERE m.IsActive = 1
                    ORDER BY m.MedicationName", conn);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvInventory.DataSource = dt;
                gvInventory.DataBind();

                lblTotalMeds.Text = dt.Rows.Count.ToString();

                int lowStockCount = 0;
                decimal totalValue = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if ((int)row["QuantityInStock"] <= (int)row["ReorderLevel"]) lowStockCount++;
                    totalValue += (decimal)row["StockValue"];
                }
                lblLowStockCount.Text = lowStockCount.ToString();
                lblStockValue.Text = totalValue.ToString("C2");
            }
        }
    }
}
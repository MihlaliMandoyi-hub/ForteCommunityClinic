using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Pharmacy
{
    public partial class LowStock : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Pharmacist", "Admin", "ClinicManager");

            if (!IsPostBack)
            {
                BindLowStock();
            }
        }

        private void BindLowStock()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT m.MedicationName, c.CategoryName, m.QuantityInStock, m.ReorderLevel
                    FROM Medications m
                    JOIN MedicationCategories c ON m.MedicationCategoryID = c.MedicationCategoryID
                    WHERE m.QuantityInStock <= m.ReorderLevel AND m.IsActive = 1
                    ORDER BY m.QuantityInStock ASC", conn);

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvLowStock.DataSource = dt;
                gvLowStock.DataBind();
            }
        }
    }
}
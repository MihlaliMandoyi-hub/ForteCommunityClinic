using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Pharmacy
{
    public partial class MedicationList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Pharmacist", "Admin");

            if (!IsPostBack)
            {
                BindMedications(null);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e) => BindMedications(txtSearchName.Text.Trim());

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearchName.Text = "";
            BindMedications(null);
        }

        private void BindMedications(string nameFilter)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT m.MedicationID, m.MedicationName, c.CategoryName, m.QuantityInStock,
                           m.ReorderLevel, m.UnitPrice, m.ExpiryDate
                    FROM Medications m
                    JOIN MedicationCategories c ON m.MedicationCategoryID = c.MedicationCategoryID
                    WHERE m.IsActive = 1
                      AND (@NameFilter = '' OR m.MedicationName LIKE '%' + @NameFilter + '%')
                    ORDER BY m.MedicationName", conn);
                cmd.Parameters.AddWithValue("@NameFilter", nameFilter ?? "");

                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                gvMedications.DataSource = dt;
                gvMedications.DataBind();
            }
        }
    }
}
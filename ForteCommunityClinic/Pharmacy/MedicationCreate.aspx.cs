using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Pharmacy
{
    public partial class MedicationCreate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Pharmacist", "Admin");

            if (!IsPostBack)
            {
                LoadCategories();
            }
        }

        private void LoadCategories()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand("SELECT MedicationCategoryID, CategoryName FROM MedicationCategories", conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlCategory.DataSource = dt;
                ddlCategory.DataTextField = "CategoryName";
                ddlCategory.DataValueField = "MedicationCategoryID";
                ddlCategory.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO Medications
                        (MedicationName, Description, MedicationCategoryID, QuantityInStock, ReorderLevel, UnitPrice, ExpiryDate)
                    VALUES
                        (@MedicationName, @Description, @MedicationCategoryID, @QuantityInStock, @ReorderLevel, @UnitPrice, @ExpiryDate)", conn);

                cmd.Parameters.AddWithValue("@MedicationName", txtMedicationName.Text.Trim());
                cmd.Parameters.AddWithValue("@Description",
                    string.IsNullOrWhiteSpace(txtDescription.Text) ? (object)DBNull.Value : txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@MedicationCategoryID", int.Parse(ddlCategory.SelectedValue));
                cmd.Parameters.AddWithValue("@QuantityInStock", int.Parse(txtQuantityInStock.Text));
                cmd.Parameters.AddWithValue("@ReorderLevel", int.Parse(txtReorderLevel.Text));
                cmd.Parameters.AddWithValue("@UnitPrice", decimal.Parse(txtUnitPrice.Text));
                cmd.Parameters.AddWithValue("@ExpiryDate",
                    string.IsNullOrWhiteSpace(txtExpiryDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtExpiryDate.Text));

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Response.Redirect("~/Pharmacy/MedicationList.aspx");
                }
                catch (Exception)
                {
                    lblMessage.Text = "An error occurred while saving the medication. Please try again.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
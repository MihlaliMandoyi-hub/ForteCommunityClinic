using System;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Consultations
{
    public partial class PrescriptionCreate : System.Web.UI.Page
    {
        private int consultationId;

        // Holds the items the doctor has added so far, across postbacks, until Finish is clicked
        private DataTable DraftItems
        {
            get
            {
                if (ViewState["DraftItems"] == null)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("MedicationID", typeof(int));
                    dt.Columns.Add("MedicationName", typeof(string));
                    dt.Columns.Add("Dosage", typeof(string));
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("Instructions", typeof(string));
                    ViewState["DraftItems"] = dt;
                }
                return (DataTable)ViewState["DraftItems"];
            }
            set { ViewState["DraftItems"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Doctor", "Admin");

            if (!int.TryParse(Request.QueryString["consultationId"], out consultationId))
            {
                Response.Redirect("~/Appointments/AppointmentList.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPatientInfo();
                LoadMedications();
            }
        }

        private void LoadPatientInfo()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT u.FirstName + ' ' + u.Surname + ' (' + p.PatientNumber + ')' AS PatientInfo
                    FROM Consultations c
                    JOIN Patients p ON c.PatientID = p.PatientID
                    JOIN Users u ON p.UserID = u.UserID
                    WHERE c.ConsultationID = @ConsultationID", conn);
                cmd.Parameters.AddWithValue("@ConsultationID", consultationId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result == null)
                {
                    Response.Redirect("~/Appointments/AppointmentList.aspx");
                    return;
                }
                lblPatientInfo.Text = "Patient: " + result;
            }
        }

        private void LoadMedications()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                var cmd = new SqlCommand(
                    "SELECT MedicationID, MedicationName + ' (Stock: ' + CAST(QuantityInStock AS VARCHAR) + ')' AS DisplayName " +
                    "FROM Medications WHERE IsActive = 1 ORDER BY MedicationName", conn);

                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlMedication.DataSource = dt;
                ddlMedication.DataTextField = "DisplayName";
                ddlMedication.DataValueField = "MedicationID";
                ddlMedication.DataBind();
            }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDosage.Text) || string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                lblMessage.Text = "Dosage and quantity are required for each medication.";
                lblMessage.Visible = true;
                return;
            }

            var items = DraftItems;
            var row = items.NewRow();
            row["MedicationID"] = int.Parse(ddlMedication.SelectedValue);
            row["MedicationName"] = ddlMedication.SelectedItem.Text;
            row["Dosage"] = txtDosage.Text.Trim();
            row["Quantity"] = int.Parse(txtQuantity.Text);
            row["Instructions"] = txtInstructions.Text.Trim();
            items.Rows.Add(row);
            DraftItems = items;

            gvDraftItems.DataSource = items;
            gvDraftItems.DataBind();

            txtDosage.Text = "";
            txtQuantity.Text = "";
            txtInstructions.Text = "";
            lblMessage.Visible = false;
        }

        protected void btnFinish_Click(object sender, EventArgs e)
        {
            if (DraftItems.Rows.Count == 0)
            {
                lblMessage.Text = "Add at least one medication before finishing the prescription.";
                lblMessage.Visible = true;
                return;
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    var getDoctorCmd = new SqlCommand(
                        "SELECT DoctorID FROM Consultations WHERE ConsultationID = @ConsultationID",
                        conn, transaction);
                    getDoctorCmd.Parameters.AddWithValue("@ConsultationID", consultationId);
                    int doctorId = (int)getDoctorCmd.ExecuteScalar();

                    var insertPrescriptionCmd = new SqlCommand(@"
                        INSERT INTO Prescriptions (ConsultationID, DoctorID)
                        OUTPUT INSERTED.PrescriptionID
                        VALUES (@ConsultationID, @DoctorID)", conn, transaction);
                    insertPrescriptionCmd.Parameters.AddWithValue("@ConsultationID", consultationId);
                    insertPrescriptionCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    int prescriptionId = (int)insertPrescriptionCmd.ExecuteScalar();

                    foreach (DataRow row in DraftItems.Rows)
                    {
                        var insertItemCmd = new SqlCommand(@"
                            INSERT INTO PrescriptionItems (PrescriptionID, MedicationID, Dosage, Quantity, Instructions)
                            VALUES (@PrescriptionID, @MedicationID, @Dosage, @Quantity, @Instructions)",
                            conn, transaction);
                        insertItemCmd.Parameters.AddWithValue("@PrescriptionID", prescriptionId);
                        insertItemCmd.Parameters.AddWithValue("@MedicationID", row["MedicationID"]);
                        insertItemCmd.Parameters.AddWithValue("@Dosage", row["Dosage"]);
                        insertItemCmd.Parameters.AddWithValue("@Quantity", row["Quantity"]);
                        insertItemCmd.Parameters.AddWithValue("@Instructions",
                            string.IsNullOrWhiteSpace(row["Instructions"].ToString()) ? (object)DBNull.Value : row["Instructions"]);
                        insertItemCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    using (var auditConn = DatabaseHelper.GetConnection())
                    {
                        auditConn.Open();
                        AuditHelper.Log(auditConn, (int?)Session["UserID"], "CREATE PRESCRIPTION", "Prescription", prescriptionId, "Prescription created.");
                    }

                    Response.Redirect("ConsultationDetails.aspx?id=" + consultationId);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    lblMessage.Text = "An error occurred while saving the prescription. Please try again.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
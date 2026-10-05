using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic.Billing
{
    public partial class MyBillPayment : System.Web.UI.Page
    {
        private int billId;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireRole(this, "Patient");

            if (!int.TryParse(Request.QueryString["billId"], out billId))
            {
                Response.Redirect("~/Billing/MyBills.aspx");
                return;
            }

            lnkBack.HRef = "MyBillDetails.aspx?id=" + billId;

            if (!IsPostBack)
            {
                LoadBillInfo();
                LoadPaymentMethods();
            }
        }

        private void LoadBillInfo()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                // Ownership check again — a patient can only pay a bill that is their own
                var cmd = new SqlCommand(@"
                    SELECT OutstandingAmount FROM Bills
                    WHERE BillID = @BillID
                      AND PatientID = (SELECT PatientID FROM Patients WHERE UserID = @UserID)", conn);
                cmd.Parameters.AddWithValue("@BillID", billId);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    Response.Redirect("~/Billing/MyBills.aspx");
                    return;
                }

                lblBillInfo.Text = $"Outstanding balance: {Convert.ToDecimal(result):C2}";
            }
        }

        private void LoadPaymentMethods()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                // Cash is excluded — a patient paying online has no cash-in-hand transaction to record
                var cmd = new SqlCommand(
                    "SELECT PaymentMethodID, MethodName FROM PaymentMethods WHERE MethodName <> 'Cash'", conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);

                ddlPaymentMethod.DataSource = dt;
                ddlPaymentMethod.DataTextField = "MethodName";
                ddlPaymentMethod.DataValueField = "PaymentMethodID";
                ddlPaymentMethod.DataBind();
            }
        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            decimal amount = decimal.Parse(txtAmount.Text);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // Re-check ownership AND get current totals inside the transaction
                    var checkCmd = new SqlCommand(@"
                        SELECT TotalAmount, AmountPaid FROM Bills
                        WHERE BillID = @BillID
                          AND PatientID = (SELECT PatientID FROM Patients WHERE UserID = @UserID)", conn, transaction);
                    checkCmd.Parameters.AddWithValue("@BillID", billId);
                    checkCmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                    var reader = checkCmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        transaction.Rollback();
                        Response.Redirect("~/Billing/MyBills.aspx");
                        return;
                    }
                    decimal totalAmount = (decimal)reader["TotalAmount"];
                    decimal currentPaid = (decimal)reader["AmountPaid"];
                    reader.Close();

                    if (currentPaid + amount > totalAmount)
                    {
                        transaction.Rollback();
                        lblMessage.Text = $"This payment would exceed the bill total. Maximum you can pay is {(totalAmount - currentPaid):C2}.";
                        lblMessage.Visible = true;
                        return;
                    }

                    string referenceNumber = "SELFPAY-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    var insertPaymentCmd = new SqlCommand(@"
                        INSERT INTO Payments (BillID, PaymentMethodID, AmountPaid, ReceivedBy, ReferenceNumber)
                        OUTPUT INSERTED.PaymentID
                        VALUES (@BillID, @PaymentMethodID, @AmountPaid, @ReceivedBy, @ReferenceNumber)",
                        conn, transaction);
                    insertPaymentCmd.Parameters.AddWithValue("@BillID", billId);
                    insertPaymentCmd.Parameters.AddWithValue("@PaymentMethodID", int.Parse(ddlPaymentMethod.SelectedValue));
                    insertPaymentCmd.Parameters.AddWithValue("@AmountPaid", amount);
                    insertPaymentCmd.Parameters.AddWithValue("@ReceivedBy", Session["UserID"]); // patient "received" their own self-pay
                    insertPaymentCmd.Parameters.AddWithValue("@ReferenceNumber", referenceNumber);
                    int paymentId = (int)insertPaymentCmd.ExecuteScalar();

                    decimal newAmountPaid = currentPaid + amount;
                    string newStatus = newAmountPaid >= totalAmount ? "Paid" : "PartiallyPaid";

                    var updateBillCmd = new SqlCommand(
                        "UPDATE Bills SET AmountPaid = @AmountPaid, Status = @Status WHERE BillID = @BillID",
                        conn, transaction);
                    updateBillCmd.Parameters.AddWithValue("@AmountPaid", newAmountPaid);
                    updateBillCmd.Parameters.AddWithValue("@Status", newStatus);
                    updateBillCmd.Parameters.AddWithValue("@BillID", billId);
                    updateBillCmd.ExecuteNonQuery();

                    AuditHelper.Log(conn, transaction, (int?)Session["UserID"], "RECORD PAYMENT", "Payment", paymentId, "Patient paid via self-service portal.");

                    transaction.Commit();
                    Response.Redirect("MyBillDetails.aspx?id=" + billId);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    lblMessage.Text = "An error occurred while processing the payment. Please try again.";
                    lblMessage.Visible = true;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ForteCommunityClinic.DAL;
using ForteCommunityClinic.Helpers;

namespace ForteCommunityClinic
{
    public partial class Dashboard : System.Web.UI.Page
    {
        // =========================================================
        // PAGE LOAD
        // =========================================================
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthHelper.RequireLogin(this);
            if (IsPostBack) return;

            string role = Session["Role"]?.ToString();
            int userId = Convert.ToInt32(Session["UserID"]);

            // Missed-appointment sweep only needs to run when staff open their dashboard
            if (role != "Patient") SweepMissedAppointments();

            litTitle.Text = "Welcome, " + Session["FullName"];

            var cards = new List<KeyValuePair<string, string>>();
            var actions = new List<KeyValuePair<string, string>>();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                switch (role)
                {
                    case "Patient": BuildPatient(conn, userId, cards, actions); break;
                    case "Receptionist": BuildReceptionist(conn, cards, actions); break;
                    case "Doctor": BuildDoctor(conn, userId, cards, actions); break;
                    case "Pharmacist": BuildPharmacist(conn, cards, actions); break;
                    case "Admin":
                    case "ClinicManager": BuildManagement(conn, cards, actions); break;
                }
            }

            rptCards.DataSource = cards;
            rptCards.DataBind();
            rptActions.DataSource = actions;
            rptActions.DataBind();
        }

        // =========================================================
        // MARK MISSED APPOINTMENTS
        // =========================================================
        private void SweepMissedAppointments()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string findSql = @"
                    SELECT a.AppointmentID, p.UserID AS PatientUserID
                    FROM Appointments a
                    INNER JOIN Patients p ON a.PatientID = p.PatientID
                    INNER JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                    WHERE s.StatusName = 'Scheduled'
                      AND (CAST(a.AppointmentDate AS DATETIME) + CAST(a.AppointmentTime AS DATETIME)) < GETDATE()";

                var appointmentsToMark = new List<Tuple<int, object>>();

                using (SqlCommand findCmd = new SqlCommand(findSql, conn))
                {
                    using (SqlDataReader reader = findCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            appointmentsToMark.Add(new Tuple<int, object>(
                                Convert.ToInt32(reader["AppointmentID"]),
                                reader["PatientUserID"]));
                        }
                    }
                }

                foreach (var appointment in appointmentsToMark)
                {
                    string updateSql = @"
                        UPDATE Appointments
                        SET AppointmentStatusID =
                            (SELECT AppointmentStatusID FROM AppointmentStatuses WHERE StatusName = 'Missed')
                        WHERE AppointmentID = @AppointmentID";

                    using (SqlCommand updateCmd = new SqlCommand(updateSql, conn))
                    {
                        updateCmd.Parameters.Add("@AppointmentID", SqlDbType.Int).Value = appointment.Item1;
                        updateCmd.ExecuteNonQuery();
                    }

                    // Create notification for patient
                    if (appointment.Item2 != DBNull.Value)
                    {
                        NotificationHelper.Create(
                            conn,
                            Convert.ToInt32(appointment.Item2),
                            "Missed Appointment",
                            "You missed a scheduled appointment. Please contact reception to reschedule.",
                            "MissedAppointment");
                    }
                }
            }
        }

        // =========================================================
        // SHARED SQL
        // =========================================================
        private const string TodaySchedule = @"
            SELECT u1.FirstName + ' ' + u1.Surname AS Patient,
                   u2.FirstName + ' ' + u2.Surname AS Doctor,
                   LEFT(CONVERT(VARCHAR(8), a.AppointmentTime, 108), 5) AS [Time],
                   s.StatusName AS Status
            FROM Appointments a
            JOIN Patients p ON a.PatientID = p.PatientID
            JOIN Users u1 ON p.UserID = u1.UserID
            JOIN Doctors d ON a.DoctorID = d.DoctorID
            JOIN Users u2 ON d.UserID = u2.UserID
            JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
            WHERE a.AppointmentDate = CAST(GETDATE() AS DATE) {0}
            ORDER BY a.AppointmentTime";

        private const string LowStockSql = @"
            SELECT MedicationName AS Medication, QuantityInStock AS Available, ReorderLevel AS [Reorder Level]
            FROM Medications
            WHERE QuantityInStock <= ReorderLevel AND IsActive = 1
            ORDER BY QuantityInStock";

        private const string PendingClaimsSql = @"
            SELECT COUNT(*) FROM MedicalAidClaims c
            JOIN ClaimStatuses s ON c.ClaimStatusID = s.ClaimStatusID
            WHERE s.StatusName IN ('Pending', 'Submitted', 'Under Review')";

        // =========================================================
        // ROLE DASHBOARDS
        // =========================================================
        private void BuildPatient(SqlConnection conn, int userId,
            List<KeyValuePair<string, string>> cards, List<KeyValuePair<string, string>> actions)
        {
            litSubtitle.Text = "Your appointments and bills at a glance.";
            const string me = "(SELECT PatientID FROM Patients WHERE UserID = @u)";

            cards.Add(Card("Upcoming Appointments", Scalar(conn, $@"
                SELECT COUNT(*) FROM Appointments a
                JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                WHERE a.PatientID = {me} AND a.AppointmentDate >= CAST(GETDATE() AS DATE)
                  AND s.StatusName IN ('Scheduled', 'Checked In')", U(userId))));
            cards.Add(Card("Outstanding Balance", Money(Scalar(conn,
                $"SELECT ISNULL(SUM(OutstandingAmount), 0) FROM Bills WHERE PatientID = {me}", U(userId)))));
            cards.Add(Card("Unread Notifications", Scalar(conn,
                "SELECT COUNT(*) FROM Notifications WHERE UserID = @u AND IsRead = 0", U(userId))));

            actions.Add(Card("Book Appointment", "~/Appointments/BookAppointment.aspx"));
            actions.Add(Card("My Appointments", "~/Appointments/MyAppointments.aspx"));
            actions.Add(Card("My Bills", "~/Billing/MyBills.aspx"));
            actions.Add(Card("My Profile", "~/Account/MyProfile.aspx"));

            ShowTable(pnlTable1, litTable1Title, gvTable1, "My Next Appointments", Table(conn, $@"
                SELECT TOP 5 u.FirstName + ' ' + u.Surname AS Doctor,
                       CONVERT(VARCHAR(10), a.AppointmentDate, 120) AS [Date],
                       LEFT(CONVERT(VARCHAR(8), a.AppointmentTime, 108), 5) AS [Time],
                       s.StatusName AS Status
                FROM Appointments a
                JOIN Doctors d ON a.DoctorID = d.DoctorID
                JOIN Users u ON d.UserID = u.UserID
                JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                WHERE a.PatientID = {me} AND a.AppointmentDate >= CAST(GETDATE() AS DATE)
                  AND s.StatusName IN ('Scheduled', 'Checked In')
                ORDER BY a.AppointmentDate, a.AppointmentTime", U(userId)));

            ShowTable(pnlTable2, litTable2Title, gvTable2, "My Recent Bills", Table(conn, $@"
                SELECT TOP 5 CONVERT(VARCHAR(10), BillDate, 120) AS [Date],
                       'R ' + CONVERT(VARCHAR(20), TotalAmount) AS Total,
                       'R ' + CONVERT(VARCHAR(20), OutstandingAmount) AS Outstanding,
                       Status
                FROM Bills WHERE PatientID = {me} ORDER BY BillDate DESC", U(userId)));
        }

        private void BuildReceptionist(SqlConnection conn,
            List<KeyValuePair<string, string>> cards, List<KeyValuePair<string, string>> actions)
        {
            litSubtitle.Text = "Today's front-desk overview.";

            cards.Add(Card("Today's Appointments", Scalar(conn,
                "SELECT COUNT(*) FROM Appointments WHERE AppointmentDate = CAST(GETDATE() AS DATE)")));
            cards.Add(Card("Checked In Now", Scalar(conn, @"
                SELECT COUNT(*) FROM Appointments a
                JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                WHERE a.AppointmentDate = CAST(GETDATE() AS DATE) AND s.StatusName = 'Checked In'")));
            cards.Add(Card("Today's Payments", Money(Scalar(conn,
                "SELECT ISNULL(SUM(AmountPaid), 0) FROM Payments WHERE CAST(PaymentDate AS DATE) = CAST(GETDATE() AS DATE)"))));
            cards.Add(Card("Pending Claims", Scalar(conn, PendingClaimsSql)));

            actions.Add(Card("Book Appointment", "~/Appointments/AppointmentCreate.aspx"));
            actions.Add(Card("Register Patient", "~/Patients/PatientCreate.aspx"));
            actions.Add(Card("Appointments", "~/Appointments/AppointmentList.aspx"));
            actions.Add(Card("Create Bill", "~/Billing/BillCreate.aspx"));
            actions.Add(Card("Claims", "~/Claims/ClaimList.aspx"));

            ShowTable(pnlTable1, litTable1Title, gvTable1, "Today's Appointments",
                Table(conn, string.Format(TodaySchedule, "")));

            ShowTable(pnlTable2, litTable2Title, gvTable2, "Bills Awaiting Payment", Table(conn, @"
                SELECT TOP 10 u.FirstName + ' ' + u.Surname AS Patient,
                       'R ' + CONVERT(VARCHAR(20), b.OutstandingAmount) AS Outstanding,
                       b.Status
                FROM Bills b
                JOIN Patients p ON b.PatientID = p.PatientID
                JOIN Users u ON p.UserID = u.UserID
                WHERE b.OutstandingAmount > 0
                ORDER BY b.BillDate"));
        }

        private void BuildDoctor(SqlConnection conn, int userId,
            List<KeyValuePair<string, string>> cards, List<KeyValuePair<string, string>> actions)
        {
            litSubtitle.Text = "Your patients and consultations today.";
            const string myDoctorId = "(SELECT DoctorID FROM Doctors WHERE UserID = @u)";

            cards.Add(Card("My Appointments Today", Scalar(conn, $@"
                SELECT COUNT(*) FROM Appointments
                WHERE AppointmentDate = CAST(GETDATE() AS DATE) AND DoctorID = {myDoctorId}", U(userId))));
            cards.Add(Card("Waiting (Checked In)", Scalar(conn, $@"
                SELECT COUNT(*) FROM Appointments a
                JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                WHERE a.AppointmentDate = CAST(GETDATE() AS DATE) AND a.DoctorID = {myDoctorId}
                  AND s.StatusName = 'Checked In'", U(userId))));
            cards.Add(Card("Completed Today", Scalar(conn, $@"
                SELECT COUNT(*) FROM Consultations
                WHERE DoctorID = {myDoctorId} AND Status = 'Completed'
                  AND CAST(ConsultationDate AS DATE) = CAST(GETDATE() AS DATE)", U(userId))));

            actions.Add(Card("Today's Patients (Vitals)", "~/Nursing/TodaysPatients.aspx"));
            actions.Add(Card("My Appointments", "~/Appointments/AppointmentList.aspx"));
            actions.Add(Card("Patients", "~/Patients/PatientList.aspx"));

            ShowTable(pnlTable1, litTable1Title, gvTable1, "My Schedule Today",
                Table(conn, string.Format(TodaySchedule, "AND a.DoctorID = " + myDoctorId), U(userId)));

            ShowTable(pnlTable2, litTable2Title, gvTable2, "My Recent Consultations", Table(conn, $@"
                SELECT TOP 5 u.FirstName + ' ' + u.Surname AS Patient,
                       CONVERT(VARCHAR(16), c.ConsultationDate, 120) AS [Date],
                       c.Status
                FROM Consultations c
                JOIN Patients p ON c.PatientID = p.PatientID
                JOIN Users u ON p.UserID = u.UserID
                WHERE c.DoctorID = {myDoctorId}
                ORDER BY c.ConsultationDate DESC", U(userId)));
        }

        private void BuildPharmacist(SqlConnection conn,
            List<KeyValuePair<string, string>> cards, List<KeyValuePair<string, string>> actions)
        {
            litSubtitle.Text = "Prescriptions waiting and stock levels.";

            cards.Add(Card("Items To Dispense", Scalar(conn,
                "SELECT COUNT(*) FROM PrescriptionItems WHERE IsDispensed = 0")));
            cards.Add(Card("Low Stock Items", Scalar(conn,
                "SELECT COUNT(*) FROM Medications WHERE QuantityInStock <= ReorderLevel AND IsActive = 1")));
            cards.Add(Card("Medications In Catalogue", Scalar(conn,
                "SELECT COUNT(*) FROM Medications WHERE IsActive = 1")));

            actions.Add(Card("Dispense Medication", "~/Pharmacy/DispenseMedication.aspx"));
            actions.Add(Card("Inventory", "~/Pharmacy/Inventory.aspx"));
            actions.Add(Card("Medication List", "~/Pharmacy/MedicationList.aspx"));
            actions.Add(Card("Low Stock", "~/Pharmacy/LowStock.aspx"));
            actions.Add(Card("Add Medication", "~/Pharmacy/MedicationCreate.aspx"));

            ShowTable(pnlTable1, litTable1Title, gvTable1, "Prescriptions Waiting", Table(conn, @"
                SELECT TOP 10 u.FirstName + ' ' + u.Surname AS Patient,
                       m.MedicationName AS Medication, pi.Quantity AS Qty,
                       m.QuantityInStock AS [In Stock]
                FROM PrescriptionItems pi
                JOIN Prescriptions p ON pi.PrescriptionID = p.PrescriptionID
                JOIN Consultations c ON p.ConsultationID = c.ConsultationID
                JOIN Patients pt ON c.PatientID = pt.PatientID
                JOIN Users u ON pt.UserID = u.UserID
                JOIN Medications m ON pi.MedicationID = m.MedicationID
                WHERE pi.IsDispensed = 0
                ORDER BY p.PrescriptionDate"));

            ShowTable(pnlTable2, litTable2Title, gvTable2, "Low Stock Medications", Table(conn, LowStockSql));
        }

        private void BuildManagement(SqlConnection conn,
            List<KeyValuePair<string, string>> cards, List<KeyValuePair<string, string>> actions)
        {
            litSubtitle.Text = "Clinic-wide performance and activity.";

            cards.Add(Card("Registered Patients", Scalar(conn, "SELECT COUNT(*) FROM Patients WHERE IsActive = 1")));
            cards.Add(Card("Staff Accounts", Scalar(conn, @"
                SELECT COUNT(DISTINCT ur.UserID) FROM UserRoles ur
                JOIN Roles r ON ur.RoleID = r.RoleID WHERE r.RoleName <> 'Patient'")));
            cards.Add(Card("Today's Appointments", Scalar(conn,
                "SELECT COUNT(*) FROM Appointments WHERE AppointmentDate = CAST(GETDATE() AS DATE)")));
            cards.Add(Card("Missed Appointments", Scalar(conn, @"
                SELECT COUNT(*) FROM Appointments a
                JOIN AppointmentStatuses s ON a.AppointmentStatusID = s.AppointmentStatusID
                WHERE s.StatusName = 'Missed'")));
            cards.Add(Card("Today's Revenue", Money(Scalar(conn,
                "SELECT ISNULL(SUM(AmountPaid), 0) FROM Payments WHERE CAST(PaymentDate AS DATE) = CAST(GETDATE() AS DATE)"))));
            cards.Add(Card("Outstanding Bills", Money(Scalar(conn,
                "SELECT ISNULL(SUM(OutstandingAmount), 0) FROM Bills WHERE OutstandingAmount > 0"))));
            cards.Add(Card("Pending Claims", Scalar(conn, PendingClaimsSql)));
            cards.Add(Card("Low Stock Items", Scalar(conn,
                "SELECT COUNT(*) FROM Medications WHERE QuantityInStock <= ReorderLevel AND IsActive = 1")));

            actions.Add(Card("Revenue Report", "~/Reports/RevenueReport.aspx"));
            actions.Add(Card("Claims Report", "~/Reports/ClaimsReport.aspx"));
            actions.Add(Card("Daily Appointments", "~/Reports/DailyAppointments.aspx"));
            actions.Add(Card("Inventory Report", "~/Reports/InventoryReport.aspx"));
            if (Session["Role"]?.ToString() == "Admin")
                actions.Add(Card("Audit Logs", "~/Admin/AuditLogs.aspx"));

            ShowTable(pnlTable1, litTable1Title, gvTable1, "Today's Appointments",
                Table(conn, string.Format(TodaySchedule, "")));
            ShowTable(pnlTable2, litTable2Title, gvTable2, "Low Stock Medications", Table(conn, LowStockSql));
        }

        // =========================================================
        // SMALL HELPERS
        // =========================================================
        private static KeyValuePair<string, string> Card(string key, object value)
        {
            return new KeyValuePair<string, string>(key, Convert.ToString(value));
        }

        private static string Money(object value)
        {
            return "R " + Convert.ToDecimal(value).ToString("N2");
        }

        // A SqlParameter can only belong to one command, so always create a fresh one
        private static SqlParameter U(int userId)
        {
            return new SqlParameter("@u", userId);
        }

        private static object Scalar(SqlConnection conn, string sql, params SqlParameter[] ps)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddRange(ps);
                object result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? 0 : result;
            }
        }

        private static DataTable Table(SqlConnection conn, string sql, params SqlParameter[] ps)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                using (var adapter = new SqlDataAdapter(cmd)) adapter.Fill(dt);
                return dt;
            }
        }

        private static void ShowTable(System.Web.UI.WebControls.Panel panel,
            System.Web.UI.WebControls.Literal title, System.Web.UI.WebControls.GridView grid,
            string heading, DataTable data)
        {
            title.Text = heading;
            grid.DataSource = data;
            grid.DataBind();
            panel.Visible = true;
        }
    }
}
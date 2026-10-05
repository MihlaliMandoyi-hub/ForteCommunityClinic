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
        protected void Page_Load(object sender, EventArgs e)
        {
            // Make sure the user is logged in.
            AuthHelper.RequireLogin(this);

            // Automatically mark old scheduled appointments
            // as missed.
            SweepMissedAppointments();

            // Only load dashboard data the first time
            // the page opens.
            if (!IsPostBack)
            {
                LoadSummaryCards();
                LoadTodaysAppointments();
                LoadLowStock();
            }
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
                    SELECT
                        a.AppointmentID,
                        p.UserID AS PatientUserID

                    FROM Appointments a

                    INNER JOIN Patients p
                        ON a.PatientID = p.PatientID

                    INNER JOIN AppointmentStatuses s
                        ON a.AppointmentStatusID =
                           s.AppointmentStatusID

                    WHERE s.StatusName = 'Scheduled'

                    AND
                    (
                        CAST(a.AppointmentDate AS DATETIME)
                        +
                        CAST(a.AppointmentTime AS DATETIME)
                    ) < GETDATE()";


                var appointmentsToMark =
                    new List<Tuple<int, object>>();


                using (SqlCommand findCmd =
                    new SqlCommand(findSql, conn))
                {
                    using (SqlDataReader reader =
                        findCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            appointmentsToMark.Add(
                                new Tuple<int, object>(
                                    Convert.ToInt32(
                                        reader["AppointmentID"]
                                    ),
                                    reader["PatientUserID"]
                                )
                            );
                        }
                    }
                }


                foreach (var appointment
                    in appointmentsToMark)
                {
                    string updateSql = @"
                        UPDATE Appointments

                        SET AppointmentStatusID =
                        (
                            SELECT AppointmentStatusID

                            FROM AppointmentStatuses

                            WHERE StatusName = 'Missed'
                        )

                        WHERE AppointmentID =
                              @AppointmentID";


                    using (SqlCommand updateCmd =
                        new SqlCommand(updateSql, conn))
                    {
                        updateCmd.Parameters.Add(
                            "@AppointmentID",
                            SqlDbType.Int
                        ).Value =
                            appointment.Item1;

                        updateCmd.ExecuteNonQuery();
                    }


                    // Create notification for patient.
                    if (appointment.Item2 != DBNull.Value)
                    {
                        NotificationHelper.Create(
                            conn,
                            Convert.ToInt32(
                                appointment.Item2
                            ),
                            "Missed Appointment",
                            "You missed a scheduled appointment. " +
                            "Please contact reception to reschedule.",
                            "MissedAppointment"
                        );
                    }
                }
            }
        }


        // =========================================================
        // DASHBOARD SUMMARY CARDS
        // =========================================================

        private void LoadSummaryCards()
        {
            using (SqlConnection conn =
                DatabaseHelper.GetConnection())
            {
                conn.Open();


                // Today's appointments
                lblTodaysAppointments.Text =
                    RunScalar(
                        conn,
                        @"
                        SELECT COUNT(*)

                        FROM Appointments

                        WHERE AppointmentDate =
                              CAST(GETDATE() AS DATE)"
                    );


                // Registered patients
                lblRegisteredPatients.Text =
                    RunScalar(
                        conn,
                        @"
                        SELECT COUNT(*)

                        FROM Patients

                        WHERE IsActive = 1"
                    );


                // Low stock medicines
                lblLowStock.Text =
                    RunScalar(
                        conn,
                        @"
                        SELECT COUNT(*)

                        FROM Medications

                        WHERE QuantityInStock <= ReorderLevel
                        AND IsActive = 1"
                    );


                // Pending medical aid claims
                lblPendingClaims.Text =
                    RunScalar(
                        conn,
                        @"
                        SELECT COUNT(*)

                        FROM MedicalAidClaims c

                        INNER JOIN ClaimStatuses s
                            ON c.ClaimStatusID =
                               s.ClaimStatusID

                        WHERE s.StatusName IN
                        (
                            'Pending',
                            'Submitted',
                            'Under Review'
                        )"
                    );


                // Today's revenue
                lblTodaysRevenue.Text =
                    RunScalar(
                        conn,
                        @"
                        SELECT
                            ISNULL(
                                SUM(AmountPaid),
                                0
                            )

                        FROM Payments

                        WHERE CAST(
                                  PaymentDate AS DATE
                              )
                              =
                              CAST(
                                  GETDATE() AS DATE
                              )",
                        "0.00"
                    );


                // Missed appointments
                lblMissedAppointments.Text =
                    RunScalar(
                        conn,
                        @"
                        SELECT COUNT(*)

                        FROM Appointments a

                        INNER JOIN AppointmentStatuses s
                            ON a.AppointmentStatusID =
                               s.AppointmentStatusID

                        WHERE s.StatusName = 'Missed'"
                    );


                // Completed consultations
                lblCompletedConsultations.Text =
                    RunScalar(
                        conn,
                        @"
                        SELECT COUNT(*)

                        FROM Consultations

                        WHERE Status = 'Completed'"
                    );


                // Outstanding bills
                lblOutstandingBills.Text =
                    RunScalar(
                        conn,
                        @"
                        SELECT
                            ISNULL(
                                SUM(OutstandingAmount),
                                0
                            )

                        FROM Bills

                        WHERE OutstandingAmount > 0",
                        "0.00"
                    );
            }
        }


        // =========================================================
        // RUN SINGLE-VALUE SQL QUERY
        // =========================================================

        private string RunScalar(
            SqlConnection conn,
            string sql,
            string format = null)
        {
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                object result =
                    cmd.ExecuteScalar();


                if (result == null ||
                    result == DBNull.Value)
                {
                    if (format == "0.00")
                    {
                        return "0.00";
                    }

                    return "0";
                }


                if (format == "0.00")
                {
                    decimal amount =
                        Convert.ToDecimal(result);

                    return amount.ToString("N2");
                }


                return result.ToString();
            }
        }


        // =========================================================
        // LOAD TODAY'S APPOINTMENTS
        // =========================================================

        private void LoadTodaysAppointments()
        {
            using (SqlConnection conn =
                DatabaseHelper.GetConnection())
            {
                string sql = @"
                    SELECT

                        p.PatientNumber
                        + ' - '
                        + u1.FirstName
                        + ' '
                        + u1.Surname
                        AS PatientName,


                        u2.FirstName
                        + ' '
                        + u2.Surname
                        AS DoctorName,


                        a.AppointmentTime,


                        s.StatusName


                    FROM Appointments a


                    INNER JOIN Patients p
                        ON a.PatientID =
                           p.PatientID


                    INNER JOIN Users u1
                        ON p.UserID =
                           u1.UserID


                    INNER JOIN Doctors d
                        ON a.DoctorID =
                           d.DoctorID


                    INNER JOIN Users u2
                        ON d.UserID =
                           u2.UserID


                    INNER JOIN AppointmentStatuses s
                        ON a.AppointmentStatusID =
                           s.AppointmentStatusID


                    WHERE a.AppointmentDate =
                          CAST(GETDATE() AS DATE)


                    ORDER BY
                        a.AppointmentTime";


                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    conn.Open();


                    using (SqlDataAdapter adapter =
                        new SqlDataAdapter(cmd))
                    {
                        DataTable dt =
                            new DataTable();

                        adapter.Fill(dt);


                        gvTodaysAppointments.DataSource =
                            dt;

                        gvTodaysAppointments.DataBind();
                    }
                }
            }
        }


        // =========================================================
        // FORMAT APPOINTMENT TIME
        // =========================================================

        public string FormatAppointmentTime(
            object value)
        {
            if (value == null ||
                value == DBNull.Value)
            {
                return "";
            }


            // SQL TIME normally becomes
            // System.TimeSpan in C#.
            if (value is TimeSpan)
            {
                TimeSpan time =
                    (TimeSpan)value;

                DateTime displayTime =
                    DateTime.Today.Add(time);

                return displayTime.ToString(
                    "hh:mm tt"
                );
            }


            // This is included in case the database
            // ever returns DateTime instead.
            if (value is DateTime)
            {
                DateTime dateTime =
                    (DateTime)value;

                return dateTime.ToString(
                    "hh:mm tt"
                );
            }


            // Final fallback.
            TimeSpan parsedTime;

            if (TimeSpan.TryParse(
                value.ToString(),
                out parsedTime))
            {
                return DateTime.Today
                    .Add(parsedTime)
                    .ToString("hh:mm tt");
            }


            return value.ToString();
        }


        // =========================================================
        // FORMAT STATUS BADGE
        // =========================================================

        public string GetStatusCssClass(
            object statusValue)
        {
            string status =
                statusValue == null
                    ? ""
                    : statusValue
                        .ToString()
                        .Trim()
                        .ToLower();


            switch (status)
            {
                case "scheduled":
                    return "badge bg-primary";

                case "checked in":
                case "checkedin":
                    return "badge bg-info text-dark";

                case "completed":
                    return "badge bg-success";

                case "missed":
                    return "badge bg-danger";

                case "cancelled":
                case "canceled":
                    return "badge bg-secondary";

                default:
                    return "badge bg-secondary";
            }
        }


        // =========================================================
        // LOAD LOW-STOCK MEDICATIONS
        // =========================================================

        private void LoadLowStock()
        {
            using (SqlConnection conn =
                DatabaseHelper.GetConnection())
            {
                string sql = @"
                    SELECT

                        MedicationName,

                        QuantityInStock,

                        ReorderLevel

                    FROM Medications

                    WHERE
                        QuantityInStock <= ReorderLevel

                    AND IsActive = 1

                    ORDER BY
                        QuantityInStock ASC";


                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    conn.Open();


                    using (SqlDataAdapter adapter =
                        new SqlDataAdapter(cmd))
                    {
                        DataTable dt =
                            new DataTable();

                        adapter.Fill(dt);


                        gvLowStock.DataSource =
                            dt;

                        gvLowStock.DataBind();
                    }
                }
            }
        }
    }
}
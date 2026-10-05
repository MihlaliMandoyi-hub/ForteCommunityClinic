<%@ Page Title="Dashboard"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="ForteCommunityClinic.Dashboard" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <h3 class="clinic-page-title">
        Welcome to Forte Community Clinic
    </h3>

    <div class="row g-3 mb-3">

        <div class="col-md-3">
            <div class="clinic-card text-center">
                <div class="text-muted">
                    Today's Appointments
                </div>

                <div class="fs-3 fw-bold">
                    <asp:Label
                        ID="lblTodaysAppointments"
                        runat="server"
                        Text="0" />
                </div>
            </div>
        </div>

        <div class="col-md-3">
            <div class="clinic-card text-center">
                <div class="text-muted">
                    Registered Patients
                </div>

                <div class="fs-3 fw-bold">
                    <asp:Label
                        ID="lblRegisteredPatients"
                        runat="server"
                        Text="0" />
                </div>
            </div>
        </div>

        <div class="col-md-3">
            <div class="clinic-card text-center">
                <div class="text-muted">
                    Low Stock Items
                </div>

                <div class="fs-3 fw-bold text-danger">
                    <asp:Label
                        ID="lblLowStock"
                        runat="server"
                        Text="0" />
                </div>
            </div>
        </div>

        <div class="col-md-3">
            <div class="clinic-card text-center">
                <div class="text-muted">
                    Pending Claims
                </div>

                <div class="fs-3 fw-bold">
                    <asp:Label
                        ID="lblPendingClaims"
                        runat="server"
                        Text="0" />
                </div>
            </div>
        </div>

    </div>

    <div class="row g-3 mb-4">

        <div class="col-md-3">
            <div class="clinic-card text-center">

                <div class="text-muted">
                    Today's Revenue
                </div>

                <div class="fs-3 fw-bold text-success">
                    R<asp:Label
                        ID="lblTodaysRevenue"
                        runat="server"
                        Text="0.00" />
                </div>

            </div>
        </div>

        <div class="col-md-3">
            <div class="clinic-card text-center">

                <div class="text-muted">
                    Missed Appointments
                </div>

                <div class="fs-3 fw-bold">
                    <asp:Label
                        ID="lblMissedAppointments"
                        runat="server"
                        Text="0" />
                </div>

            </div>
        </div>

        <div class="col-md-3">
            <div class="clinic-card text-center">

                <div class="text-muted">
                    Completed Consultations
                </div>

                <div class="fs-3 fw-bold">
                    <asp:Label
                        ID="lblCompletedConsultations"
                        runat="server"
                        Text="0" />
                </div>

            </div>
        </div>

        <div class="col-md-3">
            <div class="clinic-card text-center">

                <div class="text-muted">
                    Outstanding Bills
                </div>

                <div class="fs-3 fw-bold text-warning">
                    R<asp:Label
                        ID="lblOutstandingBills"
                        runat="server"
                        Text="0.00" />
                </div>

            </div>
        </div>

    </div>

    <div class="clinic-card mb-4">

        <h5 class="clinic-page-title">
            Today's Appointments
        </h5>

        <asp:GridView
            ID="gvTodaysAppointments"
            runat="server"
            CssClass="table table-hover"
            AutoGenerateColumns="false"
            EmptyDataText="No appointments today.">

            <Columns>

                <asp:BoundField
                    DataField="PatientName"
                    HeaderText="Patient" />

                <asp:BoundField
                    DataField="DoctorName"
                    HeaderText="Doctor" />

                <asp:TemplateField HeaderText="Time">
                    <ItemTemplate>
                        <%# FormatAppointmentTime(Eval("AppointmentTime")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='<%# GetStatusCssClass(Eval("StatusName")) %>'>
                            <%# Eval("StatusName") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>

        </asp:GridView>

    </div>

    <div class="clinic-card">

        <h5 class="clinic-page-title">
            Low Stock Medications
        </h5>

        <asp:GridView
            ID="gvLowStock"
            runat="server"
            CssClass="table table-hover"
            AutoGenerateColumns="false"
            EmptyDataText="No low-stock medications.">

            <Columns>

                <asp:BoundField
                    DataField="MedicationName"
                    HeaderText="Medication" />

                <asp:BoundField
                    DataField="QuantityInStock"
                    HeaderText="Available" />

                <asp:BoundField
                    DataField="ReorderLevel"
                    HeaderText="Reorder Level" />

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class="badge bg-danger">
                            LOW STOCK
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>

        </asp:GridView>

    </div>

</asp:Content>
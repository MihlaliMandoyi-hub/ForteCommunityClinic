<%@ Page Title="Daily Appointments Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DailyAppointments.aspx.cs" Inherits="ForteCommunityClinic.Reports.DailyAppointments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Daily Appointments Report</h3>

    <div class="clinic-card mb-3">
        <div class="row g-2">
            <div class="col-md-3">
                <asp:TextBox ID="txtReportDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnRun" runat="server" Text="Run Report" CssClass="btn btn-primary" OnClick="btnRun_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <div class="clinic-card">
        <h5 class="clinic-page-title">Summary</h5>
        <div class="row g-3 mb-3">
            <div class="col-md-3"><div class="clinic-card text-center"><div class="text-muted">Total</div><div class="fs-4 fw-bold"><asp:Label ID="lblTotal" runat="server" /></div></div></div>
            <div class="col-md-3"><div class="clinic-card text-center"><div class="text-muted">Completed</div><div class="fs-4 fw-bold text-success"><asp:Label ID="lblCompleted" runat="server" /></div></div></div>
            <div class="col-md-3"><div class="clinic-card text-center"><div class="text-muted">Cancelled</div><div class="fs-4 fw-bold text-danger"><asp:Label ID="lblCancelled" runat="server" /></div></div></div>
            <div class="col-md-3"><div class="clinic-card text-center"><div class="text-muted">Missed</div><div class="fs-4 fw-bold text-warning"><asp:Label ID="lblMissed" runat="server" /></div></div></div>
        </div>

        <asp:GridView ID="gvAppointments" runat="server" CssClass="table table-hover" AutoGenerateColumns="false" EmptyDataText="No appointments for this date.">
            <Columns>
                <asp:BoundField DataField="AppointmentTime" HeaderText="Time" DataFormatString="{0:hh\:mm}" />
                <asp:BoundField DataField="PatientName" HeaderText="Patient" />
                <asp:BoundField DataField="DoctorName" HeaderText="Doctor" />
                <asp:BoundField DataField="StatusName" HeaderText="Status" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
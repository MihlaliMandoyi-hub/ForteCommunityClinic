<%@ Page Title="Reschedule Appointment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AppointmentReschedule.aspx.cs" Inherits="ForteCommunityClinic.Appointments.AppointmentReschedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Reschedule Appointment</h3>
    <div class="clinic-card" style="max-width: 500px;">
        <asp:Label ID="lblAppointmentInfo" runat="server" CssClass="mb-3 d-block fw-bold" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="row g-3">
            <div class="col-md-6">
                <label>New Date</label>
                <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date" />
                <asp:RequiredFieldValidator ControlToValidate="txtDate" runat="server" ErrorMessage="Date is required." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-6">
                <label>New Time</label>
                <asp:TextBox ID="txtTime" runat="server" CssClass="form-control" TextMode="Time" />
                <asp:RequiredFieldValidator ControlToValidate="txtTime" runat="server" ErrorMessage="Time is required." CssClass="text-danger" Display="Dynamic" />
            </div>
        </div>

        <asp:Button ID="btnReschedule" runat="server" Text="Reschedule" CssClass="btn btn-primary mt-3" OnClick="btnReschedule_Click" />
        <a href="~/Appointments/AppointmentList.aspx" runat="server" class="btn btn-outline-secondary mt-3">Cancel</a>
    </div>
</asp:Content>
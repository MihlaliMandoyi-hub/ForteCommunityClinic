<%@ Page Title="Book Appointment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BookAppointment.aspx.cs" Inherits="ForteCommunityClinic.Appointments.BookAppointment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">📅 Book an Appointment</h3>
    <div class="clinic-card" style="max-width: 500px;">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="mb-3">
            <label>Doctor</label>
            <asp:DropDownList ID="ddlDoctor" runat="server" CssClass="form-select" />
        </div>

        <div class="row g-3">
            <div class="col-md-6">
                <label>Date</label>
                <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date" />
                <asp:RequiredFieldValidator ControlToValidate="txtDate" runat="server" ErrorMessage="Date is required." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-6">
                <label>Time</label>
                <asp:TextBox ID="txtTime" runat="server" CssClass="form-control" TextMode="Time" />
                <asp:RequiredFieldValidator ControlToValidate="txtTime" runat="server" ErrorMessage="Time is required." CssClass="text-danger" Display="Dynamic" />
            </div>
        </div>

        <div class="mb-3 mt-3">
            <label>Reason for visit</label>
            <asp:TextBox ID="txtReason" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Book Appointment" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <a href="~/Appointments/MyAppointments.aspx" runat="server" class="btn btn-outline-secondary">Cancel</a>
    </div>
</asp:Content>
<%@ Page Title="New Appointment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AppointmentCreate.aspx.cs" Inherits="ForteCommunityClinic.Appointments.AppointmentCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">New Appointment</h3>
    <div class="clinic-card" style="max-width: 600px;">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="mb-3">
            <label>Patient</label>
            <div class="input-group">
                <asp:TextBox ID="txtPatientSearch" runat="server" CssClass="form-control" placeholder="Search by patient number or surname" />
                <asp:Button ID="btnFindPatient" runat="server" Text="Find" CssClass="btn btn-outline-secondary" OnClick="btnFindPatient_Click" CausesValidation="false" />
            </div>
            <asp:DropDownList ID="ddlPatient" runat="server" CssClass="form-select mt-2" />
            <asp:CustomValidator ID="cvPatient" runat="server" ErrorMessage="Please find and select a patient." CssClass="text-danger" Display="Dynamic" OnServerValidate="cvPatient_ServerValidate" />
        </div>

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
            <label>Reason</label>
            <asp:TextBox ID="txtReason" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Book Appointment" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <a href="~/Appointments/AppointmentList.aspx" runat="server" class="btn btn-outline-secondary">Cancel</a>
    </div>
</asp:Content>
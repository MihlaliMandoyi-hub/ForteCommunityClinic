<%@ Page Title="New Consultation" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultationCreate.aspx.cs" Inherits="ForteCommunityClinic.Consultations.ConsultationCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">New Consultation</h3>
    <div class="clinic-card" style="max-width: 700px;">
        <asp:Label ID="lblPatientInfo" runat="server" CssClass="mb-3 d-block fw-bold" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="mb-3">
            <label>Symptoms</label>
            <asp:TextBox ID="txtSymptoms" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
            <asp:RequiredFieldValidator ControlToValidate="txtSymptoms" runat="server" ErrorMessage="Symptoms are required." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="mb-3">
            <label>Clinical Notes</label>
            <asp:TextBox ID="txtClinicalNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
        </div>
        <div class="mb-3">
            <label>Diagnosis (one per line if more than one)</label>
            <asp:TextBox ID="txtDiagnoses" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
            <asp:RequiredFieldValidator ControlToValidate="txtDiagnoses" runat="server" ErrorMessage="At least one diagnosis is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Save Consultation" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <a href="~/Appointments/AppointmentList.aspx" runat="server" class="btn btn-outline-secondary">Cancel</a>
    </div>
</asp:Content>
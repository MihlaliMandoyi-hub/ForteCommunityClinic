<%@ Page Title="Patient Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PatientDetails.aspx.cs" Inherits="ForteCommunityClinic.Patients.PatientDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Patient Details</h3>
    <div class="clinic-card" style="max-width: 600px;">
        <asp:Label ID="lblNotFound" runat="server" CssClass="alert alert-warning d-block" Visible="false" Text="Patient not found." />

        <asp:Panel ID="pnlDetails" runat="server">
            <dl class="row">
                <dt class="col-sm-4">Patient Number</dt><dd class="col-sm-8"><asp:Literal ID="litPatientNumber" runat="server" /></dd>
                <dt class="col-sm-4">Full Name</dt><dd class="col-sm-8"><asp:Literal ID="litFullName" runat="server" /></dd>
                <dt class="col-sm-4">Date of Birth</dt><dd class="col-sm-8"><asp:Literal ID="litDOB" runat="server" /></dd>
                <dt class="col-sm-4">Gender</dt><dd class="col-sm-8"><asp:Literal ID="litGender" runat="server" /></dd>
                <dt class="col-sm-4">Email</dt><dd class="col-sm-8"><asp:Literal ID="litEmail" runat="server" /></dd>
                <dt class="col-sm-4">Phone</dt><dd class="col-sm-8"><asp:Literal ID="litPhone" runat="server" /></dd>
                <dt class="col-sm-4">ID Number</dt><dd class="col-sm-8"><asp:Literal ID="litIDNumber" runat="server" /></dd>
                <dt class="col-sm-4">Address</dt><dd class="col-sm-8"><asp:Literal ID="litAddress" runat="server" /></dd>
                <dt class="col-sm-4">Province</dt><dd class="col-sm-8"><asp:Literal ID="litProvince" runat="server" /></dd>
                <dt class="col-sm-4">Emergency Contact</dt><dd class="col-sm-8"><asp:Literal ID="litEmergency" runat="server" /></dd>
                <dt class="col-sm-4">Registered</dt><dd class="col-sm-8"><asp:Literal ID="litRegistered" runat="server" /></dd>
            </dl>
            <a href="~/Patients/PatientList.aspx" runat="server" class="btn btn-outline-secondary">Back to List</a>
            <asp:HyperLink ID="lnkEdit" runat="server" CssClass="btn btn-primary">Edit</asp:HyperLink>
        </asp:Panel>
    </div>
</asp:Content>
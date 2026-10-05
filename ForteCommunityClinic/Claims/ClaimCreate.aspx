<%@ Page Title="New Claim" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClaimCreate.aspx.cs" Inherits="ForteCommunityClinic.Claims.ClaimCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">New Medical Aid Claim</h3>
    <div class="clinic-card" style="max-width: 600px;">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="mb-3">
            <label>Patient</label>
            <div class="input-group">
                <asp:TextBox ID="txtPatientSearch" runat="server" CssClass="form-control" placeholder="Search by patient number or surname" />
                <asp:Button ID="btnFindPatient" runat="server" Text="Find" CssClass="btn btn-outline-secondary" OnClick="btnFindPatient_Click" CausesValidation="false" />
            </div>
            <asp:DropDownList ID="ddlPatient" runat="server" CssClass="form-select mt-2" AutoPostBack="true" OnSelectedIndexChanged="ddlPatient_SelectedIndexChanged" />
        </div>

        <div class="mb-3">
            <label>Bill</label>
            <asp:DropDownList ID="ddlBill" runat="server" CssClass="form-select" />
            <small class="text-muted">Only unpaid or partially paid bills for this patient are shown.</small>
        </div>

        <div class="mb-3">
            <label>Medical Aid Provider</label>
            <asp:DropDownList ID="ddlProvider" runat="server" CssClass="form-select" />
        </div>

        <div class="mb-3">
            <label>Membership Number</label>
            <asp:TextBox ID="txtMembershipNumber" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ControlToValidate="txtMembershipNumber" runat="server" ErrorMessage="Membership number is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="mb-3">
            <label>Claim Amount</label>
            <asp:TextBox ID="txtClaimAmount" runat="server" CssClass="form-control" TextMode="Number" />
            <asp:RequiredFieldValidator ControlToValidate="txtClaimAmount" runat="server" ErrorMessage="Claim amount is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Submit Claim" CssClass="btn btn-primary" OnClick="btnSave_Click" />
        <a href="~/Claims/ClaimList.aspx" runat="server" class="btn btn-outline-secondary">Cancel</a>
    </div>
</asp:Content>

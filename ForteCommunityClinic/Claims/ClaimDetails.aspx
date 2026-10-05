<%@ Page Title="Claim Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClaimDetails.aspx.cs" Inherits="ForteCommunityClinic.Claims.ClaimDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Claim Details</h3>
    <div class="clinic-card" style="max-width: 600px;">
        <dl class="row">
            <dt class="col-sm-4">Patient</dt><dd class="col-sm-8"><asp:Literal ID="litPatient" runat="server" /></dd>
            <dt class="col-sm-4">Bill</dt><dd class="col-sm-8"><asp:Literal ID="litBill" runat="server" /></dd>
            <dt class="col-sm-4">Provider</dt><dd class="col-sm-8"><asp:Literal ID="litProvider" runat="server" /></dd>
            <dt class="col-sm-4">Membership #</dt><dd class="col-sm-8"><asp:Literal ID="litMembership" runat="server" /></dd>
            <dt class="col-sm-4">Claim Amount</dt><dd class="col-sm-8"><asp:Literal ID="litAmount" runat="server" /></dd>
            <dt class="col-sm-4">Submitted</dt><dd class="col-sm-8"><asp:Literal ID="litSubmitted" runat="server" /></dd>
            <dt class="col-sm-4">Current Status</dt><dd class="col-sm-8"><asp:Literal ID="litStatus" runat="server" /></dd>
        </dl>

        <hr />
        <h5 class="clinic-page-title">Update Status</h5>
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="mb-3">
            <label>New Status</label>
            <asp:DropDownList ID="ddlNewStatus" runat="server" CssClass="form-select" />
        </div>
        <asp:Panel ID="pnlRejectionReason" runat="server">
            <div class="mb-3">
                <label>Rejection Reason</label>
                <asp:TextBox ID="txtRejectionReason" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
            </div>
        </asp:Panel>

        <asp:Button ID="btnUpdateStatus" runat="server" Text="Update Status" CssClass="btn btn-primary" OnClick="btnUpdateStatus_Click" CausesValidation="false" />
        <a href="~/Claims/ClaimList.aspx" runat="server" class="btn btn-outline-secondary">Back to List</a>
    </div>
</asp:Content>
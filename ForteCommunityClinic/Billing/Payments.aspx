<%@ Page Title="Record Payment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Payments.aspx.cs" Inherits="ForteCommunityClinic.Billing.Payments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Record Payment</h3>
    <div class="clinic-card" style="max-width: 500px;">
        <asp:Label ID="lblBillInfo" runat="server" CssClass="mb-3 d-block fw-bold" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="mb-3">
            <label>Payment Method</label>
            <asp:DropDownList ID="ddlPaymentMethod" runat="server" CssClass="form-select" />
        </div>
        <div class="mb-3">
            <label>Amount</label>
            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" TextMode="Number" />
            <asp:RequiredFieldValidator ControlToValidate="txtAmount" runat="server" ErrorMessage="Amount is required." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="mb-3">
            <label>Reference Number</label>
            <asp:TextBox ID="txtReference" runat="server" CssClass="form-control" placeholder="e.g. EFT reference or card slip #" />
        </div>

        <asp:Button ID="btnRecordPayment" runat="server" Text="Record Payment" CssClass="btn btn-primary" OnClick="btnRecordPayment_Click" />
        <a id="lnkBack" runat="server" class="btn btn-outline-secondary">Back to Bill</a>
    </div>
</asp:Content>
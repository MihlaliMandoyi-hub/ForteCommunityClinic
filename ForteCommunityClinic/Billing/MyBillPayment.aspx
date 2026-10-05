<%@ Page Title="Make a Payment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyBillPayment.aspx.cs" Inherits="ForteCommunityClinic.Billing.MyBillPayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Make a Payment</h3>
    <div class="clinic-card" style="max-width: 500px;">
        <asp:Label ID="lblBillInfo" runat="server" CssClass="mb-3 d-block fw-bold" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="mb-3">
            <label>Payment Method</label>
            <asp:DropDownList ID="ddlPaymentMethod" runat="server" CssClass="form-select" />
            <small class="text-muted">This is a simulated payment for demonstration purposes.</small>
        </div>
        <div class="mb-3">
            <label>Amount</label>
            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" TextMode="Number" />
            <asp:RequiredFieldValidator ControlToValidate="txtAmount" runat="server" ErrorMessage="Amount is required." CssClass="text-danger" Display="Dynamic" />
        </div>

        <asp:Button ID="btnPay" runat="server" Text="Pay Now" CssClass="btn btn-primary" OnClick="btnPay_Click" />
        <a id="lnkBack" runat="server" class="btn btn-outline-secondary">Cancel</a>
    </div>
</asp:Content>

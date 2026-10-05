<%@ Page Title="Receipt" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Receipt.aspx.cs" Inherits="ForteCommunityClinic.Billing.Receipt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Payment Receipt</h3>
    <div class="clinic-card" style="max-width: 500px;">
        <dl class="row">
            <dt class="col-sm-4">Reference</dt><dd class="col-sm-8"><asp:Literal ID="litReference" runat="server" /></dd>
            <dt class="col-sm-4">Patient</dt><dd class="col-sm-8"><asp:Literal ID="litPatient" runat="server" /></dd>
            <dt class="col-sm-4">Amount Paid</dt><dd class="col-sm-8"><asp:Literal ID="litAmount" runat="server" /></dd>
            <dt class="col-sm-4">Method</dt><dd class="col-sm-8"><asp:Literal ID="litMethod" runat="server" /></dd>
            <dt class="col-sm-4">Date</dt><dd class="col-sm-8"><asp:Literal ID="litDate" runat="server" /></dd>
            <dt class="col-sm-4">Received By</dt><dd class="col-sm-8"><asp:Literal ID="litReceivedBy" runat="server" /></dd>
        </dl>
        <a id="lnkBackToBill" runat="server" class="btn btn-primary">Back to Bill</a>
    </div>
</asp:Content>
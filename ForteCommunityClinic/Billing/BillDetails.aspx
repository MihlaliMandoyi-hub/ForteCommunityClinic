<%@ Page Title="Bill Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BillDetails.aspx.cs" Inherits="ForteCommunityClinic.Billing.BillDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Bill Details</h3>
    <div class="clinic-card mb-3" style="max-width: 700px;">
        <dl class="row">
            <dt class="col-sm-3">Patient</dt><dd class="col-sm-9"><asp:Literal ID="litPatient" runat="server" /></dd>
            <dt class="col-sm-3">Bill Date</dt><dd class="col-sm-9"><asp:Literal ID="litDate" runat="server" /></dd>
            <dt class="col-sm-3">Status</dt><dd class="col-sm-9"><asp:Literal ID="litStatus" runat="server" /></dd>
            <dt class="col-sm-3">Total</dt><dd class="col-sm-9"><asp:Literal ID="litTotal" runat="server" /></dd>
            <dt class="col-sm-3">Paid</dt><dd class="col-sm-9"><asp:Literal ID="litPaid" runat="server" /></dd>
            <dt class="col-sm-3">Outstanding</dt><dd class="col-sm-9"><asp:Literal ID="litOutstanding" runat="server" /></dd>
        </dl>

        <h5 class="clinic-page-title">Items</h5>
        <asp:GridView ID="gvBillItems" runat="server" CssClass="table" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="LineTotal" HeaderText="Total" DataFormatString="{0:C2}" />
            </Columns>
        </asp:GridView>

        <h5 class="clinic-page-title">Payments</h5>
        <asp:GridView ID="gvPayments" runat="server" CssClass="table" AutoGenerateColumns="false" EmptyDataText="No payments recorded yet.">
            <Columns>
                <asp:BoundField DataField="PaymentDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="MethodName" HeaderText="Method" />
                <asp:BoundField DataField="AmountPaid" HeaderText="Amount" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="ReferenceNumber" HeaderText="Reference" />
            </Columns>
        </asp:GridView>

        <a id="lnkRecordPayment" runat="server" class="btn btn-primary mt-3">Record Payment</a>
        <a href="~/Billing/BillCreate.aspx" runat="server" class="btn btn-outline-secondary mt-3">New Bill</a>
    </div>
</asp:Content>
<%@ Page Title="Bill Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyBillDetails.aspx.cs" Inherits="ForteCommunityClinic.Billing.MyBillDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Bill Details</h3>
    <asp:Label ID="lblNotFound" runat="server" CssClass="alert alert-warning d-block" Visible="false" Text="Bill not found." />

    <asp:Panel ID="pnlDetails" runat="server">
        <div class="clinic-card mb-3" style="max-width: 700px;">
            <dl class="row">
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

            <h5 class="clinic-page-title">Payment History</h5>
            <asp:GridView ID="gvPayments" runat="server" CssClass="table" AutoGenerateColumns="false" EmptyDataText="No payments recorded yet.">
                <Columns>
                    <asp:BoundField DataField="PaymentDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:BoundField DataField="MethodName" HeaderText="Method" />
                    <asp:BoundField DataField="AmountPaid" HeaderText="Amount" DataFormatString="{0:C2}" />
                    <asp:BoundField DataField="ReferenceNumber" HeaderText="Reference" />
                </Columns>
            </asp:GridView>

            <a id="lnkMakePayment" runat="server" class="btn btn-primary mt-3">Make a Payment</a>
            <a href="~/Billing/MyBills.aspx" runat="server" class="btn btn-outline-secondary mt-3">Back to My Bills</a>
        </div>
    </asp:Panel>
</asp:Content>
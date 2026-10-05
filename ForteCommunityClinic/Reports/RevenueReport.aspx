<%@ Page Title="Revenue Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RevenueReport.aspx.cs" Inherits="ForteCommunityClinic.Reports.RevenueReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Revenue Report</h3>

    <div class="clinic-card mb-3">
        <div class="row g-2">
            <div class="col-md-3">
                <label>From</label>
                <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-3">
                <label>To</label>
                <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-3 d-flex align-items-end">
                <asp:Button ID="btnRun" runat="server" Text="Run Report" CssClass="btn btn-primary" OnClick="btnRun_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <div class="row g-3 mb-3">
        <div class="col-md-4"><div class="clinic-card text-center"><div class="text-muted">Total Revenue (Period)</div><div class="fs-4 fw-bold text-success"><asp:Label ID="lblPeriodRevenue" runat="server" /></div></div></div>
        <div class="col-md-4"><div class="clinic-card text-center"><div class="text-muted">Today's Revenue</div><div class="fs-4 fw-bold"><asp:Label ID="lblTodayRevenue" runat="server" /></div></div></div>
        <div class="col-md-4"><div class="clinic-card text-center"><div class="text-muted">This Month's Revenue</div><div class="fs-4 fw-bold"><asp:Label ID="lblMonthRevenue" runat="server" /></div></div></div>
    </div>

    <div class="clinic-card mb-3">
        <h5 class="clinic-page-title">Revenue by Payment Method</h5>
        <asp:GridView ID="gvByMethod" runat="server" CssClass="table" AutoGenerateColumns="false" EmptyDataText="No payments in this period.">
            <Columns>
                <asp:BoundField DataField="MethodName" HeaderText="Method" />
                <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="PaymentCount" HeaderText="# Payments" />
            </Columns>
        </asp:GridView>
    </div>

    <div class="clinic-card">
        <h5 class="clinic-page-title">Payment Detail</h5>
        <asp:GridView ID="gvPayments" runat="server" CssClass="table table-hover" AutoGenerateColumns="false" EmptyDataText="No payments in this period.">
            <Columns>
                <asp:BoundField DataField="PaymentDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="PatientName" HeaderText="Patient" />
                <asp:BoundField DataField="MethodName" HeaderText="Method" />
                <asp:BoundField DataField="AmountPaid" HeaderText="Amount" DataFormatString="{0:C2}" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

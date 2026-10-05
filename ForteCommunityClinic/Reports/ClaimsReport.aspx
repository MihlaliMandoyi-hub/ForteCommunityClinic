<%@ Page Title="Claims Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClaimsReport.aspx.cs" Inherits="ForteCommunityClinic.Reports.ClaimsReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Medical Aid Claims Report</h3>

    <div class="clinic-card mb-3">
        <h5 class="clinic-page-title">Claims by Status</h5>
        <asp:GridView ID="gvByStatus" runat="server" CssClass="table" AutoGenerateColumns="false" EmptyDataText="No claims on file.">
            <Columns>
                <asp:BoundField DataField="StatusName" HeaderText="Status" />
                <asp:BoundField DataField="ClaimCount" HeaderText="# Claims" />
                <asp:BoundField DataField="TotalAmount" HeaderText="Total Amount" DataFormatString="{0:C2}" />
            </Columns>
        </asp:GridView>
    </div>

    <div class="clinic-card">
        <h5 class="clinic-page-title">Outstanding Bills</h5>
        <asp:GridView ID="gvOutstanding" runat="server" CssClass="table table-hover" AutoGenerateColumns="false" EmptyDataText="No outstanding bills.">
            <Columns>
                <asp:BoundField DataField="PatientName" HeaderText="Patient" />
                <asp:BoundField DataField="BillDate" HeaderText="Bill Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="OutstandingAmount" HeaderText="Outstanding" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="Status" HeaderText="Status" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

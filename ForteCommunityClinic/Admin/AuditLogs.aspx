<%@ Page Title="Audit Logs" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AuditLogs.aspx.cs" Inherits="ForteCommunityClinic.Admin.AuditLogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="clinic-page-title mb-0">📝 Audit Logs</h3>
        <asp:Button ID="btnExport" runat="server" Text="⬇ Export to CSV" CssClass="btn btn-outline-primary" OnClick="btnExport_Click" CausesValidation="false" />
    </div>

    <div class="clinic-card mb-3">
        <div class="row g-2">
            <div class="col-md-3">
                <asp:TextBox ID="txtFilterUser" runat="server" CssClass="form-control" placeholder="Username" />
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlFilterAction" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtFilterDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-primary" OnClick="btnFilter_Click" CausesValidation="false" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-outline-secondary" OnClick="btnClear_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <div class="clinic-card">
        <asp:GridView ID="gvAuditLogs" runat="server" CssClass="table table-hover" AutoGenerateColumns="false" EmptyDataText="No audit log entries found.">
            <Columns>
                <asp:BoundField DataField="CreatedDate" HeaderText="Date/Time" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                <asp:BoundField DataField="Username" HeaderText="User" />
                <asp:BoundField DataField="Action" HeaderText="Action" />
                <asp:BoundField DataField="EntityType" HeaderText="Entity" />
                <asp:BoundField DataField="EntityID" HeaderText="Entity ID" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:BoundField DataField="IPAddress" HeaderText="IP Address" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="ForteCommunityClinic.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h3 class="clinic-page-title mb-1"><asp:Literal ID="litTitle" runat="server" Mode="Encode" /></h3>
    <p class="text-muted mb-4"><asp:Literal ID="litSubtitle" runat="server" Mode="Encode" /></p>

    <div class="row g-3 mb-4">
        <asp:Repeater ID="rptCards" runat="server">
            <ItemTemplate>
                <div class="col-lg-3 col-sm-6">
                    <div class="clinic-card stat-card">
                        <div class="stat-label"><%#: Eval("Key") %></div>
                        <div class="stat-value"><%#: Eval("Value") %></div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <div class="clinic-card mb-4">
        <h5 class="clinic-page-title">Quick Actions</h5>
        <asp:Repeater ID="rptActions" runat="server">
            <ItemTemplate>
                <a class="btn btn-primary me-2 mb-2" href='<%# ResolveUrl(Eval("Value").ToString()) %>'><%#: Eval("Key") %></a>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <asp:Panel ID="pnlTable1" runat="server" CssClass="clinic-card mb-4" Visible="false">
        <h5 class="clinic-page-title"><asp:Literal ID="litTable1Title" runat="server" Mode="Encode" /></h5>
        <asp:GridView ID="gvTable1" runat="server" CssClass="table table-hover" AutoGenerateColumns="true"
            EmptyDataText="Nothing to show right now." GridLines="None" />
    </asp:Panel>

    <asp:Panel ID="pnlTable2" runat="server" CssClass="clinic-card mb-4" Visible="false">
        <h5 class="clinic-page-title"><asp:Literal ID="litTable2Title" runat="server" Mode="Encode" /></h5>
        <asp:GridView ID="gvTable2" runat="server" CssClass="table table-hover" AutoGenerateColumns="true"
            EmptyDataText="Nothing to show right now." GridLines="None" />
    </asp:Panel>

</asp:Content>
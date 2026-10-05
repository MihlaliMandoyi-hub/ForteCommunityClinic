<%@ Page Title="Staff Approvals" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StaffApprovals.aspx.cs" Inherits="ForteCommunityClinic.Admin.StaffApprovals" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">✅ Pending Staff Approvals</h3>
    <div class="clinic-card">
        <asp:GridView ID="gvPending" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            DataKeyNames="UserID" EmptyDataText="No staff accounts awaiting approval." OnRowCommand="gvPending_RowCommand">
            <Columns>
                <asp:BoundField DataField="FullName" HeaderText="Name" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="RoleName" HeaderText="Role" />
                <asp:BoundField DataField="CreatedDate" HeaderText="Requested" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button runat="server" Text="Approve" CssClass="btn btn-sm btn-success"
                            CommandName="Approve" CommandArgument='<%# Eval("UserID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
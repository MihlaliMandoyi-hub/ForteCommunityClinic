<%@ Page Title="Notifications" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NotificationList.aspx.cs" Inherits="ForteCommunityClinic.Notifications.NotificationList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Notifications</h3>
    <div class="clinic-card">
        <asp:GridView ID="gvNotifications" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            DataKeyNames="NotificationID" EmptyDataText="You have no notifications." OnRowCommand="gvNotifications_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <span class='badge <%# (bool)Eval("IsRead") ? "bg-secondary" : "bg-primary" %>'>
                            <%# (bool)Eval("IsRead") ? "Read" : "New" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="Message" HeaderText="Message" />
                <asp:BoundField DataField="CreatedDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:Button runat="server" Text="Mark as Read" CssClass="btn btn-sm btn-outline-secondary"
                            CommandName="MarkRead" CommandArgument='<%# Eval("NotificationID") %>'
                            Visible='<%# !(bool)Eval("IsRead") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
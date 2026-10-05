<%@ Page Title="Access Denied" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="ForteCommunityClinic.AccessDenied" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="clinic-card">
        <h3 class="clinic-page-title text-danger">Access Denied</h3>
        <p>You do not have permission to view this page.</p>
        <a href="~/Dashboard.aspx" runat="server" class="btn btn-primary">Back to Dashboard</a>
    </div>
</asp:Content>
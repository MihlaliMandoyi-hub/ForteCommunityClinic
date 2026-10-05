<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ForteCommunityClinic.Account.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="clinic-card login-card mx-auto">
        <div class="text-center mb-3">
            <i class="bi bi-hospital login-icon"></i>
            <h3 class="clinic-page-title mt-2 mb-1">Sign in</h3>
            <div class="text-muted small">PrecisionCare Health Clinic</div>
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />
        <asp:Label ID="lblRegistered" runat="server" CssClass="alert alert-success d-block" Visible="false" Text="Registration successful — please log in." />

        <asp:Panel runat="server" DefaultButton="btnLogin">
            <div class="mb-3">
                <label class="form-label">Username</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
            </div>
            <div class="mb-3">
                <label class="form-label">Password</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
            </div>
            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary w-100" OnClick="btnLogin_Click" />
        </asp:Panel>

        <p class="text-center text-muted small mt-3 mb-0">
            New patient? <a href="~/Account/Register.aspx" runat="server">Register here</a>
        </p>
    </div>
</asp:Content>
<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ForteCommunityClinic.Account.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="clinic-card mx-auto" style="max-width: 400px;">
        <h3 class="clinic-page-title">🏥 Login</h3>

        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />
        <asp:Label ID="lblRegistered" runat="server" CssClass="alert alert-success d-block" Visible="false" Text="Registration successful — please log in." />

        <div class="mb-3">
            <label>Username</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label>Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary w-100" OnClick="btnLogin_Click" />
        <p class="text-center mt-3">
            No account? <a href="~/Account/Register.aspx" runat="server">Register as a patient</a>
        </p>
        <p class="text-center">
            Are you clinic staff? <a href="~/Account/StaffRegister.aspx" runat="server">Register as staff</a>
        </p>
    </div>
</asp:Content>
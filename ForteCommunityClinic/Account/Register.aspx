<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="ForteCommunityClinic.Account.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="clinic-card mx-auto" style="max-width: 480px;">
        <h3 class="clinic-page-title">Patient Registration</h3>

        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="mb-3">
            <label>First Name</label>
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ControlToValidate="txtFirstName" runat="server" ErrorMessage="First name is required." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="mb-3">
            <label>Surname</label>
            <asp:TextBox ID="txtSurname" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ControlToValidate="txtSurname" runat="server" ErrorMessage="Surname is required." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="mb-3">
            <label>Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
            <asp:RequiredFieldValidator ControlToValidate="txtEmail" runat="server" ErrorMessage="Email is required." CssClass="text-danger" Display="Dynamic" />
            <asp:RegularExpressionValidator ControlToValidate="txtEmail" runat="server"
                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" ErrorMessage="Enter a valid email." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="mb-3">
            <label>Username</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ControlToValidate="txtUsername" runat="server" ErrorMessage="Username is required." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="mb-3">
            <label>Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
            <asp:RequiredFieldValidator ControlToValidate="txtPassword" runat="server" ErrorMessage="Password is required." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="mb-3">
            <label>Confirm Password</label>
            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" />
            <asp:CompareValidator ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword" runat="server"
                ErrorMessage="Passwords do not match." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="mb-3">
            <label>Date of Birth</label>
            <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" TextMode="Date" />
            <asp:RequiredFieldValidator ControlToValidate="txtDateOfBirth" runat="server" ErrorMessage="Date of birth is required." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="mb-3">
            <label>Gender</label>
            <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select" />
        </div>

        <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-primary w-100" OnClick="btnRegister_Click" />

        <p class="text-center mt-3">
            Already have an account? <a href="~/Account/Login.aspx" runat="server">Login</a>
        </p>
    </div>
</asp:Content>
<%@ Page Title="Staff Registration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StaffRegister.aspx.cs" Inherits="ForteCommunityClinic.Account.StaffRegister" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="clinic-card mx-auto" style="max-width: 480px;">
        <h3 class="clinic-page-title">🩺 Staff Registration</h3>
        <p class="text-muted">Your account will need to be approved by a receptionist before you can log in.</p>

        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />
        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success d-block" Visible="false">
            Registration submitted! Please wait for a receptionist to approve your account before logging in.
        </asp:Panel>

        <asp:Panel ID="pnlForm" runat="server">
            <div class="mb-3">
                <label>Role</label>
                <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Doctor" Value="Doctor" />
                    <asp:ListItem Text="Nurse" Value="Nurse" />
                </asp:DropDownList>
            </div>
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
                <label>Specialization <small class="text-muted">(Doctors only)</small></label>
                <asp:TextBox ID="txtSpecialization" runat="server" CssClass="form-control" />
            </div>

            <asp:Button ID="btnRegister" runat="server" Text="Submit Registration" CssClass="btn btn-primary w-100" OnClick="btnRegister_Click" />
        </asp:Panel>

        <p class="text-center mt-3">
            <a href="~/Account/Login.aspx" runat="server">Back to Login</a>
        </p>
    </div>
</asp:Content>
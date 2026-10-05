<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="ForteCommunityClinic.Account.MyProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">👤 My Profile</h3>
    <div class="clinic-card" style="max-width: 500px;">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success d-block" Visible="false" Text="Profile updated successfully." />

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
        </div>
        <div class="mb-3">
            <label>Phone Number</label>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label>Address</label>
            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label>Emergency Contact Name</label>
            <asp:TextBox ID="txtEmergencyName" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label>Emergency Contact Phone</label>
            <asp:TextBox ID="txtEmergencyPhone" runat="server" CssClass="form-control" />
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn btn-primary" OnClick="btnSave_Click" />
    </div>
</asp:Content>
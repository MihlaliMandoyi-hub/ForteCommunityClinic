<%@ Page Title="New Patient" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PatientCreate.aspx.cs" Inherits="ForteCommunityClinic.Patients.PatientCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">New Patient</h3>
    <div class="clinic-card" style="max-width: 600px;">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="row g-3">
            <div class="col-md-6">
                <label>First Name</label>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ControlToValidate="txtFirstName" runat="server" ErrorMessage="First name is required." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-6">
                <label>Surname</label>
                <asp:TextBox ID="txtSurname" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ControlToValidate="txtSurname" runat="server" ErrorMessage="Surname is required." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-6">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
            </div>
            <div class="col-md-6">
                <label>Phone Number</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6">
                <label>Date of Birth</label>
                <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" TextMode="Date" />
                <asp:RequiredFieldValidator ControlToValidate="txtDateOfBirth" runat="server" ErrorMessage="Date of birth is required." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-6">
                <label>Gender</label>
                <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-6">
                <label>ID Number</label>
                <asp:TextBox ID="txtIDNumber" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6">
                <label>Province</label>
                <asp:DropDownList ID="ddlProvince" runat="server" CssClass="form-select" />
            </div>
            <div class="col-12">
                <label>Address</label>
                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6">
                <label>Emergency Contact Name</label>
                <asp:TextBox ID="txtEmergencyName" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6">
                <label>Emergency Contact Phone</label>
                <asp:TextBox ID="txtEmergencyPhone" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="mt-3">
            <asp:Button ID="btnSave" runat="server" Text="Register Patient" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            <a href="~/Patients/PatientList.aspx" runat="server" class="btn btn-outline-secondary">Cancel</a>
        </div>
    </div>
</asp:Content>
<%@ Page Title="New Medication" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MedicationCreate.aspx.cs" Inherits="ForteCommunityClinic.Pharmacy.MedicationCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">New Medication</h3>
    <div class="clinic-card" style="max-width: 600px;">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <div class="row g-3">
            <div class="col-md-8">
                <label>Medication Name</label>
                <asp:TextBox ID="txtMedicationName" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ControlToValidate="txtMedicationName" runat="server" ErrorMessage="Medication name is required." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-4">
                <label>Category</label>
                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select" />
            </div>
            <div class="col-12">
                <label>Description</label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
            </div>
            <div class="col-md-4">
                <label>Initial Stock</label>
                <asp:TextBox ID="txtQuantityInStock" runat="server" CssClass="form-control" TextMode="Number" Text="0" />
            </div>
            <div class="col-md-4">
                <label>Reorder Level</label>
                <asp:TextBox ID="txtReorderLevel" runat="server" CssClass="form-control" TextMode="Number" Text="10" />
                <asp:RequiredFieldValidator ControlToValidate="txtReorderLevel" runat="server" ErrorMessage="Reorder level is required." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-4">
                <label>Unit Price</label>
                <asp:TextBox ID="txtUnitPrice" runat="server" CssClass="form-control" TextMode="Number" />
                <asp:RequiredFieldValidator ControlToValidate="txtUnitPrice" runat="server" ErrorMessage="Unit price is required." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-6">
                <label>Expiry Date</label>
                <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Save Medication" CssClass="btn btn-primary mt-3" OnClick="btnSave_Click" />
        <a href="~/Pharmacy/MedicationList.aspx" runat="server" class="btn btn-outline-secondary mt-3">Cancel</a>
    </div>
</asp:Content>

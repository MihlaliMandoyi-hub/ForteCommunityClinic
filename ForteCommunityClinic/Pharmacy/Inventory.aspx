<%@ Page Title="Receive Stock" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inventory.aspx.cs" Inherits="ForteCommunityClinic.Pharmacy.Inventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Receive Stock</h3>
    <div class="clinic-card" style="max-width: 600px;">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success d-block" Visible="false" Text="Stock received successfully." />

        <div class="mb-3">
            <label>Medication</label>
            <asp:DropDownList ID="ddlMedication" runat="server" CssClass="form-select" />
        </div>
        <div class="mb-3">
            <label>Quantity Received</label>
            <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" TextMode="Number" />
            <asp:RequiredFieldValidator ControlToValidate="txtQuantity" runat="server" ErrorMessage="Quantity is required." CssClass="text-danger" Display="Dynamic" />
            <asp:RangeValidator ControlToValidate="txtQuantity" runat="server" Type="Integer" MinimumValue="1" MaximumValue="1000000"
                ErrorMessage="Quantity must be greater than 0." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="mb-3">
            <label>Notes</label>
            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" placeholder="e.g. Supplier invoice #1234" />
        </div>

        <asp:Button ID="btnReceive" runat="server" Text="Receive Stock" CssClass="btn btn-primary" OnClick="btnReceive_Click" />
        <a href="~/Pharmacy/MedicationList.aspx" runat="server" class="btn btn-outline-secondary">Back</a>
    </div>
</asp:Content>

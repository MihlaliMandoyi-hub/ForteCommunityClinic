<%@ Page Title="New Prescription" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrescriptionCreate.aspx.cs" Inherits="ForteCommunityClinic.Consultations.PrescriptionCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">New Prescription</h3>
    <asp:Label ID="lblPatientInfo" runat="server" CssClass="mb-3 d-block fw-bold" />
    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

    <div class="clinic-card mb-3" style="max-width: 800px;">
        <h5 class="clinic-page-title">Add Medication</h5>
        <div class="row g-2">
            <div class="col-md-4">
                <asp:DropDownList ID="ddlMedication" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtDosage" runat="server" CssClass="form-control" placeholder="Dosage e.g. 500mg twice daily" />
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" TextMode="Number" placeholder="Qty" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtInstructions" runat="server" CssClass="form-control" placeholder="Instructions" />
            </div>
        </div>
        <asp:Button ID="btnAddItem" runat="server" Text="+ Add to Prescription" CssClass="btn btn-outline-primary mt-2" OnClick="btnAddItem_Click" CausesValidation="false" />
    </div>

    <div class="clinic-card mb-3" style="max-width: 800px;">
        <h5 class="clinic-page-title">Prescription Items</h5>
        <asp:GridView ID="gvDraftItems" runat="server" CssClass="table" AutoGenerateColumns="false" EmptyDataText="No items added yet.">
            <Columns>
                <asp:BoundField DataField="MedicationName" HeaderText="Medication" />
                <asp:BoundField DataField="Dosage" HeaderText="Dosage" />
                <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                <asp:BoundField DataField="Instructions" HeaderText="Instructions" />
            </Columns>
        </asp:GridView>
    </div>

    <asp:Button ID="btnFinish" runat="server" Text="Finish Prescription" CssClass="btn btn-primary" OnClick="btnFinish_Click" CausesValidation="false" />
    <a href="~/Appointments/AppointmentList.aspx" runat="server" class="btn btn-outline-secondary">Cancel</a>
</asp:Content>

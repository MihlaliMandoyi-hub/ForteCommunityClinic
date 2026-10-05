<%@ Page Title="Dispense Medication" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DispenseMedication.aspx.cs" Inherits="ForteCommunityClinic.Pharmacy.DispenseMedication" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Pending Prescriptions</h3>
    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

    <div class="clinic-card">
        <asp:GridView ID="gvPendingItems" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            DataKeyNames="PrescriptionItemID" EmptyDataText="No pending prescription items." OnRowCommand="gvPendingItems_RowCommand">
            <Columns>
                <asp:BoundField DataField="PatientName" HeaderText="Patient" />
                <asp:BoundField DataField="MedicationName" HeaderText="Medication" />
                <asp:BoundField DataField="Dosage" HeaderText="Dosage" />
                <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                <asp:BoundField DataField="QuantityInStock" HeaderText="Available" />
                <asp:BoundField DataField="PrescriptionDate" HeaderText="Prescribed" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button runat="server" Text="Dispense" CssClass="btn btn-sm btn-primary"
                            CommandName="Dispense" CommandArgument='<%# Eval("PrescriptionItemID") %>'
                            OnClientClick="return confirm('Dispense this item?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
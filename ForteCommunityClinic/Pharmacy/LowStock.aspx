<%@ Page Title="Low Stock" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LowStock.aspx.cs" Inherits="ForteCommunityClinic.Pharmacy.LowStock" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Low Stock Medications</h3>
    <div class="clinic-card">
        <asp:GridView ID="gvLowStock" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            EmptyDataText="No medications are currently low on stock.">
            <Columns>
                <asp:BoundField DataField="MedicationName" HeaderText="Medication" />
                <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                <asp:BoundField DataField="QuantityInStock" HeaderText="Available" />
                <asp:BoundField DataField="ReorderLevel" HeaderText="Reorder Level" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <a href="~/Pharmacy/Inventory.aspx" runat="server" class="btn btn-sm btn-primary">Receive Stock</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
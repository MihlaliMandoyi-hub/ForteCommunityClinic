<%@ Page Title="Inventory Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InventoryReport.aspx.cs" Inherits="ForteCommunityClinic.Reports.InventoryReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Inventory Report</h3>

    <div class="row g-3 mb-3">
        <div class="col-md-4"><div class="clinic-card text-center"><div class="text-muted">Total Medications</div><div class="fs-4 fw-bold"><asp:Label ID="lblTotalMeds" runat="server" /></div></div></div>
        <div class="col-md-4"><div class="clinic-card text-center"><div class="text-muted">Low Stock Items</div><div class="fs-4 fw-bold text-danger"><asp:Label ID="lblLowStockCount" runat="server" /></div></div></div>
        <div class="col-md-4"><div class="clinic-card text-center"><div class="text-muted">Total Stock Value</div><div class="fs-4 fw-bold text-success"><asp:Label ID="lblStockValue" runat="server" /></div></div></div>
    </div>

    <div class="clinic-card">
        <asp:GridView ID="gvInventory" runat="server" CssClass="table table-hover" AutoGenerateColumns="false" EmptyDataText="No medications on file.">
            <Columns>
                <asp:BoundField DataField="MedicationName" HeaderText="Medication" />
                <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                <asp:BoundField DataField="QuantityInStock" HeaderText="In Stock" />
                <asp:BoundField DataField="ReorderLevel" HeaderText="Reorder Level" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="StockValue" HeaderText="Stock Value" DataFormatString="{0:C2}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge <%# (int)Eval("QuantityInStock") <= (int)Eval("ReorderLevel") ? "bg-danger" : "bg-success" %>'>
                            <%# (int)Eval("QuantityInStock") <= (int)Eval("ReorderLevel") ? "LOW STOCK" : "OK" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
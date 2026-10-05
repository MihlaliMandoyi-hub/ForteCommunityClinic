<%@ Page Title="Medications" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MedicationList.aspx.cs" Inherits="ForteCommunityClinic.Pharmacy.MedicationList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="clinic-page-title mb-0">Medications</h3>
        <div>
            <a href="~/Pharmacy/Inventory.aspx" runat="server" class="btn btn-outline-primary">Receive Stock</a>
            <a href="~/Pharmacy/MedicationCreate.aspx" runat="server" class="btn btn-primary">+ New Medication</a>
        </div>
    </div>

    <div class="clinic-card mb-3">
        <div class="row g-2">
            <div class="col-md-6">
                <asp:TextBox ID="txtSearchName" runat="server" CssClass="form-control" placeholder="Search by medication name" />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" CausesValidation="false" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-outline-secondary" OnClick="btnClear_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <div class="clinic-card">
        <asp:GridView ID="gvMedications" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            DataKeyNames="MedicationID" EmptyDataText="No medications found.">
            <Columns>
                <asp:BoundField DataField="MedicationName" HeaderText="Medication" />
                <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                <asp:BoundField DataField="QuantityInStock" HeaderText="In Stock" />
                <asp:BoundField DataField="ReorderLevel" HeaderText="Reorder Level" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="ExpiryDate" HeaderText="Expiry" DataFormatString="{0:yyyy-MM-dd}" />
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
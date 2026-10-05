<%@ Page Title="New Bill" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BillCreate.aspx.cs" Inherits="ForteCommunityClinic.Billing.BillCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">New Bill</h3>
    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

    <div class="clinic-card mb-3" style="max-width: 700px;">
        <label>Patient</label>
        <div class="input-group">
            <asp:TextBox ID="txtPatientSearch" runat="server" CssClass="form-control" placeholder="Search by patient number or surname" />
            <asp:Button ID="btnFindPatient" runat="server" Text="Find" CssClass="btn btn-outline-secondary" OnClick="btnFindPatient_Click" CausesValidation="false" />
        </div>
        <asp:DropDownList ID="ddlPatient" runat="server" CssClass="form-select mt-2" />
    </div>

    <div class="clinic-card mb-3" style="max-width: 700px;">
        <h5 class="clinic-page-title">Add Line Item</h5>
        <div class="row g-2">
            <div class="col-md-5">
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" placeholder="e.g. Consultation fee" />
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" TextMode="Number" placeholder="Qty" Text="1" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtUnitPrice" runat="server" CssClass="form-control" TextMode="Number" placeholder="Unit Price" />
            </div>
        </div>
        <asp:Button ID="btnAddItem" runat="server" Text="+ Add Item" CssClass="btn btn-outline-primary mt-2" OnClick="btnAddItem_Click" CausesValidation="false" />
    </div>

    <div class="clinic-card mb-3" style="max-width: 700px;">
        <h5 class="clinic-page-title">Bill Items</h5>
        <asp:GridView ID="gvDraftItems" runat="server" CssClass="table" AutoGenerateColumns="false" EmptyDataText="No items added yet.">
            <Columns>
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="LineTotal" HeaderText="Total" DataFormatString="{0:C2}" />
            </Columns>
        </asp:GridView>
        <div class="text-end fw-bold mt-2">
            Grand Total: <asp:Label ID="lblGrandTotal" runat="server" Text="R0.00" />
        </div>
    </div>

    <asp:Button ID="btnSaveBill" runat="server" Text="Save Bill" CssClass="btn btn-primary" OnClick="btnSaveBill_Click" CausesValidation="false" />
    <a href="~/Billing/BillCreate.aspx" runat="server" class="btn btn-outline-secondary">Cancel</a>
</asp:Content>
<%@ Page Title="My Bills" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyBills.aspx.cs" Inherits="ForteCommunityClinic.Billing.MyBills" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">💳 My Bills</h3>
    <div class="clinic-card">
        <asp:GridView ID="gvMyBills" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            DataKeyNames="BillID" EmptyDataText="You have no bills.">
            <Columns>
                <asp:BoundField DataField="BillDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="TotalAmount" HeaderText="Total" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="AmountPaid" HeaderText="Paid" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="OutstandingAmount" HeaderText="Outstanding" DataFormatString="{0:C2}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge <%# GetStatusBadgeClass(Eval("Status").ToString()) %>'>
                            <%# Eval("Status") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <a href='MyBillDetails.aspx?id=<%# Eval("BillID") %>' class="btn btn-sm btn-outline-primary">View</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

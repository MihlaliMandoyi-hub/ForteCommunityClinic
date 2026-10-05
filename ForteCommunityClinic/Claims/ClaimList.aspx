<%@ Page Title="Medical Aid Claims" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClaimList.aspx.cs" Inherits="ForteCommunityClinic.Claims.ClaimList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="clinic-page-title mb-0">Medical Aid Claims</h3>
        <a href="~/Claims/ClaimCreate.aspx" runat="server" class="btn btn-primary">+ New Claim</a>
    </div>

    <div class="clinic-card mb-3">
        <div class="row g-2">
            <div class="col-md-4">
                <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFilterStatus_SelectedIndexChanged" />
            </div>
        </div>
    </div>

    <div class="clinic-card">
        <asp:GridView ID="gvClaims" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            EmptyDataText="No claims found.">
            <Columns>
                <asp:BoundField DataField="PatientName" HeaderText="Patient" />
                <asp:BoundField DataField="ProviderName" HeaderText="Provider" />
                <asp:BoundField DataField="ClaimAmount" HeaderText="Amount" DataFormatString="{0:C2}" />
                <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge <%# GetStatusBadgeClass(Eval("StatusName").ToString()) %>'>
                            <%# Eval("StatusName") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <a href='ClaimDetails.aspx?id=<%# Eval("ClaimID") %>' class="btn btn-sm btn-outline-primary">View / Update</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
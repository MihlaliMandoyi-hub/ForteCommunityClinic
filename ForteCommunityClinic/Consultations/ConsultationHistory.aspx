<%@ Page Title="Consultation History" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultationHistory.aspx.cs" Inherits="ForteCommunityClinic.Consultations.ConsultationHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Consultation History</h3>
    <asp:Label ID="lblPatientInfo" runat="server" CssClass="mb-3 d-block fw-bold" />
    <div class="clinic-card">
        <asp:GridView ID="gvHistory" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            EmptyDataText="No previous consultations for this patient.">
            <Columns>
                <asp:BoundField DataField="ConsultationDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="DoctorName" HeaderText="Doctor" />
                <asp:BoundField DataField="Symptoms" HeaderText="Symptoms" />
                <asp:BoundField DataField="Status" HeaderText="Status" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <a href='ConsultationDetails.aspx?id=<%# Eval("ConsultationID") %>' class="btn btn-sm btn-outline-primary">View</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
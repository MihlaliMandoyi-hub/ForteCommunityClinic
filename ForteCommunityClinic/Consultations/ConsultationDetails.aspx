<%@ Page Title="Consultation Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsultationDetails.aspx.cs" Inherits="ForteCommunityClinic.Consultations.ConsultationDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Consultation Details</h3>
    <div class="clinic-card" style="max-width: 700px;">
        <dl class="row">
            <dt class="col-sm-3">Patient</dt><dd class="col-sm-9"><asp:Literal ID="litPatient" runat="server" /></dd>
            <dt class="col-sm-3">Doctor</dt><dd class="col-sm-9"><asp:Literal ID="litDoctor" runat="server" /></dd>
            <dt class="col-sm-3">Date</dt><dd class="col-sm-9"><asp:Literal ID="litDate" runat="server" /></dd>
            <dt class="col-sm-3">Symptoms</dt><dd class="col-sm-9"><asp:Literal ID="litSymptoms" runat="server" /></dd>
            <dt class="col-sm-3">Clinical Notes</dt><dd class="col-sm-9"><asp:Literal ID="litNotes" runat="server" /></dd>
            <dt class="col-sm-3">Status</dt><dd class="col-sm-9"><asp:Literal ID="litStatus" runat="server" /></dd>
        </dl>

        <h5 class="clinic-page-title">Diagnoses</h5>
        <asp:Repeater ID="rptDiagnoses" runat="server">
            <ItemTemplate>
                <div class="clinic-card mb-2"><%# Eval("DiagnosisDescription") %></div>
            </ItemTemplate>
        </asp:Repeater>

        <a id="lnkAddPrescription" runat="server" class="btn btn-outline-primary mt-3">Add Prescription</a>
        <asp:Button ID="btnComplete" runat="server" Text="Mark Consultation Complete" CssClass="btn btn-success mt-3" OnClick="btnComplete_Click" />
        <a href="~/Appointments/AppointmentList.aspx" runat="server" class="btn btn-outline-secondary mt-3">Back</a>
    </div>
</asp:Content>
<%@ Page Title="Appointments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AppointmentList.aspx.cs" Inherits="ForteCommunityClinic.Appointments.AppointmentList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="clinic-page-title mb-0">Appointments</h3>
        <a href="~/Appointments/AppointmentCreate.aspx" runat="server" class="btn btn-primary">+ New Appointment</a>
    </div>

    <div class="clinic-card mb-3">
        <div class="row g-2">
            <div class="col-md-3">
                <asp:TextBox ID="txtFilterDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlFilterDoctor" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtFilterPatient" runat="server" CssClass="form-control" placeholder="Patient name/number" />
            </div>
        </div>
        <div class="mt-2">
            <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-primary" OnClick="btnFilter_Click" CausesValidation="false" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-outline-secondary" OnClick="btnClear_Click" CausesValidation="false" />
        </div>
    </div>

    <div class="clinic-card">
        <asp:GridView ID="gvAppointments" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            DataKeyNames="AppointmentID" EmptyDataText="No appointments found." OnRowCommand="gvAppointments_RowCommand">
            <Columns>
                <asp:BoundField DataField="PatientName" HeaderText="Patient" />
                <asp:BoundField DataField="DoctorName" HeaderText="Doctor" />
                <asp:BoundField DataField="AppointmentDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="AppointmentTime" HeaderText="Time" DataFormatString="{0:hh\:mm}" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge badge-<%# Eval("StatusName").ToString().Replace(" ", "").ToLower() %>'>
                            <%# Eval("StatusName") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <a runat="server" CssClass="btn btn-sm btn-outline-primary"
                           Visible='<%# !IsDoctorView && (Eval("StatusName").ToString() == "Scheduled" || Eval("StatusName").ToString() == "Checked In") %>'
                           href='<%# "AppointmentReschedule.aspx?id=" + Eval("AppointmentID") %>'>Reschedule</a>
                        <asp:Button runat="server" Text="Check In" CssClass="btn btn-sm btn-outline-success"
                            CommandName="CheckIn" CommandArgument='<%# Eval("AppointmentID") %>'
                            Visible='<%# !IsDoctorView && Eval("StatusName").ToString() == "Scheduled" %>' />
                        <asp:Button runat="server" Text="Cancel" CssClass="btn btn-sm btn-outline-danger"
                            CommandName="Cancel" CommandArgument='<%# Eval("AppointmentID") %>'
                            Visible='<%# !IsDoctorView && (Eval("StatusName").ToString() == "Scheduled" || Eval("StatusName").ToString() == "Checked In") %>' />
                        <asp:Button runat="server" Text="Start Consultation" CssClass="btn btn-sm btn-primary"
                            CommandName="StartConsultation" CommandArgument='<%# Eval("AppointmentID") %>'
                            Visible='<%# IsDoctorView && Eval("StatusName").ToString() == "Checked In" %>' />
                        <a runat="server" CssClass="btn btn-sm btn-outline-primary"
                           Visible='<%# IsDoctorView && Eval("StatusName").ToString() == "In Consultation" %>'
                           href='<%# "/Consultations/ConsultationCreate.aspx?appointmentId=" + Eval("AppointmentID") %>'>Continue</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
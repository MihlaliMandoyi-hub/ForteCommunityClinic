<%@ Page Title="Today's Patients" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TodaysPatients.aspx.cs" Inherits="ForteCommunityClinic.Nursing.TodaysPatients" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Today's Patients</h3>
    <div class="clinic-card">
        <asp:GridView ID="gvTodaysPatients" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            DataKeyNames="AppointmentID" EmptyDataText="No patients scheduled for today.">
            <Columns>
                <asp:BoundField DataField="PatientName" HeaderText="Patient" />
                <asp:BoundField DataField="DoctorName" HeaderText="Doctor" />
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
                        <a href='PatientVitals.aspx?appointmentId=<%# Eval("AppointmentID") %>' class="btn btn-sm btn-primary">Capture Vitals</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

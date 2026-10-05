<%@ Page Title="My Appointments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyAppointments.aspx.cs" Inherits="ForteCommunityClinic.Appointments.MyAppointments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="clinic-page-title mb-0">📅 My Appointments</h3>
        <a href="~/Appointments/BookAppointment.aspx" runat="server" class="btn btn-primary">+ Book Appointment</a>
    </div>

    <div class="clinic-card">
        <asp:GridView ID="gvMyAppointments" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            DataKeyNames="AppointmentID" EmptyDataText="You have no appointments." OnRowCommand="gvMyAppointments_RowCommand">
            <Columns>
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
                        <asp:Button runat="server" Text="Cancel" CssClass="btn btn-sm btn-outline-danger"
                            CommandName="Cancel" CommandArgument='<%# Eval("AppointmentID") %>'
                            Visible='<%# Eval("StatusName").ToString() == "Scheduled" %>'
                            OnClientClick="return confirm('Cancel this appointment?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

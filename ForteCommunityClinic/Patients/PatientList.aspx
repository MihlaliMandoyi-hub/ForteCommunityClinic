<%@ Page Title="Patients" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PatientList.aspx.cs" Inherits="ForteCommunityClinic.Patients.PatientList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="clinic-page-title mb-0">Patients</h3>
        <a href="~/Patients/PatientCreate.aspx" runat="server" class="btn btn-primary">+ New Patient</a>
    </div>

    <div class="clinic-card mb-3">
        <div class="row g-2">
            <div class="col-md-3">
                <asp:TextBox ID="txtSearchPatientNumber" runat="server" CssClass="form-control" placeholder="Patient Number" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtSearchFirstName" runat="server" CssClass="form-control" placeholder="First Name" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtSearchSurname" runat="server" CssClass="form-control" placeholder="Surname" />
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtSearchPhone" runat="server" CssClass="form-control" placeholder="Phone Number" />
            </div>
        </div>
        <div class="mt-2">
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-outline-secondary" OnClick="btnClear_Click" CausesValidation="false" />
        </div>
    </div>

    <div class="clinic-card">
        <asp:GridView ID="gvPatients" runat="server" CssClass="table table-hover" AutoGenerateColumns="false"
            DataKeyNames="PatientID" EmptyDataText="No patients found.">
            <Columns>
                <asp:BoundField DataField="PatientNumber" HeaderText="Patient #" />
                <asp:BoundField DataField="FullName" HeaderText="Name" />
                <asp:BoundField DataField="DateOfBirth" HeaderText="DOB" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="PhoneNumber" HeaderText="Phone" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <a href='PatientDetails.aspx?id=<%# Eval("PatientID") %>' class="btn btn-sm btn-outline-primary">View</a>
                        <a href='PatientEdit.aspx?id=<%# Eval("PatientID") %>' class="btn btn-sm btn-outline-secondary">Edit</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>

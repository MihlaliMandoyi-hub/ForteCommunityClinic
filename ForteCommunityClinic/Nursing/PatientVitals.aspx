<%@ Page Title="Capture Vitals" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PatientVitals.aspx.cs" Inherits="ForteCommunityClinic.Nursing.PatientVitals" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h3 class="clinic-page-title">Capture Vitals</h3>
    <div class="clinic-card" style="max-width: 500px;">
        <asp:Label ID="lblPatientInfo" runat="server" CssClass="mb-3 d-block fw-bold" />
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-success d-block" Visible="false" Text="Vitals saved successfully." />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger" DisplayMode="BulletList" />

        <div class="row g-3">
            <div class="col-md-6">
                <label>Temperature (&deg;C)</label>
                <asp:TextBox ID="txtTemperature" runat="server" CssClass="form-control" TextMode="Number" />
                <asp:RangeValidator ControlToValidate="txtTemperature" runat="server" Type="Double" MinimumValue="30" MaximumValue="45"
                    ErrorMessage="Enter a realistic temperature (30-45)." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-6">
                <label>Pulse Rate (bpm)</label>
                <asp:TextBox ID="txtPulseRate" runat="server" CssClass="form-control" TextMode="Number" />
                <asp:RangeValidator ControlToValidate="txtPulseRate" runat="server" Type="Integer" MinimumValue="30" MaximumValue="220"
                    ErrorMessage="Enter a realistic pulse rate." CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-6">
                <label>Weight (kg)</label>
                <asp:TextBox ID="txtWeight" runat="server" CssClass="form-control" TextMode="Number" />
            </div>
            <div class="col-md-6">
                <label>Height (cm)</label>
                <asp:TextBox ID="txtHeight" runat="server" CssClass="form-control" TextMode="Number" />
            </div>
            <div class="col-md-6">
                <label>Blood Pressure</label>
                <asp:TextBox ID="txtBloodPressure" runat="server" CssClass="form-control" placeholder="e.g. 120/80" />
                <asp:RegularExpressionValidator ControlToValidate="txtBloodPressure" runat="server"
                    ValidationExpression="^\d{2,3}/\d{2,3}$" ErrorMessage="Format: systolic/diastolic, e.g. 120/80"
                    CssClass="text-danger" Display="Dynamic" />
            </div>
            <div class="col-md-6">
                <label>Oxygen Saturation (%)</label>
                <asp:TextBox ID="txtOxygenSaturation" runat="server" CssClass="form-control" TextMode="Number" />
                <asp:RangeValidator ControlToValidate="txtOxygenSaturation" runat="server" Type="Integer" MinimumValue="50" MaximumValue="100"
                    ErrorMessage="Enter a realistic oxygen saturation (50-100)." CssClass="text-danger" Display="Dynamic" />
            </div>
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Save Vitals" CssClass="btn btn-primary mt-3" OnClick="btnSave_Click" />
        <a href="~/Nursing/TodaysPatients.aspx" runat="server" class="btn btn-outline-secondary mt-3">Back</a>
    </div>
</asp:Content>
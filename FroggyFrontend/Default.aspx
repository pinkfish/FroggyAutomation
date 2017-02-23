<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FroggyFrontend._Default" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <asp:Repeater ID="DeviceRepeater" runat="server" DataSourceID="DevicesDataSource">
    </asp:Repeater>
    <asp:ObjectDataSource ID="DevicesDataSource" runat="server" TypeName="AutomationAdapter" SelectMethod="Devices"></asp:ObjectDataSource>
</asp:Content>

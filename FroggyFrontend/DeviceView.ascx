<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeviceView.ascx.cs" Inherits="FroggyFrontend.WebUserControl1" %>
<asp:Table ID="Table1" runat="server" Height="99px" Width="100%">
    <asp:TableRow runat="server">
        <asp:TableCell ID="Image" runat="server" Width="50px"><asp:Image ID="DeviceImage" runat="server" /></asp:TableCell>
        <asp:TableCell ID="Main" runat="server">
            <asp:Label ID="DeviceName" runat="server"></asp:Label>
            <asp:Label ID="DeviceType" runat="server"></asp:Label>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>



<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="FroggyFrontend.About" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>Froggy Automation</h2>
    </hgroup>

    <article>
        <p>        
            For automating the world, using the power of frogs.
        </p>
    </article>
</asp:Content>
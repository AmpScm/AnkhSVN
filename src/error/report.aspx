<%@ Page language="c#" Codebehind="report.aspx.cs" AutoEventWireup="false" Inherits="error.report" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD>
        <title>WebForm1</title>
        <meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
        <meta name="CODE_LANGUAGE" Content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    </HEAD>
    <body>
        <form id="Form1" method="post" runat="server">
            <P>
                <asp:TextBox id="emailBox" runat="server"></asp:TextBox>
                <asp:Label id="Label1" runat="server">Your email address(Optional)</asp:Label></P>
            <P>
                <asp:Label id="Label2" runat="server">Error message:</asp:Label></P>
            <P>
                <asp:TextBox id="errorMessageBox" runat="server" Columns="80" Rows="25" TextMode="MultiLine"></asp:TextBox></P>
            <P>&nbsp;</P>
            <P>
                <asp:Label id="Label4" runat="server">Additional comments:</asp:Label></P>
            <P>
                <asp:TextBox id="commentsBox" runat="server" Columns="40" Rows="6" TextMode="MultiLine"></asp:TextBox></P>
            <P>
                <asp:Button id="sendButton" runat="server" Text="Send"></asp:Button></P>
        </form>
    </body>
</HTML>

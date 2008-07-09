<%@ Page language="c#" Codebehind="report.aspx.cs" AutoEventWireup="false" Inherits="error.report" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
    <HEAD>
        <title>AnkhSVN error page</title>
        <meta content="True" name="vs_showGrid">
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
        <meta content="C#" name="CODE_LANGUAGE">
        <meta content="JavaScript" name="vs_defaultClientScript">
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    </HEAD>
    <body bgColor="#ffffff" ms_positioning="GridLayout">
        <form id="Form1" method="post" runat="server">
            <P><asp:textbox id="emailBox" style="Z-INDEX: 102; LEFT: 400px; POSITION: absolute; TOP: 528px" runat="server" Width="192px"></asp:textbox></P>
            <P><asp:textbox id="errorMessageBox" style="Z-INDEX: 101; LEFT: 11px; POSITION: absolute; TOP: 176px" runat="server" ReadOnly="True" Height="292px" Columns="80" Rows="18" TextMode="MultiLine" BackColor="#E0E0E0"></asp:textbox></P>
            <DIV style="DISPLAY: inline; Z-INDEX: 106; LEFT: 16px; WIDTH: 224px; POSITION: absolute; TOP: 88px; HEIGHT: 32px" ms_positioning="FlowLayout"><FONT face="Arial Narrow" size="5">Error 
                    report page</FONT></DIV>
            <P><asp:image id="Image1" style="Z-INDEX: 105; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server" Width="634px" Height="74px" ImageUrl="ankh.png"></asp:image></P>
            <P><asp:textbox id="commentsBox" style="Z-INDEX: 104; LEFT: 16px; POSITION: absolute; TOP: 520px" runat="server" Columns="40" Rows="6" TextMode="MultiLine"></asp:textbox></P>
            <DIV style="DISPLAY: inline; Z-INDEX: 108; LEFT: 16px; WIDTH: 256px; POSITION: absolute; TOP: 496px; HEIGHT: 24px" ms_positioning="FlowLayout"><FONT face="Arial Narrow">Additional 
                    comments (Optional):</FONT></DIV>
            <P><asp:button id="sendButton" style="Z-INDEX: 103; LEFT: 16px; POSITION: absolute; TOP: 640px" runat="server" Width="80px" Text="Send"></asp:button></P>
            <DIV style="DISPLAY: inline; Z-INDEX: 107; LEFT: 16px; WIDTH: 120px; POSITION: absolute; TOP: 152px; HEIGHT: 24px" ms_positioning="FlowLayout"><FONT face="Arial Narrow" size="4">Error 
                    message:</FONT></DIV>
            <HR style="Z-INDEX: 109; LEFT: 16px; WIDTH: 60.16%; POSITION: absolute; TOP: 488px; HEIGHT: 1px" width="60.16%" SIZE="1">
            <DIV style="DISPLAY: inline; Z-INDEX: 110; LEFT: 400px; WIDTH: 192px; POSITION: absolute; TOP: 496px; HEIGHT: 25px" ms_positioning="FlowLayout"><FONT face="Arial Narrow">Your 
                    email adress (Optional):</FONT></DIV>
            <HR style="Z-INDEX: 111; LEFT: 16px; POSITION: absolute; TOP: 136px; HEIGHT: 1px" width="60.16%" SIZE="1">
            <asp:TextBox id="versionBox" style="Z-INDEX: 112; LEFT: 464px; POSITION: absolute; TOP: 584px" runat="server" Width="64px" ReadOnly="True" BackColor="#E0E0E0"></asp:TextBox>
            <asp:Label id="Label1" runat="server">Label</asp:Label>
            <asp:Label id="Label2" runat="server">Label</asp:Label>
            <DIV style="DISPLAY: inline; Z-INDEX: 113; LEFT: 400px; WIDTH: 56px; POSITION: absolute; TOP: 584px; HEIGHT: 24px" ms_positioning="FlowLayout"><FONT face="Arial Narrow" size="4">Version:</FONT></DIV>
        </form>
    </body>
</HTML>

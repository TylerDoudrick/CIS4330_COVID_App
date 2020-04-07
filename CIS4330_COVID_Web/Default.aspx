<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CIS4330_COVID_Web.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button runat="server" OnClick="btnGrabRecords_Click" Text="Grab Records"/>
            <asp:Button runat="server" OnClick="btnAddRecord_Click" Text="Add Record"/>
            <asp:TextBox runat="server" ID="txtName"></asp:TextBox>
        </div>
    </form>
</body>
</html>

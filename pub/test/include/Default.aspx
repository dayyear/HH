<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="pub_test_include_Default" %>
<%@ Register src="WebUserControl.ascx" tagname="WebUserControl" tagprefix="uc1" %>
<%@ Register src="WebUserControl2.ascx" tagname="WebUserControl2" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Default</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc1:WebUserControl runat="server" />
        <uc1:WebUserControl2 runat="server" />
    </div>
    </form>
</body>
</html>

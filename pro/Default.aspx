<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="pro_Default" %>
<%@ Register src="include/topNav.ascx" tagname="topNav" tagprefix="uc1" %>
<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>南通辉煌国旅</title>
    <link href="../pub/themes/cupertino/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../pub/css/common.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        h2{padding:8px 0; color:#00A0E8; font-weight:bolder;}
        .box{padding:16px 0; border-top:2px solid #FFC20D;}
    </style>

    <script src="../pub/js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../pub/js/jquery-ui.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[type=submit], input[type=reset], input[type=button], a.btn").button();
        });
    </script>
</head>
<body>
<uc1:topNav ID="topNav1" runat="server" />
<div id="wrapper">
    <div id="header">
        <div id="title">南通辉煌国旅</div>
    </div><!--header-->


    <h2>请选择</h2>
    <div class="box">
        <%if(Page.User.IsInRole("TWN")){%>
        <a class="btn" href="<%=Page.ResolveClientUrl("~/pro/TWN")%>">产品部证照管理</a>
        <%}%>
        <%if(Page.User.IsInRole("DRP")){%>
        <a class="btn" href="<%=Page.ResolveClientUrl("~/pro/DRP")%>">同行分销</a>
        <%}%>
        <%if(Page.User.IsInRole("ADMIN")){%>
        <a class="btn" href="<%=Page.ResolveClientUrl("~/pro/ADMIN/dm.aspx")%>">系统管理</a>
        <%}%>
    </div>
</div>
</body>
</html>

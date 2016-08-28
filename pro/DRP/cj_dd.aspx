<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cj_dd.aspx.cs" Inherits="pro_DRP_cj_dd" ViewStateMode="Disabled" %>
<%@ Register src="../include/topNav.ascx" tagname="topNav" tagprefix="uc1" %>
<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>同行分销 — 出境订单</title>
    <link href="../../pub/themes/cupertino/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../../pub/css/common.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #condition{display:none;}
        #datagrid table{border-collapse:collapse; width:100%;}
        #datagrid td{margin:0; padding:4px; white-space:nowrap}
        #datagrid th{margin:0; padding:4px; white-space:nowrap; background: #00ADEF; color:#FFF; border-right:1px solid #FFF;}
        #datagrid a{color:#01AEF0; font-weight:bolder;}
        #datagrid tr.odd{background-color: #DCDCDC;}
        #datagrid tr.selecting{background: #EAF2FF;}
        #datagrid tr.selected{background-color: #FFE48D;}
        #toolbar{padding:8px 0;}
        #datagrid,#pagination{border:1px solid #DDDDDD;}
        #exception{font-size:200%; text-align:center; color:#f00; background:#ff0;}
        
        .zt1{color:#01AEF0; font-weight:bolder;} .zt0{color:#F7B900; font-weight:bolder;}
    </style>

    <script src="../../pub/js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../../pub/js/common.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            $(".btn").button();
            $(".btnDisable").button().button("disable");
        });
    </script>
</head>
<body>
<form id="form1" runat="server">
<!--#include file="~/pro/DRP/include/qq.htm"-->
<uc1:topNav ID="topNav1" runat="server" />
<div id="wrapper">
    
    <div id="header">
        <div id="title">同行分销 — 出境订单</div>
        <!--#include file="~/pro/DRP/include/menu.htm"-->
    </div><!--header-->

    <div id="toolbar" runat="server" />

    <div id="result">
        <div id="exception" runat="server" />
        <div id="datagrid" runat="server" />
        <div id="pagination" runat="server" />
    </div><!--result-->
</div><!--wrapper-->
</form>
</body>
</html>

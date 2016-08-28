<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="pro_DRP_Default" %>
<%@ Register src="../include/topNav.ascx" tagname="topNav" tagprefix="uc1" %>
<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<form id="form1" runat="server">
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>同行分销</title>

    <link href="../../pub/themes/cupertino/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../../pub/css/common.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        /*div.box{text-align:center;}
        .box a{font-size:x-large; }*/
        /*#main p{font-size:larger;margin-top:30px;}*/
        h2{padding:8px 0; color:#00A0E8; font-weight:bolder;}
        .box{padding:16px 0; border-top:2px solid #FFC20D;}
    </style>

    <script src="../../pub/js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery-ui.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[type=submit], input[type=reset], input[type=button], a.btn").button();
        });
    </script>
</head>
<body>
<!--#include file="~/pro/DRP/include/qq.htm"-->
<uc1:topNav ID="topNav1" runat="server" />
<div id="wrapper">
    
    <div id="header">
        <div id="title">同行分销</div>
        <!--#include file="~/pro/DRP/include/menu.htm"-->
    </div><!--header-->
    
    <h2>线路预定</h2>
    <div class="box">
        <div id="XLYD">
            <a class="btn" href="twn_td.aspx">台湾线路预定</a>
            <a class="btn" href="cj_td.aspx">出境线路预定</a>
            <a class="btn" href="gn_td.aspx">国内线路预定</a>
            <a class="btn" href="http://www.nthhly.com/visa/visa.html" target="_blank">签证预定</a>
        </div>
    </div>

    <h2>我的订单</h2>
    <div class="box">
        <div id="WDDD">
            <a class="btn" href="twn_dd.aspx">台湾线路订单</a>
            <a class="btn" href="cj_dd.aspx">出境线路订单</a>
            <a class="btn" href="gn_dd.aspx">国内线路订单</a>
        </div>
    </div>
</div><!--wrapper-->


</body>
</html>
</form>


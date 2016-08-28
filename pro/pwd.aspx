<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pwd.aspx.cs" Inherits="pro_pwd" ViewStateMode="Disabled" %>
<%@ Register src="./include/topNav.ascx" tagname="topNav" tagprefix="uc1" %>
<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>修改密码</title>
    <link href="../pub/themes/cupertino/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../pub/css/common.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #main{font-size:150%; margin:36px 0 0 36px;}
        #main input{font-size:100%;}
        #main td, #main th{padding:6px;}
        #exception{font-size:200%; text-align:center; color:#f00; background:#ff0;}
    </style>
    <script src="../pub/js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../pub/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../pub/js/common.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".btn").button();



            $("#确定").click(function (e) {
                e.preventDefault();
                $("#loading").show();
                $.post(window.location.pathname + "?r=" + new Date().getTime(), $("form").serialize(), function (response) {
                    $("#loading").hide();
                    $("input").val("");
                    if (response.success) {
                        alert("修改密码成功");
                    }
                    else {
                        alert("修改密码失败: " + response.message);
                    }
                }, "json");
            });
            $("#清空").click(function (e) {
                e.preventDefault();
                $("input").val("");
            });
        });
    </script>
</head>
<body>
<form id="form1" action="#">
<uc1:topNav ID="topNav1" runat="server" />
<div id="wrapper">
    
    <div id="header">
        <div id="title">修改密码</div>
    </div><!--header-->

    <div id="exception" runat="server" />

    <div id="main"><table>
    <tr>
        <th>登　录　名</th>
        <td><span id="loginname" class="init" runat="server"></span></td>
    </tr>
    <tr>
        <th>用　户　名</th>
        <td><span id="username" class="init" runat="server"></span></td>
    </tr>
    <tr>
        <th>旧　密　码</th>
        <td><input type="password" name="pwd1" /></td>
    </tr>
    <tr>
        <th>新　密　码</th>
        <td><input type="password" name="pwd2" /></td>
    </tr>
    <tr>
        <th>确认新密码</th>
        <td><input type="password" name="pwd3" /></td>
    </tr>
    <tr>
        <th colspan="2">
        <a href="javascript:void(0);" id="确定" class="btn">确定</a>
        <a href="javascript:void(0);" id="清空" class="btn">清空</a>
        </th>
    </tr>
    </table></div>

</div><!--wrapper-->
<div id="loading">
<img src="../pub/img/loading.GIF" alt="loading..." />
</div>
</form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="public_login_login" %>
<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>登录</title>
    <link href="../themes/cupertino/jquery-ui.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        body{font-size:20px;}
        #wrapper{width:260px;margin:0 auto;margin-top:100px;}
        #logo{background: url('../img/logo.jpg') no-repeat 0 0;height:79px;width:50px;float:left;}
        h2{height:79px;margin:0 0 0 50px;padding:0; line-height:79px;font-size:150%;}
        form{margin:0;}
        table td{padding:6px;}
        #toolbar{font-size:16px;}
        #userid{margin:0;padding:2px;border:1px solid gray;width:156px;}
        #loginname{margin:0;padding:2px;border:1px solid gray;width:150px;}
        #password{margin:0;padding:2px;border:1px solid gray;width:150px;}
    </style>
    
</head>
<body>
    <div id="wrapper">

    <center><span id="logo">&nbsp;</span><h2>南通辉煌国旅</h2></center>

    <center><form action="#">
    <table>
        <tr>
            <td>登录名</td>
            <td><input type="text" name="loginname" id="loginname" /></td>
        </tr>
        <tr>
            <td>密　码</td>
            <td><input type="password" name="password" id="password" /></td>
        </tr>
        <tr>
            <td colspan="2"><input type="checkbox" name="rememberme" id="rememberme" />一个月内自动登录</td>
        </tr>
        <tr id="toolbar">
            <td colspan="2" align="center">
                <input type="submit" value="登　录" id="登录"/>
            </td>
        </tr>
    </table></form></center>

    <center><div id="message"></div></center>
    </div>

    <script src="../js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../js/common.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[type=submit]").button();
            $("form").submit(function (e) {
                e.preventDefault();
                $.post(window.location.pathname + "?r=" + new Date().getTime(), $("form").serialize(), function (response) {
                    if (response.success)
                        if (getUrlVars()["ReturnUrl"])
                            window.location = decodeURIComponent(getUrlVars()["ReturnUrl"]);
                        else
                            window.location = getRootPath() + "/pro/";
                    else
                        $("#message").text(response.message);
                }, "json");
            });
        });
    </script>

</body>
</html>

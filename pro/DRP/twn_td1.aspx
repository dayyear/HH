<%@ Page Language="C#" AutoEventWireup="true" CodeFile="twn_td1.aspx.cs" Inherits="pro_DRP_twn_td1" %>
<%@ Register src="../include/topNav.ascx" tagname="topNav" tagprefix="uc1" %>
<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>同行分销 — 台湾线路</title>

    <link href="../../pub/themes/cupertino/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../../pub/css/common.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        h2{padding:8px 0; color:#00A0E8; font-weight:bolder;}
        .box{padding:16px 0; border-top:2px solid #FFC20D;}
        .box table{border-collapse:collapse; border:1px solid #DDDDDD; width:100%;}
        .box th{margin:0; padding:4px; white-space:nowrap; border:1px solid #DCDCDC; /*background: #DCDCDC;*/}
        .box td{margin:0; padding:4px; /*white-space:nowrap;*/ border:1px solid #DCDCDC;}
        .toolbar{padding:16px 0; border-top:2px solid #FFC20D;}
    </style>

    <script src="../../pub/js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../../pub/js/ajaxfileupload.js" type="text/javascript"></script>
    <script src="../../pub/js/handlebars-v3.0.1.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery.loadJSON.js" type="text/javascript"></script>
    <script src="../../pub/js/common.js" type="text/javascript"></script>
    <script src="twn_td1.js" type="text/javascript"></script>

    <script type="text/template" id="BJXXTemplate">
        <tr>
            <td>{{sn}}</td>
            <td><input type="text" name="BJMC" size="65" value="{{BJMC}}" /></td>
            <td><input type="text" name="MSJ" size="10" value="{{MSJ}}" /></td>
            <td><input type="text" name="FL" size="10" value="{{FL}}" /></td>
        </tr>
    </script>

</head>
<body>
<!--#include file="~/pro/DRP/include/qq.htm"-->
<uc1:topNav ID="topNav1" runat="server" />
<form action="#">
<input type="hidden" name="ID" value="<%=ID%>" />
<input type="hidden" name="VERB" value="<%=VERB%>" />
<div id="wrapper">
    
    <div id="header">
        <div id="title">同行分销 — 台湾线路</div>
        <!--#include file="~/pro/DRP/include/menu.htm"-->
    </div><!--header-->
    
    <h2>基本信息</h2>
    <div class="box">
        <table>
            <tbody>
            <tr>
                <th>团队编号</th>
                <td><input type="text" name="BH" size="11" /></td>
                <th>线路名称</th>
                <td><input type="text" name="XLMC" size="15" /></td>
                <th>出发日期</th>
                <td><input type="text" name="CFRQ" class="datepicker" size="11" /></td>
                <th>住宿</th>
                <td><input type="text" name="ZS" size="40" /></td>
            </tr>
            <tr>
                <th>天数</th>
                <td><input type="text" name="TS" size="11" /></td>
                <th>人数</th>
                <td><input type="text" name="RS" size="15" /></td>
                <th>状态</th>
                <td><select name="ZT"><option value="">请选择</option><option value="0">关闭</option><option value="1">开通</option></select></td>
                <th>行程</th>
                <td><a target="_blank" href="twn_td_fj.aspx?ID=<%=ID%>"><img src="../../pub/img/word.gif" alt="word" /></a>
                <input type="text" name="XC" readonly="readonly" size="30" />
                <input type="file" id="file_XC" name="file_XC" size="4" style="width:70px" /></td>
            </tr>
            <%if (VERB.ToUpper() == "SELECT"){%>
            <tr>
                <th>已定</th>
                <td><input type="text" name="YDRS" size="40" /></td>
                <th>剩余</th>
                <td><input type="text" name="SYRS" size="40" /></td>
                <th>发布人</th>
                <td><input type="text" name="FBR1" size="40" /></td>
                <th>发布时间</th>
                <td><input type="text" name="FBSJ" size="40" /></td>
            </tr>
            <%}%>
            </tbody>
        </table>
    </div><!--box-->
    
    <h2>报价信息</h2>
    <div class="box">
        <table>
        <thead>
        <tr>
            <th>报价数量</th>
            <td><input type="text" id="BJSL" value="3" size="10" /></td>
            <th>单房差(元)</th>
            <td><input type="text" name="DFC"  size="10" /></td>
        </tr>
        </thead>
        </table>

        <table>
        <thead>
        <tr>
            <th>序号</th>
            <th>报价名称</th>
            <th>门市价(元)</th>
            <th>返利(元)</th>
        </tr>
        </thead>
        <tbody id="BJXX" >
        <tr>
            <td>1</td>
            <td><input type="text" name="BJMC" size="65" value="" /></td>
            <td><input type="text" name="MSJ" size="10" value="" /></td>
            <td><input type="text" name="FL" size="10" value="" /></td>
        </tr>
        <tr>
            <td>2</td>
            <td><input type="text" name="BJMC" size="65" value="" /></td>
            <td><input type="text" name="MSJ" size="10" value="" /></td>
            <td><input type="text" name="FL" size="10" value="" /></td>
        </tr>
        <tr>
            <td>3</td>
            <td><input type="text" name="BJMC" size="65" value="" /></td>
            <td><input type="text" name="MSJ" size="10" value="" /></td>
            <td><input type="text" name="FL" size="10" value="" /></td>
        </tr>
        </tbody>
        </table>
    </div><!--box-->

    <div class="toolbar">
        <%if (VERB.ToUpper() == "SELECT"){%>
              <a href="twn_dd1.aspx?VERB=INSERT&TD_ID=<%=ID%>" class="btn" id="预定">预定</a>
        <%}%>
        <%if (VERB.ToUpper() == "SELECT" && User.IsInRole("ADMIN")){%>
            <a href="twn_td1.aspx?VERB=UPDATE&ID=<%=ID%>" class="btn" id="编辑">编辑</a>
            <a href="javascript:void(0);" onclick="save('COPY');" class="btn" id="复制">复制</a>
        <%}%>
        <%if (VERB.ToUpper() == "INSERT"){%>
        <a href="javascript:void(0);" onclick="save('INSERT');" class="btn">发布</a>
        <%}%>
        <%if (VERB.ToUpper() == "UPDATE"){%>
        <a href="javascript:void(0)" onclick="save('UPDATE');" class="btn">保存</a>
        <%}%>
        <a href="javascript:window.opener=null;window.open('','_self');window.close();" class="btn">关闭</a>
    </div>
</div><!--wrapper-->
</form>

<div id="loading">
    <img src="../../pub/img/loading.GIF" alt="loading..."/>
</div><!--loading-->

</body>
</html>

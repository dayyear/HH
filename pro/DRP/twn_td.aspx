<%@ Page Language="C#" AutoEventWireup="true" CodeFile="twn_td.aspx.cs" Inherits="pro_DRP_twn_td" %>
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
        #condition{display:none;}
        #datagrid table{border-collapse:collapse; width:100%;}
        #datagrid td{margin:0; padding:4px; white-space:nowrap}
        #datagrid th{margin:0; padding:4px; white-space:nowrap; background: #00ADEF; color:#FFF; border-right:1px solid #FFF;}
        #datagrid a{color:#01AEF0; font-weight:bolder;}
        #datagrid tr.odd{background-color: #DCDCDC;}
        #datagrid tr.selecting{background: #EAF2FF;}
        #datagrid tr.selected{background-color: #FFE48D;}
        #toolbar,#pagination{padding:8px 0;}
        #datagrid,#pagination{border:1px solid #DDDDDD;}
        
        .zt1{color:#01AEF0; font-weight:bolder;} .zt0{color:#F7B900; font-weight:bolder;}
    </style>

    <script src="../../pub/js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../../pub/js/ajaxfileupload.js" type="text/javascript"></script>
    <script src="../../pub/js/handlebars-v3.0.1.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery.loadJSON.js" type="text/javascript"></script>
    <script src="../../pub/js/common.js" type="text/javascript"></script>
    <script src="twn_td.js" type="text/javascript"></script>

    <script type="text/template" id="resultTemplate">
        <tr>
            <td>{{sn}}</td>
            <td><a href="twn_td1.aspx?VERB=SELECT&ID={{ID}}" target="_blank">{{BH}}</a></td>
            <td style='white-space:normal;'>{{XLMC}}</td>
            <td>{{CFRQ}}</td>
            <td><a href="twn_td_fj.aspx?ID={{ID}}" target="_blank"><img src="../../pub/img/word.gif" /></a></td>
            <td>{{TS}}</td>
            <!--<td>{{DFC}}</td>-->
            <td>{{RS}}</td>
            <td>{{YDRS}}</td>
            <td>{{SYRS}}</td>
            <td class="zt{{ZT}}">{{ZT1}}</td>
            <td>{{FBR1}}</td>
            <td>{{FBSJ}}</td>
            <!--<td><a href="twn_dd1.aspx?VERB=INSERT&TD_ID={{ID}}" target="_blank">[预定]</a></td>-->
        </tr>
    </script>
</head>
<body>
<!--#include file="~/pro/DRP/include/qq.htm"-->
<uc1:topNav ID="topNav1" runat="server" />
<form action="twn_td.aspx" method="get">
<input type="hidden" name="rows" value="<%=rows%>" />
<input type="hidden" name="page" value="<%=page%>" />
<div id="wrapper">
    
    <div id="header">
        <div id="title">同行分销 — 台湾线路</div>
        <!--#include file="~/pro/DRP/include/menu.htm"-->
    </div><!--header-->

    <div id="toolbar">
        <%if (User.IsInRole("ADMIN")){%>
        <a href="twn_td1.aspx?VERB=INSERT" class="btn" target="_blank">发布</a>
        <%}%>
    </div><!--toolbar-->

    <div id="result">
        <div id="datagrid">
            <table>
            <thead>
            <tr>
                <th>序号</th>
                <th>团队编号</th>
                <th>线路名称</th>
                <th>[出发日期]</th>
                <th>行程</th>
                <th>天数</th>
                <th>人数</th>
                <th>已订</th>
                <th>剩余</th>
                <th>状态</th>
                <th>发布人</th>
                <th>发布时间</th>
            </tr>
            </thead>
            <tbody></tbody>
            </table>
        </div><!--datagrid-->

        <div id="pagination">
            <table>
            <tr>
                <td>
                    <span>共有<strong id="total">0</strong>条记录,</span>
                    <span>第<strong id="page">0</strong>页/</span><span>共<strong id="totalpage">0</strong>页</span>
                </td>
                <td>
                        <input type="button" value="首页" id="首页"/>
                        <input type="button" value="上一页" id="上一页"/>
                        <input type="button" value="下一页" id="下一页"/>
                        <input type="button" value="尾页" id="尾页"/>
                </td>
                <td>
                    <span>[注]：查询结果按照<strong>[出发日期]</strong>倒序排列</span>
                </td>
            </tr>
            </table>
        </div><!--pagination-->
    </div><!--result-->
</div><!--wrapper-->
</form>

<div id="loading">
    <img src="../../pub/img/loading.GIF" alt="loading..."/>
</div><!--loading-->

</body>
</html>

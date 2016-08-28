<%@ Page Language="C#" AutoEventWireup="true" CodeFile="twn_dd1.aspx.cs" Inherits="pro_DRP_twn_dd1" %>
<%@ Register src="../include/topNav.ascx" tagname="topNav" tagprefix="uc1" %>
<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>同行分销 — 台湾线路预定</title>

    <link href="../../pub/themes/cupertino/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../../pub/css/common.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #left{float:left; width:760px;}
        #right{float:right; width:230px; background:#EFEFEF;}
        #DDBH{text-align:center; font-size:150%;}
        .box{padding:16px 0;border-top:2px solid #FFC20D; /*border-bottom:1px solid #aaa;*/}
        #left h2{padding:8px 0; color:#00A0E8; font-weight:bolder;}
        .box table{border-collapse:collapse; /*border:1px solid #DDDDDD;*/ width:100%;}
        .box th{margin:0; padding:4px; white-space:nowrap; border:1px solid #DCDCDC; /*background: #EFEFEF;*/}
        .box td{margin:0; padding:4px; /*white-space:nowrap;*/ border:1px solid #DCDCDC;}
        #right p{padding:8px;}
        #right h2{padding:8px 0; color:#00A0E8; font-weight:bolder; text-align:center;}
    </style>

    <script src="../../pub/js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../../pub/js/ajaxfileupload.js" type="text/javascript"></script>
    <script src="../../pub/js/handlebars-v3.0.1.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery.loadJSON.js" type="text/javascript"></script>
    <script src="../../pub/js/common.js" type="text/javascript"></script>
    <script src="twn_dd1.js" type="text/javascript"></script>

    <script type="text/template" id="FYJSTemplate">
        <tr>
            <td>{{SN}}</td>
            <td>{{BJMC}}</td>
            <td><span class="MSJ">{{MSJ}}</span></td>
            <td><span class="FL">{{FL}}</span></td>
            <td><input type="text" class="RS" name="RS" value="{{RS}}" size="5" /></td>
            <td><input type="text" class="RS_DFC" name="RS_DFC" value="{{RS_DFC}}" size="5" /></td>
            <td><span class="FYXJ">{{FYXJ}}</span><span class="FLXJ" style="display:none;">{{FLXJ}}</span></td>
        </tr>
    </script>
    <script type="text/template" id="YKMDTemplate">
        <tr>
            <td>{{sn}}</td>
            <td><input type="text" name="XM" size="10" /></td>
            <td><input type="text" name="SFZH" size="19" /></td>
            <td><select name="XB"><option value="">请选择</option><option value="1">男</option><option value="2">女</option></select></td>
            <td><input type="text" name="CSRQ" class="datepicker" size="11"/></td>
            <td><input type="text" name="LXDH" size="12" /></td>
            <td><select name="SFJS"><option value="1">是</option><option value="0">否</option></select></td>
        </tr>
    </script>
</head>
<body>
<!--#include file="~/pro/DRP/include/qq.htm"-->
<uc1:topNav ID="topNav1" runat="server" />
<form action="#">
<input type="hidden" name="ID" value="<%=ID%>" />
<input type="hidden" name="TD_ID" value="<%=TD_ID%>" />
<input type="hidden" name="VERB" value="<%=VERB%>" />
<div id="wrapper">

    <div id="header">
        <div id="title">同行分销 — 台湾线路订单</div>
        <!--#include file="~/pro/DRP/include/menu.htm"-->
    </div><!--header-->

    <div id="left">
    <%if (VERB.ToUpper() == "UPDATE"){%>
    <div id="DDBH">订单编号：<span id="BH"></span></div>
    <%}%>
    <h2>线路信息</h2>
    <div class="box">
        <!--<ul>
            <li><a href="#XLXX">线路信息</a></li>
        </ul>-->
        <div id="XLXX">
        <table>
        <tbody>
        <tr>
            <th>团队编号</th>
            <td><input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>BH" /></td>
            <th>线路名称</th>
            <td><input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>XLMC" /></td>
            <th>出发日期</th>
            <td><input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>CFRQ" class="datepicker" /></td>
            <th>住宿</th>
            <td><input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>ZS" /></td>
        </tr>
        <tr>
            <th>天数</th>
            <td><input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>TS" /></td>
            <th>人数</th>
            <td><input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>RS" /></td>
            <%if(VERB.ToUpper()=="INSERT"){%>
            <th>状态</th>
            <td><input type="text" name="ZT1" /></td>
            <th>行程</th>
            <td><a target="_blank" href="twn_td_fj.aspx?ID=<%=TD_ID%>"><img src="../../pub/img/word.gif" alt="word" /></a></td>
        </tr>
        <tr>
            <th>已定</th>
            <td><input type="text" name="YDRS" /></td>
            <th>剩余</th>
            <td><input type="text" name="SYRS" /></td>
            <%}%>
            <th>发布人</th>
            <td><input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_FBR"); else Response.Write("FBR1");%>" /></td>
            <th>发布时间</th>
            <td><input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>FBSJ" /></td>
        </tr>
        </tbody>
        </table>
        </div>
    </div>

    <h2>费用计算</h2>
    <div class="box">
        <!--<ul>
            <li><a href="#FYJS">费用计算</a></li>
        </ul>-->
        <div id="FYJS">
        <table>
        <thead>
        <tr>
            <th colspan="2">单房差(元)</th>
            <td colspan="5"><input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>DFC"  size="10" /></td>
        </tr>
        <tr>
            <th>序号</th>
            <th>报价名称</th>
            <th>门市价(元)</th>
            <th>返利(元)</th>
            <th>人数</th>
            <th>人数(单房差)</th>
            <th>小计(元)</th>
        </tr>
        </thead>
        <tbody>
        </tbody>
        <tfoot><tr>
            <th colspan="7">费用合计(元)：<span id="FYHJ">0.00</span></th></tr><tr>
            <th colspan="7">结算价格(元)：<span id="JSJG">0.00</span></th>
        </tr></tfoot>
        </table>
        </div>
    </div>

    <h2>游客名单</h2>
    <div class="box">
        <!--<ul>
            <li><a href="#YKMD">游客名单</a></li>
        </ul>-->
        <div id="YKMD">
        <table>
        <thead>
        <tr>
            <th>序号</th>
            <th>姓名</th>
            <th>身份证号</th>
            <th>性别</th>
            <th>出生日期</th>
            <th>联系电话</th>
            <th>是否含接送</th>
        </tr>
        </thead>
        <tbody>
        </tbody>
        </table>
        </div>
    </div>
    
    <h2>其他信息</h2>
    <div class="box">
        <div id="YDXX">
        <table>
        <tbody>
            <tr>
                <th>上车点</th>
                <td><select name="SCD" class="flat"><option value="">请选择</option><option value="1">南通大饭店</option></select></td>
                <th>订单备注</th>
                <td><input type="text" name="BZ" size="55"/></td>
            </tr>
            <tr>
                <th>经办人</th>
                <td><input type="text" name="JBR" /></td>
                <th>经办人手机</th>
                <td><input type="text" name="JBRSJ" size="55"/></td>
            </tr>
            <%if (VERB.ToUpper() == "UPDATE"){%>
            <tr>
                <th>预定用户</th>
                <td><input type="text" name="YDR1" /></td>
                <th>预定时间</th>
                <td><input type="text" name="YDSJ" size="55"/></td>
            </tr>
            <tr>
                <th>订单状态</th>
                <td colspan="3">
                    <select name="ZT" <%if (!User.IsInRole("ADMIN")){%>class="flat"<%}%>>
                        <option value="1">待审核</option>
                        <option value="2">已审核</option>
                        <option value="0">已取消</option>
                    </select>
                </td>
            </tr>
            <%}%>
        </tbody>
        </table>
        </div>
    </div>

    <a href="javascript:void(0);" onclick="save();" class="btn">保存</a>
    <%if (VERB.ToUpper() == "UPDATE"){%><a href="javascript:window.print();" class="btn">打印</a><%}%>
    <%if (VERB.ToUpper() == "UPDATE"){%><a href="javascript:void(0);" onclick="cancel();" class="btn">取消订单</a><%}%>
    
    <a href="javascript:window.opener=null;window.open('','_self');window.close();" class="btn">关闭</a>

    

    </div><!--left-->

    <div id="right">
        <h2>友情提醒</h2>
        <p>款请付至以下账号，谢谢支持！！ </p>
        <p>公司帐号：<br/>南通辉煌国际旅行社有限公司</p>
        <p>开户行：<br/>农行桃坞路分理处</p>
        <p>开户账号：<br/>708201040001819</p>
        <p>电话：<br/>0513-85126018（财务直线）</p>
        <p>传真：<br/>0513-85121033（COFAX自动传真）</p>
        <p>由于同一天进账的款项较多，有的数目差不多，很难分辨，请您汇好款后及时将银行回执传真到我们公司【FAX：85121033】或电话告知我公司财务【林霞15962966547】或告知【龚艳玲18106299262】。</p>
        <p>谢谢配合和理解！</p>
    </div><!--right-->
</div><!--wrapper-->
</form>

<div id="loading">
    <img src="../../pub/img/loading.GIF" alt="loading..." />
</div><!--loading-->

</body>
</html>

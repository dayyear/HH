<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cj_dd1.aspx.cs" Inherits="pro_DRP_cj_dd1" ViewStateMode="Disabled" %>
<%@ Register src="../include/topNav.ascx" tagname="topNav" tagprefix="uc1" %>
<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>同行分销 — 出境线路预定</title>

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
        #exception{font-size: 200%; color:#f00; font-weight:bold; text-align:center; background:#ff0;}
    </style>

    <script src="../../pub/js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../../pub/js/ajaxfileupload.js" type="text/javascript"></script>
    <script src="../../pub/js/handlebars-v3.0.1.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery.loadJSON.js" type="text/javascript"></script>
    <script src="../../pub/js/common.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".btn").button();
            $(".btnDisable").button().button("disable");
            datepicker();

            var FYJSTemplate = Handlebars.compile($("#FYJSTemplate").html());
            var YKMDTemplate = Handlebars.compile($("#YKMDTemplate").html());

            $(".RS,.RS_DFC").live("blur", function () {
                var $this = $(this);

                // 整数判断
                var i = parseInt($this.val());
                if (isNaN(i)) $this.val(0);
                else $this.val(Math.abs(i));

                // 判断单房差人数是否小于人数
                var $tr = $this.closest("tr");
                if (parseInt($tr.find(".RS").val()) < parseInt($tr.find(".RS_DFC").val())) {
                    if ($this.hasClass("RS")) $this.val($tr.find(".RS_DFC").val());
                    else $this.val($tr.find(".RS").val());
                }

                // 计算总人数
                var ZRS = 0;
                $(".RS").each(function () { ZRS += parseInt($(this).val()); });

                // 名单中已有人数
                var MDRS = $("#YKMD tr").length;

                // 添加游客名单
                if (ZRS > MDRS) {
                    for (var sn = $("#YKMD tr").length + 1; sn <= ZRS; sn++) {
                        $("#YKMD").append(YKMDTemplate({ sn: sn }));
                    }
                    datepicker();
                }

                // 删除游客名单
                if (ZRS < MDRS) {
                    for (var sn = 1; sn <= MDRS - ZRS; sn++) {
                        $("#YKMD tr").eq(-1).remove();
                    }
                }

                // 计算费用小记
                var $tr = $this.closest("tr");
                var FYXJ = $tr.find(".MSJ").text() * $tr.find(".RS").val() + $("#TD_DFC").text() * $tr.find(".RS_DFC").val();
                var FLXJ = $tr.find(".FL").text() * $tr.find(".RS").val();
                $tr.find(".FYXJ").text(FYXJ.toFixed(2));
                $tr.find(".FLXJ").text(FLXJ.toFixed(2));

                // 计算费用合计
                var FYHJ = 0.0;
                var FLHJ = 0.0;
                $(".FYXJ").each(function () {
                    FYHJ += parseFloat($(this).text());
                });
                $(".FLXJ").each(function () {
                    FLHJ += parseFloat($(this).text());
                });
                $("#FYHJ").text(FYHJ.toFixed(2));
                $("#JSJG").text((FYHJ - FLHJ).toFixed(2));
            });


        });

        function save() {
            $.ajax({ type: "POST",
                url: window.location.pathname + "?r=" + new Date().getTime(),
                data: $("form").serialize(),
                success: function (response) {
                    if (response.success) {
                        alert("保存成功");
                        window.location.href = "cj_dd1.aspx?VERB=UPDATE&ID=" + response.message;
                    }
                    else {
                        alert(response.message);
                    }
                }
            });
        } //save

        function cancel() {
            $.ajax({ type: "POST",
                url: window.location.pathname + "?r=" + new Date().getTime() + "&ZT=0",
                data: $("form").serialize(),
                success: function (response) {
                    if (response.success) {
                        alert("保存成功");
                        window.location.href = "cj_dd1.aspx?VERB=UPDATE&ID=" + response.message;
                    }
                    else {
                        alert(response.message);
                    }
                }
            });
        } //cancel
    </script>

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
<form id="form1" runat="server">
<!--#include file="~/pro/DRP/include/qq.htm"-->
<uc1:topNav ID="topNav1" runat="server" />
<div id="wrapper">
    <input type="hidden" name="ID" value="<%=ID%>" />
    <input type="hidden" name="TD_ID" value="<%=TD_ID%>" />
    <input type="hidden" name="VERB" value="<%=VERB%>" />

    <div id="header">
        <div id="title">同行分销 — 出境线路订单</div>
        <!--#include file="~/pro/DRP/include/menu.htm"-->
    </div><!--header-->

    <div id="exception" runat="server" />

    <div id="left">
    <%if (VERB.ToUpper() == "UPDATE"){%>
    <div id="DDBH">订单编号：<span id="BH" runat="server"></span></div>
    <%}%>
    <h2>线路信息</h2>
    <div class="box">
        <div id="XLXX">
        <table>
        <tbody>
        <tr>
            <th>团队编号</th>
            <td><span id="TD_BH" runat="server"></span><!--<input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>BH" />--></td>
            <th>线路名称</th>
            <td><span id="TD_XLMC" runat="server"></span><!--<input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>XLMC" />--></td>
            <th>住宿</th>
            <td><span id="TD_ZS" runat="server"></span><!--<input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>ZS" />--></td>
            <th>出发日期</th>
            <td><span id="TD_CFRQ" runat="server"></span><!--<input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>CFRQ" class="datepicker" />--></td>
            <th>材料截止日期</th>
            <td><span id="TD_CLJZRQ" runat="server"></span><!--<input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>CFRQ" class="datepicker" />--></td>
        </tr>
        <tr>
            <th>天数</th>
            <td><span id="TD_TS" runat="server"></span><!--<input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>TS" />--></td>
            <th>人数</th>
            <td><span id="TD_RS" runat="server"></span><!--<input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>RS" />--></td>
            <%if(VERB.ToUpper()=="INSERT"){%>
            <th>状态</th>
            <td><span id="ZT1" runat="server"></span><!--<input type="text" name="ZT1" />--></td>
            <th>行程</th>
            <td><span id="XC" runat="server"></span><!--<a target="_blank" href="cj_td_fj.aspx?ID=<%=TD_ID%>"><img src="../../pub/img/word.gif" alt="word" /></a>--></td>
            <th>签证材料</th>
            <td><span id="QZCL" runat="server"></span><!--<a target="_blank" href="cj_td_fj.aspx?ID=<%=TD_ID%>"><img src="../../pub/img/word.gif" alt="word" /></a>--></td>
        </tr>
        <tr>
            <th>已定</th>
            <td><span id="YDRS" runat="server"></span><!--<input type="text" name="YDRS" />--></td>
            <th>剩余</th>
            <td><span id="SYRS" runat="server"></span><!--<input type="text" name="SYRS" />--></td>
            <%}%>
            <th>发布人</th>
            <td><span id="TD_FBR" runat="server"></span><!--<input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_FBR"); else Response.Write("FBR1");%>" />--></td>
            <th>发布时间</th>
            <td colspan="3"><span id="TD_FBSJ" runat="server"></span><!--<input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>FBSJ" />--></td>
        </tr>
        </tbody>
        </table>
        </div>
    </div>

    <h2>费用计算</h2>
    <div class="box">
        <table>
        <thead>
        <tr>
            <th colspan="2">单房差(元)</th>
            <td colspan="5"><span id="TD_DFC" runat="server"></span><!--<input type="text" name="<%if(VERB.ToUpper()!="INSERT") Response.Write("TD_");%>DFC"  size="10" />--></td>
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
        <tbody id="FYJS" runat="server">
        </tbody>
        <tfoot><tr>
            <th colspan="7">费用合计(元)：<span id="FYHJ" runat="server">0.00</span></th></tr><tr>
            <th colspan="7">结算价格(元)：<span id="JSJG" runat="server">0.00</span></th>
        </tr></tfoot>
        </table>
    </div>

    <h2>游客名单</h2>
    <div class="box">
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
        <tbody id="YKMD" runat="server">
        </tbody>
        </table>
    </div>
    
    <h2>其他信息</h2>
    <div class="box">
        <div id="YDXX">
        <table>
        <tbody>
            <tr>
                <th>上车点</th>
                <td id="SCD" runat="server"></td>
                <th>订单备注</th>
                <td id="BZ" runat="server"><input type="text" name="BZ" size="55" /></td>
            </tr>
            <tr>
                <th>经办人</th>
                <td id="JBR" runat="server"><input type="text" name="JBR" /></td>
                <th>经办人手机</th>
                <td id="JBRSJ" runat="server"><input type="text" name="JBRSJ" size="55" /></td>
            </tr>
            <%if (VERB.ToUpper() == "UPDATE"){%>
            <tr>
                <th>预定用户</th>
                <td id="YDR" runat="server"><input type="text" name="YDR1" /></td>
                <th>预定时间</th>
                <td id="YDSJ" runat="server"><input type="text" name="YDSJ" size="55"/></td>
            </tr>
            <tr>
                <th>订单状态</th>
                <td colspan="3" id="ZT" runat="server">
                    <select name="ZT">
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
</body>
</html>

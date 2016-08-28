<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cj_td1.aspx.cs" Inherits="pro_DRP_cj_td1" ViewStateMode="Disabled" %>
<%@ Register src="../include/topNav.ascx" tagname="topNav" tagprefix="uc1" %>
<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>同行分销 — 出境线路</title>

    <link href="../../pub/themes/cupertino/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../../pub/css/common.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        h2{padding:8px 0; color:#00A0E8; font-weight:bolder;}
        .box{padding:16px 0; border-top:2px solid #FFC20D;}
        .box table{border-collapse:collapse; border:1px solid #DDDDDD; width:100%;}
        .box th{margin:0; padding:4px; white-space:nowrap; border:1px solid #DCDCDC; /*background: #DCDCDC;*/}
        .box td{margin:0; padding:4px; /*white-space:nowrap;*/ border:1px solid #DCDCDC;}
        .toolbar{padding:16px 0; border-top:2px solid #FFC20D;}
        #exception{font-size: 200%; color:#f00; font-weight:bold; text-align:center; background:#ff0;}
    </style>

    <script src="../../pub/js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../../pub/js/ajaxfileupload.js" type="text/javascript"></script>
    <script src="../../pub/js/handlebars-v3.0.1.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery.loadJSON.js" type="text/javascript"></script>
    <script src="../../pub/js/common.js" type="text/javascript"></script>
    <!--<script src="cj_td1.js" type="text/javascript"></script>-->
    <script type="text/javascript">
        $(document).ready(function () {
            $(".btn").button();
            $(".btnDisable").button().button("disable");
            datepicker();
            var BJXXTemplate = Handlebars.compile($("#BJXXTemplate").html());

            //ajax附件上传
            $("input[type='file']").live("change", function () {
                $("#loading").show();
                $this = $(this);
                $next = $this.next();
                $.ajaxFileUpload({
                    url: "cj_td_fj.aspx",
                    secureuri: false,
                    dataType: 'json',
                    fileElementId: $this.attr("ID"),
                    success: function (response, status) {
                        $("#loading").hide();
                        if (response.success) {
                            $next.val(response.message);
                        }
                        else alert("上传失败：" + response.message);
                    }
                });
            });

            // 报价数量变化事件
            $("#BJSL input").live("blur", function () {
                $this = $(this);
                // 报价中已有数量
                var YYSL = $("#BJXX tr").length;
                // 整数判断
                var BJSL = parseInt($this.val());
                if (isNaN(BJSL)) $this.val(YYSL);
                else $this.val(BJSL);
                // 添加报价信息
                if (BJSL > YYSL) {
                    for (var sn = $("#BJXX tr").length + 1; sn <= BJSL; sn++) {
                        $("#BJXX").append(BJXXTemplate({ sn: sn }));
                    }
                }
                // 删除报价信息
                if (BJSL < YYSL) {
                    for (var sn = 1; sn <= YYSL - BJSL; sn++) {
                        $("#BJXX tr").eq(-1).remove();
                    }
                }
            });

            if ($("#ZT").text() !== "开通") {
                $("#预定").button({ disabled: true });
            }
            if (parseInt($("#SYRS").text()) <= 0) {
                $("#预定").button({ disabled: true }).find("span").text("已订满");
            }

        });

        // 保存或发布
        function save(VERB) {
            $(".toolbar a").button("disable");
            $.ajax({ type: "POST",
                url: window.location.pathname + "?r=" + new Date().getTime() + "&VERB=" + VERB,
                data: $("form").serialize(),
                success: function (response) {
                    $(".toolbar a").button("enable");
                    if (response.success) {
                        alert("操作成功");
                        if (VERB.toUpperCase() === "COPY")
                            window.location.href = "cj_td1.aspx?VERB=UPDATE&ID=" + response.message;
                        else
                            window.location.href = "cj_td1.aspx?VERB=SELECT&ID=" + response.message;
                    }
                    else {
                        alert(response.message);
                    }
                }
            });
        } //save
    </script>
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
<form id="form1" runat="server">
<!--#include file="~/pro/DRP/include/qq.htm"-->
<uc1:topNav ID="topNav1" runat="server" />
<div id="wrapper">
    <input type="hidden" name="ID" value="<%=ID%>" />
    
    <div id="header">
        <div id="title">同行分销 — 出境线路</div>
        <!--#include file="~/pro/DRP/include/menu.htm"-->
    </div><!--header-->
    
    <div id="exception" runat="server" />

    <h2>基本信息</h2>
    <div class="box">
        <table>
            <tbody>
            <tr>
                <th>团队编号</th>
                <td><span id="BH" runat="server"></span></td>
                <th>线路名称</th>
                <td><span id="XLMC" runat="server"></span></td>
                <th>住宿</th>
                <td><span id="ZS" runat="server"></span></td>
                <th>出发日期</th>
                <td><span id="CFRQ" runat="server"></span></td>
                <th>材料截止日期</th>
                <td><span id="CLJZRQ" runat="server"></span></td>
            </tr>
            <tr>
                <th>天数</th>
                <td><span id="TS" runat="server"></span></td>
                <th>人数</th>
                <td><span id="RS" runat="server"></span></td>
                <th>状态</th>
                <td><span id="ZT" runat="server"></span></td>
                <th>行程</th>
                <td><span id="XC" runat="server"></span></td>
                <th>签证材料</th>
                <td><span id="QZCL" runat="server"></span></td>
            </tr>
            <%if (VERB.ToUpper() == "SELECT"){%>
            <tr>
                <th>已定</th>
                <td><span id="YDRS" runat="server" /></td>
                <th>剩余</th>
                <td><span id="SYRS" runat="server" /></td>
                <th>发布人</th>
                <td><span id="FBR" runat="server" /></td>
                <th>发布时间</th>
                <td colspan="3"><span id="FBSJ" runat="server" /></td>
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
            <td><span id="BJSL" runat="server"></span></td>
            <th>单房差(元)</th>
            <td><span id="DFC" runat="server"></span></td>
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
        <tbody id="BJXX" runat="server">
        </tbody>
        </table>
    </div><!--box-->

    <div class="toolbar">
        <%if (VERB.ToUpper() == "SELECT"){%>
              <a href="cj_dd1.aspx?VERB=INSERT&TD_ID=<%=ID%>" class="btn" id="预定">预定</a>
        <%}%>
        <%if (VERB.ToUpper() == "SELECT" && User.IsInRole("ADMIN")){%>
              <a href="cj_td1.aspx?VERB=UPDATE&ID=<%=ID%>" class="btn" id="编辑">编辑</a>
              <a href="javascript:void(0);" onclick="save('COPY');" class="btn" id="复制">复制</a>
        <%}%>
        <%if (VERB.ToUpper() == "INSERT"){%>
        <a href="javascript:void(0);" onclick="save('INSERT');" class="btn">发布</a>
        <%}%>
        <%if (VERB.ToUpper() == "UPDATE"){%>
        <a href="javascript:void(0)" onclick="save('UPDATE');" class="btn">保存</a>
        <%}%>
        <a href="javascript:window.opener=null;window.open('','_self');window.close();" class="btn">关闭</a>
    </div><!--toolbar-->
</div><!--wrapper-->
</form>

<div id="loading">
    <img src="../../pub/img/loading.GIF" alt="loading..."/>
</div><!--loading-->

</body>
</html>

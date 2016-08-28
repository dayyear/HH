<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="index" %>
<%@ Register src="../include/topNav.ascx" tagname="topNav" tagprefix="uc1" %>
<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>产品部证照管理</title>
    <link href="../../pub/themes/cupertino/jquery-ui.css" rel="stylesheet" type="text/css" />
    <!--<link href="../../pub/jquery-ui-1.11.4/themes/cupertino/jquery-ui.min.css" rel="stylesheet" type="text/css" />-->
    <link href="../../pub/css/common.css" rel="stylesheet" type="text/css" />
    <link href="Default.css" rel="stylesheet" type="text/css" />

    <script src="../../pub/js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery.bgiframe-2.1.2.js" type="text/javascript"></script>
    <script src="../../pub/js/ajaxfileupload.js" type="text/javascript"></script>
    <script src="../../pub/js/handlebars-v3.0.1.js" type="text/javascript"></script>
    <script src="../../pub/js/jquery.loadJSON.js" type="text/javascript"></script>
    <script src="../../pub/js/common.js" type="text/javascript"></script>
    <script src="Default.js" type="text/javascript"></script>

    <script type="text/template" id="resultTemplate">
        <tr>
            <td class="sn">{{sn}}</td>
            <td>{{XM}}</td>
            <td>{{XB1}}</td>
            <td>{{CSRQ}}</td>
            <!--<td>{{LXFS}}</td>-->
            <td>{{YYBZRQ}}</td>
            <td>{{HZD}}</td>
            <td style="white-space:normal;">{{BZJD}}</td>
            <td>{{CXSJ}}</td>
            <td>{{ZJZL1}}</td>
            <td>{{ZJHM}}</td>
            <td>{{YWY1}}</td>
            <td>{{TDID}}</td>
            <td>{{LRSJ}}</td>
        </tr>
    </script>

</head>
<body>
<uc1:topNav runat="server" />
    <div id="wrapper">

        <div id="header">
            <div id="title">产品部证照管理</div>
        </div><!--header-->

        <div id="condition" class="ui-widget-content ui-corner-all">
        <form action="#"><table>
            <tr>
                <th>姓名</th>
                <td><input type="text" name="XM" /></td>
                <th>联系方式</th>
                <td><input type="text" name="LXFS" /></td>
                <th>出生日期</th>
                <td><input type="text" name="CSRQ" class="datepicker" /></td>
                <th>性别</th>
                <td><select name="XB">
                    <option value="">全部</option>
                    <option value="1">男</option>
                    <option value="2">女</option>
                </select></td>
            </tr>
            <tr>
                <th>预约办证日期</th>
                <td><input type="text" name="YYBZRQ" class="datepicker" /></td>
                <th>回执单</th>
                <td><input type="text" name="HZD" /></td>
                <th>办证进度</th>
                <td><input type="text" name="BZJD" /></td>
                <th>证件种类</th>
                <td><select name="ZJZL">
                    <option value="">全部</option>
                    <%=optionZJZL%>
                </select></td>
                <th>证件号码</th>
                <td><input type="text" name="ZJHM" /></td>
            </tr>
            <tr>
                <th>团队编号</th>
                <td><input type="text" name="TDID" /></td>
                <th>业务员</th>
                <td><select name="YWY">
                    <option value="">全部</option>
                    <%=optionYWY%>
                </select></td>
                <th>显示行数</th>
                <td><select id="rows">
                    <option value="10">10</option>
                    <option value="50">50</option>
                    <option value="100">100</option>
                    <option value="500">500</option>
                    <option value="1000">1000</option>
                </select></td>
                <td colspan="2" align="right">
                    <input type="submit" value="查询" />
                    <input type="reset" value="清空" />
                </td>
            </tr>
        </table></form>
        </div><!--condition-->

        <div id="toolbar">
            <input type="button" id="新增" value="新增" />
            <input type="button" id="编辑" value="编辑" />
            <input type="button" id="删除" value="删除" />
            <input type="button" id="预约办证名单打印" value="预约办证名单打印" />
            <input type="button" id="团队名单打印" value="团队名单打印" />
            <input type="button" id="团队电子材料打包" value="团队电子材料打包" />
        </div><!--toolbar-->

        <div id="result" style="display:none;">
            <div id="datagrid">
                <table>
                <thead>
                <tr>
                    <th class="sn" rowspan="2">序号</th>
                    <th colspan="3">个人信息</th>
                    <th colspan="6">办证信息</th>
                    <th colspan="3">其他</th>
                </tr>
                <tr>
                    <th class="">姓名</th>
                    <th class="">性别</th>
                    <th class="">出生日期</th>
                    <!--<th class="">联系方式</th>-->
                    <th class="">预约日期</th>
                    <th class="">回执单</th>
                    <th class="">办证进度</th>
                    <th class="">查询时间</th>
                    <th class="">证件种类</th>
                    <th class="">证件号码</th>
                    <th class="">业务员</th>
                    <th class="">团队编号</th>
                    <th class="">录入时间</th>
                </tr>
                </thead>
                <tbody>
                </tbody>
                </table>
            </div><!--datagrid-->

            <div id="pagination">
                <table>
                <tr>
                    <td>
                        <span>共有<strong id="total">0</strong>条记录，</span>
                        <span>第<input type="text" id="page" disabled="disabled" value="0" style="width:30px;" />页</span>
                        <span>共<strong id="totalpage">0</strong>页</span>
                    </td>
                    <td>
                        <input type="button" value="首页" id="首页"/>
                        <input type="button" value="上一页" id="上一页"/>
                        <input type="button" value="下一页" id="下一页"/>
                        <input type="button" value="尾页" id="尾页"/>
                    </td>
                    <td>
                        <span>[注]：查询结果按照<strong>录入时间</strong>倒序排列</span>
                    </td>
                </tr>
                </table>
            </div><!--pagination-->
        </div><!--result-->

    </div><!--wrapper-->

    <div id="loading">
        <img src="../../pub/img/loading.GIF" alt="loading..."/>
    </div><!--loading-->

    <div id="dialog-change-password" style="display:none;">
        <form action="#">
        <input type="reset" value="清空" style="display:none;" />
        <table>
            <tr>
                <th>旧密码</th>
                <td><input type="password" name="old_password" /></td>
            </tr>
            <tr>
                <th>新密码</th>
                <td><input type="password" name="new_password" /></td>
            </tr>
            <tr>
                <th>确认新密码</th>
                <td><input type="password" name="new_password2" /></td>
            </tr>
        </table></form>
    </div><!--dialog-change-password-->

    <div id="dialog-insert" style="display:none;">
        <form action="#">
        <input type="reset" value="清空" style="display:none;" />
        <table>
            <tr>
                <th>团队编号</th>
                <td><input type="text" name="TDID" /></td>
            </tr>
            <tr>
                <th>证件种类</th>
                <td><select name="ZJZL">
                    <option value="">请选择</option>
                    <%=optionZJZL%>
                </select></td>
            </tr>
            <tr>
                <th>姓名</th>
                <td><input type="text" name="XM" /></td>
            </tr>
            <tr>
                <th>性别</th>
                <td><select name="XB">
                    <option value="">请选择</option>
                    <option value="1">男</option>
                    <option value="2">女</option>
                </select></td>
            </tr>
            <tr>
                <th>出生日期</th>
                <td><input type="text" name="CSRQ" class="datepicker" /></td>
            </tr>
            <tr>
                <th>联系方式</th>
                <td><input type="text" name="LXFS" /></td>
            </tr>
            <tr>
                <th>预约办证日期</th>
                <td><input type="text" name="YYBZRQ" class="datepicker" /></td>
            </tr>
        </table></form>
    </div><!--dialog-insert-->
    
    <div id="dialog-update" style="display:none;overflow:hidden;">
        <form action="#" class="text">
        <table>
            <tr>
                <th colspan="4">个人信息</th>
            </tr>
            <tr>
                <th>姓名</th>
                <td><input type="text" name="XM" /></td>
                <th>拼音姓名</th>
                <td><input type="text" name="PYXM" /></td>
            </tr>
            <tr>
                <th>性别</th>
                <td><select name="XB">
                    <option value="">请选择</option>
                    <option value="1">男</option>
                    <option value="2">女</option>
                </select></td>
                <th>出生日期</th>
                <td><input type="text" name="CSRQ" class="datepicker" /></td>
            </tr>
            <tr>
                <th>联系方式</th>
                <td><input type="text" name="LXFS" /></td>
                <th>职业</th>
                <td><input type="text" name="ZY" /></td>
            </tr>
            <tr>
                <th>出境记录</th>
                <td><select name="CJJL">
                    <option value="">请选择</option>
                    <%=optionCJJL%>
                </select></td>
                <th>婚姻状况</th>
                <td><select name="HYZK">
                    <option value="">请选择</option>
                    <%=optionHYZK%>
                </select></td>
            </tr>
            <tr>
                <th>出生地</th>
                <td colspan="3"><select name="CSD">
                    <option value="">请选择</option>
                    <%=optionCSD%>
                </select></td>
            </tr>
            <tr>
                <th colspan="4">办证信息</th>
            </tr>
            <tr>
                <th>预约办证日期</th>
                <td><input type="text" name="YYBZRQ" class="datepicker" /></td>
                <th>回执单</th>
                <td>
                    <input type="text" name="HZD" />
                    <img src="../../pub/img/police20.PNG" alt="进度查询" id="进度查询" style="cursor:pointer;"/>
                </td>
            </tr>
            <tr>
                <th>证件种类</th>
                <td><select name="ZJZL">
                    <%=optionZJZL%>
                </select></td>
                <th>办证进度</th>
                <td><input type="text" name="BZJD" /></td>
            </tr>
            <tr>
                <th>证件号码</th>
                <td><input type="text" name="ZJHM" /></td>
                <th>查询时间</th>
                <td><input type="text" name="CXSJ" /></td>
            </tr>
            <tr>
                <th>签发日期</th>
                <td><input type="text" name="QFRQ" class="datepicker" /></td>
                <th>有效期至</th>
                <td><input type="text" name="YXQZ" class="datepicker" /></td>
            </tr>
            <tr>
                <th>签发地</th>
                <td colspan="3"><select name="QFD">
                    <option value="">请选择</option>
                    <%=optionQFD%>
                </select></td>
            </tr>
            <tr>
                <th colspan="4">其他</th>
            </tr>
            <tr>
                <th>业务员</th>
                <td><select name="YWY" disabled="disabled">
                    <option value="">无</option>
                    <%=optionYWY%>
                </select>
                </td>

                <th>团队编号</th>
                <td><input type="text" name="TDID" /></td>
            </tr>
            <tr>
                <th>身份确认</th>
                <td><select name="SFQR">
                    <option value="">请选择</option>
                    <%=optionSFQR%>
                </select></td>
                <th>经济能力确认</th>
                <td><input type="text" name="JJNLQR" /></td>
            </tr>
            <tr>
                <th>金额</th>
                <td><input type="text" name="JE" /></td>
                <th>销售</th>
                <td><input type="text" name="XS" /></td>
            </tr>
            <tr>
                <th>录入时间</th>
                <td><input type="text" name="LRSJ" disabled="disabled" /></td>
                <th>用房</th>
                <td><input type="text" name="YF" /></td>
            </tr>
            <tr>
                <th>地区</th>
                <td><input type="text" name="DQ" /></td>
                <th>备注</th>
                <td><input type="text" name="BZ" /></td>
            </tr>
        </table>
        <input type="hidden" name="PASS1" />
        <input type="hidden" name="PHOTO" />
        <input type="hidden" name="IDCARD1" />
        <input type="hidden" name="IDCARD2" />
        <input type="hidden" name="HOUSEHOLD1" />
        <input type="hidden" name="HOUSEHOLD2" />
        <input type="hidden" name="HOUSEHOLD3" />
        <input type="hidden" name="HOUSEHOLD4" />
        <input type="hidden" name="BIRTH" />
        <input type="hidden" name="CONSENT" />
        <input type="hidden" name="OTHER1" />
        <input type="hidden" name="OTHER2" />
        </form>

        <div class="buttonpane">
            <input type="button" value="保存" />
            <input type="button" value="取消" />
        </div>

        <form class="image" action="#">
            <img alt="证件"id="img-pass1" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>证件</td>
            <td><input type="file" name="file" id="file-PASS1" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="PASS1" />
            <input type="hidden" name="ID" value="" />
        </form>
        <form class="image" action="#">
            <img alt="照片" id="img-photo" src="#" title="点击查看原图" /><br />
            <table border="0"><tr>
            <td>照片</td>
            <td><input type="file" name="file" id="file-PHOTO" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="PHOTO" />
            <input type="hidden" name="ID" value="" />
        </form>
        <form class="image" action="#">
            <img alt="身份证1" id="img-idcard1" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>身份证1</td>
            <td><input type="file" name="file" id="file-IDCARD1" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="IDCARD1" />
            <input type="hidden" name="ID" value="" />
        </form>
        <form class="image" action="#">
            <img alt="身份证2" id="img-idcard2" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>身份证2</td>
            <td><input type="file" name="file" id="file-IDCARD2" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="IDCARD2" />
            <input type="hidden" name="ID" value="" />
        </form>
        <form class="image" action="#">
            <img alt="户口簿1"id="img-household1" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>户口簿1</td>
            <td><input type="file" name="file" id="file-HOUSEHOLD1" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="HOUSEHOLD1" />
            <input type="hidden" name="ID" value="" />
        </form>

        <form class="image" action="#">
            <img alt="户口簿2"id="img-household2" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>户口簿2</td>
            <td><input type="file" name="file" id="file-HOUSEHOLD2" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="HOUSEHOLD2" />
            <input type="hidden" name="ID" value="" />
        </form>

        <form class="image" action="#">
            <img alt="户口簿3"id="img-household3" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>户口簿3</td>
            <td><input type="file" name="file" id="file-HOUSEHOLD3" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="HOUSEHOLD3" />
            <input type="hidden" name="ID" value="" />
        </form>

        <form class="image" action="#">
            <img alt="户口簿4"id="img-household4" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>户口簿4</td>
            <td><input type="file" name="file" id="file-HOUSEHOLD4" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="HOUSEHOLD4" />
            <input type="hidden" name="ID" value="" />
        </form>
        <form class="image" action="#">
            <img alt="出生证明" id="img-birth" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>出生证明</td>
            <td><input type="file" name="file" id="file-BIRTH" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="BIRTH" />
            <input type="hidden" name="ID" value="" />
        </form>
        <form class="image" action="#">
            <img alt="同意书" id="img-consent" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>同意书</td>
            <td><input type="file" name="file" id="file-CONSENT" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="CONSENT" />
            <input type="hidden" name="ID" value="" />
        </form>
        <form class="image" action="#">
            <img alt="其他1" id="img-other1" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>其他1</td>
            <td><input type="file" name="file" id="file-OTHER1" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="OTHER1" />
            <input type="hidden" name="ID" value="" />
        </form>
        <form class="image" action="#">
            <img alt="其他2" id="img-other2" src="#" title="点击查看原图" /><br />
            <table><tr>
            <td>其他2</td>
            <td><input type="file" name="file" id="file-OTHER2" size="4" style="width:77px"/></td>
            <td><button style="height:23px;">删除</button></td>
            </tr></table>
            <input type="hidden" name="FIELD" value="OTHER2" />
            <input type="hidden" name="ID" value="" />
        </form>

        

    </div><!--dialog-update-->
    
    <div id="dialog-delete" style="display:none;">
        <h3>你确定要删除吗？删除之后将无法恢复数据！</h3>
    </div><!--dialog-delete-->

    <div id="report1" style="display:none;">
        <form action="report1.ashx" target="_blank" method="post">
        <input type="reset" style="display:none;" />
        <table>
            <tr>
                <th>预约办证日期</th>
                <td><input type="text" name="YYBZRQ" class="datepicker" /></td>
            </tr>
        </table>
        </form>
    </div><!--report1-->

    <div id="report2" style="display:none;">
        <form action="report2.ashx" target="_blank" method="post">
        <input type="reset" style="display:none;" />
        <table>
            <tr>
                <th>团队编号</th>
                <td><input type="text" name="TDID" /></td>
            </tr>
        </table>
        </form>
    </div><!--report2-->

    <div id="zip" style="display:none;">
        <form action="zip.ashx"target="_blank" method="post">
        <input type="reset" style="display:none;" />
        <table>
            <tr>
                <th>团队编号</th>
                <td><input type="text" name="TDID" /></td>
            </tr>
        </table>
        </form>
    </div><!--zip-->

</body>
</html>

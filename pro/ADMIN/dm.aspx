<%@ Page Title="参数设置" Language="C#" MasterPageFile="~/pri/master/MasterPage.master" AutoEventWireup="true" CodeFile="dm.aspx.cs" Inherits="pro_ADMIN_dm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="style" Runat="Server">
    <link href="../../pub/css/pagination.css" rel="stylesheet" type="text/css" />
    <link href="../../pub/css/datatables.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="script" Runat="Server">
    <script src="../../pub/js/jquery.pagination.js" type="text/javascript"></script>
    <script src="../../pub/js/mustache.min.js" type="text/javascript"></script>
    <script src="dm.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" Runat="Server">
<div id="wrapper">
    
    <div id="header">
        <div id="title">系统管理 — 参数设置</div>
        <!--#include file="include/menu.htm"-->
    </div><!--header-->

    <form action="#" class="condition">
        <table>
            <tr>
                <th>类型</th>
                <td><select name="LX">
                    <option value="">全部</option>
                    <%=optionLx%>
                </select></td>
                <th>代码</th>
                <td><input type="text" name="DM"/></td>
                <th>名称</th>
                <td><input type="text" name="MC"/></td>
                <th>是否有效</th>
                <td><select name="SFYX">
                    <option value="">全部</option>
                    <%=optionSfyx%>
                </select></td>
                <th>显示行数</th>
                <td>
                    <select id="rows">
                    <%=optionRows%>
                    </select>
                </td>
            </tr>
        </table>
        <p>
            <input type="submit" value="查询" class="btn"/>
            <input type="reset" value="清除条件" class="btn"/>
            <input type="button" value="新建参数" class="btn new"/>
        </p>
    </form>
    
    <table class="dataTable">
    <thead>
    <tr>
        <th>序号</th>
        <th>类型</th>
        <th>代码</th>
        <th>名称</th>
        <th>是否有效</th>
        <th>备注</th>
        <th>排序</th>
        <th>变动时间</th>
    </tr>
    </thead>
    <script id="template" type="x-tmpl-mustache">
    
        <td><span>{{RowNumber}}</span></td>
        <td><span>{{LX1}}</span></td>
        <td><span>{{DM}}</span></td>
        <td><span><a href="javascript:void(0);" class="edit">{{MC}}</a></span></td>
        <td><span>{{SFYX1}}</span></td>
        <td><span>{{BZ}}</span></td>
        <td><span>{{ORDER}}</span></td>
        <td><span>{{BDSJ}}</span></td>
    
    </script>
    <tbody>
    </tbody>
    </table>

    <div class="dataTablePagination">
    </div>
    <div class="dataTableInfo">
    </div>

</div><!--wrapper-->

<div id="dialog-edit" class="dialog ui-helper-hidden">
    <form action="#">
    <table>
        <tr>
            <th>类型</th>
            <td><input type="text" name="LX1" disabled="disabled"/><input type="hidden" name="LX"/></td>
        </tr>
        <tr>
            <th>代码</th>
            <td><input type="text" name="DM" disabled="disabled"/><input type="hidden" name="DM"/></td>
        </tr>
        <tr>
            <th>名称</th>
            <td><input type="text" name="MC"/></td>
        </tr>
        <tr>
            <th>是否有效</th>
            <td><select name="SFYX">
                <%=optionSfyx%>
            </select></td>
        </tr>
        <tr>
            <th>排序</th>
            <td><input type="text" name="ORDER"/></td>
        </tr>
        <tr>
            <th>备注</th>
            <td><input type="text" name="BZ"/></td>
        </tr>
    </table>
    </form>
</div><!--dialog-edit-->

<div id="dialog-new" class="dialog ui-helper-hidden">
    <form action="#">
    <table>
        <tr>
            <th>类型</th>
            <td><select name="LX">
                <option value=""></option>
                <%=optionLx%>
            </select></td>
        </tr>
        <tr>
            <th>代码</th>
            <td><input type="text" name="DM"/></td>
        </tr>
        <tr>
            <th>名称</th>
            <td><input type="text" name="MC"/></td>
        </tr>
        <tr>
            <th>是否有效</th>
            <td><select name="SFYX">
                <%=optionSfyx%>
            </select></td>
        </tr>
        <tr>
            <th>排序</th>
            <td><input type="text" name="ORDER"/></td>
        </tr>
        <tr>
            <th>备注</th>
            <td><input type="text" name="BZ"/></td>
        </tr>
    </table>
    </form>
</div><!--dialog-new-->
</asp:Content>


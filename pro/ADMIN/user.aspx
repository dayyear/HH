<%@ Page Title="用户管理" Language="C#" MasterPageFile="~/pri/master/MasterPage.master" AutoEventWireup="true" CodeFile="user.aspx.cs" Inherits="pro_ADMIN_user" %>
<asp:Content ID="Content1" ContentPlaceHolderID="style" Runat="Server">
    <link href="../../pub/css/pagination.css" rel="stylesheet" type="text/css" />
    <link href="../../pub/css/datatables.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="script" Runat="Server">
    <script src="../../pub/js/jquery.pagination.js" type="text/javascript"></script>
    <script src="../../pub/js/mustache.min.js" type="text/javascript"></script>
    <script src="user.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" Runat="Server">
<div id="wrapper">
    
    <div id="header">
        <div id="title">系统管理 — 用户管理</div>
        <!--#include file="include/menu.htm"-->
    </div><!--header-->

    <form action="#" class="condition">
        <table>
            <tr>
                <th>登录名称</th>
                <td><input type="text" name="loginname"/></td>
                <th>用户名称</th>
                <td><input type="text" name="username"/></td>
                <th>用户角色</th>
                <td><select name="roleID">
                    <option value="">全部</option>
                    <%=optionRoleID%>
                </select></td>
                <th>是否有效</th>
                <td><select name="sfyx">
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
            <input type="button" value="新建用户" class="btn new"/>
        </p>
    </form>
    
    <table class="dataTable">
    <thead>
    <tr>
        <th>序号</th>
        <th>登录名称</th>
        <th>用户名称</th>
        <th>用户角色</th>
        <th>是否有效</th>
        <th>变动时间</th>
    </tr>
    </thead>
    <script id="template" type="x-tmpl-mustache">
    
        <td><span>{{RowNumber}}</span></td>
        <td><span>{{loginname}}</span></td>
        <td><span><a href="javascript:void(0);" class="edit">{{username}}</a></span></td>
        <td><span>{{roleID1}}</span></td>
        <td><span>{{sfyx1}}</span></td>
        <td><span>{{bdsj}}</span></td>
    
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
            <th>ID</th>
            <td><input type="text" name="ID" disabled="disabled"/><input type="hidden" name="ID"/></td>
        </tr>
        <tr>
            <th>登录名称</th>
            <td><input type="text" name="loginname" disabled="disabled"/><input type="hidden" name="loginname"/></td>
        </tr>
        <tr>
            <th>用户名称</th>
            <td><input type="text" name="username"/></td>
        </tr>
        <tr>
            <th>用户角色</th>
            <td><select name="roleID">
                <%=optionRoleID%>
            </select></td>
        </tr>
        <tr>
            <th>是否有效</th>
            <td><select name="sfyx">
                <%=optionSfyx%>
            </select></td>
        </tr>
    </table>
    </form>
</div><!--dialog-edit-->

<div id="dialog-new" class="dialog ui-helper-hidden">
    <form action="#">
    <table>
        <tr>
            <th>登录名称</th>
            <td><input type="text" name="loginname"/></td>
        </tr>
        <tr>
            <th>用户名称</th>
            <td><input type="text" name="username"/></td>
        </tr>
        <tr>
            <th>密码</th>
            <td><input type="password" name="password"/></td>
        </tr>
        <tr>
            <th>用户角色</th>
            <td><select name="roleID">
                <%=optionRoleID%>
            </select></td>
        </tr>
        <tr>
            <th>是否有效</th>
            <td><select name="sfyx">
                <%=optionSfyx%>
            </select></td>
        </tr>
    </table>
    </form>
</div><!--dialog-new-->
</asp:Content>


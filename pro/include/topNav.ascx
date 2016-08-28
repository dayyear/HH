<%@ Control Language="C#" AutoEventWireup="true" CodeFile="topNav.ascx.cs" Inherits="pro_include_topNav" %>
<div id="topNav">
<div id="topBar">
    <div id="loginNav">
        <a href="<%=Page.ResolveClientUrl("~/pro")%>">首页</a>
        <%if(Page.User.IsInRole("TWN")){%>
        <span class="pipe">|</span>
        <a href="<%=Page.ResolveClientUrl("~/pro/TWN")%>">产品部证照管理</a>
        <%}%>
        <%if(Page.User.IsInRole("DRP")){%>
        <span class="pipe">|</span>
        <a href="<%=Page.ResolveClientUrl("~/pro/DRP")%>">同行分销</a>
        <%}%>
        <%if(Page.User.IsInRole("ADMIN")){%>
        <span class="pipe">|</span>
        <a href="<%=Page.ResolveClientUrl("~/pro/ADMIN/dm.aspx")%>">系统管理</a>
        <%}%>
    </div>
    <div id="loginWrap">
        <a id="username"><%=username%></a>
        <span class="pipe">|</span>
        <a id="change_password" href="../pwd.aspx" runat="server">修改密码</a>
        <span class="pipe">|</span>
        <a id="logout" href="<%=Page.ResolveClientUrl("~/pub/login/logout.aspx")%>">退出</a>
    </div>
</div>
</div>

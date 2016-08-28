<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        //在应用程序启动时运行的代码
        
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //在应用程序关闭时运行的代码
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        //在出现未处理的错误时运行的代码
    }

    void Session_Start(object sender, EventArgs e) 
    {
        //在新会话启动时运行的代码
    }

    void Session_End(object sender, EventArgs e) 
    {
        //在会话结束时运行的代码。 
        // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
        // InProc 时，才会引发 Session_End 事件。如果会话模式 
        //设置为 StateServer 或 SQLServer，则不会引发该事件。
    }
    
    protected void Application_AuthenticateRequest(Object sender, EventArgs e)
    {
        HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        if (authCookie != null)
        {
            //Extract the forms authentication cookie
            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            // If caching roles in userData field then extract
            string[] roles = authTicket.UserData.Split(new char[] { ',' });

            // Create the IIdentity instance
            System.Security.Principal.IIdentity id = new FormsIdentity(authTicket);

            // Create the IPrinciple instance
            System.Security.Principal.IPrincipal principal = new System.Security.Principal.GenericPrincipal(id, roles);

            // Set the context user 
            Context.User = principal;
        }
    }
</script>

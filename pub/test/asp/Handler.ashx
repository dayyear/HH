<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

public class Handler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Server.Execute("Default2.aspx");
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
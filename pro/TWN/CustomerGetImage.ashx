<%@ WebHandler Language="C#" Class="CustomerGetImage" %>

using System.Web;

public class CustomerGetImage : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var Response = context.Response;
        var Request = context.Request;

        Response.ContentType = "image/png";
        Response.Clear();
        Response.BufferOutput = true;

        try { Response.BinaryWrite(Twn.CustomerImage.Select(Request["ID"], Request["FIELD"])); }
        catch { Response.WriteFile("~/pub/img/noimg.png"); }
        finally { Response.End(); }
    }//ProcessRequest

    public bool IsReusable { get { return true; } }

}//class
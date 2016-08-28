<%@ WebHandler Language="C#" Class="userJson" %>

using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;

public class userJson : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        var Request = context.Request;
        var Response = context.Response;

        //////////////////////////////////////////////////////////////////////////
        // 初始化
        //////////////////////////////////////////////////////////////////////////
        Response.ContentType = "application/json";
        Response.Clear();
        Response.BufferOutput = true;

        var success = true;
        var message = "";
        var total = 0;
        var items = new List<Dictionary<string, object>>();

        // 动词
        var VERB = Request["VERB"];

        try
        {
            Common.ValidString(VERB, CanNull.False, CanEmpty.False, "动词VERB");
            switch (VERB.Trim().ToUpper())
            {
                case "SELECT":
                    items = ADMIN.User.Select(out total, Request["ID"], Request["loginname"], Request["username"], Request["roleID"], Request["sfyx"], Request["rows"], Request["page"], Request["order"]);
                    break;
                case "INSERT":
                    total = ADMIN.User.Insert(Request["loginname"], Request["username"], Request["password"], Request["roleID"], Request["sfyx"]);
                    break;
                case "UPDATE":
                    total = ADMIN.User.Update(Request["username"], Request["roleID"], Request["sfyx"], Request["ID"]);
                    break;
                case "RESET":
                    total = ADMIN.User.Reset(Request["ID"]);
                    break;
                case "DELETE":
                    total = ADMIN.User.Delete(Request["ID"]);
                    break;
                default:
                    throw new Exception("无效的[VERB]: " + VERB);
            }
        }
        catch (Exception ex)
        {
            success = false;
            message = ex.Message;
            total = 0;
            items.Clear();
        }

        Response.Write(JsonConvert.SerializeObject(new { success, message, total, items }, Formatting.Indented));
        Response.End();
    }//ProcessRequest

    public bool IsReusable { get { return true; } }

}//class
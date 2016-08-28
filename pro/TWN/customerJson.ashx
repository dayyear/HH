<%@ WebHandler Language="C#" Class="customerJson" %>

using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;

public class customerJson : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var Request = context.Request;
        var Response = context.Response;
        var User = context.User;

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
                    items = Twn.Customer.Select(out total, Request["ID"], Request["XM"], Request["LXFS"], Request["CSRQ"], Request["XB"], Request["YYBZRQ"], Request["HZD"], Request["BZJD"], Request["ZJHM"], Request["TDID"], Request["YWY"], Request["ZJZL"], Request["LRSJ1"], Request["LRSJ2"], Request["rows"], Request["page"], Request["order"]); 
                    break;
                case "INSERT":
                    total = Twn.Customer.Insert(Request["TDID"], Request["ZJZL"], Request["XM"], Request["XB"], Request["CSRQ"], Request["LXFS"], Request["YYBZRQ"], User);
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
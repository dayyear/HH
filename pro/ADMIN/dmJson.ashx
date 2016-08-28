<%@ WebHandler Language="C#" Class="dmJson" %>

using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;

public class dmJson : IHttpHandler
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
                    var result =  ADMIN.DM.SelectWithTotal(Request["LX"], Request["DM"], Request["MC"], Request["SFYX"], Request["rows"], Request["page"], Request["order"]);
                    items = result.Item1;
                    total = result.Item2;
                    break;
                case "INSERT":
                    total = ADMIN.DM.Insert(Request["LX"], Request["DM"], Request["MC"], Request["SFYX"], Request["BZ"], Request["ORDER"]);
                    break;
                case "UPDATE":
                    total = ADMIN.DM.Update(Request["LX"], Request["DM"], Request["MC"], Request["SFYX"], Request["BZ"], Request["ORDER"]);
                    break;
                case "DELETE":
                    total = ADMIN.DM.Delete(Request["LX"], Request["DM"]);
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
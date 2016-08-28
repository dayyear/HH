<%@ WebHandler Language="C#" Class="BZJDCX" %>

using System;
using System.Web;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

public class BZJDCX : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        HttpResponse Response = context.Response;
        HttpServerUtility Server = context.Server;

        //////////////////////////////////////////////////////////////////////////
        // 初始化
        //////////////////////////////////////////////////////////////////////////
        Response.ContentType = "application/json";
        Response.Clear();
        Response.BufferOutput = true;

        bool success = true;
        string message = null;
        int total = 0;
        List<Dictionary<string, Object>> items = new List<Dictionary<string, object>>();

        // 表单
        string HZD = context.Request["HZD"];

        try
        {
            if (string.IsNullOrWhiteSpace(HZD))
                throw new Exception(string.Format("[回执单]不能为空"));
            HZD = HZD.Trim();

            CertificateScheduleWeb web = new CertificateScheduleWeb("jsbee.com.cn", "80");
            
            int c = 1;
            string responseString = web.Query(HZD);

            Match match = Regex.Match(responseString, CertificateScheduleWeb.ALERT_Pattern);
            if (match.Success) throw new Exception(match.Groups[1].Value);

            while (responseString.Contains("20秒"))
            {
                System.Threading.Thread.Sleep(3000);
                responseString = web.Query(HZD);
                if (++c > 10) throw new Exception("查询超时");
            }

            match = Regex.Match(responseString, CertificateScheduleWeb.BZJD_Pattern);
            if (!match.Success) throw new Exception("未知错误");

            success = true;
            message = match.Groups[1].Value;
        }
        catch (Exception ex)
        {
            success = false;
            message = ex.Message;
            total = 0;
            items.Clear();
        }


        Response.Write(JsonConvert.SerializeObject(new
        {
            success = success,
            message = message,
            total = total,
            items = items
        }, Formatting.Indented));

        Response.End();
    }

    public bool IsReusable { get { return true; } }

}
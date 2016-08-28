<%@ WebHandler Language="C#" Class="CustomerUploadImage" %>

using System;
using System.Web;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

public class CustomerUploadImage : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        HttpServerUtility Server = context.Server;

        //////////////////////////////////////////////////////////////////////////
        // 初始化
        //////////////////////////////////////////////////////////////////////////
        Response.ContentType = "application/json";
        Response.ContentType = "text/html";
        Response.Clear();
        Response.BufferOutput = true;

        bool success = true;
        string message = null;
        int total = 0;
        List<Dictionary<string, Object>> items = new List<Dictionary<string, object>>();

        // 表单
        string ID = Request["ID"];
        string FIELD = Request["FIELD"];

        try
        {
            if (string.IsNullOrWhiteSpace(ID))
                throw new Exception("[主键]不能为空");
            ID = ID.Trim();
            try { int.Parse(ID); }
            catch (Exception) { throw new Exception(string.Format("[主键]格式不正确: {0}", ID)); }

            if (string.IsNullOrWhiteSpace(FIELD))
                throw new Exception("[字段名]不能为空");
            if (!(new List<string>() { "PASS1", "PHOTO", "IDCARD1", "IDCARD2", "HOUSEHOLD1", "HOUSEHOLD2", "HOUSEHOLD3", "HOUSEHOLD4", "BIRTH", "CONSENT", "OTHER1", "OTHER2" }).Contains(FIELD.ToUpper()))
                throw new Exception("无效的[字段名]：" + FIELD);

            HttpPostedFile file = Request.Files["file"];
            if (file == null) throw new Exception("[上传图片]不能为空");
            if (file.ContentLength > 512 * 1024) throw new Exception("[上传图片]不能超过512K，请缩小图片尺寸");
            try { using (Bitmap bitmap = new Bitmap(file.InputStream)) bitmap.Dispose(); }
            catch (Exception) { throw new Exception("[上传图片]不是有效的图片文件"); }
            
            string guid = Guid.NewGuid().ToString();
            file.SaveAs(Server.MapPath("~/pub/attachment/" + guid + ".jpg"));
            message = "/pub/attachment/" + guid + ".jpg";


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
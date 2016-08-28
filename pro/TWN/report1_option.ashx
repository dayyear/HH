<%@ WebHandler Language="C#" Class="report1_option" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;

public class report1_option : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        var Request = context.Request;
        var Response = context.Response;
        var Server = context.Server;

        try
        {
            var YYBZRQ = Request["YYBZRQ"];
            Common.ValidDateTime(YYBZRQ, CanNull.False, CanEmpty.False, "yyyy-MM-dd", "预约办证日期");

            var items = Twn.Customer.Select(YYBZRQ: YYBZRQ, rows: "1000", order: "LRSJ ASC");
            if (!items.Any())
                throw new Exception("未找到记录");


            int i = 1, j = 1;
            using (var pck = new ExcelPackage())
            using (var ws = pck.Workbook.Worksheets.Add("Sheet1"))
            {
                var namedStyle = pck.Workbook.Styles.CreateNamedStyle("HyperLink");
                namedStyle.Style.Font.UnderLine = true;
                namedStyle.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                foreach (var t in new List<string>() { "序号", "姓名", "联系方式", "业务员" })
                    ws.Cells[i, j++].Value = t;
                foreach (var item in items)
                {
                    i++; j = 1;
                    foreach (var t in new List<string>() { "RowNumber", "XM", "LXFS", "YWY1" })
                        ws.Cells[i, j++].Value = item[t];
                    ws.Cells[i, j].Formula = @"HYPERLINK(""http://www.qq.com"", ""QQ"")";
                    ws.Cells[i, j].StyleName = "HyperLink";
                }
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Clear();
                Response.BufferOutput = true;
                Response.AddHeader("content-disposition", string.Format("filename={0}.xlsx", Server.UrlPathEncode("预约办证名单")));
                Response.BinaryWrite(pck.GetAsByteArray());
            }//ExcelPackage
        }
        catch (Exception ex)
        {
            Response.ContentType = "text/html";
            Response.Clear();
            Response.BufferOutput = true;
            Response.Write("<h1>错误</h1>");
            Response.Write(string.Format("<p>{0}</p>", ex.Message));
        }
        finally
        {
            Response.End();
        }
    }//ProcessRequest

    public bool IsReusable { get { return true; } }

}//class
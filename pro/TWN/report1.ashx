<%@ WebHandler Language="C#" Class="report1" %>

using System;
using System.Web;
using System.Data.OleDb;
using System.IO;
using System.Linq;

public class report1 : IHttpHandler
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

            var filename = Server.MapPath(string.Format("~/pub/attachment/{0}.xls", Guid.NewGuid()));
            using (var cn = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;", filename)))
            {
                cn.Open();
                using (var ta = cn.BeginTransaction())
                {
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = ta;

                        cmd.CommandText = @"CREATE TABLE [Sheet1]([序号] string, [姓名] string, [联系方式] string, [业务员] string);";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"INSERT INTO [Sheet1$] VALUES(@RowNumber, @XM, @LXFS, @YWY1)";
                        foreach (var item in items)
                        {
                            cmd.Parameters.Clear();
                            Common.AddParameter(cmd, "@RowNumber", item["RowNumber"]);
                            Common.AddParameter(cmd, "@XM", item["XM"]);
                            Common.AddParameter(cmd, "@LXFS", item["LXFS"]);
                            Common.AddParameter(cmd, "@YWY1", item["YWY1"]);
                            cmd.ExecuteNonQuery();
                        }
                    }//cmd
                    ta.Commit();
                }//ta
            }//cn
            Response.ContentType = "application/vnd.ms-excel";
            Response.Clear();
            Response.BufferOutput = true;
            Response.AddHeader("content-disposition", string.Format("filename={0}.xls", Server.UrlPathEncode("预约办证名单")));
            Response.BinaryWrite(File.ReadAllBytes(filename));
            File.Delete(filename);
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
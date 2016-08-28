<%@ WebHandler Language="C#" Class="report2" %>

using System;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;

public class report2 : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var Request = context.Request;
        var Response = context.Response;
        var Server = context.Server;

        try
        {
            var TDID = Request["TDID"];
            Common.ValidString(TDID, CanNull.False, CanEmpty.False, "团队编号");

            var items = Twn.Customer.Select(TDID: TDID, rows: "1000", order: "LRSJ ASC");
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

                        cmd.CommandText = @"CREATE TABLE [Sheet1](序号 string,团队编号 string,姓名 string,拼音姓名 string, 性别 string, 出生日期 string, 联系方式 string, 出生地 string, 职业 string, 出境记录 string, 婚姻状况 string, 证件种类 string, 证件号码 string, 签发地 string, 签发日期 string, 有效期至 string, 身份确认 string, 经济能力确认 string, 金额 string, 销售 string, 用房 string, 地区 string, 备注 string);";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"INSERT INTO [Sheet1$] VALUES(@RowNumber, @TDID, @XM, @PYXM, @XB1, @CSRQ, @LXFS, @CSD1, @ZY, @CJJL1, @HYZK1, @ZJZL1, @ZJHM, @QFD1, @QFRQ, @YXQZ, @SFQR1, @JJNLQR, @JE, @XS, @YF, @DQ, @BZ)";
                        foreach (var item in items)
                        {
                            cmd.Parameters.Clear();
                            Common.AddParameter(cmd, "@RowNumber", int.Parse(item["RowNumber"].ToString()) + 1);
                            Common.AddParameter(cmd, "@TDID", item["TDID"]);
                            Common.AddParameter(cmd, "@XM", item["XM"]);
                            Common.AddParameter(cmd, "@PYXM", item["PYXM"]);
                            Common.AddParameter(cmd, "@XB1", item["XB1"]);
                            Common.AddParameter(cmd, "@CSRQ", item["CSRQ"]);
                            Common.AddParameter(cmd, "@LXFS", item["LXFS"]);
                            Common.AddParameter(cmd, "@CSD1", item["CSD1"]);
                            Common.AddParameter(cmd, "@ZY", item["ZY"]);
                            Common.AddParameter(cmd, "@CJJL1", item["CJJL1"]);
                            Common.AddParameter(cmd, "@HYZK1", item["HYZK1"]);
                            Common.AddParameter(cmd, "@ZJZL1", item["ZJZL1"]);
                            Common.AddParameter(cmd, "@ZJHM", item["ZJHM"]);
                            Common.AddParameter(cmd, "@QFD1", item["QFD1"]);
                            Common.AddParameter(cmd, "@QFRQ", item["QFRQ"]);
                            Common.AddParameter(cmd, "@YXQZ", item["YXQZ"]);
                            Common.AddParameter(cmd, "@SFQR1", item["SFQR1"]);
                            Common.AddParameter(cmd, "@JJNLQR", item["JJNLQR"]);
                            Common.AddParameter(cmd, "@JE", item["JE"]);
                            Common.AddParameter(cmd, "@XS", item["XS"]);
                            Common.AddParameter(cmd, "@YF", item["YF"]);
                            Common.AddParameter(cmd, "@DQ", item["DQ"]);
                            Common.AddParameter(cmd, "@BZ", item["BZ"]);
                            cmd.ExecuteNonQuery();
                        }
                    }//cmd
                    ta.Commit();
                }//ta
            }//cn
            Response.ContentType = "application/vnd.ms-excel";
            Response.Clear();
            Response.BufferOutput = true;
            Response.AddHeader("content-disposition", string.Format("filename={0}.xls", Server.UrlPathEncode("团队名单")));
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
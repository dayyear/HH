using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;

public partial class public_task_PLBZJDCX : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Server.ScriptTimeout = 5*60;

        if (IsPostBack)
            return;

        var log = "";
        try
        {
            //var items = Twn.Customer.Select(LRSJ1: DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss"), rows: "1000");



            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString))
            {
                cn.Open();

                var root = new XElement("root");

                // 1. 读数据
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT * FROM [TWN].[Customer] WHERE LRSJ>@LRSJ";
                    Common.AddParameter(cmd, "@LRSJ", DateTime.Now.AddMonths(-1));
                    using (DbDataReader dr = cmd.ExecuteReader())
                        while (dr.Read())
                        {
                            if (dr["BZJD"].ToString() == "已收证")
                                continue;
                            if (dr["BZJD"].ToString() == "已发证")
                                continue;
                            if (dr["BZJD"].ToString() == "代办单位已集中领取")
                                continue;
                            if (dr["BZJD"].ToString() == "证件已领取")
                                continue;
                            if (dr["BZJD"].ToString() == "您只能查询三个月之内的受理业务")
                                continue;
                            if (dr["HZD"].ToString().Length != 15)
                                continue;
                            if (!dr["HZD"].ToString().StartsWith("32"))
                                continue;
                            root.Add(new XElement("item",
                                new XAttribute("ID", dr["ID"].ToString()),
                                new XElement("HZD", dr["HZD"].ToString())
                            ));
                        }//read
                }//cmd

                // 2. 查进度
                var web = new CertificateScheduleWeb("jsbee.com.cn", "80");
                foreach (var item in root.Elements("item"))
                    web.Query(item.Element("HZD").Value);
                System.Threading.Thread.Sleep(3000);
                foreach (var item in root.Elements("item"))
                    web.Query(item.Element("HZD").Value);

                foreach (var item in root.Elements("item"))
                {
                    try
                    {
                        Match match = null;
                        var responseString = "";
                        var hzd = item.Element("HZD").Value;

                        Log(hzd);
                        responseString = web.Query(hzd);
                        Log(responseString);

                        match = Regex.Match(responseString, CertificateScheduleWeb.ALERT_Pattern);
                        if (match.Success) throw new Exception(match.Groups[1].Value);

                        var c = 1;
                        while (responseString.Contains("20秒"))
                        {
                            System.Threading.Thread.Sleep(3000);
                            Log(hzd);
                            responseString = web.Query(hzd);
                            Log(responseString);
                            if (++c > 10) throw new Exception("查询超时");
                        }

                        match = Regex.Match(responseString, CertificateScheduleWeb.BZJD_Pattern);
                        if (!match.Success) throw new Exception("未知错误：" + responseString);

                        item.Add(new XElement("success", true), new XElement("BZJD", match.Groups[1].Value));
                    }
                    catch (Exception ex)
                    {
                        item.Add(new XElement("success", false), new XElement("message", ex.Message));
                    }
                }

                // 3. 结果保存
                //root.Save(Server.MapPath(string.Format("~/pub/log/root{0}.xml", DateTime.Now.ToString("yyyyMMddHHmmss"))));
                using (DbTransaction ta = cn.BeginTransaction())
                {
                    using (DbCommand cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = ta;
                        foreach (XElement item in root.Elements("item"))
                        {
                            var success = bool.Parse(item.Element("success").Value);
                            if (!success)
                                continue;
                            var id = int.Parse(item.Attribute("ID").Value);
                            var BZJD = item.Element("BZJD").Value;

                            cmd.CommandText = "UPDATE [TWN].[Customer] SET BZJD=@BZJD,CXSJ=@CXSJ WHERE ID=@ID";
                            cmd.Parameters.Clear();
                            Common.AddParameter(cmd, "@BZJD", BZJD);
                            Common.AddParameter(cmd, "@CXSJ", DateTime.Now);
                            Common.AddParameter(cmd, "@ID", id);
                            cmd.ExecuteNonQuery();
                        }
                    }//cmd
                    ta.Commit();
                }//ta
            }//cn
            log = "办证进度查询成功";

        }
        catch (Exception ex)
        {
            log = ex.ToString();
        }
        finally
        {
            Log(log);
        }
    }//Page_Load

    private void Log(string message)
    {
        var now = DateTime.Now;
        /*using (var sw = new StreamWriter(Server.MapPath(string.Format("~/pub/log/{0}.log", now.ToString("yyyy-MM-dd"))), true, Encoding.UTF8))
            sw.WriteLine("{0} {1}", now.ToString("HH:mm:ss"), message);*/
    }
}//class
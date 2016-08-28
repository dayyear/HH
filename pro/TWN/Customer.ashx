<%@ WebHandler Language="C#" Class="Customer" %>

using System;
using System.Web;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using System.IO;

public class Customer : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //System.Threading.Thread.Sleep(2000);
        HttpRequest Request = context.Request;
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

        // 动词
        string VERB = Request["VERB"];

        try
        {
            if (string.IsNullOrWhiteSpace(VERB))
                throw new Exception("[动词]不能为空");

            if (!(new List<string>() { "SELECT", "INSERT", "UPDATE", "DELETE" }).Contains(VERB.ToUpper()))
                throw new Exception("无效的[动词]：" + VERB);

            string providerName = ConfigurationManager.ConnectionStrings["HuiHuang"].ProviderName;
            string connectionString = ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString;

            DbProviderFactory df = DbProviderFactories.GetFactory(providerName);
            using (DbConnection cn = df.CreateConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                //using (DbCommand cmd = cn.CreateCommand())
                //{
                if (VERB.ToUpper().Equals("UPDATE"))
                    Upd(context, cn);
                else if (VERB.ToUpper().Equals("DELETE"))
                    Del(context, cn);
                //}//DbConnection
            }//DbConnection
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

    private static void Upd(HttpContext context, DbConnection cn)
    {
        using (DbTransaction ta = cn.BeginTransaction())
        {
            using (DbCommand cmd = cn.CreateCommand(), cmdUpdate = cn.CreateCommand(), cmdInsert = cn.CreateCommand())
            {
                cmd.Transaction = ta;
                cmdUpdate.Transaction = ta;
                cmdInsert.Transaction = ta;
                //////////////////////////////////////////////////////////////////////////
                // 表单参数
                //////////////////////////////////////////////////////////////////////////
                string ID = context.Request["ID"];
                string XM = context.Request["XM"];
                string PYXM = context.Request["PYXM"];
                string XB = context.Request["XB"];
                string CSRQ = context.Request["CSRQ"];
                string LXFS = context.Request["LXFS"];
                string YYBZRQ = context.Request["YYBZRQ"];
                string HZD = context.Request["HZD"];
                string BZJD = context.Request["BZJD"];
                string ZJHM = context.Request["ZJHM"];
                string QFRQ = context.Request["QFRQ"];
                string YXQZ = context.Request["YXQZ"];
                string DQ = context.Request["DQ"];
                string BZ = context.Request["BZ"];
                string YF = context.Request["YF"];
                string TDID = context.Request["TDID"];
                string ZJZL = context.Request["ZJZL"];
                string CXSJ = context.Request["CXSJ"];
                string QFD = context.Request["QFD"];
                string CSD = context.Request["CSD"];

                var ZY = context.Request["ZY"];
                var CJJL = context.Request["CJJL"];
                var HYZK = context.Request["HYZK"];
                var SFQR = context.Request["SFQR"];
                var JJNLQR = context.Request["JJNLQR"];
                var JE = context.Request["JE"];
                var XS = context.Request["XS"];

                string PASS1 = context.Request["PASS1"];
                string PHOTO = context.Request["PHOTO"];
                string IDCARD1 = context.Request["IDCARD1"];
                string IDCARD2 = context.Request["IDCARD2"];
                //string PASS2 = context.Request["PASS2"];
                string HOUSEHOLD1 = context.Request["HOUSEHOLD1"];
                string HOUSEHOLD2 = context.Request["HOUSEHOLD2"];
                string HOUSEHOLD3 = context.Request["HOUSEHOLD3"];
                string HOUSEHOLD4 = context.Request["HOUSEHOLD4"];
                string BIRTH = context.Request["BIRTH"];
                string CONSENT = context.Request["CONSENT"];
                string OTHER1 = context.Request["OTHER1"];
                string OTHER2 = context.Request["OTHER2"];

                //////////////////////////////////////////////////////////////////////////
                // 1. 表单验证
                //////////////////////////////////////////////////////////////////////////
                // 主键
                if (string.IsNullOrWhiteSpace(ID))
                    throw new Exception(string.Format("[主键]不能为空"));
                ID = ID.Trim();
                try { int.Parse(ID); }
                catch (Exception) { throw new Exception(string.Format("[主键]格式不正确: {0}", ID)); }
                // 性别
                if (!string.IsNullOrWhiteSpace(XB))
                {
                    XB = XB.Trim();
                    try { int.Parse(XB); }
                    catch (Exception) { throw new Exception(string.Format("[性别]格式不正确: {0}", XB)); }
                }
                // 出生日期
                if (!string.IsNullOrWhiteSpace(CSRQ))
                {
                    CSRQ = CSRQ.Trim();
                    try { DateTime.ParseExact(CSRQ, "yyyy-MM-dd", null); }
                    catch (Exception) { throw new Exception(string.Format("[出生日期]格式不正确: {0}", CSRQ)); }
                }
                // 预约办证日期
                if (!string.IsNullOrWhiteSpace(YYBZRQ))
                {
                    YYBZRQ = YYBZRQ.Trim();
                    try { DateTime.ParseExact(YYBZRQ, "yyyy-MM-dd", null); }
                    catch (Exception) { throw new Exception(string.Format("[预约办证日期]格式不正确: {0}", YYBZRQ)); }
                }
                // 签发日期
                if (!string.IsNullOrWhiteSpace(QFRQ))
                {
                    QFRQ = QFRQ.Trim();
                    try { DateTime.ParseExact(QFRQ, "yyyy-MM-dd", null); }
                    catch (Exception) { throw new Exception(string.Format("[签发日期]格式不正确: {0}", QFRQ)); }
                }
                // 有效期至
                if (!string.IsNullOrWhiteSpace(YXQZ))
                {
                    YXQZ = YXQZ.Trim();
                    try { DateTime.ParseExact(YXQZ, "yyyy-MM-dd", null); }
                    catch (Exception) { throw new Exception(string.Format("[有效期至]格式不正确: {0}", YXQZ)); }
                }
                // 查询时间
                if (!string.IsNullOrWhiteSpace(CXSJ))
                {
                    CXSJ = CXSJ.Trim();
                    try { DateTime.ParseExact(CXSJ, "yyyy-MM-dd HH:mm", null); }
                    catch (Exception) { throw new Exception(string.Format("[查询时间]格式不正确: {0}", CXSJ)); }
                }
                if (string.IsNullOrWhiteSpace(TDID))
                    throw new Exception(string.Format("[团队编号]不能为空"));
                TDID = TDID.Trim();

                //////////////////////////////////////////////////////////////////////////
                // 2. 权限验证
                //////////////////////////////////////////////////////////////////////////
                cmd.CommandText = "SELECT YWY FROM [HuiHuang].[TWN].[Customer] WHERE ID=@ID";
                cmd.Parameters.Clear();
                Common.AddParameter(cmd, "@ID", DbType.Int32).Value = int.Parse(ID);
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        throw new Exception(string.Format("不存在该记录"));
                    if (!(new List<string>() { "1", "2", dr[0].ToString() }).Contains(context.User.Identity.Name))
                        throw new Exception(string.Format("您无权修改该记录"));
                }

                //////////////////////////////////////////////////////////////////////////
                // 3. 数据更新(文字)
                //////////////////////////////////////////////////////////////////////////
                cmdUpdate.CommandText = @"UPDATE [HuiHuang].[TWN].[Customer] SET [XM]=@XM, [PYXM]=@PYXM, [XB]=@XB, [CSRQ]=@CSRQ, [LXFS]=@LXFS, [YYBZRQ]=@YYBZRQ, [HZD]=@HZD, [BZJD]=@BZJD, [ZJHM]=@ZJHM, [QFRQ]=@QFRQ, [YXQZ]=@YXQZ, [DQ]=@DQ, [BZ]=@BZ, [YF]=@YF, [TDID]=@TDID, [ZJZL]=@ZJZL, [CXSJ]=@CXSJ, [QFD]=@QFD, [CSD]=@CSD, [ZY]=@ZY, [CJJL]=@CJJL, [HYZK]=@HYZK, [SFQR]=@SFQR, [JJNLQR]=@JJNLQR, [JE]=@JE, [XS]=@XS WHERE ([ID]=@ID)";

                Common.AddParameter(cmdUpdate, "@ID", DbType.Int32).Value = string.IsNullOrWhiteSpace(ID) ? Convert.DBNull : int.Parse(ID);
                Common.AddParameter(cmdUpdate, "@XM", DbType.String).Value = string.IsNullOrWhiteSpace(XM) ? Convert.DBNull : XM.Trim();
                Common.AddParameter(cmdUpdate, "@PYXM", DbType.String).Value = string.IsNullOrWhiteSpace(PYXM) ? Convert.DBNull : PYXM.Trim().ToUpper();
                Common.AddParameter(cmdUpdate, "@XB", DbType.Int32).Value = string.IsNullOrWhiteSpace(XB) ? Convert.DBNull : int.Parse(XB);
                Common.AddParameter(cmdUpdate, "@CSRQ", DbType.Date).Value = string.IsNullOrWhiteSpace(CSRQ) ? Convert.DBNull : DateTime.Parse(CSRQ);
                Common.AddParameter(cmdUpdate, "@LXFS", DbType.String).Value = string.IsNullOrWhiteSpace(LXFS) ? Convert.DBNull : LXFS.Trim();
                Common.AddParameter(cmdUpdate, "@YYBZRQ", DbType.Date).Value = string.IsNullOrWhiteSpace(YYBZRQ) ? Convert.DBNull : DateTime.Parse(YYBZRQ);
                Common.AddParameter(cmdUpdate, "@HZD", DbType.String).Value = string.IsNullOrWhiteSpace(HZD) ? Convert.DBNull : HZD.Trim();
                Common.AddParameter(cmdUpdate, "@BZJD", DbType.String).Value = string.IsNullOrWhiteSpace(BZJD) ? Convert.DBNull : BZJD.Trim();
                Common.AddParameter(cmdUpdate, "@ZJHM", DbType.String).Value = string.IsNullOrWhiteSpace(ZJHM) ? Convert.DBNull : ZJHM.Trim().ToUpper();
                Common.AddParameter(cmdUpdate, "@QFRQ", DbType.Date).Value = string.IsNullOrWhiteSpace(QFRQ) ? Convert.DBNull : DateTime.Parse(QFRQ);
                Common.AddParameter(cmdUpdate, "@YXQZ", DbType.Date).Value = string.IsNullOrWhiteSpace(YXQZ) ? Convert.DBNull : DateTime.Parse(YXQZ);
                Common.AddParameter(cmdUpdate, "@DQ", DbType.String).Value = string.IsNullOrWhiteSpace(DQ) ? Convert.DBNull : DQ.Trim();
                Common.AddParameter(cmdUpdate, "@BZ", DbType.String).Value = string.IsNullOrWhiteSpace(BZ) ? Convert.DBNull : BZ.Trim();
                Common.AddParameter(cmdUpdate, "@YF", DbType.String).Value = string.IsNullOrWhiteSpace(YF) ? Convert.DBNull : YF.Trim();
                Common.AddParameter(cmdUpdate, "@TDID", DbType.String).Value = string.IsNullOrWhiteSpace(TDID) ? Convert.DBNull : TDID.Trim();
                Common.AddParameter(cmdUpdate, "@CXSJ", DbType.Date).Value = string.IsNullOrWhiteSpace(CXSJ) ? Convert.DBNull : CXSJ.Trim();
                Common.AddParameter(cmdUpdate, "@ZJZL", DbType.String).Value = string.IsNullOrWhiteSpace(ZJZL) ? Convert.DBNull : ZJZL.Trim();
                Common.AddParameter(cmdUpdate, "@QFD", DbType.String).Value = string.IsNullOrWhiteSpace(QFD) ? Convert.DBNull : QFD.Trim();
                Common.AddParameter(cmdUpdate, "@CSD", DbType.String).Value = string.IsNullOrWhiteSpace(CSD) ? Convert.DBNull : CSD.Trim();

                Common.AddParameter(cmdUpdate, "@ZY", DbType.String).Value = string.IsNullOrWhiteSpace(ZY) ? Convert.DBNull : ZY.Trim();
                Common.AddParameter(cmdUpdate, "@CJJL", DbType.String).Value = string.IsNullOrWhiteSpace(CJJL) ? Convert.DBNull : CJJL.Trim();
                Common.AddParameter(cmdUpdate, "@HYZK", DbType.String).Value = string.IsNullOrWhiteSpace(HYZK) ? Convert.DBNull : HYZK.Trim();
                Common.AddParameter(cmdUpdate, "@SFQR", DbType.String).Value = string.IsNullOrWhiteSpace(SFQR) ? Convert.DBNull : SFQR.Trim();
                Common.AddParameter(cmdUpdate, "@JJNLQR", DbType.String).Value = string.IsNullOrWhiteSpace(JJNLQR) ? Convert.DBNull : JJNLQR.Trim();
                Common.AddParameter(cmdUpdate, "@JE", DbType.String).Value = string.IsNullOrWhiteSpace(JE) ? Convert.DBNull : JE.Trim();
                Common.AddParameter(cmdUpdate, "@XS", DbType.String).Value = string.IsNullOrWhiteSpace(XS) ? Convert.DBNull : XS.Trim();

                cmdUpdate.ExecuteNonQuery();

                //////////////////////////////////////////////////////////////////////////
                // 4. 数据更新(图像)
                //////////////////////////////////////////////////////////////////////////
                cmdUpdate.CommandText = @"UPDATE [HuiHuang].[TWN].[CustomerImage] SET [PASS1]=@PASS1, [PHOTO]=@PHOTO, [IDCARD1]=@IDCARD1, [IDCARD2]=@IDCARD2, [HOUSEHOLD1]=@HOUSEHOLD1, [HOUSEHOLD2]=@HOUSEHOLD2, [HOUSEHOLD3]=@HOUSEHOLD3, [HOUSEHOLD4]=@HOUSEHOLD4, [BIRTH]=@BIRTH, [CONSENT]=@CONSENT, [OTHER1]=@OTHER1, [OTHER2]=@OTHER2 WHERE ([ID]=@ID)";
                cmdInsert.CommandText = @"INSERT INTO [HuiHuang].[TWN].[CustomerImage](ID, PASS1, PHOTO, IDCARD1, IDCARD2, HOUSEHOLD1, HOUSEHOLD2, HOUSEHOLD3, HOUSEHOLD4, BIRTH, CONSENT, OTHER1, OTHER2) VALUES(@ID, @PASS1, @PHOTO, @IDCARD1, @IDCARD2, @HOUSEHOLD1, @HOUSEHOLD2, @HOUSEHOLD3, @HOUSEHOLD4, @BIRTH, @CONSENT, @OTHER1, @OTHER2)";
                cmdUpdate.Parameters.Clear();
                cmdInsert.Parameters.Clear();
                Common.AddParameter(cmdUpdate, "@ID", DbType.Int32).Value = string.IsNullOrWhiteSpace(ID) ? Convert.DBNull : int.Parse(ID);
                Common.AddParameter(cmdInsert, "@ID", DbType.Int32).Value = string.IsNullOrWhiteSpace(ID) ? Convert.DBNull : int.Parse(ID);

                byte[] bytes;
                if (string.IsNullOrWhiteSpace(PASS1))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@PASS1", "[PASS1]");
                    Common.AddParameter(cmdInsert, "@PASS1", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + PASS1));
                    Common.AddParameter(cmdUpdate, "@PASS1", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@PASS1", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(PHOTO))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@PHOTO", "[PHOTO]");
                    Common.AddParameter(cmdInsert, "@PHOTO", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + PHOTO));
                    Common.AddParameter(cmdUpdate, "@PHOTO", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@PHOTO", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(IDCARD1))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@IDCARD1", "[IDCARD1]");
                    Common.AddParameter(cmdInsert, "@IDCARD1", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + IDCARD1));
                    Common.AddParameter(cmdUpdate, "@IDCARD1", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@IDCARD1", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(IDCARD2))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@IDCARD2", "[IDCARD2]");
                    Common.AddParameter(cmdInsert, "@IDCARD2", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + IDCARD2));
                    Common.AddParameter(cmdUpdate, "@IDCARD2", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@IDCARD2", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(HOUSEHOLD1))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@HOUSEHOLD1", "[HOUSEHOLD1]");
                    Common.AddParameter(cmdInsert, "@HOUSEHOLD1", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + HOUSEHOLD1));
                    Common.AddParameter(cmdUpdate, "@HOUSEHOLD1", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@HOUSEHOLD1", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(HOUSEHOLD2))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@HOUSEHOLD2", "[HOUSEHOLD2]");
                    Common.AddParameter(cmdInsert, "@HOUSEHOLD2", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + HOUSEHOLD2));
                    Common.AddParameter(cmdUpdate, "@HOUSEHOLD2", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@HOUSEHOLD2", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(HOUSEHOLD3))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@HOUSEHOLD3", "[HOUSEHOLD3]");
                    Common.AddParameter(cmdInsert, "@HOUSEHOLD3", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + HOUSEHOLD3));
                    Common.AddParameter(cmdUpdate, "@HOUSEHOLD3", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@HOUSEHOLD3", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(HOUSEHOLD4))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@HOUSEHOLD4", "[HOUSEHOLD4]");
                    Common.AddParameter(cmdInsert, "@HOUSEHOLD4", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + HOUSEHOLD4));
                    Common.AddParameter(cmdUpdate, "@HOUSEHOLD4", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@HOUSEHOLD4", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(BIRTH))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@BIRTH", "[BIRTH]");
                    Common.AddParameter(cmdInsert, "@BIRTH", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + BIRTH));
                    Common.AddParameter(cmdUpdate, "@BIRTH", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@BIRTH", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(CONSENT))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@CONSENT", "[CONSENT]");
                    Common.AddParameter(cmdInsert, "@CONSENT", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + CONSENT));
                    Common.AddParameter(cmdUpdate, "@CONSENT", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@CONSENT", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(OTHER1))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@OTHER1", "[OTHER1]");
                    Common.AddParameter(cmdInsert, "@OTHER1", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + OTHER1));
                    Common.AddParameter(cmdUpdate, "@OTHER1", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@OTHER1", DbType.Binary).Value = bytes;
                }
                if (string.IsNullOrWhiteSpace(OTHER2))
                {
                    cmdUpdate.CommandText = cmdUpdate.CommandText.Replace("@OTHER2", "[OTHER2]");
                    Common.AddParameter(cmdInsert, "@OTHER2", DbType.Binary).Value = Convert.DBNull;
                }
                else
                {
                    bytes = File.ReadAllBytes(context.Server.MapPath("~" + OTHER2));
                    Common.AddParameter(cmdUpdate, "@OTHER2", DbType.Binary).Value = bytes;
                    Common.AddParameter(cmdInsert, "@OTHER2", DbType.Binary).Value = bytes;
                }

                if (cmdUpdate.ExecuteNonQuery() == 0)
                    cmdInsert.ExecuteNonQuery();

            }//DbCommand
            ta.Commit();
        }//DbTransaction
    }//Update

    private static void Del(HttpContext context, DbConnection cn)
    {
        using (DbTransaction ta = cn.BeginTransaction())
        {
            using (DbCommand cmd = cn.CreateCommand())
            {
                cmd.Transaction = ta;
                //////////////////////////////////////////////////////////////////////////
                // 表单参数
                //////////////////////////////////////////////////////////////////////////
                string ID = context.Request["ID"];

                //////////////////////////////////////////////////////////////////////////
                // 1. 表单验证
                //////////////////////////////////////////////////////////////////////////
                // 主键
                if (string.IsNullOrWhiteSpace(ID))
                    throw new Exception(string.Format("[主键]不能为空"));
                ID = ID.Trim();
                try { int.Parse(ID); }
                catch (Exception) { throw new Exception(string.Format("[主键]格式不正确: {0}", ID)); }

                //////////////////////////////////////////////////////////////////////////
                // 2. 权限验证
                //////////////////////////////////////////////////////////////////////////
                cmd.CommandText = "SELECT YWY FROM [HuiHuang].[TWN].[Customer] WHERE ID=@ID";
                cmd.Parameters.Clear();
                Common.AddParameter(cmd, "@ID", DbType.Int32).Value = int.Parse(ID);
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        throw new Exception(string.Format("不存在该记录"));
                    if (!(new List<string>() { "1", dr[0].ToString() }).Contains(context.User.Identity.Name))
                        throw new Exception(string.Format("您无权删除该记录"));
                }

                //////////////////////////////////////////////////////////////////////////
                // 3. 数据删除
                //////////////////////////////////////////////////////////////////////////
                cmd.CommandText = "DELETE FROM [HuiHuang].[TWN].[Customer] WHERE ID=@ID";
                cmd.Parameters.Clear();
                Common.AddParameter(cmd, "@ID", DbType.Int32).Value = int.Parse(ID);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM [HuiHuang].[TWN].[CustomerImage] WHERE ID=@ID";
                cmd.Parameters.Clear();
                Common.AddParameter(cmd, "@ID", DbType.Int32).Value = int.Parse(ID);
                cmd.ExecuteNonQuery();
            }//DbCommand
            ta.Commit();
        }//DbTransaction
    }//Delete


    public bool IsReusable { get { return true; } }

}
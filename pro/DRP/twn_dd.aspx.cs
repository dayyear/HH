using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

public partial class pro_DRP_twn_dd : System.Web.UI.Page
{
    protected string rows;
    protected string page;

    private bool success = true;
    private string message = null;
    private int total = 0;
    private List<Dictionary<string, Object>> items = new List<Dictionary<string, object>>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;

        if (Request.HttpMethod == "GET")
        {
            try
            {
                rows = Request["rows"];
                page = Request["page"];

                int result;
                if (!int.TryParse(rows, out result))
                    rows = "10";
                if (!int.TryParse(page, out result))
                    page = "1";
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
                Response.End();
            }
        }//GET

        else if (Request.HttpMethod == "POST")
        {
            POST();
        }//POST
    }//Page_Load

    private void POST()
    {
        Response.ContentType = "application/json";
        Response.Clear();
        Response.BufferOutput = true;

        try
        {
            string VERB = Request["VERB"];
            if (string.IsNullOrWhiteSpace(VERB)) throw new Exception("[动词]不能为空");
            if (VERB.ToUpper() == "SELECT")
            {
                Response.Write(JsonConvert.SerializeObject(DRP.TWN_DD.SELECT(
                    User: User,
                    ID: Request["ID"],
                    rows: Request["rows"],
                    page: Request["page"]),
                Formatting.None));
                Response.End();
            }
            else if (VERB.ToUpper() == "INSERT")
            {
                Response.Write(JsonConvert.SerializeObject(DRP.TWN_DD.INSERT(
                    Request["TD_ID"], Request["SCD"], Request["BZ"], Request["JBR"], Request["JBRSJ"],
                    Request.Params.GetValues("RS"), Request.Params.GetValues("RS_DFC"),
                    Request.Params.GetValues("XM"), Request.Params.GetValues("SFZH"), Request.Params.GetValues("XB"), Request.Params.GetValues("CSRQ"), Request.Params.GetValues("LXDH"), Request.Params.GetValues("SFJS"),
                    User),
                Formatting.None));
                Response.End();
            }
            else if (VERB.ToUpper() == "UPDATE")
            {
                Response.Write(JsonConvert.SerializeObject(DRP.TWN_DD.UPDATE(
                  Request["ID"], Request["ZT"],
                  Request.Params.GetValues("XM"), Request.Params.GetValues("SFZH"), Request.Params.GetValues("XB"), Request.Params.GetValues("CSRQ"), Request.Params.GetValues("LXDH"), Request.Params.GetValues("SFJS"),
                  User),
              Formatting.None));
                Response.End();
            }
            else throw new Exception("无效的[动词]：" + VERB);
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
    }//POST

    private void SELECT()
    {
        //////////////////////////////////////////////////////////////////////////
        // 表单验证
        // ID,row,page
        //////////////////////////////////////////////////////////////////////////
        StringBuilder where = new StringBuilder();

        string ID = Request["ID"];
        if (!string.IsNullOrWhiteSpace(ID))
        {
            if (where.Length > 0) where.Append(" and ");
            where.AppendFormat("ID={0}", ID);
        }

        if (where.Length > 0) where.Insert(0, " where ");

        int intTemp, page = 1, rows = 10;
        if (int.TryParse(Request["page"], out intTemp)) page = intTemp;
        if (int.TryParse(Request["rows"], out intTemp)) rows = intTemp;

        //////////////////////////////////////////////////////////////////////////
        // 数据库操作
        //////////////////////////////////////////////////////////////////////////
        string providerName = ConfigurationManager.ConnectionStrings["HuiHuang"].ProviderName;
        string connectionString = ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString;
        DbProviderFactory df = DbProviderFactories.GetFactory(providerName);
        using (DbConnection cn = df.CreateConnection())
        {
            cn.ConnectionString = connectionString;
            cn.Open();
            using (DbCommand cmd = cn.CreateCommand())
            {
                // 1. 订单基本信息
                cmd.CommandText = string.Format(@"SELECT COUNT(*) FROM [DRP].[TWN_DD] {0}", where);
                total = int.Parse(cmd.ExecuteScalar().ToString());
                cmd.CommandText = string.Format(@"SELECT *, ( SELECT username FROM [ADMIN].[User] WHERE ID = X.YDR ) AS YDR1, ( SELECT MC FROM [ADMIN].[DM_SCD] WHERE DM = X.SCD ) AS SCD1, ( CASE ZT WHEN '1' THEN '待审核' WHEN '2' THEN '已审核' WHEN '0' THEN '已取消' END ) AS ZT1 FROM ( SELECT *, ROW_NUMBER () OVER (ORDER BY YDSJ DESC) AS RowNumber FROM [DRP].[TWN_DD] {0}) AS X WHERE RowNumber > {1} AND RowNumber <= {2} ORDER BY YDSJ DESC", where, rows * (page - 1), rows * page);
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Dictionary<string, Object> item = new Dictionary<string, object>();
                        items.Add(item);
                        for (int ordinal = 0; ordinal < dr.FieldCount; ordinal++)
                        {
                            if (Convert.IsDBNull(dr[ordinal]))
                                item.Add(dr.GetName(ordinal), null);
                            else if (dr.GetDataTypeName(ordinal) == "date")
                                item.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd"));
                            else if (dr.GetDataTypeName(ordinal) == "money")
                                item.Add(dr.GetName(ordinal), float.Parse(dr[ordinal].ToString()).ToString("0.00"));
                            else if (dr.GetDataTypeName(ordinal) == "datetime2")
                                item.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                            else
                                item.Add(dr.GetName(ordinal), dr[ordinal]);
                        }//for (int ordinal = 0; ordinal < dr.FieldCount; ordinal++)
                        item.Add("RS", new List<Dictionary<string, Object>>());
                        item.Add("MD", new List<Dictionary<string, Object>>());
                    }//while (dr.Read())
                }//DbDataReader

                List<string> IDs = new List<string>();
                foreach (var item in items) IDs.Add(item["ID"].ToString());
                if (IDs.Count > 0)
                {
                    // 2. 订单的人数及费用信息
                    cmd.CommandText = string.Format("SELECT * FROM [DRP].[TWN_DD_RS] WHERE ID IN({0}) ORDER BY ID,SN", string.Join(",", IDs)); ;
                    using (DbDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            List<Dictionary<string, Object>> RS = items.Where(x => x["ID"].ToString() == dr["ID"].ToString()).FirstOrDefault()["RS"] as List<Dictionary<string, Object>>;
                            float DFC = float.Parse(items.Where(x => x["ID"].ToString() == dr["ID"].ToString()).FirstOrDefault()["TD_DFC"].ToString());
                            Dictionary<string, Object> rs = new Dictionary<string, object>();
                            RS.Add(rs);
                            for (int ordinal = 0; ordinal < dr.FieldCount; ordinal++)
                            {
                                if (Convert.IsDBNull(dr[ordinal]))
                                    rs.Add(dr.GetName(ordinal), null);
                                else if (dr.GetDataTypeName(ordinal) == "date")
                                    rs.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd"));
                                else if (dr.GetDataTypeName(ordinal) == "money")
                                    rs.Add(dr.GetName(ordinal), float.Parse(dr[ordinal].ToString()).ToString("0.00"));
                                else if (dr.GetDataTypeName(ordinal) == "datetime2")
                                    rs.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                else
                                    rs.Add(dr.GetName(ordinal), dr[ordinal]);
                            }//for (int ordinal = 0; ordinal < dr.FieldCount; ordinal++)
                        }//while (dr.Read())
                    }//DbDataReader

                    // 3. 订单的游客名单
                    cmd.CommandText = string.Format("SELECT * FROM [DRP].[TWN_DD_MD] WHERE ID IN({0}) ORDER BY ID,SN", string.Join(",", IDs)); ;
                    using (DbDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            List<Dictionary<string, Object>> MD = items.Where(x => x["ID"].ToString() == dr["ID"].ToString()).FirstOrDefault()["MD"] as List<Dictionary<string, Object>>;
                            Dictionary<string, Object> md = new Dictionary<string, object>();
                            MD.Add(md);
                            for (int ordinal = 0; ordinal < dr.FieldCount; ordinal++)
                            {
                                if (Convert.IsDBNull(dr[ordinal]))
                                    md.Add(dr.GetName(ordinal), null);
                                else if (dr.GetDataTypeName(ordinal) == "date")
                                    md.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd"));
                                else if (dr.GetDataTypeName(ordinal) == "money")
                                    md.Add(dr.GetName(ordinal), float.Parse(dr[ordinal].ToString()).ToString("0.00"));
                                else if (dr.GetDataTypeName(ordinal) == "datetime2")
                                    md.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                else
                                    md.Add(dr.GetName(ordinal), dr[ordinal]);
                            }//for (int ordinal = 0; ordinal < dr.FieldCount; ordinal++)
                        }//while (dr.Read())
                    }//DbDataReader
                }//if (total > 0)
            }//DbCommand
        }//DbConnection
    }

    private void INSERT()
    {
        //////////////////////////////////////////////////////////////////////////
        // 表单验证
        //////////////////////////////////////////////////////////////////////////
        string TD_ID = Request["TD_ID"];
        if (string.IsNullOrWhiteSpace(TD_ID)) throw new Exception("[团队ID]不能为空");

        string SCD = Request["SCD"];
        if (string.IsNullOrWhiteSpace(SCD)) throw new Exception("[上车点]不能为空");

        string BZ = Request["BZ"] ?? "";

        string JBR = Request["JBR"];
        if (string.IsNullOrWhiteSpace(JBR)) throw new Exception("[经办人]不能为空");

        string JBRSJ = Request["JBRSJ"];
        if (string.IsNullOrWhiteSpace(JBRSJ)) throw new Exception("[经办人手机]不能为空");

        string[] RS = Request.Params.GetValues("RS");
        string[] RS_DFC = Request.Params.GetValues("RS_DFC");
        if (RS.Length != RS_DFC.Length) throw new Exception("[人数]与[人数(单房差)]数量不一致");
        if (RS.Length == 0) throw new Exception("[人数]与[人数(单房差)]不能为空");

        string[] XM = Request.Params.GetValues("XM");
        string[] SFZH = Request.Params.GetValues("SFZH");
        string[] XB = Request.Params.GetValues("XB");
        string[] CSRQ = Request.Params.GetValues("CSRQ");
        string[] LXDH = Request.Params.GetValues("LXDH");
        if (XM.Length != SFZH.Length || XM.Length != XB.Length || XM.Length != CSRQ.Length || XM.Length != LXDH.Length) throw new Exception("[名单]各项数量不一致");
        if (XM.Length == 0) throw new Exception("[名单]不能为空");

        //////////////////////////////////////////////////////////////////////////
        // 团队信息留痕
        //////////////////////////////////////////////////////////////////////////
        string TD_BH, TD_XLMC, TD_CFRQ, TD_ZS, TD_TS, TD_DFC, TD_RS, TD_FBR, TD_FBSJ;
        List<Dictionary<string, string>> TD_BJ = new List<Dictionary<string, string>>();

        string providerName = ConfigurationManager.ConnectionStrings["HuiHuang"].ProviderName;
        string connectionString = ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString;
        DbProviderFactory df = DbProviderFactories.GetFactory(providerName);
        using (DbConnection cn = df.CreateConnection())
        {
            cn.ConnectionString = connectionString;
            cn.Open();
            using (DbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT *, (SELECT username FROM [ADMIN].[User] WHERE ID=X.FBR) AS FBR1 FROM [DRP].[TWN_TD] AS X WHERE ID=@ID";
                cmd.Parameters.Clear();
                Common.AddParameter(cmd, "@ID", DbType.String).Value = TD_ID;
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        throw new Exception("[团队]不存在：" + TD_ID);
                    TD_BH = dr["BH"].ToString();
                    TD_XLMC = dr["XLMC"].ToString();
                    TD_CFRQ = dr["CFRQ"].ToString();
                    TD_ZS = dr["ZS"].ToString();
                    TD_TS = dr["TS"].ToString();
                    TD_DFC = dr["DFC"].ToString();
                    TD_RS = dr["RS"].ToString();
                    TD_FBR = dr["FBR1"].ToString();
                    TD_FBSJ = dr["FBSJ"].ToString();
                }//DbDataReader

                cmd.CommandText = "SELECT * FROM [DRP].[TWN_TD_BJ] WHERE ID=@ID ORDER BY SN";
                cmd.Parameters.Clear();
                Common.AddParameter(cmd, "@ID", DbType.String).Value = TD_ID;
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Dictionary<string, string> bj = new Dictionary<string, string>();
                        TD_BJ.Add(bj);
                        bj.Add("SN", dr["SN"].ToString());
                        bj.Add("BJMC", dr["BJMC"].ToString());
                        bj.Add("MSJ", dr["MSJ"].ToString());
                    }
                }//DbDataReader
            }//DbCommand
        }//DbConnection

        //////////////////////////////////////////////////////////////////////////
        // 数据库操作
        //////////////////////////////////////////////////////////////////////////
        using (DbConnection cn = df.CreateConnection())
        {
            cn.ConnectionString = connectionString;
            cn.Open();
            using (DbTransaction ta = cn.BeginTransaction())
            {
                using (DbCommand cmd = cn.CreateCommand())
                {
                    cmd.Transaction = ta;

                    // 1. 订单基本信息
                    cmd.CommandText = "INSERT INTO [DRP].[TWN_DD](TD_ID, TD_BH, TD_XLMC, TD_CFRQ, TD_ZS, TD_TS, TD_DFC, TD_RS, TD_FBR, TD_FBSJ, SCD, BZ, JBR, JBRSJ, ZT, YDR, YDSJ) VALUES(@TD_ID, @TD_BH, @TD_XLMC, @TD_CFRQ, @TD_ZS, @TD_TS, @TD_DFC, @TD_RS, @TD_FBR, @TD_FBSJ, @SCD, @BZ, @JBR, @JBRSJ, @ZT, @YDR, @YDSJ);SELECT  @@IDENTITY";
                    cmd.Parameters.Clear();
                    Common.AddParameter(cmd, "@TD_ID", DbType.String).Value = TD_ID;
                    Common.AddParameter(cmd, "@TD_BH", DbType.String).Value = TD_BH;
                    Common.AddParameter(cmd, "@TD_XLMC", DbType.String).Value = TD_XLMC;
                    Common.AddParameter(cmd, "@TD_CFRQ", DbType.String).Value = TD_CFRQ;
                    Common.AddParameter(cmd, "@TD_ZS", DbType.String).Value = TD_ZS;
                    Common.AddParameter(cmd, "@TD_TS", DbType.String).Value = TD_TS;
                    Common.AddParameter(cmd, "@TD_DFC", DbType.String).Value = TD_DFC;
                    Common.AddParameter(cmd, "@TD_RS", DbType.String).Value = TD_RS;
                    Common.AddParameter(cmd, "@TD_FBR", DbType.String).Value = TD_FBR;
                    Common.AddParameter(cmd, "@TD_FBSJ", DbType.String).Value = TD_FBSJ;

                    Common.AddParameter(cmd, "@SCD", DbType.String).Value = SCD.Trim();
                    Common.AddParameter(cmd, "@BZ", DbType.String).Value = BZ.Trim();
                    Common.AddParameter(cmd, "@JBR", DbType.String).Value = JBR.Trim();
                    Common.AddParameter(cmd, "@JBRSJ", DbType.String).Value = JBRSJ.Trim();

                    Common.AddParameter(cmd, "@FYHJ", DbType.String).Value = "0";
                    Common.AddParameter(cmd, "@ZT", DbType.String).Value = "1";
                    Common.AddParameter(cmd, "@YDR", DbType.String).Value = User.Identity.Name;
                    Common.AddParameter(cmd, "@YDSJ", DbType.String).Value = DateTime.Now;

                    message = cmd.ExecuteScalar().ToString();

                    // 2. 订单费用计算
                    float FYHJ = 0.0F;
                    for (int i = 0; i < RS.Length; i++)
                    {
                        string rs = RS[i];
                        string rs_dfc = RS_DFC[i];
                        if (string.IsNullOrWhiteSpace(rs)) throw new Exception("[人数]不能有空");
                        if (string.IsNullOrWhiteSpace(rs_dfc)) throw new Exception("[人数(单房差)]不能有空");
                        try { int.Parse(rs); }
                        catch (Exception) { throw new Exception(string.Format("[人数]格式不正确: {0}", rs)); }
                        try { int.Parse(rs_dfc); }
                        catch (Exception) { throw new Exception(string.Format("[人数(单房差)]格式不正确: {0}", rs_dfc)); }

                        float FYXJ = int.Parse(rs) * float.Parse(TD_BJ[i]["MSJ"]) + int.Parse(rs_dfc) * float.Parse(TD_DFC);
                        FYHJ += FYXJ;
                        cmd.CommandText = "INSERT INTO [DRP].[TWN_DD_RS](ID,SN,BJMC,MSJ,RS,RS_DFC,FYXJ) VALUES(@ID,@SN,@BJMC,@MSJ,@RS,@RS_DFC,@FYXJ)";
                        cmd.Parameters.Clear();
                        Common.AddParameter(cmd, "@ID", DbType.String).Value = message;
                        Common.AddParameter(cmd, "@SN", DbType.Int32).Value = i + 1;
                        Common.AddParameter(cmd, "@BJMC", DbType.String).Value = TD_BJ[i]["BJMC"];
                        Common.AddParameter(cmd, "@MSJ", DbType.String).Value = TD_BJ[i]["MSJ"];
                        Common.AddParameter(cmd, "@RS", DbType.String).Value = rs;
                        Common.AddParameter(cmd, "@RS_DFC", DbType.String).Value = rs_dfc;
                        Common.AddParameter(cmd, "@FYXJ", DbType.Currency).Value = FYXJ;
                        cmd.ExecuteNonQuery();
                    }//for (int i = 0; i < RS.Length; i++)

                    cmd.CommandText = "UPDATE [DRP].[TWN_DD] SET FYHJ=@FYHJ WHERE ID=@ID";
                    cmd.Parameters.Clear();
                    Common.AddParameter(cmd, "@ID", DbType.String).Value = message;
                    Common.AddParameter(cmd, "@FYHJ", DbType.Currency).Value = FYHJ;
                    cmd.ExecuteNonQuery();

                    // 3. 订单人员名单
                    for (int i = 0; i < XM.Length; i++)
                    {
                        string xm = XM[i];
                        string sfzh = SFZH[i];
                        string xb = XB[i];
                        string csrq = CSRQ[i];
                        string lxdh = LXDH[i];

                        if (string.IsNullOrWhiteSpace(xm)) throw new Exception("[姓名]不能有空");
                        if (!string.IsNullOrWhiteSpace(sfzh) && sfzh.Trim().Length > 18) throw new Exception("[身份证号]过长");
                        if (!string.IsNullOrWhiteSpace(csrq))
                            try { DateTime.Parse(csrq); }
                            catch (Exception) { throw new Exception(string.Format("[出生日期]格式不正确: {0}", csrq)); }
                        if (!string.IsNullOrWhiteSpace(lxdh) && lxdh.Trim().Length > 11) throw new Exception("[联系电话]过长");

                        cmd.CommandText = "INSERT INTO [DRP].[TWN_DD_MD](ID,SN,XM,SFZH,XB,CSRQ,LXDH) VALUES(@ID,@SN,@XM,@SFZH,@XB,@CSRQ,@LXDH)";
                        cmd.Parameters.Clear();
                        Common.AddParameter(cmd, "@ID", DbType.String).Value = message;
                        Common.AddParameter(cmd, "@SN", DbType.Int32).Value = i + 1;
                        Common.AddParameter(cmd, "@XM", DbType.String).Value = xm.Trim();
                        Common.AddParameter(cmd, "@SFZH", DbType.String).Value = string.IsNullOrWhiteSpace(sfzh) ? Convert.DBNull : sfzh.Trim();
                        Common.AddParameter(cmd, "@XB", DbType.String).Value = string.IsNullOrWhiteSpace(xb) ? Convert.DBNull : xb.Trim();
                        Common.AddParameter(cmd, "@CSRQ", DbType.Date).Value = string.IsNullOrWhiteSpace(csrq) ? Convert.DBNull : DateTime.Parse(csrq);
                        Common.AddParameter(cmd, "@LXDH", DbType.String).Value = string.IsNullOrWhiteSpace(lxdh) ? Convert.DBNull : lxdh.Trim();
                        cmd.ExecuteNonQuery();
                    }//for (int i = 0; i < XM.Length; i++)
                }//DbCommand
                ta.Commit();
            }//DbTransaction
        }//DbConnection
    }//INSERT

    private void UPDATE()
    {
        //////////////////////////////////////////////////////////////////////////
        // 表单验证
        //////////////////////////////////////////////////////////////////////////
        string ID = Request["ID"];
        if (string.IsNullOrWhiteSpace(ID)) throw new Exception("[主键]不能为空");

        string[] XM = Request.Params.GetValues("XM");
        string[] SFZH = Request.Params.GetValues("SFZH");
        string[] XB = Request.Params.GetValues("XB");
        string[] CSRQ = Request.Params.GetValues("CSRQ");
        string[] LXDH = Request.Params.GetValues("LXDH");
        if (XM.Length != SFZH.Length || XM.Length != XB.Length || XM.Length != CSRQ.Length || XM.Length != LXDH.Length) throw new Exception("[名单]各项数量不一致");
        if (XM.Length == 0) throw new Exception("[名单]不能为空");

        //////////////////////////////////////////////////////////////////////////
        // 数据库操作
        //////////////////////////////////////////////////////////////////////////
        string providerName = ConfigurationManager.ConnectionStrings["HuiHuang"].ProviderName;
        string connectionString = ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString;
        DbProviderFactory df = DbProviderFactories.GetFactory(providerName);
        using (DbConnection cn = df.CreateConnection())
        {
            cn.ConnectionString = connectionString;
            cn.Open();
            using (DbTransaction ta = cn.BeginTransaction())
            {
                using (DbCommand cmd = cn.CreateCommand())
                {
                    cmd.Transaction = ta;
                    cmd.CommandText = @"DELETE FROM [DRP].[TWN_DD_MD] WHERE ID=@ID";
                    cmd.Parameters.Clear();
                    Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                    cmd.ExecuteNonQuery();
                    message = ID.Trim();

                    for (int i = 0; i < XM.Length; i++)
                    {
                        string xm = XM[i];
                        string sfzh = SFZH[i];
                        string xb = XB[i];
                        string csrq = CSRQ[i];
                        string lxdh = LXDH[i];

                        if (string.IsNullOrWhiteSpace(xm)) throw new Exception("[姓名]不能有空");
                        if (!string.IsNullOrWhiteSpace(sfzh) && sfzh.Trim().Length > 18) throw new Exception("[身份证号]过长");
                        if (!string.IsNullOrWhiteSpace(csrq))
                            try { DateTime.Parse(csrq); }
                            catch (Exception) { throw new Exception(string.Format("[出生日期]格式不正确: {0}", csrq)); }
                        if (!string.IsNullOrWhiteSpace(lxdh) && lxdh.Trim().Length > 11) throw new Exception("[联系电话]过长");

                        cmd.CommandText = "INSERT INTO [DRP].[TWN_DD_MD](ID,SN,XM,SFZH,XB,CSRQ,LXDH) VALUES(@ID,@SN,@XM,@SFZH,@XB,@CSRQ,@LXDH)";
                        cmd.Parameters.Clear();
                        Common.AddParameter(cmd, "@ID", DbType.String).Value = message;
                        Common.AddParameter(cmd, "@SN", DbType.Int32).Value = i + 1;
                        Common.AddParameter(cmd, "@XM", DbType.String).Value = xm.Trim();
                        Common.AddParameter(cmd, "@SFZH", DbType.String).Value = string.IsNullOrWhiteSpace(sfzh) ? Convert.DBNull : sfzh.Trim();
                        Common.AddParameter(cmd, "@XB", DbType.String).Value = string.IsNullOrWhiteSpace(xb) ? Convert.DBNull : xb.Trim();
                        Common.AddParameter(cmd, "@CSRQ", DbType.Date).Value = string.IsNullOrWhiteSpace(csrq) ? Convert.DBNull : DateTime.Parse(csrq);
                        Common.AddParameter(cmd, "@LXDH", DbType.String).Value = string.IsNullOrWhiteSpace(lxdh) ? Convert.DBNull : lxdh.Trim();
                        cmd.ExecuteNonQuery();
                    }//for (int i = 0; i < XM.Length; i++)
                }//DbCommand
                ta.Commit();
            }//DbTransaction
        }//DbConnection
    }//UPDATE

}//class


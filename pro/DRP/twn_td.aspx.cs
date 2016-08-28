using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;

public partial class pro_DRP_twn_td : System.Web.UI.Page
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
            if (VERB.ToUpper() == "SELECT") SELECT();
            else if (VERB.ToUpper() == "INSERT") INSERT();
            else if (VERB.ToUpper() == "UPDATE") UPDATE();
            else if (VERB.ToUpper() == "COPY") COPY();
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
    }

    private void SELECT()
    {
        //////////////////////////////////////////////////////////////////////////
        // 表单验证
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
                cmd.CommandText = string.Format("SELECT COUNT(*) FROM [DRP].[TWN_TD] {0}", where);
                total = int.Parse(cmd.ExecuteScalar().ToString());
                cmd.CommandText = string.Format(
                    @"SELECT *, 
                        ISNULL((SELECT SUM (RS) FROM [DRP].[TWN_DD_RS] WHERE ID IN ( SELECT ID FROM [DRP].[TWN_DD] WHERE TD_ID = X.ID AND ZT IN('1','2') )), 0 ) AS YDRS, 
                        RS - ISNULL((SELECT SUM (RS) FROM [DRP].[TWN_DD_RS] WHERE ID IN ( SELECT ID FROM [DRP].[TWN_DD] WHERE TD_ID = X.ID AND ZT IN('1','2') )), 0 ) AS SYRS, 
                        (SELECT username FROM [ADMIN].[User] WHERE ID=X.FBR) AS FBR1, 
                        (CASE ZT WHEN '1' THEN '开通' WHEN '0' THEN '关闭' END) AS ZT1 
                    FROM ( SELECT *, ROW_NUMBER () OVER (ORDER BY CFRQ DESC) AS RowNumber FROM [DRP].[TWN_TD] {0} ) AS X 
                    WHERE RowNumber > {1} AND RowNumber <= {2} ORDER BY CFRQ DESC",
                    where, rows*(page - 1), rows*page);
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
                            else if (Regex.IsMatch(dr.GetName(ordinal), @"\A(CFRQ)\z", RegexOptions.IgnoreCase))
                                item.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd"));
                            else if (Regex.IsMatch(dr.GetName(ordinal), @"\A(DFC)\z", RegexOptions.IgnoreCase))
                                item.Add(dr.GetName(ordinal), float.Parse(dr[ordinal].ToString()).ToString("0.00"));
                            else if (Regex.IsMatch(dr.GetName(ordinal), @"\A(FBSJ)\z", RegexOptions.IgnoreCase))
                                item.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                            else
                                item.Add(dr.GetName(ordinal), dr[ordinal]);
                        }//for (int ordinal = 0; ordinal < dr.FieldCount; ordinal++)
                        item.Add("BJ", new List<Dictionary<string, Object>>());
                    }//while (dr.Read())
                }//DbDataReader

                List<string> IDs = new List<string>();
                foreach (var item in items) IDs.Add(item["ID"].ToString());
                if (IDs.Count > 0)
                {
                    cmd.CommandText = string.Format("SELECT * FROM [DRP].[TWN_TD_BJ] WHERE ID IN({0}) ORDER BY ID,SN", string.Join(",", IDs)); ;
                    using (DbDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            List<Dictionary<string, Object>> BJ = items.Where(x => x["ID"].ToString() == dr["ID"].ToString()).FirstOrDefault()["BJ"] as List<Dictionary<string, Object>>;
                            Dictionary<string, Object> bj = new Dictionary<string, object>();
                            BJ.Add(bj);
                            bj.Add("SN", dr["SN"]);
                            bj.Add("BJMC", dr["BJMC"]);
                            bj.Add("MSJ", float.Parse(dr["MSJ"].ToString()).ToString("0.00"));
                            bj.Add("FL", float.Parse(dr["FL"].ToString()).ToString("0.00"));
                        }//while (dr.Read())
                    }//DbDataReader
                }
            }//DbCommand
        }//DbConnection
    }//SELECT

    private void INSERT()
    {
        //////////////////////////////////////////////////////////////////////////
        // 表单验证
        //////////////////////////////////////////////////////////////////////////
        string BH = Request["BH"];
        if (string.IsNullOrWhiteSpace(BH)) throw new Exception("[团队编号]不能为空");

        string XLMC = Request["XLMC"];
        if (string.IsNullOrWhiteSpace(XLMC)) throw new Exception("[线路名称]不能为空");

        string CFRQ = Request["CFRQ"];
        if (string.IsNullOrWhiteSpace(CFRQ)) throw new Exception("[出发日期]不能为空");
        try { DateTime.ParseExact(CFRQ, "yyyy-MM-dd", null); }
        catch (Exception) { throw new Exception(string.Format("[出发日期]格式不正确: {0}", CFRQ)); }

        string ZS = Request["ZS"];
        if (string.IsNullOrWhiteSpace(ZS)) throw new Exception("[住宿]不能为空");

        string ZT = Request["ZT"];
        if (string.IsNullOrWhiteSpace(ZT)) throw new Exception("[状态]不能为空");

        string RS = Request["RS"];
        if (string.IsNullOrWhiteSpace(RS)) throw new Exception("[人数]不能为空");
        try { int.Parse(RS); }
        catch (Exception) { throw new Exception(string.Format("[人数]格式不正确: {0}", RS)); }

        string TS = Request["TS"];
        if (string.IsNullOrWhiteSpace(TS)) throw new Exception("[天数]不能为空");
        try { int.Parse(TS); }
        catch (Exception) { throw new Exception(string.Format("[天数]格式不正确: {0}", TS)); }

        string DFC = Request["DFC"];
        if (string.IsNullOrWhiteSpace(DFC)) throw new Exception("[单房差]不能为空");
        try { float.Parse(DFC); }
        catch (Exception) { throw new Exception(string.Format("[单房差]格式不正确: {0}", DFC)); }

        string XC = Request["XC"];
        if (string.IsNullOrWhiteSpace(XC)) throw new Exception("[行程]不能为空");

        string[] BJMC = Request.Params.GetValues("BJMC");
        string[] MSJ = Request.Params.GetValues("MSJ");
        string[] FL = Request.Params.GetValues("FL");
        if (BJMC.Length != MSJ.Length) throw new Exception("[报价名称]与[门市价]数量不一致");
        if (BJMC.Length != FL.Length) throw new Exception("[报价名称]与[返利]数量不一致");
        if (BJMC.Length == 0) throw new Exception("[报价名称]与[门市价]不能为空");

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

                    cmd.CommandText = "INSERT INTO [DRP].[TWN_TD](BH, XLMC, CFRQ, ZS, TS, DFC, RS, ZT, FBR, FBSJ) VALUES(@BH, @XLMC, @CFRQ, @ZS, @TS, @DFC, @RS, @ZT, @FBR, @FBSJ);SELECT  @@IDENTITY";
                    cmd.Parameters.Clear();
                    Common.AddParameter(cmd, "@BH", DbType.String).Value = BH.Trim().ToUpper();
                    Common.AddParameter(cmd, "@XLMC", DbType.String).Value = XLMC.Trim();
                    Common.AddParameter(cmd, "@CFRQ", DbType.Date).Value = DateTime.Parse(CFRQ);
                    Common.AddParameter(cmd, "@ZS", DbType.String).Value = ZS.Trim();
                    Common.AddParameter(cmd, "@TS", DbType.Int32).Value = int.Parse(TS);
                    Common.AddParameter(cmd, "@DFC", DbType.Currency).Value = float.Parse(DFC);
                    Common.AddParameter(cmd, "@RS", DbType.Int32).Value = int.Parse(RS);
                    Common.AddParameter(cmd, "@ZT", DbType.String).Value = ZT.Trim();

                    Common.AddParameter(cmd, "@FBR", DbType.Int32).Value = User.Identity.Name;
                    Common.AddParameter(cmd, "@FBSJ", DbType.String).Value = DateTime.Now;
                    message = cmd.ExecuteScalar().ToString();

                    cmd.CommandText = "INSERT INTO [DRP].[TWN_TD_FJ](ID, XC) VALUES(@ID, @XC)";
                    cmd.Parameters.Clear();
                    Common.AddParameter(cmd, "@ID", DbType.String).Value = message;
                    Common.AddParameter(cmd, "@XC", DbType.Binary).Value = File.ReadAllBytes(Server.MapPath("~/pub/attachment/" + XC));
                    cmd.ExecuteNonQuery();

                    for (int i = 0; i < BJMC.Length; i++)
                    {
                        string bjmc = BJMC[i];
                        string msj = MSJ[i];
                        string fl = FL[i];
                        if (string.IsNullOrWhiteSpace(bjmc)) throw new Exception("[报价名称]不能有空");
                        if (string.IsNullOrWhiteSpace(msj)) throw new Exception("[门市价]不能有空");
                        try { float.Parse(msj); }
                        catch (Exception) { throw new Exception(string.Format("[门市价]格式不正确: {0}", msj)); }
                        if (string.IsNullOrWhiteSpace(fl)) throw new Exception("[返利]不能有空");
                        try { float.Parse(fl); }
                        catch (Exception) { throw new Exception(string.Format("[返利]格式不正确: {0}", fl)); }

                        cmd.CommandText = "INSERT INTO [DRP].[TWN_TD_BJ](ID,SN,BJMC,MSJ,FL) VALUES(@ID,@SN,@BJMC,@MSJ,@FL)";
                        cmd.Parameters.Clear();
                        Common.AddParameter(cmd, "@ID", DbType.String).Value = message;
                        Common.AddParameter(cmd, "@SN", DbType.Int32).Value = i + 1;
                        Common.AddParameter(cmd, "@BJMC", DbType.String).Value = bjmc;
                        Common.AddParameter(cmd, "@MSJ", DbType.String).Value = msj;
                        Common.AddParameter(cmd, "@FL", DbType.String).Value = fl;
                        cmd.ExecuteNonQuery();
                    }

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

        string BH = Request["BH"];
        if (string.IsNullOrWhiteSpace(BH)) throw new Exception("[团队编号]不能为空");

        string XLMC = Request["XLMC"];
        if (string.IsNullOrWhiteSpace(XLMC)) throw new Exception("[线路名称]不能为空");

        string CFRQ = Request["CFRQ"];
        if (string.IsNullOrWhiteSpace(CFRQ)) throw new Exception("[出发日期]不能为空");
        try { DateTime.ParseExact(CFRQ, "yyyy-MM-dd", null); }
        catch (Exception) { throw new Exception(string.Format("[出发日期]格式不正确: {0}", CFRQ)); }

        string ZS = Request["ZS"];
        if (string.IsNullOrWhiteSpace(ZS)) throw new Exception("[住宿]不能为空");

        string ZT = Request["ZT"];
        if (string.IsNullOrWhiteSpace(ZT)) throw new Exception("[状态]不能为空");

        string RS = Request["RS"];
        if (string.IsNullOrWhiteSpace(RS)) throw new Exception("[人数]不能为空");
        try { int.Parse(RS); }
        catch (Exception) { throw new Exception(string.Format("[人数]格式不正确: {0}", RS)); }

        string TS = Request["TS"];
        if (string.IsNullOrWhiteSpace(TS)) throw new Exception("[天数]不能为空");
        try { int.Parse(TS); }
        catch (Exception) { throw new Exception(string.Format("[天数]格式不正确: {0}", TS)); }

        string DFC = Request["DFC"];
        if (string.IsNullOrWhiteSpace(DFC)) throw new Exception("[单房差]不能为空");
        try { float.Parse(DFC); }
        catch (Exception) { throw new Exception(string.Format("[单房差]格式不正确: {0}", DFC)); }

        string XC = Request["XC"];
        //if (string.IsNullOrWhiteSpace(XC)) throw new Exception("[行程]不能为空");

        string[] BJMC = Request.Params.GetValues("BJMC");
        string[] MSJ = Request.Params.GetValues("MSJ");
        string[] FL = Request.Params.GetValues("FL");

        if (BJMC.Length != MSJ.Length) throw new Exception("[报价名称]与[门市价]数量不一致");
        if (BJMC.Length != FL.Length) throw new Exception("[报价名称]与[返利]数量不一致");
        if (BJMC.Length == 0) throw new Exception("[报价名称]与[门市价]不能为空");
        //List<object> BJ = new List<object>();

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

                    cmd.CommandText = "UPDATE [DRP].[TWN_TD] SET BH=@BH, XLMC=@XLMC, CFRQ=@CFRQ, ZS=@ZS, TS=@TS, DFC=@DFC, RS=@RS, ZT=@ZT WHERE ID=@ID";
                    cmd.Parameters.Clear();
                    Common.AddParameter(cmd, "@ID", DbType.String).Value = ID.Trim();
                    Common.AddParameter(cmd, "@BH", DbType.String).Value = BH.Trim().ToUpper();
                    Common.AddParameter(cmd, "@XLMC", DbType.String).Value = XLMC.Trim();
                    Common.AddParameter(cmd, "@CFRQ", DbType.Date).Value = DateTime.Parse(CFRQ);
                    Common.AddParameter(cmd, "@ZS", DbType.String).Value = ZS.Trim();
                    Common.AddParameter(cmd, "@TS", DbType.Int32).Value = int.Parse(TS);
                    Common.AddParameter(cmd, "@DFC", DbType.Currency).Value = float.Parse(DFC);
                    Common.AddParameter(cmd, "@RS", DbType.Int32).Value = int.Parse(RS);
                    Common.AddParameter(cmd, "@ZT", DbType.String).Value = ZT.Trim();
                    cmd.ExecuteNonQuery();
                    message = ID.Trim();

                    if (!string.IsNullOrWhiteSpace(XC))
                    {
                        cmd.CommandText = "UPDATE [DRP].[TWN_TD_FJ] SET XC=@XC WHERE ID=@ID";
                        cmd.Parameters.Clear();
                        Common.AddParameter(cmd, "@ID", DbType.String).Value = ID.Trim();
                        Common.AddParameter(cmd, "@XC", DbType.Binary).Value = File.ReadAllBytes(Server.MapPath("~/pub/attachment/" + XC));
                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommandText = "DELETE FROM [DRP].[TWN_TD_BJ] WHERE ID=@ID";
                    cmd.Parameters.Clear();
                    Common.AddParameter(cmd, "@ID", DbType.String).Value = ID.Trim();
                    cmd.ExecuteNonQuery();
                    for (int i = 0; i < BJMC.Length; i++)
                    {
                        string bjmc = BJMC[i];
                        string msj = MSJ[i];
                        string fl = FL[i];
                        if (string.IsNullOrWhiteSpace(bjmc)) throw new Exception("[报价名称]不能有空");
                        if (string.IsNullOrWhiteSpace(msj)) throw new Exception("[门市价]不能有空");
                        try { float.Parse(msj); }
                        catch (Exception) { throw new Exception(string.Format("[门市价]格式不正确: {0}", msj)); }
                        if (string.IsNullOrWhiteSpace(fl)) throw new Exception("[返利]不能有空");
                        try { float.Parse(fl); }
                        catch (Exception) { throw new Exception(string.Format("[返利]格式不正确: {0}", fl)); }

                        cmd.CommandText = "INSERT INTO [DRP].[TWN_TD_BJ](ID,SN,BJMC,MSJ,FL) VALUES(@ID,@SN,@BJMC,@MSJ,@FL)";
                        cmd.Parameters.Clear();
                        Common.AddParameter(cmd, "@ID", DbType.String).Value = message;
                        Common.AddParameter(cmd, "@SN", DbType.Int32).Value = i + 1;
                        Common.AddParameter(cmd, "@BJMC", DbType.String).Value = bjmc;
                        Common.AddParameter(cmd, "@MSJ", DbType.String).Value = msj;
                        Common.AddParameter(cmd, "@FL", DbType.String).Value = fl;
                        cmd.ExecuteNonQuery();
                    }


                }//DbCommand
                ta.Commit();
            }//DbTransaction
        }//DbConnection
    }//UPDATE

    private void COPY()
    {
        //////////////////////////////////////////////////////////////////////////
        // 表单验证
        //////////////////////////////////////////////////////////////////////////
        string ID = Request["ID"];
        if (string.IsNullOrWhiteSpace(ID)) throw new Exception("[主键]不能为空");

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
                    cmd.CommandText = "INSERT INTO [DRP].[TWN_TD](BH, XLMC, CFRQ, ZS, TS, DFC, RS, ZT, FBR, FBSJ) SELECT BH, XLMC, CFRQ, ZS, TS, DFC, RS, ZT, @FBR, @FBSJ FROM [DRP].[TWN_TD] WHERE ID=@ID;SELECT  @@IDENTITY";
                    Common.AddParameter(cmd, "@ID", DbType.Int32).Value = ID;
                    Common.AddParameter(cmd, "@FBR", DbType.Int32).Value = User.Identity.Name;
                    Common.AddParameter(cmd, "@FBSJ", DbType.String).Value = DateTime.Now;
                    message = cmd.ExecuteScalar().ToString();
                }//DbCommand
                using (DbCommand cmd = cn.CreateCommand())
                {
                    cmd.Transaction = ta;
                    cmd.CommandText = "INSERT INTO [DRP].[TWN_TD_BJ](ID, SN, BJMC, MSJ, FL) SELECT @ID1, SN, BJMC, MSJ, FL FROM [DRP].[TWN_TD_BJ] WHERE ID=@ID2";
                    Common.AddParameter(cmd, "@ID1", DbType.Int32).Value = message;
                    Common.AddParameter(cmd, "@ID2", DbType.Int32).Value = ID;
                    cmd.ExecuteNonQuery();
                }//DbCommand
                using (DbCommand cmd = cn.CreateCommand())
                {
                    cmd.Transaction = ta;
                    cmd.CommandText = "INSERT INTO [DRP].[TWN_TD_FJ](ID, XC) SELECT @ID1, XC FROM [DRP].[TWN_TD_FJ] WHERE ID=@ID2";
                    Common.AddParameter(cmd, "@ID1", DbType.Int32).Value = message;
                    Common.AddParameter(cmd, "@ID2", DbType.Int32).Value = ID;
                    cmd.ExecuteNonQuery();
                }//DbCommand
                ta.Commit();
            }//DbTransaction
        }//DbConnection
    }//COPY
}//class
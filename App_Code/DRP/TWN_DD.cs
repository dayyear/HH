using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Common;
using System.Text;
using System.Data;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace DRP
{
    public class TWN_DD
    {
        public TWN_DD() { }

        public static Resp SELECT(
            IPrincipal User, string ID = null,
            string rows = null, string page = null)
        {
            Resp resp = new Resp();
            try
            {
                //////////////////////////////////////////////////////////////////////////
                // 表单验证
                //////////////////////////////////////////////////////////////////////////
                StringBuilder where = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(ID))
                    where.AppendFormat(" and ID=@ID");
                if (!User.IsInRole("ADMIN"))
                    where.AppendFormat(" and YDR=@YDR");

                int result;
                if (!int.TryParse(rows, out result))
                    rows = "10";
                if (!int.TryParse(page, out result))
                    page = "1";

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
                        cmd.CommandText = string.Format(@"SELECT COUNT(*) FROM [DRP].[TWN_DD] WHERE 1=1 {0}", where);
                        if (!string.IsNullOrWhiteSpace(ID))
                            Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                        if (!User.IsInRole("ADMIN"))
                            Common.AddParameter(cmd, "@YDR", DbType.String).Value = User.Identity.Name;
                        resp.total = int.Parse(cmd.ExecuteScalar().ToString());
                    }//DbCommand
                    using (DbCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = string.Format(
                            @"SELECT A.*, 
                                ( SELECT username FROM [ADMIN].[User] WHERE ID = A.YDR ) AS YDR1,
                                ( SELECT MC FROM [ADMIN].[DM_SCD] WHERE DM = A.SCD ) AS SCD1,
                                ( CASE ZT WHEN '1' THEN '待审核' WHEN '2' THEN '已审核' WHEN '0' THEN '已取消' END ) AS ZT1,
                                B.SN AS SN1,B.BJMC,B.MSJ,B.FL,B.RS,B.RS_DFC,B.FYXJ,
                                C.SN AS SN2,C.XM,C.SFZH,C.XB,C.CSRQ,C.LXDH,C.SFJS
                            FROM ( SELECT * FROM ( SELECT *, ROW_NUMBER () OVER (ORDER BY YDSJ DESC) AS RowNumber FROM [DRP].[TWN_DD] WHERE 1=1 {0}) AS X
                            WHERE RowNumber > {1} AND RowNumber <= {2}
                            ) AS A LEFT JOIN [DRP].[TWN_DD_RS] AS B ON A.ID=B.ID
                            LEFT JOIN [DRP].[TWN_DD_MD] AS C ON A.ID=C.ID
                            ORDER BY A.RowNumber,B.SN,C.SN", where, int.Parse(rows) * (int.Parse(page) - 1), int.Parse(rows) * int.Parse(page));
                        if (!string.IsNullOrWhiteSpace(ID))
                            Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                        if (!User.IsInRole("ADMIN"))
                            Common.AddParameter(cmd, "@YDR", DbType.String).Value = User.Identity.Name;
                        using (DbDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Dictionary<string, Object> item;
                                var itemTmp = resp.items.Where(x => x["ID"].ToString() == dr["ID"].ToString());
                                if (itemTmp.Count() > 0) item = itemTmp.First();
                                else
                                {
                                    item = new Dictionary<string, object>();
                                    resp.items.Add(item);
                                    for (int ordinal = 0; ordinal < 25; ordinal++)
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
                                }

                                List<Dictionary<string, Object>> itemRS = item["RS"] as List<Dictionary<string, Object>>;
                                if (!Convert.IsDBNull(dr["SN1"]) && itemRS.Where(x => x["SN"].ToString() == dr["SN1"].ToString()).Count() == 0)
                                {
                                    Dictionary<string, object> rs = new Dictionary<string, object>();
                                    itemRS.Add(rs);
                                    for (int ordinal = 25; ordinal < 32; ordinal++)
                                    {
                                        if (Convert.IsDBNull(dr[ordinal]))
                                            rs.Add(dr.GetName(ordinal), null);
                                        else if (dr.GetName(ordinal) == "SN1")
                                            rs.Add("SN", dr[ordinal]);
                                        else if (dr.GetDataTypeName(ordinal) == "date")
                                            rs.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd"));
                                        else if (dr.GetDataTypeName(ordinal) == "money")
                                            rs.Add(dr.GetName(ordinal), float.Parse(dr[ordinal].ToString()).ToString("0.00"));
                                        else if (dr.GetDataTypeName(ordinal) == "datetime2")
                                            rs.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                        else
                                            rs.Add(dr.GetName(ordinal), dr[ordinal]);
                                    }//for (int ordinal = 0; ordinal < dr.FieldCount; ordinal++)
                                }

                                List<Dictionary<string, Object>> itemMD = item["MD"] as List<Dictionary<string, Object>>;
                                if (!Convert.IsDBNull(dr["SN2"]) && itemMD.Where(x => x["SN"].ToString() == dr["SN2"].ToString()).Count() == 0)
                                {
                                    Dictionary<string, object> md = new Dictionary<string, object>();
                                    itemMD.Add(md);
                                    for (int ordinal = 32; ordinal < 39; ordinal++)
                                    {
                                        if (Convert.IsDBNull(dr[ordinal]))
                                            md.Add(dr.GetName(ordinal), null);
                                        else if (dr.GetName(ordinal) == "SN2")
                                            md.Add("SN", dr[ordinal]);
                                        else if (dr.GetDataTypeName(ordinal) == "date")
                                            md.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd"));
                                        else if (dr.GetDataTypeName(ordinal) == "money")
                                            md.Add(dr.GetName(ordinal), float.Parse(dr[ordinal].ToString()).ToString("0.00"));
                                        else if (dr.GetDataTypeName(ordinal) == "datetime2")
                                            md.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                        else
                                            md.Add(dr.GetName(ordinal), dr[ordinal]);
                                    }//for (int ordinal = 0; ordinal < dr.FieldCount; ordinal++)
                                }

                            }//while (dr.Read())
                        }//DbDataReader
                    }//DbCommand
                }//DbConnection
            }//try
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = ex.Message;
                resp.total = 0;
                resp.items.Clear();
            }
            return resp;
        }//SELECT

        public static Resp INSERT(
            string TD_ID, string SCD, string BZ, string JBR, string JBRSJ,
            string[] RS, string[] RS_DFC,
            string[] XM, string[] SFZH, string[] XB, string[] CSRQ, string[] LXDH, string[] SFJS,
            IPrincipal User)
        {
            Resp resp = new Resp();
            try
            {
                string ID;
                //////////////////////////////////////////////////////////////////////////
                // 表单验证
                //////////////////////////////////////////////////////////////////////////
                if (string.IsNullOrWhiteSpace(TD_ID))
                    throw new Exception("[团队ID]不能为空");
                if (string.IsNullOrWhiteSpace(SCD))
                    throw new Exception("[上车点]不能为空");
                if (string.IsNullOrWhiteSpace(BZ))
                    BZ = "";
                if (string.IsNullOrWhiteSpace(JBR))
                    throw new Exception("[经办人]不能为空");
                if (string.IsNullOrWhiteSpace(JBRSJ))
                    throw new Exception("[经办人手机]不能为空");

                if (RS.Length == 0)
                    throw new Exception("[人数]与[人数(单房差)]不能为空");
                if (XM.Length == 0)
                    throw new Exception("[名单]不能为空");

                //////////////////////////////////////////////////////////////////////////
                // 团队信息留痕
                //////////////////////////////////////////////////////////////////////////
                //团队基本信息
                string TD_BH, TD_XLMC, TD_CFRQ, TD_ZS, TD_TS, TD_DFC, TD_RS, TD_FBR, TD_FBSJ;
                //团队报价信息
                List<Dictionary<string, string>> TD_BJ = new List<Dictionary<string, string>>();

                string providerName = ConfigurationManager.ConnectionStrings["HuiHuang"].ProviderName;
                string connectionString = ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString;
                DbProviderFactory df = DbProviderFactories.GetFactory(providerName);
                using (DbConnection cn = df.CreateConnection())
                {
                    cn.ConnectionString = connectionString;
                    cn.Open();
                    #region 1. 读取团队基本信息
                    using (DbCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText =
                            @"SELECT *, (SELECT username FROM [ADMIN].[User] WHERE ID=X.FBR) AS FBR1 
                            FROM [DRP].[TWN_TD] AS X WHERE ID=@ID";
                        Common.AddParameter(cmd, "@ID", DbType.String).Value = TD_ID;
                        using (DbDataReader dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                                throw new Exception("[团队ID]不存在：" + TD_ID);
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
                    }//DbCommand
                    #endregion
                    #region 2. 读取团队报价信息
                    using (DbCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText =
                            @"SELECT * FROM [DRP].[TWN_TD_BJ] WHERE ID=@ID ORDER BY SN";
                        Common.AddParameter(cmd, "@ID", DbType.String).Value = TD_ID;
                        using (DbDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Dictionary<string, string> td_bj = new Dictionary<string, string>();
                                TD_BJ.Add(td_bj);
                                td_bj.Add("SN", dr["SN"].ToString());
                                td_bj.Add("BJMC", dr["BJMC"].ToString());
                                td_bj.Add("MSJ", dr["MSJ"].ToString());
                                td_bj.Add("FL", dr["FL"].ToString());
                            }
                        }//DbDataReader
                    }//DbCommand
                    #endregion
                }//DbConnection

                //////////////////////////////////////////////////////////////////////////
                // 检测预定人数
                //////////////////////////////////////////////////////////////////////////
                using (DbConnection cn = df.CreateConnection())
                {
                    cn.ConnectionString = connectionString;
                    cn.Open();
                    using (DbCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT ISNULL(SUM(RS),0) FROM [DRP].[TWN_DD_RS] WHERE ID IN (SELECT ID FROM [DRP].[TWN_DD] WHERE TD_ID = @TD_ID AND ZT<>'0')";
                        Common.AddParameter(cmd, "@TD_ID", DbType.String).Value = TD_ID;
                        //throw new Exception(TD_ID);
                        if (int.Parse(cmd.ExecuteScalar().ToString()) + XM.Length > int.Parse(TD_RS))
                            throw new Exception("预定人数过多");
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
                        #region 1. 订单基本信息
                        using (DbCommand cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = ta;
                            cmd.CommandText =
                                @"INSERT INTO 
                                [DRP].[TWN_DD](TD_ID, TD_BH, TD_XLMC, TD_CFRQ, TD_ZS, TD_TS, TD_DFC, TD_RS, TD_FBR, TD_FBSJ, SCD, BZ, JBR, JBRSJ, ZT, YDR, YDSJ) 
                                VALUES(@TD_ID, @TD_BH, @TD_XLMC, @TD_CFRQ, @TD_ZS, @TD_TS, @TD_DFC, @TD_RS, @TD_FBR, @TD_FBSJ, @SCD, @BZ, @JBR, @JBRSJ, @ZT, @YDR, @YDSJ);
                                SELECT  @@IDENTITY";
                            Common.AddParameter(cmd, "@TD_ID", DbType.String).Value = string.IsNullOrWhiteSpace(TD_ID) ? Convert.DBNull : TD_ID.Trim();
                            Common.AddParameter(cmd, "@TD_BH", DbType.String).Value = string.IsNullOrWhiteSpace(TD_BH) ? Convert.DBNull : TD_BH.Trim().ToUpper();
                            Common.AddParameter(cmd, "@TD_XLMC", DbType.String).Value = string.IsNullOrWhiteSpace(TD_XLMC) ? Convert.DBNull : TD_XLMC.Trim();
                            Common.AddParameter(cmd, "@TD_CFRQ", DbType.String).Value = string.IsNullOrWhiteSpace(TD_CFRQ) ? Convert.DBNull : TD_CFRQ.Trim();
                            Common.AddParameter(cmd, "@TD_ZS", DbType.String).Value = string.IsNullOrWhiteSpace(TD_ZS) ? Convert.DBNull : TD_ZS.Trim();
                            Common.AddParameter(cmd, "@TD_TS", DbType.String).Value = string.IsNullOrWhiteSpace(TD_TS) ? Convert.DBNull : TD_TS.Trim();
                            Common.AddParameter(cmd, "@TD_DFC", DbType.String).Value = string.IsNullOrWhiteSpace(TD_DFC) ? Convert.DBNull : TD_DFC.Trim();
                            Common.AddParameter(cmd, "@TD_RS", DbType.String).Value = string.IsNullOrWhiteSpace(TD_RS) ? Convert.DBNull : TD_RS.Trim();
                            Common.AddParameter(cmd, "@TD_FBR", DbType.String).Value = string.IsNullOrWhiteSpace(TD_FBR) ? Convert.DBNull : TD_FBR.Trim();
                            Common.AddParameter(cmd, "@TD_FBSJ", DbType.String).Value = string.IsNullOrWhiteSpace(TD_FBSJ) ? Convert.DBNull : TD_FBSJ.Trim();

                            Common.AddParameter(cmd, "@SCD", DbType.String).Value = string.IsNullOrWhiteSpace(SCD) ? Convert.DBNull : SCD.Trim();
                            Common.AddParameter(cmd, "@BZ", DbType.String).Value = string.IsNullOrWhiteSpace(BZ) ? Convert.DBNull : BZ.Trim();
                            Common.AddParameter(cmd, "@JBR", DbType.String).Value = string.IsNullOrWhiteSpace(JBR) ? Convert.DBNull : JBR.Trim();
                            Common.AddParameter(cmd, "@JBRSJ", DbType.String).Value = string.IsNullOrWhiteSpace(JBRSJ) ? Convert.DBNull : JBRSJ.Trim();

                            //Common.AddParameter(cmd, "@FYHJ", DbType.String).Value = "0";
                            //Common.AddParameter(cmd, "@JSJG", DbType.String).Value = "0";
                            Common.AddParameter(cmd, "@ZT", DbType.String).Value = "1";
                            Common.AddParameter(cmd, "@YDR", DbType.String).Value = User.Identity.Name;
                            Common.AddParameter(cmd, "@YDSJ", DbType.String).Value = DateTime.Now;

                            ID = cmd.ExecuteScalar().ToString();
                        }//DbCommand
                        #endregion
                        #region 2. 订单费用计算
                        using (DbCommand cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = ta;
                            float FYHJ = 0.0F;
                            float FLHJ = 0.0F;
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
                                float FLXJ = int.Parse(rs)*float.Parse(TD_BJ[i]["FL"]);
                                FLHJ += FLXJ;

                                cmd.CommandText =
                                    @"INSERT INTO [DRP].[TWN_DD_RS](ID,SN,BJMC,MSJ,FL,RS,RS_DFC,FYXJ) 
                                    VALUES(@ID,@SN,@BJMC,@MSJ,@FL,@RS,@RS_DFC,@FYXJ)";
                                cmd.Parameters.Clear();
                                Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                                Common.AddParameter(cmd, "@SN", DbType.String).Value = i + 1;
                                Common.AddParameter(cmd, "@BJMC", DbType.String).Value = TD_BJ[i]["BJMC"];
                                Common.AddParameter(cmd, "@MSJ", DbType.String).Value = TD_BJ[i]["MSJ"];
                                Common.AddParameter(cmd, "@FL", DbType.String).Value = TD_BJ[i]["FL"];
                                Common.AddParameter(cmd, "@RS", DbType.String).Value = rs;
                                Common.AddParameter(cmd, "@RS_DFC", DbType.String).Value = rs_dfc;
                                Common.AddParameter(cmd, "@FYXJ", DbType.String).Value = FYXJ;
                                cmd.ExecuteNonQuery();
                            }//for (int i = 0; i < RS.Length; i++)

                            cmd.CommandText = "UPDATE [DRP].[TWN_DD] SET BH=@BH, FYHJ=@FYHJ, JSJG=@JSJG WHERE ID=@ID";
                            cmd.Parameters.Clear();
                            Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                            Common.AddParameter(cmd, "@BH", DbType.String).Value = "TW" + int.Parse(ID).ToString("00000000");
                            Common.AddParameter(cmd, "@FYHJ", DbType.String).Value = FYHJ;
                            Common.AddParameter(cmd, "@JSJG", DbType.String).Value = FYHJ - FLHJ;
                            cmd.ExecuteNonQuery();
                        }//DbCommand
                        #endregion
                        #region 3. 订单人员名单
                        using (DbCommand cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = ta;
                            for (int i = 0; i < XM.Length; i++)
                            {
                                string xm = XM[i];
                                string sfzh = SFZH[i];
                                string xb = XB[i];
                                string csrq = CSRQ[i];
                                string lxdh = LXDH[i];
                                string sfjs = SFJS[i];

                                if (string.IsNullOrWhiteSpace(xm)) throw new Exception("[姓名]不能有空");
                                if (!string.IsNullOrWhiteSpace(sfzh) && sfzh.Trim().Length > 18) throw new Exception("[身份证号]过长");
                                if (!string.IsNullOrWhiteSpace(csrq))
                                    try { DateTime.Parse(csrq); }
                                    catch (Exception) { throw new Exception(string.Format("[出生日期]格式不正确: {0}", csrq)); }
                                if (!string.IsNullOrWhiteSpace(lxdh) && lxdh.Trim().Length > 11) throw new Exception("[联系电话]过长");
                                if (string.IsNullOrWhiteSpace(sfjs)) throw new Exception("[是否含送接]不能有空");

                                cmd.CommandText =
                                    @"INSERT INTO [DRP].[TWN_DD_MD](ID,SN,XM,SFZH,XB,CSRQ,LXDH,SFJS) 
                                    VALUES(@ID,@SN,@XM,@SFZH,@XB,@CSRQ,@LXDH,@SFJS)";
                                cmd.Parameters.Clear();
                                Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                                Common.AddParameter(cmd, "@SN", DbType.String).Value = i + 1;
                                Common.AddParameter(cmd, "@XM", DbType.String).Value = string.IsNullOrWhiteSpace(xm) ? Convert.DBNull : xm.Trim();
                                Common.AddParameter(cmd, "@SFZH", DbType.String).Value = string.IsNullOrWhiteSpace(sfzh) ? Convert.DBNull : sfzh.Trim().ToUpper();
                                Common.AddParameter(cmd, "@XB", DbType.String).Value = string.IsNullOrWhiteSpace(xb) ? Convert.DBNull : xb.Trim();
                                Common.AddParameter(cmd, "@CSRQ", DbType.String).Value = string.IsNullOrWhiteSpace(csrq) ? Convert.DBNull : DateTime.Parse(csrq);
                                Common.AddParameter(cmd, "@LXDH", DbType.String).Value = string.IsNullOrWhiteSpace(lxdh) ? Convert.DBNull : lxdh.Trim();
                                Common.AddParameter(cmd, "@SFJS", DbType.String).Value = sfjs;
                                cmd.ExecuteNonQuery();
                            }//for (int i = 0; i < XM.Length; i++)
                        }//DbCommand
                        #endregion
                        ta.Commit();
                    }//DbTransaction
                }//DbConnection
                resp.message = ID;
            }//try
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = ex.Message;
                resp.total = 0;
                resp.items.Clear();
            }
            return resp;
        }//INSERT

        public static Resp UPDATE(
            string ID, string ZT,
            string[] XM, string[] SFZH, string[] XB, string[] CSRQ, string[] LXDH, string[] SFJS,
            IPrincipal User)
        {
            Resp resp = new Resp();
            try
            {
                //////////////////////////////////////////////////////////////////////////
                // 表单验证
                //////////////////////////////////////////////////////////////////////////
                if (string.IsNullOrWhiteSpace(ID))
                    throw new Exception("[主键]不能为空");

                if (XM.Length == 0)
                    throw new Exception("[名单]不能为空");

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
                        #region 清除名单
                        using (DbCommand cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = ta;
                            cmd.CommandText = @"DELETE FROM [DRP].[TWN_DD_MD] WHERE ID=@ID";
                            Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                            if (cmd.ExecuteNonQuery().ToString() == "0")
                                throw new Exception("无效的[主键]：" + ID);
                        }//DbCommand
                        #endregion
                        #region 依次添加名单
                        using (DbCommand cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = ta;
                            for (int i = 0; i < XM.Length; i++)
                            {
                                string xm = XM[i];
                                string sfzh = SFZH[i];
                                string xb = XB[i];
                                string csrq = CSRQ[i];
                                string lxdh = LXDH[i];
                                string sfjs = SFJS[i];

                                if (string.IsNullOrWhiteSpace(xm)) throw new Exception("[姓名]不能有空");
                                if (!string.IsNullOrWhiteSpace(sfzh) && sfzh.Trim().Length > 18) throw new Exception("[身份证号]过长");
                                if (!string.IsNullOrWhiteSpace(csrq))
                                    try { DateTime.Parse(csrq); }
                                    catch (Exception) { throw new Exception(string.Format("[出生日期]格式不正确: {0}", csrq)); }
                                if (!string.IsNullOrWhiteSpace(lxdh) && lxdh.Trim().Length > 11) throw new Exception("[联系电话]过长");
                                if (string.IsNullOrWhiteSpace(sfjs)) throw new Exception("[是否含接送]不能有空");

                                cmd.CommandText =
                                    @"INSERT INTO [DRP].[TWN_DD_MD](ID,SN,XM,SFZH,XB,CSRQ,LXDH,SFJS) 
                                    VALUES(@ID,@SN,@XM,@SFZH,@XB,@CSRQ,@LXDH,@SFJS)";
                                cmd.Parameters.Clear();
                                Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                                Common.AddParameter(cmd, "@SN", DbType.String).Value = i + 1;
                                Common.AddParameter(cmd, "@XM", DbType.String).Value = string.IsNullOrWhiteSpace(xm) ? Convert.DBNull : xm.Trim();
                                Common.AddParameter(cmd, "@SFZH", DbType.String).Value = string.IsNullOrWhiteSpace(sfzh) ? Convert.DBNull : sfzh.Trim().ToUpper();
                                Common.AddParameter(cmd, "@XB", DbType.String).Value = string.IsNullOrWhiteSpace(xb) ? Convert.DBNull : xb.Trim();
                                Common.AddParameter(cmd, "@CSRQ", DbType.String).Value = string.IsNullOrWhiteSpace(csrq) ? Convert.DBNull : DateTime.Parse(csrq);
                                Common.AddParameter(cmd, "@LXDH", DbType.String).Value = string.IsNullOrWhiteSpace(lxdh) ? Convert.DBNull : lxdh.Trim();
                                Common.AddParameter(cmd, "@SFJS", DbType.String).Value = sfjs;
                                cmd.ExecuteNonQuery();
                            }//for (int i = 0; i < XM.Length; i++)
                        }//DbCommand
                        #endregion
                        #region 更新状态
                        if(!string.IsNullOrWhiteSpace(ZT))
                            using (DbCommand cmd = cn.CreateCommand())
                            {
                                cmd.Transaction = ta;
                                cmd.CommandText = "UPDATE [DRP].[TWN_DD] SET ZT=@ZT WHERE ID=@ID";
                                Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                                Common.AddParameter(cmd, "@ZT", DbType.String).Value = ZT;
                                cmd.ExecuteNonQuery();
                            }//DbCommand
                        #endregion
                        ta.Commit();
                    }//DbTransaction
                }//DbConnection
                resp.message = ID;
            }//try
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = ex.Message;
                resp.total = 0;
                resp.items.Clear();
            }
            return resp;
        }//UPDATE

    }//public class TWN_DD
}//namespace
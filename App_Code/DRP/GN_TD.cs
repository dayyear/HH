using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Security.Principal;
using System.IO;
using System.Dynamic;

namespace DRP
{
    /// <summary>
    ///GN_TD 的摘要说明
    /// </summary>
    public class GN_TD
    {
        public static Resp Select(string ID = null, int rows = 10, int page = 1)
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
                        cmd.CommandText = string.Format(@"SELECT COUNT(*) FROM [DRP].[GN_TD] WHERE 1=1 {0}", where);
                        if (!string.IsNullOrWhiteSpace(ID))
                            Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                        resp.total = int.Parse(cmd.ExecuteScalar().ToString());
                    }//DbCommand

                    using (DbCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = string.Format(
                            @"SELECT A.*, 
                        ISNULL((SELECT SUM(RS) FROM [DRP].[GN_DD_RS] WHERE ID IN (SELECT ID FROM [DRP].[GN_DD] WHERE TD_ID=A.ID AND ZT IN('1','2'))), 0) AS YDRS,
                        RS - ISNULL((SELECT SUM(RS) FROM [DRP].[GN_DD_RS] WHERE ID IN (SELECT ID FROM [DRP].[GN_DD] WHERE TD_ID=A.ID AND ZT IN('1','2'))), 0) AS SYRS,
                        (SELECT MC FROM [DRP].[DM] WHERE LX='TD_ZT' AND DM=A.ZT) AS ZT1,
                        (SELECT username FROM [ADMIN].[User] WHERE ID=A.FBR) AS FBR1,
                        B.SN, B.BJMC, B.MSJ, B.FL
                        FROM ( SELECT * FROM ( SELECT *, ROW_NUMBER () OVER (ORDER BY CFRQ DESC) AS RowNumber FROM [DRP].[GN_TD] WHERE 1=1 {0}) AS X
                        WHERE RowNumber > {1} AND RowNumber <= {2} ) AS A 
                        LEFT JOIN [DRP].[GN_TD_BJ] AS B ON A.ID=B.ID
                        ORDER BY A.RowNumber,B.SN", where, rows * (page - 1), rows * page);
                        if (!string.IsNullOrWhiteSpace(ID))
                            Common.AddParameter(cmd, "@ID", DbType.String).Value = ID;
                        using (DbDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Dictionary<string, Object> item;
                                var itemTmp = resp.items.Where(x => x["ID"].ToString() == dr["ID"].ToString());
                                if (itemTmp.Any()) item = itemTmp.First();
                                else
                                {
                                    item = new Dictionary<string, object>();
                                    resp.items.Add(item);
                                    for (int ordinal = 0; ordinal < 16; ordinal++)
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
                                    item.Add("BJ", new List<Dictionary<string, Object>>());
                                }

                                List<Dictionary<string, Object>> itemBJ = item["BJ"] as List<Dictionary<string, Object>>;
                                if (!Convert.IsDBNull(dr["SN"]) && !itemBJ.Where(x => x["SN"].ToString() == dr["SN"].ToString()).Any())
                                {
                                    Dictionary<string, object> bj = new Dictionary<string, object>();
                                    itemBJ.Add(bj);
                                    for (int ordinal = 16; ordinal < 20; ordinal++)
                                    {
                                        if (Convert.IsDBNull(dr[ordinal]))
                                            bj.Add(dr.GetName(ordinal), null);
                                        else if (dr.GetName(ordinal) == "SN")
                                            bj.Add("SN", dr[ordinal]);
                                        else if (dr.GetDataTypeName(ordinal) == "date")
                                            bj.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd"));
                                        else if (dr.GetDataTypeName(ordinal) == "money")
                                            bj.Add(dr.GetName(ordinal), float.Parse(dr[ordinal].ToString()).ToString("0.00"));
                                        else if (dr.GetDataTypeName(ordinal) == "datetime2")
                                            bj.Add(dr.GetName(ordinal), DateTime.Parse(dr[ordinal].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                        else
                                            bj.Add(dr.GetName(ordinal), dr[ordinal]);
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

        }//Select1

        public static Resp Insert(IPrincipal User,
            string BH = null, string XLMC = null, string ZS = null, string CFRQ = null,
            string ZT = null, string RS = null, string TS = null, string DFC = null, string XC = null,
            string[] BJMC = null, string[] MSJ = null, string[] FL = null)
        {
            Resp resp = new Resp();
            try
            {
                //////////////////////////////////////////////////////////////////////////
                // 表单验证
                //////////////////////////////////////////////////////////////////////////
                if (string.IsNullOrWhiteSpace(BH)) throw new Exception("[团队编号]不能为空");
                if (string.IsNullOrWhiteSpace(XLMC)) throw new Exception("[线路名称]不能为空");
                if (string.IsNullOrWhiteSpace(ZS)) throw new Exception("[住宿]不能为空");
                if (string.IsNullOrWhiteSpace(CFRQ)) throw new Exception("[出发日期]不能为空");
                try { DateTime.ParseExact(CFRQ, "yyyy-MM-dd", null); }
                catch (Exception) { throw new Exception(string.Format("[出发日期]格式不正确: {0}", CFRQ)); }
                if (string.IsNullOrWhiteSpace(ZT)) throw new Exception("[状态]不能为空");
                if (string.IsNullOrWhiteSpace(RS)) throw new Exception("[人数]不能为空");
                try { int.Parse(RS); }
                catch (Exception) { throw new Exception(string.Format("[人数]格式不正确: {0}", RS)); }
                if (string.IsNullOrWhiteSpace(TS)) throw new Exception("[天数]不能为空");
                try { int.Parse(TS); }
                catch (Exception) { throw new Exception(string.Format("[天数]格式不正确: {0}", TS)); }
                if (string.IsNullOrWhiteSpace(DFC)) throw new Exception("[单房差]不能为空");
                try { float.Parse(DFC); }
                catch (Exception) { throw new Exception(string.Format("[单房差]格式不正确: {0}", DFC)); }
                if (string.IsNullOrWhiteSpace(XC)) throw new Exception("[行程]不能为空");

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

                            cmd.CommandText = "INSERT INTO [DRP].[GN_TD](BH, XLMC, CFRQ, ZS, TS, DFC, RS, ZT, FBR, FBSJ) VALUES(@BH, @XLMC, @CFRQ, @ZS, @TS, @DFC, @RS, @ZT, @FBR, @FBSJ);SELECT  @@IDENTITY";
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
                            resp.message = cmd.ExecuteScalar().ToString();

                            cmd.CommandText = "INSERT INTO [DRP].[GN_TD_FJ](ID, XC) VALUES(@ID, @XC)";
                            cmd.Parameters.Clear();
                            Common.AddParameter(cmd, "@ID", DbType.String).Value = resp.message;
                            Common.AddParameter(cmd, "@XC", DbType.Binary).Value = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/pub/attachment/" + XC));
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

                                cmd.CommandText = "INSERT INTO [DRP].[GN_TD_BJ](ID,SN,BJMC,MSJ,FL) VALUES(@ID,@SN,@BJMC,@MSJ,@FL)";
                                cmd.Parameters.Clear();
                                Common.AddParameter(cmd, "@ID", DbType.String).Value = resp.message;
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
            }//try
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = ex.Message;
                resp.total = 0;
                resp.items.Clear();
            }
            return resp;

        }//Insert

        public static Resp Copy(IPrincipal User, string ID)
        {
            Resp resp = new Resp();
            try
            {
                //////////////////////////////////////////////////////////////////////////
                // 表单验证
                //////////////////////////////////////////////////////////////////////////
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
                            cmd.CommandText = "INSERT INTO [DRP].[GN_TD](BH, XLMC, CFRQ, ZS, TS, DFC, RS, ZT, FBR, FBSJ) SELECT BH, XLMC, CFRQ, ZS, TS, DFC, RS, ZT, @FBR, @FBSJ FROM [DRP].[GN_TD] WHERE ID=@ID;SELECT  @@IDENTITY";
                            Common.AddParameter(cmd, "@ID", DbType.Int32).Value = ID;
                            Common.AddParameter(cmd, "@FBR", DbType.Int32).Value = User.Identity.Name;
                            Common.AddParameter(cmd, "@FBSJ", DbType.String).Value = DateTime.Now;
                            resp.message = cmd.ExecuteScalar().ToString();
                        }//DbCommand
                        using (DbCommand cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = ta;
                            cmd.CommandText = "INSERT INTO [DRP].[GN_TD_BJ](ID, SN, BJMC, MSJ, FL) SELECT @ID1, SN, BJMC, MSJ, FL FROM [DRP].[GN_TD_BJ] WHERE ID=@ID2";
                            Common.AddParameter(cmd, "@ID1", DbType.Int32).Value = resp.message;
                            Common.AddParameter(cmd, "@ID2", DbType.Int32).Value = ID;
                            cmd.ExecuteNonQuery();
                        }//DbCommand
                        using (DbCommand cmd = cn.CreateCommand())
                        {
                            cmd.Transaction = ta;
                            cmd.CommandText = "INSERT INTO [DRP].[GN_TD_FJ](ID, XC) SELECT @ID1, XC FROM [DRP].[GN_TD_FJ] WHERE ID=@ID2";
                            Common.AddParameter(cmd, "@ID1", DbType.Int32).Value = resp.message;
                            Common.AddParameter(cmd, "@ID2", DbType.Int32).Value = ID;
                            cmd.ExecuteNonQuery();
                        }//DbCommand
                        ta.Commit();
                    }//DbTransaction
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
        }//Copy

        public static Resp Update(IPrincipal User, string ID,
            string BH = null, string XLMC = null, string ZS = null, string CFRQ = null,
            string ZT = null, string RS = null, string TS = null, string DFC = null, string XC = null,
            string[] BJMC = null, string[] MSJ = null, string[] FL = null)
        {
            Resp resp = new Resp();
            try
            {
                //////////////////////////////////////////////////////////////////////////
                // 表单验证
                //////////////////////////////////////////////////////////////////////////
                if (string.IsNullOrWhiteSpace(ID)) throw new Exception("[主键]不能为空");

                if (string.IsNullOrWhiteSpace(BH)) throw new Exception("[团队编号]不能为空");
                if (string.IsNullOrWhiteSpace(XLMC)) throw new Exception("[线路名称]不能为空");
                if (string.IsNullOrWhiteSpace(ZS)) throw new Exception("[住宿]不能为空");
                if (string.IsNullOrWhiteSpace(CFRQ)) throw new Exception("[出发日期]不能为空");
                try { DateTime.ParseExact(CFRQ, "yyyy-MM-dd", null); }
                catch (Exception) { throw new Exception(string.Format("[出发日期]格式不正确: {0}", CFRQ)); }
                if (string.IsNullOrWhiteSpace(ZT)) throw new Exception("[状态]不能为空");
                if (string.IsNullOrWhiteSpace(RS)) throw new Exception("[人数]不能为空");
                try { int.Parse(RS); }
                catch (Exception) { throw new Exception(string.Format("[人数]格式不正确: {0}", RS)); }
                if (string.IsNullOrWhiteSpace(TS)) throw new Exception("[天数]不能为空");
                try { int.Parse(TS); }
                catch (Exception) { throw new Exception(string.Format("[天数]格式不正确: {0}", TS)); }
                if (string.IsNullOrWhiteSpace(DFC)) throw new Exception("[单房差]不能为空");
                try { float.Parse(DFC); }
                catch (Exception) { throw new Exception(string.Format("[单房差]格式不正确: {0}", DFC)); }
                //if (string.IsNullOrWhiteSpace(XC)) throw new Exception("[行程]不能为空");

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

                            cmd.CommandText = "UPDATE [DRP].[GN_TD] SET BH=@BH, XLMC=@XLMC, CFRQ=@CFRQ, ZS=@ZS, TS=@TS, DFC=@DFC, RS=@RS, ZT=@ZT WHERE ID=@ID";
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
                            resp.message = ID.Trim();

                            if (!string.IsNullOrWhiteSpace(XC))
                            {
                                cmd.CommandText = "UPDATE [DRP].[GN_TD_FJ] SET XC=@XC WHERE ID=@ID";
                                cmd.Parameters.Clear();
                                Common.AddParameter(cmd, "@ID", DbType.String).Value = ID.Trim();
                                Common.AddParameter(cmd, "@XC", DbType.Binary).Value = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/pub/attachment/" + XC));
                                cmd.ExecuteNonQuery();
                            }

                            cmd.CommandText = "DELETE FROM [DRP].[GN_TD_BJ] WHERE ID=@ID";
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

                                cmd.CommandText = "INSERT INTO [DRP].[GN_TD_BJ](ID,SN,BJMC,MSJ,FL) VALUES(@ID,@SN,@BJMC,@MSJ,@FL)";
                                cmd.Parameters.Clear();
                                Common.AddParameter(cmd, "@ID", DbType.String).Value = resp.message;
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
            }
            catch (Exception ex)
            {
                resp.success = false;
                resp.message = ex.Message;
                resp.total = 0;
                resp.items.Clear();
            }
            return resp;
        }//Update
    }
}
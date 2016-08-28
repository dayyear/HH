using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;

namespace Twn
{
    public static class Customer
    {
        public static List<Dictionary<string, object>> Select(string ID = null, string XM = null, string LXFS = null, string CSRQ = null, string XB = null, string YYBZRQ = null, string HZD = null, string BZJD = null, string ZJHM = null, string TDID = null, string YWY = null, string ZJZL = null, string LRSJ1 = null, string LRSJ2 = null, string rows = null, string page = null, string order = null)
        {
            int total;
            return Select(out total, ID, XM, LXFS, CSRQ, XB, YYBZRQ, HZD, BZJD, ZJHM, TDID, YWY, ZJZL, LRSJ1, LRSJ2, rows, page, order);
        } //Select

        public static List<Dictionary<string, object>> Select(out int total, string ID = null, string XM = null, string LXFS = null, string CSRQ = null, string XB = null, string YYBZRQ = null, string HZD = null, string BZJD = null, string ZJHM = null, string TDID = null, string YWY = null, string ZJZL = null, string LRSJ1 = null, string LRSJ2 = null, string rows = null, string page = null, string order = null)
        {
            var items = new List<Dictionary<string, object>>();

            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString))
            {
                cn.Open();
                using (var cmdCount = cn.CreateCommand())
                using (var cmdItems = cn.CreateCommand())
                {
                    var whereList = new List<string>() { "1=1" };

                    #region 参数验证
                    if (ID != null)
                    {
                        ID = ID.Trim();
                        if (ID.Length > 0)
                        {
                            whereList.Add(@"ID=@ID");
                            cmdCount.Parameters.Add("@ID", SqlDbType.BigInt).Value = ID;
                            cmdItems.Parameters.Add("@ID", SqlDbType.BigInt).Value = ID;
                        }
                    }
                    if (LXFS != null)
                    {
                        LXFS = LXFS.Trim();
                        if (LXFS.Length > 0)
                        {
                            whereList.Add(@"LXFS=@LXFS");
                            cmdCount.Parameters.Add("@LXFS", SqlDbType.VarChar).Value = LXFS;
                            cmdItems.Parameters.Add("@LXFS", SqlDbType.VarChar).Value = LXFS;
                        }
                    }
                    if (CSRQ != null)
                    {
                        CSRQ = CSRQ.Trim();
                        if (CSRQ.Length > 0)
                        {
                            whereList.Add(@"CSRQ=@CSRQ");
                            cmdCount.Parameters.Add("@CSRQ", SqlDbType.Date).Value = CSRQ;
                            cmdItems.Parameters.Add("@CSRQ", SqlDbType.Date).Value = CSRQ;
                        }
                    }
                    if (XB != null)
                    {
                        XB = XB.Trim();
                        if (XB.Length > 0)
                        {
                            whereList.Add(@"XB=@XB");
                            cmdCount.Parameters.Add("@XB", SqlDbType.Int).Value = XB;
                            cmdItems.Parameters.Add("@XB", SqlDbType.Int).Value = XB;
                        }
                    }
                    if (YYBZRQ != null)
                    {
                        YYBZRQ = YYBZRQ.Trim();
                        if (YYBZRQ.Length > 0)
                        {
                            whereList.Add(@"YYBZRQ=@YYBZRQ");
                            cmdCount.Parameters.Add("@YYBZRQ", SqlDbType.Date).Value = YYBZRQ;
                            cmdItems.Parameters.Add("@YYBZRQ", SqlDbType.Date).Value = YYBZRQ;
                        }
                    }
                    if (HZD != null)
                    {
                        HZD = HZD.Trim();
                        if (HZD.Length > 0)
                        {
                            whereList.Add(@"HZD=@HZD");
                            cmdCount.Parameters.Add("@HZD", SqlDbType.VarChar).Value = HZD;
                            cmdItems.Parameters.Add("@HZD", SqlDbType.VarChar).Value = HZD;
                        }
                    }
                    if (BZJD != null)
                    {
                        BZJD = BZJD.Trim();
                        if (BZJD.Length > 0)
                        {
                            whereList.Add(@"BZJD=@BZJD");
                            cmdCount.Parameters.Add("@BZJD", SqlDbType.VarChar).Value = BZJD;
                            cmdItems.Parameters.Add("@BZJD", SqlDbType.VarChar).Value = BZJD;
                        }
                    }
                    if (ZJHM != null)
                    {
                        ZJHM = ZJHM.Trim();
                        if (ZJHM.Length > 0)
                        {
                            whereList.Add(@"ZJHM=@ZJHM");
                            cmdCount.Parameters.Add("@ZJHM", SqlDbType.VarChar).Value = ZJHM;
                            cmdItems.Parameters.Add("@ZJHM", SqlDbType.VarChar).Value = ZJHM;
                        }
                    }
                    if (TDID != null)
                    {
                        TDID = TDID.Trim();
                        if (TDID.Length > 0)
                        {
                            whereList.Add(@"TDID=@TDID");
                            cmdCount.Parameters.Add("@TDID", SqlDbType.VarChar).Value = TDID;
                            cmdItems.Parameters.Add("@TDID", SqlDbType.VarChar).Value = TDID;
                        }
                    }
                    if (YWY != null)
                    {
                        YWY = YWY.Trim();
                        if (YWY.Length > 0)
                        {
                            whereList.Add(@"YWY=@YWY");
                            cmdCount.Parameters.Add("@YWY", SqlDbType.Int).Value = YWY;
                            cmdItems.Parameters.Add("@YWY", SqlDbType.Int).Value = YWY;
                        }
                    }
                    if (ZJZL != null)
                    {
                        ZJZL = ZJZL.Trim();
                        if (ZJZL.Length > 0)
                        {
                            whereList.Add(@"ZJZL=@ZJZL");
                            cmdCount.Parameters.Add("@ZJZL", SqlDbType.VarChar).Value = ZJZL;
                            cmdItems.Parameters.Add("@ZJZL", SqlDbType.VarChar).Value = ZJZL;
                        }
                    }
                    if (LRSJ1 != null)
                    {
                        LRSJ1 = LRSJ1.Trim();
                        if (LRSJ1.Length > 0)
                        {
                            whereList.Add(@"LRSJ>=@LRSJ1");
                            cmdCount.Parameters.Add("@LRSJ1", SqlDbType.DateTime2).Value = LRSJ1;
                            cmdItems.Parameters.Add("@LRSJ1", SqlDbType.DateTime2).Value = LRSJ1;
                        }
                    }
                    if (LRSJ2 != null)
                    {
                        LRSJ2 = LRSJ2.Trim();
                        if (LRSJ2.Length > 0)
                        {
                            whereList.Add(@"LRSJ<=@LRSJ2");
                            cmdCount.Parameters.Add("@LRSJ2", SqlDbType.DateTime2).Value = LRSJ2;
                            cmdItems.Parameters.Add("@LRSJ2", SqlDbType.DateTime2).Value = LRSJ2;
                        }
                    }
                    if (XM != null)
                    {
                        XM = XM.Trim();
                        if (XM.Length > 0)
                        {
                            whereList.Add(@"XM LIKE @XM");
                            cmdCount.Parameters.Add("@XM", SqlDbType.VarChar).Value = "%" + XM + "%";
                            cmdItems.Parameters.Add("@XM", SqlDbType.VarChar).Value = "%" + XM + "%";
                        }
                    }
                    #endregion

                    #region 分页验证
                    Common.ValidInt(rows, CanNull.True, CanEmpty.False, "每页行数rows", 1);
                    rows = rows ?? "10";
                    Common.ValidInt(page, CanNull.True, CanEmpty.False, "当前页page", 1);
                    page = page ?? "1";
                    cmdItems.Parameters.Add("@RowNumber2", SqlDbType.Int).Value = int.Parse(rows) * int.Parse(page);
                    cmdItems.Parameters.Add("@RowNumber1", SqlDbType.Int).Value = int.Parse(rows) * (int.Parse(page) - 1);
                    #endregion

                    #region 排序
                    order = order ?? "LRSJ DESC";
                    #endregion

                    #region 执行SQL

                    cmdCount.CommandText = string.Format(@"SELECT COUNT(*) FROM [TWN].[Customer] WHERE {0}",
                        string.Join(" AND ", whereList));
                    total = Convert.ToInt32(cmdCount.ExecuteScalar());

                    cmdItems.CommandText = string.Format(@"SELECT A.*,
                        ISNULL((SELECT MC FROM [ADMIN].[DM] WHERE LX='XB' AND DM=A.XB), A.XB) XB1,
                        ISNULL((SELECT username FROM [ADMIN].[User] WHERE ID=A.YWY), A.YWY) YWY1,
                        ISNULL((SELECT MC FROM [ADMIN].[DM] WHERE LX='ZJZL' AND DM=A.ZJZL), A.ZJZL) ZJZL1,
                        ISNULL((SELECT MC FROM [ADMIN].[DM] WHERE LX='GJDQ' AND DM=A.QFD), A.QFD) QFD1,
                        ISNULL((SELECT MC FROM [ADMIN].[DM] WHERE LX='GJDQ' AND DM=A.CSD), A.CSD) CSD1,
                        ISNULL((SELECT MC FROM [ADMIN].[DM] WHERE LX='CJJL' AND DM=A.CJJL), A.CJJL) CJJL1,
                        ISNULL((SELECT MC FROM [ADMIN].[DM] WHERE LX='HYZK' AND DM=A.HYZK), A.HYZK) HYZK1,
                        ISNULL((SELECT MC FROM [ADMIN].[DM] WHERE LX='SFQR' AND DM=A.SFQR), A.SFQR) SFQR1
                        FROM (
                            SELECT *, ROW_NUMBER () OVER (ORDER BY {1}) AS RowNumber FROM [TWN].[Customer] WHERE {0}
                        ) AS A WHERE RowNumber > @RowNumber1 AND RowNumber <= @RowNumber2
                        ORDER BY RowNumber", string.Join(" AND ", whereList), order);
                    using (var dr = cmdItems.ExecuteReader())
                        while (dr.Read())
                        {
                            var item = new Dictionary<string, object>();
                            items.Add(item);
                            for (var i = 0; i < dr.FieldCount; i++)
                                if (dr.IsDBNull(i))
                                    item.Add(dr.GetName(i), string.Empty);
                                else
                                    switch (dr.GetName(i))
                                    {
                                        case "CSRQ":
                                        case "YYBZRQ":
                                        case "QFRQ":
                                        case "YXQZ":
                                            item.Add(dr.GetName(i), dr.GetDateTime(i).ToString("yyyy-MM-dd"));
                                            break;
                                        case "LRSJ":
                                        case "CXSJ":
                                            item.Add(dr.GetName(i), dr.GetDateTime(i).ToString("yyyy-MM-dd HH:mm"));
                                            break;
                                        default:
                                            item.Add(dr.GetName(i), dr.GetValue(i));
                                            break;
                                    } //switch
                        } //dr
                    #endregion
                } //cmd
            } //cn
            return items;
        } //Select

        public static int Insert(string TDID, string ZJZL, string XM, string XB, string CSRQ, string LXFS, string YYBZRQ, IPrincipal User)
        {
            var C = 0;
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString))
            {
                cn.Open();
                using (var ta = cn.BeginTransaction())
                {
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = ta;
                        var fieldList = new List<string>();
                        var valueList = new List<string>();

                        #region 参数验证
                        Common.ValidString(TDID, CanNull.False, CanEmpty.False, "团队编号");
                        fieldList.Add("TDID");
                        valueList.Add("@TDID");
                        //Common.AddParameter(cmd, "@TDID", TDID);
                        cmd.Parameters.Add("@TDID", SqlDbType.VarChar).Value = Common.HandDBNull(TDID);

                        // 检查团队人数是否已满
                        var items = ADMIN.DM.Select("PARAMETER", "TDZDRS");
                        if (!items.Any())
                            throw new Exception("未找到参数[最大团队人数TDZDRS]");
                        var TDZDRS = Convert.ToInt32(items.First()["MC"]);
                        items = Select(TDID: TDID, rows: "1000");
                        if (items.Count >= TDZDRS)
                            throw new Exception(string.Format("团队人数已满，最大人数为[{0}]", TDZDRS));

                        Common.ValidCode(ZJZL, CanNull.False, CanEmpty.False, "证件种类", ADMIN.DM.Select("ZJZL").Select(x => x["DM"].ToString()));
                        fieldList.Add("ZJZL");
                        valueList.Add("@ZJZL");
                        Common.AddParameter(cmd, "@ZJZL", ZJZL);

                        Common.ValidString(XM, CanNull.False, CanEmpty.True, "姓名");
                        fieldList.Add("XM");
                        valueList.Add("@XM");
                        Common.AddParameter(cmd, "@XM", XM);

                        Common.ValidCode(XB, CanNull.False, CanEmpty.True, "性别", ADMIN.DM.Select("XB").Select(x => x["DM"].ToString()));
                        fieldList.Add("XB");
                        valueList.Add("@XB");
                        Common.AddParameter(cmd, "@XB", XB);

                        Common.ValidDateTime(CSRQ, CanNull.False, CanEmpty.True, "yyyy-MM-dd", "出生日期");
                        fieldList.Add("CSRQ");
                        valueList.Add("@CSRQ");
                        Common.AddParameter(cmd, "@CSRQ", CSRQ);

                        Common.ValidString(LXFS, CanNull.False, CanEmpty.True, "联系方式");
                        fieldList.Add("LXFS");
                        valueList.Add("@LXFS");
                        Common.AddParameter(cmd, "@LXFS", LXFS);

                        Common.ValidDateTime(YYBZRQ, CanNull.False, CanEmpty.True, "yyyy-MM-dd", "预约办证日期");
                        fieldList.Add("YYBZRQ");
                        valueList.Add("@YYBZRQ");
                        Common.AddParameter(cmd, "@YYBZRQ", YYBZRQ);

                        fieldList.Add("YWY");
                        valueList.Add("@YWY");
                        Common.AddParameter(cmd, "@YWY", User.Identity.Name);

                        fieldList.Add("LRSJ");
                        valueList.Add("@LRSJ");
                        Common.AddParameter(cmd, "@LRSJ", DateTime.Now);
                        #endregion

                        #region 执行SQL
                        cmd.CommandText = string.Format(@"INSERT INTO [TWN].[Customer]({0}) VALUES({1})", string.Join(",", fieldList), string.Join(",", valueList));
                        C += cmd.ExecuteNonQuery();
                        #endregion

                    } //cmd
                    ta.Commit();
                }//ta
            }//cn
            return C;
        }// Insert

    }//class
}//namespace

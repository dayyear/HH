using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace ADMIN
{
    public static class DM
    {

        public static List<Dictionary<string, object>> Select(string LX = null, string DM = null, string MC = null, string SFYX = null, string rows = null, string page = null, string order = null)
        {
            return SelectWithTotal(LX, DM, MC, SFYX, rows, page, order).Item1;
        } //Select

        public static Tuple<List<Dictionary<string, object>>, int> SelectWithTotal(string LX, string DM, string MC, string SFYX, string rows, string page, string order)
        {
            var items = new List<Dictionary<string, object>>();
            int total;

            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString))
            {
                cn.Open();
                using (var cmdCount = cn.CreateCommand())
                using (var cmdItems = cn.CreateCommand())
                {
                    var whereList = new List<string> { "1=1" };

                    #region 类型

                    if (LX != null)
                    {
                        LX = LX.Trim();
                        if (LX.Length == 0)
                            whereList.Add("[LX] IS NULL");
                        else
                        {
                            whereList.Add("[LX]=@LX");
                            Common.AddSqlParameter(cmdCount, "@LX", SqlDbType.VarChar, LX);
                            Common.AddSqlParameter(cmdItems, "@LX", SqlDbType.VarChar, LX);
                        }
                    }

                    #endregion

                    #region 代码

                    if (DM != null)
                    {
                        DM = DM.Trim();
                        if (DM.Length == 0)
                            whereList.Add("[DM] IS NULL");
                        else
                        {
                            whereList.Add("[DM]=@DM");
                            Common.AddSqlParameter(cmdCount, "@DM", SqlDbType.VarChar, DM);
                            Common.AddSqlParameter(cmdItems, "@DM", SqlDbType.VarChar, DM);
                        }
                    }

                    #endregion

                    #region 名称

                    if (MC != null)
                    {
                        MC = MC.Trim();
                        if (MC.Length == 0)
                            whereList.Add("[MC] IS NULL");
                        else
                        {
                            whereList.Add("[MC] LIKE @MC");
                            Common.AddSqlParameter(cmdCount, "@MC", SqlDbType.VarChar, "%" + MC + "%");
                            Common.AddSqlParameter(cmdItems, "@MC", SqlDbType.VarChar, "%" + MC + "%");
                        }
                    }

                    #endregion

                    #region 是否有效

                    if (SFYX != null)
                    {
                        SFYX = SFYX.Trim();
                        if (SFYX.Length == 0)
                            whereList.Add("[SFYX] IS NULL");
                        else
                        {
                            whereList.Add("[SFYX]=@SFYX");
                            Common.AddSqlParameter(cmdCount, "@SFYX", SqlDbType.VarChar, SFYX);
                            Common.AddSqlParameter(cmdItems, "@SFYX", SqlDbType.VarChar, SFYX);
                        }
                    }

                    #endregion

                    #region 分页

                    int rowsInt, pageInt;

                    if (string.IsNullOrWhiteSpace(rows))
                        rowsInt = int.MaxValue;
                    else if (!int.TryParse(rows, out rowsInt))
                        throw new ArgumentException("无效的[每页行数]", "rows");

                    if (string.IsNullOrWhiteSpace(page))
                        pageInt = 1;
                    else if (!int.TryParse(page, out pageInt))
                        throw new ArgumentException("无效的[当前页]", "page");

                    Common.AddSqlParameter(cmdItems, "@RowNumber2", SqlDbType.Int, rowsInt * pageInt);
                    Common.AddSqlParameter(cmdItems, "@RowNumber1", SqlDbType.Int, rowsInt * (pageInt - 1));

                    #endregion

                    #region 排序

                    if (string.IsNullOrWhiteSpace(order))
                        order = "[LX], [ORDER]";

                    #endregion

                    #region 执行SQL

                    cmdCount.CommandText = string.Format(@"SELECT COUNT(*) FROM [ADMIN].[DM] WHERE {0}", string.Join(" AND ", whereList));
                    total = Convert.ToInt32(cmdCount.ExecuteScalar());

                    cmdItems.CommandText = string.Format(@"SELECT A.*,
                        ISNULL((SELECT MC FROM [ADMIN].[DM] WHERE LX='_LX' AND DM=A.LX), A.LX) LX1,
                        ISNULL((SELECT MC FROM [ADMIN].[DM] WHERE LX='SFYX' AND DM=A.SFYX), A.SFYX) SFYX1
                        FROM (
                            SELECT *, ROW_NUMBER () OVER (ORDER BY {0}) AS RowNumber FROM [ADMIN].[DM] WHERE {1}
                        ) AS A WHERE RowNumber > @RowNumber1 AND RowNumber <= @RowNumber2
                        ORDER BY RowNumber", order, string.Join(" AND ", whereList));
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
                                        case "BDSJ":
                                            item.Add(dr.GetName(i), dr.GetDateTime(i).ToString("yyyy-MM-dd HH:mm:ss"));
                                            break;
                                        default:
                                            item.Add(dr.GetName(i), dr.GetValue(i));
                                            break;
                                    } //switch
                        } //dr

                    #endregion
                } //cmd
            } //cn
            return Tuple.Create(items, total);
        } //Select

        public static int Insert(string LX, string DM, string MC, string SFYX, string BZ, string ORDER)
        {
            var c = 0;
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

                        #region 类型

                        if (LX == null)
                            throw new ArgumentNullException("LX", "[类型]不能为null");
                        LX = LX.Trim();
                        if (LX.Length == 0)
                            throw new ArgumentNullException("LX", "[类型]不能为空");
                        fieldList.Add("[LX]");
                        valueList.Add("@LX");
                        Common.AddSqlParameter(cmd, "@LX", SqlDbType.VarChar, LX);

                        #endregion

                        #region 代码

                        if (DM == null)
                            throw new ArgumentNullException("DM", "[代码]不能为null");
                        DM = DM.Trim();
                        if (DM.Length == 0)
                            throw new ArgumentNullException("DM", "[代码]不能为空");
                        fieldList.Add("[DM]");
                        valueList.Add("@DM");
                        Common.AddSqlParameter(cmd, "@DM", SqlDbType.VarChar, DM);

                        #endregion

                        #region UNIQUE

                        if (Select(LX: LX, DM: DM).Any())
                            throw new ArgumentException("重复的[类型,代码]: " + LX + "," + DM);

                        #endregion

                        #region 名称

                        if (MC == null)
                            throw new ArgumentNullException("MC", "[名称]不能为null");
                        MC = MC.Trim();
                        if (MC.Length == 0)
                            throw new ArgumentNullException("MC", "[名称]不能为空");
                        fieldList.Add("[MC]");
                        valueList.Add("@MC");
                        Common.AddSqlParameter(cmd, "@MC", SqlDbType.VarChar, MC);

                        #endregion

                        #region 是否有效

                        if (SFYX == null)
                            throw new ArgumentNullException("SFYX", "[是否有效]不能为null");
                        SFYX = SFYX.Trim();
                        if (SFYX.Length == 0)
                            throw new ArgumentNullException("SFYX", "[是否有效]不能为空");
                        if (!ADMIN.DM.Select(LX: "SFYX", DM: SFYX).Any())
                            throw new ArgumentException("无效的[是否有效]: " + SFYX, "SFYX");
                        fieldList.Add("[SFYX]");
                        valueList.Add("@SFYX");
                        Common.AddSqlParameter(cmd, "@SFYX", SqlDbType.VarChar, SFYX);

                        #endregion

                        #region 备注

                        if (BZ == null)
                            throw new ArgumentNullException("BZ", "[备注]不能为null");
                        BZ = BZ.Trim();
                        fieldList.Add("[BZ]");
                        valueList.Add("@BZ");
                        Common.AddSqlParameter(cmd, "@BZ", SqlDbType.VarChar, BZ);

                        #endregion

                        #region 排序

                        if (ORDER == null)
                            throw new ArgumentNullException("ORDER", "[排序]不能为null");
                        ORDER = ORDER.Trim();
                        if (ORDER.Length == 0)
                            throw new ArgumentNullException("ORDER", "[排序]不能为空");
                        fieldList.Add("[ORDER]");
                        valueList.Add("@ORDER");
                        Common.AddSqlParameter(cmd, "@ORDER", SqlDbType.VarChar, ORDER);

                        #endregion

                        #region 变动时间

                        fieldList.Add("[BDSJ]");
                        valueList.Add("SYSDATETIME()");

                        #endregion

                        #region 执行SQL

                        cmd.CommandText = string.Format(@"INSERT INTO [ADMIN].[DM]({0}) VALUES({1})", string.Join(",", fieldList), string.Join(",", valueList));
                        c += cmd.ExecuteNonQuery();

                        #endregion
                    } //cmd
                    ta.Commit();
                } //ta
            } //cn
            return c;
        } //Insert

        public static int Update(string LX, string DM, string MC, string SFYX, string BZ, string ORDER)
        {
            var c = 0;
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString))
            {
                cn.Open();
                using (var ta = cn.BeginTransaction())
                {
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = ta;
                        var setList = new List<string>();
                        var whereList = new List<string>();

                        #region 名称

                        if (MC != null)
                        {
                            MC = MC.Trim();
                            if (MC.Length == 0)
                                throw new ArgumentNullException("MC", "[名称]不能为空");
                            setList.Add("[MC]=@MC");
                            Common.AddSqlParameter(cmd, "@MC", SqlDbType.VarChar, MC);
                        }

                        #endregion

                        #region 是否有效

                        if (SFYX != null)
                        {
                            SFYX = SFYX.Trim();
                            if (SFYX.Length == 0)
                                throw new ArgumentNullException("SFYX", "[是否有效]不能为空");
                            if (!ADMIN.DM.Select(LX: "SFYX", DM: SFYX).Any())
                                throw new ArgumentException("无效的[是否有效]: " + SFYX, "SFYX");
                            setList.Add("[SFYX]=@SFYX");
                            Common.AddSqlParameter(cmd, "@SFYX", SqlDbType.VarChar, SFYX);
                        }

                        #endregion

                        #region 备注

                        if (BZ != null)
                        {
                            BZ = BZ.Trim();
                            setList.Add("[BZ]=@BZ");
                            Common.AddSqlParameter(cmd, "@BZ", SqlDbType.VarChar, BZ);
                        }

                        #endregion

                        #region 排序

                        if (ORDER != null)
                        {
                            ORDER = ORDER.Trim();
                            if (ORDER.Length == 0)
                                throw new ArgumentNullException("ORDER", "[排序]不能为空");
                            setList.Add("[ORDER]=@ORDER");
                            Common.AddSqlParameter(cmd, "@ORDER", SqlDbType.VarChar, ORDER);
                        }

                        #endregion

                        #region 变动时间

                        setList.Add("[BDSJ]=SYSDATETIME()");

                        #endregion

                        #region 类型

                        if (LX == null)
                            throw new ArgumentNullException("LX", "[类型]不能为null");
                        LX = LX.Trim();
                        if (LX.Length == 0)
                            throw new ArgumentNullException("LX", "[类型]不能为空");
                        whereList.Add("[LX]=@LX");
                        Common.AddSqlParameter(cmd, "@LX", SqlDbType.VarChar, LX);

                        #endregion

                        #region 代码

                        if (DM == null)
                            throw new ArgumentNullException("DM", "[代码]不能为null");
                        DM = DM.Trim();
                        if (DM.Length == 0)
                            throw new ArgumentNullException("DM", "[代码]不能为空");
                        whereList.Add("[DM]=@DM");
                        Common.AddSqlParameter(cmd, "@DM", SqlDbType.VarChar, DM);

                        #endregion

                        #region VALIDITY

                        if (!Select(LX: LX, DM: DM).Any())
                            throw new ArgumentException("无效的[类型,代码]: " + LX + "," + DM);

                        #endregion

                        #region 执行SQL

                        cmd.CommandText = string.Format(@"UPDATE [ADMIN].[DM] SET {0} WHERE {1}", string.Join(",", setList), string.Join(" AND ", whereList));
                        c += cmd.ExecuteNonQuery();

                        #endregion
                    } //cmd
                    ta.Commit();
                } //ta
            } //cn
            return c;
        } //Update

        public static int Delete(string LX, string DM)
        {
            var c = 0;
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString))
            {
                cn.Open();
                using (var ta = cn.BeginTransaction())
                {
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = ta;
                        var whereList = new List<string>();

                        #region 类型

                        if (LX == null)
                            throw new ArgumentNullException("LX", "[类型]不能为null");
                        LX = LX.Trim();
                        if (LX.Length == 0)
                            throw new ArgumentNullException("LX", "[类型]不能为空");
                        whereList.Add("[LX]=@LX");
                        Common.AddSqlParameter(cmd, "@LX", SqlDbType.VarChar, LX);

                        #endregion

                        #region 代码

                        if (DM == null)
                            throw new ArgumentNullException("DM", "[代码]不能为null");
                        DM = DM.Trim();
                        if (DM.Length == 0)
                            throw new ArgumentNullException("DM", "[代码]不能为空");
                        whereList.Add("[DM]=@DM");
                        Common.AddSqlParameter(cmd, "@DM", SqlDbType.VarChar, DM);

                        #endregion

                        #region VALIDITY

                        if (!Select(LX: LX, DM: DM).Any())
                            throw new ArgumentException("无效的[类型,代码]: " + LX + "," + DM);

                        #endregion

                        #region 执行SQL

                        cmd.CommandText = string.Format(@"DELETE FROM [ADMIN].[DM] WHERE {0}", string.Join(" AND ", whereList));
                        c += cmd.ExecuteNonQuery();

                        #endregion
                    } //cmd
                    ta.Commit();
                } //ta
            } //cn
            return c;
        } //Delete

        public static string Option(string LX, string SFYX, string selectedDM)
        {
            var sb = new StringBuilder();

            //////////////////////////////////////////////////////////////////////////
            // 表单验证
            //////////////////////////////////////////////////////////////////////////
            StringBuilder where = new StringBuilder();

            if (string.IsNullOrWhiteSpace(LX))
                throw new Exception("[类型LX]不能为空");
            where.AppendFormat(" AND LX=@LX");
            if (!string.IsNullOrWhiteSpace(SFYX))
                where.AppendFormat(" AND SFYX=@SFYX");

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
                    cmd.CommandText = string.Format(@"SELECT * FROM [ADMIN].[DM] WHERE 1=1 {0}", where);
                    cmd.Parameters.Clear();
                    Common.AddParameter(cmd, "@LX", DbType.String).Value = LX;
                    if (!string.IsNullOrWhiteSpace(SFYX))
                        Common.AddParameter(cmd, "@SFYX", DbType.String).Value = SFYX;
                    using (DbDataReader dr = cmd.ExecuteReader())
                        while (dr.Read())
                            sb.AppendFormat("<option value='{0}' {1}>{2}</option>",
                                HttpUtility.HtmlEncode(dr["DM"].ToString()),
                                dr["DM"].ToString() == selectedDM ? "selected='selected'" : "",
                                HttpUtility.HtmlEncode(dr["MC"].ToString()));
                } //DbCommand
            } //DbConnection



            return sb.ToString();
        } //Option
    } //class
} //namespace
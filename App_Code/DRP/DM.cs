using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Data;

namespace DRP
{
    /// <summary>
    /// DM 的摘要说明
    /// </summary>
    public class DM
    {
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
                    cmd.CommandText = string.Format(@"SELECT * FROM [DRP].[DM] WHERE 1=1 {0}", where);
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
                }//DbCommand
            }//DbConnection



            return sb.ToString();
        }
    }//class
}//namespace
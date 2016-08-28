
using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;

namespace Twn
{
    public static class CustomerImage
    {
        public static byte[] Select(string ID, string field)
        {
            byte[] bytes;
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString))
            {
                cn.Open();
                using (var cmd = cn.CreateCommand())
                {
                    Common.ValidInt(ID, CanNull.False, CanEmpty.False, "ID", 1);
                    Common.ValidString(field, CanNull.False, CanEmpty.False, "field");

                    cmd.CommandText = string.Format(@"SELECT {0} FROM [TWN].[CustomerImage] WHERE ID=@ID", field);
                    Common.AddParameter(cmd, "@ID", ID);
                    using (DbDataReader dr = cmd.ExecuteReader())
                        if (!dr.Read())
                            bytes = null;
                        else if (dr.IsDBNull(0))
                            bytes = null;
                        else
                            bytes = (byte[])dr[0];
                }//cmd
            }//cn
            return bytes;
        }//Select
    }//class
}//namespace

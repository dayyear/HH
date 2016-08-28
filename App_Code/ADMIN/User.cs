using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ADMIN
{
    public static class User
    {
        public static List<Dictionary<string, object>> Select(string ID = null, string loginname = null, string username = null, string roleID = null, string sfyx = null, string rows = null, string page = null, string order = null)
        {
            int total;
            return Select(out total, ID, loginname, username, roleID, sfyx, rows, page, order);
        } //Select

        public static List<Dictionary<string, object>> Select(out int total, string ID, string loginname, string username, string roleID, string sfyx, string rows, string page, string order)
        {
            var items = new List<Dictionary<string, object>>();

            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString))
            {
                cn.Open();
                using (var cmdCount = cn.CreateCommand())
                using (var cmdItems = cn.CreateCommand())
                {
                    var whereList = new List<string> { "1=1" };

                    #region 参数验证
                    ID = Common.ValidInt(ID, CanNull.True, CanEmpty.False, "主键ID", 1);
                    if (ID != null)
                    {
                        whereList.Add("[ID]=@ID");
                        cmdCount.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                        cmdItems.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                    }
                    loginname = Common.ValidString(loginname, CanNull.True, CanEmpty.False, "登录名称loginname");
                    if (loginname != null)
                    {
                        whereList.Add("UPPER([loginname])=UPPER(@loginname)");
                        cmdCount.Parameters.Add("@loginname", SqlDbType.VarChar).Value = loginname;
                        cmdItems.Parameters.Add("@loginname", SqlDbType.VarChar).Value = loginname;
                    }
                    username = Common.ValidString(username, CanNull.True, CanEmpty.False, "用户名称username");
                    if (username != null)
                    {
                        whereList.Add("[username]=@username");
                        cmdCount.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                        cmdItems.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                    }
                    roleID = Common.ValidString(roleID, CanNull.True, CanEmpty.False, "用户角色roleID");
                    if (roleID != null)
                    {
                        whereList.Add("[roleID]=@roleID");
                        cmdCount.Parameters.Add("@roleID", SqlDbType.VarChar).Value = roleID;
                        cmdItems.Parameters.Add("@roleID", SqlDbType.VarChar).Value = roleID;
                    }
                    sfyx = Common.ValidString(sfyx, CanNull.True, CanEmpty.False, "是否有效sfyx");
                    if (sfyx != null)
                    {
                        whereList.Add("[sfyx]=@sfyx");
                        cmdCount.Parameters.Add("@sfyx", SqlDbType.VarChar).Value = sfyx;
                        cmdItems.Parameters.Add("@sfyx", SqlDbType.VarChar).Value = sfyx;
                    }
                    #endregion

                    #region 分页验证
                    rows = Common.ValidInt(rows, CanNull.True, CanEmpty.True, "每页行数rows", 1);
                    rows = string.IsNullOrWhiteSpace(rows) ? int.MaxValue.ToString() : rows;
                    page = Common.ValidInt(page, CanNull.True, CanEmpty.True, "当前页page", 1);
                    page = string.IsNullOrWhiteSpace(page) ? "1" : page;
                    cmdItems.Parameters.Add("@RowNumber2", SqlDbType.Int).Value = int.Parse(rows) * int.Parse(page);
                    cmdItems.Parameters.Add("@RowNumber1", SqlDbType.Int).Value = int.Parse(rows) * (int.Parse(page) - 1);
                    #endregion

                    #region 排序验证
                    order = Common.ValidString(order, CanNull.True, CanEmpty.True, "排序order");
                    order = string.IsNullOrWhiteSpace(order) ? "[ID] DESC" : order;
                    #endregion

                    #region 执行SQL
                    cmdCount.CommandText = string.Format(@"SELECT COUNT(*) FROM [ADMIN].[User] WHERE {0}", string.Join(" AND ", whereList));
                    total = Convert.ToInt32(cmdCount.ExecuteScalar());

                    cmdItems.CommandText = string.Format(@"SELECT A.*,
                        ISNULL((SELECT name FROM [ADMIN].[Role] WHERE ID=A.roleID), A.roleID) roleID1,
                        ISNULL((SELECT MC FROM [ADMIN].[DM] WHERE LX='SFYX' AND DM=A.sfyx), A.sfyx) sfyx1
                        FROM (
                            SELECT *, ROW_NUMBER () OVER (ORDER BY {0}) AS RowNumber FROM [ADMIN].[User] WHERE {1}
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
                                        case "password":
                                            break;
                                        case "bdsj":
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
            return items;
        } //Select

        public static int Insert(string loginname, string username, string password, string roleID, string sfyx)
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
                        loginname = Common.ValidString(loginname, CanNull.False, CanEmpty.False, "登录名称");
                        fieldList.Add("loginname");
                        valueList.Add("UPPER(@loginname)");
                        cmd.Parameters.Add("@loginname", SqlDbType.VarChar).Value = loginname;

                        if (Select(loginname: loginname).Any())
                            throw new ArgumentException("重复的[登录名称]: " + loginname);

                        username = Common.ValidString(username, CanNull.False, CanEmpty.False, "用户名称");
                        fieldList.Add("username");
                        valueList.Add("@username");
                        cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;

                        if (Select(username: username).Any())
                            throw new ArgumentException("重复的[用户名称]: " + username);

                        password = Common.ValidString(password, CanNull.False, CanEmpty.False, "密码");
                        fieldList.Add("password");
                        valueList.Add("@password");
                        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;

                        //roleID = Common.ValidCode(roleID, CanNull.False, CanEmpty.False, "用户角色", Role.Select().Select(x => x["ID"].ToString()));
                        roleID = Common.ValidCode(roleID, CanNull.False, CanEmpty.False, "用户角色", ADMIN.DM.Select("JS").Select(x => x["DM"].ToString()));
                        fieldList.Add("roleID");
                        valueList.Add("@roleID");
                        cmd.Parameters.Add("@roleID", SqlDbType.VarChar).Value = roleID;

                        sfyx = Common.ValidCode(sfyx, CanNull.False, CanEmpty.False, "是否有效", DM.Select("SFYX").Select(x => x["DM"].ToString()));
                        fieldList.Add("sfyx");
                        valueList.Add("@sfyx");
                        cmd.Parameters.Add("@sfyx", SqlDbType.VarChar).Value = sfyx;

                        fieldList.Add("bdsj");
                        valueList.Add("@bdsj");
                        cmd.Parameters.Add("@bdsj", SqlDbType.DateTime2).Value = DateTime.Now;
                        #endregion

                        #region 执行SQL
                        cmd.CommandText = string.Format(@"INSERT INTO [ADMIN].[User]({0}) VALUES({1})", string.Join(",", fieldList), string.Join(",", valueList));
                        C += cmd.ExecuteNonQuery();
                        #endregion

                    } //cmd
                    ta.Commit();
                }//ta
            }//cn
            return C;
        }//Insert

        public static int Update(string username, string roleID, string sfyx, string ID)
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
                        var setList = new List<string>();
                        var whereList = new List<string>();

                        #region 参数验证
                        ID = Common.ValidInt(ID, CanNull.False, CanEmpty.False, "主键", 1);
                        if (!Select(ID).Any())
                            throw new ArgumentException("无效的[主键]: " + ID);

                        username = Common.ValidString(username, CanNull.True, CanEmpty.False, "用户名称");
                        if (username != null)
                        {
                            if (Select(username: username).Any(x => !ID.Equals(x["ID"].ToString())))
                                throw new ArgumentException("重复的[用户名称]: " + username);
                            setList.Add(@"username=@username");
                            cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                        }

                        //roleID = Common.ValidCode(roleID, CanNull.True, CanEmpty.False, "用户角色", Role.Select().Select(x => x["ID"].ToString()));
                        roleID = Common.ValidCode(roleID, CanNull.True, CanEmpty.False, "用户角色", ADMIN.DM.Select("JS").Select(x => x["DM"].ToString()));
                        if (roleID != null)
                        {
                            setList.Add(@"roleID=@roleID");
                            cmd.Parameters.Add("@roleID", SqlDbType.VarChar).Value = roleID;
                        }

                        sfyx = Common.ValidCode(sfyx, CanNull.True, CanEmpty.False, "是否有效", DM.Select("SFYX").Select(x => x["DM"].ToString()));
                        if (sfyx != null)
                        {
                            setList.Add(@"sfyx=@sfyx");
                            cmd.Parameters.Add("@sfyx", SqlDbType.VarChar).Value = sfyx;
                        }

                        setList.Add(@"bdsj=@bdsj");
                        cmd.Parameters.Add("@bdsj", SqlDbType.DateTime2).Value = DateTime.Now;

                        whereList.Add(@"ID=@ID");
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                        #endregion

                        #region 执行SQL
                        cmd.CommandText = string.Format(@"UPDATE [ADMIN].[User] SET {0} WHERE {1}", string.Join(",", setList), string.Join(" AND ", whereList));
                        C += cmd.ExecuteNonQuery();
                        #endregion
                    } //cmd
                    ta.Commit();
                }//ta
            }//cn
            return C;
        }//Update

        public static int Reset(string ID)
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
                        var setList = new List<string>();
                        var whereList = new List<string>();

                        #region 参数验证
                        ID = Common.ValidInt(ID, CanNull.False, CanEmpty.False, "主键", 1);
                        if (!Select(ID).Any())
                            throw new ArgumentException("无效的[主键]: " + ID);

                        setList.Add(@"password=@password");
                        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = "111111";

                        setList.Add(@"bdsj=@bdsj");
                        cmd.Parameters.Add("@bdsj", SqlDbType.DateTime2).Value = DateTime.Now;

                        whereList.Add(@"ID=@ID");
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                        #endregion

                        #region 执行SQL
                        cmd.CommandText = string.Format(@"UPDATE [ADMIN].[User] SET {0} WHERE {1}", string.Join(",", setList), string.Join(" AND ", whereList));
                        C += cmd.ExecuteNonQuery();
                        #endregion

                    } //cmd
                    ta.Commit();
                }//ta
            }//cn
            return C;
        }//Reset

        public static int Delete(string ID)
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
                        var whereList = new List<string>();

                        #region 参数验证
                        ID = Common.ValidInt(ID, CanNull.False, CanEmpty.False, "主键", 1);
                        if (!Select(ID).Any())
                            throw new ArgumentException("无效的[主键]: " + ID);
                        whereList.Add("ID=@ID");
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                        #endregion

                        #region 执行SQL
                        cmd.CommandText = string.Format(@"DELETE FROM [ADMIN].[User] WHERE {0}", string.Join(" AND ", whereList));
                        C += cmd.ExecuteNonQuery();
                        #endregion
                    } //cmd
                    ta.Commit();
                }//ta
            }//cn
            return C;
        }//Delete

        public static void ChangePassword(string ID, string pwd1, string pwd2)
        {
            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString))
            {
                cn.Open();

                // 验证用户是否存在，密码是否正确
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT password FROM [ADMIN].[User] WHERE ID=@ID";
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) throw new ArgumentException("该用户不存在");
                        if ((string)dr[0] != pwd1) throw new ArgumentException("[旧密码]错误");
                    }//DbDataReader
                }//cmd

                // 修改密码
                using (var ta = cn.BeginTransaction())
                {
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.Transaction = ta;
                        cmd.CommandText = "UPDATE [ADMIN].[User] SET password=@password WHERE ID=@ID";
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = pwd2;
                        cmd.ExecuteNonQuery();
                    }//cmd
                    ta.Commit();
                }//ta
            }//cn
        }//ChangePassword
    }//class
}//namespace
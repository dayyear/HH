using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using System.Web.Security;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

public partial class public_login_login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;

        if (Request.IsAuthenticated && !string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
        {
            Response.Redirect("~/pub/login/403.aspx");
            return;
        }

        if (Request.HttpMethod == "GET")
        {
        }//GET
        else//if (Request.HttpMethod == "POST")
        {
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

            // 表单
            string loginname = Request["loginname"];
            string password = Request["password"];
            string rememberme = Request["rememberme"];

            try
            {
                //////////////////////////////////////////////////////////////////////////
                // 1. 表单验证
                //////////////////////////////////////////////////////////////////////////
                if (string.IsNullOrWhiteSpace(loginname))
                    throw new Exception("[登录名]不能为空");
                loginname = loginname.Trim();

                if (string.IsNullOrWhiteSpace(password))
                    throw new Exception("[密码]不能为空");

                //////////////////////////////////////////////////////////////////////////
                // 2. 用户验证
                //////////////////////////////////////////////////////////////////////////
                string userID = "";
                string roleID = "";
                List<string> userData = new List<string>();

                string providerName = ConfigurationManager.ConnectionStrings["HuiHuang"].ProviderName;
                string connectionString = ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString;
                DbProviderFactory df = DbProviderFactories.GetFactory(providerName);
                using (DbConnection cn = df.CreateConnection())
                {
                    cn.ConnectionString = connectionString;
                    cn.Open();
                    using (DbCommand cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "select ID,roleID from [ADMIN].[User] where loginname=@loginname and password=@password and sfyx='1'";
                        cmd.Parameters.Clear();
                        Common.AddParameter(cmd, "@loginname", DbType.String).Value = loginname.ToUpper();
                        Common.AddParameter(cmd, "@password", DbType.String).Value = password;
                        using (DbDataReader dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read()) throw new Exception("登录名或密码错误");
                            userID = dr["ID"].ToString();
                            roleID = dr["roleID"].ToString();
                        }//DbDataReader

                        cmd.CommandText = "select moduleID from [ADMIN].[RoleModule] where roleID=@roleID";
                        cmd.Parameters.Clear();
                        Common.AddParameter(cmd, "@roleID", DbType.String).Value = roleID;
                        using (DbDataReader dr = cmd.ExecuteReader())
                            while (dr.Read())
                                userData.Add(dr["moduleID"].ToString());
                    }//DbCommand
                }//DbConnection

                //////////////////////////////////////////////////////////////////////////
                // 3. 用户授权
                //////////////////////////////////////////////////////////////////////////
                bool isPersistent = true;

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                  1,                                     // ticket version
                  userID,                                // authenticated username
                  DateTime.Now,                          // issueDate
                  DateTime.Now.AddMonths(1),             // expiryDate
                  isPersistent,                          // true to persist across browser sessions
                  string.Join(",", userData),            // can be used to store additional user data
                  FormsAuthentication.FormsCookiePath);  // the path for the cookie

                // Encrypt the ticket using the machine key
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                // Add the cookie to the request to save it
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                cookie.HttpOnly = true;
                if (!string.IsNullOrWhiteSpace(rememberme))
                    cookie.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Add(cookie);

                // Your redirect logic
                message = "登录成功";


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
    }//Page_Load
}//class
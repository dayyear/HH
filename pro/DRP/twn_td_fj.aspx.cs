using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.Common;
using System.Data;
using Newtonsoft.Json;

public partial class pro_DRP_twn_td_fj : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;

        if (Request.HttpMethod == "GET")
        {
            GET();
        }//GET
        else if (Request.HttpMethod == "POST")
        {
            POST();
        }//POST

        
    }//Page_Load

    private void GET()
    {
        Response.ContentType = "application/msword";
        Response.AddHeader("content-disposition", string.Format("filename={0}.doc", Server.UrlPathEncode("行程安排")));
        Response.Clear();
        Response.BufferOutput = true;

        try
        {
            string ID = Request["ID"];
            if (string.IsNullOrWhiteSpace(ID)) throw new Exception("主键不能为空");

            string providerName = ConfigurationManager.ConnectionStrings["HuiHuang"].ProviderName;
            string connectionString = ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString;
            DbProviderFactory df = DbProviderFactories.GetFactory(providerName);
            using (DbConnection cn = df.CreateConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                using (DbCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "SELECT XC FROM [DRP].[TWN_TD_FJ] WHERE ID=@ID";
                    Common.AddParameter(cmd, "@ID", DbType.String).Value = ID.Trim();
                    using (DbDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) throw new Exception("未找到记录");
                        if (Convert.IsDBNull(dr[0])) throw new Exception("附件为空");
                        Response.BinaryWrite((byte[])dr[0]);
                    }//DbDataReader
                }//DbCommand
            }//DbConnection
        }//try
        catch (Exception ex)
        {
            Response.ContentType = "text/html";
            Response.Write(string.Format("<script type='text/javascript'>alert('{0}');window.open('','_self');window.close();</script>", ex.Message.Replace(@"'", @"\'")));
        }//catch

        Response.End();
    }//GET

    private void POST()
    {
        Response.ContentType = "text/html";
        Response.Clear();
        Response.BufferOutput = true;

        bool success = true;
        string message = null;
        int total = 0;
        List<Dictionary<string, Object>> items = new List<Dictionary<string, object>>();

        try
        {
            HttpPostedFile file = Request.Files[0];
            if (file == null) throw new Exception("[上传文件]不能为空");

            message = Guid.NewGuid().ToString();
            file.SaveAs(Server.MapPath("~/pub/attachment/" + message));

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
}//class
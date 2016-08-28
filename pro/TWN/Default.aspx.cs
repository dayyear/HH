using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Text;

public partial class index : System.Web.UI.Page
{
    public string optionYWY = "";
    public string optionZJZL = "";
    public string optionCSD = "";
    public string optionQFD = "";

    public string optionCJJL = "";
    public string optionHYZK = "";
    public string optionSFQR = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string providerName = ConfigurationManager.ConnectionStrings["HuiHuang"].ProviderName;
            string connectionString = ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString;

            DbProviderFactory df = DbProviderFactories.GetFactory(providerName);
            using (DbConnection cn = df.CreateConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                using (DbCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "select * from [ADMIN].[User]";
                    using (DbDataReader dr = cmd.ExecuteReader())
                    {
                        StringBuilder sb1 = new StringBuilder();
                        while (dr.Read()) sb1.AppendFormat("<option value='{0}'>{1}</option>", dr["ID"], dr["username"]);
                        optionYWY = sb1.ToString();
                    }//DbDataReader
                }//DbCommand
            }//DbConnection


            var items = ADMIN.DM.Select("ZJZL");
            var sb = new StringBuilder();
            foreach (var item in items)
                sb.AppendFormat("<option value='{0}'>{1}</option>", item["DM"], item["MC"]);
            optionZJZL = sb.ToString();

            items = ADMIN.DM.Select("GJDQ");
            sb = new StringBuilder();
            foreach (var item in items)
                sb.AppendFormat("<option value='{0}'>{0}-{1}</option>", item["DM"], item["MC"]);
            optionCSD = sb.ToString();
            optionQFD = sb.ToString();

            items = ADMIN.DM.Select("CJJL");
            sb = new StringBuilder();
            foreach (var item in items)
                sb.AppendFormat("<option value='{0}'>{1}</option>", item["DM"], item["MC"]);
            optionCJJL = sb.ToString();

            items = ADMIN.DM.Select("HYZK");
            sb = new StringBuilder();
            foreach (var item in items)
                sb.AppendFormat("<option value='{0}'>{1}</option>", item["DM"], item["MC"]);
            optionHYZK = sb.ToString();

            items = ADMIN.DM.Select("SFQR");
            sb = new StringBuilder();
            foreach (var item in items)
                sb.AppendFormat("<option value='{0}'>{1}</option>", item["DM"], item["MC"]);
            optionSFQR = sb.ToString();


        }//if (!IsPostBack)
    }//Page_Load
}//class
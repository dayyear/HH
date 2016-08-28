using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ionic.Zip;
using System.Text;
using System.Configuration;
using System.Data.Common;

public partial class public_test_zip_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Clear();


        using (ZipFile zip = new ZipFile(Encoding.UTF8))
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
                    cmd.CommandText = string.Format("SELECT * FROM [TWN].[CustomerImage] WHERE ID=127");
                    using (DbDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();
                        zip.AddEntry("127啊/127啊.jpg", (byte[])dr["PHOTO"]);
                    }//DbDataReader
                }//DbCommand
            }//DbConnection


            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "filename=" + "a.zip");

            zip.Save(Response.OutputStream);
        }
        Response.End();
    }
}
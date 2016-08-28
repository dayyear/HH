using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class pub_test_form_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //foreach (var A1 in Request.Params.GetValues("A1"))
        //    Response.Write(A1 + "<hr />");
        using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["HuiHuang"].ConnectionString))
        {
            cn.Open();
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [TWN].[Customer] WHERE XM=@XM";
                //cmd.Parameters.AddWithValue("@XM", "翁静");
                cmd.Parameters.Add("@XM", SqlDbType.VarChar).Value = "翁静";
                Response.Write(cmd.Parameters[0].SqlValue);
                Response.Write(cmd.Parameters[0].Value);
            } //cmd
        }//cn
    }//Page_Load
}//class
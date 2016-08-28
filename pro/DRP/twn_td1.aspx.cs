using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class pro_DRP_twn_td1 : System.Web.UI.Page
{
    protected string ID;
    protected string VERB;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;

        if (Request.HttpMethod == "GET")
        {
            try
            {
                VERB = Request["VERB"];
                ID = Request["ID"];

                if (string.IsNullOrWhiteSpace(VERB))
                    throw new Exception("[动词]不能为空");
                if (!Regex.IsMatch(VERB, @"\A(SELECT|INSERT|UPDATE)\z", RegexOptions.IgnoreCase))
                    throw new Exception("无效的[动词]:" + VERB);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
                Response.End();
            }
        }//GET

        else if (Request.HttpMethod == "POST")
        {

        }//POST
    }
}
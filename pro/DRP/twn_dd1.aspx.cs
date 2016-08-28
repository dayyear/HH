using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class pro_DRP_twn_dd1 : System.Web.UI.Page
{
    protected string ID;
    protected string TD_ID;
    protected string VERB;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;

        if (Request.HttpMethod == "GET")
        {
            try
            {
                ID = Request["ID"];
                TD_ID = Request["TD_ID"];
                VERB = Request["VERB"];

                if (string.IsNullOrWhiteSpace(VERB))
                    throw new Exception("[动词]不能为空");
                if (!Regex.IsMatch(VERB, @"\A(INSERT|UPDATE)\z", RegexOptions.IgnoreCase))
                    throw new Exception("无效的[动词]:" + VERB);

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
                Response.End();
            }
        }//GET
    }
}
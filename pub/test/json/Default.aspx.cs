using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using System.Data.Common;
using System.Text;
using System.Data;
using Newtonsoft.Json;

public partial class pub_test_json_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "application/json";
        Response.Clear();
        Response.BufferOutput = true;

        Resp resp = DRP.TWN_DD.SELECT(
            User: User,
            ID: Request["ID"],
            rows: Request["rows"],
            page: Request["page"]);

        Response.Write(JsonConvert.SerializeObject(resp, Formatting.Indented));
        Response.End();
    }
}
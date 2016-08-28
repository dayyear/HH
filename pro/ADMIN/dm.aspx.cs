using System;
using System.Text;
using System.Web.UI;

public partial class pro_ADMIN_dm : Page
{
    protected string optionRows;
    protected string optionSfyx;
    protected string optionLx;
    protected void Page_Load(object sender, EventArgs e)
    {
        var items = ADMIN.DM.Select("rows");
        var sb = new StringBuilder();
        foreach (var item in items)
            sb.AppendFormat("<option value='{0}'>{1}</option>", item["DM"], item["MC"]);
        optionRows = sb.ToString();

        items = ADMIN.DM.Select("SFYX");
        sb = new StringBuilder();
        foreach (var item in items)
            sb.AppendFormat("<option value='{0}'>{1}</option>", item["DM"], item["MC"]);
        optionSfyx = sb.ToString();

        items = ADMIN.DM.Select("_LX");
        sb = new StringBuilder();
        foreach (var item in items)
            sb.AppendFormat("<option value='{0}'>{1}</option>", item["DM"], item["MC"]);
        optionLx = sb.ToString();


    }
}
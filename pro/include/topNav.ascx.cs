using System;
using System.Linq;
using System.Web.UI;

public partial class pro_include_topNav : UserControl
{
    protected string username;
    protected void Page_Load(object sender, EventArgs e)
    {
        var items = ADMIN.User.Select(Page.User.Identity.Name);
        if (!items.Any())
            throw new Exception("未找到用户记录，ID：" + Page.User.Identity.Name);
        var item = items[0];
        username = (string)item["username"];
    }//Page_Load
}//class
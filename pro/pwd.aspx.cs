using System;
using System.Linq;
using System.Web.UI;
using Newtonsoft.Json;

public partial class pro_pwd : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;
        if (Request.HttpMethod == "GET")
            GET();
        else if (Request.HttpMethod == "POST")
            POST();
    }//Page_Load

    private void GET()
    {
        try
        {
            var items = ADMIN.User.Select(User.Identity.Name);
            if (!items.Any())
                throw new Exception("未找到用户记录，ID：" + User.Identity.Name);
            var item = items[0];
            loginname.InnerText = (string)item["loginname"];
            username.InnerText = (string)item["username"];
        }
        catch (Exception ex)
        {
            exception.InnerText = ex.ToString();
        }
    }//GET

    private void POST()
    {
        Response.ContentType = "application/json";
        Response.Clear();
        Response.BufferOutput = true;

        Resp resp = new Resp();
        try
        {
            string pwd1 = Request["pwd1"];
            string pwd2 = Request["pwd2"];
            string pwd3 = Request["pwd3"];

            if (string.IsNullOrWhiteSpace(pwd1)) throw new Exception("[旧密码]不能为空");
            if (string.IsNullOrWhiteSpace(pwd2)) throw new Exception("[新密码]不能为空");
            if (string.IsNullOrWhiteSpace(pwd3)) throw new Exception("[确认新密码]不能为空");
            if (!pwd2.Equals(pwd3)) throw new Exception("两次输入的新密码不一致");

            ADMIN.User.ChangePassword(User.Identity.Name, pwd1, pwd2);
        }
        catch (Exception ex)
        {
            resp.success = false;
            resp.message = ex.Message;
            resp.total = 0;
            resp.items.Clear();
        }
        Response.Write(JsonConvert.SerializeObject(new { resp.success, resp.message, resp.total, resp.items }, Formatting.Indented));
        Response.End();

    }//POST
}//class
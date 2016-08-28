using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Newtonsoft.Json;

public partial class pro_DRP_cj_td : System.Web.UI.Page
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
            //////////////////////////////////////////////////////////////////////////
            // toolbar
            //////////////////////////////////////////////////////////////////////////
            if (User.IsInRole("ADMIN"))
                toolbar.InnerHtml = "<a href='cj_td1.aspx?VERB=INSERT' class='btn' target='_blank'>发布</a>";

            //////////////////////////////////////////////////////////////////////////
            // 表单验证
            //////////////////////////////////////////////////////////////////////////
            int rows = 10;
            if (!string.IsNullOrWhiteSpace(Request["rows"])) rows = int.Parse(Request["rows"]);
            int page = 1;
            if (!string.IsNullOrWhiteSpace(Request["page"])) page = int.Parse(Request["page"]);

            //////////////////////////////////////////////////////////////////////////
            // 数据查询
            //////////////////////////////////////////////////////////////////////////
            Resp resp = DRP.CJ_TD.Select(null, rows, page);

            if (!resp.success)
                throw new Exception(resp.message);
            else if (resp.total == 0)
                throw new Exception("未找到记录");
            else
            {

                //////////////////////////////////////////////////////////////////////////
                // datagrid
                //////////////////////////////////////////////////////////////////////////
                var sb = new StringBuilder();
                sb.AppendFormat("<table>");
                sb.AppendFormat("<tr>");
                sb.AppendFormat("<th>序号</th>");
                sb.AppendFormat("<th>团队编号</th>");
                sb.AppendFormat("<th>线路名称</th>");
                sb.AppendFormat("<th>出发日期</th>");
                sb.AppendFormat("<th>材料截止日期</th>");
                sb.AppendFormat("<th>行程</th>");
                sb.AppendFormat("<th>签证材料</th>");
                sb.AppendFormat("<th>天数</th>");
                sb.AppendFormat("<th>人数</th>");
                sb.AppendFormat("<th>已订</th>");
                sb.AppendFormat("<th>剩余</th>");
                sb.AppendFormat("<th>状态</th>");
                sb.AppendFormat("<th>发布人</th>");
                sb.AppendFormat("<th>发布时间</th>");
                sb.AppendFormat("</tr>");

                var sn = (page - 1) * rows;
                var i = 0;
                foreach (var item in resp.items)
                {
                    if (++i % 2 == 0) sb.AppendFormat("<tr class='odd'>");
                    else sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", ++sn);
                    sb.AppendFormat("<td><a href='cj_td1.aspx?VERB=SELECT&ID={0}' target='_blank'>{1}</a></td>", item["ID"], HttpUtility.HtmlEncode(item["BH"]));
                    sb.AppendFormat("<td style='white-space:normal;'>{0}</td>", HttpUtility.HtmlEncode(item["XLMC"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(item["CFRQ"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(item["CLJZRQ"]));
                    sb.AppendFormat("<td><a href='cj_td_fj.aspx?TYPE=XC&ID={0}' target='_blank'><img src='../../pub/img/word.gif'></a></td>", item["ID"]);
                    sb.AppendFormat("<td><a href='cj_td_fj.aspx?TYPE=QZCL&ID={0}' target='_blank'><img src='../../pub/img/word.gif'></a></td>", item["ID"]);
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(item["TS"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(item["RS"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(item["YDRS"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(item["SYRS"]));
                    sb.AppendFormat("<td class='zt{0}'>{1}</td>", HttpUtility.HtmlEncode(item["ZT"]), HttpUtility.HtmlEncode(item["ZT1"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(item["FBR1"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(item["FBSJ"]));
                    sb.AppendFormat("</tr>");
                }
                sb.AppendFormat("</table>");
                datagrid.InnerHtml = sb.ToString();

                //////////////////////////////////////////////////////////////////////////
                // pagination
                //////////////////////////////////////////////////////////////////////////
                sb = new StringBuilder();
                sb.AppendFormat("<table>");
                sb.AppendFormat("<tr>");
                sb.AppendFormat("<td>");
                sb.AppendFormat("<span>共有<strong>{0}</strong>条记录,</span> ", resp.total);
                sb.AppendFormat("<span>第<strong>{0}</strong>页/</span>", page);
                sb.AppendFormat("<span>共<strong>{0}</strong>页</span>", Math.Ceiling((double)resp.total / (double)rows));
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td>");
                sb.AppendFormat("<a class='{0}' href='cj_td.aspx?rows={1}&page={2}'>首页</a>", page <= 1 ? "btnDisable" : "btn", rows, 1);
                sb.AppendFormat("<a class='{0}' href='cj_td.aspx?rows={1}&page={2}'>上一页</a>", page <= 1 ? "btnDisable" : "btn", rows, page - 1);
                sb.AppendFormat("<a class='{0}' href='cj_td.aspx?rows={1}&page={2}'>下一页</a>", page * rows >= resp.total ? "btnDisable" : "btn", rows, page + 1);
                sb.AppendFormat("<a class='{0}' href='cj_td.aspx?rows={1}&page={2}'>尾页</a>", page * rows >= resp.total ? "btnDisable" : "btn", rows, Math.Ceiling((double)resp.total / (double)rows));
                sb.AppendFormat("</td>");
                sb.AppendFormat("<td>");
                sb.AppendFormat("<span>[注]：查询结果按照<strong>[出发日期]</strong>倒序排列</span>");
                sb.AppendFormat("</td>");
                sb.AppendFormat("</tr>");
                sb.AppendFormat("</table>");
                pagination.InnerHtml = sb.ToString();

            }//else
        }//try
        catch (Exception ex)
        {
            exception.InnerHtml = ex.Message;
        }//catch
    }//GET

    private void POST()
    {
    }//POST
}//class
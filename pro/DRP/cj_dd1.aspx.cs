using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Text;

public partial class pro_DRP_cj_dd1 : System.Web.UI.Page
{
    protected string ID;
    protected string TD_ID;
    protected string VERB;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "GET")
            GET();
        else if (Request.HttpMethod == "POST")
            POST();
    }//Page_Load

    private void GET()
    {
        try
        {
            ID = Request["ID"];
            TD_ID = Request["TD_ID"];
            VERB = Request["VERB"];

            if (string.IsNullOrWhiteSpace(VERB))
                throw new Exception("[动词VERB]不能为空");
            if (!Regex.IsMatch(VERB, @"\A(INSERT|UPDATE)\z", RegexOptions.IgnoreCase))
                throw new Exception("无效的[动词VERB]:" + VERB);

            if (VERB.ToUpper() == "UPDATE")
            {
                if (string.IsNullOrWhiteSpace(ID))
                    throw new Exception("[主键ID]不能为空");

                Resp resp = DRP.CJ_DD.Select(User, ID);
                if (!resp.success) throw new Exception(resp.message);
                if (resp.total <= 0) throw new Exception("未找到记录");
                var item = resp.items.First();

                // 基本信息
                BH.InnerHtml = HttpUtility.HtmlEncode(item["BH"]);

                TD_BH.InnerHtml = HttpUtility.HtmlEncode(item["TD_BH"]);
                TD_XLMC.InnerHtml = HttpUtility.HtmlEncode(item["TD_XLMC"]);
                TD_ZS.InnerHtml = HttpUtility.HtmlEncode(item["TD_ZS"]);
                TD_CFRQ.InnerHtml = HttpUtility.HtmlEncode(item["TD_CFRQ"]);
                TD_CLJZRQ.InnerHtml = HttpUtility.HtmlEncode(item["TD_CLJZRQ"]);
                TD_TS.InnerHtml = HttpUtility.HtmlEncode(item["TD_TS"]);
                TD_RS.InnerHtml = HttpUtility.HtmlEncode(item["TD_RS"]);
                TD_FBR.InnerHtml = HttpUtility.HtmlEncode(item["TD_FBR"]);
                TD_FBSJ.InnerHtml = HttpUtility.HtmlEncode(item["TD_FBSJ"]);

                // 费用计算
                TD_DFC.InnerHtml = HttpUtility.HtmlEncode(item["TD_DFC"]);
                FYHJ.InnerHtml = HttpUtility.HtmlEncode(item["FYHJ"]);
                JSJG.InnerHtml = HttpUtility.HtmlEncode(item["JSJG"]);
                var sb = new StringBuilder();
                foreach (var rs in item["RS"] as List<Dictionary<string, Object>>)
                {
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(rs["SN"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(rs["BJMC"]));
                    sb.AppendFormat("<td><span class='MSJ'>{0}</span></td>", HttpUtility.HtmlEncode(rs["MSJ"]));
                    sb.AppendFormat("<td><span class='FL'>{0}</span></td>", HttpUtility.HtmlEncode(rs["FL"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(rs["RS"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(rs["RS_DFC"]));
                    sb.AppendFormat("<td><span class='FYXJ'>{0}</span><span class='FLXJ' style='display:none;'>0</span></td>", HttpUtility.HtmlEncode(rs["FYXJ"]));
                    sb.AppendFormat("</tr>");
                }
                FYJS.InnerHtml = sb.ToString();

                // 游客名单
                sb = new StringBuilder();
                foreach (var md in item["MD"] as List<Dictionary<string, Object>>)
                {
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(md["SN"]));
                    sb.AppendFormat("<td><input type='text' name='XM' size='10' value='{0}' /></td>", HttpUtility.HtmlEncode(md["XM"]));
                    sb.AppendFormat("<td><input type='text' name='SFZH' size='19' value='{0}' /></td>", HttpUtility.HtmlEncode(md["SFZH"]));
                    sb.AppendFormat("<td><select name='XB'><option value=''>请选择</option>{0}</select></td>", ADMIN.DM.Option("XB", null, md["XB"] == null ? null : md["XB"].ToString()));
                    sb.AppendFormat("<td><input type='text' name='CSRQ' class='datepicker' size='11' value='{0}'/></td>", HttpUtility.HtmlEncode(md["CSRQ"]));
                    sb.AppendFormat("<td><input type='text' name='LXDH' size='12' value='{0}'/></td>", HttpUtility.HtmlEncode(md["LXDH"]));
                    sb.AppendFormat("<td><select name='SFJS'>{0}</select></td>", DRP.DM.Option("SFJS", null, md["SFJS"].ToString()));
                    sb.AppendFormat("</tr>");
                }
                YKMD.InnerHtml = sb.ToString();

                // 其他信息
                SCD.InnerHtml = HttpUtility.HtmlEncode(item["SCD1"]);
                BZ.InnerHtml = HttpUtility.HtmlEncode(item["BZ"]);
                JBR.InnerHtml = HttpUtility.HtmlEncode(item["JBR"]);
                JBRSJ.InnerHtml = HttpUtility.HtmlEncode(item["JBRSJ"]);
                YDR.InnerHtml = HttpUtility.HtmlEncode(item["YDR1"]);
                YDSJ.InnerHtml = HttpUtility.HtmlEncode(item["YDSJ"]);
                if (User.IsInRole("ADMIN"))
                    ZT.InnerHtml = string.Format("<select name='ZT'>{0}</select>", DRP.DM.Option("DD_ZT", null, item["ZT"].ToString()));
                else
                    ZT.InnerHtml = HttpUtility.HtmlEncode(item["ZT1"]);

            }//UPDATE
            else if (VERB.ToUpper() == "INSERT")
            {
                // 工具团队主键TD_ID确定团队相关信息
                if (string.IsNullOrWhiteSpace(TD_ID))
                    throw new Exception("[团队主键TD_ID]不能为空");
                Resp resp = DRP.CJ_TD.Select(TD_ID);
                if (!resp.success) throw new Exception(resp.message);
                if (resp.total <= 0) throw new Exception("未找到记录");
                var item = resp.items.First();

                // 基本信息
                TD_BH.InnerHtml = HttpUtility.HtmlEncode(item["BH"]);
                TD_XLMC.InnerHtml = HttpUtility.HtmlEncode(item["XLMC"]);
                TD_ZS.InnerHtml = HttpUtility.HtmlEncode(item["ZS"]);
                TD_CFRQ.InnerHtml = HttpUtility.HtmlEncode(item["CFRQ"]);
                TD_CLJZRQ.InnerHtml = HttpUtility.HtmlEncode(item["CLJZRQ"]);
                TD_TS.InnerHtml = HttpUtility.HtmlEncode(item["TS"]);
                TD_RS.InnerHtml = HttpUtility.HtmlEncode(item["RS"]);
                ZT1.InnerHtml = HttpUtility.HtmlEncode(item["ZT1"]);
                XC.InnerHtml = string.Format("<a href='cj_td_fj.aspx?TYPE=XC&ID={0}' target='_blank'><img src='../../pub/img/word.gif'></a>", item["ID"]);
                QZCL.InnerHtml = string.Format("<a href='cj_td_fj.aspx?TYPE=QZCL&ID={0}' target='_blank'><img src='../../pub/img/word.gif'></a>", item["ID"]);
                YDRS.InnerHtml = HttpUtility.HtmlEncode(item["YDRS"]);
                SYRS.InnerHtml = HttpUtility.HtmlEncode(item["SYRS"]);
                TD_FBR.InnerHtml = HttpUtility.HtmlEncode(item["FBR"]);
                TD_FBSJ.InnerHtml = HttpUtility.HtmlEncode(item["FBSJ"]);

                // 费用计算
                TD_DFC.InnerHtml = HttpUtility.HtmlEncode(item["DFC"]);
                var sb = new StringBuilder();
                foreach (var bj in item["BJ"] as List<Dictionary<string, Object>>)
                {
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(bj["SN"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(bj["BJMC"]));
                    sb.AppendFormat("<td><span class='MSJ'>{0}</span></td>", HttpUtility.HtmlEncode(bj["MSJ"]));
                    sb.AppendFormat("<td><span class='FL'>{0}</span></td>", HttpUtility.HtmlEncode(bj["FL"]));
                    sb.AppendFormat("<td><input type='text' class='RS' name='RS' value='{0}' size='5' /></td>", 0);
                    sb.AppendFormat("<td><input type='text' class='RS_DFC' name='RS_DFC' value='{0}' size='5' /></td>", 0);
                    sb.AppendFormat("<td><span class='FYXJ'>0</span><span class='FLXJ' style='display:none;'>{0}</span></td>", 0);
                    sb.AppendFormat("</tr>");
                }
                FYJS.InnerHtml = sb.ToString();

                // 游客名单

                // 其他信息
                SCD.InnerHtml = string.Format("<select name='SCD'><option value=''>请选择</option>{0}</select>", DRP.DM.Option("SCD", null, null));
            }//INSERT
        }//try
        catch (Exception ex)
        {
            exception.InnerText = ex.Message;
        }//catch
    }//GET

    private void POST()
    {
        Response.ContentType = "application/json";
        Response.Clear();
        Response.BufferOutput = true;
        Resp resp = new Resp();

        try
        {
            string VERB = Request["VERB"];
            if (string.IsNullOrWhiteSpace(VERB)) throw new Exception("[动词]不能为空");
            if (VERB.ToUpper() == "INSERT")
                resp = DRP.CJ_DD.Insert(User, Request["TD_ID"], Request["SCD"], Request["BZ"], Request["JBR"], Request["JBRSJ"],
                    Request.Params.GetValues("RS"), Request.Params.GetValues("RS_DFC"),
                    Request.Params.GetValues("XM"), Request.Params.GetValues("SFZH"), Request.Params.GetValues("XB"),
                    Request.Params.GetValues("CSRQ"), Request.Params.GetValues("LXDH"), Request.Params.GetValues("SFJS"));
            else if (VERB.ToUpper() == "UPDATE")
                resp = DRP.CJ_DD.Update(User, Request["ID"], Request["ZT"],
                    Request.Params.GetValues("XM"), Request.Params.GetValues("SFZH"), Request.Params.GetValues("XB"),
                    Request.Params.GetValues("CSRQ"), Request.Params.GetValues("LXDH"), Request.Params.GetValues("SFJS"));
            else throw new Exception("无效的[动词]：" + VERB);
        }
        catch (Exception ex)
        {
            resp.success = false;
            resp.message = ex.Message;
            resp.total = 0;
            resp.items.Clear();
        }
        Response.Write(JsonConvert.SerializeObject(new
        {
            success = resp.success,
            message = resp.message,
            total = resp.total,
            items = resp.items
        }, Formatting.Indented));
        Response.End();
    }//POST

}
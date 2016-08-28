using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;

public partial class pro_DRP_cj_td1 : System.Web.UI.Page
{
    protected string ID;
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
            VERB = Request["VERB"];
            ID = Request["ID"];

            if (string.IsNullOrWhiteSpace(VERB))
                throw new Exception("[动词VERB]不能为空");
            if (!Regex.IsMatch(VERB, @"\A(SELECT|INSERT|UPDATE)\z", RegexOptions.IgnoreCase))
                throw new Exception("无效的[动词VERB]:" + VERB);

            if (VERB.ToUpper() == "SELECT")
            {
                Resp resp = DRP.CJ_TD.Select(ID);
                if (!resp.success) throw new Exception(resp.message);
                if (resp.total <= 0) throw new Exception("未找到记录");
                var item = resp.items.First();

                // 基本信息
                BH.InnerHtml = HttpUtility.HtmlEncode(item["BH"]);
                XLMC.InnerHtml = HttpUtility.HtmlEncode(item["XLMC"]);
                ZS.InnerHtml = HttpUtility.HtmlEncode(item["ZS"]);
                CFRQ.InnerHtml = HttpUtility.HtmlEncode(item["CFRQ"]);
                CLJZRQ.InnerHtml = HttpUtility.HtmlEncode(item["CLJZRQ"]);
                TS.InnerHtml = HttpUtility.HtmlEncode(item["TS"]);
                RS.InnerHtml = HttpUtility.HtmlEncode(item["RS"]);
                ZT.InnerHtml = HttpUtility.HtmlEncode(item["ZT1"]);
                XC.InnerHtml = string.Format("<a href='cj_td_fj.aspx?TYPE=XC&ID={0}' target='_blank'><img src='../../pub/img/word.gif'></a>", item["ID"]);
                QZCL.InnerHtml = string.Format("<a href='cj_td_fj.aspx?TYPE=QZCL&ID={0}' target='_blank'><img src='../../pub/img/word.gif'></a>", item["ID"]);
                YDRS.InnerHtml = HttpUtility.HtmlEncode(item["YDRS"]);
                SYRS.InnerHtml = HttpUtility.HtmlEncode(item["SYRS"]);
                FBR.InnerHtml = HttpUtility.HtmlEncode(item["FBR1"]);
                FBSJ.InnerHtml = HttpUtility.HtmlEncode(item["FBSJ"]);

                // 报价信息
                BJSL.InnerHtml = (item["BJ"] as List<Dictionary<string, Object>>).Count.ToString();
                DFC.InnerHtml = HttpUtility.HtmlEncode(item["DFC"]);
                var sb = new StringBuilder();
                foreach (var bj in item["BJ"] as List<Dictionary<string, Object>>)
                {
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(bj["SN"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(bj["BJMC"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(bj["MSJ"]));
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(bj["FL"]));
                    sb.AppendFormat("</tr>");
                }
                BJXX.InnerHtml = sb.ToString();
            }//SELECT
            else if (VERB.ToUpper() == "UPDATE")
            {
                Resp resp = DRP.CJ_TD.Select(ID);
                if (!resp.success) throw new Exception(resp.message);
                if (resp.total <= 0) throw new Exception("未找到记录");
                var item = resp.items.First();

                // 基本信息
                BH.InnerHtml = string.Format("<input type='text' name='BH' size='11' value='{0}' />", HttpUtility.HtmlEncode(item["BH"]));
                XLMC.InnerHtml = string.Format("<input type='text' name='XLMC' size='11' value='{0}' />", HttpUtility.HtmlEncode(item["XLMC"]));
                ZS.InnerHtml = string.Format("<input type='text' name='ZS' size='11' value='{0}' />", HttpUtility.HtmlEncode(item["ZS"]));
                CFRQ.InnerHtml = string.Format("<input type='text' name='CFRQ' class='datepicker' size='11' value='{0}' />", HttpUtility.HtmlEncode(item["CFRQ"]));
                CLJZRQ.InnerHtml = string.Format("<input type='text' name='CLJZRQ' class='datepicker' size='11' value='{0}' />", HttpUtility.HtmlEncode(item["CLJZRQ"]));
                TS.InnerHtml = string.Format("<input type='text' name='TS' size='11' value='{0}' />", HttpUtility.HtmlEncode(item["TS"]));
                RS.InnerHtml = string.Format("<input type='text' name='RS' size='11' value='{0}' />", HttpUtility.HtmlEncode(item["RS"]));
                ZT.InnerHtml = string.Format("<select name='ZT'>{0}</select>", DRP.DM.Option("TD_ZT", null, item["ZT"].ToString()));
                XC.InnerHtml = string.Format("<a href='cj_td_fj.aspx?TYPE=XC&ID={0}' target='_blank'><img src='../../pub/img/word.gif'></a> <input type='file' id='file_XC' name='file_XC' size='4' style='width:70px' /><input type='text' name='XC' readonly='readonly' size='15' />", item["ID"]);
                QZCL.InnerHtml = string.Format("<a href='cj_td_fj.aspx?TYPE=QZCL&ID={0}' target='_blank'><img src='../../pub/img/word.gif'></a> <input type='file' id='file_QZCL' name='file_QZCL' size='4' style='width:70px' /><input type='text' name='QZCL' readonly='readonly' size='15' />", item["ID"]);

                // 报价信息
                BJSL.InnerHtml = string.Format("<input type='text' value='{0}' size='10' />", (item["BJ"] as List<Dictionary<string, Object>>).Count);
                DFC.InnerHtml = string.Format("<input type='text' value='{0}' name='DFC' size='10' />", HttpUtility.HtmlEncode(item["DFC"]));
                var sb = new StringBuilder();
                foreach (var bj in item["BJ"] as List<Dictionary<string, Object>>)
                {
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", HttpUtility.HtmlEncode(bj["SN"]));
                    sb.AppendFormat("<td><input type='text' name='BJMC' size='65' value='{0}' /></td>", HttpUtility.HtmlEncode(bj["BJMC"]));
                    sb.AppendFormat("<td><input type='text' name='MSJ' size='10' value='{0}' /></td>", HttpUtility.HtmlEncode(bj["MSJ"]));
                    sb.AppendFormat("<td><input type='text' name='FL' size='10' value='{0}' /></td>", HttpUtility.HtmlEncode(bj["FL"]));
                    sb.AppendFormat("</tr>");
                }
                BJXX.InnerHtml = sb.ToString();

            }//UPDATE
            else if (VERB.ToUpper() == "INSERT")
            {
                // 基本信息
                BH.InnerHtml = string.Format("<input type='text' name='BH' size='11' />");
                XLMC.InnerHtml = string.Format("<input type='text' name='XLMC' size='11' />");
                ZS.InnerHtml = string.Format("<input type='text' name='ZS' size='11' />");
                CFRQ.InnerHtml = string.Format("<input type='text' name='CFRQ' class='datepicker' size='11' />");
                CLJZRQ.InnerHtml = string.Format("<input type='text' name='CLJZRQ' class='datepicker' size='11' />");
                TS.InnerHtml = string.Format("<input type='text' name='TS' size='11' />");
                RS.InnerHtml = string.Format("<input type='text' name='RS' size='11' />");
                ZT.InnerHtml = string.Format("<select name='ZT'><option value=''>请选择</option>{0}</select>", DRP.DM.Option("TD_ZT", null, null));
                XC.InnerHtml = string.Format("<input type='file' id='file_XC' name='file_XC' size='4' style='width:70px' /><input type='text' name='XC' readonly='readonly' size='15' />");
                QZCL.InnerHtml = string.Format("<input type='file' id='file_QZCL' name='file_QZCL' size='4' style='width:70px' /><input type='text' name='QZCL' readonly='readonly' size='15' />");

                // 报价信息
                BJSL.InnerHtml = string.Format("<input type='text' value='3' size='10' />");
                DFC.InnerHtml = string.Format("<input type='text' name='DFC' size='10' />");
                var sb = new StringBuilder();
                for (int i = 0; i < 3; i++)
                {
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", i + 1);
                    sb.AppendFormat("<td><input type='text' name='BJMC' size='65' /></td>");
                    sb.AppendFormat("<td><input type='text' name='MSJ' size='10' /></td>");
                    sb.AppendFormat("<td><input type='text' name='FL' size='10' /></td>");
                    sb.AppendFormat("</tr>");
                }
                BJXX.InnerHtml = sb.ToString();
            }//INSERT
        }//try
        catch (Exception ex)
        {
            exception.InnerText = ex.Message;
            //Common.Alert(ex.Message);
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
                resp = DRP.CJ_TD.Insert(User, Request["BH"], Request["XLMC"], Request["ZS"], Request["CFRQ"], Request["CLJZRQ"],
                    Request["ZT"], Request["RS"], Request["TS"], Request["DFC"], Request["XC"], Request["QZCL"],
                    Request.Params.GetValues("BJMC"), Request.Params.GetValues("MSJ"), Request.Params.GetValues("FL"));
            else if (VERB.ToUpper() == "UPDATE")
                resp = DRP.CJ_TD.Update(User, Request["ID"], Request["BH"], Request["XLMC"], Request["ZS"], Request["CFRQ"], Request["CLJZRQ"],
                    Request["ZT"], Request["RS"], Request["TS"], Request["DFC"], Request["XC"], Request["QZCL"],
                    Request.Params.GetValues("BJMC"), Request.Params.GetValues("MSJ"), Request.Params.GetValues("FL"));
            else if (VERB.ToUpper() == "COPY")
            {
                resp = DRP.CJ_TD.Copy(User, Request["ID"]);
            }
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

}//class
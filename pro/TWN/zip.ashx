<%@ WebHandler Language="C#" Class="zip" %>

using System;
using System.Web;
using System.Linq;
using System.Text;
using Ionic.Zip;
using System.Security.Cryptography;

public class zip : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var Request = context.Request;
        var Response = context.Response;
        var Server = context.Server;

        try
        {
            var TDID = Request["TDID"];
            Common.ValidString(TDID, CanNull.False, CanEmpty.False, "团队编号");
            var items = Twn.Customer.Select(TDID: TDID, rows: "1000", order: "LRSJ ASC");
            if (!items.Any())
                throw new Exception("未找到记录");

            using (var zip = new ZipFile(Encoding.UTF8))
            {
                var sn = 1;
                const string noimg = "37c048ff6c49b2d24ccf847d1e2d8cca";
                foreach (var item in items)
                {
                    sn++;
                    var ID = item["ID"].ToString();

                    var img = Twn.CustomerImage.Select(ID, "PASS1");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("0{0}.jpg", sn), img);

                    img = Twn.CustomerImage.Select(ID, "PHOTO");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}.jpg", sn), img);

                    var i = 0;
                    img = Twn.CustomerImage.Select(ID, "IDCARD1");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}-{1}.jpg", sn, ++i), img);
                    img = Twn.CustomerImage.Select(ID, "IDCARD2");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}-{1}.jpg", sn, ++i), img);
                    img = Twn.CustomerImage.Select(ID, "HOUSEHOLD1");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}-{1}.jpg", sn, ++i), img);
                    img = Twn.CustomerImage.Select(ID, "HOUSEHOLD2");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}-{1}.jpg", sn, ++i), img);
                    img = Twn.CustomerImage.Select(ID, "HOUSEHOLD3");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}-{1}.jpg", sn, ++i), img);
                    img = Twn.CustomerImage.Select(ID, "HOUSEHOLD4");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}-{1}.jpg", sn, ++i), img);
                    img = Twn.CustomerImage.Select(ID, "BIRTH");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}-{1}.jpg", sn, ++i), img);
                    img = Twn.CustomerImage.Select(ID, "CONSENT");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}-{1}.jpg", sn, ++i), img);
                    img = Twn.CustomerImage.Select(ID, "OTHER1");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}-{1}.jpg", sn, ++i), img);
                    img = Twn.CustomerImage.Select(ID, "OTHER2");
                    if (img != null && ComputeMd5(img) != noimg)
                        zip.AddEntry(string.Format("{0}-{1}.jpg", sn, ++i), img);
                }//foreach items
                Response.ContentType = "application/zip";
                Response.Clear();
                Response.BufferOutput = true;
                Response.AddHeader("content-disposition", string.Format("filename={0}.zip", Server.UrlPathEncode(TDID)));
                zip.Save(Response.OutputStream);
            }//zip
        }
        catch (Exception ex)
        {
            Response.ContentType = "text/html";
            Response.Clear();
            Response.BufferOutput = true;
            Response.Write("<h1>错误</h1>");
            Response.Write(string.Format("<p>{0}</p>", ex.Message));
        }
        finally
        {
            Response.End();
        }
    }//ProcessRequest

    private string ComputeMd5(byte[] bytes)
    {
        var md5 = MD5.Create();
        var data = md5.ComputeHash(bytes);
        var sb = new StringBuilder();
        foreach (var b in data)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }//ComputeMd5

    public bool IsReusable { get { return true; } }

}//class
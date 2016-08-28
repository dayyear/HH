using System;

using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


/// <summary>
/// 办证进度查询Web
/// </summary>
public class CertificateScheduleWeb
{
    //aspx
    public static string VIEWSTATE_Pattern1 = @"id=""__VIEWSTATE"" value=""([\S]*?)""";
    public static string EVENTVALIDATION_Pattern1 = @"id=""__EVENTVALIDATION"" value=""([\S]*?)""";
    public static string VIEWSTATE_Pattern2 = @"__VIEWSTATE\|([\S]*?)\|";
    public static string EVENTVALIDATION_Pattern2 = @"__EVENTVALIDATION\|([\S]*?)\|";
    private string VIEWSTATE = "";
    private string EVENTVALIDATION = "";

    // alert模板
    public static string ALERT_Pattern = @"alert\('([\s\S]*?)'\)";
    // 办证进度模板
    public static string BZJD_Pattern = @"<a id=""ctl00_contentPlaceHolder_contentListRP_ctl01_editHL"">([\s\S]*?)</a>";

    // 内部变量
    private CookieContainer cookieContainer = new CookieContainer();
    private string ip = "";
    private string port = "";

    // 常量
    private Encoding UTF8 = Encoding.UTF8;

    // 临时变量
    string responseString = "";
    string checkCode = "";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public CertificateScheduleWeb(string ip, string port)
    {
        this.ip = ip;
        this.port = port;
    }


    /// <summary>
    /// 查询制定业务编号的办证进度
    /// </summary>
    /// <param name="ywbh">业务编号</param>
    /// <returns></returns>
    public string Query(string ywbh)
    {
        string postString = "";

        responseString = Get(string.Format("http://{0}:{1}/GovAffairs/Wsbzcx/CertificateSchedule.aspx", ip, port));
        UpdateState();
        Get(string.Format("http://{0}:{1}/GovAffairs/Manage/RandomCode.aspx?change=1", ip, port));
        checkCode = cookieContainer.GetCookies(new Uri(string.Format("http://{0}:{1}/", ip, port)))["CheckCode"].Value;
        postString = string.Format("ctl00%24toolkitScriptManager=ctl00%24toolkitScriptManager%7Cctl00%24contentPlaceHolder%24ImageButton2&ctl00_toolkitScriptManager_HiddenField=&__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE={0}&__EVENTVALIDATION={1}&ctl00%24contentPlaceHolder%24resultHF=&ctl00%24contentPlaceHolder%24TextYWBH={2}&ctl00%24contentPlaceHolder%24tbRandomCode={3}&__ASYNCPOST=true&ctl00%24contentPlaceHolder%24ImageButton2.x=48&ctl00%24contentPlaceHolder%24ImageButton2.y=11",
            VIEWSTATE, EVENTVALIDATION, ywbh, checkCode);
        responseString = Post(string.Format("http://{0}:{1}/GovAffairs/Wsbzcx/CertificateSchedule.aspx", ip, port), postString);

        return responseString;
    }

    /// <summary>
    /// HTTP GET
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    private string Get(string uri)
    {
        string s = "";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.CookieContainer = cookieContainer;
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader sr = new StreamReader(stream, UTF8))
            s = sr.ReadToEnd();

        return s;
    }

    /// <summary>
    /// HTTP POST
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="postString"></param>
    /// <returns></returns>
    private string Post(string uri, string postString)
    {
        string s = "";
        
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.CookieContainer = cookieContainer;

        // 设置POST数据
        byte[] postByte = UTF8.GetBytes(postString);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
        request.ContentLength = postByte.Length;
        using (Stream stream = request.GetRequestStream())
            stream.Write(postByte, 0, postByte.Length);

        // 发送POST
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader sr = new StreamReader(stream, UTF8))
            s = sr.ReadToEnd();

        return s;

    }

    private void UpdateState(bool isInit = true)
    {
        if (isInit)
        {
            Match match = Regex.Match(responseString, VIEWSTATE_Pattern1);
            if (!match.Success) throw new Exception("找不到VIEWSTATE");
            VIEWSTATE = match.Groups[1].Value;
            match = Regex.Match(responseString, EVENTVALIDATION_Pattern1);
            if (!match.Success) throw new Exception("找不到EVENTVALIDATION");
            EVENTVALIDATION = match.Groups[1].Value;
        }
        else
        {
            Match match = Regex.Match(responseString, VIEWSTATE_Pattern2);
            if (!match.Success) throw new Exception("找不到VIEWSTATE");
            VIEWSTATE = match.Groups[1].Value;
            match = Regex.Match(responseString, EVENTVALIDATION_Pattern2);
            if (!match.Success) throw new Exception("找不到EVENTVALIDATION");
            EVENTVALIDATION = match.Groups[1].Value;
        }
    }
}

using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

public enum CanNull { False, True };
public enum CanEmpty { False, True };

/// <summary>
///Common 的摘要说明
/// </summary>
public static class Common
{
    /// <summary>
    /// 在命令中添加参数，不指定参数值
    /// </summary>
    /// <param name="cmd">命令</param>
    /// <param name="parameterName">参数名称</param>
    /// <param name="dbType">参数类型</param>
    /// <returns>参数</returns>
    public static DbParameter AddParameter(DbCommand cmd, string parameterName, DbType dbType = DbType.String)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.DbType = dbType;
        cmd.Parameters.Add(p);
        return p;
    }//AddParameter

    /// <summary>
    /// 在命令中添加参数，指定参数值
    /// </summary>
    /// <param name="cmd">命令</param>
    /// <param name="parameterName">参数名称</param>
    /// <param name="value">参数值</param>
    public static void AddParameter(DbCommand cmd, string parameterName, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = parameterName;
        p.Value = HandDBNull(value);
        cmd.Parameters.Add(p);
    }//AddParameter

    /// <summary>
    /// 空字符串转化为DBNull
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object HandDBNull(object value)
    {
        if (value == null)
            return DBNull.Value;
        if (value is string)
            return string.IsNullOrWhiteSpace(value.ToString()) ? (object)DBNull.Value : value.ToString().Trim();
        return value;
    } //HandleDBNull

    /// <summary>
    /// 字符串参数验证
    /// </summary>
    /// <param name="value">参数</param>
    /// <param name="canNull">是否可以为null</param>
    /// <param name="canEmpty">是否可以为empty</param>
    /// <param name="comment">注释</param>
    /// <returns>Trim</returns>
    public static string ValidString(string value, CanNull canNull, CanEmpty canEmpty, string comment)
    {
        if (value == null)
        {
            if (canNull == CanNull.True)
                return null;
            throw new ArgumentException(string.Format("[{0}]不能为null", comment));
        }
        value = value.Trim();
        if (value.Length == 0)
        {
            if (canEmpty == CanEmpty.True)
                return string.Empty;
            throw new ArgumentException(string.Format("[{0}]不能为空", comment));
        }
        return value;
    }//ValidString

    /// <summary>
    /// 日期时间参数验证
    /// </summary>
    /// <param name="value">参数</param>
    /// <param name="canNull">是否可以为null</param>
    /// <param name="canEmpty">是否可以为empty</param>
    /// <param name="format">日期时间格式</param>
    /// <param name="comment">注释</param>
    /// <returns>Trim</returns>
    public static string ValidDateTime(string value, CanNull canNull, CanEmpty canEmpty, string format, string comment)
    {
        if (value == null)
        {
            if (canNull == CanNull.True)
                return null;
            throw new ArgumentException(string.Format("[{0}]不能为null", comment));
        }
        value = value.Trim();
        if (value.Length == 0)
        {
            if (canEmpty == CanEmpty.True)
                return string.Empty;
            throw new ArgumentException(string.Format("[{0}]不能为空", comment));
        }
        DateTime tempDateTime;
        if (format == null)
        {
            if (!DateTime.TryParse(value, null, DateTimeStyles.None, out tempDateTime))
                throw new ArgumentException(string.Format("无效的[{0}]: {1}", comment, value));
        }
        else
        {
            if (!DateTime.TryParseExact(value, format, null, DateTimeStyles.None, out tempDateTime))
                throw new ArgumentException(string.Format("无效的[{0}]: {1}", comment, value));
        }
        return value;
    }//ValidDateTime

    /// <summary>
    /// 整数参数验证
    /// </summary>
    /// <param name="value">参数</param>
    /// <param name="canNull">是否可以为null</param>
    /// <param name="canEmpty">是否可以为empty</param>
    /// <param name="comment">注释</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>Trim</returns>
    public static string ValidInt(string value, CanNull canNull, CanEmpty canEmpty, string comment, int min = int.MinValue, int max = int.MaxValue)
    {
        if (value == null)
        {
            if (canNull == CanNull.True)
                return null;
            throw new ArgumentException(string.Format("[{0}]不能为null", comment));
        }
        value = value.Trim();
        if (value.Length == 0)
        {
            if (canEmpty == CanEmpty.True)
                return string.Empty;
            throw new ArgumentException(string.Format("[{0}]不能为空", comment));
        }
        int tempInt;
        if (!int.TryParse(value, out tempInt) || tempInt < min || tempInt > max)
            throw new ArgumentException(string.Format("无效的[{0}]: {1}", comment, value));
        return value;
    }//ValidInt

    /// <summary>
    /// 浮点数参数验证
    /// </summary>
    /// <param name="value">参数</param>
    /// <param name="canNull">是否可以为null</param>
    /// <param name="canEmpty">是否可以为empty</param>
    /// <param name="comment">注释</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>Trim</returns>
    public static string ValidFloat(string value, CanNull canNull, CanEmpty canEmpty, string comment, float min = float.MinValue, float max = float.MaxValue)
    {
        if (value == null)
        {
            if (canNull == CanNull.True)
                return null;
            throw new ArgumentException(string.Format("[{0}]不能为null", comment));
        }
        value = value.Trim();
        if (value.Length == 0)
        {
            if (canEmpty == CanEmpty.True)
                return string.Empty;
            throw new ArgumentException(string.Format("[{0}]不能为空", comment));
        }
        float tempFloat;
        if (!float.TryParse(value, out tempFloat) || tempFloat < min || tempFloat > max)
            throw new ArgumentException(string.Format("无效的[{0}]: {1}", comment, value));
        return value;
    }//ValidFloat

    /// <summary>
    /// 代码参数验证
    /// </summary>
    /// <param name="value">参数</param>
    /// <param name="canNull">是否可以为null</param>
    /// <param name="canEmpty">是否可以为empty</param>
    /// <param name="codes">代码集合</param>
    /// <param name="comment">注释</param>
    /// <returns>Trim</returns>
    public static string ValidCode(string value, CanNull canNull, CanEmpty canEmpty, string comment, IEnumerable<string> codes)
    {
        if (value == null)
        {
            if (canNull == CanNull.True)
                return null;
            throw new ArgumentException(string.Format("[{0}]不能为null", comment));
        }
        value = value.Trim();
        if (value.Length == 0)
        {
            if (canEmpty == CanEmpty.True)
                return string.Empty;
            throw new ArgumentException(string.Format("[{0}]不能为空", comment));
        }
        if (!codes.Contains(value))
            throw new ArgumentException(string.Format("无效的[{0}]: {1}", comment, value));
        return value;
    }//ValidCode

    /// <summary>
    /// 二进制参数验证
    /// </summary>
    /// <param name="value">参数</param>
    /// <param name="canNull">是否可以为null</param>
    /// <param name="canEmpty">是否可以为empty</param>
    /// <param name="comment"></param>
    public static void ValidBytes(byte[] value, CanNull canNull, CanEmpty canEmpty, string comment)
    {
        if (value == null)
        {
            if (canNull == CanNull.True)
                return;
            throw new ArgumentException(string.Format("[{0}]不能为null", comment));
        }
        if (value.Length == 0)
        {
            if (canEmpty == CanEmpty.True)
                return;
            throw new ArgumentException(string.Format("[{0}]不能为空", comment));
        }
    }//ValidBytes

    public static void AddSqlParameter(SqlCommand cmd, string parameterName, SqlDbType sqlDbType, object value)
    {
        if (value is string)
            value = value.ToString().Trim();
        cmd.Parameters.Add(parameterName, sqlDbType).Value = (value is string && value.ToString().Length == 0) ? Convert.DBNull : value;
    }//AddSqlParameter

    public static void BuildWhere(object value, List<string> sqlList, string parameterName, string template, SqlDbType sqlDbType, params SqlCommand[] cmds)
    {
        if (value == null)
            return;
        if (value is string)
            value = value.ToString().Trim();

        sqlList.Add(template);
        foreach (var cmd in cmds)
            if (value is string && value.ToString().Length == 0)
                cmd.Parameters.Add(parameterName, sqlDbType).Value = Convert.DBNull;
            else
                cmd.Parameters.Add(parameterName, sqlDbType).Value = value;
    }//BuildWhere

    public static void BuildInsert(object value, List<string> setList, List<string> valueList, string fieldName, string parameterName, SqlDbType sqlDbType, SqlCommand cmd)
    {
        if (value == null)
            return;
        if (value is string)
            value = value.ToString().Trim();

        setList.Add(fieldName);
        valueList.Add(parameterName);
        if (value is string && value.ToString().Length == 0)
            cmd.Parameters.Add(parameterName, sqlDbType).Value = Convert.DBNull;
        else
            cmd.Parameters.Add(parameterName, sqlDbType).Value = value;
    }//BuildInsert

    public static void BuildUpdate(object value, List<string> sqlList, string parameterName, string template, SqlDbType sqlDbType, SqlCommand cmd)
    {
        if (value == null)
            return;
        if (value is string)
            value = value.ToString().Trim();

        sqlList.Add(template);
        if (value is string && value.ToString().Length == 0)
            cmd.Parameters.Add(parameterName, sqlDbType).Value = Convert.DBNull;
        else
            cmd.Parameters.Add(parameterName, sqlDbType).Value = value;
    }//BuildUpdate

    public static string Post(string uri, string postString)
    {
        string responseString;

        var request = (HttpWebRequest)WebRequest.Create(uri);
        request.Timeout = 600000;

        // 设置POST数据
        var postByte = Encoding.UTF8.GetBytes(postString);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = postByte.Length;
        using (var stream = request.GetRequestStream())
            stream.Write(postByte, 0, postByte.Length);

        // 发送POST
        using (var response = (HttpWebResponse)request.GetResponse())
        using (var stream = response.GetResponseStream())
        {
            if (stream == null)
                throw new ArgumentException("[stream] is null");
            if (response.CharacterSet == null)
                throw new ArgumentException("[response.CharacterSet] is null");
            using (var sr = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet)))
                responseString = sr.ReadToEnd();
        }

        return responseString;
    } //Post

    public static string Get(string uri)
    {
        string responseString;

        var request = (HttpWebRequest)WebRequest.Create(uri);
        request.Timeout = 600000;

        using (var response = (HttpWebResponse)request.GetResponse())
        using (var stream = response.GetResponseStream())
        {
            if (stream == null)
                throw new ArgumentException("[stream] is null");
            if (response.CharacterSet == null)
                throw new ArgumentException("[response.CharacterSet] is null");
            using (var sr = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet)))
                responseString = sr.ReadToEnd();
        }

        return responseString;
    } //Get


}//class
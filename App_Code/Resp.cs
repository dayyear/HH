using System;
using System.Collections.Generic;

/// <summary>
///Resp 的摘要说明
/// </summary>
public class Resp
{
    public bool success = true;
    public string message = null;
    public int total = 0;
    public List<Dictionary<string, Object>> items = new List<Dictionary<string, object>>();
}

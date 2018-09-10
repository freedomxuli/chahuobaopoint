<%@ WebHandler Language="C#" Class="InterFaceHandler" %>

using System;
using System.Web;
using System.Collections;

public class InterFaceHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        string str = "";
        switch (context.Request["action"])
        {
            case "getyanzhengma":
                str = getyanzhengma(context);
                break;
        }
        context.Response.Write(str);
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

    public string getyanzhengma(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        //操作类型
        string type = context.Request["type"];
        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取验证码失败！";
        try
        {
            int usercount = new Handler().UserCount(UserName);
            if (type == "zhuce")
            {
                if (usercount > 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "用户已存在，无需重新注册！";
                }
                else
                {
                    SendMessage getyanzhenma = new SendMessage();
                    string yanzhengma = getyanzhenma.yanzhengma("查货app", "SMS_137666565", UserName);
                    if (string.IsNullOrEmpty(yanzhengma) == false)
                    {
                        if (yanzhengma.Length == 6)
                        {
                            hash["sign"] = "1";
                            hash["msg"] = "获取验证码成功！";
                            hash["yanzhengma"] = yanzhengma;
                        }
                    }
                }
            }

            if (type == "chongzhimima")
            {
                if (usercount > 0)
                {
                    SendMessage getyanzhenma = new SendMessage();
                    string yanzhengma = getyanzhenma.yanzhengma("查货app", "SMS_137666565", UserName);
                    if (string.IsNullOrEmpty(yanzhengma) == false)
                    {
                        if (yanzhengma.Length == 6)
                        {
                            hash["sign"] = "1";
                            hash["msg"] = "获取验证码成功！";
                            hash["yanzhengma"] = yanzhengma;
                        }
                    }
                }
                else
                {
                    hash["sign"] = "0";
                    hash["msg"] = "未查询到用户，密码重置失败！";
                }
            }
        }
        catch (Exception ex)
        {
            hash["sign"] = "0";
            hash["msg"] = "内部错误:" + ex.Message;
        }
        return Newtonsoft.Json.JsonConvert.SerializeObject(hash);
    }

}
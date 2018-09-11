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
            case "ZhuCe"://专线注册
                str = ZhuCe(context);
                break;
            case "ZhuCe1"://三方注册
                str = ZhuCe1(context);
                break;
            case "login_confirm"://登录
                str = login_confirm(context);
                break;
            case "ChongZhiMiMa"://重置密码
                str = ChongZhiMiMa(context);
                break;
            case "ChongZhiMiMaZF"://重置支付密码
                str = ChongZhiMiMaZF(context);
                break;
            case "tijiaoshenqing"://申请积分
                str = tijiaoshenqing(context);
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
    
    public string ZhuCe(HttpContext context)
    {

        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        string UserXM = context.Request["UserXM"];
        string FromRoute = context.Request["FromRoute"];
        string ToRoute = context.Request["ToRoute"];
        string UserPassword = context.Request["UserPassword"];
        string PayPassword = context.Request["PayPassword"];

        string type = context.Request["type"];
        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "注册失败！";
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                System.Data.DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + UserName + "'");
                if (dt_user.Rows.Count > 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户名已存在！";
                }
                else
                {
                    //用户表
                    var YHID = Guid.NewGuid().ToString();
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["UserName"] = UserName;
                    dr["Password"] = UserPassword;
                    dr["AddTime"] = DateTime.Now;
                    dr["IsSHPass"] = 1;
                    dr["Points"] = 0;
                    dr["ClientKind"] = 1;
                    //dr["Discount"] = ;
                    dr["UserXM"] = UserXM;
                    dr["UserTel"] = UserName;
                    dr["FromRoute"] = FromRoute;
                    dr["ToRoute"] = ToRoute;
                    //dr["companyId"] =;
                    dr["PayPassword"] = PayPassword;
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);

                    hash["sign"] = "1";
                    hash["msg"] = "注册成功！";
                }

                dbc.CommitTransaction();

                
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                hash["sign"] = "0";
                hash["msg"] = "内部错误:" + ex.Message;
            }
        }
        

        return Newtonsoft.Json.JsonConvert.SerializeObject(hash);
    }


    public string ZhuCe1(HttpContext context)
    {

        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        string UserPassword = context.Request["UserPassword"];
        string PayPassword = context.Request["PayPassword"];

        string type = context.Request["type"];
        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "注册失败！";
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                System.Data.DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName));
                if (dt_user.Rows.Count > 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户名已存在！";
                }
                else
                {
                    //用户表
                    var YHID = Guid.NewGuid().ToString();
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["UserName"] = UserName;
                    dr["Password"] = UserPassword;
                    dr["AddTime"] = DateTime.Now;
                    dr["IsSHPass"] = 1;
                    dr["Points"] = 0;
                    dr["ClientKind"] = 2;
                    //dr["Discount"] = ;
                    //dr["UserXM"] = UserXM;
                    dr["UserTel"] = UserName;
                    //dr["FromRoute"] = FromRoute;
                    //dr["ToRoute"] = ToRoute;
                    //dr["companyId"] =;
                    dr["PayPassword"] = PayPassword;
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);

                    hash["sign"] = "1";
                    hash["msg"] = "注册成功！";
                }
                dbc.CommitTransaction();

                
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                hash["sign"] = "0";
                hash["msg"] = "内部错误:" + ex.Message;
            }
        }


        return Newtonsoft.Json.JsonConvert.SerializeObject(hash);
    }

    public string login_confirm(HttpContext context)
    {

        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        string UserPassword = context.Request["UserPassword"];

        string type = context.Request["type"];
        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "登录失败！";
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string str = "select * from tb_b_user where UserName=@UserName and Password=@Password";
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.AddWithValue("@Password", UserPassword);
                System.Data.DataTable dt = dbc.ExecuteDataTable(cmd);
                if (dt.Rows.Count > 0)
                {
                    hash["sign"] = "1";
                    hash["msg"] = "注册成功！";
                }
                else
                {
                    hash["sign"] = "0";
                    hash["msg"] = "用户名或密码错误！";
                }
            }
            catch (Exception ex)
            {
                hash["sign"] = "0";
                hash["msg"] = "内部错误:" + ex.Message;
            }
        }


        return Newtonsoft.Json.JsonConvert.SerializeObject(hash);
    }

    public string ChongZhiMiMa(HttpContext context)
    {
        
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        string UserPassword = context.Request["UserPassword"];

        string type = context.Request["type"];
        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "重置密码失败！";
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                System.Data.DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName));
                if (dt_user.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {

                    string str = "update tb_b_user set Password=" + dbc.ToSqlValue(UserPassword) + " where UserName=" + dbc.ToSqlValue(UserName);
                    dbc.ExecuteDataTable(str);

                    hash["sign"] = "1";
                    hash["msg"] = "修改成功！";
                }
                dbc.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                hash["sign"] = "0";
                hash["msg"] = "内部错误:" + ex.Message;
            }
        }


        return Newtonsoft.Json.JsonConvert.SerializeObject(hash);
    }

    public string ChongZhiMiMaZF(HttpContext context)
    {

        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        string PayPassword = context.Request["PayPassword"];

        string type = context.Request["type"];
        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "重置支付密码失败！";
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                System.Data.DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName));
                if (dt_user.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {
                    string str = "update tb_b_user set PayPassword=" + dbc.ToSqlValue(PayPassword) + " where UserName=" + dbc.ToSqlValue(UserName);
                    dbc.ExecuteDataTable(str);

                    hash["sign"] = "1";
                    hash["msg"] = "修改成功！";
                }
                dbc.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                hash["sign"] = "0";
                hash["msg"] = "内部错误:" + ex.Message;
            }
        }
        return Newtonsoft.Json.JsonConvert.SerializeObject(hash);
    }

    public string tijiaoshenqing(HttpContext context)
    {

        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        string sqjf = context.Request["sqjf"];
        string memo = context.Request["memo"];

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "申请失败！";
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                System.Data.DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName));
                if (dt_user.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {

                    var dt = dbc.GetEmptyDataTable("tb_b_jfsq");
                    var dr = dt.NewRow();
                    dr["sqId"] = Guid.NewGuid().ToString();
                    dr["userId"] = dt_user.Rows[0]["UserID"];
                    dr["sqrq"] = DateTime.Now;
                    dr["memo"] = memo;
                    dr["sqjf"] = sqjf;
                    dr["issq"] = 0;
                    //dr["shtime"] = ;
                    //dr["shuserId"] = ;
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);

                    hash["sign"] = "1";
                    hash["msg"] = "申请成功！";
                }
                dbc.CommitTransaction();

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                hash["sign"] = "0";
                hash["msg"] = "内部错误:" + ex.Message;
            }
        }
        return Newtonsoft.Json.JsonConvert.SerializeObject(hash);
    }
}
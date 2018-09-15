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
            case "GetMyCardsList"://申请积分
                str = GetMyCardsList(context);
                break;
            case "panduanyh"://申请积分
                str = panduanyh(context);
                break;
            case "JudgeIsZX":
                str = JudgeIsZX(context);
                break;
            case "MyZXPoints":
                str = MyZXPoints(context);
                break;
            case "GivePointsToCHB":
                str = GivePointsToCHB(context);
                break;
            case "PlatSaleList":
                str = PlatSaleList(context);
                break;
            case "GetPayRecordList":
                str = GetPayRecordList(context);
                break;
            case "GetPlatToSaleDetail":
                str = GetPlatToSaleDetail(context);
                break;
            case "OrderPlat":
                str = OrderPlat(context);
                break;
            case "GetOrderDetail":
                str = GetOrderDetail(context);
                break;
            case "PayPointsList":
                str = PayPointsList(context);
                break;
            case "JudgeTelByPayPoints":
                str = JudgeTelByPayPoints(context);
                break;
            case "MyOrderList":
                str = MyOrderList(context);
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
                    if (Convert.ToInt32(dt_user.Rows[0]["ClientKind"]) != 1)
                    {
                        hash["sign"] = "0";
                        hash["msg"] = "对不起，您的账号无法申请积分！";
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

    public string GetMyCardsList(HttpContext context)
    {

        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);

        int cp = Convert.ToInt32(context.Request["pagnum"]);
        int pagesize=Convert.ToInt32(context.Request["pagesize"]);
        int ac = 0;
        
        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";
        
        hash["value"] = new object();
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string str = "select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName);
                System.Data.DataTable udt = dbc.ExecuteDataTable(str);
                if (udt.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {
                    str = @"select a.*,b.UserName as syr,c.UserXM as zxmc from tb_b_mycard a left join tb_b_user b  on a.UserID=b.UserID
                            left join tb_b_user c on a.CardUserID=c.UserID
                             where a.UserID='" + udt.Rows[0]["UserID"] + "' and a.status=0 order by a.points";
                    System.Data.DataTable dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);

                    hash["sign"] = "1";
                    hash["msg"] = "获取成功！";
                    hash["value"] = new  { dt = dtPage, cp = cp, ac = ac };
                    
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

    public string panduanyh(HttpContext context)
    {

        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";

        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string str = "select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName);
                System.Data.DataTable udt = dbc.ExecuteDataTable(str);
                if (udt.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {
                    if (Convert.ToInt32(udt.Rows[0]["ClientKind"]) == 1)
                    {
                        hash["sign"] = "1";
                        hash["msg"] = "请申请积分";
                    }
                    else
                    {
                        hash["sign"] = "0";
                        hash["msg"] = "对不起，您的账号无法申请积分！";
                    }
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

    public string JudgeIsZX(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";

        using (SmartFramework4v2.Data.SqlServer.DBConnection db = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string sql = "select count(*) num from tb_b_user where UserName = " + db.ToSqlValue(UserName) + " and ClientKind = 1";
                int num = Convert.ToInt32(db.ExecuteScalar(sql));
                if (num > 0)
                {
                    hash["sign"] = "1";
                    hash["msg"] = "获取成功！";
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

    public string MyZXPoints(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";

        using (SmartFramework4v2.Data.SqlServer.DBConnection db = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string sql = "select count(*) num from tb_b_user where UserName = " + db.ToSqlValue(UserName) + " and ClientKind = 1";
                int num = Convert.ToInt32(db.ExecuteScalar(sql));
                sql = "select Points from tb_b_user where UserName = " + db.ToSqlValue(UserName) + " and ClientKind = 1";
                System.Data.DataTable dt = db.ExecuteDataTable(sql);
                if (num > 0)
                {
                    hash["sign"] = "1";
                    hash["msg"] = "获取成功！";
                    hash["points"] = dt.Rows[0]["Points"].ToString();
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

    public string GivePointsToCHB(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        string Points = context.Request["Points"];
        string ReceiveUser = context.Request["ReceiveUser"];
        string PayPassword = context.Request["PayPassword"];

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "支付失败！";

        using (SmartFramework4v2.Data.SqlServer.DBConnection db = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                db.BeginTransaction();

                if (ReceiveUser == "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9")
                {
                    string sql = "select Points,PayPassword,ClientKind,UserID from tb_b_user where UserName = " + db.ToSqlValue(UserName);
                    System.Data.DataTable dt = db.ExecuteDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["ClientKind"].ToString() == "1")
                        {
                            if (dt.Rows[0]["PayPassword"].ToString() == PayPassword)
                            {
                                if (Convert.ToInt32(dt.Rows[0]["Points"].ToString()) >= Convert.ToInt32(Points))
                                {
                                    System.Data.DataTable dt_new = db.GetEmptyDataTable("tb_b_platpoints");
                                    SmartFramework4v2.Data.DataTableTracker dtt = new SmartFramework4v2.Data.DataTableTracker(dt_new);
                                    
                                    sql = "select PlatPointId,Points from tb_b_platpoints where UserID = " + db.ToSqlValue(dt.Rows[0]["UserID"].ToString());
                                    System.Data.DataTable dt_points = db.ExecuteDataTable(sql);
                                    
                                    System.Data.DataRow dr = dt_new.NewRow();
                                    if (dt_points.Rows.Count > 0)
                                        dr["PlatPointId"] = dt_points.Rows[0]["PlatPointId"].ToString();
                                    else
                                        dr["PlatPointId"] = Guid.NewGuid();
                                    dr["companyId"] = "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9";
                                    dr["UserID"] = dt.Rows[0]["UserID"].ToString();
                                    if (dt_points.Rows.Count > 0)
                                        dr["Points"] = Convert.ToInt32(dt_points.Rows[0]["Points"].ToString()) + Convert.ToInt32(Points);
                                    else
                                        dr["Points"] = Convert.ToInt32(Points);
                                    dr["status"] = 0;
                                    dt_new.Rows.Add(dr);
                                    if (dt_points.Rows.Count > 0)
                                        db.UpdateTable(dt_new, dtt);
                                    else
                                        db.InsertTable(dt_new);

                                    System.Data.DataTable dt_new1 = db.GetEmptyDataTable("tb_b_user");
                                    SmartFramework4v2.Data.DataTableTracker dtt1 = new SmartFramework4v2.Data.DataTableTracker(dt_new1);
                                    System.Data.DataRow dr1 = dt_new1.NewRow();
                                    dr1["UserID"] = dt.Rows[0]["UserID"].ToString();
                                    dr1["Points"] = Convert.ToInt32(dt.Rows[0]["Points"].ToString()) - Convert.ToInt32(Points);
                                    dt_new1.Rows.Add(dr1);
                                    db.UpdateTable(dt_new1, dtt1);
                                    
                                    hash["sign"] = "1";
                                    hash["msg"] = "支付成功！";
                                }
                                else
                                {
                                    hash["sign"] = "5";
                                    hash["msg"] = "您输入的积分数不足，支付失败！";
                                }
                            }
                            else
                            {
                                hash["sign"] = "4";
                                hash["msg"] = "您输入的支付密码错误，支付失败！";
                            }
                        }
                        else
                        {
                            hash["sign"] = "3";
                            hash["msg"] = "你不是专线用户，不可扫查货宝平台二维码！";
                        }
                    }
                    else
                    {
                        hash["sign"] = "6";
                        hash["msg"] = "警报！你是非法使用者！";
                    }
                }
                else
                {
                    hash["sign"] = "2";
                    hash["msg"] = "二维码不是查货宝平台！请确认！";
                }
                
                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                db.RoolbackTransaction();
                hash["sign"] = "0";
                hash["msg"] = "内部错误:" + ex.Message;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(hash);
        }
    }

    public string GetPayRecordList(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);

        int cp = Convert.ToInt32(context.Request["pagnum"]);
        int pagesize = Convert.ToInt32(context.Request["pagesize"]);
        int ac = 0;

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";

        hash["value"] = new object();
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string str = "select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName);
                System.Data.DataTable udt = dbc.ExecuteDataTable(str);
                if (udt.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {
                    str = @"select b.UserXM as wuliu,c.UserName as jydx,a.AddTime,a.Points from tb_b_pay a left join tb_b_user b on a.CardUserID=b.UserID
                             left join tb_b_user c on a.ReceiveUserID=c.UserID where PayUserID='" + udt.Rows[0]["UserID"] + "' order by AddTime desc";
                    System.Data.DataTable dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);

                    hash["sign"] = "1";
                    hash["msg"] = "获取成功！";
                    hash["value"] = new { dt = dtPage, cp = cp, ac = ac };

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

    public string PlatSaleList(HttpContext context)
    {

        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);

        int cp = Convert.ToInt32(context.Request["pagnum"]);
        int pagesize = Convert.ToInt32(context.Request["pagesize"]);
        int ac = 0;

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";

        hash["value"] = new object();
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string str = "select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName);
                System.Data.DataTable udt = dbc.ExecuteDataTable(str);
                if (udt.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {
                    str = @"select a.*,b.UserXM from tb_b_plattosale a left join tb_b_user b on a.UserID=b.UserID
                             where a.status=0 and a.points > 0 order by a.addtime desc";
                    System.Data.DataTable dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);

                    hash["sign"] = "1";
                    hash["msg"] = "获取成功！";
                    hash["value"] = new { dt = dtPage, cp = cp, ac = ac };

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

    public string GetPlatToSaleDetail(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        var PlatToSaleId = context.Request["PlatToSaleId"];
        
        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";

        hash["value"] = new object();
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string str = "select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName);
                System.Data.DataTable udt = dbc.ExecuteDataTable(str);
                if (udt.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {
                    str = @"select a.*,b.UserXM from tb_b_plattosale a left join tb_b_user b on a.UserID=b.UserID
                             where a.status=0 and a.points > 0 and a.PlatToSaleId = " + dbc.ToSqlValue(PlatToSaleId);
                    System.Data.DataTable dt = dbc.ExecuteDataTable(str);

                    if (dt.Rows.Count > 0)
                    {
                        hash["sign"] = "1";
                        hash["msg"] = "获取成功！";
                        hash["dt"] = dt;
                    }
                    else
                    {
                        hash["sign"] = "2";
                        hash["msg"] = "电子券已被抢空！";
                    }
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

    public string OrderPlat(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        string PlatToSaleId = context.Request["PlatToSaleId"];
        string Points = context.Request["Points"];

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "下单失败！";

        using (SmartFramework4v2.Data.SqlServer.DBConnection db = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                db.BeginTransaction();
                string str = "select * from tb_b_user where UserName=" + db.ToSqlValue(UserName);
                System.Data.DataTable udt = db.ExecuteDataTable(str);
                if (udt.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {
                    str = @"select a.*,b.UserXM from tb_b_plattosale a left join tb_b_user b on a.UserID=b.UserID
                             where a.status=0 and a.points > 0 and a.PlatToSaleId = " + db.ToSqlValue(PlatToSaleId);
                    System.Data.DataTable dt = db.ExecuteDataTable(str);

                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt.Rows[0]["points"].ToString()) >= Convert.ToInt32(Points))
                        {
                            System.Data.DataTable dt_new = db.GetEmptyDataTable("tb_b_order");
                            System.Data.DataRow dr = dt_new.NewRow();
                            dr["OrderID"] = Guid.NewGuid();
                            dr["OrderCode"] = "001" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            dr["BuyUserID"] = udt.Rows[0]["UserID"];
                            dr["SaleUserID"] = dt.Rows[0]["UserID"];
                            dr["Points"] = Points;
                            dr["Money"] = Math.Ceiling(Convert.ToDecimal(dt.Rows[0]["discount"]) * Convert.ToInt32(Points));
                            dr["AddTime"] = DateTime.Now;
                            dr["Status"] = 0;
                            dr["ZhiFuZT"] = 0;
                            dr["PlatToSaleId"] = PlatToSaleId;
                            dt_new.Rows.Add(dr);
                            db.InsertTable(dt_new);

                            System.Data.DataTable dt_new1 = db.GetEmptyDataTable("tb_b_plattosale");
                            SmartFramework4v2.Data.DataTableTracker dtt_new1 = new SmartFramework4v2.Data.DataTableTracker(dt_new1);
                            System.Data.DataRow dr1 = dt_new1.NewRow();
                            dr1["PlatToSaleId"] = PlatToSaleId;
                            dr1["points"] = Convert.ToInt32(dt.Rows[0]["points"].ToString()) - Convert.ToInt32(Points);
                            dt_new1.Rows.Add(dr1);
                            db.UpdateTable(dt_new1, dtt_new1);
                            
                            hash["sign"] = "1";
                            hash["msg"] = "下单成功！";
                            hash["OrderID"] = dr["OrderID"];
                        }
                        else
                        {
                            hash["sign"] = "3";
                            hash["msg"] = "你想购买的电子券已超过购买量，请重新确认购买量！";
                        }
                    }
                    else
                    {
                        hash["sign"] = "2";
                        hash["msg"] = "电子券已被抢空！";
                    }
                }
                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                db.RoolbackTransaction();
                hash["sign"] = "0";
                hash["msg"] = "内部错误:" + ex.Message;
            }
        }
        return Newtonsoft.Json.JsonConvert.SerializeObject(hash);
    }

    public string GetOrderDetail(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);
        var OrderID = context.Request["OrderID"];

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";

        hash["value"] = new object();
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string str = "select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName);
                System.Data.DataTable udt = dbc.ExecuteDataTable(str);
                if (udt.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {
                    str = @"select a.*,b.UserXM from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID
                             where a.status=0 and a.OrderID = " + dbc.ToSqlValue(OrderID);
                    System.Data.DataTable dt = dbc.ExecuteDataTable(str);

                    if (dt.Rows.Count > 0)
                    {
                        hash["sign"] = "1";
                        hash["msg"] = "获取成功！";
                        hash["dt"] = dt;
                    }
                    else
                    {
                        hash["sign"] = "2";
                        hash["msg"] = "电子券已被抢空！";
                    }
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

    public string PayPointsList(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        string PayPassword = context.Request["PayPassword"];
        string ReceiveUser = context.Request["ReceiveUser"];
        string CardUserID = context.Request["CardUserID"];
        string Points = context.Request["Points"];

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";
        using (SmartFramework4v2.Data.SqlServer.DBConnection db = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                db.BeginTransaction();

                string sql = @"select a.PayPassword,a.UserID,b.points,b.mycardId from tb_b_user a left join tb_b_mycard b on a.UserID=b.UserID
                                where b.status=0 and b.CardUserID=" + db.ToSqlValue(CardUserID) + " and a.UserName=" + db.ToSqlValue(UserName);
                    System.Data.DataTable dt = db.ExecuteDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0]["mycardId"].ToString()))
                        {
                            string str = "select * from tb_b_user where UserName<>" + db.ToSqlValue(UserName) + "and  UserName=" + db.ToSqlValue(ReceiveUser);
                            System.Data.DataTable rdt = db.ExecuteDataTable(str);
                            if (rdt.Rows.Count > 0)
                            {
                                if (dt.Rows[0]["PayPassword"].ToString() == PayPassword)
                                {
                                    if (Convert.ToInt32(dt.Rows[0]["Points"].ToString()) >= Convert.ToInt32(Points))
                                    {
                                        System.Data.DataTable paydt = db.GetEmptyDataTable("tb_b_pay");
                                        var paydr = paydt.NewRow();
                                        paydr["PayID"] = Guid.NewGuid();
                                        paydr["PayUserID"] = dt.Rows[0]["UserID"].ToString();
                                        paydr["ReceiveUserID"] = rdt.Rows[0]["UserID"].ToString();
                                        paydr["Points"] = Convert.ToDecimal(Points);
                                        paydr["AddTime"] = DateTime.Now;
                                        //paydr["Memo"] =
                                        paydr["CardUserID"] = CardUserID;
                                        paydt.Rows.Add(paydr);
                                        db.InsertTable(paydt);

                                        //我的账户
                                        if (Convert.ToInt32(dt.Rows[0]["Points"].ToString()) == Convert.ToInt32(Points))
                                        {
                                            System.Data.DataTable deldt = db.GetEmptyDataTable("tb_b_mycard");
                                            SmartFramework4v2.Data.DataTableTracker deldtt = new SmartFramework4v2.Data.DataTableTracker(deldt);
                                            var deldr = deldt.NewRow();
                                            deldr["mycardId"] = dt.Rows[0]["mycardId"].ToString();
                                            deldr["status"] = 1;
                                            deldt.Rows.Add(deldr);
                                            db.UpdateTable(deldt, deldtt);
                                        }
                                        else
                                        {
                                            System.Data.DataTable cdt = db.GetEmptyDataTable("tb_b_mycard");
                                            SmartFramework4v2.Data.DataTableTracker cdtt = new SmartFramework4v2.Data.DataTableTracker(cdt);
                                            var cdr = cdt.NewRow();
                                            cdr["mycardId"] = dt.Rows[0]["mycardId"].ToString();
                                            cdr["points"] = Convert.ToDecimal(Convert.ToInt32(dt.Rows[0]["Points"].ToString()) - Convert.ToInt32(Points));
                                            cdt.Rows.Add(cdr);
                                            db.UpdateTable(cdt, cdtt);
                                        }

                                        if (rdt.Rows[0]["ClientKind"].ToString() == "1")
                                        {
                                            //专线
                                            System.Data.DataTable zudt = db.GetEmptyDataTable("tb_b_user");
                                            SmartFramework4v2.Data.DataTableTracker zudtt = new SmartFramework4v2.Data.DataTableTracker(zudt);
                                            var zudr = zudt.NewRow();
                                            zudr["UserID"] = rdt.Rows[0]["UserID"].ToString();
                                            zudr["Points"] = Convert.ToDecimal(Convert.ToInt32(rdt.Rows[0]["Points"].ToString()) + Convert.ToInt32(Points));
                                            zudt.Rows.Add(zudr);
                                            db.UpdateTable(zudt, zudtt);
                                        }
                                        else
                                        {
                                            //三方
                                            var rmcardstr = "select * from tb_b_mycard where UserID=" + db.ToSqlValue(rdt.Rows[0]["UserID"].ToString()) + " and CardUserID=" + db.ToSqlValue(CardUserID) + " and status=0";
                                            System.Data.DataTable rmcarddt = db.ExecuteDataTable(rmcardstr);
                                            if (rmcarddt.Rows.Count > 0)
                                            {
                                                System.Data.DataTable rmdt = db.GetEmptyDataTable("tb_b_mycard");
                                                SmartFramework4v2.Data.DataTableTracker rmdtt = new SmartFramework4v2.Data.DataTableTracker(rmdt);
                                                var rmdr = rmdt.NewRow();
                                                rmdr["mycardId"] = rmcarddt.Rows[0]["mycardId"].ToString();
                                                rmdr["points"] = Convert.ToDecimal(Convert.ToInt32(dt.Rows[0]["Points"].ToString()) + Convert.ToInt32(Points));
                                                rmdt.Rows.Add(rmdr);
                                                db.UpdateTable(rmdt, rmdtt);
                                            }
                                            else
                                            {
                                                System.Data.DataTable rmidt = db.GetEmptyDataTable("tb_b_mycard");
                                                var rmidr = rmidt.NewRow();
                                                rmidr["mycardId"] = Guid.NewGuid();
                                                rmidr["points"] = Convert.ToDecimal(Points);
                                                rmidr["UserID"] = rdt.Rows[0]["UserID"].ToString();
                                                rmidr["CardUserID"] = CardUserID;
                                                rmidr["status"] = 0;
                                                rmidt.Rows.Add(rmidr);
                                                db.InsertTable(rmidt);
                                            }
                                        }
                                        hash["sign"] = "1";
                                        hash["msg"] = "支付成功！";
                                    }
                                    else
                                    {
                                        hash["sign"] = "5";
                                        hash["msg"] = "您输入的积分数不足，支付失败！";
                                    }
                                }
                                else
                                {
                                    hash["sign"] = "4";
                                    hash["msg"] = "您输入的支付密码错误，支付失败！";
                                }
                            }
                            else
                            {
                                hash["sign"] = "3";
                                hash["msg"] = "请扫描有效二维码！";
                            }
                        }
                        else {
                            hash["sign"] = "7";
                            hash["msg"] = "你没有该用户卡包！";
                        }
                    }
                    else
                    {
                        hash["sign"] = "6";
                        hash["msg"] = "警报！你是非法使用者！";
                    }
                
            

                db.CommitTransaction();
            }

            catch (Exception ex)
            {
                db.RoolbackTransaction();
                hash["sign"] = "0";
                hash["msg"] = "内部错误:" + ex.Message;
            }
        }
        return Newtonsoft.Json.JsonConvert.SerializeObject(hash);
    }

    public string JudgeTelByPayPoints(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        string ReceiveUser = context.Request["ReceiveUser"];

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";

        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string str = "select * from tb_b_user where UserName<>" + dbc.ToSqlValue(UserName) + "and  UserName=" + dbc.ToSqlValue(ReceiveUser);
                System.Data.DataTable udt = dbc.ExecuteDataTable(str);

                if (udt.Rows.Count > 0)
                {
                    hash["sign"] = "1";
                    hash["msg"] = "有效二维码！";
                }
                else {
                    hash["sign"] = "0";
                    hash["msg"] = "请扫描有效二维码！";
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

    public string MyOrderList(HttpContext context)
    {

        context.Response.ContentType = "text/plain";
        //用户名
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        string UserName = context.Request["UserName"];
        UserName = HttpUtility.UrlDecode(UserName.ToUpper(), utf8);

        int cp = Convert.ToInt32(context.Request["pagnum"]);
        int pagesize = Convert.ToInt32(context.Request["pagesize"]);
        int ac = 0;

        Hashtable hash = new Hashtable();
        hash["sign"] = "0";
        hash["msg"] = "获取失败！";

        hash["value"] = new object();
        using (SmartFramework4v2.Data.SqlServer.DBConnection dbc = new SmartFramework4v2.Data.SqlServer.DBConnection())
        {
            try
            {
                string str = "select * from tb_b_user where UserName=" + dbc.ToSqlValue(UserName);
                System.Data.DataTable udt = dbc.ExecuteDataTable(str);
                if (udt.Rows.Count == 0)
                {
                    hash["sign"] = "0";
                    hash["msg"] = "该用户不存在，请注册！";
                }
                else
                {
                    str = @"select a.*,b.UserXM from tb_b_order a left join tb_b_user b on a.BuyUserID=b.UserID
                             where a.ZhiFuZT<>1 and a.status=0 and a.BuyUserID = " + dbc.ToSqlValue(udt.Rows[0]["UserID"].ToString()) + " order by a.addtime desc";
                    System.Data.DataTable dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);

                    hash["sign"] = "1";
                    hash["msg"] = "获取成功！";
                    hash["value"] = new { dt = dtPage, cp = cp, ac = ac };

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
}
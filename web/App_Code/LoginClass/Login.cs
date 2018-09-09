using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartFramework4v2.Web.WebExecutor;
using System.Data.SqlClient;
using SmartFramework4v2.Data.SqlServer;
using System.Text;


/// <summary>
///Login 的摘要说明
/// </summary>
[CSClass("UserClass")]
public class UserClass
{
    [CSMethod("Login")]
    public bool Login(string username, string password, string captcha)
    {
        if (username.Trim() == string.Empty)
        {
            throw new Exception("用户名不能为空！");
        }
        if (password.Trim() == string.Empty)
        {
            throw new Exception("密码不能为空！");

        }
        if (captcha.Trim() == string.Empty)
        {
            throw new Exception("验证码不能为空！");
        }
        if (!VerCode.CheckCode("log", captcha))
        {
            throw new Exception("验证码验证错误！");
        }
        
        var su = SystemUser.Login(username, password);
        if (su != null)
        {
            HttpCookie cookie = new HttpCookie("login_Username", username)
            {
                Expires = DateTime.Now.AddYears(1)
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
            return true;
        }
        else
        {
            return false;
        }
    }
    [CSMethod("Logout")]
    public bool QuitSystem()
    {
        try
        {
            SystemUser.Logout();
        }
        catch
        {
        }
        return true;
    }
}

/// <summary>
/// 手机登陆login
/// </summary>
[CSClass("UserSJClass")]
public class UserSJClass
{
    [CSMethod("Login")]
    public string Login(string username, string password)
    {
        if (username.Trim() == string.Empty)
        {
            throw new Exception("用户名不能为空！");
        }
        if (password.Trim() == string.Empty)
        {
            throw new Exception("密码不能为空！");
        }
        var su = SystemUser.Login(username, password);
        if (su != null)
        {
            HttpCookie cookie = new HttpCookie("login_Username", username)
            {
                Expires = DateTime.Now.AddYears(1)
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
            return su.UserID.ToString();
        }
        else
        {
            return null;
        }
    }


    [CSMethod("Getauthority")]
    public DataTable Getauthority(string userId)
    {
        using (DBConnection db = new DBConnection())
        {
            try
            {
                Guid use = new Guid(userId);
                string sql = "select JS_ID from tb_b_User_JS_Gl where delflag =  0 and User_ID = @User_ID order by updatetime";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@User_ID", use);
                DataTable dt = db.ExecuteDataTable(cmd);
                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    [CSMethod("Logout")]
    public bool QuitSystem()
    {
        try
        {
            SystemUser.Logout();
        }
        catch
        {
        }
        return true;
    }
}

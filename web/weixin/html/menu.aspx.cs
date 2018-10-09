using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using WxPayAPI;
using SmartFramework4v2.Data.SqlServer;
using System.Data.SqlClient;
using SmartFramework4v2.Data;

public partial class weixin_html_menu : System.Web.UI.Page
{
    public string openid;
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            JsApiPay jsApiPay = new JsApiPay(this);
            try
            {
                //调用【网页授权获取用户信息】接口获取用户的openid和access_token
                jsApiPay.GetOpenidAndAccessToken();

                //获取收货地址js函数入口参数
                openid = jsApiPay.openid;
                HttpContext.Current.Response.Cookies.Add(new HttpCookie("openid", openid) { HttpOnly = true });
                using(var db = new DBConnection())
                {
                    //Response.Write("<span style='color:#FF0000;font-size:20px'>" + "您的id为" + HttpContext.Current.Request.Cookies["userid"].Value + "，微信标识为：" + openid + "</span>");
                    DataTable dt = db.GetEmptyDataTable("tb_b_user");
                    DataTableTracker dtt = new DataTableTracker(dt);
                    DataRow dr = dt.NewRow();
                    dr["UserID"] = HttpContext.Current.Request.Cookies["userid"].Value;
                    dr["OpenID"] = openid;
                    dt.Rows.Add(dr);
                    db.UpdateTable(dt, dtt);
                }
                //new Handler().SendWeText(HttpContext.Current.Request.Cookies["openid"].Value, "欢迎登陆222");
            }
            catch (Exception ex)
            {
                //Response.Write("<span style='color:#FF0000;font-size:20px'>" + "页面加载出错，请重试" + ex + "</span>");
            }
        }
    }
}
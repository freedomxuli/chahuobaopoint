using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WxPayAPI;

public partial class weixin_html_BuyPointsStart : System.Web.UI.Page
{
    public static string wxEditAddrParam { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            JsApiPay jsApiPay = new JsApiPay(this);
            try
            {
                //调用【网页授权获取用户信息】接口获取用户的openid和access_token
                jsApiPay.GetOpenidAndAccessToken();

                //获取收货地址js函数入口参数
                wxEditAddrParam = jsApiPay.GetEditAddressParameters();
                ViewState["openid"] = jsApiPay.openid;
            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:#FF0000;font-size:20px'>" + "页面加载出错，请重试</span>");
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        string total_fee = totalfee.Text;
        string code = ordercode.Text;
        if (ViewState["openid"] != null)
        {
            string openid = ViewState["openid"].ToString();
            string url = "http://wx.chahuobao.net/weixin/html/JsApiPayPage.aspx?openid=" + openid + "&total_fee=" + total_fee + "&orderid=" + code;
            Response.Redirect(url);
        }
        else
        {
            Response.Write("<span style='color:#FF0000;font-size:20px'>" + "页面缺少参数，请返回重试" + "</span>");
        }
    }
}
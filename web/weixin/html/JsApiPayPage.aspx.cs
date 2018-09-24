using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WxPayAPI;
using SmartFramework4v2.Data.SqlServer;

public partial class weixin_html_JsApiPayPage : System.Web.UI.Page
{
    public static string wxJsApiParam { get; set; } //H5调起JS API参数
    public string total_fee_yuan = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        Log.Info(this.GetType().ToString(), "page load");
        if (!IsPostBack)
        {
            string openid = Request.QueryString["openid"];
            string orderid = Request.QueryString["orderid"];
            total_fee_yuan = Request.QueryString["total_fee"];
            string total_fee = (100 * Convert.ToInt32(total_fee_yuan)).ToString();
            //检测是否给当前页面传递了相关参数
            if (string.IsNullOrEmpty(openid) || string.IsNullOrEmpty(total_fee))
            {
                Response.Write("<span style='color:#FF0000;font-size:20px'>" + "页面传参出错,请返回重试" + "</span>");
                Log.Error(this.GetType().ToString(), "This page have not get params, cannot be inited, exit...");
                submit.Visible = false;
                return;
            }

            using (var db = new DBConnection())
            {
                string sql = "select count(OrderID) num from tb_b_order where Status = 0 and ZhiFuZT = 0 and OrderCode =" + db.ToSqlValue(orderid);
                int num = Convert.ToInt32(db.ExecuteScalar(sql).ToString());
                if (num > 0)
                {
                    //若传递了相关参数，则调统一下单接口，获得后续相关接口的入口参数
                    JsApiPay jsApiPay = new JsApiPay(this);
                    jsApiPay.openid = openid;
                    jsApiPay.total_fee = int.Parse(total_fee);
                    jsApiPay.out_trade_no = orderid;

                    //JSAPI支付预处理
                    try
                    {
                        WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult();
                        wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    
                        Log.Debug(this.GetType().ToString(), "wxJsApiParam : " + wxJsApiParam);
                        //在页面上显示订单信息
                        //Response.Write("<span style='color:#00CD00;font-size:20px'>" + orderid + "</span><br/>");
                        //Response.Write("<span style='color:#00CD00;font-size:20px'>" + unifiedOrderResult.ToPrintStr() + "</span>");

                    }
                    catch (Exception ex)
                    {
                        Response.Write("<span style='color:#FF0000;font-size:20px'>" + "订单已失效，请返回下单" + "</span>");
                        submit.Visible = false;
                    }
                }
                else
                {
                    Response.Write("<span style='color:#FF0000;font-size:20px'>" + "下单失败，请返回重试" + "</span>");
                    submit.Visible = false;
                }
            }
        }
        else
        {
            Response.Redirect("http://wx.chahuobao.net/weixin/html/MyOrders.html");
        }
    }
}
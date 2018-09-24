using SmartFramework4v2.Data.SqlServer;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace WxPayAPI
{
    /// <summary>
    /// 支付结果通知回调处理类
    /// 负责接收微信支付后台发送的支付结果并对订单有效性进行验证，将验证结果反馈给微信支付后台
    /// </summary>
    public class ResultNotify:Notify
    {
        public ResultNotify(Page page):base(page)
        {
        }

        public override void ProcessNotify()
        {
            WxPayData notifyData = GetNotifyData();

            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                page.Response.Write(res.ToXml());
                page.Response.End();
            }

            string transaction_id = notifyData.GetValue("transaction_id").ToString();

            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                page.Response.Write(res.ToXml());
                page.Response.End();
            }
            //查询订单成功
            else
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());

                using (var db = new DBConnection())
                {
                    string ordercode = notifyData.GetValue("attach").ToString();
                    string sql = "select * from tb_b_order where OrderCode =" + db.ToSqlValue(ordercode);
                    DataTable dt = db.ExecuteDataTable(sql);

                    sql = "update tb_b_order set ZhiFuZT = 1 where OrderCode =" + db.ToSqlValue(ordercode);
                    db.ExecuteNonQuery(sql);

                    sql = "select * from tb_b_mycard where status = 0 and CardUserID = '" + dt.Rows[0]["SaleUserID"].ToString() + "' and UserID = '" + dt.Rows[0]["BuyUserID"].ToString() + "'";
                    DataTable dt_card = db.ExecuteDataTable(sql);

                    DataTable dt_new = db.GetEmptyDataTable("tb_b_mycard");
                    SmartFramework4v2.Data.DataTableTracker dtt_new = new SmartFramework4v2.Data.DataTableTracker(dt_new);

                    DataRow dr = dt_new.NewRow();
                    if(dt_card.Rows.Count == 0)
                        dr["mycardId"] = Guid.NewGuid();
                    else
                        dr["mycardId"] = dt_card.Rows[0]["mycardId"].ToString();
                    if (dt_card.Rows.Count == 0)
                        dr["points"] = Convert.ToDecimal(dt.Rows[0]["Points"]);
                    else
                        dr["points"] = Convert.ToDecimal(Convert.ToInt32(dt_card.Rows[0]["points"]) + Convert.ToInt32(dt.Rows[0]["Points"]));
                    dr["UserID"] = dt.Rows[0]["BuyUserID"];
                    dr["CardUserID"] = dt.Rows[0]["SaleUserID"];
                    dr["status"] = 0;
                    dt_new.Rows.Add(dr);
                    if (dt_card.Rows.Count == 0)
                        db.InsertTable(dt_new);
                    else
                        db.UpdateTable(dt_new, dtt_new);
                }
                page.Response.Write(res.ToXml());
                //page.Response.Redirect("http://wx.chahuobao.net/weixin/html/menu.html");
                //page.Response.Write("<script>window.open('http://wx.chahuobao.net/weixin/html/menu.html');</script>");
                page.Response.End();
            }
        }

        //查询订单
        private bool QueryOrder(string transaction_id)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = WxPayApi.OrderQuery(req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
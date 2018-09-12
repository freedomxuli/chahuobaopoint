using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Helpers;

public partial class weixin_html_PayPoints : System.Web.UI.Page
{
    public string appID;
    public string timestamp;
    public string nonceStr;
    public string signature;
    protected void Page_Load(object sender, EventArgs e)
    {
        string ticket = string.Empty;
        timestamp = JSSDKHelper.GetTimestamp();
        nonceStr = JSSDKHelper.GetNoncestr();
        appID = "wx422044a7a4be9609";
        ticket = JsApiTicketContainer.GetJsApiTicket(appID);
        signature = JSSDKHelper.GetSignature(ticket, nonceStr, timestamp, Request.Url.AbsoluteUri.ToString());
    }
}
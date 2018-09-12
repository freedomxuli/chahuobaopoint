using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP;
using System.IO;
using log4net;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP.CommonAPIs;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", ConfigFileExtension = "log4net", Watch = true)]
public partial class GetMessage : System.Web.UI.Page
{
    private readonly string Token = "08efecf60208e6909df002";//定义一个局部变量不可以被修改，这里定义的变量要与接口配置信息中填写的Token一致  
    protected void Page_Load(object sender, EventArgs e)
    {
        log4net.LogManager.GetLogger("loginfo").Info("可以访问！");

        string signature = Request["signature"];
        string timestamp = Request["timestamp"];
        string nonce = Request["nonce"];
        string echostr = Request["echostr"];

        if (Request.HttpMethod == "GET")
        {
            //get method - 仅在微信后台填写URL验证时触发
            if (CheckSignature.Check(signature, timestamp, nonce, Token))
            {
                WriteContent(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                WriteContent("failed:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, Token) + "。" +
                            "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
            Response.End();
        }
        else
        {
            log4net.LogManager.GetLogger("loginfo").Info("开始获取accesstoken值");
            var accessToken = AccessTokenContainer.TryGetAccessToken("wx422044a7a4be9609", "a12eca08b53a58d3806c6226efbb1364");//appid,appsecret
            log4net.LogManager.GetLogger("loginfo").Info("已获取accesstoken");
            log4net.LogManager.GetLogger("loginfo").Info("开始创建菜单");

            ButtonGroup bg = new ButtonGroup();
            bg.button.Add(new SingleViewButton()
            {
                name = "自由查单",
                url = "http://client.chahuobao.net/manager/WxQuery"
            });

            //二级菜单
            var subButton = new SubButton()
            {
                name = "微服务"
            };
            subButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://www.wuliubaoxianpingtai.com/weixin/",
                name = "关于我们"
            });
            subButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://a.app.qq.com/o/simple.jsp?pkgname=io.dcloud.H5A57DD98",
                name = "软件下载"
            });

            bg.button.Add(subButton);

            bg.button.Add(new SingleViewButton()
            {
                name = "电子券",
                url = "http://wx.chahuobao.net/weixin/html/MGps_login.html"
            });

            var result = CommonApi.CreateMenu(accessToken, bg);
            log4net.LogManager.GetLogger("loginfo").Info(result);
            log4net.LogManager.GetLogger("loginfo").Info("菜单创建完毕");

            //post method - 当有用户向公众账号发送消息时触发
            if (!CheckSignature.Check(signature, timestamp, nonce, Token))
            {
                WriteContent("参数错误！");
                return;
            }

            //post method - 当有用户想公众账号发送消息时触发
            //var postModel = new PostModel()
            //{
            //    Signature = Request.QueryString["signature"],
            //    Msg_Signature = Request.QueryString["msg_signature"],
            //    Timestamp = Request.QueryString["timestamp"],
            //    Nonce = Request.QueryString["nonce"],
            //    //以下保密信息不会（不应该）在网络上传播，请注意
            //    Token = Token,
            //    EncodingAESKey = "OOUa8906bBKb8l6aaMJTKyM5iIFJcI5PZkpWXZtIbNq",//根据自己后台的设置保持一致
            //    AppId = "wx422044a7a4be9609"//根据自己后台的设置保持一致
            //};

            //var maxRecordCount = 10;

            ////自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            //var messageHandler = new CustomMessageHandler(Request.InputStream, postModel);

            //try
            //{
            //    //执行微信处理过程
            //    messageHandler.Execute();

            //    WriteContent(messageHandler.ResponseDocument.ToString());
            //    return;
            //}
            //catch (Exception ex)
            //{
            //    using (TextWriter tw = new StreamWriter(Server.MapPath("~/App_Data/Error_" + DateTime.Now.Ticks + ".txt")))
            //    {
            //        tw.WriteLine(ex.Message);
            //        tw.WriteLine(ex.InnerException.Message);
            //        if (messageHandler.ResponseDocument != null)
            //        {
            //            tw.WriteLine(messageHandler.ResponseDocument.ToString());
            //        }
            //        tw.Flush();
            //        tw.Close();
            //    }
            //}
            //finally
            //{
            //    Response.End();
            //}
        }
    }

    private void WriteContent(string str)
    {
        Response.Output.Write(str);
    }
}
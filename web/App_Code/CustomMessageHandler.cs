using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Entities.Request;

/// <summary>
/// CustomMessageHandler 的摘要说明
/// </summary>
namespace Senparc.Weixin.MP.Sample.Weixin
{
    public class CustomMessageHandler : MessageHandler<MessageContext<IRequestMessageBase, IResponseMessageBase>>
    {
        public CustomMessageHandler(Stream inputStream, PostModel postModel)
            : base(inputStream, postModel)
        {

        }
        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }
        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>(); //ResponseMessageText也可以是News等其他类型
            responseMessage.Content = "查货宝：零担物流服务平台。致力于为专线带去货源，为发货人带去优惠。平台为专线提供：智能产品、基础耗材、线上广告、金融保险、提货、干线运力、配送等服务。您可通过此公众号、查货APP享受服务，欲了解更多，请咨询：400-688-7856";
            return responseMessage;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "";//"感谢亲的关注！首次关注，亲可以输入“我要红包”获得1-10元不等的关注红包哦！";//GetWelcomeInfo();
            return responseMessage;
        }

        /// <summary>
        /// 输入关键词回复
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "查货宝：零担物流服务平台。致力于为专线带去货源，为发货人带去优惠。平台为专线提供：智能产品、基础耗材、线上广告、金融保险、提货、干线运力、配送等服务。您可通过此公众号、查货APP享受服务，欲了解更多，请咨询：400-688-7856";//"您的OpenID是：" + requestMessage.FromUserName + "。\r\n您发送了文字信息：" + requestMessage.Content;     //这里的requestMessage.FromUserName也可以直接写成base.WeixinOpenId
                                      //\r\n用于换行，requestMessage.Content即用户发过来的文字内容

            return responseMessage;
        }
    }
}

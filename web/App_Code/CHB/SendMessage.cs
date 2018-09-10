using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Dysmsapi.Model.V20170525;

/// <summary>
/// SendMessage 的摘要说明
/// </summary>
public class SendMessage
{
    public string yanzhengma(string signname, string templatecode, string mobile)
    {
        Random ran = new Random();
        string content_yzm = ran.Next(100000, 999999).ToString();
        //初始化ascClient需要的几个参数
        string product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
        string domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
        //IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "your accessKey", "your accessSecret");
        IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAIELSMzx2Cwd7Y", "8RlnYhbfjFPenoeUZFojgsEKMS39TE");
        DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
        IAcsClient client = new DefaultAcsClient(profile);
        SendSmsRequest request = new SendSmsRequest();
        try
        {
            request.SignName = signname;//"管理控制台中配置的短信签名（状态必须是验证通过）"
            request.TemplateCode = templatecode;//管理控制台中配置的审核通过的短信模板的模板CODE（状态必须是验证通过）"
            request.PhoneNumbers = mobile;//"接收号码，多个号码可以逗号分隔"
            request.TemplateParam = "{\'yanzhengma\':\'" + content_yzm + "\'}";//短信模板中的变量；数字需要转换为字符串；个人用户每个变量长度必须小于15个字符。"
            SendSmsResponse httpResponse = client.GetAcsResponse(request);
            return content_yzm;
        }
        catch (ServerException e)
        {
            //e.printStackTrace();
            e.StackTrace.ToString();
            return "";
        }
        catch (ClientException e)
        {
            //e.printStackTrace();
            e.StackTrace.ToString();
            return "";
        }
    }

    public string testmessage(string fileText)
    {
        Random ran = new Random();
        string content_yzm = ran.Next(100000, 999999).ToString();
        //初始化ascClient需要的几个参数
        string product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
        string domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
        //IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "your accessKey", "your accessSecret");
        IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAIELSMzx2Cwd7Y", "8RlnYhbfjFPenoeUZFojgsEKMS39TE");
        DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
        IAcsClient client = new DefaultAcsClient(profile);
        SendSmsRequest request = new SendSmsRequest();
        try
        {
            request.SignName = "查货app";//"管理控制台中配置的短信签名（状态必须是验证通过）"
            request.TemplateCode = "SMS_130840263";//管理控制台中配置的审核通过的短信模板的模板CODE（状态必须是验证通过）"
            request.PhoneNumbers = fileText;//"接收号码，多个号码可以逗号分隔"
            request.TemplateParam = "";//短信模板中的变量；数字需要转换为字符串；个人用户每个变量长度必须小于15个字符。"
            SendSmsResponse httpResponse = client.GetAcsResponse(request);
            return content_yzm;
        }
        catch (ServerException e)
        {
            //e.printStackTrace();
            e.StackTrace.ToString();
            return "";
        }
        catch (ClientException e)
        {
            //e.printStackTrace();
            e.StackTrace.ToString();
            return "";
        }
    }

    public string zhidanmessage(string fileText, string daodadi, string company, string code)
    {
        Random ran = new Random();
        string content_yzm = ran.Next(100000, 999999).ToString();
        //初始化ascClient需要的几个参数
        string product = "Dysmsapi";//短信API产品名称（短信产品名固定，无需修改）
        string domain = "dysmsapi.aliyuncs.com";//短信API产品域名（接口地址固定，无需修改）
        //IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "your accessKey", "your accessSecret");
        IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAIELSMzx2Cwd7Y", "8RlnYhbfjFPenoeUZFojgsEKMS39TE");
        DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
        IAcsClient client = new DefaultAcsClient(profile);
        SendSmsRequest request = new SendSmsRequest();
        try
        {
            request.SignName = "查货app";//"管理控制台中配置的短信签名（状态必须是验证通过）"
            request.TemplateCode = "SMS_130830308";//管理控制台中配置的审核通过的短信模板的模板CODE（状态必须是验证通过）"
            request.PhoneNumbers = fileText;//"接收号码，多个号码可以逗号分隔"
            request.TemplateParam = "{\'daodadi\':\'" + daodadi + "\',\'company\':\'" + company + "\',\'code\':\'" + code + "\'}";//短信模板中的变量；数字需要转换为字符串；个人用户每个变量长度必须小于15个字符。"
            SendSmsResponse httpResponse = client.GetAcsResponse(request);
            return content_yzm;
        }
        catch (ServerException e)
        {
            //e.printStackTrace();
            e.StackTrace.ToString();
            return "";
        }
        catch (ClientException e)
        {
            //e.printStackTrace();
            e.StackTrace.ToString();
            return "";
        }
    }
}
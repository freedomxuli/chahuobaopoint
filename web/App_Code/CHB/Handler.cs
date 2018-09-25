using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using SmartFramework4v2.Web.WebExecutor;
using SmartFramework4v2.Data.SqlServer;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Drawing;
//using Aspose.BarCode;
//using WxPayAPI;
using System.Collections;
using SmartFramework4v2.Web.Common.JSON;
using Aspose.Cells;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Web.Script.Serialization;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using WxPayAPI;
using Senparc.Weixin.MP.Containers;
using LitJson;

/// <summary>
/// Handler 的摘要说明
/// </summary>
[CSClass("Handler")]
public class Handler
{
	public Handler()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    public void SendWeText(string openid,string content)
    {
        CustomApi.SendText(WxPayConfig.GetConfig().GetAppID(), openid, content);
    }

    public int UserCount(string UserName)
    {
        using (var db = new DBConnection())
        {
            string sql = "select count(UserName) from tb_b_user where UserName = @UserName";
            SqlCommand cmd = db.CreateCommand(sql);
            cmd.Parameters.Add("@UserName", UserName);
            int usercount = Convert.ToInt32(db.ExecuteScalar(cmd).ToString());
            return usercount;
        }
    }
}
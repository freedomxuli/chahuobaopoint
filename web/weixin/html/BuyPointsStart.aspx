﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BuyPointsStart.aspx.cs" Inherits="weixin_html_BuyPointsStart" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="content-type" content="text/html;charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/> 
    <title>电子券支付</title>
</head>

       <script type="text/javascript">
           //获取共享地址
           function editAddress()
           {
               WeixinJSBridge.invoke(
                   'editAddress',
                   function (res)
                   {
                       var addr1 = res.proviceFirstStageName;
                       var addr2 = res.addressCitySecondStageName;
                       var addr3 = res.addressCountiesThirdStageName;
                       var addr4 = res.addressDetailInfo;
                       var tel = res.telNumber;
                       var addr = addr1 + addr2 + addr3 + addr4;
                       //alert(addr + ":" + tel);
                   }
               );
         }

         window.onload = function ()
         {
             if (typeof WeixinJSBridge == "undefined")
             {
                 if (document.addEventListener)
                 {
                     document.addEventListener('WeixinJSBridgeReady', editAddress, false);
                 }
                 else if (document.attachEvent)
                 {
                     document.attachEvent('WeixinJSBridgeReady', editAddress);
                     document.attachEvent('onWeixinJSBridgeReady', editAddress);
                 }
             }
             else
             {
                 editAddress();
             }
         };

	    </script>

<body>
    <%--<form id="Form1" runat="server">
        <br/>
        <div>
            <asp:Label ID="Label1" runat="server" style="color:#00CD00;"><b>商品一：价格为<span style="color:#f00;font-size:50px">1分</span>钱</b></asp:Label><br/><br/>
        </div>
	    <div align="center">
            <asp:Button ID="Button1" runat="server" Text="立即购买" style="width:210px; height:50px; border-radius: 15px;background-color:#00CD00; border:0px #FE6714 solid; cursor: pointer;  color:white;  font-size:16px;" OnClick="Button1_Click" />
	    </div>
    </form>--%>
</body>
</html>

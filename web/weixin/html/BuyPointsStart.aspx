<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BuyPointsStart.aspx.cs" Inherits="weixin_html_BuyPointsStart" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>订单详情</title>
	<meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
	<meta name="apple-mobile-web-app-capable" content="yes">
	<meta name="apple-mobile-web-app-status-bar-style" content="black">

	<!--标准mui.css-->
	<link rel="stylesheet" href="../css/mui.min.css">
	<!--App自定义的css-->
	<link rel="stylesheet" type="text/css" href="../css/app.css"/>
</head>

       <script type="text/javascript">
           //获取共享地址
           function editAddress() {
               WeixinJSBridge.invoke(
                   'editAddress',
                   function (res) {
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

           window.onload = function () {
               if (typeof WeixinJSBridge == "undefined") {
                   if (document.addEventListener) {
                       document.addEventListener('WeixinJSBridgeReady', editAddress, false);
                   }
                   else if (document.attachEvent) {
                       document.attachEvent('WeixinJSBridgeReady', editAddress);
                       document.attachEvent('onWeixinJSBridgeReady', editAddress);
                   }
               }
               else {
                   editAddress();
               }
           };

	    </script>

<body>
    <form id="Form1" runat="server">
        <div class="mui-content">
		    <div class="mui-content-padded">
                <br />
			    <h3 id="title"></h3>
                <br />
			    <p>
				    电子券可抵扣相关物流产品。
			    </p>
                <p style="color:red;">
				    自下单起，请于5分钟内付款，否则订单失效！
			    </p>
                <br />
                <div class="mui-input-row">
				    <input id="points" type="number" readonly="readonly" class="mui-input-clear" placeholder="填写需要购买的电子券，100起拍">
			    </div>
                <br />
                <asp:TextBox ID="totalfee" runat="server" style="display:none;" class="total_fee"></asp:TextBox>
                <asp:TextBox ID="ordercode" runat="server" style="display:none;" class="ordercode"></asp:TextBox>
                <asp:Button ID="Pay" runat="server" type="button" class="mui-btn mui-btn-primary mui-btn-block" style="height:50px;" OnClick="Button1_Click" Text="确认支付"></asp:Button>
		    </div>
	    </div>
     </form>
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
<script src="../js/mui.min.js"></script>
<script src="../js/app.js"></script>
<script src="../js/jquery.1.9.1.js"></script>
<script type="text/javascript">
    var jQuery = jQuery.noConflict();
</script>
<script>
    var money = 0;
    var ordercode = "";
    mui.ready(function () {
        var UserName = localStorage.getItem("mgps_UserName");
        var OrderID = localStorage.getItem("OrderID");

        mui.ajax(grobal_url, {
            dataType: "json",
            type: "post",
            data: {
                "action": "GetOrderDetail",
                "UserName": UserName,
                "OrderID": OrderID
            },
            success: function (data, status, xhr) {
                if (data.sign == '1') {
                    jQuery("#title").html(data.dt[0]["UserXM"] + "　　电子券");
                    jQuery("#points").val(data.dt[0]["Points"]);
                    money = parseFloat(data.dt[0]["Money"]);
                    ordercode = data.dt[0]["OrderCode"];
                    jQuery(".total_fee").val(money);
                    jQuery(".ordercode").val(ordercode);
                } else {
                    mui.alert("获取失败！");
                }
            }
        });
    });

    function Pay() {
        
    }
    function Cancel() {
        document.location.href = "BuyPointsList.html";
    }
</script>
</html>

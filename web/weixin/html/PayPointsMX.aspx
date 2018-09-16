<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PayPointsMX.aspx.cs" Inherits="weixin_html_PayPointsMX" %>

<!DOCTYPE html>
<html>

	<head>
		<meta charset="utf-8">
		<title>支付电子券</title>
		<meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
		<meta name="apple-mobile-web-app-capable" content="yes">
		<meta name="apple-mobile-web-app-status-bar-style" content="black">

		<!--标准mui.css-->
		<link rel="stylesheet" href="../css/mui.min.css">
		<!--App自定义的css-->
		<link rel="stylesheet" type="text/css" href="../css/app.css"/>
        <script src="https://code.jquery.com/jquery-3.3.1.min.js"></script>
        <script src="https://res.wx.qq.com/open/js/jweixin-1.0.0.js"></script>
        <script type="text/javascript">
             var jQuery = jQuery.noConflict();
	    </script>
        <script>
            wx.config({
                debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
                appId: '<%= appID %>', // 必填，公众号的唯一标识
                timestamp: '<%= timestamp %>', // 必填，生成签名的时间戳
                nonceStr: '<%= nonceStr %>', // 必填，生成签名的随机串
                signature: '<%= signature %>',// 必填，签名，见附录1
                // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
                jsApiList: [
                    'checkJsApi', 'scanQRCode'
                ]
            });

            wx.error(function (res) {
                alert("出错了：" + res.errMsg);//这个地方的好处就是wx.config配置错误，会弹出窗口哪里错误，然后根据微信文档查询即可。
            });

            wx.ready(function () {
                wx.checkJsApi({
                    jsApiList: ['scanQRCode'],
                    success: function (res) {

                    }
                });
            });
        </script>
	</head>
	<body>
		<div class="mui-content">
			<div class="mui-content-padded">
                <br />
				<h3><span id="zxmc"></span>电子券：<span id="MyPoints" style="color:red;font-weight:bold;"></span></h3>
                <br />
                <form class="mui-input-group">
                    <div class="mui-input-row">
					    <label>支付电子券</label>
					    <input id="GivePoints" type="text" class="mui-input-clear" placeholder="请填入大于100的整数">
				    </div>
                </form>
                <br />
				<p>
					仅可用于查货宝平台电子券支付。
				</p>
                <br />
                <button type="button" class="mui-btn mui-btn-primary mui-btn-block" onclick="GiveToPoints();">扫码支付</button>
                <br />
			</div>
		</div>
	</body>
	<script src="../js/mui.min.js"></script>
    <script src="../js/app.js"></script>
	<script>
	    mui.ready(function () {
	        var zxmc = localStorage.getItem("PPzxmc");
	        var points = localStorage.getItem("PPpoints");
	       // var CardUserID = localStorage.getItem("PPCardUserID");
	       // var UserName = localStorage.getItem("mgps_UserName");

	        jQuery("#zxmc").html(zxmc);
	        jQuery("#MyPoints").html(points);

	        //mui.ajax(grobal_url, {
	        //    dataType: "json",
	        //    type: "post",
	        //    data: {
	        //        "action": "MyZXPoints",
	        //        "UserName": UserName
	        //    },
	        //    success: function (data, status, xhr) {
	        //        if (data.sign == '1') {
	        //            jQuery("#MyPoints").html(data.points + "分");
	        //        } else {
	        //            mui.alert(data.msg);
	        //        }
	        //    }
	        //});
	    });
	    function GiveToPoints()
	    {
	        //e.detail.gesture.preventDefault();
	        var ex = /^\d+$/;
	        if (ex.test(jQuery("#GivePoints").val())) {
	            if (parseInt(jQuery("#GivePoints").val()) >= 100) {
	                //点击按钮扫描二维码
	                wx.scanQRCode({
	                    needResult: 1, // 默认为0，扫描结果由微信处理，1则直接返回扫描结果，
	                    scanType: ["qrCode"], // 可以指定扫二维码还是一维码，默认二者都有
	                    success: function (res) {
	                        var result = res.resultStr; // 当needResult 为 1 时，扫码返回的结果
	                        var btnArray = ['确定', '取消'];

	                        if (result.length == 11 && result != localStorage.getItem("mgps_UserName")) {

	                            mui.ajax(grobal_url, {
	                                dataType: "json",
	                                type: "post",
	                                data: {
	                                    "action": "JudgeTelByPayPoints",
	                                    "ReceiveUser": result,
	                                    "UserName": localStorage.getItem("mgps_UserName")
	                                },
	                                success: function (data, status, xhr) {
	                                    if (data.sign == '1') {
	                                        mui.prompt('请输入支付密码', '', '提示', btnArray, function (e) {
	                                            var PayPassword = e.value;
	                                            if (e.index == 0) {
	                                                mui.ajax(grobal_url, {
	                                                    dataType: "json",
	                                                    type: "post",
	                                                    data: {
	                                                        "action": "PayPointsList",
	                                                        "Points": jQuery("#GivePoints").val(),
	                                                        "PayPassword": PayPassword,
	                                                        "ReceiveUser": result,
	                                                        "UserName": localStorage.getItem("mgps_UserName"),
	                                                        "CardUserID": localStorage.getItem("PPCardUserID")
	                                                    },
	                                                    success: function (data, status, xhr) {
	                                                        if (data.sign == '1') {
	                                                            mui.alert('支付成功', '提示', function () {
	                                                                document.location.href = "menu.html";
	                                                            });
	                                                        } else {
	                                                            mui.alert(data.msg);
	                                                        }
	                                                    }
	                                                });
	                                            }
	                                        })
	                                    } else {
	                                        mui.alert(data.msg);
	                                    }
	                                }
	                            });
	                            //alert(result);
	                            //window.location.href = result;//因为我这边是扫描后有个链接，然后跳转到该页面
	                        } else {
	                            mui.alert("请扫描有效的二维码！");
	                        }
	                    }
	                });
	            } else {
	                mui.alert("请填入大于100的整数");
	            }
	        } else {
	            mui.alert("请填入整数");
	        }
	    }
	</script>
</html>

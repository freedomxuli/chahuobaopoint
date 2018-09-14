<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PayPoints.aspx.cs" Inherits="weixin_html_PayPoints" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>支付积分</title>
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no" />
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="apple-mobile-web-app-status-bar-style" content="black" />
	<link rel="stylesheet" href="../css/mui.min.css" />
    
    <script src="https://code.jquery.com/jquery-3.3.1.min.js"></script>
    <script src="https://res.wx.qq.com/open/js/jweixin-1.0.0.js"></script>
    <script>
        wx.config({
            debug: true, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
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
    <form id="form1" runat="server">
    <!--下拉刷新容器-->
	<div id="pullrefresh" class="mui-content mui-scroll-wrapper">
		<div class="mui-scroll">
			<!--数据列表-->
			<ul class="mui-table-view">
					
			</ul>
		</div>
	</div>
	<script src="../js/mui.min.js"></script>
    <script src="../js/mui.showLoading.js"></script>
    <script src="../js/app.js"></script>
    <script>
        var list;
        var pagnum = 1;
        var pagesize = 2;
        var totalcount = 0;
        var flag = false;
    	mui.init({
    		pullRefresh: {
    		    container: '#pullrefresh',
    		    up: {
    		        contentrefresh: '正在加载...',
    		        callback: pullupRefresh
    		    }
    		}
    	});

    	/**
            * 上拉加载具体业务实现
            */
    	function pullupRefresh() {
    	    mui.showLoading("正在加载..", "div");
    	    var ajax_sign;
    	    var ajax_msg;
    	    mui.ajax(grobal_url, {
    	        dataType: "json",
    	        type: "post",
    	        data: {
    	            "action": "GetMyCardsList",
    	            "UserName": localStorage.getItem("mgps_UserName"),
    	            "pagnum": pagnum,
    	            "pagesize": pagesize
    	        },
    	        success: function (data, status, xhr) {
    	            ajax_sign = data.sign;
    	            ajax_msg = data.msg;
    	            if (ajax_sign == '1') {
    	                list = data.value.dt;
    	                pagnum = (data.value.cp + 1);
    	                totalcount = data.value.ac;
    	                if (totalcount <= (pagnum - 1) * pagesize) {
    	                    flag = true;
    	                }
    	                setTimeout(function () {
    	                    mui('#pullrefresh').pullRefresh().endPullupToRefresh(flag); //参数为true代表没有更多数据了。
    	                    var table = document.body.querySelector('.mui-table-view');
    	                    var cells = document.body.querySelectorAll('.mui-table-view-cell');
    	                    for (var i = cells.length, len = i + 20; i < len; i++) {
    	                        var li = document.createElement('li');
    	                        li.className = 'mui-table-view-cell';
    	                        //li.innerHTML = '<a class="mui-navigate-right">Item ' + (i + 1) + '</a>';
    	                        li.innerHTML = '<div class="mui-table">' +
                                                    '<div class="mui-table-cell">' +
                                                        '<h4 class="mui-ellipsis">' + list[i]["zxmc"] + '　　电子券　　<span style="color:red;">' + list[i]["points"] + '分</span></h4>' +
                                                        '<h5>所有人：' + list[i]["syr"] + '</h5>' +
                                                        '<p class="mui-h6 mui-ellipsis">备注：该券购买相关产品时可抵扣使用</p>' +
                                                    '</div>' +
                                                    '<div class="mui-table-cell mui-col-xs-2 mui-text-center">' +
                                                        '<br /><button type="button" class="mui-btn mui-btn-primary mui-btn-outlined" data-index="wl" onClick="PayPoints(\'' + list[i]["CardUserID"] + '\',\'' + list[i]["zxmc"] + '\',\'' + list[i]["points"] + '\');">' +
                                                            '扫码' +
                                                        '</button>' +
                                                    '</div>' +
                                                '</div>';
    	                        table.appendChild(li);
    	                    }
    	                }, 1500);
    	            } else {
    	                mui.alert(ajax_msg)
    	            }
    	        }
    	    });
    	}

    	function PayPoints(CardUserID, zxmc, points) {
    	    localStorage.setItem("PPCardUserID", val);
    	    localStorage.setItem("PPzxmc", val);
    	    localStorage.setItem("PPpoints", val);
    	    document.location.href = "PayPointsMX.aspx";
    	    ////点击按钮扫描二维码
    	    //wx.scanQRCode({
    	    //    needResult: 1, // 默认为0，扫描结果由微信处理，1则直接返回扫描结果，
    	    //    scanType: ["qrCode"], // 可以指定扫二维码还是一维码，默认二者都有
    	    //    success: function (res) {
    	    //        var result = res.resultStr; // 当needResult 为 1 时，扫码返回的结果
    	    //        alert(result);
    	    //        //window.location.href = result;//因为我这边是扫描后有个链接，然后跳转到该页面
    	    //    }
    	    //});
    	}

    	if (mui.os.plus) {
    		mui.plusReady(function () {
    		    setTimeout(function () {
    		        mui('#pullrefresh').pullRefresh().pullupLoading();
    		    }, 1000);

    		});
    	} else {
    		mui.ready(function () {
    		    mui('#pullrefresh').pullRefresh().pullupLoading();
    		});
    	}
    </script>
    </form>
</body>
</html>

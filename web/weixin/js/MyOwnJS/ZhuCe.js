function ZhuCe(UserNamevalue, UserXMvalue, FromRoutevalue, ToRoutevalue, UserPasswordvalue, PayPasswordvalue) {
    mui.showLoading("正在加载..", "div");
	var ajax_sign;
	var ajax_msg;
	mui.ajax(grobal_url, {
		dataType: "json",
		type: "post",
		data: {
		    "action": "ZhuCe",
		    "UserName": UserNamevalue,
		    "UserXM": UserXMvalue,
		    "FromRoute": FromRoutevalue,
		    "ToRoute": ToRoutevalue,
		    "UserPassword": UserPasswordvalue,
		    "PayPassword": PayPasswordvalue
		},
		success: function(data, status, xhr) {
		    ajax_sign = data.sign;
		    ajax_msg = data.msg;
		    if(ajax_sign == '1') {
		        mui.alert(ajax_msg);
		        document.location.href = "http://wx.chahuobao.net/weixin/html/menu.html";
		        //mui.openWindow({
		        //    id: 'MGps_login',
		        //    url: 'MGps_login.html',
		        //    show: {
		        //        aniShow: 'pop-in'
		        //    },
		        //    waiting: {
		        //        autoShow: false
		        //    },
		        //    extras: {} //额外扩展参数
		        //});
		    } else {
		        mui.alert(ajax_msg)
		    }
		}
	});
}

function ZhuCe1(UserNamevalue, UserPasswordvalue, PayPasswordvalue) {
    mui.showLoading("正在加载..", "div");
    var ajax_sign;
    var ajax_msg;
    mui.ajax(grobal_url, {
        dataType: "json",
        type: "post",
        data: {
            "action": "ZhuCe1",
            "UserName": UserNamevalue,
            //"UserXM": UserXMvalue,
            "UserPassword": UserPasswordvalue,
            "PayPassword": PayPasswordvalue
        },
        success: function (data, status, xhr) {
            ajax_sign = data.sign;
            ajax_msg = data.msg;
            if (ajax_sign == '1') {
                mui.alert(ajax_msg);
                document.location.href = "http://wx.chahuobao.net/weixin/html/MGps_login.html";
                //mui.openWindow({
                //    id: 'MGps_login',
                //    url: 'MGps_login.html',
                //    show: {
                //        aniShow: 'pop-in'
                //    },
                //    waiting: {
                //        autoShow: false
                //    },
                //    extras: {} //额外扩展参数
                //});
            } else {
                mui.alert(ajax_msg)
            }
        }
    });
}
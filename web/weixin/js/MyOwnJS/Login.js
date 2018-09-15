function login_confirm(UserName, UserPassword, UserLeiXing) {
    mui.showLoading("正在加载..", "div");
	var ajax_sign;
	var ajax_msg;
	mui.ajax(grobal_url, {
		dataType: "json",
		type: "post",
		data: {
		    "action": "login_confirm",
			"UserName": UserName,
			"UserPassword": UserPassword,
			"UserLeiXing": UserLeiXing
		},
		success: function (data, status, xhr) {
		    //mui.hideLoading();
			ajax_sign = data.sign;
			ajax_msg = data.msg;
			if (ajax_sign == '1') {
				localStorage.setItem("mgps_UserName", UserName);
				localStorage.setItem("mgps_UserPassword", UserPassword);
				localStorage.setItem("tuichudenglu", "false");
				document.location.href = "http://wx.chahuobao.net/weixin/html/menu.html";
			} else {
				mui.alert(ajax_msg)
			}
		}
	});
}
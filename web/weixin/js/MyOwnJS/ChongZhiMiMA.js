function ChongZhiMiMa(UserName, UserPassword,UserLeiXing) {
    mui.showLoading("正在加载..", "div");
	var ajax_sign;
	var ajax_msg;
	mui.ajax(grobal_url, {
		dataType: "json",
		type: "post",
		data: {
		    "action": "ChongZhiMiMa",
			"UserName": UserName,
			"UserPassword": UserPassword,
			"UserLeiXing":UserLeiXing
		},
		success: function(data, status, xhr) {
			ajax_sign = data.sign;
			ajax_msg = data.msg;
			if(ajax_sign == '1') {
				mui.alert(ajax_msg);
				mui.openWindow({
					id: 'MGps_login',
					url: 'MGps_login.html',
					show: {
						aniShow: 'pop-in'
					},
					waiting: {
						autoShow: false
					},
					extras: {} //额外扩展参数
				});
			} else {
				mui.alert(ajax_msg)
			}
		}
	});
}
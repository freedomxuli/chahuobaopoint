function ChongZhiMiMa(UserName, UserPassword,UserLeiXing) {
	plus.nativeUI.showWaiting("请稍候...");
	var ajax_sign;
	var ajax_msg;
//	mui.ajax("http://192.168.1.109:7287/WebService/APP_ChongZhiMiMa.ashx", {
		mui.ajax("http://chb.yk56.net/WebService/APP_ChongZhiMiMa.ashx", {
		dataType: "json",
		type: "post",
		data: {
			"UserName": UserName,
			"UserPassword": UserPassword,
			"UserLeiXing":UserLeiXing
		},
		success: function(data, status, xhr) {
			plus.nativeUI.closeWaiting();
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
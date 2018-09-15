function getyanzhengma(UserName, type) {
    //alert(1);
	var ajax_sign;
	var ajax_msg;
	mui.ajax(grobal_url, {
		dataType: "json",
		type: "post",
		data: {
		    "action": "getyanzhengma",
			"UserName": UserName,
			"type": type
		},
		success: function(data, status, xhr) {
			ajax_sign = data.sign;
			ajax_msg = data.msg;
			if(ajax_sign == '1') {
				yanzhengma = data.yanzhengma;
				if(type == 'zhuce') {
					localStorage.setItem("yanzhengma_zhuce", yanzhengma);
				}
				if(type == 'chongzhimima') {
					localStorage.setItem("yanzhengma_chongzhimima", yanzhengma);
				}
				if(type == 'tuidanshenqing') {
					localStorage.setItem("yanzhengma_tuidanshenqing", yanzhengma);
				}
				mui.alert(ajax_msg)
			} else {
				mui.alert(ajax_msg)
			}
		}
	});
}

function senmobile(t) {
	document.getElementById('send').classList.add('mui-disabled');
	for(i = 1; i <= t; i++) {
		window.setTimeout("update_a(" + i + "," + t + ")", i * 1000);
	}
}

function update_a(num, t) {
	var get_code = document.getElementById('send');
	if(num == t) {
		localStorage.setItem("yanzhengma_zhuce", "");
		localStorage.setItem("yanzhengma_chongzhimima", "");
		localStorage.setItem("yanzhengma_tuidanshenqing", "");
		get_code.innerText = " 重新发送 ";
		document.getElementById('send').classList.remove('mui-disabled');
	} else {
		var printnr = t - num;
		get_code.innerText = printnr + " 秒";
	}
}
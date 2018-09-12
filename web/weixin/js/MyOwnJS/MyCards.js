var list;
var pagnum = 1;
var pagesize =2;
var totalcount = 0;
var flag = false;

function BindData(UserNamevalue) {
    mui.showLoading("正在加载..", "div");
	var ajax_sign;
	var ajax_msg;
	mui.ajax(grobal_url, {
		dataType: "json",
		type: "post",
		data: {
		    "action": "GetMyCardsList",
		    "UserName": UserNamevalue,
		    "pagnum": pagnum,
		    "pagesize": pagesize
		},
		success: function(data, status, xhr) {
		    ajax_sign = data.sign;
		    ajax_msg = data.msg;
		    if (ajax_sign == '1') {
		        list = data.value.dt;
		        pagnum = (data.value.cp+1);
		        totalcount = data.value.ac;
		        if (totalcount <= (pagnum - 1) * pagesize) {
		            flag = true;
		        }
		        //mui.alert(ajax_msg);
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


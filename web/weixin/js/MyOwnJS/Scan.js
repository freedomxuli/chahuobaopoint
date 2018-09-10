function setFlash() {
	scan.setFlash(open);
}
function startScan() {
	document.getElementById("bcid").style.height = "325px";
	if(mui.os.android) {
		document.getElementById("bcid").style.marginTop ="20px";
		document.getElementById("content").style.marginTop ="-43px";
		document.getElementById("slider").style.top = "387px";
	} else {
		document.getElementById("slider").style.top = "412px";
	}
	document.getElementById("btn_cancelscan").style.visibility = "visible";
	document.getElementById("btn_startscan").style.visibility = "hidden";
	if(scan == null) {
		scan = new plus.barcode.Barcode('bcid');
		scan.onmarked = onmarked;
	}
	scan.start();
}
function cancelScan() {
	scan.close();
	scan = null;
	if(mui.os.android) {
		document.getElementById("bcid").style.height = "0px";
		document.getElementById("bcid").style.marginTop ="20px";
		document.getElementById("slider").style.top = "90px";
		document.getElementById("content").style.marginTop ="-17px";
	} else {
		document.getElementById("bcid").style.height = "0px";
		document.getElementById("slider").style.top = "85px";
	}
	document.getElementById("btn_cancelscan").style.visibility = "hidden";
	document.getElementById("btn_startscan").style.visibility = "visible";
}

function onmarked(type, result) {
	mui.alert("请扫描指定GPS设备");
	setTimeout("scan.start()", 1000);
}
function getNowFormatDate(date) {
	var seperator1 = "-";
	var seperator2 = ":";
	var month = date.getMonth() + 1;
	var strDate = date.getDate();
	if(month >= 1 && month <= 9) {
		month = "0" + month;
	}
	if(strDate >= 0 && strDate <= 9) {
		strDate = "0" + strDate;
	}
	var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate + " " + date.getHours() + seperator2 + date.getMinutes() + seperator2 + date.getSeconds();
	return currentdate;
}
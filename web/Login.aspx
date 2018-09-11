<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta name="renderer" content="webkit" />
    <title></title>
    <script src="JS/jquery-1.7.1.min.js"></script>
    <script src="JS/jquery.blockUI.js"></script>
    <script src="JS/jquery.cb.js"></script>
     <script type="text/javascript" src="js/extjs/ext-all.js"></script>
    <link rel="Stylesheet" type="text/css" href="js/extjs/resources/css/ext-all.css" />
    <script type="text/javascript" src="js/extjs/ext-lang-zh_CN.js"></script>

    <script type="text/javascript" src="js/json.js"></script>

    <script type="text/javascript" src="js/cb.js"></script>

    <script type="text/javascript" src="js/fun.js"></script>
    <style type="text/css">
        html, body {
            height: 100%;
            margin: 0px;
            padding: 0px;
        }

        body {
            background: url(Images/login/index_bg.jpg) no-repeat no-repeat center center;
        }

        #username {
            border-style: none;
            border-color: inherit;
            border-width: 0px;
            background-color: transparent;
            height: 33px;
            line-height: 33px;
            width: 290px;
            padding: 0px;
            position: relative;
            top: 276px;
            left: 327px;
            font-size: 24px;
        }

        #captcha {
            border-style: none;
            border-color: inherit;
            border-width: 0px;
            background-color: transparent;
            height: 33px;
            line-height: 33px;
            width: 141px;
            padding: 0px;
            position: relative;
            top: 416px;
            left: -263px;
            font-size: 24px;
        }

        #password {
            border-style: none;
            border-color: inherit;
            border-width: 0px;
            background-color: transparent;
            height: 33px;
            line-height: 33px;
            width: 290px;
            padding: 0px;
            position: relative;
            top: 345px;
            left: 33px;
            right: -33px;
            font-size: 24px;
        }

        #container {
            width: 1010px;
            height: 600px;
            background: url(Images/login/login_form_bg2.png) no-repeat no-repeat center center;
            position: absolute;
            top: 50%;
            left: 50%;
            margin-left: -480px;
            margin-top: -280px;
        }

        #btnSubmit {
            width: 411px;
            height: 57px;
            position: relative;
            top: 435px;
            left: 285px;
            cursor: pointer;
        }

        #imgcode {
            width: 120px;
            height: 35px;
            position: relative;
            top: 412px;
            left: -249px;
            cursor: pointer;
        }
    </style>
</head>
<body onkeydown="Send()">
    <div id="container">
        <input id="username" type="text" />
        <input id="password" type="password" />
        <input id="captcha" type="text" />
        <img id="imgcode" src="captcha.aspx?vctype=log" style="cursor: pointer; vertical-align: top;" onclick='code(this);' />
        <img src="Images/login/login_btn.png" id="btnSubmit" />
    </div>
    <script type="text/javascript">
        function Send() {
            if (window.event.keyCode == 13) {
                Login();
            }
        }
        function code(v) {
            setTimeout(function () { v.src = "captcha.aspx?vctype=log&r=" + Math.random().toString() }, 1);
        }
        function Login() {
            CS("CZCLZ.UserClass.Login", function (retVal) {
                if (!retVal) {
                    Ext.MessageBox.show({
                        title: "错误",
                        msg: "登陆失败",
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.WARNING
                    });
                    code(document.getElementById('imgcode'));
                }
                else {
                    window.location.href = 'Main/Index.aspx';
                }
            }, function (retValue) {
                Ext.MessageBox.show({
                    title: "错误",
                    msg: retValue,
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.WARNING
                });
                code(document.getElementById('imgcode'));
            },
            document.getElementById('username').value,
            document.getElementById('password').value,
            document.getElementById('captcha').value);
        }


        $('#btnSubmit').click(function () {
            Login();
        });
    </script>
</body>
</html>



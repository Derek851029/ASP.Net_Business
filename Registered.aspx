<%@ Page Language="C#" EnableEventValidation="true" AutoEventWireup="true" CodeFile="Registered.aspx.cs" Inherits="_Registered" %>
<head runat="server">
        <title>Registered</title>
<%--    -----------------------下方UI套件-------------------------------------%>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>

        <link rel="shortcut icon" href="../favicon.ico"/> 
        <link rel="stylesheet" type="text/css" href="css/demo.css" />
        <link rel="stylesheet" type="text/css" href="css/style.css" />
		<link rel="stylesheet" type="text/css" href="css/animate-custom.css" />

        <script src="https://www.google.com/recaptcha/api.js"></script>
        <script src="https://www.google.com/recaptcha/api.js?render=6LeBOcEZAAAAAFws-jHxyVwPo0Em2us_qKCCPFT0"></script>

        <link href="../DataTables/jquery.dataTables.min.css" rel="stylesheet" />
        <link href="../bootstrap-chosen-master/bootstrap-chosen.css" rel="stylesheet" />
        <script src="../js/jquery.js"></script>
        <script  src="../js/jquery.datetimepicker.full.min.js"></script>
        <script  src="../chosen/chosen.jquery.js"></script>
        <script src="../js/jquery.validate.min.js"></script>
        <script type="text/javascript">


            $(function () {
                grecaptcha.ready(function () {
                    grecaptcha.execute('6LeBOcEZAAAAAFws-jHxyVwPo0Em2us_qKCCPFT0', { action: 'action_name' }).then(function (token) {
                        $.ajax({
                            url: 'Registered.aspx/Robot',
                            type: 'POST',
                            data: JSON.stringify({
                                token: token
                            }),
                            contentType: 'application/json; charset=UTF-8',
                            dataType: "json",
                            success: function (doc) {
                                var json = JSON.parse(doc.d);
                                if (json.success ==false || json.score <= 0.5) {
                                    alert('您是機器人哦，不要亂來');
                                    window.close();
                                }
                            }
                        });
                    });
                });
            });

            function Registered() {
                var Agent_Name = document.getElementById('username').value;
                var UserID = document.getElementById('account').value;
                var Agent_Mail = document.getElementById('email').value;
                var Agent_Phone = document.getElementById('phone').value;
                var Agent_Company = document.getElementById('company').value;
                //var Agent_Team = document.getElementById('department').value;
                var Password = document.getElementById('password').value;
                var password_confirm = document.getElementById('password_confirm').value;
                if (Password != password_confirm) {
                    alert('確認密碼輸入錯誤，請重新嘗試！')
                }
                $.ajax({
                    url: 'Registered.aspx/Registered',
                    type: 'POST',
                    data: JSON.stringify({
                        Agent_Name: Agent_Name,
                        UserID: UserID,
                        Agent_Mail: Agent_Mail,
                        Agent_Phone: Agent_Phone,
                        Agent_Company: Agent_Company,
                        //Agent_Team: Agent_Team,
                        Password: Password
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) { //success之後觸發另一個ajax
                        var json = JSON.parse(doc.d.toString());
                        if (json.status == '註冊成功') {
                            $.ajax({
                                url: 'Registered.aspx/SendMail',
                                type: 'POST',
                                data: JSON.stringify({
                                    Agent_Name: Agent_Name,
                                    Agent_Mail: Agent_Mail,
                                    UserID: UserID,
                                }),
                                contentType: 'application/json; charset=UTF-8',
                                dataType: "json",
                                success: function (doc) {
                                    var json2 = JSON.parse(doc.d.toString());
                                    if (json2.status == '認證信發送失敗, 請重新嘗試或詢問管理員。') {
                                        alert(json2.status);
                                        document.getElementById('resend').style.display = "";
                                    } else {
                                        alert('註冊成功，已發送認證信至您的信箱');
                                    }
                                },
                             });
                        } else {
                            alert(json.status);
                        }
                    },
                });
            }

            var resendbtn = document.getElementById('resend');
            resendbtn.onclick = function () {
                let Agent_Name = document.getElementById('username').value;
                let UserID = document.getElementById('account').value;
                let Agent_Mail = document.getElementById('email').value;s
                $.ajax({
                    url: 'Registered.aspx/SendMail',
                    type: 'POST',
                    data: JSON.stringify({
                        Agent_Name: Agent_Name,
                        Agent_Mail: Agent_Mail,
                        UserID: UserID,
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) {
                        var json2 = JSON.parse(doc.d.toString());
                        if (json2.status == '認證信發送失敗, 請重新嘗試或詢問管理員。') {
                            alert(json2.status);
                            document.getElementById('resend').style.display = "";
                        } else {
                            alert('註冊成功，已發送認證信至您的信箱');
                            window.location.href("http://210.68.227.123:8002/Login.aspx");
                        }
                    },
                });
            }
        </script>

</head>
<body>
    <div class="container">
            <section>				
                <div id="container_demo" >
                    <div id="wrapper">
                        <div id="login" class="animate form">
                                <h1> 註冊 </h1> 
                                <p> 
                                    <label for="usernamesignup" class="uname" data-icon="u">使用者名稱</label>
                                    <input id="username" name="username" required="required" type="text" placeholder="請輸入使用者名稱" />
                                </p>
                                <p> 
                                    <label for="usernamesignup" class="uname" data-icon="u">請輸入帳號</label>
                                    <input id="account" name="account" required="required" type="text" placeholder="請輸入帳號" />
                                </p>
                                <p> 
                                    <label for="emailsignup" class="youmail" data-icon="e" > 請輸入信箱</label>
                                    <input id="email"  required="required" type="email" placeholder="請輸入信箱"/> 
                                </p>
                                <p> 
                                    <label for="emailsignup" class="youphone" data-icon="P" > 行動電話</label>
                                    <input id="phone" name="phone" required="required" type="email" onkeyup = "value=value.replace(/[^\d]/g,'')" placeholder="請輸入電話"/> 
                                </p>
                                <p> 
                                    <label for="emailsignup" class="youcompany" data-icon="C" > 所屬公司</label>
                                    <input id="company" name="compay" required="required" type="text" placeholder="請輸入公司"/> 
                                </p>
<%--                                <p> 
                                    <label for="emailsignup" class="youdepartment" data-icon="D" > 所屬部門</label>
                                    <input id="department" name="department" required="required" type="text" placeholder="請輸入部門"/> 
                                </p>--%>
                                <p> 
                                    <label for="passwordsignup" class="youpasswd" data-icon="p">請輸入密碼 </label>
                                    <input id="password"name="password" required="required" type="password" placeholder="請輸入密碼"/>
                                </p>
                                <p> 
                                    <label for="passwordsignup_confirm" class="youpasswd" data-icon="p">確認密碼 </label>
                                    <input id="password_confirm" name="password_confirm" required="required" type="password" />
                                </p>
                                <p class="signin button">
									<input type="button" onclick="Registered()" value="註冊" /> 
                                    <input  type="button" id="resend" value="重新發送認證信" style="display:none"/>
								</p>
                                <p class="change_link">  
									已經是會員 ?
									<a href="Login.aspx" class="to_login"> 前往登入 </a>
								</p>
                        </div>
                    </div>
                </div>  
            </section>
        </div>
</body>


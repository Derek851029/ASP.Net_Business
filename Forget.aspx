<%@ Page Language="C#"  EnableEventValidation="true" AutoEventWireup="true" CodeFile="Forget.aspx.cs" Inherits="_Forget" %>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>

        <link rel="shortcut icon" href="../favicon.ico"/> 
        <link rel="stylesheet" type="text/css" href="css/demo.css" />
        <link rel="stylesheet" type="text/css" href="css/style.css" />
		<link rel="stylesheet" type="text/css" href="css/animate-custom.css" />

        <link href="../DataTables/jquery.dataTables.min.css" rel="stylesheet" />
        <link href="../bootstrap-chosen-master/bootstrap-chosen.css" rel="stylesheet" />
        <script src="../js/jquery.js"></script>
        <script  src="../js/jquery.datetimepicker.full.min.js"></script>
        <script  src="../chosen/chosen.jquery.js"></script>
        <script src="../js/jquery.validate.min.js"></script>
        <script type="text/javascript">

            function Reset() {
                var account = document.getElementById('account').value;
                var email = document.getElementById('email').value;
                var Password = document.getElementById('password').value;
                var password_confirm = document.getElementById('password_confirm').value;
                if (Password != password_confirm) {
                    alert('確認密碼輸入錯誤，請重新嘗試！')
                }
                $.ajax({
                    url: 'Forget.aspx/Check',
                    type: 'POST',
                    data: JSON.stringify({
                        account: account,
                        email: email,
                        Password: Password,
                    }),
                    contentType: 'application/json; charset=UTF-8',
                    dataType: "json",
                    success: function (doc) { //success之後觸發另一個ajax
                        var json = JSON.parse(doc.d.toString());
                        if (json.status == '確認完成') {
                            $.ajax({
                                url: 'Forget.aspx/SendMail',
                                type: 'POST',
                                data: JSON.stringify({
                                    account: account,
                                    email: email,
                                }),
                                contentType: 'application/json; charset=UTF-8',
                                dataType: "json",
                                success: function (doc) { //success之後觸發另一個ajax
                                    var json2 = JSON.parse(doc.d.toString());
                                    alert(json2.status);
                                },
                            });
                        }
                        else {
                            alert(json.status);
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
                                <h1> 忘記密碼 </h1> 
                                <p> 
                                    <label for="usernamesignup" class="uname" data-icon="u">請輸入帳號</label>
                                    <input id="account" name="account" required="required" type="text" placeholder="請輸入帳號" />
                                </p>
                                <p> 
                                    <label for="emailsignup" class="youmail" data-icon="e" > 請輸入信箱</label>
                                    <input id="email"  required="required" type="email" placeholder="請輸入信箱"/> 
                                </p>
                                <p> 
                                        <label for="passwordsignup" class="youpasswd" data-icon="p">請輸入新密碼 </label>
                                        <input id="password"name="password" required="required" type="password" placeholder="請輸入新密碼"/>
                                    </p>
                                    <p> 
                                        <label for="passwordsignup_confirm" class="youpasswd" data-icon="p">確認密碼 </label>
                                        <input id="password_confirm" name="password_confirm" required="required" type="password" />
                                    </p>
                                
                                <p class="signin button">
									<input type="button" onclick="Reset()" value="送出" /> 
								</p>
                                <p class="change_link">  
									<a href="Login.aspx" class="to_login"> 返回登入頁面 </a>
								</p>
                        </div>
                    </div>
                </div>  
            </section>
        </div>
</body>

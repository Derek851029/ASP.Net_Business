<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Login</title>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/bootstrap-theme.css" rel="stylesheet" />
    <link href="css/bootstrap-theme.min.css" rel="stylesheet" />
   <%-- -------------------------新版登入頁面----------------------%>

    <%-------------------------------For Google--%>
        <script async defer src="https://apis.google.com/js/api.js"
            onload="this.onload=function(){};GoogleClientInit()"
            onreadystatechange="if (this.readyState === 'complete') this.onload()">
    </script>
    <%-----------------------------------------------%>  
    <link rel="shortcut icon" href="../favicon.ico"/> 
    <link rel="stylesheet" type="text/css" href="css/demo.css" />
    <link rel="stylesheet" type="text/css" href="css/style.css" />
    <link rel="stylesheet" type="text/css" href="css/animate-custom.css" />
    <script src="js/jquery.min.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <script src="js/jquery.js"></script>
    <script type="text/javascript">

        //For Google
        let Client_ID = "904327137201-5g4nj2c6b4g3uuo39arn0cif47upu4c2.apps.googleusercontent.com";
        let Discovery_docs = ["https://www.googleapis.com/discovery/v1/apis/people/v1/rest"];

        $(function () {
            $("#Google_Login").on("click", function () {
                GoogleLogin();
            });
            $("#FaceBook_Login").on("click", function () {
                FBLogin();
            });
        });
        //For Facebook
        let FB_appID = "306126927375605";

        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s); js.id = id;
            js.src = "https://connect.facebook.net/en_US/sdk.js";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));

        window.fbAsyncInit = function () {
            FB.init({
                appId: FB_appID,
                cookie: true,
                xfbml: true,
                version: 'v8.0'
            });
            FB.AppEvents.logPageView();
         }


        function GoogleClientInit() {
            gapi.load('client', function () {
                gapi.client.init({
                    clientId: Client_ID,
                    scope:'https://www.googleapis.com/auth/user.phonenumbers.read  https://www.googleapis.com/auth/user.emails.read',
                    discoveryDocs: Discovery_docs
                })
            })
        }

        function GoogleLogin() {
            let auth2 = gapi.auth2.getAuthInstance();
            auth2.signIn().then(function (GoogleUser) {
                let AuthResponse = GoogleUser.getAuthResponse(true);
                let id_token = AuthResponse.id_token;
                gapi.client.people.people.get({
                    'resourceName': 'people/me',
                    'personFields': 'names,phoneNumbers,emailAddresses,addresses,residences,genders,birthdays,occupations'
                }).then(function (res) {
                    let Json_google = res.result;
                    let gmailName = Json_google.names[0].displayName;
                    let gmail_ID = GoogleUser.getId();
                    let UserID = Json_google.emailAddresses[0].value;
                    let gmailEmail = Json_google.emailAddresses[0].value;
                    $.ajax({
                        url: 'HomePage.aspx/GoogleLogin',
                        type: 'POST',
                        data: JSON.stringify({ gmailName: gmailName, gmail_ID: gmail_ID,gmailEmail: gmailEmail, UserID: UserID }),
                        contentType: 'application/json; charset=UTF-8',
                        dataType: "json",
                        success: function (doc) {
                            var json = JSON.parse(doc.d.toString());
                            if (json.status == '【註冊成功！】') {
                                let account = document.getElementById('txt_id');
                                let password = document.getElementById('txt_pwd');
                                let login = document.getElementById('UserLogin');
                                account.value = gmailEmail;
                                password.value = gmail_ID;
                                login.click();
                            } else {
                                alert('【登入失敗, 請重新嘗試或詢問管理員】');
                            }
                        }
                    });
                });
            },
                function (error) {
                    console.log(error);
                });
        }

        function Google_disconnect() {
            let auth2 = gapi.auth2.getAuthInstance(); //取得GoogleAuth物件
            auth2.disconnect().then(function () {
                console.log('User disconnect.');
            });
        }

        function FBLogin() {
            FB.getLoginStatus(function (res) {
                //console.log('status:${res.status}');//Debug
                if (res.status === "connected") {
                    let UserID = res["authResponse"]["userID"];
                    GetProfile();
                } else if (res.status === 'not_authorized' || res.status === "unknown") {
                    //App未授權或用戶登出FB網站才讓用戶執行登入動作
                    FB.login(function (response) {
                        if (response.status === 'connected') {
                            //user已登入FB
                            //抓userID
                            let userID = response["authResponse"]["userID"];
                            GetProfile();

                        } else {
                            // user FB取消授權
                            alert("Facebook帳號無法登入");
                        }
                    }, { scope: 'email' });
                }
            });
        }

        function GetProfile() {
            FB.api("/me", "GET", { fields: 'last_name,first_name,name,email' }, function (user) {
                if (user.error) {
                    console.log(response);
                } else {
                    let UserID = user.email;
                    let fbName = user.name;
                    let fbEmail = user.email;
                    let fb_ID = user.id;
                    $.ajax({
                        url: 'HomePage.aspx/FaceBookLogin',
                        type: 'POST',
                        data: JSON.stringify({ fbName: fbName, fbEmail: fbEmail, UserID: UserID, fb_ID: fb_ID, }),
                        contentType: 'application/json; charset=UTF-8',
                        dataType: "json",
                        success: function (doc) {
                            var json = JSON.parse(doc.d.toString());
                            if (json.status == '【註冊成功！】') {
                                let account = document.getElementById('txt_id');
                                let password = document.getElementById('txt_pwd');
                                let login = document.getElementById('UserLogin');
                                account.value = fbEmail;
                                password.value = fb_ID;
                                login.click();
                            } else {
                                alert('【登入失敗, 請詢問管理員】');
                            }
                        }
                    });
                }
            });
        }

        function Del_FB_App() {

            FB.getLoginStatus(function (response) {//取得目前user是否登入FB網站
                //debug用
                console.log(response);
                if (response.status === 'connected') {
                    //抓userID
                    //let userID = response["authResponse"]["userID"];
                    FB.api("/me/permissions", "DELETE", function (response) {
                        console.log("刪除結果");
                        console.log(response); //gives true on app delete success
                        FB.getLoginStatus(function (res) { }, true);//強制刷新cache避免login status下次誤判
                    });

                } else {
                    console.log("無法刪除FB App");
                }
            });

        } 

    </script>
    <style>
/*        body{
            width: 500px;
            padding-left:20px;
        }*/
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="container">
            <section>				
                <div id="container_demo" >
                    <div id="wrapper">
                        <div id="login" class="animate form">
                            <form autocomplete="on"> 
                                <h1>登入</h1> 
                                <p> 
                                    <label for="username" class="uname" data-icon="u" > 帳號 </label>
                                    <asp:TextBox ID="txt_id" value="" runat="server" Placeholder="帳號" type="text" autocomplete="off"></asp:TextBox>
                                </p>
                                <p> 
                                    <label for="password" class="youpasswd" data-icon="p"> 密碼 </label>
                                    <asp:TextBox ID="txt_pwd" value="" runat="server" TextMode="Password" placeholder="密碼"></asp:TextBox>
                                </p>
                                <p class="login button"> 
                                    <asp:Button ID="UserLogin" runat="server" Text="登入" style="width:100%; padding:1px 2px;"></asp:Button> 
                                    <button type="button" id="btnDisconnect" style ="display:none">斷連Google App</button>
								</p>
                                <p style="height:52px; margin-top:25px; width:370px">
                                    <a href="#" draggable="false" id="Google_Login" style="padding:0px">
										       <img src="images/google_login.png" alt="" draggable="false" style="width:48%"/>
								    </a>
                                    <a href="#" draggable="false" id="FaceBook_Login">
										       <img src="images/facebook_login.png" alt="" draggable="false" style="width:48%; margin-left:10px"/>
								    </a>
                               </p>
                                <input type="button" value="Disconnect App" onclick="Del_FB_App()"  style="display:none"/>
                                <p class="change_link" style="width:100%;">
                                    <a href="Forget.aspx" style="position:relative;right:120px; font-size:5px">忘記密碼</a>
									還不是會員?
									<a href="Registered.aspx">立即註冊</a>
								</p>
                            </form>
                        </div>
                    </div>
                </div>  
            </section>
        </div>
    </form>
    <script src="https://apis.google.com/js/platform.js?onload=init async defer"></script> <%--google api, 猶豫不支援async, 故放在最下面--%>
</body>
</html>

<%@ Page Language="C#"  EnableEventValidation="true" AutoEventWireup="true" CodeFile="0020010001Cer.aspx.cs" Inherits="_0020010001Cer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <script src="js/jquery.min.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            setTimeout(Login, 3000);
            function Login() {
                window.location.href = "http://210.68.227.123:8002/Login.aspx";
            }
        });
    </script>
</head>
<body>
    <style>
        body {
            font-family: "Microsoft JhengHei",Helvetica,Arial,Verdana,sans-serif;
            font-size: 18px;
        }
    </style>
    <form id="form1" runat="server">
        <div class="container" style="width: 100%">
            <br />
            <br />
            <br />
            <div class="alert alert-info" style="width: 100%">
                <div><label style="font-size:72px;">驗證已完成，歡迎使用業務系統</label> </div>
                <div style="width: 100%; height: 500px;">
                    <h1 class="alert-heading" style="float: left;"><strong>提醒：</strong>5秒後將返回到登入頁面<br/>　　　或點選下方回到登入頁面<br/>　<a href="http://210.68.227.123:8002/Login.aspx">返回登入頁面</a></h1>
                    <img style="width: 20%; height: auto; float: right;" src="styles/images/index/apple-touch-icon.png" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>



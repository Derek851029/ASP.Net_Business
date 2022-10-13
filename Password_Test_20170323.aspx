<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Password_Test_20170323.aspx.cs" Inherits="Password_Test_20170323" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="../js/jquery.min.js"></script>
    <script type="text/javascript">
        function Encryption() {
            var Password = document.getElementById("txt_Encryption").value.trim();
            $.ajax({
                url: 'Password_Test_20170323.aspx/Encryption',
                type: 'POST',
                data: JSON.stringify({ Password: Password }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    document.getElementById("txt_1").innerHTML = "顯示：" + json.p;
                    document.getElementById("txt_2").innerHTML = "原碼：" + Password;
                }
            });
        };

        function Decrypted() {
            var Password = document.getElementById("txt_Decrypted").value.trim();
            $.ajax({
                url: 'Password_Test_20170323.aspx/Decrypted',
                type: 'POST',
                data: JSON.stringify({ Password: Password }),
                contentType: 'application/json; charset=UTF-8',
                dataType: "json",
                success: function (doc) {
                    var json = JSON.parse(doc.d.toString());
                    document.getElementById("txt_1").innerHTML = "顯示：" + json.p;
                    document.getElementById("txt_2").innerHTML = "原碼：" + Password;
                }
            });
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            加密：<input type="text" id="txt_Encryption" class="form-control" placeholder="加密" maxlength="30" style="Font-Size: 18px; width: 20%;" />
        </div>
        <div>
            解密：<input type="text" id="txt_Decrypted" class="form-control" placeholder="解密" maxlength="30" style="Font-Size: 18px; width: 20%;" />
        </div>
        <div>
            <label id="txt_1">顯示：</label>
        </div>
        <div>
            <label id="txt_2">原碼：</label>
        </div>
        <div>
            <button id="Post" type="button" class="btn btn-primary btn-lg" onclick="Encryption()">加密</button>
            <button id="Button1" type="button" class="btn btn-primary btn-lg" onclick="Decrypted()">解密</button>
        </div>
    </form>
</body>
</html>

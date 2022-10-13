using Newtonsoft.Json;
using System;
using System.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.UI;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Security.Cryptography;

public partial class _Forget : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Check(string account, string email, string Password)
    {
        if (String.IsNullOrEmpty(account))
        {
            return JsonConvert.SerializeObject(new { status = "請輸入您的帳號！" });
        }
        if (String.IsNullOrEmpty(email))
        {
            return JsonConvert.SerializeObject(new { status = "請輸入您的信箱！" });
        }
        string sqlstr2 = @"SELECT Agent_Mail, UserID FROM DispatchSystem";
        var b = DBTool.Query<ClassTemplate>(sqlstr2);
        if (b.Where(p => p.UserID == account).Any())
        {

        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "帳號不存在，請重新輸入" });
        }

        if (b.Where(p => p.Agent_Mail == email).Any())
        {

        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "信箱不存在，請重新輸入" });
        }

        if (b.Where(p => p.UserID == account ).Any() && b.Where(p => p.Agent_Mail == email).Any())
        {
                string UpdateTime = DateTime.Now.ToString("yyyy/MM/dd");
                string sqlstr = @"UPDATE DispatchSystem set Password = @Password, Agent_Status = @Agent_Status,  UpdateTime = @UpdateTime " +
                                            "WHERE UserID = @UserID AND Agent_Mail = @Agent_Mail";
                var a = DBTool.Query<ClassTemplate>(sqlstr, new
                {
                    Agent_Status = "忘記密碼尚未認證",
                    Agent_Mail = email,
                    UserID = account,
                    Password = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "MD5").ToUpper(),
                    UpdateTime = UpdateTime
                 });
        }
        else
        {
                return JsonConvert.SerializeObject(new { status = "信箱或帳號輸入錯誤" });
        }
             return JsonConvert.SerializeObject(new { status = "確認完成" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SendMail(string account, string email)
    {
        //DES加密
        string original = account; //要被加密的字串
        string key = "abcdefgh"; //要8位數
        string iv = "12345678"; //要8位數
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        des.Key = Encoding.ASCII.GetBytes(key);
        des.IV = Encoding.ASCII.GetBytes(iv);
        byte[] s = Encoding.ASCII.GetBytes(original);
        ICryptoTransform desencrypt = des.CreateEncryptor();
        string account_encryption = BitConverter.ToString(desencrypt.TransformFinalBlock(s, 0, s.Length)).Replace("-", string.Empty); //加密後的字串
        try
        {
            string str = "http://210.68.227.123:8002/0030010001Forget.aspx/?type=" + account_encryption;
            MailMessage msg = new MailMessage();
            //msg.From = new MailAddress("acrm@phrs.com.tw");
            msg.From = new MailAddress("acrm@phrs.com.tw");
            msg.To.Add(email);
            msg.Subject = "業務管理系統(忘記密碼)";
            msg.Body = "請點選網址，以完成變更您的密碼。<a  href=" + str + ">點擊回到登入畫面</a>";
            msg.IsBodyHtml = true;
            SmtpClient smt = new SmtpClient();
            smt.Host = "smtp.gmail.com";
            System.Net.NetworkCredential ntwd = new NetworkCredential();
            ntwd.UserName = "acrm@phrs.com.tw"; //Your Email ID  
            ntwd.Password = "12345678"; // Your Password  
            smt.UseDefaultCredentials = true;
            smt.Credentials = ntwd;
            smt.Port = 587;
            smt.EnableSsl = true; //是否開啟SSL加密
            smt.Send(msg);
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new { status = "發送重設密碼認證信件失敗，請重新嘗試洽管理員。" });
        }
        return JsonConvert.SerializeObject(new { status = "已發送重設密碼的認證信件至您的信箱" });
    }
}
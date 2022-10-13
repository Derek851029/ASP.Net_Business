using Newtonsoft.Json;
using System;
using System.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Windows.Forms;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.SS.Formula.Functions;
using System.ServiceModel.Channels;
using System.Security.Policy;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Security.Cryptography;

public partial class _Registered : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Registered(string Agent_Name, string UserID, string Agent_Mail, string Agent_Phone, string Agent_Company/*, string Agent_Team*/, string Password)
    {
        if (String.IsNullOrEmpty(Agent_Name)) {
            return JsonConvert.SerializeObject(new { status = "請輸入使用者名稱！" });
        }
        if (String.IsNullOrEmpty(UserID))
        {
            return JsonConvert.SerializeObject(new { status = "請輸入帳號！" });
        }
        if (String.IsNullOrEmpty(Agent_Mail))
        {
            return JsonConvert.SerializeObject(new { status = "請輸入信箱！" });
        }
        if (Agent_Mail.Contains("@") == false)
        {
            return JsonConvert.SerializeObject(new { status = "信箱輸入錯誤，請輸入正確的信箱！" });
        }
        if (String.IsNullOrEmpty(Agent_Phone))
        {
            return JsonConvert.SerializeObject(new { status = "請輸入行動電話！" });
        }
        if (Agent_Phone.Length < 10)
        {
            return JsonConvert.SerializeObject(new { status = "請輸入正確的行動電話！" });
        }
        //if (String.IsNullOrEmpty(Agent_Company))
        //{
        //    return JsonConvert.SerializeObject(new { status = "請輸入所屬公司！" });
        //}
        //if (String.IsNullOrEmpty(Agent_Team))
        //{
        //    return JsonConvert.SerializeObject(new { status = "請輸入所屬部門！" });
        //}
        if (String.IsNullOrEmpty(Password))
        {
            return JsonConvert.SerializeObject(new { status = "請輸入密碼！" });
        }

        string sqlstr2 = @"SELECT Agent_Name, Agent_Mail, UserID FROM DispatchSystem";
        var b = DBTool.Query<ClassTemplate>(sqlstr2);
        if (b.Where(p => p.Agent_Mail == Agent_Mail).Any())
        {
            return JsonConvert.SerializeObject(new { status = "【此信箱已註冊！】" });
        }else if (b.Where(p => p.UserID == UserID).Any())
        {
            return JsonConvert.SerializeObject(new { status = "【此帳號已註冊，請更換帳號！】" });
        }

        string UpdateTime = DateTime.Now.ToString("yyyy/MM/dd");
        string sqlstr = @"INSERT INTO DispatchSystem (Agent_Name, Agent_Status, UserID,  Agent_Mail, Agent_Phone, Agent_Company, Password, Role_ID, Agent_LV, UpdateTime)" +
            "VALUES(@Agent_Name, @Agent_Status, @UserID, @Agent_Mail, @Agent_Phone, @Agent_Company, @Password, @Role_ID, @Agent_LV, @UpdateTime)";
        var a = DBTool.Query<ClassTemplate>(sqlstr, new {
            Agent_Name = Agent_Name.Trim(),
            UserID = UserID.Trim(),
            Agent_Status = "尚未信箱認證",
            Agent_Company = Agent_Company.Trim(),
            //Agent_Team = Agent_Team.Trim(),
            Agent_Mail = Agent_Mail.Trim(),
            Agent_Phone = Agent_Phone.Trim(),
            Password = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "MD5").ToUpper(),
            Role_ID = "100",
            Agent_LV = "10",
            UpdateTime = UpdateTime
        });
        return JsonConvert.SerializeObject(new { status = "註冊成功"});
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SendMail(string Agent_Mail, string Agent_Name, string UserID)
    {
        //DES加密
        string original = UserID; //要被加密的字串
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
            string str = "http://210.68.227.123:8002/0020010001Cer.aspx/?type=" + account_encryption;
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("acrm@phrs.com.tw");
            msg.To.Add(Agent_Mail);
            msg.Subject = "驗證業務管理系統";
            msg.Body = "嗨 "+Agent_Name+ ", 感謝您註冊業務管理系統! 在開始之前, 我們需要確認您的身分, 請點選網址以完成註冊動作。<a  href="+str+ ">點擊回到登入畫面</a> ";
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
            return JsonConvert.SerializeObject(new { status = "認證信發送失敗, 請重新嘗試或詢問管理員。" });
            //return JsonConvert.SerializeObject(new { status = "0", txt = "訊息發送失敗。" });
        }
        return JsonConvert.SerializeObject(new { status = "" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Robot(string token)
    {
        string url = "https://www.google.com/recaptcha/api/siteverify?secret=6LeBOcEZAAAAAM_JLHkK8JKymDpV6BkabRKnQV5a&response="+token+"";
        WebRequest request = (HttpWebRequest)WebRequest.Create(url);
        WebResponse response = request.GetResponse();
        StreamReader stream = new StreamReader(response.GetResponseStream());
        return stream.ReadToEnd();
    }
}
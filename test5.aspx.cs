using Dapper;
using Newtonsoft.Json;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;
using System.ServiceModel.Channels;
using System.Windows.Forms;
//using log4net;
//using log4net.Config;
public partial class test5 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Check();
        }
    }

    [WebMethod(true)]//或[WebMethod(true)]
    public static string List_Team()
    {
        //Check();
        string sqlstr = @"SELECT BUSINESSNAME,APP_EMAIL FROM BusinessData " +
            "where Type = '保留' ";
        //sqlstr = @"SELECT BUSINESSNAME,APP_EMAIL FROM BusinessData " +
        //    "where Type = '保留' ";
        var a = DBTool.Query<Message_Value>(sqlstr).ToList().Select(p => new
        {
            BUSINESSNAME = p.BUSINESSNAME,
            APP_EMAIL = p.APP_EMAIL
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(true)]//或[WebMethod(true)]
    public static string List_Can_Message()
    {
        Check();
        string sqlstr = @"SELECT EMAIL_TITLE,EMAIL_CONTENT FROM Email_Can_Message ";
        var a = DBTool.Query<Message_Value>(sqlstr).ToList().Select(p => new
        {
            EMAIL_TITLE = p.EMAIL_TITLE,
            EMAIL_CONTENT = p.EMAIL_CONTENT
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Check()
    {
        string Check = JASON.Check_ID("0060010035.aspx");
        if (Check == "NO")
        {
            System.Web.HttpContext.Current.Response.Redirect("~/Default.aspx");
        }
        return "";
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Send_mail(string form, string address, string title, string body)
    {
        try
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(form);
            msg.To.Add(address);
            msg.Subject = title;
            msg.Body = body;
            msg.IsBodyHtml = true;
            if (string.IsNullOrEmpty(address))//防呆
            {
                return JsonConvert.SerializeObject(new { status = "請輸入信箱地址。" });
            }
            SmtpClient smt = new SmtpClient();
            smt.Host = "smtp.office365.com";
            System.Net.NetworkCredential ntwd = new NetworkCredential();
            ntwd.UserName = "derek4504@hotmail.com"; //Your Email ID  
            ntwd.Password = "jkjk4747jkjk"; // Your Password  
            smt.UseDefaultCredentials = true;
            smt.Credentials = ntwd;
            smt.Port = 587;
            smt.EnableSsl = true; //是否開啟SSL加密
            smt.Send(msg);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            //return JsonConvert.SerializeObject(new { status = "0", txt = "訊息發送失敗。" });
        }
        return JsonConvert.SerializeObject(new { status =  "訊息發送成功。" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Create_Can_Message(string add_title, string add_content)
    {
        if (String.IsNullOrEmpty(add_title))
        {
            return JsonConvert.SerializeObject(new { status = "請輸入標題" });
            
        }
        if (String.IsNullOrEmpty(add_content))
        {
            return JsonConvert.SerializeObject(new { status = "請輸入內容" });

        }
        string sqlstr = @"INSERT INTO Email_Can_Message(EMAIL_TITLE,EMAIL_CONTENT) VALUES( @add_title, @add_content) ";
        var a = DBTool.Query<ClassTemplate>(sqlstr, new
        {
            add_title = add_title,
            add_content = add_content,
        });
        return JsonConvert.SerializeObject(new { status = "新增成功" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string DeleteSelectTable(string delete_title, string delete_content)

    {
        if (String.IsNullOrEmpty(delete_title))
        {
            return JsonConvert.SerializeObject(new { status = "請選擇刪除項目" });
        }
            string sqlstr = @"DELETE FROM Email_Can_Message WHERE EMAIL_TITLE=@delete_title ";
        var a = DBTool.Query<ClassTemplate>(sqlstr, new
        {
            delete_title = delete_title,
            delete_content = delete_content,
        });
        return JsonConvert.SerializeObject(new { status = "刪除成功" });
    }

    public class Message_Value
    {
        public string PID { get; set; }
        public string BUSINESSNAME { get; set; }
        public string APP_EMAIL { get; set; }
        public string EMAIL_TITLE { get; set; }
        public string EMAIL_CONTENT { get; set; }
        public string add_title { get; set; }
        public string add_content { get; set; }
        public string SYSID { get; set; }
        public string ID { get; set; }
        public string Tag_Team { get; set; }
        public string Create_Team { get; set; }
        public string Create_Name { get; set; }
        public DateTime Create_Time { get; set; }
        public DateTime Response_Time { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string time_response { get; set; }
    }
}
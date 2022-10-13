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
using System.Security.AccessControl;

public partial class HomePage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        HttpContext.Current.Response.Cache.SetNoServerCaching();
        HttpContext.Current.Response.Cache.SetNoStore();
    }
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string UserOrNot()
    {
        string UserIDNAME = (string)(HttpContext.Current.Session["UserIDNAME"]); 
        return JsonConvert.SerializeObject(new { status = UserIDNAME });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string GoogleLogin(string gmailName, string gmailEmail, string UserID, string gmail_ID)
    {
        string sqlstr2 = @"SELECT Agent_Name, Agent_Mail UserID FROM DispatchSystem";
        var a = DBTool.Query<ClassTemplate>(sqlstr2);
        if (a.Where(p => p.UserID == UserID).Any())
        {
            return JsonConvert.SerializeObject(new { status = "【註冊成功！】" }); //為了方便下面做事, return相同字串, 其實是直接登入
        }


        string UpdateTime = DateTime.Now.ToString("yyyy/MM/dd");
        string sqlstr = @"INSERT INTO DispatchSystem (Agent_Name, Agent_Status, Agent_Mail, UserID, Password, Role_ID, Agent_LV, UpdateTime)"+
                                    "VALUES(@gmailName, @Agent_Status, @gmailEmail, @UserID, @Password, @Role_ID, @Agent_LV, @UpdateTime)";
        DBTool.Query<ClassTemplate>(sqlstr, new { 
            gmailName = gmailName,
            Agent_Status = "在職",
            gmailEmail = gmailEmail,
            UserID = gmailEmail,
            Password = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(gmail_ID, "MD5").ToUpper(),
            Role_ID  = "100", 
            Agent_LV  = "10",
            UpdateTime= UpdateTime
        });
        return JsonConvert.SerializeObject(new { status = "【註冊成功！】" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string FaceBookLogin(string fbName, string fbEmail, string UserID, string fb_ID)
    {
        string sqlstr2 = @"SELECT Agent_Name, Agent_Mail UserID FROM DispatchSystem";
        var a = DBTool.Query<ClassTemplate>(sqlstr2);
        if (a.Where(p => p.UserID == UserID).Any())
        {
            return JsonConvert.SerializeObject(new { status = "【註冊成功！】" });
        }

        string UpdateTime = DateTime.Now.ToString("yyyy/MM/dd");
        string sqlstr = @"INSERT INTO DispatchSystem (Agent_Name, Agent_Status, Agent_Mail, UserID, Password, Role_ID, Agent_LV, UpdateTime)" +
                                    "VALUES(@gmailName, @Agent_Status, @gmailEmail, @UserID, @Password, @Role_ID, @Agent_LV, @UpdateTime)";
        DBTool.Query<ClassTemplate>(sqlstr, new
        {
            gmailName = fbName,
            Agent_Status = "在職",
            gmailEmail = fbEmail,
            UserID = fbEmail,
            Password = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(fb_ID, "MD5").ToUpper(),
            Role_ID = "100",
            Agent_LV = "10",
            UpdateTime = UpdateTime
        });
        return JsonConvert.SerializeObject(new { status = "【註冊成功！】" });
    }
}
    
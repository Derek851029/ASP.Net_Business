using Dapper;
using log4net.Repository.Hierarchy;
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
using log4net;
//using log4net;
//using log4net.Config;
public partial class test5 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Check();
        }
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string test()
    {
        //Check();
        string sqlstr = "SELECT BUSINESSNAME,APP_EMAIL FROM BusinessData " +
            "where Type = '保留' ";
        //sqlstr = @"SELECT BUSINESSNAME,APP_EMAIL FROM BusinessData " +
        //    "where Type = '保留' ";
        var a = DBTool.Query<Message_Value>(sqlstr, new { });
        string outputJson = JsonConvert.SerializeObject(a.ToList());
        return outputJson;
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

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string List_Message()
    {
        Check();
        string sqlstr = "";
        string Agent_Team = HttpContext.Current.Session["Agent_Team"].ToString();
        string Agent_LV = HttpContext.Current.Session["Agent_LV"].ToString();

        if (Agent_LV == "10")
        {
            sqlstr = @"SELECT SYSID, Tag_Team, Create_Team, Create_Name, Create_Time, Title, Message FROM Msg_Message WHERE Tag_Team IN (@Agent_Team, '全部' ) ";
        }
        else
        {
            sqlstr = @"SELECT SYSID, Tag_Team, Create_Team, Create_Name, Create_Time, Title, Message FROM Msg_Message  ";
        }

        var a = DBTool.Query<Message_Value>(sqlstr, new { Agent_Team = Agent_Team }).ToList().Select(p => new
        {
            SYSID = p.SYSID,
            Tag_Team = p.Tag_Team,
            Create_Team = p.Create_Team,
            Create_Name = p.Create_Name,
            Create_Time = p.Create_Time.ToString("yyyy/MM/dd HH:mm"),
            Title = HttpUtility.HtmlEncode(p.Title.Trim()),
            Message = HttpUtility.HtmlEncode(p.Message.Trim())
        });

        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string List_Response(string ID)
    {
        Check();

        if (ID.Length > 10)
        {
            ID = "0";
        }
        else
        {
            if (JASON.IsInt(ID) != true)
            {
                ID = "0";
            }
        }

        string sqlstr = "";
        sqlstr = @"SELECT Agent_Name, Agent_Team, Response, Response_Time FROM Msg_Response WHERE ID=@ID ORDER BY Response_Time ";
        var a = DBTool.Query<Message_Value>(sqlstr, new { ID = ID }).ToList().Select(p => new
        {
            Agent_Team = p.Agent_Team,
            Agent_Name = p.Agent_Name,
            Response = HttpUtility.HtmlEncode(p.Response.Trim()),
            Response_Time = p.Response_Time.ToString("yyyy/MM/dd HH:mm")
        });

        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string New_Msg(string Msg, string ID)
    {
        Check();
        int int_len = 0;
        string value = "";
        string error = "訊息發送失敗。";

        if (ID.Length > 10)
        {
            return JsonConvert.SerializeObject(new { status = "1", txt = error });
        }
        else
        {
            if (JASON.IsInt(ID) != true)
            {
                return JsonConvert.SerializeObject(new { status = "1", txt = error });
            }
        }

        Msg = Msg.Trim();

        if (Msg.Length < 1)
        {
            return JsonConvert.SerializeObject(new { status = "1", txt = error });
        }

        if (Msg.Length > 250)
        {
            return JsonConvert.SerializeObject(new { status = "1", txt = error });
        }
        else
        {
            int_len = Msg.Length;
            value = HttpUtility.HtmlEncode(Msg);
            if (value.Length != int_len)
            {
                return JsonConvert.SerializeObject(new { status = "1", txt = error });
            };
        }

        string UserID = HttpContext.Current.Session["UserID"].ToString();
        string UserIDNAME = HttpContext.Current.Session["UserIDNAME"].ToString();
        string Agent_Team = HttpContext.Current.Session["Agent_Team"].ToString();
        string time_response = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        string sqlstr = "";
        sqlstr = @"INSERT INTO Msg_Response ( ID, Agent_ID, Agent_Name, Agent_Team, Response,  Response_Time ) " +
            " VALUES ( @ID, @Agent_ID, @Agent_Name, @Agent_Team, @Response, @time_response ) ";

        Message_Value Template = new Message_Value()
        {
            ID = ID,
            Agent_ID = UserID,
            Agent_Name = UserIDNAME,
            Agent_Team = Agent_Team,
            Response = Msg,
            time_response = time_response
        };

        using (IDbConnection db = DBTool.GetConn())
        {
            db.Execute(sqlstr, Template);
            db.Close();
        }

        return JsonConvert.SerializeObject(new { status = "0", txt = "訊息發送成功。" });
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
    public static string SendMag(string[] str_Array, string Title, string Message)
    {
        Check();
        string Agent_ID = HttpContext.Current.Session["UserID"].ToString();
        string Agent_Name = HttpContext.Current.Session["UserIDNAME"].ToString();
        string Agent_Team = HttpContext.Current.Session["Agent_Team"].ToString();
        string Create_Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        string check = "";
        string sqlstr = "";
        string back = "";
        bool all = Array.IndexOf(str_Array, "全部") >= 0;

        List<XXS> check_value = new List<XXS>();
        check_value.Add(new XXS { URL_ID = Title, MiniLen = 1, MaxLen = 250, Alert_Name = "公告標題", URL_Type = "txt" });
        check_value.Add(new XXS { URL_ID = Message, MiniLen = 1, MaxLen = 250, Alert_Name = "公告內容", URL_Type = "txt" });
        JavaScriptSerializer Serializer = new JavaScriptSerializer();
        string outputJson = Serializer.Serialize(check_value);
        back = JASON.Check_XSS(outputJson);
        if (back != "")
        {
            return back;
        };

        if (str_Array.Length != 0)
        {
            if (all == true)
            {
                sqlstr = @"INSERT INTO Msg_Message (Tag_ID, Tag_Name, Tag_Team, Create_ID, Create_Name, Create_Team, Title, Message, Create_Time) " +
                    "VALUES ('', '', '全部', @Agent_ID, @Agent_Name, @Agent_Team, @Title, @Message, @Create_Time ) ";
            }
            else
            {
                sqlstr = "Declare @Array table(Value nvarchar(20)) ";
                for (int i = 0; i < str_Array.Length; i++)
                {
                    //=========================================
                    check = @"SELECT TOP 1 SYSID FROM DispatchSystem WHERE Agent_Status != '離職' AND Agent_Team=@Agent_Team";
                    var a = DBTool.Query<ClassTemplate>(check, new { Agent_Team = str_Array[i] });
                    if (a.Any())
                    {
                        sqlstr += "INSERT INTO @Array (Value) VALUES ('" + str_Array[i] + "') ";
                    };
                    //=========================================
                }
                sqlstr += @"INSERT INTO Msg_Message (Tag_ID, Tag_Name, Tag_Team, Create_ID, Create_Name, Create_Team, Title, Message, Create_Time) " +
                    "SELECT '', '', Value, @Agent_ID, @Agent_Name, @Agent_Team, @Title, @Message, @Create_Time " +
                    "FROM @Array ";
            }

            var b = DBTool.Query<ClassTemplate>(sqlstr, new { Agent_ID = Agent_ID, Agent_Name = Agent_Name, Agent_Team = Agent_Team, Title = Title, Message = Message, Create_Time = Create_Time });
            return "公告發送完成。";
        }
        else
        {
            return "請選擇要發送的部門。";
        }
    }

    private void EMAIL()
        {
                    //建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port 
                    try
                    {
                        System.Net.Mail.SmtpClient MySmtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
                        //設定你的帳號密碼
                        MySmtp.Credentials = new System.Net.NetworkCredential(email_id, email_pw);
                        //Gmial 的 smtp 使用 SSL
                        MySmtp.EnableSsl = true;
                        //發送Email
                        
                        //MySmtp.Send("manstrong.dispatch@gmail.com", q.E_MAIL, "派工系統通知", "需求單狀態：" + no + "\n" +
                        MySmtp.Send("");
                        Response.Write("郵件發送狀態：成功" + "<br>");
                        Response.Write("發送編號：" + q.Form_ID + "<br>");
                        Response.Write("目標信箱地址：" + q.Mail);
                        using (IDbConnection db = DBTool.GetConn())
                        {
                            db.Execute(sql, new { Form_ID = q.Form_ID });
                            db.Close();
                        }
                    }
                    catch (Exception error)
                    {
                        Response.Write("郵件發送狀態：失敗" + "<br>");
                        Response.Write("發送編號：" + q.Form_ID + "<br>");
                        Response.Write("目標信箱地址：" + q.Mail);
                        logger.Info("===== Email_Delay =====");
                        logger.Info("郵件發送狀態：失敗");
                        logger.Info("發送編號：" + q.Form_ID);
                        logger.Info("目標信箱地址：" + q.Mail);
                        logger.Info("ERROR：" + error);
                        logger.Info("==================");

                        //sql = "UPDATE tblAssign SET Email_Flag='2' WHERE SYSID=@SYSID";
                        sql = "UPDATE Work_Table SET Email_Flag='2', Send_Date=GetDate() WHERE Form_ID=@Form_ID";
                        Ready_Reason = q.Ready_Reason.Trim(); //刪除字串中的空白
                        using (IDbConnection db = DBTool.GetConn())
                        {
                            db.Execute(sql, new { Form_ID = q.Form_ID });
                            db.Close();
                        }
                    }
                }
            }
            else
            {
                Response.Write("郵件發送狀態：沒有需要發送的E-Mail");
            }
      
    }



    public class Message_Value
    {
        public string BUSINESSNAME { get; set; }
        public string APP_EMAIL { get; set; }
        public string SYSID { get; set; }
        public string ID { get; set; }
        public string Tag_Team { get; set; }
        public string Create_Team { get; set; }
        public string Create_Name { get; set; }
        public DateTime Create_Time { get; set; }
        public string Agent_ID { get; set; }
        public string Agent_Team { get; set; }
        public string Agent_Company { get; set; }
        public string Agent_Name { get; set; }
        public string Response { get; set; }
        public DateTime Response_Time { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class _Default : System.Web.UI.Page
{
    protected string str_time = "";
    protected string type = "";
    protected string Team = "";
    static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected void Page_Load(object sender, EventArgs e)
    {
        string UserIDNO = (string)(Session["UserIDNO"]);
        string UserIDNAME = (string)(Session["UserIDNAME"]);
        string UserID = (string)(Session["UserID"]);
        string Agent_LV = (string)(Session["Agent_LV"]);
        Session["Time_Flag"] = "0";
        Team = (string)(Session["Agent_Team"]);
        if (Agent_LV == "20" || Agent_LV == "30")
        {
            table_data3.Style.Add("display", "none");
        }
        else
        {
            table_data.Style.Add("display", "none");
        }

        if (!string.IsNullOrEmpty(Request.Params["type"]))
        {
            type = Request.Params["type"];
        }

        if (!string.IsNullOrEmpty(Request.Params["str_time"]))
        {
            Session["Time_Flag"] = Request.Params["str_time"];
            str_time = Request.Params["str_time"];
        }
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string GetClassGroup()
    {
        string Time_Flag = "0";
        try
        {
            Time_Flag = HttpContext.Current.Session["Time_Flag"].ToString();
        }
        catch
        {
            Time_Flag = "0";
        }
        string Agent_Team = HttpContext.Current.Session["Agent_Team"].ToString();
        string Agent_LV = HttpContext.Current.Session["Agent_LV"].ToString();
        string Agent_ID = HttpContext.Current.Session["UserID"].ToString();
        return JsonConvert.SerializeObject(ClassScheduleRepository.Default_Table(Agent_Team, Agent_LV, Agent_ID, Time_Flag));
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Flag(string Team, string Flag, string Agent_LV, string Agent_ID)
    {
        if (Flag == "6" || Flag == "8")
        {
            var a = ClassScheduleRepository.Default_Flag(Team, Flag, Agent_LV, Agent_ID)
                   .Select(p => new
                   {
                       SYS_ID = p.SYSID,
                       MNo = p.CNo,
                       Time_01 = p.StartTime.ToString("MM/dd HH:mm"),
                       Type = p.Type,
                       ServiceName = p.ServiceName,
                       Labor_CName = p.Danger,
                       Question = p.Answer,
                       Create_Name = p.Agent_Name
                   });
            return JsonConvert.SerializeObject(a, Formatting.Indented);
        }
        else
        {
            var a = ClassScheduleRepository.Default_Flag(Team, Flag, Agent_LV, Agent_ID)
                   .Select(p => new
                   {
                       SYS_ID = p.SYS_ID,
                       MNo = p.MNo,
                       Time_01 = p.Time_01.ToString("MM/dd HH:mm"),
                       Type = p.Type,
                       ServiceName = p.ServiceName,
                       Labor_CName = p.Labor_CName,
                       Question = p.Question,
                       Create_Name = p.Cust_Name
                   });
            return JsonConvert.SerializeObject(a, Formatting.Indented);
        }
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string List_Record()
    {
        string Agent_LV = HttpContext.Current.Session["Agent_LV"].ToString();
        string Agent_Name = HttpContext.Current.Session["UserIDNAME"].ToString();
        string Sqlstr = @"SELECT b.Agent_Name, a.* FROM Daily_Itinerary as a " +
  "left join  DispatchSystem as b on a.Agent_ID=b.Agent_ID " +
  "where Job_Agent = @Agent_Name and Day > GETDATE()-8 "; 

        //string Sqlstr = @"SELECT * " +
        //    "FROM Daily_Itinerary  WHERE Job_Agent=@Agent_Name ";   //         '楊少華' 
        var a = DBTool.Query<ClassTemplate>(Sqlstr, new {Agent_Name = Agent_Name  }).ToList().Select(p => new   //  
        {
            A = Value(p.DI_No),
            //B = Convert.ToDateTime(p.SetupTime).ToString("yyyy/MM/dd HH:mm"),
            B = Value(p.Agent_Name),
            C = Value3(p.Day),
            D = Value(p.Schedule),
            E = Value2(p.S_UpdateTime),
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }
    //============= 接受職代 =============
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Accept(string DI_No)
    {        
        string sqlstr = @"update Daily_Itinerary set " +
                " Schedule = '休假', S_UpdateTime = @S_UpdateTime   " +
                " where DI_No = @DI_No ";      // 原始的不會 Group    CONVERT(varchar(100), Upload_Time, 111) 要整串放進 Group 中
                var chk = DBTool.Query<ClassTemplate>(sqlstr, new
                {
                    DI_No = DI_No,
                    S_UpdateTime = DateTime.Now,  
                });
        return JsonConvert.SerializeObject(new { status = "success" });
    }
    //============= 拒絕職代 =============
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Reject(string DI_No)
    {
        string sqlstr = @"update Daily_Itinerary set " +
                " Schedule = '代理被拒', S_UpdateTime = @S_UpdateTime   " +
                " where DI_No = @DI_No ";      // 原始的不會 Group    CONVERT(varchar(100), Upload_Time, 111) 要整串放進 Group 中
        var chk = DBTool.Query<ClassTemplate>(sqlstr, new
        {
            DI_No = DI_No,
            S_UpdateTime = DateTime.Now,
        });
        return JsonConvert.SerializeObject(new { status = "success" });
    }

    public static string DealingProcess(string value)
    {
        if (value == "0")
        {
            return "結案";
        }
        else
        {
            return "處理中";
        }
    }
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string URL()
    {
        //Check();
        string str_url = "../0030010000/0030010006.aspx";         //轉跳補打卡頁面
        return JsonConvert.SerializeObject(new { status = str_url, type = "ok" });
    }

    public static string Value(string value)        // 當值為null時跳過  非 null 時去除後方空白
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = value.Trim();
        }
        return value;
    }
    public static string Value2(string value)        // 當值為null時跳過  非 null 時改時間格式
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = DateTime.Parse(value).ToString("yyyy-MM-dd HH:mm");
        }
        return value;
    }
    public static string Value3(string value)        // 當值為null時跳過  非 null 時改時間格式
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = DateTime.Parse(value).ToString("yyyy-MM-dd");
        }
        return value;
    }
}
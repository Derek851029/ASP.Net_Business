using Dapper;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using System.Threading;
public partial class _0010010003 : System.Web.UI.Page
{
    static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    string sql_txt;

    protected string opinionsubject = "";
    protected string seqno = "";
    protected string str_title = "新增";
    protected string str_type = "存檔後系統自動編號";
    protected string new_mno = "";
    protected string new_mno2 = "";
    protected string new_mno3 = "";
    protected long mno3 = 0;
    protected string str_time;
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Load()
    {
        string Agent_ID = HttpContext.Current.Session["UserID"].ToString();
        string Sqlstr = @"SELECT TOP 1 * FROM DispatchSystem WHERE Agent_ID = @Agent_ID AND Agent_Status != '離職'";      // 員工名單內且未離職
        var a = DBTool.Query<ClassTemplate>(Sqlstr, new { Agent_ID = Agent_ID }).ToList().Select(p => new
        {
            Agent_Team = p.Agent_Team,
            Agent_Name = p.Agent_Name,
            Agent_ID = Value(p.Agent_ID),
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }


    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Client_Code_Search(string Owner)
    {
        string Sqlstr = @"SELECT BUSINESSNAME,ID FROM BusinessData where Type = '保留'  AND Owner = @Owner";
        var a = DBTool.Query<ClassTemplate>(Sqlstr, new { Owner = Owner}).ToList().Select(p => new
        {
            A = p.BUSINESSNAME,
            B = p.ID,
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string ShowClientData(string value)
    {
        string Sqlstr = @"SELECT *  FROM BusinessData WHERE PID=@value";
        var a = DBTool.Query<ClassTemplate>(Sqlstr, new { value = value }).ToList().Select(p => new
        {
            A = Value(p.PID),
            C = Value(p.BUSINESSNAME),
        });
        string outputJson = JsonConvert.SerializeObject(a);

        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Data_Save(string Owner, string BusinessName, string RunTime, string OpinionProblem, string OpinionContent,
        string OpinionRemarks, string Work_Name, string ScheduleStatus, string flag)
    {
        if (String.IsNullOrEmpty(BusinessName))
        {
            return JsonConvert.SerializeObject(new { status = "請選擇【客戶】" });
        }
        if (String.IsNullOrEmpty(RunTime))
        {
            return JsonConvert.SerializeObject(new { status = "請選擇【預計時程 】" });
        }
        if (String.IsNullOrEmpty(OpinionProblem))
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【問題處理 】" });
        }
        if (String.IsNullOrEmpty(OpinionContent))
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【內容 】" });
        }
        if (String.IsNullOrEmpty(ScheduleStatus))
        {
            return JsonConvert.SerializeObject(new { status = "請選擇【進度狀態 】" });
        }
        string SetupTime = DateTime.Now.ToString("yyyy/MM/dd");
        string UploadTime = DateTime.Now.ToString("yyyy/MM/dd");
        string Sqlstr = @"INSERT INTO DailyProgress ( Owner, BusinessName, RunTime, OpinionProblem, OpinionContent, " +
                " OpinionRemarks, Work_Name, ScheduleStatus,SetupTime,UploadTime,flag) " +
                " VALUES (@Owner, @BusinessName, @RunTime, @OpinionProblem, @OpinionContent, " +
                " @OpinionRemarks, @Work_Name, @ScheduleStatus, @SetupTime ,@UploadTime, @flag  ) ";
        var a = DBTool.Query<ClassTemplate>(Sqlstr, new
        {
            Owner= Owner,
            BusinessName = BusinessName,
            RunTime = RunTime,
            OpinionProblem = OpinionProblem,
            OpinionContent = OpinionContent,
            OpinionRemarks = OpinionRemarks,
            Work_Name = Work_Name,
            ScheduleStatus = ScheduleStatus,
            SetupTime = SetupTime,
            UploadTime = UploadTime,
            flag = flag
        });
        return JsonConvert.SerializeObject(new { status = "新增完成！" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string List_Dispatch_Team()
    {
        string Sqlstr = @"SELECT DISTINCT Agent_Company FROM DispatchSystem WHERE Agent_Status = '在職' ORDER BY Agent_Company ";
        var a = DBTool.Query<ClassTemplate>(Sqlstr).ToList().Select(p => new
        {
            A = p.Agent_Company
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }
    //----------------------------------------行事曆----------------------------------------------

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string GetClassGroup(string Owner, string Permission,DateTime start, DateTime end)
    {
        string sqlstr = "";
        if (Permission == "部門主管" || Permission == "管理人員")
        {
            sqlstr = @"select PID, Work_Name,"+
                                     "flag as value," +
                                    "BusinessName as title," +
                                    "CONVERT(varchar(100), SetupTime, 111) as start, " +
                                    "CONVERT(varchar(100), RunTime, 111) as 'end'" +
                                     " FROM DailyProgress";
        }
        else
        {
            sqlstr = @"select  PID, Work_Name," +
                                    "flag as value," +
                                    "BusinessName as title," +
                                    "CONVERT(varchar(100), SetupTime, 111) as start, " +
                                    "CONVERT(varchar(100), RunTime, 111) as 'end'" +
                                     " FROM DailyProgress WHERE Owner = '"+Owner+"'";
        }
        var a = DBTool.Query<T_0030010002>(sqlstr, new      //行事曆案件整理
        {
            startDate = start,
            ednDate = end,
        });
        var b = a.ToList().Select(p => new
        {
            PID = p.PID,
            title = p.title+"-"+p.Work_Name,
            start = p.start.ToString("yyyy-MM-dd"),
            end = p.end.ToString("yyyy-MM-dd"),
            value = p.value
        });
        return JsonConvert.SerializeObject(b);            //  6/8 PM1600 之前的版本

    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string GetClassScheduleList(string PID)     
    {
        string sqlstr = @" SELECT SetupTime, UploadTime, BusinessName, Work_Name, RunTime, ScheduleStatus FROM DailyProgress WHERE PID = @PID";
        var a = DBTool.Query<Location_str>(sqlstr, new { PID = PID });
        var b = a.ToList().Select(p => new
        {
            SetupTime = p.SetupTime.ToString("yyyy-MM-dd"),
            UploadTime = p.UploadTime.ToString("yyyy-MM-dd"),
            BusinessName = p.BusinessName,
            Work_Name = p.Work_Name,
            RunTime = p.RunTime.ToString("yyyy-MM-dd"),
            ScheduleStatus = p.ScheduleStatus
        });
        return JsonConvert.SerializeObject(b);
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string GetALLSchedule(string Owner, string Permission)
    {
        string sqlstr = "";
        if (Permission == "部門主管" || Permission == "管理人員")
        {
            sqlstr  = @" SELECT BusinessName, Work_Name, RunTime, ScheduleStatus, PID FROM DailyProgress";
        }
        else
        {
            sqlstr = @" SELECT BusinessName, Work_Name, RunTime, ScheduleStatus, PID FROM DailyProgress WHERE Owner = '"+Owner+"'";
        }
        var a = DBTool.Query<Location_str>(sqlstr).ToList().Select(p => new
        {
            SetupTime = p.SetupTime.ToString("yyyy-MM-dd"),
            UploadTime = p.UploadTime.ToString("yyyy-MM-dd"),
            BusinessName = p.BusinessName,
            Work_Name = p.Work_Name,
            RunTime = p.RunTime.ToString("yyyy-MM-dd"),
            ScheduleStatus = p.ScheduleStatus,
            PID = p.PID,
        });
        return JsonConvert.SerializeObject(a);
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string LoadDaily(string PID)
    {
        string Sqlstr = @"SELECT PID, BusinessName, RunTime,OpinionProblem,OpinionContent, OpinionRemarks, Work_Name, ScheduleStatus, CancelCause FROM DailyProgress WHERE PID = @PID";
        var a = DBTool.Query<Location_str>(Sqlstr, new { PID = PID });
        var b = a.ToList().Select(p => new
        {
            PID = p.PID,
            BusinessName = p.BusinessName,
            RunTime = p.RunTime,
            OpinionProblem = p.OpinionProblem,
            OpinionContent = p.OpinionContent,
            OpinionRemarks = p.OpinionRemarks,
            Work_Name = p.Work_Name,
            ScheduleStatus = p.ScheduleStatus,
            CancelCause = p.CancelCause
        });
        return JsonConvert.SerializeObject(b);
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string UpdateDaily(string PID, string BusinessName, string OpinionProblem, string OpinionContent, string OpinionRemarks, string ScheduleStatus, string flag, string CancelCause)
        
    {
        if (String.IsNullOrEmpty(BusinessName))
        {
            return JsonConvert.SerializeObject(new { status = "請選擇【客戶】" });
        }
        if (String.IsNullOrEmpty(OpinionProblem))
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【問題處理 】" });
        }
        if (String.IsNullOrEmpty(OpinionContent))
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【內容 】" });
        }
        if (String.IsNullOrEmpty(ScheduleStatus))
        {
            return JsonConvert.SerializeObject(new { status = "請選擇【進度狀態 】" });
        }
        string sqlstr = @"UPDATE DailyProgress SET OpinionProblem = @OpinionProblem,  OpinionContent = @OpinionContent, OpinionRemarks  = @OpinionRemarks," +
                                        "ScheduleStatus = @ScheduleStatus, UploadTime = @UploadTime, flag = @flag, CancelCause = @CancelCause WHERE PID = @PID";
            string UploadTime = DateTime.Now.ToString("yyyy/MM/dd");
            DBTool.Query<ClassTemplate>(sqlstr, new
            {
                PID =PID,
                BusinessName = BusinessName,
                OpinionProblem = OpinionProblem,
                OpinionContent = OpinionContent,
                OpinionRemarks = OpinionRemarks,
                ScheduleStatus = ScheduleStatus,
                UploadTime = UploadTime,
                flag = flag,
                CancelCause = CancelCause
            });
            return JsonConvert.SerializeObject(new { status = "修改完成！" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string ViewPDF(string PID)
    {
        string sqlstr = @"SELECT  FileName, SYSID FROM DailyPath WHERE PID= @PID";
        var a = DBTool.Query<Location_str>(sqlstr, new {PID = PID }).ToList().Select(p => new
        {
            FileName = p.FileName,
            SYSID = p.SYSID
        });
        return JsonConvert.SerializeObject(a);
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string DeleteFile(string SYSID)
    {
        string sqlstr = @"DELETE FROM DailyPath WHERE SYSID = @SYSID";
        var a = DBTool.Query<Location_str>(sqlstr, new { SYSID = SYSID }).ToList().Select(p => new
        {
            SYSID = SYSID,
        });
        return JsonConvert.SerializeObject(new { status = "刪除成功！" });
    }

    public static string Value(string value)        // 當值為null時跳過  非 null 時去除後方空白
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = value.Trim();
        }
        else value = "";
        return value;
    }

    public class Location_str
    {
        public string PID { get; set; }
        public string SYSID { get; set; }
        public DateTime SetupTime { get; set; }
        public DateTime UploadTime { get; set; }
        public string BusinessName { get; set; }
        public string OpinionProblem { get; set; }
        public string OpinionContent { get; set; }
        public string OpinionRemarks { get; set; }
        public string Work_Name { get; set; }
        public DateTime RunTime { get; set; }
        public string ScheduleStatus { get; set; }
        public string CancelCause { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }
    public class T_0030010099
    {
        public string ID { get; set; }
        public string Contact_ADDR { get; set; }

        public string SetupDate { get; set; }
        public string UpdateDate { get; set; }
        public string BUSINESSNAME { get; set; }
        public string PID { get; set; }
        public string Type { get; set; }
        public string APPNAME { get; set; }
        public string CONTACT_ADDR { get; set; }

        public string Warranty_Date { get; set; }
        public string Warr_Time { get; set; }

        public string Case_ID { get; set; }
        public string SetupTime { get; set; }
        public string Creat_Agent { get; set; }
        public string ADDR { get; set; }
        public string APP_EMAIL { get; set; }
        public string OpinionContent { get; set; }
        public string Handle_Agent { get; set; }

        public string UploadTime { get; set; }
        public string ReachTime { get; set; }
        public string UserID { get; set; }
        public string Features { get; set; }
        public string Contents { get; set; }
        public string Problem { get; set; }
        public string Remarks { get; set; }
        public string time_course { get; set; }
        public string Project { get; set; }
        public string Client_Name { get; set; }
        public string Status { get; set; }
        public string System_Status { get; set; }
    }
    public class T_0030010002
    {
        public string Work_Name { get; set; }
        public string PID { get; set; }
        public string title { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }
}
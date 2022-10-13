using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;

public partial class _Default2 : System.Web.UI.Page
{
    protected string str_time;
    protected string str_day;
    protected string str_type;
    protected string abc;
    static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected void Page_Load(object sender, EventArgs e)
    {
        //Check();
        if (!string.IsNullOrEmpty(Request.Params["date"]))
        {
            str_day = Request.Params["date"];
            str_type = Request.Params["type"];
            abc = Request.Params["abc"];
        }
        else
        {
            str_day = DateTime.Now.ToString("yyyy-MM-dd");
            str_type = "已儲存";
        }
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string GetClassGroup(DateTime start, DateTime end, string time)
    {
        //Check();
        string Agent_ID = HttpContext.Current.Session["UserID"].ToString();
        string Agent_LV = HttpContext.Current.Session["Agent_LV"].ToString();
        string Agent_Team = HttpContext.Current.Session["Agent_Team"].ToString();

        string sqlstr = @"select  SYSID, Price, Item, CreateDate, PID" +
             " FROM DimaxCallcenter.dbo.Test " +
             " --GROUP by Type_Value,Type,CONVERT(varchar(100), SetupTime, 111) ";      // 原始的不會 Group    CONVERT(varchar(100), SetupTime, 111) 要整串放進 Group 中

        var a = DBTool.Query<T_0030010002>(sqlstr, new      //行事曆案件整理
        {
            startDate = start,
            ednDate = end,
            Agent_Team = Agent_Team,
            Agent_ID = Agent_ID
        });

        var b = a.ToList().Select(p => new
        {
            title = p.Item,
            value = p.SYSID,
            start = p.CreateDate.ToString("yyyy-MM-dd"),
            id = p.PID
        });
        return JsonConvert.SerializeObject(b);            //  6/8 PM1600 之前的版本

    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string GetClassScheduleList(DateTime date, string Type)       //案件列表程式
    {
        //Check();
        string Agent_ID = HttpContext.Current.Session["UserID"].ToString();
        string Agent_LV = HttpContext.Current.Session["Agent_LV"].ToString();
        string Agent_Team = HttpContext.Current.Session["Agent_Team"].ToString();

        string sqlstr2 = @" select  *  " +
        " FROM Test " +
        " WHERE SYSID = @Type AND CONVERT(varchar(100), CreateDate, 111) = @date ";      //    AND date = @date

        var a = DBTool.Query<T_0030010002_2>(sqlstr2, new
        {
            date = date,
            //ednDate = end,        //不知道end該改啥 暫用date頂替
            Type = Type,
            Agent_Team = Agent_Team,
            Agent_ID = Agent_ID
        });

        var b = a.ToList().Select(p => new
        {
            SetupTime = p.CreateDate.ToString("MM/dd HH:mm"),
            Client_Name = p.PID,
            Price = p.Price,
            Project = p.Item,
            Type = p.SYSID,
        });

        return JsonConvert.SerializeObject(b);
    }
    public static string Value3(string value, string value2)        // 當value值為""  非 value=value2
    {
        if (string.IsNullOrEmpty(value))
        {
            value = value2.Trim();
        }
        else
            value = value.Trim();
        return value;
    }
    public class T_0030010002
    {
        public string Item { get; set; }
        public string PID { get; set; }
        public string SYSID { get; set; }
        public DateTime CreateDate { get; set; }
    }
    public class T_0030010002_2
    {
        public string title { get; set; }
        public string end { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string Case_ID { get; set; }
        public string ReplyType { get; set; }
        public string OpinionSubject { get; set; }
        public string SYSID { get; set; }
        public string Urgency { get; set; }
        public string ID { get; set; }
        public string Cust_Name { get; set; }
        public DateTime start { get; set; }
        public DateTime Upload_Time { get; set; }
        public DateTime EstimatedFinishTime { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UploadTime { get; set; }
        public string Creat_Agent { get; set; }
        public string BUSINESSNAME { get; set; }
        public string OpinionType { get; set; }
        public string Handle_Agent { get; set; }
        public string Price { get; set; }
        public string Item { get; set; }
        public string time_course { get; set; }
        public string PID { get; set; }
        public string Client_Name { get; set; }
        public string Status { get; set; }
    }
}
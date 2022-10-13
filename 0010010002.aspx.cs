using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Data;
using Dapper;
using System.Diagnostics;
using System.Activities.Statements;
using Microsoft.SqlServer.Server;
using DocumentFormat.OpenXml.EMMA;
using System.IO;
using System.ServiceModel.Channels;
using System.Windows.Forms;
using System.Web;
using System.Net;

public partial class KeywordSearch : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        sdate.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        edate.Value = DateTime.Now.ToString("yyyy-MM-dd");

    }

    [WebMethod(EnableSession = true)]
    public static string searchresult(string Owner, string sdate, string edate,string key)
    {
        string sqlstr = "";
        if (String.IsNullOrEmpty(key))
        {
            sqlstr = @"SELECT * FROM [dbo].[KeywordSearch] 
                                    WHERE CONVERT(varchar(100), public_time, 23) between @sdate and @edate AND Owner = @Owner";
        }
        else
        {
            string keystring = "'" + key + "'"; //為了直接帶入SQL語法, 前後+上 ', 和, 前後+上'
            sqlstr = @"SELECT * FROM [dbo].[KeywordSearch] 
                                    WHERE CONVERT(varchar(100), public_time, 23) between @sdate and @edate AND Owner = @Owner AND keyword in(" + keystring + ")";
        }

            var a = DBTool.Query<resultitem>(sqlstr, new { Owner = Owner, sdate = sdate, edate = edate,}).ToList().Select(p => new
            {
                date = p.public_time.ToString("yyyy-MM-dd HH:mm:ss"),
                keyword = p.keyword,
                title = p.title,
                source = p.source,
                href = p.href.Trim() + ";" + p.title,
                browser = p.browser,
                sysid = p.SYSID,
            });

        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]
    public static string browser(string id)
    {
        string sqlstr = "UPDATE [dbo].[KeywordSearch] SET browser = browser+1  WHERE SYSID = @id";

        using (IDbConnection db = DBTool.GetConn())
        {
            db.Execute(sqlstr, new
            {
                id = id
            });
            db.Close();
        }


        string outputJson = JsonConvert.SerializeObject("");
        return outputJson;

    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string List_company(string Owner)
    {
        string sqlstr = "";
        if (Owner == "系統管理員")
        {
            sqlstr = @"SELECT BUSINESSNAME FROM BusinessData " +
           "where Type = '保留'";
        }
        else
        {
            sqlstr = @"SELECT BUSINESSNAME FROM BusinessData " +
            "where Type = '保留' AND Owner = '"+Owner+"'";
        }
        var a = DBTool.Query<ClassTemplate>(sqlstr).ToList().Select(p => new
        {         
            BUSINESSNAME = p.BUSINESSNAME
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SafeKey(string Owner, string Key)
    {
        if(String.IsNullOrEmpty(Key))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入關鍵字】" });
        }
        string sqlstr = @"INSERT INTO KeyWord(Owner, KeyWord, flag) VALUES(@Owner, @Key,@flag) ";
        DBTool.Query<ClassTemplate>(sqlstr, new { Owner = Owner, Key = Key, flag = "0" });
        return JsonConvert.SerializeObject(new { status = "新增關鍵字成功！" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string KeyListTable(string Owner)
    {
        string sqlstr = "";
        if (Owner == "系統管理員")
        {
            sqlstr = @"SELECT KeyWord FROM KeyWord";
        }
        else
        {
            sqlstr = @"SELECT KeyWord FROM KeyWord  WHERE Owner = '"+Owner+"'";
        }
        var a = DBTool.Query<resultitem>(sqlstr).ToList().Select(p => new
        {
            KeyWord = p.KeyWord
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string DeleteKeyWord(string KeyWord)
    {
        string sqlstr = @"DELETE FROM KeyWord WHERE KeyWord = @KeyWord ";
        DBTool.Query<ClassTemplate>(sqlstr, new { KeyWord = KeyWord });
        return JsonConvert.SerializeObject(new { status = "刪除關鍵字成功！" });
    }


    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string ListKeyWord(string Owner)
    {
        string sqlstr = "系統管理員";
        if(Owner == "系統管理員")
        {
            sqlstr = @"SELECT KeyWord FROM KeyWord";
        }
        else {
            sqlstr = @"SELECT KeyWord FROM KeyWord  WHERE Owner = '"+Owner+"'";
        }
        var a = DBTool.Query<resultitem>(sqlstr).ToList().Select(p => new
        {
            KeyWord = p.KeyWord
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SaveNews(string Owner, DateTime date, string keyword, string href)
    {
        string sqlstr = @"INSERT INTO SaveNews(date, keyword, href, Owner) VALUES(@date, @keyword, @href, @Owner)";
        DBTool.Query<ClassTemplate>(sqlstr, new { date = date, keyword = keyword, href = href, Owner = Owner});
        return JsonConvert.SerializeObject(new { status = "【保存新聞成功！】" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string RemoveNew(string href)
    {
        string sqlstr = @"DELETE FROM SaveNews WHERE href = @href";
        DBTool.Query<ClassTemplate>(sqlstr, new { href = href });
        return JsonConvert.SerializeObject(new { status = "【移除儲存成功！】" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SaveNewsList(string Owner)
    {
        string sqlstr = @"SELECT * FROM SaveNews WHERE Owner = @Owner  ";
        var a = DBTool.Query<resultitem>(sqlstr, new {Owner = Owner }).ToList().Select(p => new
        {
            date = p.date.ToString("yyyy-MM-dd HH:mm:ss"),
            keyword = p.keyword,
            href = p.href.Trim() + ";" + p.title,
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SelectKey(string Owner)
    {
        string sqlstr = @"SELECT Keyword as KeyWord FROM KeyWord WHERE Owner=@Owner AND flag='0'"+
            "union SELECT BUSINESSNAME FROM BusinessData where type='保留' AND Owner=@Owner AND flag='0'";

        var a = DBTool.Query<resultitem>(sqlstr, new { Owner = Owner}).ToList().Select(p => new {
            KeyWord = p.KeyWord,
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SetFlag(string key, string Owner)
    {
        string sqlstr = @"UPDATE BusinessData SET flag='1' WHERE Owner = @Owner AND BUSINESSNAME = @key;UPDATE KeyWord SET flag='1' WHERE Owner = @Owner AND KeyWord = @key";
        DBTool.Query<ClassTemplate>(sqlstr, new { Owner = Owner, key=key});
        return JsonConvert.SerializeObject(sqlstr);
    }

    //搜尋失敗後將flag改回0
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string reductionFlag(string key, string Owner)
    {
        string sqlstr = @"UPDATE BusinessData SET flag='0' WHERE Owner = @Owner AND BUSINESSNAME = @key;UPDATE KeyWord SET flag='0' WHERE Owner = @Owner AND KeyWord = @key";
        DBTool.Query<ClassTemplate>(sqlstr, new { Owner = Owner, key = key });
        return JsonConvert.SerializeObject(sqlstr);
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SetAutoSend(string Owner)
    {
        string sqlstr = @"SELECT BUSINESSNAME, PID,cycle,day FROM BusinessData WHERE Owner = @Owner AND Type= '保留'";
        var a = DBTool.Query<ClassTemplate>(sqlstr, new { Owner = Owner }).ToList().Select(p => new
        {
            BUSINESSNAME = p.BUSINESSNAME,
            PID = p.PID,
            cycle = p.cycle,
            day = p.day
        }) ;
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string AutoSend_insertSQL(string businessname, string cycle, string day, string Owner)
    {
        string sqlstr4 = @"UPDATE BusinessData SET cycle=@cycle, day=@day WHERE Owner=@Owner AND BUSINESSNAME=@businessname AND Type='保留'";
        DBTool.Query<ClassTemplate>(sqlstr4, new { cycle = cycle, day = day, businessname = businessname, Owner = Owner });

        string sqlstr = @"INSERT INTO SendNewSchedule(businessname,cycle,day,Owner) VALUES(@businessname, @cycle,@day, @Owner)";
        string sqlstr2 = @"SELECT * from SendNewSchedule WHERE businessname=@businessname and Owner = @Owner";
        string twoweek = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");
        if (cycle == "兩周")
        {
            sqlstr = @"INSERT INTO SendNewSchedule(businessname,cycle,day,Owner,senddate) VALUES(@businessname, @cycle,@day, @Owner, "+twoweek+")";
        }
        var a = DBTool.Query<ClassTemplate>(sqlstr2, new { Owner = Owner, businessname = businessname }).ToList();
        if (a.Any())
        {
             sqlstr = @"UPDATE SendNewSchedule SET cycle=@cycle,day=@day WHERE Owner=@Owner AND businessname = @businessname";
            if (cycle == "兩周")
            {
                sqlstr = @"UPDATE SendNewSchedule SET cycle=@cycle,day=@day, senddate='"+twoweek+"' WHERE Owner=@Owner AND businessname = @businessname";
            }
        }
        DBTool.Query<ClassTemplate>(sqlstr, new {
            businessname = businessname,
            cycle = cycle,
            day = day,
            Owner = Owner,
        });
        return JsonConvert.SerializeObject(new { status = "【設定成功！】" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string LoadNews(string Owner, string BUSINESSNAME)
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        string sqlstr = @"SELECT SYSID, title,href FROM AutoSendSearch WHERE CONVERT(varchar(100), public_time, 23) = CONVERT(varchar(100), GETDATE(), 23)" +
                                    "AND Owner = @Owner AND bs_Owner = @keyword AND Type IS NULL";
        var a = DBTool.Query<resultitem>(sqlstr, new { Owner = Owner,  keyword = BUSINESSNAME }).ToList().Select(p => new
        {
            SYSID = p.SYSID,
            href = p.href.Trim() + ";" + p.title,
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SetAutoType(string Owner, string SYSID)
    {
        string senddate = DateTime.Now.ToString();
        string sqlstr = @"UPDATE AutoSendSearch SET Type='已加入' WHERE Owner=@Owner AND  SYSID = @SYSID";
        DBTool.Query<ClassTemplate>(sqlstr, new
        {
            Owner = Owner,
            SYSID = SYSID
        });
        return JsonConvert.SerializeObject(new { status = "【已加入！】" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Save_News_List(string Owner, string LoadNews_BUSINESSNAME)
    {
        string sqlstr = @"SELECT SYSID, title,href FROM AutoSendSearch WHERE Owner = @Owner AND keyword = @keyword AND Type= '已加入'";
        var a = DBTool.Query<resultitem>(sqlstr, new { Owner = Owner, keyword = LoadNews_BUSINESSNAME }).ToList().Select(p => new
        {
            SYSID = p.SYSID,
            href = p.href.Trim() + ";" + p.title,
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Remove_Save_New(string Owner, string SYSID)
    {
        string senddate = DateTime.Now.ToString();
        string sqlstr = @"UPDATE AutoSendSearch SET Type=NULL WHERE Owner=@Owner AND  SYSID = @SYSID";
        DBTool.Query<ClassTemplate>(sqlstr, new
        {
            Owner = Owner,
            SYSID = SYSID
        });
        return JsonConvert.SerializeObject(new { status = "【移除成功！】" });
    }


    public class resultitem
    {
        public string SYSID { get; set; }
        public string createdate { get; set; }
        public string keyword { get; set; }
        public string KeyWord { get; set; }
        public string title { get; set; }
        public string href { get; set; }
        public string source { get; set; }
        public int browser { get; set; }
        public DateTime date { get; set; }
        public DateTime public_time { get; set; }
    }


}
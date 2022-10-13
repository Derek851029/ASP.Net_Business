using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;

public partial class _0010010001 : System.Web.UI.Page
{
    static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private object PubConstant;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Check();
        }
    }

    public static void Check()
    {
        string Check = JASON.Check_ID("0060010001.aspx");
        if (Check == "NO")
        {
            System.Web.HttpContext.Current.Response.Redirect("~/Default.aspx");
        }
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string GetPartnerList(string Owner,string Permission)
    {
        string sqlstr = "";
        if (Permission == "部門主管" || Permission == "管理人員")
        {
            sqlstr = @"SELECT BUSINESSNAME, PID, CONTACT_ADDR,CONTACT,CONTACT_PHONE,COMPANY_PHONE FROM BusinessData " +
                            "where Type = '保留'";
        }
        else
        {
            sqlstr = @"SELECT BUSINESSNAME, PID, CONTACT_ADDR,CONTACT,CONTACT_PHONE,COMPANY_PHONE FROM BusinessData " +
                            "where Type = '保留' AND Owner = '" + Owner + "'";
        }

        var a = DBTool.Query<T_0010010001>(sqlstr).ToList().Select(p => new
            {
                BUSINESSNAME = p.BUSINESSNAME,
                PID = p.PID,
                CONTACT_ADDR = p.CONTACT_ADDR,
                CONTACT = p.CONTACT,
                CONTACT_PHONE = p.CONTACT_PHONE,
                COMPANY_PHONE = p.COMPANY_PHONE
            });
            string outputJson = JsonConvert.SerializeObject(a);
            return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Delete(string PID)
    {
        string sqlstr = @"update BusinessData set Type = '刪除' WHERE ID = @PID ";
        DBTool.Query<ClassTemplate>(sqlstr, new { PID = PID });
        return JsonConvert.SerializeObject(new { status = "success" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string URL(string ID)
    {
        //Check();
        //PID = PID.Trim();
        string error = "傳送系統參數錯誤，請再嘗試或詢問管理人員，謝謝。";
        if (JASON.IsInt(ID) != true)
        {
            return JsonConvert.SerializeObject(new { status = error + "_1" });
        }

        //       if (PID.Length > 16 || PID.Length < 1)        {
        //           return JsonConvert.SerializeObject(new { status = error + "_2" });        }

        /*       if (PID != "0")
               {
                   string sqlstr = @"SELECT TOP 1 PID FROM [DimaxCallcenter].[dbo].[BusinessData] WHERE PID=@PID ";
                   var a = DBTool.Query<ClassTemplate>(sqlstr, new { PID = PID });

                   if (!a.Any())
                   {
                       return JsonConvert.SerializeObject(new { status = "查無【" + PID + "】此編號客戶。", type = "no" });
                   };
               };  //*/
        string str_url = "../0010010000.aspx?seqno=" + ID;         //打開0060010000 並放入同PID號的資料
        return JsonConvert.SerializeObject(new { status = str_url, type = "ok" });
    }

    // 預定修改執行部分
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Load_Data(string PID)                                                       // 新增 讀CaseData 案件資料
    {
        string sqlstr = @"SELECT *,  Saturday_Work as Flag_1, Sunday_Work as Flag_2 FROM BusinessData WHERE PID=@PID";
        var lcd = DBTool.Query<ClassTemplate>(sqlstr, new { PID = PID }).ToList().Select(p => new
        {
            BUSINESSNAME = p.BUSINESSNAME,
            BUSINESSNAME_EN = p.BUSINESSNAME_EN,
            ID = p.ID,
            CONTACT = p.CONTACT,                                //APP_SUBTITLE = p.APP_SUBTITLE,
            EMAIL = p.EMAIL,
            CONTACT_ADDR = p.CONTACT_ADDR,
            SetupDate = p.SetupDate,                        //APP_SUBTITLE_2 = p.APP_SUBTITLE_2,
            UpdateDate = p.UpdateDate,
            CONTACT_PHONE = p.CONTACT_PHONE,
            COMPANY_PHONE = p.COMPANY_PHONE,
            FAX_PHONE = p.FAX_PHONE,
            Department= p.Department,
            CUSTOMER_REMARK = p.CUSTOMER_REMARK,
            COMPANY_REMARK = p.COMPANY_REMARK,
            Check_Saturday = Re_Checkbox(p.Flag_1),
            Check_Sunday = Re_Checkbox(p.Flag_2),
            ojkeyword = p.ojkeyword,
            ojkeyword2 = p.ojkeyword2,
            ojkeyword3 = p.ojkeyword3,
        });

        string outputJson = JsonConvert.SerializeObject(lcd);
        return outputJson;
    }
    /*
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Check_Menu(string Flag, string TREE_ID, string ROLE_ID)
    {
        Check();
        string Agent_ID = HttpContext.Current.Session["UserID"].ToString();
        if (Flag.Length > 1)
        {
            return JsonConvert.SerializeObject(new { status = "傳送系統參數錯誤，請再嘗試或詢問管理人員，謝謝。" });
        }

        if (TREE_ID.Length > 10)
        {
            return JsonConvert.SerializeObject(new { status = "傳送系統參數錯誤，請再嘗試或詢問管理人員，謝謝。" });
        }

        if (ROLE_ID.Length > 10)
        {
            return JsonConvert.SerializeObject(new { status = "傳送系統參數錯誤，請再嘗試或詢問管理人員，謝謝。" });
        }
        string sqlstr = "";

        //============= 驗證 權限代碼有無被竄改 =============
        sqlstr = @"SELECT TOP 1 * FROM ROLELIST WHERE ROLE_ID=@ROLE_ID";
        var chk = DBTool.Query<CMS_0060010000>(sqlstr, new { ROLE_ID = ROLE_ID });
        if (!chk.Any())
        {
            return JsonConvert.SerializeObject(new { status = "無此系統參數，請再嘗試或詢問管理人員，謝謝。" });
        }
        //============= 驗證 權限代碼有無被竄改 =============

        //============= 驗證 選單編號有無被竄改 =============
        sqlstr = @"SELECT TOP 1 * FROM PROGLIST WHERE TREE_ID=@TREE_ID";
        chk = DBTool.Query<CMS_0060010000>(sqlstr, new { TREE_ID = TREE_ID });
        if (!chk.Any())
        {
            return JsonConvert.SerializeObject(new { status = "無此系統參數，請再嘗試或詢問管理人員，謝謝。" });
        }
        //============= 驗證 選單編號有無被竄改 =============

        if (Flag == "1")
        {
            sqlstr = @"DELETE FROM ROLEPROG WHERE Role_ID=@ROLE_ID AND TREE_ID=@TREE_ID";
            Flag = "系統選單關閉完成。";
        }
        else
        {
            sqlstr = @"INSERT INTO ROLEPROG ( Role_ID, TREE_ID, UpDateUser, UpDateDate ) VALUES(@ROLE_ID, @TREE_ID, @Agent_ID, @DateTime)";
            Flag = "系統選單開啟完成。";
        }

        try
        {
            using (IDbConnection conn = DBTool.GetConn())
            {
                conn.Execute(sqlstr, new { TREE_ID = TREE_ID, ROLE_ID = ROLE_ID, Agent_ID = Agent_ID, DateTime = DateTime.Now });
            }
            return JsonConvert.SerializeObject(new { status = Flag });
        }
        catch (Exception err)
        {
            return JsonConvert.SerializeObject(new { status = "傳送系統參數錯誤，請再嘗試或詢問管理人員，謝謝。" });
        }
    }   */

    //============= 建新資料用=============    
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Safe(string Owner, int Flag, string BUSINESSNAME, string BUSINESSNAME_EN, string ID, string CONTACT,
        string CONTACT_PHONE, string EMAIL, string CONTACT_ADDR, string COMPANY_PHONE, string FAX_PHONE,
        string Department, string CUSTOMER_REMARK, string COMPANY_REMARK, string UpDateDate, string SetupDate, string C_Saturday, string C_Sunday,
        string ojkeyword, string ojkeyword2, string ojkeyword3
        )
    {
        if (String.IsNullOrEmpty(BUSINESSNAME))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入客戶名稱】" });
        }
        if (String.IsNullOrEmpty(BUSINESSNAME_EN))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入職稱】" });
        }
        if (String.IsNullOrEmpty(ID))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入統一編號】" });
        }
        if (String.IsNullOrEmpty(CONTACT))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入聯絡人】" });
        }
        if (String.IsNullOrEmpty(CONTACT_PHONE))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入行動電話】" });
        }

        if (BUSINESSNAME.Length > 50)
        {
            return JsonConvert.SerializeObject(new { status = "【客戶名稱】不能超過５０個字元。" });
        }

        if (ID.Length > 8)
        {
            return JsonConvert.SerializeObject(new { status = "【統一編號】不能超過８個字元。" });
        }
        if (CONTACT.Length > 50)
        {
            return JsonConvert.SerializeObject(new { status = "【聯絡人】不能超過５０個字元。" });
        }
        if (CONTACT_PHONE.Length > 50)
        {
            return JsonConvert.SerializeObject(new { status = "【行動電話】不能超過５０個字元。" });
        }

        if (Flag == 0)
        {
            string SetupTime = DateTime.Now.ToString("yyyy/MM/dd");
            //string UpDateDate = DateTime.Now.ToString("yyyy/MM/dd");
            string sqlstr = @"INSERT INTO BusinessData (Owner, BUSINESSNAME, BUSINESSNAME_EN, ID, CONTACT, CONTACT_PHONE, EMAIL, CONTACT_ADDR, COMPANY_PHONE," +
                                        "FAX_PHONE, CUSTOMER_REMARK, COMPANY_REMARK,  SetupDate, UpDateDate, Department, Saturday_Work, Sunday_Work, flag,cycle,day,ojkeyword,ojkeyword2,ojkeyword3)" +
                                       " VALUES(@Owner, @BUSINESSNAME, @BUSINESSNAME_EN, @ID,@CONTACT, @CONTACT_PHONE, @EMAIL, @CONTACT_ADDR, @COMPANY_PHONE," +
                                         "@FAX_PHONE, @CUSTOMER_REMARK, @COMPANY_REMARK, @SetupDate, @UpDateDate, @Department, @C_Saturday, @C_Sunday, @flag,@cycle,@day,@ojkeyword,@ojkeyword2,@ojkeyword3)";
            var a = DBTool.Query<ClassTemplate>(sqlstr, new

            {
                Owner = Owner,
                BUSINESSNAME = BUSINESSNAME,
                BUSINESSNAME_EN = BUSINESSNAME_EN,
                ID = ID,
                CONTACT = CONTACT,
                CONTACT_PHONE = CONTACT_PHONE,
                EMAIL = EMAIL,
                CONTACT_ADDR = CONTACT_ADDR,
                COMPANY_PHONE = COMPANY_PHONE,
                FAX_PHONE = FAX_PHONE,
                CUSTOMER_REMARK = CUSTOMER_REMARK,
                COMPANY_REMARK = COMPANY_REMARK,
                UpDateDate = UpDateDate,
                flag = "0",
                cycle = "無",
                day = "選擇日期",
                SetupDate = Test(SetupDate),
                Department = Test(Department),
                C_Saturday = Checkbox(C_Saturday),
                C_Sunday = Checkbox(C_Sunday),
                ojkeyword = ojkeyword,
                ojkeyword2 = ojkeyword2,
                ojkeyword3= ojkeyword3,
            }) ;
            int NewYear = DateTime.Now.AddMonths(3).Year;
            int NewMonth = DateTime.Now.AddMonths(3).Month;
            DateTime CheckDate = new DateTime(NewYear, NewMonth, 1);
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "傳送系統參數錯誤，請再嘗試或詢問管理人員，謝謝。" });
        }
        return JsonConvert.SerializeObject(new { status = "【新增完成！】" });
    }

    //============= 修改客戶資料 (Flag = 1)=============    

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string New(int Flag, string BUSINESSNAME, string BUSINESSNAME_EN, string ID, string CONTACT,
        string CONTACT_PHONE, string EMAIL, string CONTACT_ADDR, string COMPANY_PHONE, string FAX_PHONE,
        string CUSTOMER_REMARK, string COMPANY_REMARK, string UpDateDate, string SetupDate, string C_Saturday, string C_Sunday,
        string ojkeyword, string ojkeyword2, string ojkeyword3,string PID
        )   //共41個  
    {
        if (String.IsNullOrEmpty(BUSINESSNAME))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入客戶名稱】" });
        }
        if (String.IsNullOrEmpty(BUSINESSNAME_EN))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入職稱】" });
        }
        if (String.IsNullOrEmpty(ID))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入統一編號】" });
        }
        if (String.IsNullOrEmpty(CONTACT))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入聯絡人】" });
        }
        if (String.IsNullOrEmpty(CONTACT_PHONE))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入行動電話】" });
        }

        if (BUSINESSNAME.Length > 50)
        {
            return JsonConvert.SerializeObject(new { status = "【客戶名稱】不能超過５０個字元。" });
        }

        if (ID.Length > 8)
        {
            return JsonConvert.SerializeObject(new { status = "【統一編號】不能超過８個字元。" });
        }
        if (CONTACT.Length > 50)
        {
            return JsonConvert.SerializeObject(new { status = "【聯絡人】不能超過５０個字元。" });
        }
        if (CONTACT_PHONE.Length > 50)
        {
            return JsonConvert.SerializeObject(new { status = "【行動電話】不能超過５０個字元。" });
        }
        if (Flag == 1)
        {
            string Sqlstr = @"UPDATE BusinessData SET  BUSINESSNAME = @BUSINESSNAME, " +
                "BUSINESSNAME_EN = @BUSINESSNAME_EN, ID = @ID,  CONTACT = @CONTACT, " +
                 "CONTACT_PHONE = @CONTACT_PHONE, EMAIL = @EMAIL, CONTACT_ADDR = @CONTACT_ADDR, " +
                 "COMPANY_PHONE = @COMPANY_PHONE, FAX_PHONE = @FAX_PHONE, CUSTOMER_REMARK = @CUSTOMER_REMARK, COMPANY_REMARK = @COMPANY_REMARK," +
                 "UpDateDate = @UpDateDate, Saturday_Work = @C_Saturday, Sunday_Work = @C_Sunday,ojkeyword = @ojkeyword, ojkeyword2 = @ojkeyword2, ojkeyword3= @ojkeyword3 Where PID=@PID";

            var a = DBTool.Query<ClassTemplate>(Sqlstr, new
            {
                BUSINESSNAME = BUSINESSNAME,
                BUSINESSNAME_EN = BUSINESSNAME_EN,
                ID = ID,
                CONTACT = CONTACT,
                CONTACT_PHONE = CONTACT_PHONE,
                EMAIL = EMAIL,
                CONTACT_ADDR = CONTACT_ADDR,
                COMPANY_PHONE = COMPANY_PHONE,
                FAX_PHONE = FAX_PHONE,
                CUSTOMER_REMARK = CUSTOMER_REMARK,
                COMPANY_REMARK = COMPANY_REMARK,
                UpDateDate = Test(UpDateDate),
                C_Saturday = Checkbox(C_Saturday),
                C_Sunday = Checkbox(C_Sunday),
                ojkeyword = ojkeyword,
                ojkeyword2 = ojkeyword2,
                ojkeyword3 = ojkeyword3,
                PID = PID
            });
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "傳送系統參數錯誤，請再嘗試或詢問管理人員，謝謝。" });
        }

        return JsonConvert.SerializeObject(new { status = "【修改完成！】" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Open_Flag(string SYS_ID, string Flag)
    {
        string error = "傳送系統參數錯誤，請再嘗試或詢問管理人員，謝謝。";
        string str_back;
        if (Flag == "0")
        {
            Flag = "1";
            str_back = "編號【" + SYS_ID + "】維護任務已啟用。";
        }
        else
        {
            Flag = "0";
            str_back = "編號【" + SYS_ID + "】維護任務已停用。";
        };
        string Sqlstr = @"SELECT * FROM [InSpecation_Dimax].[dbo].[Mission_Title] WHERE SYSID=@SYSID ";
        var a = DBTool.Query<T_0010010001>(Sqlstr, new { SYSID = SYS_ID });
        if (a.Count() > 0)
        {
            Sqlstr = @"UPDATE [InSpecation_Dimax].[dbo].[Mission_Title] SET "
                + " Flag=@Flag "
                + " WHERE SYSID=@SYSID ";
            using (IDbConnection db = DBTool.GetConn())
            {
                db.Execute(Sqlstr, new { Flag = Flag, SYSID = SYS_ID });
                db.Close();
            };
            return JsonConvert.SerializeObject(new { status = str_back, Flag = "1" });
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = error, Flag = "0" });
        }
    }
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string New_Title(string PID, string T_ID, string ADDR, string Name, string MTEL, string CycleTime, string Agent, string C_Name,
        string C_1, string C_2, string C_3, string C_4, string C_5, string C_6, string C_7, string C_8, string C_9, string C_10, string C_11, string C_12
        )  //    string PID2, 
    {
        string error = "傳送系統參數錯誤，請再嘗試或詢問管理人員，謝謝。";
        string UserID = HttpContext.Current.Session["UserID"].ToString();
        string UserIDNAME = HttpContext.Current.Session["UserIDNAME"].ToString();
        int i = 0;
        Name = Value(Name);
        MTEL = Value(MTEL);
        ADDR = Value(ADDR);
        C_Name = Value(C_Name);
        //===========================================================
        //if (String.IsNullOrEmpty(PID))
        //{
        //    return JsonConvert.SerializeObject(new { status = "請選擇【客戶】", Flag = "0" });
        //}
        //===========================================================
        if (!String.IsNullOrEmpty(C_Name))
        {
            i = Name.Length;
            if (Name.Length > 50)
            {
                return JsonConvert.SerializeObject(new { status = "【客戶名稱】不能超過５０個字元。", Flag = "0" });
            }
            else
            {
                Name = HttpUtility.HtmlEncode(Name.Trim());
                if (Name.Length != i)
                {
                    return JsonConvert.SerializeObject(new { status = error + "1", Flag = "0" });
                }
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【客戶名稱】", Flag = "0" });
        }
        //==========================================================
        if (!String.IsNullOrEmpty(T_ID))
        {
            if (!new string[] { "中華電信", "遠傳", "德瑪", "其他" }.Contains(T_ID))
            {
                return JsonConvert.SerializeObject(new { status = error + "2", Flag = "0" });
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請選擇【維護廠商】", Flag = "0" });
        }
        //==========================================================
        if (!String.IsNullOrEmpty(ADDR))
        {
            i = ADDR.Length;
            if (ADDR.Length > 200)
            {
                return JsonConvert.SerializeObject(new { status = "【維護地址】不能超過２００個字元。", Flag = "0" });
            }
            else
            {
                ADDR = HttpUtility.HtmlEncode(ADDR.Trim());
                if (ADDR.Length != i)
                {
                    return JsonConvert.SerializeObject(new { status = error + "3", Flag = "0" });
                }
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【維護地址】", Flag = "0" });
        }
        //===========================================================
        if (!String.IsNullOrEmpty(Name))
        {
            i = Name.Length;
            if (Name.Length > 15)
            {
                return JsonConvert.SerializeObject(new { status = "【聯絡人】不能超過１５個字元。", Flag = "0" });
            }
            else
            {
                Name = HttpUtility.HtmlEncode(Name.Trim());
                if (Name.Length != i)
                {
                    return JsonConvert.SerializeObject(new { status = error + "4", Flag = "0" });
                }
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【聯絡人】", Flag = "0" });
        }
        //===========================================================
        if (!String.IsNullOrEmpty(MTEL))
        {
            i = MTEL.Length;
            if (MTEL.Length > 25)
            {
                return JsonConvert.SerializeObject(new { status = "【聯絡電話】不能超過２５個字元。", Flag = "0" });
            }
            else
            {
                MTEL = HttpUtility.HtmlEncode(MTEL.Trim());
                if (MTEL.Length != i)
                {
                    return JsonConvert.SerializeObject(new { status = error + "5", Flag = "0" });
                }
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【聯絡電話】", Flag = "0" });
        }
        //==========================================================
        if (!String.IsNullOrEmpty(CycleTime))
        {
            if (!new string[] { "0", "1", "2", "3", "4", "5" }.Contains(CycleTime))
            {
                return JsonConvert.SerializeObject(new { status = error + "6", Flag = "0" });
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請選擇【巡查週期】", Flag = "0" });
        }
        //===========================================================   
        string CycleName;
        switch (CycleTime)
        {
            case "0":
                CycleName = "單月";
                break;
            case "1":
                CycleName = "雙月";
                break;
            case "2":
                CycleName = "每季";
                break;
            case "3":
                CycleName = "半年";
                break;
            case "4":
                CycleName = "每年";
                break;
            case "5":
                CycleName = "不維護";
                break;
            default:
                CycleName = "";
                break;
        };
        DateTime Time_01 = DateTime.Now;
        /*string sqlstr = @"INSERT INTO [InSpecation_Dimax].[dbo].[Mission_Title] "
            + " ( "
            + " Agent_Team, Agent_ID, Agent_Name, PID, PID2, T_ID, ADDR, Name, MTEL, Cycle, Cycle_Name, Create_ID, Create_Name, mission_name "       //
            + " ) "
            + "SELECT TOP 1 Agent_Team, Agent_ID, Agent_Name, @PID, @PID2, @T_ID, @ADDR, @Name, @MTEL, @Cycle, @CycleName, @Create_ID, @Create_Name, @C_Name "
            + "FROM [DimaxCallcenter].[dbo].[DispatchSystem] WHERE Agent_ID=@Agent_ID ";    //*/
        string sqlstr = @"INSERT INTO [InSpecation_Dimax].[dbo].[Mission_Title] "
            + " ( mission_name, PID, T_ID, ADDR, Name, MTEL, Cycle, Cycle_Name, Create_ID, Create_Name , "
            + " M_1, M_2, M_3, M_4, M_5, M_6, M_7, M_8, M_9, M_10, M_11, M_12 ) "       //
            + "values(@C_Name, @PID, @T_ID, @ADDR, @Name, @MTEL, @Cycle, @CycleName, @Create_ID, @Create_Name, "
            + " @M_1, @M_2, @M_3, @M_4, @M_5, @M_6, @M_7, @M_8, @M_9, @M_10, @M_11, @M_12)";
        using (IDbConnection db = DBTool.GetConn())
        {
            db.Execute(sqlstr, new
            {
                PID = PID,
                //PID2 = PID2,
                T_ID = T_ID,
                ADDR = ADDR,
                Name = Name,
                MTEL = MTEL,
                Cycle = CycleTime,
                CycleName = CycleName,
                //Agent_ID = Agent,
                Create_ID = UserID,
                Create_Name = UserIDNAME,
                C_Name = C_Name,
                M_1 = Checkbox(C_1),
                M_2 = Checkbox(C_2),
                M_3 = Checkbox(C_3),
                M_4 = Checkbox(C_4),
                M_5 = Checkbox(C_5),
                M_6 = Checkbox(C_6),
                M_7 = Checkbox(C_7),
                M_8 = Checkbox(C_8),
                M_9 = Checkbox(C_9),
                M_10 = Checkbox(C_10),
                M_11 = Checkbox(C_11),
                M_12 = Checkbox(C_12),
            });
            db.Close();
        };
        if (!string.IsNullOrEmpty(Agent))
        {
            sqlstr = @"select Top 1 Create_Date as Time_02 FROM [InSpecation_Dimax].[dbo].[Mission_Title] " +
            "order by [Create_Date] desc ";
            var list = DBTool.Query<ClassTemplate>(sqlstr);
            if (list.Any())
            {
                ClassTemplate schedule = list.First();
                Time_01 = schedule.Time_02;
            }
            //return JsonConvert.SerializeObject(new { status = "修改工程師失敗。 " + str_time, Flag = "1" }); 
            try
            {
                sqlstr = @"update [InSpecation_Dimax].[dbo].[Mission_Title] set "
                + " Mission_Title.Agent_ID = DispatchSystem.Agent_ID, "
                + " Mission_Title.Agent_Name = DispatchSystem.Agent_Name,  "
                + " Mission_Title.Agent_Team = DispatchSystem.Agent_Team "
                + " FROM [DispatchSystem] "
                + " WHERE Mission_Title.Create_Date = @Time and DispatchSystem.Agent_ID =@Agent";
                var b = DBTool.Query<ClassTemplate>(sqlstr, new
                {
                    Time = Time_01,
                    Agent = Agent
                });
            }
            catch (Exception er)
            {
                return JsonConvert.SerializeObject(new { status = "修改工程師失敗。 " + er, Flag = "1" });
            }
        }
        return JsonConvert.SerializeObject(new { status = "維護任務 新增完成。", Flag = "1" });
    }


    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SafeGroup(string Owner, string GroupName)
    {
        if (string.IsNullOrEmpty(GroupName))
        {
            return JsonConvert.SerializeObject(new { status = "【請輸入群組名稱】" });
        }
        string sqlstr = @"INSERT INTO BusinessGroup (Owner, GroupName)VALUES(@Owner, @GroupName)";
        var a = DBTool.Query<ClassTemplate>(sqlstr, new
        {
            Owner= Owner,
            GroupName = GroupName,
        });
        return JsonConvert.SerializeObject(new { status = "【新增成功】" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string GroupList(string Owner)
    {
        string sqlstr = "";
        if (Owner == "系統管理員")
        {
            sqlstr = @"SELECT PID,GroupName FROM BusinessGroup";
        }
        else
        {
            sqlstr = @"SELECT PID, GroupName FROM BusinessGroup WHERE Owner = '"+Owner+"'";
        }
        var a = DBTool.Query<T_0010010001>(sqlstr).ToList().Select(p => new
        {
            PID = p.PID,
            GroupName = p.GroupName
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string SearchGroup(string GROUPID, string Owner)
    {
        string sqlstr = "";
        if (Owner == "系統管理員")
        {
            sqlstr = @"SELECT BUSINESSNAME, EMAIL, CONTACT,  CONTACT_PHONE, COMPANY_PHONE FROM BusinessData WHERE BUSINESS_GROUP = @GROUPID";
        }
        else
        {
            sqlstr = @"SELECT BUSINESSNAME, EMAIL, CONTACT,  CONTACT_PHONE, COMPANY_PHONE FROM BusinessData WHERE BUSINESS_GROUP = @GROUPID AND Owner = '"+Owner+"'";
        }
        var a = DBTool.Query<T_0010010001>(sqlstr, new { GROUPID = GROUPID}).ToList().Select(p => new
        {
            BUSINESSNAME = p.BUSINESSNAME,
            EMAIL = p.EMAIL,
            CONTACT = p.CONTACT,
            CONTACT_PHONE = p.CONTACT_PHONE,
            COMPANY_PHONE = p.COMPANY_PHONE
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string CustomerList(string Owner, string GROUPID)
    {
        string sqlstr = "";
        if (Owner == "系統管理員")
        {
            sqlstr = @"SELECT BUSINESSNAME, CONTACT, BUSINESS_GROUP FROM BusinessData WHERE Type = '保留' AND BUSINESS_GROUP  ISNULL";
        }
        else 
        {
            sqlstr = @"SELECT BUSINESSNAME, CONTACT, BUSINESS_GROUP FROM BusinessData WHERE Type = '保留' AND Owner = '" + Owner+ "' AND BUSINESS_GROUP IS NULL";
        }
        var a = DBTool.Query<T_0010010001>(sqlstr).ToList().Select(p => new
        {
            BUSINESSNAME = p.BUSINESSNAME,
            CONTACT = p.CONTACT,
            BUSINESS_GROUP = p.BUSINESS_GROUP,
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string AddToCompany(string BUSINESSNAME, string GROUPID)
    {
        string sqlstr = @"UPDATE BusinessData SET BUSINESS_GROUP = @GROUPID WHERE BUSINESSNAME = @BUSINESSNAME";
        var a = DBTool.Query<T_0010010001>(sqlstr, new { BUSINESSNAME = BUSINESSNAME, GROUPID = GROUPID });
        return JsonConvert.SerializeObject(new { status = "【新增完成】" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string RemoveGroup(string BUSINESSNAME)
    {
        string sqlstr = @"UPDATE BusinessData SET BUSINESS_GROUP = null WHERE BUSINESSNAME = @BUSINESSNAME";
        DBTool.Query<ClassTemplate>(sqlstr, new { BUSINESSNAME = BUSINESSNAME });
        return JsonConvert.SerializeObject(new { status = "success" });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string DeleteGroup(string GROUPID)
    {
        string sqlstr = @"UPDATE BusinessData SET BUSINESS_GROUP = null WHERE BUSINESS_GROUP = @GROUPID DELETE FROM  BusinessGroup WHERE GroupName = @GROUPID"; 
        DBTool.Query<ClassTemplate>(sqlstr, new { GROUPID = GROUPID });
        return JsonConvert.SerializeObject(new { status = "success" });
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
            value = DateTime.Parse(value).ToString("yyyy/MM/dd hh:mm");
        }
        return value;
    }
    public static string Value3(string value)        // 當值為null時跳過  非 null 時改時間格式
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = DateTime.Parse(value).ToString("yyyy/MM/dd");
        }
        return value;
    }
    public static string Test(string value)        // 當值為null時跳過  非 null 時去除後方空白
    {
        if (!string.IsNullOrEmpty(value))
        {
            value = value.Trim();
            if (value == "1900/01/01 12:00")
            {
                value = "";
            }
        }
        return value;
    }
    public static string Checkbox(string value)        // 當值為null時跳過  非 null 時去除後方空白
    {
        if (value == "True")
        {
            value = "1";
        }
        else
            value = "0";
        return value;
    }
    public static string Re_Checkbox(string value)        // 當值為null時跳過  非 null 時去除後方空白
    {
        if (value == "1")
        {
            value = "checked";
        }
        else
            value = "";
        return value;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string ProcessExcel(string Owner, string CONTACT, string BUSINESSNAME, string Department, string BUSINESSNAME_EN, string COMPANY_PHONE, string EMAIL, 
        string FAX_PHONE, string CONTACT_PHONE, string CONTACT_ADDR, string COMPANY_REMARK, string ID)
    {
        string SetupDate = DateTime.Now.ToString("yyyy/MM/dd");
        string sqlstr = @"INSERT INTO BusinessData (Owner, CONTACT, BUSINESSNAME, Department, BUSINESSNAME_EN, COMPANY_PHONE,"+
                                    "EMAIL, FAX_PHONE, CONTACT_PHONE, CONTACT_ADDR, COMPANY_REMARK, SetupDate, ID)" +
                                    "VALUES(@Owner, @CONTACT, @BUSINESSNAME, @Department, @BUSINESSNAME_EN, @COMPANY_PHONE, @EMAIL, @FAX_PHONE,"+
                                    "@CONTACT_PHONE, @CONTACT_ADDR, @COMPANY_REMARK, @SetupDate, @ID) ";
        var a = DBTool.Query<T_0010010001>(sqlstr, new {
            Owner = Owner,
            CONTACT = CONTACT,
            BUSINESSNAME = BUSINESSNAME,
            Department = Department,
            BUSINESSNAME_EN = BUSINESSNAME_EN,
            COMPANY_PHONE = COMPANY_PHONE,
            EMAIL = EMAIL,
            FAX_PHONE = FAX_PHONE,
            CONTACT_PHONE= CONTACT_PHONE,
            CONTACT_ADDR = CONTACT_ADDR,
            COMPANY_REMARK = COMPANY_REMARK,
            SetupDate = SetupDate,
            ID = ID
        });
        return JsonConvert.SerializeObject(new { status = "【新增完成】" });
    }

    public class T_0010010001
    {
        public string PID { get; set; }
        public string BUSINESSNAME { get; set; }
        public string BUSINESSNAME_EN { get; set; }
        public string BUSINESS_GROUP { get; set; }
        public string GroupName { get; set; }
        public string GROUPID { get; set; }
        public string EMAIL { get; set; }
        public string ADDRESS { get; set; }
        public string CONTACT { get; set; }
        public string CONTACT_PHONE { get; set; }
        public string COMPANY_PHONE { get; set; }
        public string BUSINESSID { get; set; }
        public string ID { get; set; }
        public string BUS_CREATE_DATE { get; set; }
        public string APPNAME { get; set; }
        public string APP_SUBTITLE { get; set; }
        public string APP_EMAIL { get; set; }
        public string APP_MTEL { get; set; }
        public string APPNAME_2 { get; set; }
        public string APP_SUBTITLE_2 { get; set; }
        public string APP_MTEL_2 { get; set; }
        public string APP_EMAIL_2 { get; set; }
        public string REGISTER_ADDR { get; set; }
        public string CONTACT_ADDR { get; set; }
        public string APP_FTEL { get; set; }
        public string Mail_Type { get; set; }
        public string Group_Name_ID { get; set; }
        public string SED { get; set; }
        public string Warranty_Date { get; set; }
        public string Warr_Time { get; set; }
        public string Protect_Date { get; set; }
        public string Prot_Time { get; set; }
        public string Receipt_Date { get; set; }
        public string Receipt_PS { get; set; }
        public string Close_Out_Date { get; set; }
        public string Close_Out_PS { get; set; }
        public string Account_PS { get; set; }
        public string Information_PS { get; set; }
        public string SetupDate { get; set; }
        public string Del_Flag { get; set; }
        public string UpdateDate { get; set; }
        //0628 子公司
        public string Name { get; set; }
        public string ADDR { get; set; }
        public string Contac_ADDR { get; set; }

        public string Cycle_Name { get; set; }
        public string Start_Time { get; set; }
        public string End_Time { get; set; }
        public string bit { get; set; }
        public string Flag { get; set; }
        public string SYSID { get; set; }
        public DateTime Work_Time { get; set; }
        public DateTime Create_Date { get; set; }
    }
}
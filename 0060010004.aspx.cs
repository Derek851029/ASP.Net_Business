using Dapper;
using Newtonsoft.Json;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using log4net.Config;

public partial class _0060010004 : System.Web.UI.Page
{
    static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Check();
        }
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string GetPartnerList(string[] Array)
    {
        Check();
        string check = "";
        string sqlstr = "";
        string str_array = "";
        if (Array.Length != 0)
        {
            sqlstr = "Declare @Array table(Value nvarchar(20)) ";
            str_array = " AND Agent_Company IN ( SELECT * FROM @Array ) ";
            for (int i = 0; i < Array.Length; i++)
            {
                //=========================================
                check = @"SELECT TOP 1 SYSID FROM DispatchSystem WHERE Agent_Status != '離職' AND Agent_Company=@Agent_Company";
                var b = DBTool.Query<ClassTemplate>(check, new { Agent_Company = Array[i] });
                if (b.Any())
                {
                    sqlstr += "insert into @Array (Value) values ('" + Array[i] + "') ";
                };
                //=========================================
            }
        }

        //sqlstr += @"SELECT * FROM DispatchSystem WHERE Agent_ID !='' AND Agent_Status = '在職' " + str_array; //agentID就是員工編號, 在一般使用者不需要
        sqlstr += @"SELECT * FROM DispatchSystem WHERE  Agent_Status = '在職' " + str_array;
        var a = DBTool.Query<ClassTemplate>(sqlstr).ToList().Select(p => new
        {
            Role_ID = p.Role_ID,
            Agent_LV = p.Agent_LV,
            SYSID = p.SYSID,
            Agent_ID = p.Agent_ID.Trim(),//編號
            Agent_Name = p.Agent_Name.Trim(),//姓名
            Agent_Company = p.Agent_Company.Trim(),//所屬公司
            Agent_Team = p.Agent_Team.Trim()//所屬部門
        });

        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Agent_Company_List()
    {
        Check();
        string sqlstr = @"SELECT DISTINCT Agent_Company FROM DispatchSystem WHERE Agent_ID !='' AND Agent_Status = '在職' ORDER BY Agent_Company";
        var a = DBTool.Query<ClassTemplate>(sqlstr).ToList().Select(p => new
        {
            Agent_Company = p.Agent_Company.Trim()
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Agent_Team_List()
    {
        Check();
        string sqlstr = @"SELECT DISTINCT Agent_Team FROM DispatchSystem WHERE Agent_ID !='' AND Agent_Status = '在職' ORDER BY Agent_Team";
        var a = DBTool.Query<ClassTemplate>(sqlstr).ToList().Select(p => new
        {
            Agent_Team = p.Agent_Team.Trim()
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    //============= 帶入【系統選單權限】資訊 =============
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string ROLELIST_List()
    {
        Check();
        string sqlstr = @"SELECT ROLE_ID, ROLE_NAME FROM ROLELIST WHERE OPEN_DEL = 'Y' ";
        var a = DBTool.Query<ClassTemplate>(sqlstr).ToList().Select(p => new
        {
            ROLE_ID = p.Role_ID.Trim(),
            ROLE_NAME = p.ROLE_NAME.Trim()
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    //============= 帶入【人員】資訊 =============
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string DispatchSystem(string Agent_Name)
    {
        Check();
        string sqlstr = @"SELECT TOP 1 * FROM DispatchSystem WHERE Agent_Name = @Agent_Name AND Agent_Status = '在職' ";

        var a = DBTool.Query<ClassTemplate>(sqlstr, new { Agent_Name = Agent_Name }).ToList().Select(p => new
        {
            Agent_ID = p.Agent_ID,
            Agent_Name = p.Agent_Name,
            Agent_Company = p.Agent_Company,
            Agent_Team = p.Agent_Team,
            Agent_Mail = p.Agent_Mail,
            Agent_Co_TEL = p.Agent_Co_TEL,
            Agent_Phone_2 = p.Agent_Phone_2,
            //Agent_Phone_3 = p.Agent_Phone_3,
            UserID = p.UserID,
            Agent_LV = p.Agent_LV,
            Role_ID = p.Role_ID,
            //Agent_Code = p.Agent_Code,
            //Agent_MVPN = p.Agent_MVPN
        });
        string outputJson = JsonConvert.SerializeObject(a);
        return outputJson;
    }

    //============= 刪除【人員】資訊 =============
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Delete(string Agent_Name)
    {
        Check();
        string sqlstr = @"UPDATE DispatchSystem SET Agent_Status='離職', UpdateTime=@UpdateTime WHERE Agent_Name = @Agent_Name AND Agent_Status = '在職' ";
        DBTool.Query<ClassTemplate>(sqlstr, new { Agent_Name = Agent_Name, UpdateTime = DateTime.Now });
        return JsonConvert.SerializeObject(new { status = "success" });
    }

    //============= 新增【人員】資訊 =============
    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string New_Agent(string Agent_ID, string Agent_Name, string Agent_Company, string Agent_Team, string Agent_Mail,
        string Agent_Phone_2, string UserID, string Password, string Role_ID, string Agent_LV, string Agent_Co_TEL, string Flag)       
        //string Agent_Phone_3, string Agent_Code, string Agent_MVPN,
    {
        Check();
        string error = "傳送系統參數錯誤，請再嘗試或詢問管理人員，謝謝。";
        string back;
        string Sqlstr;
        string sql_pass = "";

        //驗證 Agent_ID 【人員編號】
        Agent_ID = Agent_ID.Trim();
        if (!String.IsNullOrEmpty(Agent_ID))
        {
            if (Agent_ID.Length > 10)
            {
                return JsonConvert.SerializeObject(new { status = "【人員編號】不能超過１０個字元。" });
            }
            else
            {
                if (JASON.IsNumeric(Agent_ID) != true)
                {
                    return JsonConvert.SerializeObject(new { status = "【人員編號】只能輸入數字。" });
                }
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【人員編號】" });
        }

        //驗證 Agent_Name 【人員姓名】
        Agent_Name = Agent_Name.Trim();
        if (!String.IsNullOrEmpty(Agent_Name))
        {
            if (Agent_Name.Length > 10)
            {
                return JsonConvert.SerializeObject(new { status = "【人員姓名】不能超過１０個字元。" });
            }
            else
            {
                if (JASON.IsNumericOrLetterOrChinese(Agent_Name) != true)
                {
                    return JsonConvert.SerializeObject(new { status = "【人員姓名】只能輸入數字、英文或中文。" });
                }
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【人員姓名】" });
        }

        //驗證 Agent_Company 【所屬公司】
        Agent_Company = Agent_Company.Trim();
        if (!String.IsNullOrEmpty(Agent_Company))
        {
            if (Agent_Company.Length > 20)
            {
                return JsonConvert.SerializeObject(new { status = "【所屬公司】不能超過２０個字元。" });
            }
            else
            {
                Agent_Company = HttpUtility.HtmlEncode(Agent_Company);
                if (Agent_Company.Length > 50)
                {
                    return JsonConvert.SerializeObject(new { status = error });
                }
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【所屬公司】" });
        }

        //驗證 Agent_Team 【所屬部門】
        Agent_Team = Agent_Team.Trim();
        if (!String.IsNullOrEmpty(Agent_Team))
        {
            if (Agent_Team.Length > 20)
            {
                return JsonConvert.SerializeObject(new { status = "【所屬部門】不能超過２０個字元。" });
            }
            else
            {
                Agent_Team = HttpUtility.HtmlEncode(Agent_Team);
                if (Agent_Team.Length > 50)
                {
                    return JsonConvert.SerializeObject(new { status = error });
                }
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【所屬部門】" });
        }

        //驗證 Agent_Co_TEL 【電話號碼】
/*        Agent_Co_TEL = Agent_Co_TEL.Trim();
        if (!String.IsNullOrEmpty(Agent_Co_TEL))
        {
            if (Agent_Co_TEL.Length > 20)
            {
                return JsonConvert.SerializeObject(new { status = "【電話號碼】不能超過２０個字元。" });
            }
            else
            {
                if (JASON.IsTelephone(Agent_Co_TEL) != true)
                {
                    return JsonConvert.SerializeObject(new { status = "【電話號碼】格式不正確。" });
                }
            }
        }       //*/

        //驗證 Agent_Phone_3 【手機簡碼】
/*        Agent_Phone_3 = Agent_Phone_3.Trim();
        if (!String.IsNullOrEmpty(Agent_Phone_3))
        {
            if (Agent_Phone_3.Length > 10)
            {
                return JsonConvert.SerializeObject(new { status = "【手機簡碼】不能超過１０個字元。" });
            }
            else
            {
                if (JASON.IsNumeric(Agent_Phone_3) != true)
                {
                    return JsonConvert.SerializeObject(new { status = "【手機簡碼】只能輸入數字。" });
                }
            }
        }   //*/

        //驗證 Agent_Phone_2 【手機號碼】
        Agent_Phone_2 = Agent_Phone_2.Trim();
        if (!String.IsNullOrEmpty(Agent_Phone_2))
        {
            if (Agent_Phone_2.Length > 12)
            {
                return JsonConvert.SerializeObject(new { status = "【手機號碼】不能超過１２個字元。" });
            }
 /*           else
            {
                if (JASON.IsNumeric(Agent_Phone_2) != true)
                {
                    return JsonConvert.SerializeObject(new { status = "【手機號碼】只能輸入數字。" });
                }
            }   //*/
        }

        //驗證 Agent_Code 【院區簡碼】
 /*       Agent_Code = Agent_Code.Trim();
        if (!String.IsNullOrEmpty(Agent_Code))
        {
            if (Agent_Code.Length != 6)
            {
                return JsonConvert.SerializeObject(new { status = "【院區簡碼】需要６碼。" });
            }
            else
            {
                if (JASON.IsNumeric(Agent_Code) != true)
                {
                    return JsonConvert.SerializeObject(new { status = "【院區簡碼】只能輸入數字。" });
                }
            }
        }   //*/

        //驗證 Agent_MVPN 【ＭＶＰＮ】
/*        Agent_MVPN = Agent_MVPN.Trim();
        if (!String.IsNullOrEmpty(Agent_MVPN))
        {
            if (Agent_MVPN.Length != 8)
            {
                return JsonConvert.SerializeObject(new { status = "【ＭＶＰＮ】需要８碼。" });
            }
            else
            {
                if (JASON.IsNumeric(Agent_MVPN) != true)
                {
                    return JsonConvert.SerializeObject(new { status = "【ＭＶＰＮ】只能輸入數字。" });
                }
            }
        }   //*/

        //驗證 Agent_Mail 【電子信箱】
/*        Agent_Mail = Agent_Mail.Trim();
        if (!String.IsNullOrEmpty(Agent_Mail))
        {
            if (Agent_Mail.Length > 50)
            {
                return JsonConvert.SerializeObject(new { status = "【電子信箱】不能超過５０個字元。" });
            }
            else
            {
                if (JASON.IsEmail(Agent_Mail) != true)
                {
                    return JsonConvert.SerializeObject(new { status = "【電子信箱】格式不正確。" });
                }
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【電子信箱】" });
        }       //*/

        //驗證 UserID 【登入帳號】
        UserID = UserID.Trim();
        if (!String.IsNullOrEmpty(UserID))
        {
            if (UserID.Length > 10)
            {
                return JsonConvert.SerializeObject(new { status = "【登入帳號】不能超過１０個字元。" });
            }
            else
            {
                if (JASON.IsNumericOrLetter(UserID) != true)
                {
                    //return JsonConvert.SerializeObject(new { status = "【登入帳號】只能是英文或數字。" });
                }
            }
        }
        else
        {
            return JsonConvert.SerializeObject(new { status = "請填寫【登入帳號】" });
        }

        if (Flag == "0")
        {
            //驗證 Password 【登入密碼】
            Password = Password.Trim();
            if (!String.IsNullOrEmpty(Password))
            {
                if (Password.Length > 20)
                {
                    return JsonConvert.SerializeObject(new { status = "【登入密碼】不能超過２０個字元。" });
                }
                else
                {
                    if (JASON.IsNumericOrLetter(Password) != true)
                    {
                        //return JsonConvert.SerializeObject(new { status = "【登入密碼】只能是英文或數字。" });
                    }
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new { status = "請填寫【登入密碼】" });
            }
        }
        else
        {
            if (Password.Trim() != "")
            {
                //驗證 Password 【登入密碼】
                Password = Password.Trim();
                if (!String.IsNullOrEmpty(Password))
                {
                    if (Password.Length > 20)
                    {
                        return JsonConvert.SerializeObject(new { status = "【登入密碼】不能超過２０個字元。" });
                    }
                    else
                    {
                        if (JASON.IsNumericOrLetter(Password) != true)
                        {
                            return JsonConvert.SerializeObject(new { status = "【登入密碼】只能是英文或數字。" });
                        }
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new { status = "請填寫【登入密碼】" });
                }
                sql_pass = ", Password = @Password ";
            }
        }

 /*       if (Agent_LV != "10" && Agent_LV != "20" && Agent_LV != "30")
        {
            return JsonConvert.SerializeObject(new { status = "沒有此【人員權限】" });   //新增權限檢查
        }   //*/

 /*       Sqlstr = @"SELECT TOP 1 ROLE_ID FROM ROLELIST WHERE OPEN_DEL != 'N' AND Role_ID=@Role_ID";
        var a = DBTool.Query<ClassTemplate>(Sqlstr, new { Role_ID = Role_ID });
        if (!a.Any())
        {
            return JsonConvert.SerializeObject(new { status = "沒有此【系統選單權限】" });
        };   //*/

        Sqlstr = @"SELECT TOP 1 SYSID FROM DispatchSystem WHERE Agent_Status != '離職' AND UserID=@UserID AND Agent_ID!=@Agent_ID";
        var a = DBTool.Query<ClassTemplate>(Sqlstr, new { UserID = UserID, Agent_ID = Agent_ID });
        if (a.Any())
        {
            return JsonConvert.SerializeObject(new { status = "已有相同的【登入帳號】" });
        };

        if (Flag == "0")
        {
            Sqlstr = @"SELECT TOP 1 SYSID FROM DispatchSystem WHERE Agent_Status != '離職' AND Agent_ID=@Agent_ID";
            a = DBTool.Query<ClassTemplate>(Sqlstr, new { Agent_ID = Agent_ID });
            if (a.Any())
            {
                return JsonConvert.SerializeObject(new { status = "已有相同的【人員編號】" });
            };

            Sqlstr = @"INSERT INTO DispatchSystem (Agent_ID, Agent_Name, Agent_Status, Agent_Company, Agent_Team, " +
                "Agent_Mail, Agent_Phone_2, UserID, Password, Agent_LV, Role_ID,Agent_Co_TEL)" +
                " VALUES(@Agent_ID, @Agent_Name, @Agent_Status, @Agent_Company, @Agent_Team, " +
                "@Agent_Mail, @Agent_Phone_2, @UserID, @Password, @Agent_LV, @Role_ID, @Agent_Co_TEL)";
            back = "new";
        }
        else
        {
            Sqlstr = @"UPDATE DispatchSystem SET " +
               "Agent_Name = @Agent_Name, " +
               "Agent_Company = @Agent_Company, " +
               "Agent_Team = @Agent_Team, " +
               "Agent_Mail = @Agent_Mail, " +
               "Agent_Phone_2 = @Agent_Phone_2, " +
               //"Agent_Phone_3 = @Agent_Phone_3, " +
               "Agent_Co_TEL = @Agent_Co_TEL, " +
               //"Agent_Code = @Agent_Code, " +
               //"Agent_MVPN = @Agent_MVPN, " +
               "UserID = @UserID, " +
               "Agent_LV = @Agent_LV, " +
               "Role_ID = @Role_ID, " +
               "UpdateTime = @UpdateTime " + 
               sql_pass +
               "WHERE Agent_ID = @Agent_ID AND Agent_Status != '離職' ";
            back = "update";
        }

        var b = DBTool.Query<ClassTemplate>(Sqlstr, new
        {
            Agent_ID = Agent_ID.Trim(),
            Agent_Name = Agent_Name.Trim(),
            Agent_Status = "在職",
            Agent_Company = Agent_Company.Trim(),
            Agent_Team = Agent_Team.Trim(),
            Agent_Mail = Agent_Mail.Trim(),
            Agent_Phone_2 = Agent_Phone_2.Trim(),
            //Agent_Phone_3 = Agent_Phone_3.Trim(),
            Agent_Co_TEL = Agent_Co_TEL.Trim(),
            //Agent_Code = Agent_Code.Trim(),
            //Agent_MVPN = Agent_MVPN.Trim(),
            UserID = UserID.Trim(),
            Role_ID = Role_ID,
            Agent_LV = Agent_LV,
            Password = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "MD5").ToUpper(),
            UpdateTime = DateTime.Now
        });
        return JsonConvert.SerializeObject(new { status = back });
    }

    [WebMethod(EnableSession = true)]//或[WebMethod(true)]
    public static string Check()
    {
        string Check = JASON.Check_ID("0060010004.aspx");
        if (Check == "NO")
        {
            //System.Web.HttpContext.Current.Response.Redirect("~/Default.aspx");
        }
        return "";
    }
}


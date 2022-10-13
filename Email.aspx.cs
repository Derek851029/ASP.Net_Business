using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dapper;
using log4net;
using log4net.Config;

public partial class Email : System.Web.UI.Page
{
    static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    protected void Page_Load(object sender, EventArgs e)
    {
        EMAIL();
    }

    private void EMAIL()
    {
        string sqlstr;
        string time;
        string email_id = "";
        string email_pw = "";
        sqlstr = @"select a.Form_ID, a.Ready_Reason, b.Mail, a.Userid, a.Title
                   from Work_Table a
                   left join PartTimeSignUp b on a.Userid = b.Userid
                   where a.Email_Flag in ('0', '2') AND a.ServiceStartTime <= convert(varchar, getdate() + 7, 112)
                   and a.Type_Identity in ('PT', 'Other') and a.del_flag = '0'";
       // sqlstr = @"select  a.Form_ID, a.Ready_Reason, b.Mail, a.Userid
       //            from Work_Table a
       //            left join PartTimeSignUp b on a.Userid = b.Userid
				   //where a.SYSID = '1'";


        var list = DBTool.Query<ClassTemplate>(sqlstr);
            if (list.Count() > 0)
            {
                foreach (var q in list)
                {
                    //string sql = "UPDATE tblAssign SET Email_Flag='1' WHERE SYSID=@SYSID";
                    string sql = "UPDATE Work_Table SET Email_Flag='1', Send_Date=GetDate() WHERE Form_ID=@Form_ID";
                    string Ready_Reason = q.Ready_Reason; //刪除字串中的空白

                    time = DateTime.Now.ToString("HH");
                    logger.Info(time);
                    if (time == "00" || time == "04" || time == "08" || time == "12" || time == "16" || time == "20")
                    {
                        email_id = "jumpcases@gmail.com";
                        email_pw = "pisces7625";
                    }
                    else if (time == "01" || time == "05" || time == "09" || time == "13" || time == "17" || time == "21")
                    {
                        email_id = "jumpcases@gmail.com";
                        email_pw = "pisces7625";
                    }
                    else if (time == "02" || time == "06" || time == "10" || time == "14" || time == "18" || time == "22")
                    {
                        email_id = "jumpcases@gmail.com";
                        email_pw = "pisces7625";
                    }
                    else
                    {
                        email_id = "jumpcases@gmail.com";
                        email_pw = "pisces7625";
                    }

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
                        MySmtp.Send("jumpcases@gmail.com", q.Mail, "名格娛樂" + q.Title + "通告行前準備通知", "標題：" + q.Title + "\n" + 
                            "行前準備：" + Ready_Reason + "\n" +
                            "派工單連結：http://210.68.227.120:9002/ModelWork/Home/Dispatch_Table?Form_ID=" + q.Form_ID.Trim() + "&Userid=" + q.Userid.Trim() 
                            + "\n*本郵件為自動發送請勿直接回覆 謝謝!!");
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
}
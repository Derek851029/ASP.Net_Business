using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing; //直接輸出印表
using System.Text;  // Encoding

/// <summary>
/// WebServiceDoc 的摘要描述
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
[System.Web.Script.Services.ScriptService]
public class WebServiceDoc : System.Web.Services.WebService
{
    public WebServiceDoc()
    {
        //如果使用設計的元件，請取消註解下列一行
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public string marquees()
    {
        //string UserID = HttpContext.Current.Session["UserID"].ToString().Trim();
        //ShowMessage sm = new ShowMessage(UserID);
        return Marquee.SearchTotal();
    }
}

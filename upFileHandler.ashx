<%@ WebHandler Language="C#" Class="upFileHandler" %>

//ashx : 進行商業邏輯運作並回應結果

using Dapper;
using Newtonsoft.Json;
using System;
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
using System.IO;
using System.Threading;
using System.Runtime;

public class upFileHandler : IHttpHandler
{
    protected string userid = "";
    protected string caseid = "";
    protected string flag = "";

    public void ProcessRequest(HttpContext context)
    {
        string Owner = context.Request.Form.Get("Owner");
        string PID = context.Request.Form.Get("PID");

        string fileName = "";

        //先宣告一個變數等等放檔案名稱
        HttpFileCollection files = context.Request.Files;

        //建立資料夾

        string Path = @"D:\Derek\業務\PDF" + Owner + @"\";
        if (Directory.Exists(Path) == false)
            Directory.CreateDirectory(Path);

        try
        {
            if (context.Request.Files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    //如果有的話再把該檔案放進HttpPostedFile屬性中
                    HttpPostedFile file = files[i];

                    //FileName是C#的函數，可以取檔案名稱
                    fileName = file.FileName;
                    //用SaveAs的方法上傳圖片到指定的資料夾 
                    file.SaveAs(context.Server.MapPath("/PDF/" + Owner + @"/" + fileName));
                }
                DirectoryInfo Vendor_PDF_Path_DI = new DirectoryInfo(@"D:\Derek\業務\PDF" + Owner + @"\");

                string sqlstr = @"INSERT INTO DailyPath(PID, FilePath, FileName)VALUES(@PID, @PDF_Path, @fileName)";
                DBTool.Query(sqlstr, new { PDF_Path = Vendor_PDF_Path_DI.ToString(), PID = PID, fileName = fileName });
                ResponseJSON(context, new { status = "合約已上傳成功！" });
            }
            else
            {
                ResponseJSON(context, new { status = "目前無選取檔案" });
            }
        }
        catch (Exception ex)
        {
            ResponseJSON(context, new { status = "檔案上傳失敗" + ex.ToString() });
        }
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    public void ResponseJSON(HttpContext context, object json)
    {
        context.Response.ContentType = "application/json";
        context.Response.Charset = "utf-8";
        context.Response.Write(JsonConvert.SerializeObject(json));
    }
}
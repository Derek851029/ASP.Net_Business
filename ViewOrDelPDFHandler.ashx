<%@ WebHandler Language="C#" Class="ViewOrDelPDFHandler" %>

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

public class ViewOrDelPDFHandler : IHttpHandler
{
    protected string userid = "";
    protected string caseid = "";
    protected string flag = "";
    protected string status = "";
    public void ProcessRequest(HttpContext context)
    {
        //取得前端 get資料方
        userid = context.Request.QueryString["userid"].ToString().Trim();
        caseid = context.Request.QueryString["caseid"].ToString().Trim();
        flag = context.Request.QueryString["flag"].ToString().Trim();
        status = context.Request.QueryString["status"].ToString().Trim();

        if (status == "S")
        {
            string sqlstr = @"  
                                        SELECT FilePath,FileName FROM [Customer].[dbo].[DailyProgress] WHERE PID =  @PID;
                            ";
            var result = DBTool.Query<ClassTemplate>(sqlstr, new { caseid = caseid ,userid= userid}).ToList().Select(p => new
            {
                Staff_Contract_filename = p.Staff_Contract_filename,
                Vender_Contract_filename = p.Vender_Contract_filename
            });
            ResponseJSON(context, result);
        }
        //else
        //{
        //    if (flag == "vendor")
        //    {
        //        string Vendor_Contract_filename = "";
        //        DirectoryInfo Vendor_PDF_Path_DI = new DirectoryInfo(@"C:\Faremma_20170612\UploadPDF\" + caseid + @"\vendor\");

        //        foreach (FileInfo PDF_FILE in Vendor_PDF_Path_DI.GetFiles())
        //        {
        //            Vendor_Contract_filename = PDF_FILE + " || " + Vendor_Contract_filename;
        //        }

        //        string sqlstr = @"  
        //                                UPDATE [Faremma].[dbo].[Work_Applylist] SET 
        //                                Vender_Contract = @Vender_Contract,
        //                                Vender_Contract_filename = @Vender_Contract_filename 
        //                                WHERE CaseID =  @caseid;

        //                                SELECT * FROM [Faremma].[dbo].[Work_Applylist] WHERE CaseID =  @caseid;
        //                    ";
        //        DBTool.Query<ClassTemplate>(sqlstr, new { Vender_Contract = Vendor_PDF_Path_DI.ToString(), Vender_Contract_filename = Vendor_Contract_filename, caseid = caseid }).ToList().Select(p => new
        //        {

        //        });
        //        ResponseJSON(context, new { status = "廠商合約已上傳" });
        //    }
        //    else
        //    {
        //    }
        //}

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
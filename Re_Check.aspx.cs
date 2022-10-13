using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Report_Re_Check : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Check();
            string seqno = "";
            string sql_txt = "";
            seqno = Request.Params["seqno"];
            if (string.IsNullOrEmpty(seqno))
            {
                Response.Redirect("0030010000/0030010003.aspx");
            }
            else
            {
                if (HttpUtility.HtmlEncode(seqno).ToString().Length > 15)
                {
                    Response.Redirect("0030010000/0030010003.aspx");
                }

                sql_txt = @"SELECT TOP 1 Case_ID, Service_Flag FROM CaseData WHERE Case_ID=@Case_ID";
                var chk = DBTool.Query<Report>(sql_txt, new { Case_ID = seqno });
                if (!chk.Any())
                {
                    Response.Write("<script>alert('查無【" + seqno + "】派工單編號'); " +
                        "location.href='/0030010000/0030010003.aspx'; </script>");
                }
                else
                {
                    Response.Redirect("~/Report/Report_001.aspx?seqno=" + seqno);

                    Report val = chk.First();
                    if (val.Service_Flag != "0")
                    {
                        Response.Redirect("~/Report/Report_001_Picture.aspx?seqno=" + seqno);
                    }
                    else
                    {
                        Response.Redirect("~/Report/Report_001.aspx?seqno=" + seqno);
                    }   //*/
                }
            }
        }
    }

    protected void Check()
    {
        string Check = JASON.Check_ID("0030010000/0030010003.aspx");  //個人派工瀏覽
        if (Check == "NO")
        {
            System.Web.HttpContext.Current.Response.Redirect("~/Default.aspx");
        }
    }

    public class Report
    {
        public string SYSID { get; set; }
        public string Service_Image { get; set; }
        public string Service_Flag { get; set; }
    }
}
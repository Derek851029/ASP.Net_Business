using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using log4net;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dapper;
using Newtonsoft.Json;

using System.Text.RegularExpressions;   //egex.IsMatch
using System.Globalization;             //TaiwanCalendar
using System.Web.UI.HtmlControls;


/*      Function List 
    Append2File
    DumpLog
    CheckFidleNull      
    PatientInfoDDLSelected      ddl選擇後帶入案家基本資料
    PatientInfoCreateDDL        綁定ddl顯示的案家
    AreaBlock                   隱藏或顯示頁面區塊
    IsDate                      判斷字串是否日期格式
    T FindControl<T>            尋找頁面上的控件
    UpdatePanelControlSum       UpdatePanel內多項選擇結果的分數填入及加總
    DDLControlSum               DropDownList多項選擇結果的分數填入及加總
    PanelControlSum             Panel內多項選擇結果的分數填入及加總
    GetControlValue             取得控件的value存入資料庫或瑱入控件值
    ResetControlValue
    Redirect
    ShowMessage
    CalculateAge                算歲數
    ToFullTaiwanDate
    ToSimpleTaiwanDate
    ToSimpleTaiwanYear
    IsDateTime
    IsValidDate
    isNumber
    GetUserIP
    UserActionLog
    CheckIDCard
    ReplaySession
    GetUserSessionString
    checkSession
    ExecuteReader
    ExecuteNonQuery
    PrepareCommand
    GetDataTable
    WriteErrorLog
    WinSockSend
    Md5Hash
 */

public static class WebClassFunc
{
    /// <summary>
    /// Execute a select query that will return a result set
    /// </summary>
    /// <param name="connString">Connection string</param>
    //// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    /// <param name="commandText">the stored procedure name or PL/SQL command</param>
    /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
    /// <returns></returns>
    /// 
    public static void Append2File(string logMessage, string strProcessName)
    {
        string filder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        if (!System.IO.File.Exists(filder + @"\TargetId超出長度.txt"))
        {
        }
        using (StreamWriter w = File.AppendText(filder + @"\" + strProcessName + ".txt"))
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine(DateTime.Now.ToLongTimeString());
            w.WriteLine("  :");
            w.WriteLine("  :" + logMessage);
            w.WriteLine("-------------------------------");
        }
        //using (StreamReader r = File.OpenText("log.txt"))
        //{
        //    DumpLog(r);
        //}
    }

    public static void DumpLog(StreamReader r)
    {
        string line;
        while ((line = r.ReadLine()) != null)
        {
            Console.WriteLine(line);
        }
    }

    public static string CheckFidleNull(string strField, string strType)
    {
        if (!string.IsNullOrEmpty(strField))
        {
            strField = strField.Trim();
            switch (strType)
            {
                case "date":
                    if (IsDate(strField))
                    {
                        strField = Convert.ToDateTime(strField).ToString("yyyy-MM-dd");
                    }
                    break;
                case "datetime":
                    if (IsDate(strField))
                    {
                        strField = Convert.ToDateTime(strField).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    break;
                case "time":
                    if (IsDate(strField))
                    {
                        strField = Convert.ToDateTime(strField).ToString("HH:mm");
                    }
                    break;
                case "number":
                    if (!isNumber(strField))
                    {
                        strField = "0";
                    }
                    break;
            }
        }
        else
        {
            switch (strType)
            {
                case "string":
                    strField = "";
                    break;
                case "int":
                    strField = "0";
                    break;
                case "date":
                    strField = "1900-01-01";
                    break;
                case "datetime":
                    strField = "1900/01/01 00:00:00";
                    break;
                case "time":
                    strField = "00:00";
                    break;
                case "number":
                    strField = "0";
                    break;
            }
        }
        return strField;
    }
    public static void PatientInfoDDLSelected(string strControl, string strServiceType)
    {
        Page page = (Page)HttpContext.Current.Handler;
        DropDownList ctlTest = FindControl<DropDownList>(page, strControl);
        TextBox ctltxtTest = FindControl<TextBox>(page, "txt_PatientInfo");
        Control ctlElement = (Control)page.Master.FindControl("head2").FindControl(strControl);
        TextBox idCard_data = FindControl<TextBox>(page, "idCard_data");
        TextBox name_data = FindControl<TextBox>(page, "name_data");
        TextBox select06_data = FindControl<TextBox>(page, "select06_data");
        TextBox address01_data = FindControl<TextBox>(page, "address01_data");
        TextBox select02_data = FindControl<TextBox>(page, "select02_data");
        TextBox mainname_data = FindControl<TextBox>(page, "mainname_data");
        TextBox relationship02_data = FindControl<TextBox>(page, "relationship02_data");
        TextBox homephone02_data = FindControl<TextBox>(page, "homephone02_data");
        HiddenField hid_AddCaseID = FindControl<HiddenField>(page, "hid_AddCaseID");
        HiddenField hid_Birthday = FindControl<HiddenField>(page, "hid_Birthday");
        HiddenField hid_Sex = FindControl<HiddenField>(page, "hid_Sex");
        //Type_Value=0 Type=照管申請作業
        //Type_Value=1 Type=照管電家訪
        //Type_Value=2 Type=照管轉介
        //Type_Value=25 Type=照專被退案
        //Type_Value=3 Type=已接案 尚未派案
        //Type_Value=4 Type=被退案
        //Type_Value=5 Type=居服督導審核階段
        //Type_Value=6 Type=居服督導評估階段
        //Type_Value=7 Type=居服督導計畫階段
        //Type_Value=8 Type=居服督導簽約階段
        //Type_Value=9 Type=居服督導派工階段
        //Type_Value=10 Type=居服員尚未服務
        //Type_Value=11 Type=居服員服務階段
        //Type_Value=12 Type=居服督導電訪階段
        //Type_Value=99 Type=結案
        //Service_Status=追蹤中、暫停服務、結案

        try
        {
            if (ctlTest.SelectedIndex > 0)
            {
                string strSql = "select top 1 * " +
                                " from Application " +
                                " where case_id = @case_id " +
                                //" and @case_id not in " +
                                //" (select case_id from [0210010015]) " +
                                "";
                (page.ToString() + ", ddl_PatientInfo_SelectedIndexChanged, strSql=" + strSql +
                    ", @case_id = '" + ctlTest.SelectedValue + "'" +
                    "").WriteLog();
                var strConn = new CMS_db();
                using (SqlConnection sqlCon = new SqlConnection(strConn.ConnStr()))
                {
                    using (SqlDataAdapter myAdp = new SqlDataAdapter(strSql, sqlCon))
                    {
                        myAdp.SelectCommand.Parameters.Clear();
                        myAdp.SelectCommand.Parameters.Add("@case_id", SqlDbType.NVarChar).Value = ctlTest.SelectedValue;
                        using (DataSet ds = new DataSet())
                        {
                            myAdp.Fill(ds, "test");
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow itm in ds.Tables[0].Rows)
                                {
                                    idCard_data.Text = itm["idcard_data"].ToString();
                                    idCard_data.ReadOnly = true;
                                    name_data.Text = itm["name_data"].ToString();
                                    name_data.ReadOnly = true;
                                    select06_data.Text = itm["select06_data"].ToString();
                                    select06_data.ReadOnly = true;
                                    address01_data.Text = itm["Township02_1"].ToString().Trim()
                                        + " " + itm["Township02_2"].ToString().Trim()
                                        + " " + itm["Township02_3"].ToString().Trim()
                                        + " " + itm["address01_data"].ToString();
                                    address01_data.ReadOnly = true;
                                    if (itm["birth_data"] != System.DBNull.Value)
                                    {
                                        select02_data.Text = itm["select02_data"].ToString() +
                                            "/" + Convert.ToDateTime(itm["birth_data"]).ToString("yyyy-MM-dd");
                                    }
                                    else
                                    {
                                        select02_data.Text = itm["select02_data"].ToString() + "/";
                                    }
                                    select02_data.ReadOnly = true;
                                    mainname_data.Text = itm["mainname_data"].ToString();
                                    mainname_data.ReadOnly = true;
                                    relationship02_data.Text = itm["relationship02_data"].ToString();
                                    relationship02_data.ReadOnly = true;
                                    if (itm["homephone02_data"].ToString() == "")
                                    {
                                        homephone02_data.Text = itm["celephone02_data"].ToString();
                                    }
                                    else
                                    {
                                        homephone02_data.Text = itm["homephone02_data"].ToString();
                                    }
                                    homephone02_data.ReadOnly = true;
                                    hid_AddCaseID.Value = itm["case_id"].ToString();

                                    if (hid_Birthday != null && hid_Birthday.Value.Trim()!="")
                                    {
                                        hid_Birthday.Value = Convert.ToDateTime(itm["birth_data"]).ToString("yyyy-MM-dd");
                                    }
                                    if (hid_Sex != null)
                                    {
                                        hid_Sex.Value = itm["select02_data"].ToString().Trim();
                                    }
                                }
                            }
                            //else
                            //{
                            //    //Response.Write("<Script language='JavaScript'>alert('已經有照顧計畫！');</Script>");
                            //    iTem001.Text = "已有照顧計畫,無法新增";
                            //    iTem001.ForeColor = System.Drawing.Color.Red;
                            //}
                        }
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            (page.ToString() + ", ddl_PatientInfo_SelectedIndexChanged, sqlEx=" + sqlEx).WriteLog();
        }
    }
    public static void PatientInfoCreateDDL(string strControl, string strServiceType)
    {
        Page page = (Page)HttpContext.Current.Handler;
        DropDownList ctlTest = FindControl<DropDownList>(page, strControl);
        TextBox ctltxtTest = FindControl<TextBox>(page, "txt_PatientInfo");

        Control ctlElement = (Control)page.Master.FindControl("head2").FindControl(strControl);

        try
        {
            string strSql = " select case_id,idcard_data + '__' + name_data " +
                            " + '__' + case_id " +
                            " as patientInfo from Application " +
                            " where case_id <> '' " +
                            "";
            string strSqlWhere = " and idcard_data + '!!!' + case_id + '!!!' + " +
                                " name_data like '%' + @PatientInfo + '%' " +
                                "";
            if (strServiceType != "")
            {
                strSql += " and select13_data=@select13_data ";
            }
            if (ctltxtTest.Text.Trim() != "")
            {
                strSql += strSqlWhere;
            }
            (page.ToString() + ", btn_PatientInfo_Click strSql=" + strSql +
                ", @PatientInfo=" + ctltxtTest.Text.Trim() +
                ", @select13_data=" + strServiceType.Trim() +
                "").WriteLog();
            CMS_db strConn = new CMS_db();
            using (SqlConnection sqlcon = new SqlConnection(strConn.ConnStr()))
            {
                using (SqlDataAdapter myAdp = new SqlDataAdapter(strSql, sqlcon))
                {
                    myAdp.SelectCommand.Parameters.Clear();
                    myAdp.SelectCommand.Parameters.Add("@PatientInfo", SqlDbType.NVarChar).Value = ctltxtTest.Text.Trim();
                    myAdp.SelectCommand.Parameters.Add("@select13_data", SqlDbType.NVarChar).Value = strServiceType.Trim();
                    using (DataSet ds = new DataSet())
                    {
                        myAdp.Fill(ds, "test");
                        ctlTest.DataSource = ds;
                        ctlTest.DataValueField = "case_id";
                        ctlTest.DataTextField = "patientinfo";
                        ctlTest.DataBind();
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            ctlTest.Items.Insert(0, new ListItem("請選擇", ""));
                        }
                        else
                        {
                            ctlTest.Items.Insert(0, new ListItem("尚無申請服務的案家,無法帶入", ""));
                        }
                    }
                }
            }
        }
        catch (SqlException sqlEx)
        {
            (page.ToString() + ", btn_PatientInfo_Click, sqlEx=" + sqlEx).WriteLog();
        }
        //else
        //{
        //    //Response.Write("<Script language='JavaScript'>alert('沒有輸入任何搜尋文字！');</Script>");
        //    try
        //    {
        //        string strSql = "select case_id,idcard_data + '__' + name_data " +
        //                        " + '__' + case_id " +
        //                        " as patientInfo from Application " +
        //                        " where case_id <> ''  " +
        //                        "";
        //        ("0200010000, btn_PatientInfo_Click strSql=" + strSql).WriteLog();
        //        CMS_db strConn = new CMS_db();
        //        using (SqlConnection sqlcon = new SqlConnection(strConn.ConnStr()))
        //        {
        //            using (SqlDataAdapter myAdp = new SqlDataAdapter(strSql, sqlcon))
        //            {
        //                myAdp.SelectCommand.Parameters.Clear();
        //                myAdp.SelectCommand.Parameters.Add("@PatientInfo", SqlDbType.NVarChar).Value = txt_PatientInfo.Text.Trim();
        //                using (DataSet ds = new DataSet())
        //                {
        //                    myAdp.Fill(ds, "test");
        //                    ddl_PatientInfo.DataSource = ds;
        //                    ddl_PatientInfo.DataValueField = "case_id";
        //                    ddl_PatientInfo.DataTextField = "patientinfo";
        //                    ddl_PatientInfo.DataBind();
        //                    if (ds.Tables[0].Rows.Count > 0)
        //                    {
        //                        ddl_PatientInfo.Items.Insert(0, new ListItem("請選擇", ""));
        //                    }
        //                    else
        //                    {
        //                        ddl_PatientInfo.Items.Insert(0, new ListItem("沒有符合條件的已收案病患", ""));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException sqlEx)
        //    {
        //        ("0200010000, btn_PatientInfo_Click, sqlEx=" + sqlEx).WriteLog();
        //    }
        //}
    }
    public static void AreaBlock(string strArea, string strBlock)
    {
        Page page = (Page)HttpContext.Current.Handler;
        page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(),
        "<script type='text/javascript'>document.getElementById('" + strArea + "').style.display = '" + strBlock + "'</script>");
    }
    public static void AreaBlockHtml(string strArea, string strBlock)
    {
        Page page = (Page)HttpContext.Current.Handler;
        page.ClientScript.RegisterStartupScript(page.GetType(), Guid.NewGuid().ToString(),
        "<script type='text/javascript'>document.getElementById('" + strArea + "').style.display = '" + strBlock + "'</script>");
    }
    public static bool IsDate(string strDate)
    {
        DateTime dtDate;
        if (DateTime.TryParse(strDate, out dtDate))
        {
            return true;
        }
        else
        {
            //throw new Exception("不是正确的日期格式类型！");
            return false;
        }
    }

    public static T FindControl<T>(Control startingControl, string id) where T : Control
    {
        // 取得 T 的預設值，通常是 null
        //Page page = (Page)HttpContext.Current.Handler;
        T found = default(T);

        int controlCount = startingControl.Controls.Count;

        if (controlCount > 0)
        {
            for (int i = 0; i < controlCount; i++)
            {
                Control activeControl = startingControl.Controls[i];
                if (activeControl is T)
                {
                    found = startingControl.Controls[i] as T;
                    if (string.Compare(id, found.ID, true) == 0) break;
                    else found = null;
                }
                else
                {
                    found = FindControl<T>(activeControl, id);
                    if (found != null) break;
                }
            }
        }
        return found;
    }
    public static T HtmlFindControl<T>(Control startingControl, string id) where T : HtmlControl
    {
        // 取得 T 的預設值，通常是 null
        //Page page = (Page)HttpContext.Current.Handler;
        T found = default(T);

        int controlCount = startingControl.Controls.Count;

        if (controlCount > 0)
        {
            for (int i = 0; i < controlCount; i++)
            {
                HtmlControl activeControl = (HtmlControl)startingControl.Controls[i];
                if (activeControl is T)
                {
                    found = startingControl.Controls[i] as T;
                    if (string.Compare(id, found.ID, true) == 0) break;
                    else found = null;
                }
                else
                {
                    found = HtmlFindControl<T>(activeControl, id);
                    if (found != null) break;
                }
            }
        }
        return found;
    }

    public static void UpdatePanelControlSum(string strupdControl, string strpnlControl, string striTemTTCount)
    {
        Page page = (Page)HttpContext.Current.Handler;
        Control ctlElement = (Control)page.Master.FindControl("head2").FindControl(strupdControl);
        int intScore = 0;
        if (ctlElement is UpdatePanel)
        {
            UpdatePanel upnlTest = (UpdatePanel)ctlElement;
            if (upnlTest.HasControls())
            {
                foreach (Control ctlSubTest in upnlTest.Controls[0].Controls)
                {
                    if(ctlSubTest is Panel)
                    {
                        Panel pnlTest = (Panel)ctlSubTest;
                        if (pnlTest.ID == strpnlControl)
                        {
                            foreach(Control ctlinPnlTest in pnlTest.Controls)
                            {
                                if (ctlinPnlTest is RadioButtonList)
                                {
                                    RadioButtonList rdblTest = (RadioButtonList)ctlinPnlTest; 
                                    intScore += Convert.ToInt16(rdblTest.SelectedValue);
                                }
                                else if (ctlinPnlTest is TextBox)
                                {
                                    if (ctlinPnlTest.ID == strpnlControl)
                                    {
                                        TextBox ctlTextTest = (TextBox)ctlinPnlTest;
                                        ctlTextTest.Text = intScore.ToString();
                                    }
                                }
                            }
                            break;
                        }
                    }
                    /*
                    if (ctlSubTest is RadioButton)
                    {
                        RadioButton rdbTest = (RadioButton)ctlSubTest;
                    }
                    else if (ctlSubTest is RadioButtonList)
                    {
                        RadioButtonList rdblTest = (RadioButtonList)ctlSubTest;
                    }
                    else if (ctlSubTest is CheckBoxList)
                    {
                        CheckBoxList ckblTest = (CheckBoxList)ctlSubTest;
                    }
                    else if (ctlSubTest is CheckBox)
                    {
                        CheckBox ckbTest = (CheckBox)ctlSubTest;
                    }
                    else if (ctlSubTest is TextBox)
                    {
                        TextBox txtTest = (TextBox)ctlSubTest;
                        if (txtTest.ID == striTemTTCount)
                        {
                            txtTest.Text = intScore.ToString();
                        }
                    }
                    else if (ctlSubTest is Label)
                    {
                        Label lblTest = (Label)ctlSubTest;
                    }
                    else if (ctlSubTest is HiddenField)
                    {
                        HiddenField hidTest = (HiddenField)ctlSubTest;
                    }
                    else if (ctlSubTest is DropDownList)
                    {
                        DropDownList ddlTest = (DropDownList)ctlSubTest;
                    }
                    */
                }
            }
        }
        //if (ctlElement is Panel)
        //{
        //    Panel pnlTest = (Panel)ctlElement;
        //    foreach (Control ctlTest in pnlTest.Controls)
        //    {
        //    }
        //}
    }
    public static void DDLControlSum(string strSenderID, string strrdbType, string[,] arriTem, string striTemTTCount, string[,] arriTemDisable, string striTemDisableTTCount, string strDataMode)
    {
        try
        {
            //strSenderID:          要查找的Control ID
            //strrdbType:           初評, 結評
            //arriTem:              同評組要Enable的TextBox ID陣列
            //striTemTTCount:       同評組的TextBox最後放總計的TextBox ID
            //arriTemDisable:       同評組要Disable的TextBox ID陣列
            //striTemDisableTTCount 同評組要Disable的TextBox最後放總計的TextBox ID
            //strDataMode:          來源是新增:add 還是變更:update  
            Page page = (Page)HttpContext.Current.Handler;
            int intCount = 0;
            DropDownList rdbSender = (DropDownList)page.Master.FindControl("head2").FindControl(strSenderID);

            Control ctlSenderID = (Control)page.Master.FindControl("head2");
            //本應用:RadioButtonList 選到的value給TextBox
            //先取得value
            int i = arriTem.Length / 2; //二維{RadioButtonListID,TextBoxID}
            int j = 0;
            string[] arrtxtID = new string[i];
            string[] arrrdblID = new string[i];
            string[] arrDisabletxtID = new string[i];
            string[] arrDisablerdblID = new string[i];
            int intCurrentArrIndex = 0;
            string strCurrentArrTxtBoxID = "";
            //排序二維陣列對應組
            foreach (string strTest in arriTem)
            {
                if (j < i)
                {
                    arrrdblID[j] = strTest;
                    if (strTest == strSenderID)
                    {
                        //指定的strSenderID在陣列的index
                        intCurrentArrIndex = j;
                    }
                }
                else
                {
                    arrtxtID[j - i] = strTest;
                    if ((j - i) == intCurrentArrIndex)
                    {
                        //取得strSenderID相對的TextBoxID
                        strCurrentArrTxtBoxID = strTest;
                    }
                }
                j++;
            }
            j = 0;
            //傳值給文字欄
            foreach (string strrdblTest in arrrdblID)
            {
                DropDownList rdblTest = (DropDownList)FindControl<DropDownList>(page, strrdblTest);
                rdblTest.Enabled = true;
                TextBox txtTest = (TextBox)FindControl<TextBox>(page, arrtxtID[j]);
                if (strDataMode != "SingleNoText")
                {
                    txtTest.Enabled = true;
                    if (rdblTest.SelectedValue != "")
                    {
                        //if (rdblTest.ID == strSenderID)
                        //{

                        //}
                        //else
                        //{

                        //}
                        txtTest.Text = rdblTest.SelectedValue;
                        ("DDLControlSum, txtTest.Text=" + txtTest.Text +
                         ", rdblTest.ID=" + rdblTest.ID +
                         "").WriteLog();
                    }
                    if (txtTest.Text.Trim() != "" && txtTest.Text.Trim() != "NA")
                    {
                        intCount += Convert.ToInt16(txtTest.Text.Trim());
                    }
                }
                else
                {
                    if (rdblTest.SelectedValue.Trim() != "NA")
                    {
                        intCount += Convert.ToInt16(rdblTest.SelectedValue);
                    }
                }
                j++;
            }
            TextBox txtTTTest = (TextBox)FindControl<TextBox>(page, striTemTTCount);
            txtTTTest.Text = intCount.ToString();

            if (strDataMode != "single" && strDataMode != "SingleNoText")
            {
                //第二組要處理
                j = 0;
                foreach (string strTest in arriTemDisable)
                {
                    if (j < i)
                    {
                        arrDisablerdblID[j] = strTest;
                        if (strTest == strSenderID)
                        {
                            //指定的strSenderID在陣列的index
                            intCurrentArrIndex = j;
                        }
                    }
                    else
                    {
                        arrDisabletxtID[j - i] = strTest;
                        if ((j - i) == intCurrentArrIndex)
                        {
                            //取得strSenderID相對的TextBoxID
                            strCurrentArrTxtBoxID = strTest;
                        }
                    }
                    j++;
                }
                j = 0;

                //清除初評或結評組
                if (strDataMode == "add")
                {
                    foreach (string strrdblTest in arrDisablerdblID)
                    {
                        DropDownList rdblTest = (DropDownList)FindControl<DropDownList>(page, strrdblTest);
                        //rdblTest.SelectedIndex = -1;
                        TextBox txtTest = (TextBox)FindControl<TextBox>(page, arrDisabletxtID[j]);
                        txtTest.Enabled = false;
                        txtTest.Text = "";
                        j++;
                    }
                    TextBox txtTTDisableTest = (TextBox)FindControl<TextBox>(page, striTemDisableTTCount);
                    txtTTDisableTest.Text = "";
                }
                else if (strDataMode == "Update")
                {
                    foreach (string strrdblTest in arrrdblID)
                    {
                        DropDownList rdblTest = (DropDownList)FindControl<DropDownList>(page, strrdblTest);
                        rdblTest.SelectedIndex = -1;
                        TextBox txtTest = (TextBox)FindControl<TextBox>(page, arrtxtID[j]);
                        txtTest.Enabled = true;
                        j++;
                    }
                    TextBox txtTotalTest = (TextBox)FindControl<TextBox>(page, striTemDisableTTCount);
                    txtTotalTest.Enabled = true;
                    j = 0;
                    foreach (string strrdblTest in arrDisablerdblID)
                    {
                        TextBox txtTest = (TextBox)FindControl<TextBox>(page, arrDisabletxtID[j]);
                        txtTest.Enabled = true;
                        j++;
                    }
                    TextBox txtTTDisableTest = (TextBox)FindControl<TextBox>(page, striTemDisableTTCount);
                    txtTTDisableTest.Enabled = true;
                }
            }
        }
        catch (Exception ex)
        {
            ("PanelControlSum, ex=" + ex).WriteLog();
        }
    }

    public static void PanelControlSum(string strSenderID, string strrdbType, string[,] arriTem, string striTemTTCount, string[,] arriTemDisable, string striTemDisableTTCount, string strDataMode)
    {
        try
        {
            //strSenderID:          要查找的Control ID
            //strrdbType:           初評, 結評
            //arriTem:              同評組要Enable的TextBox ID陣列
            //striTemTTCount:       同評組的TextBox最後放總計的TextBox ID
            //arriTemDisable:       同評組要Disable的TextBox ID陣列
            //striTemDisableTTCount 同評組要Disable的TextBox最後放總計的TextBox ID
            //strDataMode:          來源是新增:add 還是變更:update  
            Page page = (Page)HttpContext.Current.Handler;
            int intCount = 0;
            RadioButtonList rdbSender = (RadioButtonList)page.Master.FindControl("head2").FindControl(strSenderID);

            Control ctlSenderID = (Control)page.Master.FindControl("head2");
            //本應用:RadioButtonList 選到的value給TextBox
            //先取得value
            int i = arriTem.Length / 2; //二維{RadioButtonListID,TextBoxID}
            int j = 0;
            string[] arrtxtID = new string[i];
            string[] arrrdblID = new string[i];
            string[] arrDisabletxtID = new string[i];
            string[] arrDisablerdblID = new string[i];
            int intCurrentArrIndex = 0;
            string strCurrentArrTxtBoxID = "";
            //排序二維陣列對應組
            foreach (string strTest in arriTem)
            {
                if (j < i)
                {
                    arrrdblID[j] = strTest;
                    if (strTest == strSenderID)
                    {
                        //指定的strSenderID在陣列的index
                        intCurrentArrIndex = j;
                    }
                }
                else
                {
                    arrtxtID[j - i] = strTest;
                    if ((j - i) == intCurrentArrIndex)
                    {
                        //取得strSenderID相對的TextBoxID
                        strCurrentArrTxtBoxID = strTest;
                    }
                }
                j++;
            }
            j = 0;
            //傳值給文字欄
            foreach (string strrdblTest in arrrdblID)
            {
                RadioButtonList rdblTest = (RadioButtonList)FindControl<RadioButtonList>(page, strrdblTest);
                rdblTest.Enabled = true;
                TextBox txtTest = (TextBox)FindControl<TextBox>(page, arrtxtID[j]);
                if (strDataMode != "SingleNoText")
                {
                    txtTest.Enabled = true;
                    if (rdblTest.SelectedValue != "" && rdblTest.ID == strSenderID)
                    {
                        txtTest.Text = rdblTest.SelectedValue;
                        ("PanelControlSum, txtTest.Text=" + txtTest.Text +
                         ", rdblTest.ID=" + rdblTest.ID +
                         "").WriteLog();
                    }
                    if (txtTest.Text.Trim() != "" && txtTest.Text.Trim() != "NA")
                    {
                        intCount += Convert.ToInt16(txtTest.Text.Trim());
                    }
                }
                else
                {
                    if (rdblTest.SelectedValue.Trim() != "NA")
                    {
                        intCount += Convert.ToInt16(rdblTest.SelectedValue);
                    }
                }
                j++;
            }
            TextBox txtTTTest = (TextBox)FindControl<TextBox>(page, striTemTTCount);
            txtTTTest.Text = intCount.ToString();

            if (strDataMode != "single" && strDataMode != "SingleNoText")
            {
                //第二組要處理
                j = 0;
                foreach (string strTest in arriTemDisable)
                {
                    if (j < i)
                    {
                        arrDisablerdblID[j] = strTest;
                        if (strTest == strSenderID)
                        {
                            //指定的strSenderID在陣列的index
                            intCurrentArrIndex = j;
                        }
                    }
                    else
                    {
                        arrDisabletxtID[j - i] = strTest;
                        if ((j - i) == intCurrentArrIndex)
                        {
                            //取得strSenderID相對的TextBoxID
                            strCurrentArrTxtBoxID = strTest;
                        }
                    }
                    j++;
                }
                j = 0;

                //清除初評或結評組
                if (strDataMode == "add")
                {
                    foreach (string strrdblTest in arrDisablerdblID)
                    {
                        RadioButtonList rdblTest = (RadioButtonList)FindControl<RadioButtonList>(page, strrdblTest);
                        //rdblTest.SelectedIndex = -1;
                        TextBox txtTest = (TextBox)FindControl<TextBox>(page, arrDisabletxtID[j]);
                        txtTest.Enabled = false;
                        txtTest.Text = "";
                        j++;
                    }
                    TextBox txtTTDisableTest = (TextBox)FindControl<TextBox>(page, striTemDisableTTCount);
                    txtTTDisableTest.Text = "";
                }
                else if (strDataMode == "Update")
                {
                    foreach (string strrdblTest in arrrdblID)
                    {
                        RadioButtonList rdblTest = (RadioButtonList)FindControl<RadioButtonList>(page, strrdblTest);
                        rdblTest.SelectedIndex = -1;
                        TextBox txtTest = (TextBox)FindControl<TextBox>(page, arrtxtID[j]);
                        txtTest.Enabled = true;
                        j++;
                    }
                    TextBox txtTotalTest = (TextBox)FindControl<TextBox>(page, striTemDisableTTCount);
                    txtTotalTest.Enabled = true;
                    j = 0;
                    foreach (string strrdblTest in arrDisablerdblID)
                    {
                        TextBox txtTest = (TextBox)FindControl<TextBox>(page, arrDisabletxtID[j]);
                        txtTest.Enabled = true;
                        j++;
                    }
                    TextBox txtTTDisableTest = (TextBox)FindControl<TextBox>(page, striTemDisableTTCount);
                    txtTTDisableTest.Enabled = true;
                }
            }
        }
        catch(Exception ex)
        {
            ("PanelControlSum, ex=" + ex).WriteLog();
        }
    }

    public static string GetControlValue(string strControl, string strControlType, string strCheckValue, string strDirect)
    {
        try
        {
            Page page = (Page)HttpContext.Current.Handler;

            Control ctlElement = (Control)page.Master.FindControl("head2").FindControl(strControl);
            if (ctlElement is Panel)
            {
                Panel pnlTest = (Panel)page.Master.FindControl("head2").FindControl(strControl);
                //考慮有包在UpdatePanel內
                foreach (Control ctlSubTest in pnlTest.Controls)
                {
                    if (ctlSubTest is UpdatePanel)
                    {
                        UpdatePanel upnlTest = (UpdatePanel)ctlSubTest;
                        switch (strControlType)
                        {
                            case "RadioButton":
                                switch (strDirect)
                                {
                                    case "C2D":
                                        foreach (Control ctlTest in upnlTest.Controls)
                                        {
                                            if (ctlTest is RadioButton)
                                            {
                                                RadioButton rdbTest = (RadioButton)ctlTest;
                                                return rdbTest.Text.Trim();
                                            }
                                        }
                                        break;
                                    case "D2C":
                                        foreach (Control ctlTest in upnlTest.Controls)
                                        {
                                            if (ctlTest is RadioButton)
                                            {
                                                RadioButton rdbTest = (RadioButton)ctlTest;
                                                if (rdbTest.Text == strCheckValue)
                                                {
                                                    rdbTest.Checked = true;
                                                }
                                            }
                                        }
                                        break;
                                }
                                break;
                            case "RadioButtonList":
                                break;
                            case "CheckBoxList":
                                break;
                            case "CheckBox":
                                break;
                            case "TextBox":
                                break;
                        }
                    }
                }
                //如果沒有UpdatePanel繼續
                switch (strControlType)
                {
                    case "RadioButton":
                        switch (strDirect)
                        {
                            case "C2D":
                                foreach (Control ctlTest in pnlTest.Controls)
                                {
                                    if (ctlTest is RadioButton)
                                    {
                                        RadioButton rdbTest = (RadioButton)ctlTest;
                                        if (rdbTest.Checked)
                                        {
                                            return rdbTest.Text.Trim();
                                        }
                                    }
                                }
                                break;
                            case "D2C":
                                foreach (Control ctlTest in pnlTest.Controls)
                                {
                                    if (ctlTest is RadioButton)
                                    {
                                        RadioButton rdbTest = (RadioButton)ctlTest;
                                        if (rdbTest.Text == strCheckValue)
                                        {
                                            rdbTest.Checked = true;
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                    case "RadioButtonList":
                        break;
                    case "CheckBoxList":
                        break;
                    case "CheckBox":
                        break;
                    case "TextBox":
                        break;
                }
                return "";
            }
            else if (ctlElement is UpdatePanel)
            {

            }
            else if (ctlElement is RadioButton)
            {
                bool blLoop = true;
                int i = 1;
                switch (strDirect)
                {
                    case "C2D":
                        if (strControlType == "RadioButton")
                        {
                            while (blLoop)
                            {
                                RadioButton rdbTest = FindControl<RadioButton>(page, strControl + "X" + i);
                                if (rdbTest.ID != null)
                                {
                                    if (rdbTest.Checked)
                                    {
                                        blLoop = false;
                                        return rdbTest.Text.Trim();
                                    }
                                }
                                else
                                {
                                    blLoop = false;
                                }
                                i++;
                            }
                        }
                        break;
                    case "D2C":
                        if (strControlType == "RadioButton")
                        {
                            while (blLoop)
                            {
                                RadioButton rdbTest = FindControl<RadioButton>(page, strControl + "X" + i);
                                if (rdbTest.ID != null)
                                {
                                    if (rdbTest.Text == strCheckValue)
                                    {
                                        blLoop = false;
                                        rdbTest.Checked = true;
                                        return "";
                                    }
                                }
                                else
                                {
                                    blLoop = false;
                                }
                                i++;
                            }
                        }
                        break;
                }
            }
            else if (ctlElement is RadioButtonList)
            {
                RadioButtonList rdblTest = (RadioButtonList)ctlElement;
                switch (strDirect)
                {
                    case "D2C":
                        if (strCheckValue != "")
                        {
                            rdblTest.SelectedValue = strCheckValue;
                        }
                        break;
                }
            }
            else if (ctlElement is DropDownList)
            {
                DropDownList ckblTest = (DropDownList)ctlElement;
                string strReturnResult = "";
                switch (strDirect)
                {
                    case "D2C":
                        foreach (ListItem litm in ckblTest.Items)
                        {
                            if (strCheckValue == litm.Value.Trim())
                            {
                                litm.Selected = true;
                            }
                        }
                        return "true";
                    case "C2D":
                        foreach (ListItem litm in ckblTest.Items)
                        {
                            if (litm.Selected)
                            {
                                strReturnResult = litm.Value.Trim();
                            }
                        }
                        return strReturnResult;
                }
            }
            else if (ctlElement is CheckBoxList)
            {
                CheckBoxList ckblTest = (CheckBoxList)ctlElement;
                string strReturnResult = "";
                switch (strDirect)
                {
                    case "D2C":
                        string[] strTest = strCheckValue.Split(';');
                        foreach (string sitm in strTest)
                        {
                            foreach (ListItem litm in ckblTest.Items)
                            {
                                if (sitm == litm.Value.Trim())
                                {
                                    litm.Selected = true;
                                }
                            }
                        }
                        return "true";
                    case "C2D":
                        foreach (ListItem litm in ckblTest.Items)
                        {
                            if (litm.Selected)
                            {
                                strReturnResult += litm.Value.Trim() + ";";
                            }
                        }
                        return strReturnResult;
                }
            }
            else if (ctlElement is CheckBox)
            {
                CheckBox ckbTest = (CheckBox)ctlElement;
                switch (strDirect)
                {
                    case "D2C":
                        if (strCheckValue == ckbTest.Text)
                        {
                            ckbTest.Checked = true;
                        }
                        else
                        {
                            if (strCheckValue == "1")
                            {
                                ckbTest.Checked = true;
                            }
                            else
                            {
                                ckbTest.Checked = false;
                            }
                        }
                        return "true";
                    case "C2D":
                        if (ckbTest.Checked)
                        {
                            if (ckbTest.Text.Trim() == "")
                            {
                                return "1";
                            }
                            else
                            {
                                return ckbTest.Text.Trim();
                            }
                        }
                        else
                        {
                            if (ckbTest.Text.Trim() == "")
                            {
                                return "0";
                            }
                            else
                            {
                                return "";
                            }
                        }
                }
            }
            else if (ctlElement is TextBox)
            {

            }
            else if (ctlElement == null)
            {
                bool blLoop = true;
                int i = 1;
                switch (strDirect)
                {
                    case "C2D":
                        if (strControlType == "RadioButton")
                        {
                            while (blLoop)
                            {
                                RadioButton rdbTest = FindControl<RadioButton>(page, strControl + "X" + i);
                                if (rdbTest != null)
                                {
                                    if (rdbTest.ID != null)
                                    {
                                        if (rdbTest.Checked)
                                        {
                                            blLoop = false;
                                            return rdbTest.Text.Trim();
                                        }
                                    }
                                    else
                                    {
                                        blLoop = false;
                                    }
                                }
                                else
                                {
                                    blLoop = false;
                                }
                                i++;
                            }
                        }
                        break;
                    case "D2C":
                        if (strControlType == "RadioButton")
                        {
                            while (blLoop)
                            {
                                RadioButton rdbTest = FindControl<RadioButton>(page, strControl + "X" + i);
                                if (rdbTest != null)
                                {
                                    if (rdbTest.ID != null)
                                    {
                                        if (rdbTest.Text == strCheckValue)
                                        {
                                            blLoop = false;
                                            rdbTest.Checked = true;
                                        }
                                    }
                                    else
                                    {
                                        blLoop = false;
                                    }
                                }
                                else
                                {
                                    blLoop = false;
                                }
                                i++;
                            }
                        }
                        break;
                }
            }
            return "";
        }
        catch (Exception ex)
        {
            ("PanelControlSum, ex=" + ex).WriteLog();
            return "Error, ex=" + ex;
        }
    }
    public static string ResetControlValue(string strControl)
    {
        try
        {
            Page page = (Page)HttpContext.Current.Handler;
            Control ctlElement = (Control)page.Master.FindControl("head2").FindControl(strControl);
            if (ctlElement != null)
            {
                if (ctlElement is Panel)
                {
                    Panel pnlTest = (Panel)ctlElement;
                    foreach (Control ctlTest in pnlTest.Controls)
                    {
                        if (ctlTest is RadioButton)
                        {
                            RadioButton rdbTest = (RadioButton)ctlTest;
                            rdbTest.Checked = false;
                        }
                        else if (ctlTest is RadioButtonList)
                        {
                            RadioButtonList rdblTest = (RadioButtonList)ctlTest;
                            rdblTest.SelectedIndex = -1;
                        }
                        else if (ctlTest is CheckBoxList)
                        {
                            CheckBoxList ckblTest = (CheckBoxList)ctlTest;
                            ckblTest.SelectedIndex = -1;
                        }
                        else if (ctlTest is CheckBox)
                        {
                            CheckBox ckbTest = (CheckBox)ctlTest;
                            ckbTest.Checked = false;
                        }
                        else if (ctlTest is TextBox)
                        {
                            TextBox txtTest = (TextBox)ctlTest;
                            txtTest.Text = "";
                        }
                        else if (ctlTest is Label)
                        {
                            Label lblTest = (Label)ctlTest;
                            lblTest.Text = "";
                        }
                        else if (ctlTest is HiddenField)
                        {
                            HiddenField hidTest = (HiddenField)ctlTest;
                            hidTest.Value = "";
                        }
                        else if (ctlTest is DropDownList)
                        {
                            DropDownList ddlTest = (DropDownList)ctlTest;
                            ddlTest.SelectedIndex = -1;
                            if(ddlTest.ID== "ddl_PatientInfo")
                            {
                                ddlTest.Items.Clear();
                            }
                        }
                        else if (ctlTest is UpdatePanel)
                        {
                            UpdatePanel upnlTest = (UpdatePanel)ctlTest;
                            if (upnlTest.HasControls())
                            {
                                foreach (Control ctlSubTest in upnlTest.Controls[0].Controls)
                                {
                                    if (ctlSubTest is RadioButton)
                                    {
                                        RadioButton rdbTest = (RadioButton)ctlSubTest;
                                        rdbTest.Checked = false;
                                    }
                                    else if (ctlSubTest is RadioButtonList)
                                    {
                                        RadioButtonList rdblTest = (RadioButtonList)ctlSubTest;
                                        rdblTest.SelectedIndex = -1;
                                    }
                                    else if (ctlSubTest is CheckBoxList)
                                    {
                                        CheckBoxList ckblTest = (CheckBoxList)ctlSubTest;
                                        ckblTest.SelectedIndex = -1;
                                    }
                                    else if (ctlSubTest is CheckBox)
                                    {
                                        CheckBox ckbTest = (CheckBox)ctlSubTest;
                                        ckbTest.Checked = false;
                                    }
                                    else if (ctlSubTest is TextBox)
                                    {
                                        TextBox txtTest = (TextBox)ctlSubTest;
                                        txtTest.Text = "";
                                    }
                                    else if (ctlSubTest is Label)
                                    {
                                        Label lblTest = (Label)ctlSubTest;
                                        lblTest.Text = "";
                                    }
                                    else if (ctlSubTest is HiddenField)
                                    {
                                        HiddenField hidTest = (HiddenField)ctlSubTest;
                                        hidTest.Value = "";
                                    }
                                    else if (ctlSubTest is DropDownList)
                                    {
                                        DropDownList ddlTest = (DropDownList)ctlSubTest;
                                        ddlTest.SelectedIndex = -1;
                                    }
                                    else if (ctlSubTest is HtmlTableCell)
                                    {
                                        HtmlTableCell htbTest = (HtmlTableCell)ctlSubTest;
                                        foreach (Control ctl3Test in htbTest.Controls)
                                        {
                                            if (ctl3Test is RadioButton)
                                            {
                                                RadioButton rdbTest = (RadioButton)ctl3Test;
                                                rdbTest.Checked = false;
                                            }
                                            else if (ctl3Test is RadioButtonList)
                                            {
                                                RadioButtonList rdblTest = (RadioButtonList)ctl3Test;
                                                rdblTest.SelectedIndex = -1;
                                            }
                                            else if (ctl3Test is CheckBoxList)
                                            {
                                                CheckBoxList ckblTest = (CheckBoxList)ctl3Test;
                                                ckblTest.SelectedIndex = -1;
                                            }
                                            else if (ctl3Test is CheckBox)
                                            {
                                                CheckBox ckbTest = (CheckBox)ctl3Test;
                                                ckbTest.Checked = false;
                                            }
                                            else if (ctl3Test is TextBox)
                                            {
                                                TextBox txtTest = (TextBox)ctl3Test;
                                                txtTest.Text = "";
                                            }
                                            else if (ctl3Test is Label)
                                            {
                                                Label lblTest = (Label)ctl3Test;
                                                lblTest.Text = "";
                                            }
                                            else if (ctl3Test is HiddenField)
                                            {
                                                HiddenField hidTest = (HiddenField)ctl3Test;
                                                hidTest.Value = "";
                                            }
                                            else if (ctl3Test is DropDownList)
                                            {
                                                DropDownList ddlTest = (DropDownList)ctl3Test;
                                                ddlTest.SelectedIndex = -1;
                                            }
                                        }
                                    }
                                    else if (ctlSubTest is HtmlTableRow)
                                    {
                                        HtmlTableRow htbTest = (HtmlTableRow)ctlSubTest;
                                        int x = htbTest.Controls.Count;
                                        for (int y = 0; y < x; y++)
                                        {
                                            foreach (Control ctl3Test in htbTest.Controls[y].Controls)
                                            {

                                                if (ctl3Test is RadioButton)
                                                {
                                                    RadioButton rdbTest = (RadioButton)ctl3Test;
                                                    rdbTest.Checked = false;
                                                }
                                                else if (ctl3Test is RadioButtonList)
                                                {
                                                    RadioButtonList rdblTest = (RadioButtonList)ctl3Test;
                                                    rdblTest.SelectedIndex = -1;
                                                }
                                                else if (ctl3Test is CheckBoxList)
                                                {
                                                    CheckBoxList ckblTest = (CheckBoxList)ctl3Test;
                                                    ckblTest.SelectedIndex = -1;
                                                }
                                                else if (ctl3Test is CheckBox)
                                                {
                                                    CheckBox ckbTest = (CheckBox)ctl3Test;
                                                    ckbTest.Checked = false;
                                                }
                                                else if (ctl3Test is TextBox)
                                                {
                                                    TextBox txtTest = (TextBox)ctl3Test;
                                                    txtTest.Text = "";
                                                }
                                                else if (ctl3Test is Label)
                                                {
                                                    Label lblTest = (Label)ctl3Test;
                                                    lblTest.Text = "";
                                                }
                                                else if (ctl3Test is HiddenField)
                                                {
                                                    HiddenField hidTest = (HiddenField)ctl3Test;
                                                    hidTest.Value = "";
                                                }
                                                else if (ctl3Test is DropDownList)
                                                {
                                                    DropDownList ddlTest = (DropDownList)ctl3Test;
                                                    ddlTest.SelectedIndex = -1;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                else if (ctlElement is RadioButton)
                {

                }
                else if (ctlElement is RadioButtonList)
                {

                }
                else if (ctlElement is CheckBoxList)
                {

                }
                else if (ctlElement is CheckBox)
                {

                }
                else if (ctlElement is TextBox)
                {

                }
            }
            return "";
        }
        catch (Exception ex)
        {
            ("PanelControlSum, ex=" + ex).WriteLog();
            return "Error, ex=" + ex;
        }
    }
    public static string ResetControlValueForAPP(string strControl)
    {
        try
        {
            Page page = (Page)HttpContext.Current.Handler;
            Control ctlElement = (Control)page.FindControl(strControl);
            if (ctlElement != null)
            {
                if (ctlElement is Panel)
                {
                    Panel pnlTest = (Panel)ctlElement;
                    foreach (Control ctlTest in pnlTest.Controls)
                    {
                        if (ctlTest is RadioButton)
                        {
                            RadioButton rdbTest = (RadioButton)ctlTest;
                            rdbTest.Checked = false;
                        }
                        else if (ctlTest is RadioButtonList)
                        {
                            RadioButtonList rdblTest = (RadioButtonList)ctlTest;
                            rdblTest.SelectedIndex = -1;
                        }
                        else if (ctlTest is CheckBoxList)
                        {
                            CheckBoxList ckblTest = (CheckBoxList)ctlTest;
                            ckblTest.SelectedIndex = -1;
                        }
                        else if (ctlTest is CheckBox)
                        {
                            CheckBox ckbTest = (CheckBox)ctlTest;
                            ckbTest.Checked = false;
                        }
                        else if (ctlTest is TextBox)
                        {
                            TextBox txtTest = (TextBox)ctlTest;
                            txtTest.Text = "";
                        }
                        else if (ctlTest is Label)
                        {
                            Label lblTest = (Label)ctlTest;
                            lblTest.Text = "";
                        }
                        else if (ctlTest is HiddenField)
                        {
                            HiddenField hidTest = (HiddenField)ctlTest;
                            hidTest.Value = "";
                        }
                        else if (ctlTest is DropDownList)
                        {
                            DropDownList ddlTest = (DropDownList)ctlTest;
                            ddlTest.SelectedIndex = -1;
                        }
                        else if (ctlTest is UpdatePanel)
                        {
                            UpdatePanel upnlTest = (UpdatePanel)ctlTest;
                            if (upnlTest.HasControls())
                            {
                                foreach (Control ctlSubTest in upnlTest.Controls[0].Controls)
                                {
                                    if (ctlSubTest is RadioButton)
                                    {
                                        RadioButton rdbTest = (RadioButton)ctlSubTest;
                                        rdbTest.Checked = false;
                                    }
                                    else if (ctlSubTest is RadioButtonList)
                                    {
                                        RadioButtonList rdblTest = (RadioButtonList)ctlSubTest;
                                        rdblTest.SelectedIndex = -1;
                                    }
                                    else if (ctlSubTest is CheckBoxList)
                                    {
                                        CheckBoxList ckblTest = (CheckBoxList)ctlSubTest;
                                        ckblTest.SelectedIndex = -1;
                                    }
                                    else if (ctlSubTest is CheckBox)
                                    {
                                        CheckBox ckbTest = (CheckBox)ctlSubTest;
                                        ckbTest.Checked = false;
                                    }
                                    else if (ctlSubTest is TextBox)
                                    {
                                        TextBox txtTest = (TextBox)ctlSubTest;
                                        txtTest.Text = "";
                                    }
                                    else if (ctlSubTest is Label)
                                    {
                                        Label lblTest = (Label)ctlSubTest;
                                        lblTest.Text = "";
                                    }
                                    else if (ctlSubTest is HiddenField)
                                    {
                                        HiddenField hidTest = (HiddenField)ctlSubTest;
                                        hidTest.Value = "";
                                    }
                                    else if (ctlSubTest is DropDownList)
                                    {
                                        DropDownList ddlTest = (DropDownList)ctlSubTest;
                                        ddlTest.SelectedIndex = -1;
                                    }
                                    else if (ctlSubTest is HtmlTableCell)
                                    {
                                        HtmlTableCell htbTest = (HtmlTableCell)ctlSubTest;
                                        foreach (Control ctl3Test in htbTest.Controls)
                                        {
                                            if (ctl3Test is RadioButton)
                                            {
                                                RadioButton rdbTest = (RadioButton)ctl3Test;
                                                rdbTest.Checked = false;
                                            }
                                            else if (ctl3Test is RadioButtonList)
                                            {
                                                RadioButtonList rdblTest = (RadioButtonList)ctl3Test;
                                                rdblTest.SelectedIndex = -1;
                                            }
                                            else if (ctl3Test is CheckBoxList)
                                            {
                                                CheckBoxList ckblTest = (CheckBoxList)ctl3Test;
                                                ckblTest.SelectedIndex = -1;
                                            }
                                            else if (ctl3Test is CheckBox)
                                            {
                                                CheckBox ckbTest = (CheckBox)ctl3Test;
                                                ckbTest.Checked = false;
                                            }
                                            else if (ctl3Test is TextBox)
                                            {
                                                TextBox txtTest = (TextBox)ctl3Test;
                                                txtTest.Text = "";
                                            }
                                            else if (ctl3Test is Label)
                                            {
                                                Label lblTest = (Label)ctl3Test;
                                                lblTest.Text = "";
                                            }
                                            else if (ctl3Test is HiddenField)
                                            {
                                                HiddenField hidTest = (HiddenField)ctl3Test;
                                                hidTest.Value = "";
                                            }
                                            else if (ctl3Test is DropDownList)
                                            {
                                                DropDownList ddlTest = (DropDownList)ctl3Test;
                                                ddlTest.SelectedIndex = -1;
                                            }
                                        }
                                    }
                                    else if (ctlSubTest is HtmlTableRow)
                                    {
                                        HtmlTableRow htbTest = (HtmlTableRow)ctlSubTest;
                                        int x = htbTest.Controls.Count;
                                        for (int y = 0; y < x; y++)
                                        {
                                            foreach (Control ctl3Test in htbTest.Controls[y].Controls)
                                            {

                                                if (ctl3Test is RadioButton)
                                                {
                                                    RadioButton rdbTest = (RadioButton)ctl3Test;
                                                    rdbTest.Checked = false;
                                                }
                                                else if (ctl3Test is RadioButtonList)
                                                {
                                                    RadioButtonList rdblTest = (RadioButtonList)ctl3Test;
                                                    rdblTest.SelectedIndex = -1;
                                                }
                                                else if (ctl3Test is CheckBoxList)
                                                {
                                                    CheckBoxList ckblTest = (CheckBoxList)ctl3Test;
                                                    ckblTest.SelectedIndex = -1;
                                                }
                                                else if (ctl3Test is CheckBox)
                                                {
                                                    CheckBox ckbTest = (CheckBox)ctl3Test;
                                                    ckbTest.Checked = false;
                                                }
                                                else if (ctl3Test is TextBox)
                                                {
                                                    TextBox txtTest = (TextBox)ctl3Test;
                                                    txtTest.Text = "";
                                                }
                                                else if (ctl3Test is Label)
                                                {
                                                    Label lblTest = (Label)ctl3Test;
                                                    lblTest.Text = "";
                                                }
                                                else if (ctl3Test is HiddenField)
                                                {
                                                    HiddenField hidTest = (HiddenField)ctl3Test;
                                                    hidTest.Value = "";
                                                }
                                                else if (ctl3Test is DropDownList)
                                                {
                                                    DropDownList ddlTest = (DropDownList)ctl3Test;
                                                    ddlTest.SelectedIndex = -1;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                else if (ctlElement is RadioButton)
                {

                }
                else if (ctlElement is RadioButtonList)
                {

                }
                else if (ctlElement is CheckBoxList)
                {

                }
                else if (ctlElement is CheckBox)
                {

                }
                else if (ctlElement is TextBox)
                {

                }
            }
            return "";
        }
        catch (Exception ex)
        {
            ("PanelControlSum, ex=" + ex).WriteLog();
            return "Error, ex=" + ex;
        }
    }

    public static void Redirect(string url, string target, string windowFeatures)
    {
        HttpContext context = HttpContext.Current;

        if ((String.IsNullOrEmpty(target) ||
            target.Equals("_self", StringComparison.OrdinalIgnoreCase)) &&
            String.IsNullOrEmpty(windowFeatures))
        {

            context.Response.Redirect(url);
        }
        else
        {
            Page page = (Page)context.Handler;
            if (page == null)
            {
                throw new InvalidOperationException(
                    "Cannot redirect to new window outside Page context.");
            }
            url = page.ResolveClientUrl(url);

            string script;
            if (!String.IsNullOrEmpty(windowFeatures))
            {
                script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
            }
            else
            {
                script = @"window.open(""{0}"", ""{1}"");";
            }

            script = String.Format(script, url, target, windowFeatures);
            ScriptManager.RegisterStartupScript(page,
                typeof(Page),
                "Redirect",
                script,
                true);
        }
    }
    /// 
    public static void ShowMessage(string strMsg)
    {
        //string script = "alert(\"" + strMsg + "\");";
        Page page = (Page)HttpContext.Current.Handler;
        page.ClientScript.RegisterStartupScript(page.GetType(), "message", "<script defer='' language='javascript'>alert('" + strMsg.ToString() + "');</script>");
    }
    //判斷字串是否是日期型態
    /// <summary>
    /// To the full taiwan date.
    /// </summary>
    /// <param name="datetime">The datetime.</param>
    /// <returns></returns>

    public struct Age
    {
        public int Years;
        public int Months;
        public int Days;
    }
    public static Age CalculateAge(DateTime birthDate, DateTime endDate)
    {
        if (birthDate.Date > endDate.Date)
        {
            throw new ArgumentException("birthDate cannot be higher then endDate", "birthDate");
        }

        int years = endDate.Year - birthDate.Year;
        int months = 0;
        int days = 0;

        // Check if the last year, was a full year.
        if (endDate < birthDate.AddYears(years) && years != 0)
        {
            years--;
        }

        // Calculate the number of months.
        birthDate = birthDate.AddYears(years);

        if (birthDate.Year == endDate.Year)
        {
            months = endDate.Month - birthDate.Month;
        }
        else
        {
            months = (12 - birthDate.Month) + endDate.Month;
        }

        // Check if last month was a complete month.
        if (endDate < birthDate.AddMonths(months) && months != 0)
        {
            months--;
        }

        // Calculate the number of days.
        birthDate = birthDate.AddMonths(months);

        days = (endDate - birthDate).Days;
        Age result;
        result.Years = years;
        result.Months = months;
        result.Days = days;
        return result;
    }

    public static string ToFullTaiwanDate(this DateTime datetime)
    {
        TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

        return string.Format("民國 {0} 年 {1} 月 {2} 日",
            taiwanCalendar.GetYear(datetime),
            datetime.Month,
            datetime.Day);
    }

    /// <summary>
    /// To the simple taiwan date.
    /// </summary>
    /// <param name="datetime">The datetime.</param>
    /// <returns></returns>
    public static string ToSimpleTaiwanDate(this DateTime datetime)
    {
        TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

        return string.Format("{0}/{1}/{2}",
            taiwanCalendar.GetYear(datetime),
            datetime.Month,
            datetime.Day);
    }

    public static string ToSimpleTaiwanYear(this DateTime datetime)
    {
        TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

        return string.Format("{0}",
            taiwanCalendar.GetYear(datetime));
    }

    public static Boolean IsDateTime(string strDateTime)
    {
        bool boolreturn;
        try
        {
            DateTime dt = DateTime.Parse(strDateTime);
            boolreturn = true;
        }
        catch
        {
            boolreturn = false;
        }
        return boolreturn;
    }

    //判斷字串是否是日期型態,正則表示法
    public static Boolean IsValidDate(string strIn)
    {
        return Regex.IsMatch(strIn, @"^(?ni:(?=\d)((?'year'((1[6-9])|([2-9]\d))\d\d)(?'sep'[/.-])(?'month'0?[1-9]|1[012])\2(?'day'((?<!(\2((0?[2469])|11)\2))31)|(?<!\2(0?2)\2)(29|30)|((?<=((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(16|[2468][048]|[3579][26])00)\2\3\2)29)|((0?[1-9])|(1\d)|(2[0-8])))(?:(?=\x20\d)\x20|$))?((?<time>((0?[1-9]|1[012])(:[0-5]\d){0,2}(\x20[AP]M))|([01]\d|2[0-3])(:[0-5]\d){1,2}))?)$");
    }

    public static bool isNumber(string s)
    {
        int Flag = 0;
        char[] str = s.ToCharArray();
        for (int i = 0; i < str.Length; i++)
        {
            if (Char.IsNumber(str[i]))
            {
                Flag++;
            }
            else
            {
                Flag = -1;
                break;
            }
        }

        if (Flag > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string GetUserIP()
    {
        string UserIP = "";
        string UserIP_Forwarded = "";
        string UserIP_Remote = "";
        if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
        {
            UserIP_Forwarded = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        }
        if (System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
        {
            UserIP_Remote = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
        }
        UserIP = "FOR : " + UserIP_Forwarded + ", REMOTE : " + UserIP_Remote;
        return UserIP;
    }
    public static void UserActionLog(string strUserID, string strLog, string strUserIP)
    {
        string strSql = "";
        strSql = "Insert into tblUserActionLog (UserID,UserAction,UserIP) " +
            " values(@UserID,@UserLog,@UserIP) ";
        var connsql = new CMS_db();
        SqlConnection conn = new SqlConnection(connsql.ConnStr());
        SqlCommand sqlcomm = new SqlCommand(strSql, conn);
        sqlcomm.CommandTimeout = 600;
        sqlcomm.CommandText = strSql;
        sqlcomm.Parameters.Clear();
        sqlcomm.Parameters.Add("@UserID", SqlDbType.NVarChar).Value = strUserID;
        sqlcomm.Parameters.Add("@UserLog", SqlDbType.NVarChar).Value = strLog;
        sqlcomm.Parameters.Add("@UserIP", SqlDbType.NVarChar).Value = strUserIP;
        conn.Open();
        sqlcomm.ExecuteNonQuery();
        sqlcomm.Dispose();
        conn.Close();
        conn.Dispose();
    }
    public static Boolean CheckIDCard(string strID)
    {
        try
        {
            strID.WriteLog();
            string Letter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //string[] MirrorNumber = new string[] { "10", "11", "12", "13", "14", "15"
            //    , "16", "17", "34", "18", "19", "20", "21", "22", "35", "23", "24", "25"
            //    , "26", "27", "28", "29", "30", "41", "42", "33" };
            int[] MirrorNumber = new int[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 30, 41, 42, 33 };
            int[] Checker = { 1, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
            string N1 = (strID.Substring(0, 1)).ToUpper();
            int I, N1Ten, N1Unit, Total;
            /*
        byte Pos = InStr(Letter, N1)
        Dim strReturn As String
        If Pos <= 0 Then
            strReturn = "申請人身份證字號第 1 碼必須為英文字母"
        ElseIf Len(strID) < 10 Then
            strReturn = "申請人身份證字號需要 10 碼"
        Else
            For I = 2 To 10
                If Not(Asc(Mid(strID, I, 1)) >= 48 And Asc(Mid(strID, I, 1)) <= 57) Then
                    strReturn = "申請人身份證字號第 2 ~ 9 碼必須為數字"
                End If
            Next
            If(Mid(strID, 2, 1) <> "1" And Mid(strID, 2, 1) <> "2") Then
                strReturn = "申請人身份證字號第 2 碼必須為 1 或 2"
            Else
                N1Ten = Mid(MirrorNumber(Pos - 1), 1, 1)
                N1Unit = Mid(MirrorNumber(Pos - 1), 2, 1)
                Total = N1Ten * Checker(0) + N1Unit * Checker(1)
                For I = 2 To 10
                    Total = Total + Mid(strID, I, 1) * Checker(I)
                Next
                If Total Mod 10 = 0 Then
                    strReturn = ""
                Else
                    strReturn = "申請人身份證字號輸入錯誤！"
                End If
            End If
        End If
        Return strReturn
        */
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public static void ReplaySession()
    {
        /*
        HttpContext.Current.Session["MD5"] = HttpContext.Current.Session["MD5"];
        HttpContext.Current.Session["p"] = HttpContext.Current.Session["p"];
        HttpContext.Current.Session["UserIDNAME"] = HttpContext.Current.Session["UserIDNAME"];
        HttpContext.Current.Session["Agent_Company"] = HttpContext.Current.Session["Agent_Company"];
        HttpContext.Current.Session["Agent_Team"] = HttpContext.Current.Session["Agent_Team"];
        HttpContext.Current.Session["Agent_Phone"] = HttpContext.Current.Session["Agent_Phone"];
        HttpContext.Current.Session["UserIDNO"] = HttpContext.Current.Session["UserIDNO"];
        HttpContext.Current.Session["UserID"] = HttpContext.Current.Session["UserID"];
        HttpContext.Current.Session["Agent_Name"] = HttpContext.Current.Session["Agent_Name"];
        HttpContext.Current.Session["RoleID"] = HttpContext.Current.Session["RoleID"];
        HttpContext.Current.Session["Agent_LV"] = HttpContext.Current.Session["Agent_LV"];
        HttpContext.Current.Session["Agent_Mail"] = HttpContext.Current.Session["Agent_Mail"];
        //*/
    }

    public static string GetUserSessionString()
    {
        string strUserSession = "";
        strUserSession = ("Session[\"p\"]=" + System.Web.HttpContext.Current.Session["p"].ToString() + "\r\n"
        + "Session[\"UserIDNAME\"]" + System.Web.HttpContext.Current.Session["UserIDNAME"].ToString() + "\r\n"
        + "Session[\"Agent_Company\"]" + System.Web.HttpContext.Current.Session["Agent_Company"].ToString() + "\r\n"
        + "Session[\"Agent_Team\"]" + System.Web.HttpContext.Current.Session["Agent_Team"].ToString() + "\r\n"
        + "Session[\"Agent_Phone\"]" + System.Web.HttpContext.Current.Session["Agent_Phone"].ToString() + "\r\n"
        + "Session[\"UserIDNO\"]" + System.Web.HttpContext.Current.Session["UserIDNO"].ToString() + "\r\n"
        + "Session[\"UserID\"]" + System.Web.HttpContext.Current.Session["UserID"].ToString() + "\r\n"
        + "Session[\"Agent_Name\"]" + System.Web.HttpContext.Current.Session["Agent_Name"].ToString() + "\r\n"
        + "Session[\"RoleID\"]" + System.Web.HttpContext.Current.Session["RoleID"].ToString() + "\r\n"
        + "Session[\"Agent_LV\"]" + System.Web.HttpContext.Current.Session["Agent_LV"].ToString() + "\r\n"
        + "Session[\"Agent_Mail\"]" + System.Web.HttpContext.Current.Session["Agent_Mail"].ToString() + "\r\n"
        );

        return strUserSession;
    }
    public static Boolean checkSession(string pstrCheck)
    {
        try
        {
            if (System.Web.HttpContext.Current.Session[pstrCheck].ToString() == null
                || System.Web.HttpContext.Current.Session[pstrCheck].ToString() == "")
            {
                System.Web.HttpContext.Current.Response.Redirect("~/SessionTimeout.aspx");
            }
            else
            {
                System.Web.HttpContext.Current.Session[pstrCheck] = System.Web.HttpContext.Current.Session[pstrCheck];
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static string log_path = "";
    public static string ConnStr()
    {
        string strConn = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["CMS_ENTConnectionString"].ToString() + ";Connection Timeout=300";
        return strConn;
    }
    public static void WriteLog(this String _str)
    {
        //log4net.Config.BasicConfigurator.Configure();
        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        log.Info(_str);
        //StreamWriter oStreamWriter = new StreamWriter(Path_App + _Filename, true, System.Text.Encoding.Default);//, Encoding.Unicode);\
        //oStreamWriter.Write(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + " " + _FileContent + "\r\n");
        //oStreamWriter.Close();
        //return true;
        //StreamWriter oTextWriter = new StreamWriter("arec.log", true, System.Text.Encoding.Default);
        //oTextWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + _str);
        //oTextWriter.Close();
    }
    public static SqlDataReader ExecuteReader(this string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {

        //Create the command and connection
        SqlCommand cmd = new SqlCommand();
        SqlConnection conn = new SqlConnection(connectionString);

        try
        {
            //Prepare the command to execute
            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

            //Execute the query, stating that the connection should close when the resulting datareader has been read
            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            cmd.Parameters.Clear();
            return rdr;

        }
        catch
        {
            //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
            conn.Close();
            throw;
        }
    }
    public static int ExecuteNonQuery(this string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
        // Create a new  Sql command
        SqlCommand cmd = new SqlCommand();

        //Create a connection
        using (SqlConnection connection = new SqlConnection(connectionString))
        {

            //Prepare the command
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);

            //Execute the command
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            connection.Dispose();
            connection.Close();
            return val;
        }
    }

    /// <summary>
    /// Internal function to prepare a command for execution by the database
    /// </summary>
    /// <param name="cmd">Existing command object</param>
    /// <param name="conn">Database connection object</param>
    /// <param name="trans">Optional transaction object</param>
    /// <param name="cmdType">Command type, e.g. stored procedure</param>
    /// <param name="cmdText">Command test</param>
    /// <param name="commandParameters">Parameters for the command</param>
    private static void PrepareCommand(this SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
    {

        //Open the connection if required
        if (conn.State != ConnectionState.Open)
            conn.Open();

        //Set up the command
        cmd.Connection = conn;
        cmd.CommandText = cmdText;
        cmd.CommandType = cmdType;

        //Bind it to the transaction if it exists
        if (trans != null)
            cmd.Transaction = trans;

        // Bind the parameters passed in
        if (commandParameters != null)
        {
            foreach (SqlParameter parm in commandParameters)
                cmd.Parameters.Add(parm);
        }
    }

    /// <summary>
    /// Return DataTable
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="cmdType"></param>
    /// <param name="cmdText"></param>
    /// <param name="commandParameters"></param>
    /// <returns></returns>
    /// 

    public static DataTable GetDataTable(this string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlDataReader dr = ExecuteReader(connectionString, cmdType, cmdText, commandParameters))
            {
                dt.Load(dr);
            }
            return dt;
        }
        catch (Exception e)
        {
            WriteLog("GetDataTable副程式執行失敗, 錯誤訊息: " + e.ToString());
            return null;
        }
    }

    public static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public static void WriteErrorLog(this Exception exception)
    {
        ILog logger = null;
        logger = LogManager.GetLogger("GeneralLogger");
        logger.Error(exception.Source);
        logger.Error(System.Web.HttpContext.Current.Request.Path);
        logger.Error(exception.Message);
        logger.Error(exception.StackTrace);

    }
    public static string SendRequest(this string uri, string method, string contentType, string body)
    {
        /*
                    string Url = "http://localhost:4953/Home/Login";
                    HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
                    string result = null;
                    request.Method = "POST";    // 方法
                    request.KeepAlive = true; //是否保持連線
                    request.ContentType = "application/x-www-form-urlencoded";

                    string param = "id=anyun&password=pass";
                    byte[] bs = Encoding.ASCII.GetBytes(param);

                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                    }

                    using (WebResponse response = request.GetResponse())
                    {
                        StreamReader sr = new StreamReader(response.GetResponseStream());
                        result = sr.ReadToEnd();
                        sr.Close();
                    }

                    Console.WriteLine(result);
                    Console.ReadKey();
        */


        string responseBody = String.Empty;

        HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));
        req.Method = method;
        if (!String.IsNullOrEmpty(contentType))
        {
            req.ContentType = contentType;
        }
        if (!String.IsNullOrEmpty(body))
        {
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            req.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
            req.GetRequestStream().Close();
        }
        //System.Threading.Thread.Sleep(500);
        HttpWebResponse resp;
        try
        {
            resp = (HttpWebResponse)req.GetResponse();
        }
        catch (WebException e)
        {
            resp = (HttpWebResponse)e.Response;
        }

        Stream respStream = resp.GetResponseStream();
        if (respStream != null)
        {
            responseBody = new StreamReader(respStream).ReadToEnd();
        }
        return responseBody;
    }

    public static bool WinSockSend(this object sender, EventArgs e)
    {
        //Socket s;
        //string ReturnData = "";
        //string ClientIP = "";
        //string d = "";          //傳送的字串
        //byte[] bs = null;       //傳送的字串轉成byte
        //string strExtNum = "";  //分機從登入的帳號去尋找lottery.dbo.AgentBase對應的分機
        //ClientIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //if (ClientIP == String.Empty || ClientIP == null)
        //{
        //    ClientIP = Request.ServerVariables["REMOTE_ADDR"];
        //    if (ClientIP == "::1")
        //        ClientIP = "127.0.0.1";
        //}
        //s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //s.Connect(ClientIP, 7997);

        ////傳值給對方
        //d = "GetInfo"
        //  + Convert.ToChar(13).ToString()
        //  + strExtNum
        //  + Convert.ToChar(13).ToString();
        //bs = System.Text.Encoding.ASCII.GetBytes(d);
        //s.Send(bs, bs.Length, 0);

        ////接收對方傳回來的值
        //string recvStr = "";
        //int intByte;
        //byte[] recvByte = new byte[100];
        //intByte = s.Receive(recvByte, recvByte.Length, 0);
        //recvStr = System.Text.Encoding.ASCII.GetString(recvByte, 0, intByte);
        return true;
    }

    //public static ResMsg GetMsg(this TaiDocResponseCode intResultCode, string strMsg, HttpStatusCode intStatusCode)
    //{
    //    ResMsg msg = new ResMsg()
    //    {
    //        status = true,
    //        ResponseCode = intResultCode,
    //        ResponseMsg = strMsg,
    //        httpstatusCode = intStatusCode
    //    };
    //    return msg;
    //}
    /// <summary>
    /// 32位MD5加密
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Md5Hash(string input)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }
}

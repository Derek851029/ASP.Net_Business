using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.IO;
using NPOI.SS.Util;

/// <summary>
/// 派工系統 - 以雇主服務統計 ( 前 10 家 )
/// </summary>
public class ReportRepository_101
{
    #region 屬性
    private string StartDate { get; set; }
    private string EndDate { get; set; }
    private string Supervisor_Name { get; set; }
    private string Agent_Name { get; set; }

    private string Service { get; set; }
    private string Service_ID { get; set; }
    private string Client_Name { get; set; }//客戶
    private string Project { get; set; }//專案

    private string Teamwork_name { get; set; }//合作廠商
    private string Handle_Agent { get; set; }//會計人員
    private string Type { get; set; }//會計類型
    private string Revenue { get; set; }//營收
    private string profit { get; set; }//利潤
    private string Receipt { get; set; }//收款日期
    private string SetupTime { get; set; }//紀錄時間
    private string ProjectPartner { get; set; }//紀錄時間
    private object whereobject
    {
        /*get
        {
            if (string.IsNullOrEmpty(Client_Name))
                return new { StartDate, EndDate, Type, Service, Service_ID };
            else if (string.IsNullOrEmpty(Project))
            {
                return new { StartDate, EndDate, Create_Team, Type_Value, Service, Service_ID };
            }
            else if (string.IsNullOrEmpty(Handle_Agent))
            {
                return new { StartDate, EndDate, Create_Team, Type_Value, Service, Service_ID };
            }
            else if (string.IsNullOrEmpty(Type))
            {
                return new { StartDate, EndDate, Create_Team, Type_Value, Service, Service_ID };
            }
            else if (string.IsNullOrEmpty(Teamwork_Name))
            {
                return new { StartDate, EndDate, Create_Team, Type_Value, Service, Service_ID };
            }
        }//*/
        get
        {
            return new { StartDate, EndDate, Client_Name, Project, Teamwork_name, Handle_Agent, Type , ProjectPartner };
        }
    }

    /// <summary>
    /// SQL
    /// </summary>
    private string QuerySqlStr
    {
        get
        {
            //string Sqlstr = @"select * from Agent_Schedule " +
            //    "where convert(varchar(10), Service_Start_Time , 120) between @StartDate AND @EndDate {0} {1} " +  // {2} {3}
            //    "order by convert(varchar(10), Service_Start_Time , 120) desc ";
            string Sqlstr = @" Select SetupTime,Client_Name,Project,Type,Teamwork_Name,Handle_Agent,Revenue,profit,Receipt,Project_Partner,Project_Proportion,Project_Bonus " +
                " From ( " +
                " Select a.SetupTime as SetupTime, a.Client_Name as Client_Name, a.Project as Project, m.Type as Type, '' as Teamwork_Name, a.Handle_Agent as Handle_Agent, convert(int, a.Transaction_Signing) + convert(int, a.Transaction_Online) + convert(int, a.Transaction_Acceptance) as Revenue, " +
                " convert(int, a.Transaction_Signing) + convert(int, a.Transaction_Online) + convert(int, a.Transaction_Acceptance) - convert(int, c.Transaction_Signing) - convert(int, c.Transaction_Online) - convert(int, c.Transaction_Acceptance) as profit, a.Receipt_Acceptance as Receipt, " +
                " e.Handle_Agent as Project_Partner, '' as Project_Proportion, '' as Project_Bonus " +
                " FROM [DimaxCallcenter].[dbo].AccountingData m " +
                " left join [DimaxCallcenter].[dbo].AccountingData a on a.Project = m.Project and a.Type='開發' " +
                " left join [DimaxCallcenter].[dbo].AccountingData b on b.Project = m.Project and b.Type='維護' " +
                " left join [DimaxCallcenter].[dbo].AccountingTeamworkData c on c.Project = m.Project and c.Type='開發' " +
                " left join [DimaxCallcenter].[dbo].AccountingTeamworkData d on d.Project = m.Project and d.Type='維護' " +
                " left join [DimaxCallcenter].[dbo].ProjectData e on e.Project = m.Project and e.Type ='開發' " +
                " Where convert(varchar(10), a.SetupTime , 120) between @StartDate AND @EndDate AND m.Type='開發' {0} {1} {2} {3} {5} {6} " +//{0} {1} {2} {3}
                " Group by a.SetupTime, a.Client_Name, a.Project, a.Handle_Agent, m.Type , a.Transaction_Signing, a.Transaction_Online, a.Transaction_Acceptance, c.Transaction_Signing, c.Transaction_Online, c.Transaction_Acceptance, a.Receipt_Acceptance, e.Handle_Agent " +
                " union all " +
                " Select b.SetupTime as SetupTime, b.Client_Name as Client_Name, b.Project as Project, m.Type as Type, '' as Teamwork_Name, b.Handle_Agent as Handle_Agent, b.Maintain_Transaction as Revenue, " +
                " convert(int, b.Maintain_Transaction) - convert(int, d.Maintain_Transaction) as profit, b.Maintain_Receipt as Receipt, e.Handle_Agent as Project_Partner, '' as Project_Proportion, '' as Project_Bonus " +
                " FROM [DimaxCallcenter].[dbo].AccountingData m " +
                " left join [DimaxCallcenter].[dbo].AccountingData a on a.Project = m.Project and a.Type='開發' " +
                " left join [DimaxCallcenter].[dbo].AccountingData b on b.Project = m.Project and b.Type='維護' " +
                " left join [DimaxCallcenter].[dbo].AccountingTeamworkData c on c.Project = m.Project and c.Type='開發' " +
                " left join [DimaxCallcenter].[dbo].AccountingTeamworkData d on d.Project = m.Project and d.Type='維護' " +
                " left join [DimaxCallcenter].[dbo].ProjectData e on e.Project = m.Project and e.Type ='維護' " +
                " Where convert(varchar(10), b.SetupTime , 120) between @StartDate AND @EndDate AND m.Type='維護' {0} {1} {2} {3} {5} {6} " +//{0} {1} {2} {3}
                " Group by b.SetupTime, b.Client_Name, b.Project, b.Handle_Agent, m.Type , b.Maintain_Transaction, d.Maintain_Transaction, b.Maintain_Receipt, e.Handle_Agent " +
                " union all " +
                " Select c.SetupTime as SetupTime, c.Client_Name as Client_Name, c.Project as Project, m.Type as Type, c.Teamwork_name as Teamwork_Name, c.Handle_Agent as Handle_Agent, -convert(int, c.Transaction_Signing) - convert(int, c.Transaction_Online) - convert(int, c.Transaction_Acceptance) as Revenue, " +
                " convert(int, a.Transaction_Signing) + convert(int, a.Transaction_Online) + convert(int, a.Transaction_Acceptance) - convert(int, c.Transaction_Signing) - convert(int, c.Transaction_Online) - convert(int, c.Transaction_Acceptance) as profit, c.Receipt_Acceptance as Receipt,  " +
                " '' as Project_Partner, '' as Project_Proportion, '' as Project_Bonus " +
                " FROM [DimaxCallcenter].[dbo].AccountingTeamworkData m " +
                " left join [DimaxCallcenter].[dbo].AccountingData a on a.Project = m.Project and a.Type='開發' " +
                " left join [DimaxCallcenter].[dbo].AccountingData b on b.Project = m.Project and b.Type='維護' " +
                " left join [DimaxCallcenter].[dbo].AccountingTeamworkData c on c.Project = m.Project and c.Type='開發' " +
                " left join [DimaxCallcenter].[dbo].AccountingTeamworkData d on d.Project = m.Project and d.Type='維護' " +
                " Where  convert(varchar(10), c.SetupTime , 120) between @StartDate AND @EndDate AND m.Type='開發' {0} {1} {2} {3} {4} " +//{0} {1} {2} {3} {4} 
                " Group by c.SetupTime, c.Client_Name, c.Project, c.Handle_Agent, m.Type , a.Transaction_Signing, a.Transaction_Online, a.Transaction_Acceptance, c.Transaction_Signing, c.Transaction_Online, c.Transaction_Acceptance, c.Receipt_Acceptance, c.Teamwork_name " +
                " union all " +
                " Select d.SetupTime as SetupTime, d.Client_Name as Client_Name, d.Project as Project, m.Type as Type, d.Teamwork_name as Teamwork_Name, d.Handle_Agent as Handle_Agent, - convert(int, d.Maintain_Transaction) as Revenue,  " +
                " convert(int, b.Maintain_Transaction) - convert(int, d.Maintain_Transaction) as profit, d.Maintain_Receipt as Receipt , '' as Project_Partner, '' as Project_Proportion, '' as Project_Bonus " +
                " FROM [DimaxCallcenter].[dbo].AccountingTeamworkData m " +
                " left join [DimaxCallcenter].[dbo].AccountingData a on a.Project = m.Project and a.Type='開發' " +
                " left join [DimaxCallcenter].[dbo].AccountingData b on b.Project = m.Project and b.Type='維護' " +
                " left join [DimaxCallcenter].[dbo].AccountingTeamworkData c on c.Project = m.Project and c.Type='開發' " +
                " left join [DimaxCallcenter].[dbo].AccountingTeamworkData d on d.Project = m.Project and d.Type='維護' " +
                " Where  convert(varchar(10), d.SetupTime , 120) between @StartDate AND @EndDate AND m.Type='維護' {0} {1} {2} {3} {4} " +//{0} {1} {2} {3} {4} 
                " Group by d.SetupTime, d.Client_Name, d.Project, d.Handle_Agent, m.Type , b.Maintain_Transaction, d.Maintain_Transaction, d.Maintain_Receipt, d.Teamwork_name " +
                " ) as T1 " +
                " Order by SetupTime desc ";
            return string.Format(Sqlstr,
                string.IsNullOrEmpty(Client_Name) ? string.Empty : " AND m.Client_Name=@Client_Name ",
                string.IsNullOrEmpty(Project) ? string.Empty : " AND m.Project=@Project ",
                string.IsNullOrEmpty(Handle_Agent) ? string.Empty : " AND m.Handle_Agent=@Handle_Agent ",
                string.IsNullOrEmpty(Type) ? string.Empty : " AND m.Type=@Type ",
                string.IsNullOrEmpty(Teamwork_name) ? string.Empty : " AND m.Teamwork_name=@Teamwork_name ",
                string.IsNullOrEmpty(Teamwork_name) ? string.Empty : " AND m.Teamwork_Name is not null ",
                string.IsNullOrEmpty(ProjectPartner) ? string.Empty : " AND e.Handle_Agent=@ProjectPartner "
            );
        }
    }

    #endregion

    public ReportRepository_101(string StartDate, string EndDate, string Client_Name, string Project, string Type, string Teamwork_name, string Handle_Agent, string ProjectPartner)
    {
        this.StartDate = StartDate;
        this.EndDate = EndDate;
        if (Client_Name == "所有客戶")
            Client_Name = "";
        this.Client_Name = Client_Name;
        if (Project == "所有專案")
            Project = "";
        this.Project = Project;
        if (Teamwork_name == "所有廠商")
            Teamwork_name = "";
        this.Teamwork_name = Teamwork_name;
        if (Handle_Agent == "全部會計人員")
            Handle_Agent = "";
        this.Handle_Agent = Handle_Agent;
        if (Type == "全部")
            Type = "";
        this.Type = Type;
        if (ProjectPartner == "所有專案人員")
            ProjectPartner = "";
        this.ProjectPartner = "";

    }
    public string GetView()
    {
        var list = DBTool.Query<SelfCompleteServiceData>(QuerySqlStr, whereobject);

        IWorkbook workbook = new XSSFWorkbook();
        SetSheet(workbook, list.ToList());
        // Output the HTML file
        string filename = Guid.NewGuid().ToString("D") + ".html";
        string pathstr = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", filename);
        ExcelTool.ConverHTML(workbook, pathstr);
        workbook.Close();
        return filename;
    }
    public byte[] GetReport()
    {
        var list = DBTool.Query<SelfCompleteServiceData>(QuerySqlStr, whereobject);

        IWorkbook workbook = new XSSFWorkbook();
        SetSheet(workbook, list.ToList());
        using (MemoryStream memorystream = new MemoryStream())
        {
            workbook.Write(memorystream);
            workbook.Close();
            return memorystream.ToArray();
        }
    }

    private void SetSheet(IWorkbook workbook, List<SelfCompleteServiceData> list)
    {
        ISheet sheet = workbook.CreateSheet("會計管理報表");
        int colindex = 0;
        int rowindex = 0;
        var datagroup = from a in list
                        group a by "";  //RE_Date(a.Service_Start_Time);
        IRow row = null;
        foreach (var item in datagroup)
        {
            colindex = 0;
            row = sheet.CreateRow(rowindex++);
            row.CreateCell(colindex).SetCellValue(item.Key);
            #region 表頭
            colindex = 0;
            row = sheet.CreateRow(rowindex++);
            row.CreateCell(colindex++).SetCellValue("序號");
            row.CreateCell(colindex++).SetCellValue("紀錄時間");
            row.CreateCell(colindex++).SetCellValue("客戶名稱");
            row.CreateCell(colindex++).SetCellValue("專案名稱");
            row.CreateCell(colindex++).SetCellValue("專案類型");
            row.CreateCell(colindex++).SetCellValue("合作廠商");
            row.CreateCell(colindex++).SetCellValue("會計人員");
            row.CreateCell(colindex++).SetCellValue("營收");
            row.CreateCell(colindex++).SetCellValue("利潤");
            row.CreateCell(colindex++).SetCellValue("收款日期");
            row.CreateCell(colindex++).SetCellValue("專案人員");
            row.CreateCell(colindex++).SetCellValue("績效比重");
            row.CreateCell(colindex++).SetCellValue("績效獎金");
            #endregion

            int total = 0;
            foreach (var subitem in item)
            {
                total++;
                colindex = 0;
                row = sheet.CreateRow(rowindex++);
                row.CreateCell(colindex++).SetCellValue(total);
                row.CreateCell(colindex++).SetCellValue(RE(subitem.SetupTime));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.Client_Name));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.Project));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.Type));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.Teamwork_name));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.Handle_Agent));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.Revenue));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.profit));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.Receipt));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.Project_Partner));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.Project_Proportion));
                row.CreateCell(colindex++).SetCellValue(RE(subitem.Project_Bonus));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Client_Name));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Case_ID));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Agent_ID));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Supervisor_Name));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Agent_Department));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Agent_Team));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Agent_Name));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Case_Address));  //10 +1
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Service_Name));
                //row.CreateCell(colindex++).SetCellValue(RE_Time(subitem.Service_Start_Time));
                //row.CreateCell(colindex++).SetCellValue(RE_Time(subitem.Service_End_Time));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Service_Frequency));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.Insert_Agent));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.ADJ_Agent));
                //row.CreateCell(colindex++).SetCellValue(RE_Date(subitem.Update_Time));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.GPS_N));
                //row.CreateCell(colindex++).SetCellValue(RE(subitem.GPS_E));
                //row.CreateCell(colindex++).SetCellValue(RE_Time(subitem.GPS_Time));
                //row.CreateCell(colindex++).SetCellValue(subitem.CREATE_DATE.ToString("yyyy/MM/dd HH:mm"));
            }
            sheet.CreateRow(rowindex++);
            row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("會計管理報表 " + StartDate + " 至 " + EndDate + "  共 " + total + " 筆");
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 18));

        }
    }
    public class SelfCompleteServiceData
    {
        public string Type_Value { get; set; }
        public string SYS_ID { get; set; }
        public string MNo { get; set; }
        public string Type { get; set; }
        public string Service { get; set; }
        public string ServiceName { get; set; }
        public string Cust_ID { get; set; }
        public string Cust_Name { get; set; }
        public string Labor_Company { get; set; }
        public string Labor_Team { get; set; }
        public string Labor_ID { get; set; }
        public string Labor_EName { get; set; }
        public string Labor_CName { get; set; }
        public string Labor_Country { get; set; }
        public string Labor_PID { get; set; }
        public string Labor_RID { get; set; }
        public string Labor_EID { get; set; }
        public string Labor_Phone { get; set; }
        public string Labor_Address { get; set; }
        public string Labor_Address2 { get; set; }
        public string Labor_Valid { get; set; }
        public string LocationStart { get; set; }
        public string LocationEnd { get; set; }
        public string Location { get; set; }
        public string PostCode { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone2 { get; set; }
        public string ContactPhone3 { get; set; }
        public string Contact_Co_TEL { get; set; }
        public string Hospital { get; set; }
        public string HospitalClass { get; set; }
        public string Question { get; set; }
        public string Question2 { get; set; }
        public string Answer { get; set; }
        public DateTime Time_01 { get; set; }
        public DateTime Time_02 { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public DateTime StartTime { get; set; }
        public string UpdateDate { get; set; }
        public string LastUpdateDate { get; set; }
        public string FinalUpdateDate { get; set; }

        public string UPDATE_ID { get; set; }
        public string UPDATE_Name { get; set; }
        public DateTime UPDATE_TIME { get; set; }
        public string Create_Team { get; set; }
        public string Create_ID { get; set; }
        public string Create_Name { get; set; }
        public DateTime Create_TIME { get; set; }
        public string Allow_ID { get; set; }
        public string Allow_Name { get; set; }
        public DateTime Allow_Time { get; set; }
        public string Dispatch_ID { get; set; }
        public string Dispatch_Name { get; set; }
        public DateTime Dispatch_Time { get; set; }
        public string Close_ID { get; set; }
        public string Close_Name { get; set; }
        public DateTime Close_Time { get; set; }
        public string Cancel_ID { get; set; }
        public string Cancel_Name { get; set; }
        public DateTime Cancel_Time { get; set; }

        public string Agent_ID { get; set; }
        public string Supervisor_Name { get; set; }
        public string Agent_Department { get; set; }
        public string Agent_Team { get; set; }
        public string Agent_Name { get; set; }
        public string Insert_Agent { get; set; }
        public string ADJ_Agent { get; set; }
        public string Client_Name { get; set; }
        public string Case_ID { get; set; }
        public string Case_Address { get; set; }
        public string Service_Name { get; set; }
        public string Service_Frequency { get; set; }
        public string GPS_N { get; set; }
        public string GPS_E { get; set; }
        public DateTime Service_Start_Time { get; set; }
        public DateTime Service_End_Time { get; set; }
        public DateTime Update_Time { get; set; }
        public DateTime GPS_Time { get; set; }

        public string Year { get; set; }
        public string Month { get; set; }
        public string Mission { get; set; }
        public string OnClick { get; set; }

        public string SetupTime { get; set; }
        public string Project { get; set; }
        public string Teamwork_name { get; set; }
        public string Handle_Agent { get; set; }
        public string Revenue { get; set; }
        public string profit { get; set; }
        public string Receipt { get; set; }
        public string Project_Partner { get; set; }
        public string Project_Proportion { get; set; }
        public string Project_Bonus { get; set; }
    }

    private string RE(string TXT)
    {
        try
        {
            TXT = TXT.Trim();
            if (!string.IsNullOrEmpty(TXT))
            {
                return TXT;
            }
            else
            {
                return "";
            }
        }
        catch
        {
            return "";
        }
    }

    private string RE_Time(DateTime TXT)
    {
        try
        {
            string time = TXT.ToString("yyyy/MM/dd hh:mm:ss");
            //return TXT.ToString("yyyy/MM/dd hh:mm:ss");
            if (time == "0001/01/01 12:00:00")
            {
                return " ";
            }
            else
            {
                return TXT.ToString("yyyy/MM/dd hh:mm:ss");
            }
        }
        catch
        {
            return " ";
        }
    }
    private string RE_Date(DateTime TXT)
    {
        try
        {
            string time = TXT.ToString("yyyy/MM/dd");
            //return TXT.ToString("yyyy/MM/dd hh:mm:ss");
            if (time == "0001/01/01")
            {
                return " ";
            }
            else
            {
                return TXT.ToString("yyyy/MM/dd");
            }
        }
        catch
        {
            return " ";
        }
    }

    private string time_change(string time)
    {
        try
        {
            //string A = "";
            int t = int.Parse(time);
            int mm = t % 60;
            int hh = (t / 60) % 24;
            int dd = (t / 60) / 24;
            //if (t < 0) { A = "提早 "; } else if (dd > 2) { A = "延遲 "; }
            return dd + " 天 " + hh.ToString().PadLeft(2, '0') + " 小時 " + mm.ToString().PadLeft(2, '0') + " 分";
        }
        catch
        {
            return "";
        }
    }

    private string ConverIndexToASCII(int index)
    {
        int AscIIindex = 65;
        char character = (char)(AscIIindex + index);
        return character.ToString();
    }
}
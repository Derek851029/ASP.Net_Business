using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Marquee 的摘要描述
/// </summary>
public class Marquee
{
    public int SYSID { get; set; }
    //public string MarqueeClassID
    public string Context { get; set; }
    public string UserID { get; set; }
    public DateTime CreateDate { get; set; }
    public Marquee()
    {

    }
    public static string SearchTotal()
    {
        var result = DBTool.Query<Marquees>(
            "SELECT * FROM Marquee"
            , new { });
        if (result.Any())
            return JsonConvert.SerializeObject(result);
        return "";
    }
    public static bool SearchAndCheck(string context)
    {
        var result = DBTool.Query<Marquees>("SELECT TOP 1 * FROM Marquee WHERE Context = @Context", new { Context = context });
        if (!result.Any())
            return true;
        return false;
    }
    public static bool Add(string context)
    {
        string User = (string)HttpContext.Current.Session["UserID"];
        if (!string.IsNullOrEmpty(User) && !string.IsNullOrWhiteSpace(User))
        {
            var result = DBTool.Query<Marquees>(
            @"INSERT INTO Marquee (Context,CreateDate,UserID)
              VALUES(@Context,GETDATE(),@UserID)"
            , new { Context = context , UserID = User });
            return true;
        }
        return false;
    }


}

/// <summary>
/// Marquees 的摘要描述
/// </summary>
public class Marquees
{
    public int SYSID { get; set; }
    //public string MarqueeClassID
    public string Context { get; set; }
    public string UserID { get; set; }
    public DateTime CreateDate { get; set; }
}
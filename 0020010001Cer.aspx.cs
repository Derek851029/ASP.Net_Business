using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;

public partial class _0020010001Cer : System.Web.UI.Page
{
    protected string type = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        type = Request.Params["type"];
        string key = "abcdefgh"; //與註冊要一樣
        string iv = "12345678"; //與註冊要一樣
        //DES解密
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        des.Key = Encoding.ASCII.GetBytes(key);
        des.IV = Encoding.ASCII.GetBytes(iv);
        byte[] s = new byte[type.Length / 2];
        int j = 0;
        for (int i = 0; i < type.Length / 2; i++)
        {
            s[i] = Byte.Parse(type[j].ToString() + type[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
            j += 2;
        }
        ICryptoTransform desencrypt = des.CreateDecryptor();
        string account =  Encoding.ASCII.GetString(desencrypt.TransformFinalBlock(s, 0, s.Length)); //解密後的字串

        string sqlstr = @"UPDATE DispatchSystem SET Agent_Status = @Agent_Status WHERE UserID = @UserID";
        var a = DBTool.Query<ClassTemplate>(sqlstr, new
        {
            Agent_Status = "在職",
            UserID = account,
        });
    }
}
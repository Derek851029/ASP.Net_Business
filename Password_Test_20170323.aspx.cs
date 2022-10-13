using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

public partial class Password_Test_20170323 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod(EnableSession = true)]
    public static string Encryption(string Password)
    {
        string back = "";
        string key = "ACMEACME";
        string iv = "70472615";

        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        des.Key = Encoding.ASCII.GetBytes(key);
        des.IV = Encoding.ASCII.GetBytes(iv);
        byte[] s = Encoding.ASCII.GetBytes(Password);
        ICryptoTransform desencrypt = des.CreateEncryptor();
        back = BitConverter.ToString(desencrypt.TransformFinalBlock(s, 0, s.Length)).Replace("-", string.Empty);

        return JsonConvert.SerializeObject(new { p = back, KEY = des.Key, IV = des.IV, S = s });
    }

    [WebMethod(EnableSession = true)]
    public static string Decrypted(string Password)
    {
        string back = "";
        string key = "ACMEACME";
        string iv = "70472615";

        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        des.Key = Encoding.ASCII.GetBytes(key);
        des.IV = Encoding.ASCII.GetBytes(iv);
        byte[] s = new byte[Password.Length / 2];
        int j = 0;
        for (int i = 0; i < Password.Length / 2; i++)
        {
            s[i] = Byte.Parse(Password[j].ToString() + Password[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
            j += 2;
        }
        ICryptoTransform desencrypt = des.CreateDecryptor();
        back = Encoding.ASCII.GetString(desencrypt.TransformFinalBlock(s, 0, s.Length));

        return JsonConvert.SerializeObject(new { p = back, KEY = des.Key, IV = des.IV, S = s });
    }

    public class HRM2_Location
    {
        public string SYS_ID { get; set; }
    }
}
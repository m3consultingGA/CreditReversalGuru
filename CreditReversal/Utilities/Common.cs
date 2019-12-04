using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace CreditReversal.Utilities
{
    public class Common
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private SqlConnection sqlcon;
        public string Encrypt(string clearText)
        {
            string EncryptionKey = "M3";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "M3";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public string GetLastUrl()
        {
            string url = "";

            if (HttpContext.Current.Session["Previous_Url"] != null)
            {
                url = HttpContext.Current.Session["Previous_Url"].ToString();
            }

            return url;
        }
        public int SendMail(string ToAddress, string Subject = "", string type = "", string username = "", string Model = "", string role = "", string Password = "", string client = "")
        {
            int retVal = 0;
            try
            {
                MailMessage newmsg = new MailMessage();
                //  string ToAddress = useraccount.EmailAddress;
                newmsg.From = new MailAddress("testm3consulting@gmail.com", "Support");
                newmsg.Subject = Subject;
                newmsg.To.Add(ToAddress);
                newmsg.IsBodyHtml = true;
                newmsg.Body = string.Empty;
                StringBuilder sb = new StringBuilder();

                if (type == "REGISTRATION")
                {
                    if (client != "")
                    {
                        sb.Append("<br /> Dear " + client + ", <br /> <br />");
                    }

                    if (role == "user")
                    {
                        sb.Append("&nbsp; &nbsp; Your registration successfull. <br />");
                        sb.Append("&nbsp; &nbsp;  Your login credentials:  <br />");
                        sb.Append("&nbsp; &nbsp;  User Name : " + username + " <br />");
                        sb.Append("&nbsp; &nbsp;  Password : " + Password + " <br />");
                    }
                    else if(role == "Investor")
                    {
                        sb.Append("&nbsp; &nbsp; Your registration successfull. <br />");
                        sb.Append("&nbsp; &nbsp;  Your login credentials:  <br />");
                        sb.Append("&nbsp; &nbsp;  User Name : " + username + " <br />");
                        sb.Append("&nbsp; &nbsp;  Password : " + Password + " <br />");
                    }
                    else
                    {
                        if (role == "admin")
                        {
                            sb.Append("&nbsp; &nbsp; Investor: " + client + " registered successfully. <br />");
                        }
                        else {
                            sb.Append("&nbsp; &nbsp; Client: " + client + " registered successfully. <br />");                            
                        } 
                    }
                }
                if (type == "Lost_Password")
                {
                    if (username != "")
                    {
                        sb.Append("<br /> Dear User <br /> <br />");
                    }
                    if (username != "" && Password != "")
                    {
                        sb.Append("&nbsp; &nbsp;  Your login credentials:  <br />");
                        sb.Append("&nbsp; &nbsp;  User Name : " + username + " <br />");
                        sb.Append("&nbsp; &nbsp;  Password : " + Password + " <br />");
                    }

                }
                if (type == "Staff-Registration")                {                    if (username != "")                    {                        sb.Append("<br /> Dear " + username + ", <br /> <br />");                    }                    if (username != "" && Password != "")                    {                        sb.Append("&nbsp; &nbsp;  Your login credentials:  <br />");                        sb.Append("&nbsp; &nbsp;  User Name : " + ToAddress + " <br />");                        sb.Append("&nbsp; &nbsp;  Password : " + Password + " <br />");                    }                }
                if (type == "AgentRegistration")                {                    if (username != "")                    {                        sb.Append("<br /> Dear " + username + ", <br /> <br />");                    }                    if (username != "" && Password != "")                    {                        sb.Append("&nbsp; &nbsp;  Your login credentials:  <br />");                        sb.Append("&nbsp; &nbsp;  User Name : " + ToAddress + " <br />");                        sb.Append("&nbsp; &nbsp;  Password : " + Password + " <br />");                    }                }
                sb.Append("<br /><br /> Thanks, <br /> Support Team.");
                newmsg.Body = sb.ToString();
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new System.Net.NetworkCredential(
                    "testm3consulting@gmail.com", "whocares@123");
                smtpClient.Send(newmsg);
                retVal = 1;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return retVal;
        }
        //This Method is used for sending an email to Agent for complete registration
        public bool SendMail(string toAddress = "", string link = "", string agentName = "")
        {
            bool status = false;

            try
            {
                MailMessage newmsg = new MailMessage();
                newmsg.From = new MailAddress("testm3consulting@gmail.com", "Support");
                newmsg.Subject = "Agent Registration";
                newmsg.To.Add(toAddress);
                newmsg.IsBodyHtml = true;
                newmsg.Body = string.Empty;
                StringBuilder sb = new StringBuilder();
                sb.Append("<br /> Dear " + agentName + ", <br /> <br />");
                sb.Append("&nbsp; &nbsp; Please go through the below link to complete your registration process. <br />");
                sb.Append(link);
                sb.Append("<br /><br /> Thanks, <br /> Support Team.");
                newmsg.Body = sb.ToString();
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new System.Net.NetworkCredential(
                    "testm3consulting@gmail.com", "whocares@123");
                smtpClient.Send(newmsg);

                status = true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return status;
        }
        public static string CreateRandomPassword()
        {
            int length = 5;
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }
        public int[] GetYears()
        {
            int currentYear = DateTime.Now.Year.ToString().StringToInt(0);
            List<int> years = new List<int>();
            years.Add(currentYear);
            for (int i = 1; i < 10; i++)
            {
                years.Add(currentYear + i);
            }

            return years.ToArray();
        }

        public int DeleteFile(string filename = "")
        {
            string rootFolder = "~/documents/";
            int res = 0;
            try
            {
                if (File.Exists(Path.Combine(rootFolder, filename)))
                {
                    File.Delete(Path.Combine(rootFolder, filename));
                }
            }
            catch (Exception ex)
            {

            }

            return res;
        }
        public string GetMaskSSN(string SSN = "")        {            StringBuilder sb_masked = new StringBuilder(SSN.Length);            if (!string.IsNullOrEmpty(SSN))
            {

                int mask = 5;

                for (int i = 0; i < SSN.Length; i++)
                {
                    if (i < mask)
                    {
                        sb_masked.Append('*');
                    }
                    else
                    {
                        sb_masked.Append(SSN[i]);
                    }

                    if ((i == 2) || (i == 4)) sb_masked.Append('-');
                }

            }            return sb_masked.ToString();

        }
        public string GetArrayValue(string[] arr, string key)
        {
            string str = "";

            try
            {
                for (int i = 0; i < arr.Length - 1; i++)
                {
                    if (arr[i] == key)
                    {
                        str = arr[i + 1];
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return str;
        }
        public DataSet GetDataSetFromHtml(string HTML, string tag, string from)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;
            DataRow dr = null;
            string TableExpression = "<table[^>]*>(.*?)</table>";
            string HeaderExpression = "<th[^>]*>(.*?)</th>";
            string RowExpression = "<tr[^>]*>(.*?)</tr>";
            string ColumnExpression = "<td[^>]*>(.*?)</td>";
            bool HeadersExist = false;
            int iCurrentColumn = 0;
            int iCurrentRow = 0;

            // Get a match for all the tables in the HTML   
            MatchCollection Tables = Regex.Matches(HTML, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Loop through each table element   
            foreach (Match Table in Tables)
            {

                // Reset the current row counter and the header flag   
                iCurrentRow = 0;
                HeadersExist = false;

                // Add a new table to the DataSet   


                // Create the relevant amount of columns for this table (use the headers if they exist, otherwise use default names)   
                // if (Table.Value.Contains("<th"))
                if (Table.Value.Contains(tag))
                {
                    MatchCollection Tables1 = Regex.Matches(Table.Value, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    foreach (Match Table1 in Tables1)
                    {
                        if (from == "AccountHistory")
                        {
                            dt = new DataTable();
                            dt.Columns.Add("Type");
                            dt.Columns.Add("TransUnion");
                            dt.Columns.Add("Experian");
                            dt.Columns.Add("Equifax");
                        }
                        else if (from == "Inquires")
                        {
                            dt = new DataTable();
                            dt.Columns.Add("Creditor Name");
                            dt.Columns.Add("Type of Business");
                            dt.Columns.Add("Date of inquiry");
                            dt.Columns.Add("Credit Bureau");
                        }
                        if (Table1.Value.Contains("<th"))
                        {
                            // Set the HeadersExist flag   
                            HeadersExist = true;

                            // Get a match for all the rows in the table   
                            MatchCollection Headers = Regex.Matches(Table.Value, HeaderExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                            // Loop through each header element   
                            foreach (Match Header in Headers)
                            {
                                if (!dt.Columns.Contains(Header.Groups[1].ToString()))
                                    dt.Columns.Add(Header.Groups[1].ToString());
                            }
                        }
                        else
                        {
                            for (int iColumns = 1; iColumns <= Regex.Matches(Regex.Matches(Regex.Matches(Table.Value, TableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).ToString(), RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).ToString(), ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase).Count; iColumns++)
                            {
                                dt.Columns.Add("Column " + iColumns);
                            }
                        }
                    }



                    // Get a match for all the rows in the table   
                    MatchCollection Rows = Regex.Matches(Table.Value, RowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    // Loop through each row element   
                    foreach (Match Row in Rows)
                    {

                        // Only loop through the row if it isn't a header row   
                        if (!(iCurrentRow == 0 & HeadersExist == true))
                        {

                            // Create a new row and reset the current column counter   
                            dr = dt.NewRow();
                            iCurrentColumn = 0;

                            // Get a match for all the columns in the row   
                            MatchCollection Columns = Regex.Matches(Row.Value, ColumnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                            // Loop through each column element   
                            try
                            {
                                foreach (Match Column in Columns)
                                {
                                    if (Columns.Count != iCurrentColumn)
                                    {
                                        // Add the value to the DataRow   

                                        dr[iCurrentColumn] = getValue(Column.Groups[1].ToString());
                                    }
                                    // Increase the current column    
                                    iCurrentColumn++;
                                }

                                // Add the DataRow to the DataTable   
                                dt.Rows.Add(dr);
                            }
                            catch (Exception ex)
                            {
                                // break;
                            }


                        }

                        // Increase the current row counter   
                        iCurrentRow += 1;
                    }

                    // Add the DataTable to the DataSet   
                    ds.Tables.Add(dt);
                }

            }

            return ds;

        }

        public string getValue(string txt)
        {
            string value = "";

            try
            {
                string strTUC1 = "<!-- ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'TUC'}}}) " +
                                          "--><ng-repeat ng-repeat=\"tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'TUC'}}})\"" +
                                          " class=\"ng-binding ng-scope\">";
                string strTUC2 = "</ng-repeat><!-- end ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'TUC'}}}) " +
                    "-->";

                string strEXP1 = "<!-- ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EXP'}}}) " +
                   "--><ng-repeat ng-repeat=\"tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EXP'}}})\"" +
                   " class=\"ng-binding ng-scope\">";
                string strEXP2 = "</ng-repeat><!-- end ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EXP'}}}) " +
                    "-->";

                string strEFX1 = "<!-- ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EFX'}}}) " +
                   "--><ng-repeat ng-repeat=\"tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EFX'}}})\"" +
                   " class=\"ng-binding ng-scope\">";
                string strEFX2 = "</ng-repeat><!-- end ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EFX'}}}) " +
                    "-->";
                string strCATUC1 = "<!-- ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'TUC'}}}) -->" +
                 "<ng-repeat ng-repeat=\"tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'TUC'}}})\"" +
                    " class=\"ng-scope\">";
                string strCATUC2 = "<!-- ngIf: tpartition['@accountTypeSymbol']=='Y' -->";
                string strCATUC3 = "<!-- ngIf: tpartition['@accountTypeSymbol']!='Y' --><ng ng-if=\"tpartition['@accountTypeSymbol']!='Y'\"" +
                    " class=\"ng-binding ng-scope\">";

                string strCATEXP1 = "<!-- ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EXP'}}}) -->" +
                 "<ng-repeat ng-repeat=\"tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EXP'}}})\"" +
                    " class=\"ng-scope\">";
                string strCATEFX1 = "<!-- ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EFX'}}}) -->" +
                 "<ng-repeat ng-repeat=\"tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EFX'}}})\"" +
                    " class=\"ng-scope\">";

                string strCATUC4 = "</ng><!-- end ngIf: tpartition['@accountTypeSymbol']!='Y' -->";
                string strComments1 = "<!-- ngRepeat: remark in remarks=makeArray(tradeline.Remark) -->";
                string strComments2 = "<!-- ngIf: remarks.length==0 --><ng ng-if=\"remarks.length==0\" class=\"ng-scope\">-</ng><!-- end ngIf: remarks.length==0 -->";
                string EQF1 = "</ng-repeat><!-- end ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EQF'}}}) -->";
                string EQF2 = "<div ng-repeat=\"remark in remarks=makeArray(tradeline.Remark)\" class=\"ng-binding ng-scope\">";
                string EQF3 = "</div><!-- end ngRepeat: remark in remarks=makeArray(tradeline.Remark) -->";
                string EQF4 = "<!-- ngIf: remarks.length==0 -->";
                string EQF5 = "<!-- ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EXP'}}}) -->";
                string EQF6 = "<!-- ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EQF'}}}) -->";
                string EQF7 = "<!-- ngRepeat: tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'TUC'}}}) -->";
                string EQF8 = "</ng><!-- end ngIf: tpartition['@accountTypeSymbol']!='Y' -->";
                string EQF9 = "<!-- ngRepeat: remark in remarks=makeArray(tradeline.Remark) -->";
                string EQF10 = "<!-- ngIf: remarks.length==0 --><ng ng-if=\"remarks.length==0\" class=\"ng-scope\">-</ng><!-- end ngIf: remarks.length==0 -->";
                string EQF11 = "<ng-repeat ng-repeat=\"tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EQF'}}})\" class=\"ng-binding ng-scope\">";
                string EQF12 = "<ng-repeat ng-repeat=\"tradeline in tdls=(tradlines|filter:{Source:{Bureau:{'@symbol':'EQF'}}})\" class=\"ng-scope\">";
                string str = txt.Replace("&nbsp;", "");
                str = str.Replace(strTUC1, string.Empty); str = str.Replace(strTUC2, string.Empty);
                str = str.Replace(strEXP1, string.Empty); str = str.Replace(strEXP2, string.Empty);
                str = str.Replace(strEFX1, string.Empty); str = str.Replace(strEFX2, string.Empty);
                str = str.Replace(strCATUC1, string.Empty);
                str = str.Replace(strCATUC2, string.Empty);
                str = str.Replace(strCATUC3, string.Empty);
                str = str.Replace(strCATUC4, string.Empty);
                str = str.Replace(strCATEXP1, string.Empty);
                str = str.Replace(strCATEFX1, string.Empty);
                str = str.Replace(strComments1, string.Empty);
                str = str.Replace(strComments2, string.Empty);
                str = str.Replace(EQF1, string.Empty);
                str = str.Replace(EQF2, string.Empty);
                str = str.Replace(EQF3, string.Empty);
                str = str.Replace(EQF4, string.Empty);
                str = str.Replace(EQF5, string.Empty);
                str = str.Replace(EQF6, string.Empty);
                str = str.Replace(EQF7, string.Empty);
                str = str.Replace(EQF8, string.Empty);
                str = str.Replace(EQF9, string.Empty);
                str = str.Replace(EQF10, string.Empty);
                str = str.Replace(EQF11, string.Empty);
                str = str.Replace(EQF12, string.Empty);
                value = str.Trim();
            }
            catch (Exception ex)
            { }
            return value;
        }

        public List<string> GetbanksFormhtml(string html)
        {
            List<string> banks = new List<string>();

            string Div = "<div class=\"sub_header ng-binding ng-scope\">(.*?)</div>";

            MatchCollection Tables = Regex.Matches(html, Div, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase);

            foreach (var item in Tables)
            {
                string val = item.ToString();
                val = val.Replace("<div class=\"sub_header ng-binding ng-scope\">", string.Empty).Trim();
                val = val.Replace("<!-- ngIf: tpartition['@accountTypeSymbol']=='Y' -->", string.Empty).Trim();
                val = val.Replace("</div>", string.Empty).Trim();
                banks.Add(val);
            }

            return banks;
        }

        public string GethtmlBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using CreditReversal.Models;
using CreditReversal.Utilities;

namespace CreditReversal.DAL
{
    public class Traces
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private SqlConnection sqlcon;

        DBUtilities utills = new DBUtilities();
        private Common common = new Common();
        //ExtensionMethods extension = new ExtensionMethods();
        string nl = '\n'.ToString();
        public long InsertTrace(Trace trace)
        {
            long res = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                  string  strCmd = "Insert into Traces(Exception,Error)values(@Exception,@Error)";
                    SqlCommand sqlCmd = new SqlCommand(strCmd, conn);
                    sqlCmd.Parameters.AddWithValue("@Exception", trace.Exception);
                    sqlCmd.Parameters.AddWithValue("@Error", trace.Error);
                    res = utills.ExecuteInsertCommand(sqlCmd, false);
                }
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            return res;
        }
    }
}
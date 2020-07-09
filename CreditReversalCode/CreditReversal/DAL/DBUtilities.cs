using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CreditReversal.Utilities;

namespace CreditReversal.DAL
{
    public class DBUtilities
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private SqlConnection sqlCon = new SqlConnection();
        public string errorMessage = @"";

        public bool OpenDB()
        {
            bool res = false;
            try
            {
                sqlCon.ConnectionString = connectionString;
                sqlCon.Open();
                res = true;
            }
            catch (Exception ex)
            {
                errorMessage += ex.Message + System.Environment.NewLine;
                res = false;
            }

            return res;
        }

        public void CloseDB()
        {
            try { sqlCon.Close(); }
          catch (Exception ex) {  ex.insertTrace("");  }
        }

        public DataSet GetDataSet(string sql)
        {
            return GetDataSet(sql, true);
        }
        public DataSet GetDataSet(string sql, bool closeFlag)
        {
            DataSet res = new DataSet();
            try
            {
                if (sqlCon.State != ConnectionState.Open)
                    OpenDB();
                SqlDataAdapter da = new SqlDataAdapter(sql, sqlCon);
                da.Fill(res);
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            finally
            {
                if (closeFlag)
                    CloseDB();
            }
            return res;
        }


        public DataTable GetDataTable(string sql)
        {
            return GetDataTable(sql, true);
        }
        public DataTable GetDataTable(string sql, bool closeFlag)
        {
            DataTable res = new DataTable();
            try
            {
                if (sqlCon.State != ConnectionState.Open)
                    OpenDB();
                SqlDataAdapter da = new SqlDataAdapter(sql, sqlCon);
                da.Fill(res);
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            finally
            {
                if (closeFlag)
                    CloseDB();
            }
            return res;
        }

        public long ExecuteString(string sql, bool closeFlag)
        {
            SqlCommand cmd = new SqlCommand(sql);
            return ExecuteInsertCommand(cmd, closeFlag);
        }

        public DataTable ExecuteCommand(SqlCommand cmd, bool closeFlag)
        {
            DataTable dt = new DataTable();
            try
            {
                if (sqlCon.State != ConnectionState.Open)
                {
                    OpenDB();
                }
                cmd.Connection = sqlCon;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            finally
            {
                if (closeFlag)
                {
                    sqlCon.Close();
                }
            }
            return dt;
        }

        public long ExecuteInsertCommand(SqlCommand cmd, bool closeFlag)
        {
            long res = 0;
            try
            {
                if (sqlCon.State != ConnectionState.Open)
                {
                    OpenDB();
                }
                cmd.Connection = sqlCon;
                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            finally
            {
                if (closeFlag)
                {
                    sqlCon.Close();
                }
            }

            return res;
        }
        public long ExecuteCommand(string sql, bool closeFlag)
        {
            long res = 0;

            try
            {
                if (sqlCon.State != ConnectionState.Open)
                    OpenDB();
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            finally
            {
                if (closeFlag)
                    CloseDB();
            }
            return res;

        }

        public object ExecuteScalar(string sql, bool closeFlag)
        {
            object res = 0;

            try
            {
                if (sqlCon.State != ConnectionState.Open)
                    OpenDB();
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                res = cmd.ExecuteScalar();
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            finally
            {
                if (closeFlag)
                    CloseDB();
            }
            return res;

        }


        //public bool CheckUser(string user, string password)
        //{
        //    bool res = false;

        //    string sql = "SELECT DealerID from KCM_Telematic_Users where DealerID='" + user + "' AND DealerPassword='" + password + "' AND IsActive='Y'";
        //    DataTable dt = GetDataTable(sql, false);
        //    res = dt.Rows.Count == 1;
        //    sql = "Insert into Login_Audit(LoginID) values('" + user + "')";

        //    long cnt = ExecuteCommand(sql, true);
        //    return res;
        //}
        public DataRow GetDataRow(string sql)
        {
            DataRow row = null;
            DataTable dataTable = new DataTable();
            sqlCon.ConnectionString = connectionString;
            try
            {

                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    row = dataTable.Rows[0];
                }
            }
            catch (Exception ex) {  ex.insertTrace("");  }

            return row;
        }
        public object ExecuteScalarCommand(SqlCommand cmd, bool closeFlag)
        {
            object res = string.Empty;

            try
            {
                if (sqlCon.State != ConnectionState.Open)
                    OpenDB();
                cmd.Connection = sqlCon;
                res = cmd.ExecuteScalar();
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            finally
            {
                if (closeFlag)
                    CloseDB();
            }
            return res;

        }
    }

}
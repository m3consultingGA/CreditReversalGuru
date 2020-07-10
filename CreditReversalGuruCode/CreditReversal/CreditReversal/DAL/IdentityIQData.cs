using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CreditReversal.Models;
using CreditReversal.Utilities;

namespace CreditReversal.DAL
{
	public class IdentityIQData
	{
		DBUtilities objUti = new DBUtilities();
		Common objCommon = new Common();
		string sql = string.Empty;
		long res = 0;


		public long InsertIdentityIQInfo(IdentityIQInfo IQInfo)
		{
			bool status = GetIdentityIQInfo(IQInfo.ClientId);
			try
			{
				if (!status)
				{
					sql = "Insert into IdentityIqInformation(ClientId,Answer,UserName,Password)values(@ClientId,@Answer,@UserName,@Password)";
					SqlCommand sqlCmd = new SqlCommand();
					sqlCmd.Parameters.AddWithValue("@ClientId", IQInfo.ClientId);
					//sqlCmd.Parameters.AddWithValue("@Question", IQInfo.Question);
					sqlCmd.Parameters.AddWithValue("@Answer", IQInfo.Answer);
					sqlCmd.Parameters.AddWithValue("@UserName", IQInfo.UserName);
					sqlCmd.Parameters.AddWithValue("@Password", IQInfo.Password);
					sqlCmd.CommandText = sql;
					res = objUti.ExecuteInsertCommand(sqlCmd, false);
				}
				else
				{
					res = UpdateIdentityIQInfo(IQInfo, "insert");
				}

			}
			catch (Exception ex) { ex.insertTrace(""); }
			return res;
		}
		public long UpdateIdentityIQInfo(IdentityIQInfo IQInfo, string from = "")
		{
			try
			{
					if (from == "insert")
					{
						sql = "Update IdentityIqInformation set Question=@Question,Answer=@Answer,UserName=@UserName,Password=@Password where ClientId=@ClientId";
					}
					else
					{
						sql = "Update IdentityIqInformation set ClientId=@ClientId,Question=@Question,Answer=@Answer,UserName=@UserName,Password=@Password where IdentityIqId=@IdentityIqId";
					}
					SqlCommand sqlCmd = new SqlCommand();
					if (from == "")
					{
						sqlCmd.Parameters.AddWithValue("@ClientId", IQInfo.ClientId);
					}
					sqlCmd.Parameters.AddWithValue("@Question", string.IsNullOrEmpty(IQInfo.Question) ? "" : IQInfo.Question);
					sqlCmd.Parameters.AddWithValue("@Answer", string.IsNullOrEmpty(IQInfo.Answer) ? "" : IQInfo.Answer);
					sqlCmd.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(IQInfo.UserName) ? "" : IQInfo.UserName);
					sqlCmd.Parameters.AddWithValue("@Password", string.IsNullOrEmpty(IQInfo.Password) ? "" : IQInfo.Password);
					if (from == "")
					{
						sqlCmd.Parameters.AddWithValue("@IdentityIqId", IQInfo.IdentityIqId);
					}
					else if (from == "insert")
					{
						sqlCmd.Parameters.AddWithValue("@ClientId", IQInfo.ClientId);
					}
				    sqlCmd.CommandText = sql;
					res = objUti.ExecuteInsertCommand(sqlCmd, false);

			}
			catch (Exception ex) { ex.insertTrace(""); }
			return res;
		}
        public IdentityIQInfo GetIdentityIQInfo(string Id)
        {
            IdentityIQInfo IQInfoList = new IdentityIQInfo();
            DataTable dt = null;
            DataTable dt1 = null;
            try
            {
                sql = "select IdentityIqId,ClientId,Question,Answer ,UserName ,Password from IdentityIqInformation where ClientId=" + Id;
                dt1 = objUti.GetDataTable(sql);
                if (dt1.Rows.Count > 0)
                {                    sql = "";                    sql = "select * from Client where ClientId=" + Id;                    dt = objUti.GetDataTable(sql);                    foreach (DataRow ddr in dt1.Rows)
                    {                        IQInfoList.UserName = ddr["UserName"].ToString();                        string pass = ddr["Password"].ToString();                        IQInfoList.Password = objCommon.Decrypt(pass);                        IQInfoList.IdentityIqId = Convert.ToInt64(ddr["IdentityIqId"].ToString());                        IQInfoList.ClientId = Convert.ToInt64(ddr["ClientId"].ToString());                        IQInfoList.Question = ddr["Question"].ToString();                        IQInfoList.Answer = ddr["Answer"].ToString();                    }
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return IQInfoList;
        }

        public bool GetIdentityIQInfo(long? id)
		{
			bool status = false;

			try
			{
				string sql = "select * from IdentityIqInformation where ClientId=" + id;
				DataTable dataTable = new DataTable();
				dataTable = objUti.GetDataTable(sql, true);
				if (dataTable.Rows.Count > 0)
				{
					status = true;
				}
			}
			catch (Exception ex)
			{
				ex.insertTrace("");
			}
			return status;
		}
	}
}
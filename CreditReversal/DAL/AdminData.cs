using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using CreditReversal.Models;
using CreditReversal.Utilities;
using CreditReversal.BLL;

namespace CreditReversal.DAL
{

	public class AdminData
    {
        private RegistrationFunctions functions = new RegistrationFunctions();
        Common objCommon = new Common();
		DBUtilities objUtilities = new DBUtilities();
		string strSql = string.Empty;
		int res = 0;

		#region Insert CompanyType
		public int InsertCompanTYpe(CompanyTypes objCTypes)
		{
			try
			{
				strSql = "Insert into CompanyTypes(CompanyType,Status,IsIndividual)values(@CompanyType,@Status,0)";
				SqlCommand cmd = new SqlCommand();
				cmd.Parameters.AddWithValue("@CompanyType", objCTypes.CompanyType);
				cmd.Parameters.AddWithValue("@Status", objCTypes.Status);
				cmd.CommandText = strSql;
				res = Convert.ToInt32(objUtilities.ExecuteInsertCommand(cmd, true));
			}
			catch (Exception ex) { ex.insertTrace(""); }
			return res;
		}
		#endregion

		#region Get CompanyType
		public List<CompanyTypes> GetCompanyType()
		{
			List<CompanyTypes> objCTypeList = new List<CompanyTypes>();
			try
			{
				strSql = "Select CompanyTypeId,CompanyType,IsIndividual,(Case when Status=0 then 'InActive' else 'Active' end)as CStatus from CompanyTypes";
				DataTable dt = objUtilities.GetDataTable(strSql);
				if (dt.Rows.Count > 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						CompanyTypes objCTypes = new CompanyTypes();
						objCTypes.CompanyTypeId = Convert.ToInt32(dr["CompanyTypeId"].ToString());
						objCTypes.CompanyType = dr["CompanyType"].ToString();
						objCTypes.Status = dr["CStatus"].ToString();
						objCTypes.IsIndividual = bool.Parse(dr["IsIndividual"].ToString());
						objCTypeList.Add(objCTypes);
					}

				}
			}
			catch (Exception ex) { ex.insertTrace(""); }
			return objCTypeList;
		}
		#endregion

		#region Check CompanyType
		public bool CheckCompanyType(string CompanyType)
		{
			string chkCType = string.Empty;
			bool result = false;
			try
			{
				strSql = "Select CompanyType from CompanyTypes where CompanyType='" + CompanyType + "'";
				chkCType = Convert.ToString(objUtilities.ExecuteScalar(strSql, true));
				if (chkCType != "")
				{
					result = true;
				}
			}
			catch (Exception ex) { ex.insertTrace(""); }
			return result;
		}
		#endregion

		#region Edit Company Type
		public List<CompanyTypes> GetCompanyTypeEditById(string CTId)
		{

			List<CompanyTypes> objCTypesList = new List<CompanyTypes>();
			try
			{
				strSql = "Select CompanyTypeId,CompanyType,Status from CompanyTypes where CompanyTypeId=" + CTId;
				DataTable dt = objUtilities.GetDataTable(strSql, true);
				if (dt.Rows.Count > 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						CompanyTypes objCTypes = new CompanyTypes();
						objCTypes.CompanyTypeId = Convert.ToInt32(dr["CompanyTypeId"].ToString());
						objCTypes.CompanyType = dr["CompanyType"].ToString();
						objCTypes.Status = dr["Status"].ToString();
						objCTypesList.Add(objCTypes);
					}
				}
			}
			catch (Exception ex) { ex.insertTrace(""); }
			return objCTypesList;
		}
		#endregion

		#region Update CompanyType
		public int UpdateCompanyType(CompanyTypes objCTypes)
		{
			try
			{

				strSql = "Update CompanyTypes set CompanyType=@CompanyType,Status=@Status where  CompanyTypeId=@CompanyTypeId";
				SqlCommand cmd = new SqlCommand();
				cmd.Parameters.AddWithValue("@CompanyTypeId", objCTypes.CompanyTypeId);
				cmd.Parameters.AddWithValue("@CompanyType", objCTypes.CompanyType);
				cmd.Parameters.AddWithValue("@Status", objCTypes.Status);
				cmd.CommandText = strSql;
				res = Convert.ToInt32(objUtilities.ExecuteInsertCommand(cmd, true));

			}
			catch (Exception ex) { ex.insertTrace(""); }
			return res;
		}
        #endregion

        #region Delete CompanyType
        public int DeleteCompanyType(string CompanyTypeId)
        {
            try
            {
                strSql = "Delete from CompanyTypes where  CompanyTypeId=" + CompanyTypeId;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = strSql;
                res = Convert.ToInt32(objUtilities.ExecuteInsertCommand(cmd, true));
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }


        #endregion

        public List<Agent> GetBilling()        {            List<Agent> objCTypeList = new List<Agent>();            try            {                strSql = "select c.FirstName +' '+ c.LastName as agentname, b.BillingType, c.PricingPlan, CONVERT(VARCHAR(10),  a.PaymentDate, 101)  AS FPaymentDate, a.* from Billing a ";                strSql += "inner join AgentBilling b on a.AgentId = b.AgentId and a.PaymentMethodId = b.AgentBillingId ";                strSql += "inner join Agent c on a.AgentId = c.AgentId";                DataTable dt = objUtilities.GetDataTable(strSql);                if (dt.Rows.Count > 0)                {                    foreach (DataRow dr in dt.Rows)                    {                        Agent objCTypes = new Agent();                        objCTypes.FirstName = dr["agentname"].ToString();                        objCTypes.BillingType = dr["BillingType"].ToString();                        objCTypes.AgentBillingId = dr["BillingId"].ToString();                        objCTypes.AgentId = Convert.ToInt32(dr["AgentId"]);                        objCTypes.TransactionNo = dr["TransactionNo"].ToString();                        objCTypes.CardType = dr["CardType"].ToString();                        objCTypes.PaymentMethodId = dr["PaymentMethodId"].ToString();                        objCTypes.PaymentType = dr["PaymentType"].ToString();                        objCTypes.PaymentStatus = dr["PaymentStatus"].ToString();                        objCTypes.PaymentDate = dr["FPaymentDate"].ToString();                        objCTypes.pricing = functions.GetPricingById(Convert.ToInt32(dr["PricingPlan"]));                        objCTypeList.Add(objCTypes);                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }            return objCTypeList;        }
    }
}
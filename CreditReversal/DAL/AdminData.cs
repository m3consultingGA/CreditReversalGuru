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


        #region Insert CompanyType
        public int InsertServiceSettings(ServiceSettings servicesettings)
        {
            try
            {
                strSql = "select * from ServiceSettings";
                DataTable dt = objUtilities.GetDataTable(strSql);
                if (dt.Rows.Count > 0)                {
                    strSql = "Update ServiceSettings set SendGridAPIKey=@SendGridAPIKey,Fromemail=@Fromemail,AuthApiLoginId=@AuthApiLoginId,ApiTransactionKey=@ApiTransactionKey,SecretKey=@SecretKey,AuthEnvironment=@AuthEnvironment,[14DaysMailLine1]=@Days14MailLine1,[14DaysMailLine2]=@Days14MailLine2, ";
                    strSql += "[7DaysMailLine1]=@Days7MailLine1,[7DaysMailLine2]=@Days7MailLine2,PaymentSuccessLine1=@PaymentSuccessLine1 ,PaymentSuccessLine2=@PaymentSuccessLine2 ,PaymentFailureLine1=@PaymentFailureLine1,PaymentFailureLine2=@PaymentFailureLine2,NextAttemptMailLine1=@NextAttemptMailLine1, ";
                    strSql += "NextAttemptMailLine2=@NextAttemptMailLine2,SecondPaymentFailureLine1=@SecondPaymentFailureLine1,SecondPaymentFailureLine2=@SecondPaymentFailureLine2 ";
                    
                    SqlCommand cmd = new SqlCommand();
                    if (servicesettings.SendGridAPIKey != null)
                    {
                        cmd.Parameters.AddWithValue("@SendGridAPIKey", servicesettings.SendGridAPIKey);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SendGridAPIKey", string.Empty);
                    }
                    if (servicesettings.Fromemail != null)
                    {
                        cmd.Parameters.AddWithValue("@Fromemail", servicesettings.Fromemail);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Fromemail", string.Empty);
                    }
                    if (servicesettings.AuthApiLoginId != null)
                    {
                        cmd.Parameters.AddWithValue("@AuthApiLoginId", servicesettings.AuthApiLoginId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@AuthApiLoginId", string.Empty);
                    }
                    if (servicesettings.ApiTransactionKey != null)
                    {
                        cmd.Parameters.AddWithValue("@ApiTransactionKey", servicesettings.ApiTransactionKey);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ApiTransactionKey", string.Empty);
                    }
                    if (servicesettings.SecretKey != null)
                    {
                        cmd.Parameters.AddWithValue("@SecretKey", servicesettings.SecretKey);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SecretKey", string.Empty);
                    }
                    if (servicesettings.AuthEnvironment != null)
                    {
                        cmd.Parameters.AddWithValue("@AuthEnvironment", servicesettings.AuthEnvironment);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@AuthEnvironment", string.Empty);
                    }
                    if (servicesettings.Days14MailLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@Days14MailLine1", servicesettings.Days14MailLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Days14MailLine1", string.Empty);
                    }
                    if (servicesettings.Days14MailLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@Days14MailLine2", servicesettings.Days14MailLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Days14MailLine2", string.Empty);
                    }
                    
                    if (servicesettings.Days7MailLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@Days7MailLine1", servicesettings.Days7MailLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Days7MailLine1", string.Empty);
                    }
                    if (servicesettings.Days7MailLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@Days7MailLine2", servicesettings.Days7MailLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Days7MailLine2", string.Empty);
                    }
                    if (servicesettings.PaymentSuccessLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@PaymentSuccessLine1", servicesettings.PaymentSuccessLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PaymentSuccessLine1", string.Empty);
                    }

                    if (servicesettings.PaymentSuccessLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@PaymentSuccessLine2", servicesettings.PaymentSuccessLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PaymentSuccessLine2", string.Empty);
                    }
                    if (servicesettings.PaymentFailureLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@PaymentFailureLine1", servicesettings.PaymentFailureLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PaymentFailureLine1", string.Empty);
                    }
                    if (servicesettings.PaymentFailureLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@PaymentFailureLine2", servicesettings.PaymentFailureLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PaymentFailureLine2", string.Empty);
                    }
                    if (servicesettings.NextAttemptMailLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@NextAttemptMailLine1", servicesettings.NextAttemptMailLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NextAttemptMailLine1", string.Empty);
                    }
                    
                    if (servicesettings.NextAttemptMailLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@NextAttemptMailLine2", servicesettings.NextAttemptMailLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NextAttemptMailLine2", string.Empty);
                    }
                    if (servicesettings.SecondPaymentFailureLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@SecondPaymentFailureLine1", servicesettings.SecondPaymentFailureLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SecondPaymentFailureLine1", string.Empty);
                    }
                    if (servicesettings.SecondPaymentFailureLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@SecondPaymentFailureLine2", servicesettings.SecondPaymentFailureLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SecondPaymentFailureLine2", string.Empty);
                    }

                    
                    cmd.CommandText = strSql;
                    res = Convert.ToInt32(objUtilities.ExecuteInsertCommand(cmd, true));
                }
                else {
                    strSql = "Insert into ServiceSettings(SendGridAPIKey,Fromemail,AuthApiLoginId,ApiTransactionKey,SecretKey,AuthEnvironment,[14DaysMailLine1],[14DaysMailLine2], ";
                    strSql += "[7DaysMailLine1],[7DaysMailLine2],PaymentSuccessLine1 ,PaymentSuccessLine2 ,PaymentFailureLine1,PaymentFailureLine2,NextAttemptMailLine1, ";
                    strSql += "NextAttemptMailLine2,SecondPaymentFailureLine1,SecondPaymentFailureLine2) ";
                    strSql += "values(@SendGridAPIKey,@Fromemail,@AuthApiLoginId,@ApiTransactionKey,@SecretKey,@AuthEnvironment,@Days14MailLine1,@Days14MailLine2,";
                    strSql += "@Days7MailLine1,@Days7MailLine2,@PaymentSuccessLine1 ,@PaymentSuccessLine2 ,@PaymentFailureLine1,@PaymentFailureLine2,@NextAttemptMailLine1,";
                    strSql += "@NextAttemptMailLine2,@SecondPaymentFailureLine1,@SecondPaymentFailureLine2)";
                    SqlCommand cmd = new SqlCommand();
                    if (servicesettings.SendGridAPIKey != null)
                    {
                        cmd.Parameters.AddWithValue("@SendGridAPIKey", servicesettings.SendGridAPIKey);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SendGridAPIKey", string.Empty);
                    }
                    if (servicesettings.Fromemail != null)
                    {
                        cmd.Parameters.AddWithValue("@Fromemail", servicesettings.Fromemail);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Fromemail", string.Empty);
                    }
                    if (servicesettings.AuthApiLoginId != null)
                    {
                        cmd.Parameters.AddWithValue("@AuthApiLoginId", servicesettings.AuthApiLoginId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@AuthApiLoginId", string.Empty);
                    }
                    if (servicesettings.ApiTransactionKey != null)
                    {
                        cmd.Parameters.AddWithValue("@ApiTransactionKey", servicesettings.ApiTransactionKey);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ApiTransactionKey", string.Empty);
                    }
                    if (servicesettings.SecretKey != null)
                    {
                        cmd.Parameters.AddWithValue("@SecretKey", servicesettings.SecretKey);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SecretKey", string.Empty);
                    }
                    if (servicesettings.AuthEnvironment != null)
                    {
                        cmd.Parameters.AddWithValue("@AuthEnvironment", servicesettings.AuthEnvironment);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@AuthEnvironment", string.Empty);
                    }
                    if (servicesettings.Days14MailLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@Days14MailLine1", servicesettings.Days14MailLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Days14MailLine1", string.Empty);
                    }
                    if (servicesettings.Days14MailLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@Days14MailLine2", servicesettings.Days14MailLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Days14MailLine2", string.Empty);
                    }

                    if (servicesettings.Days7MailLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@Days7MailLine1", servicesettings.Days7MailLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Days7MailLine1", string.Empty);
                    }
                    if (servicesettings.Days7MailLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@Days7MailLine2", servicesettings.Days7MailLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Days7MailLine2", string.Empty);
                    }
                    if (servicesettings.PaymentSuccessLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@PaymentSuccessLine1", servicesettings.PaymentSuccessLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PaymentSuccessLine1", string.Empty);
                    }

                    if (servicesettings.PaymentSuccessLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@PaymentSuccessLine2", servicesettings.PaymentSuccessLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PaymentSuccessLine2", string.Empty);
                    }
                    if (servicesettings.PaymentFailureLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@PaymentFailureLine1", servicesettings.PaymentFailureLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PaymentFailureLine1", string.Empty);
                    }
                    if (servicesettings.PaymentFailureLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@PaymentFailureLine2", servicesettings.PaymentFailureLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PaymentFailureLine2", string.Empty);
                    }
                    if (servicesettings.NextAttemptMailLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@NextAttemptMailLine1", servicesettings.NextAttemptMailLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NextAttemptMailLine1", string.Empty);
                    }

                    if (servicesettings.NextAttemptMailLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@NextAttemptMailLine2", servicesettings.NextAttemptMailLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NextAttemptMailLine2", string.Empty);
                    }
                    if (servicesettings.SecondPaymentFailureLine1 != null)
                    {
                        cmd.Parameters.AddWithValue("@SecondPaymentFailureLine1", servicesettings.SecondPaymentFailureLine1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SecondPaymentFailureLine1", string.Empty);
                    }
                    if (servicesettings.SecondPaymentFailureLine2 != null)
                    {
                        cmd.Parameters.AddWithValue("@SecondPaymentFailureLine2", servicesettings.SecondPaymentFailureLine2);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SecondPaymentFailureLine2", string.Empty);
                    }
                    cmd.CommandText = strSql;
                    res = Convert.ToInt32(objUtilities.ExecuteInsertCommand(cmd, true));
                }
                
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        #endregion


        public ServiceSettings GetServiceSettings()        {            ServiceSettings ServiceSettings = new ServiceSettings();            try            {                strSql = "select SendGridAPIKey,Fromemail,AuthApiLoginId,ApiTransactionKey,SecretKey,AuthEnvironment,[14DaysMailLine1],[14DaysMailLine2], ";
                strSql += "[7DaysMailLine1],[7DaysMailLine2],PaymentSuccessLine1 ,PaymentSuccessLine2 ,PaymentFailureLine1,PaymentFailureLine2,NextAttemptMailLine1, ";
                strSql += "NextAttemptMailLine2,SecondPaymentFailureLine1,SecondPaymentFailureLine2 ";
                strSql += " from ServiceSettings";                               DataTable dt = objUtilities.GetDataTable(strSql);                if (dt.Rows.Count > 0)                {                    foreach (DataRow dr in dt.Rows)                    {                                                ServiceSettings.SendGridAPIKey = dr["SendGridAPIKey"].ToString();                        ServiceSettings.Fromemail = dr["Fromemail"].ToString();                        ServiceSettings.AuthApiLoginId = dr["AuthApiLoginId"].ToString();                        ServiceSettings.ApiTransactionKey = dr["ApiTransactionKey"].ToString();

                        ServiceSettings.SecretKey = dr["SecretKey"].ToString();                        ServiceSettings.AuthEnvironment = dr["AuthEnvironment"].ToString();                        ServiceSettings.Days14MailLine1 = dr["14DaysMailLine1"].ToString();                        ServiceSettings.Days14MailLine2 = dr["14DaysMailLine2"].ToString();                        ServiceSettings.Days7MailLine1 = dr["7DaysMailLine1"].ToString();                        ServiceSettings.Days7MailLine2 = dr["7DaysMailLine2"].ToString();                        ServiceSettings.PaymentSuccessLine1 = dr["PaymentSuccessLine1"].ToString();
                        ServiceSettings.PaymentSuccessLine2 = dr["PaymentSuccessLine2"].ToString();
                        ServiceSettings.PaymentFailureLine1 = dr["PaymentFailureLine1"].ToString();
                        ServiceSettings.PaymentFailureLine2 = dr["PaymentFailureLine2"].ToString();
                        ServiceSettings.NextAttemptMailLine1 = dr["NextAttemptMailLine1"].ToString();
                        ServiceSettings.NextAttemptMailLine2 = dr["NextAttemptMailLine2"].ToString();
                        ServiceSettings.SecondPaymentFailureLine1 = dr["SecondPaymentFailureLine1"].ToString();
                        ServiceSettings.SecondPaymentFailureLine2 = dr["SecondPaymentFailureLine2"].ToString();

                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }            return ServiceSettings;        }
    }
}
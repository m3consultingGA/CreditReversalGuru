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
using CreditReversal.DAL;

namespace CreditReversal.DAL
{

	public class AdminData
    {
        private RegistrationFunctions functions = new RegistrationFunctions();
        Common objCommon = new Common();
		DBUtilities objUtilities = new DBUtilities();
		string strSql = string.Empty;
		int res = 0;
        public SessionData sessionData = new SessionData();
        
        Common common = new Common();        
        string nl = '\n'.ToString();
       

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
                    strSql += "NextAttemptMailLine2=@NextAttemptMailLine2,SecondPaymentFailureLine1=@SecondPaymentFailureLine1,SecondPaymentFailureLine2=@SecondPaymentFailureLine2,ChallengesPath=@ChallengesPath, ";
                    strSql += "ChallengesPathResult=@ChallengesPathResult,MailXStreamURL=@MailXStreamURL,MXUserid=@MXUserid,MXPassword=@MXPassword,MXTemplate=@MXTemplate ";
                    
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
                   
                    if (servicesettings.ChallengesPath != null)
                    {
                        cmd.Parameters.AddWithValue("@ChallengesPath", servicesettings.ChallengesPath);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ChallengesPath", string.Empty);
                    }

                    if (servicesettings.ChallengesPathResult != null)
                    {
                        cmd.Parameters.AddWithValue("@ChallengesPathResult", servicesettings.ChallengesPathResult);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ChallengesPathResult", string.Empty);
                    }


                    if (servicesettings.MailXStreamURL != null)
                    {
                        cmd.Parameters.AddWithValue("@MailXStreamURL", servicesettings.MailXStreamURL);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MailXStreamURL", string.Empty);
                    }

                    if (servicesettings.MXUserid != null)
                    {
                        cmd.Parameters.AddWithValue("@MXUserid", servicesettings.MXUserid);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MXUserid", string.Empty);
                    }

                    if (servicesettings.MXPassword != null)
                    {
                        cmd.Parameters.AddWithValue("@MXPassword", servicesettings.MXPassword);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MXPassword", string.Empty);
                    }

                    if (servicesettings.MXTemplate != null)
                    {
                        cmd.Parameters.AddWithValue("@MXTemplate", servicesettings.MXTemplate);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MXTemplate", string.Empty);
                    }

                    cmd.CommandText = strSql;
                    res = Convert.ToInt32(objUtilities.ExecuteInsertCommand(cmd, true));
                }
                else {
                    strSql = "Insert into ServiceSettings(SendGridAPIKey,Fromemail,AuthApiLoginId,ApiTransactionKey,SecretKey,AuthEnvironment,[14DaysMailLine1],[14DaysMailLine2], ";
                    strSql += "[7DaysMailLine1],[7DaysMailLine2],PaymentSuccessLine1 ,PaymentSuccessLine2 ,PaymentFailureLine1,PaymentFailureLine2,NextAttemptMailLine1, ";
                    strSql += "NextAttemptMailLine2,SecondPaymentFailureLine1,SecondPaymentFailureLine2,ChallengesPath,ChallengesPathResult,MailXStreamURL,MXUserid,MXPassword,MXTemplate) ";
                    strSql += "values(@SendGridAPIKey,@Fromemail,@AuthApiLoginId,@ApiTransactionKey,@SecretKey,@AuthEnvironment,@Days14MailLine1,@Days14MailLine2,";
                    strSql += "@Days7MailLine1,@Days7MailLine2,@PaymentSuccessLine1 ,@PaymentSuccessLine2 ,@PaymentFailureLine1,@PaymentFailureLine2,@NextAttemptMailLine1,";
                    strSql += "@NextAttemptMailLine2,@SecondPaymentFailureLine1,@SecondPaymentFailureLine2,@ChallengesPath,@ChallengesPathResult,@MailXStreamURL,@MXUserid,@MXPassword,@MXTemplate)";
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
                                       

                    if (servicesettings.ChallengesPath != null)
                    {
                        cmd.Parameters.AddWithValue("@ChallengesPath", servicesettings.ChallengesPath);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ChallengesPath", string.Empty);
                    }

                    if (servicesettings.ChallengesPathResult != null)
                    {
                        cmd.Parameters.AddWithValue("@ChallengesPathResult", servicesettings.ChallengesPathResult);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ChallengesPathResult", string.Empty);
                    }


                    if (servicesettings.MailXStreamURL != null)
                    {
                        cmd.Parameters.AddWithValue("@MailXStreamURL", servicesettings.MailXStreamURL);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MailXStreamURL", string.Empty);
                    }

                    if (servicesettings.MXUserid != null)
                    {
                        cmd.Parameters.AddWithValue("@MXUserid", servicesettings.MXUserid);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MXUserid", string.Empty);
                    }

                    if (servicesettings.MXPassword != null)
                    {
                        cmd.Parameters.AddWithValue("@MXPassword", servicesettings.MXPassword);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MXPassword", string.Empty);
                    }

                    if (servicesettings.MXTemplate != null)
                    {
                        cmd.Parameters.AddWithValue("@MXTemplate", servicesettings.MXTemplate);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MXTemplate", string.Empty);
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
                strSql += "NextAttemptMailLine2,SecondPaymentFailureLine1,SecondPaymentFailureLine2,ChallengesPath,ChallengesPathResult,MailXStreamURL,MXUserid,MXPassword,MXTemplate ";
                strSql += " from ServiceSettings";                               DataTable dt = objUtilities.GetDataTable(strSql);                if (dt.Rows.Count > 0)                {                    foreach (DataRow dr in dt.Rows)                    {                                                ServiceSettings.SendGridAPIKey = dr["SendGridAPIKey"].ToString();                        ServiceSettings.Fromemail = dr["Fromemail"].ToString();                        ServiceSettings.AuthApiLoginId = dr["AuthApiLoginId"].ToString();                        ServiceSettings.ApiTransactionKey = dr["ApiTransactionKey"].ToString();

                        ServiceSettings.SecretKey = dr["SecretKey"].ToString();                        ServiceSettings.AuthEnvironment = dr["AuthEnvironment"].ToString();                        ServiceSettings.Days14MailLine1 = dr["14DaysMailLine1"].ToString();                        ServiceSettings.Days14MailLine2 = dr["14DaysMailLine2"].ToString();                        ServiceSettings.Days7MailLine1 = dr["7DaysMailLine1"].ToString();                        ServiceSettings.Days7MailLine2 = dr["7DaysMailLine2"].ToString();                        ServiceSettings.PaymentSuccessLine1 = dr["PaymentSuccessLine1"].ToString();
                        ServiceSettings.PaymentSuccessLine2 = dr["PaymentSuccessLine2"].ToString();
                        ServiceSettings.PaymentFailureLine1 = dr["PaymentFailureLine1"].ToString();
                        ServiceSettings.PaymentFailureLine2 = dr["PaymentFailureLine2"].ToString();
                        ServiceSettings.NextAttemptMailLine1 = dr["NextAttemptMailLine1"].ToString();
                        ServiceSettings.NextAttemptMailLine2 = dr["NextAttemptMailLine2"].ToString();
                        ServiceSettings.SecondPaymentFailureLine1 = dr["SecondPaymentFailureLine1"].ToString();
                        ServiceSettings.SecondPaymentFailureLine2 = dr["SecondPaymentFailureLine2"].ToString();
                       
                        ServiceSettings.ChallengesPath = dr["ChallengesPath"].ToString();
                        ServiceSettings.ChallengesPathResult = dr["ChallengesPathResult"].ToString();
                        ServiceSettings.MailXStreamURL = dr["MailXStreamURL"].ToString();
                        ServiceSettings.MXUserid = dr["MXUserid"].ToString();
                        ServiceSettings.MXPassword = dr["MXPassword"].ToString();
                        ServiceSettings.MXTemplate = dr["MXTemplate"].ToString();
                       
                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }            return ServiceSettings;        }


        //Get Investor
        public Investor GetInvestor(string InvestorId)
        {
            Investor res = new Investor();
            try
            {
                strSql = "SELECT c.FirstName,c.MiddleName,c.LastName,Convert(varchar(15),c.DOB,101)as DOB,c.Address1,c.Address2,c.City,c.State,c.ZipCode,u.Password,c.SSN,c.CurrentEmail,c.CurrentPhone,c.DrivingLicense,c.SocialSecCard,c.ProofOfCard,u.UserName as uname,u.Password as passwd FROM Investor c " + nl;
                strSql += "JOIN Users u ON c.InvestorId = u.AgentClientId " + nl;
                strSql += "WHERE c.InvestorId= '" + InvestorId + "' and u.UserRole='investor'";
                DataTable dt = objUtilities.GetDataTable(strSql, true);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    res.FirstName = string.IsNullOrEmpty(row["FirstName"].ConvertObjectToStringIfNotNull()) ? "" : row["FirstName"].ConvertObjectToStringIfNotNull();
                    res.MiddleName = string.IsNullOrEmpty(row["MiddleName"].ConvertObjectToStringIfNotNull()) ? "" : row["MiddleName"].ConvertObjectToStringIfNotNull();
                    res.LastName = row["LastName"].ConvertObjectToStringIfNotNull();
                    res.FullName = res.FirstName + " " + res.LastName;
                    res.DOB = row["DOB"].ConvertObjectToStringIfNotNull();

                    res.Address1 = row["Address1"].ConvertObjectToStringIfNotNull();
                    res.Address2 = row["Address2"].ConvertObjectToStringIfNotNull();
                    res.City = row["City"].ConvertObjectToStringIfNotNull();
                    res.State = row["State"].ConvertObjectToStringIfNotNull();
                    res.ZipCode = row["ZipCode"].ConvertObjectToStringIfNotNull();

                    res.SSN = common.Decrypt(row["SSN"].ConvertObjectToStringIfNotNull());
                    res.CurrentEmail = row["CurrentEmail"].ConvertObjectToStringIfNotNull();
                    res.CurrentPhone = row["CurrentPhone"].ConvertObjectToStringIfNotNull();
                    res.sDrivingLicense = row["DrivingLicense"].ConvertObjectToStringIfNotNull();
                    res.sSocialSecCard = row["SocialSecCard"].ConvertObjectToStringIfNotNull();
                    res.sProofOfCard = row["ProofOfCard"].ConvertObjectToStringIfNotNull();
                    res.Password = common.Decrypt(row["Password"].ConvertObjectToStringIfNotNull());

                 

                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return res;
        }

        //get investors
        public List<Investor> GetInvestors()
        {
            List<Investor> Investor = new List<Investor>();
            try
            {

                strSql = "select * from Investor";

                DataTable dt = objUtilities.GetDataTable(strSql, true);

                foreach (DataRow row in dt.Rows)
                {
                    Investor Inv = new Investor();
                    Inv.InvestorId = row["InvestorId"].ConvertObjectToIntIfNotNull();
                    Inv.FirstName = row["FirstName"].ConvertObjectToStringIfNotNull();
                    Inv.MiddleName = row["MiddleName"].ConvertObjectToStringIfNotNull();
                    Inv.LastName = row["LastName"].ConvertObjectToStringIfNotNull();
                    Inv.DOB = row["DOB"].ConvertObjectToStringIfNotNull();
                    Inv.SSN = row["SSN"].ConvertObjectToStringIfNotNull();
                    Inv.CurrentEmail = row["CurrentEmail"].ConvertObjectToStringIfNotNull();
                    Inv.CurrentPhone = row["CurrentPhone"].ConvertObjectToStringIfNotNull();
                    Inv.sDrivingLicense = row["DrivingLicense"].ConvertObjectToStringIfNotNull();
                    Inv.sSocialSecCard = row["SocialSecCard"].ConvertObjectToStringIfNotNull();
                    Inv.sProofOfCard = row["ProofOfCard"].ConvertObjectToStringIfNotNull();
                    Inv.Status = row["Status"].ConvertObjectToStringIfNotNull();
                    Inv.CreatedBy = row["CreatedBy"].objectToInt(0);
                    Inv.CreatedDate = row["CreatedDate"].ConvertObjectToStringIfNotNull();

                    Investor.Add(Inv);
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Investor;
        }



        public long DeleteInvestor(string InvestorId)
        {
            long res = 0;
            try
            {
                if (InvestorId != "" || InvestorId != null)
                {
                    strSql = "Delete from Investor where InvestorId='" + InvestorId + "'";
                    var cmd1 = new SqlCommand();
                    cmd1.CommandText = strSql;
                    res = objUtilities.ExecuteInsertCommand(cmd1, true);
                    if (res > 0)
                    {
                        strSql = "Delete from Users where AgentClientId='" + InvestorId + "'";
                        var cmd = new SqlCommand();
                        cmd.CommandText = strSql;
                        res = objUtilities.ExecuteInsertCommand(cmd, true);
                    }

                    return res;
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }


        //Create investor
        public long CreateInvestor(Investor InvestorModel)
        {
            long res = 0;
            string encryPassw = "";
            try
            {

                int? id = InvestorModel.InvestorId;
                DataTable dt = new DataTable();
                if (id != null)
                {
                    strSql = "SELECT * from Investor where InvestorId='" + InvestorModel.InvestorId + "'";
                    dt = objUtilities.GetDataTable(strSql);
                }

                InvestorModel.Password = Common.CreateRandomPassword();
                string encrySSN = common.Encrypt(InvestorModel.SSN);

                string encryptidpassword = string.IsNullOrEmpty(InvestorModel.IdPassword) ? "" : common.Encrypt(InvestorModel.IdPassword);
                InvestorModel.IdPassword = encryptidpassword;
                if (InvestorModel.Password != null)
                {
                    encryPassw = common.Encrypt(InvestorModel.Password);
                }

                if (dt.Rows.Count > 0)
                {
                    strSql = "Update Investor" + nl;
                    strSql += "set FirstName=@FirstName,MiddleName=@MiddleName,LastName=@LastName,DOB=@DOB,Address1=@Address1,Address2=@Address2,City=@City,State=@State,ZipCode=@ZipCode,SSN=@SSN,CurrentEmail=@CurrentEmail,CurrentPhone=@CurrentPhone,DrivingLicense=@DrivingLicense," + nl; ;
                    strSql += "SocialSecCard=@SocialSecCard,ProofOfCard=@ProofOfCard,Status=@Status,CreatedBy=@CreatedBy,CreatedDate=@CreatedDate where InvestorId=@InvestorId" + nl;
                    var cmd = new SqlCommand();
                    cmd.Parameters.AddWithValue("@InvestorId", InvestorModel.InvestorId);
                    cmd.Parameters.AddWithValue("@FirstName", InvestorModel.FirstName);
                    if (InvestorModel.MiddleName != null)
                    {
                        cmd.Parameters.AddWithValue("@MiddleName", InvestorModel.MiddleName);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MiddleName", string.Empty);
                    }

                    if (InvestorModel.CurrentPhone != null)
                    {
                        cmd.Parameters.AddWithValue("@CurrentPhone", InvestorModel.CurrentPhone);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CurrentPhone", string.Empty);
                    }

                    if (InvestorModel.DOB == "01/01/1900" || InvestorModel.DOB == "" || InvestorModel.DOB == null)
                    {
                        cmd.Parameters.Add("@DOB", SqlDbType.Date).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@DOB", InvestorModel.DOB);
                    }

                    cmd.Parameters.AddWithValue("@Address1", string.IsNullOrEmpty(InvestorModel.Address1) ? "" : InvestorModel.Address1);
                    cmd.Parameters.AddWithValue("@Address2", string.IsNullOrEmpty(InvestorModel.Address2) ? "" : InvestorModel.Address2);
                    cmd.Parameters.AddWithValue("@City", string.IsNullOrEmpty(InvestorModel.City) ? "" : InvestorModel.City);
                    cmd.Parameters.AddWithValue("@State", string.IsNullOrEmpty(InvestorModel.State) ? "" : InvestorModel.State);
                    cmd.Parameters.AddWithValue("@ZipCode", string.IsNullOrEmpty(InvestorModel.ZipCode) ? "" : InvestorModel.ZipCode);

                    if (InvestorModel.sDrivingLicense == null)
                    {
                        cmd.Parameters.AddWithValue("@DrivingLicense", string.Empty);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@DrivingLicense", InvestorModel.sDrivingLicense);
                    }
                    if (InvestorModel.sSocialSecCard == null)
                    {
                        cmd.Parameters.AddWithValue("@SocialSecCard", string.Empty);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SocialSecCard", InvestorModel.sSocialSecCard);
                    }
                    if (InvestorModel.sProofOfCard == null)
                    {
                        cmd.Parameters.AddWithValue("@ProofOfCard", string.Empty);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ProofOfCard", InvestorModel.sProofOfCard);
                    }

                    cmd.Parameters.AddWithValue("@LastName", InvestorModel.LastName);
                    cmd.Parameters.AddWithValue("@SSN", encrySSN);
                    cmd.Parameters.AddWithValue("@CurrentEmail", InvestorModel.CurrentEmail);
                    cmd.Parameters.AddWithValue("@Status", InvestorModel.Status);
                    cmd.Parameters.AddWithValue("@CreatedBy", InvestorModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedDate", InvestorModel.CreatedDate);
                    cmd.CommandText = strSql;
                    res = objUtilities.ExecuteInsertCommand(cmd, true);

                    if (res > 0)
                    {
                        strSql = "Update Users" + nl;
                        strSql += "set EmailAddress=@EmailAddress," + nl;
                        strSql += "UserRole=@UserRole,Status=@Status,CreatedBy=@CreatedBy,CreatedDate=@CreatedDate where UserRole=@UserRole and AgentClientId=@AgentClientId" + nl;
                        cmd = new SqlCommand();
                        cmd.Parameters.AddWithValue("@AgentClientId", InvestorModel.InvestorId);
                        cmd.Parameters.AddWithValue("@EmailAddress", InvestorModel.CurrentEmail);
                        cmd.Parameters.AddWithValue("@UserRole", InvestorModel.UserRole);
                        cmd.Parameters.AddWithValue("@Status", InvestorModel.Status);
                        cmd.Parameters.AddWithValue("@CreatedBy", InvestorModel.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedDate", InvestorModel.CreatedDate);
                        cmd.CommandText = strSql;
                        res = objUtilities.ExecuteInsertCommand(cmd, true);
                        res = (long)InvestorModel.InvestorId;
                    }
                    return res;
                }
                else
                {
                    if (string.IsNullOrEmpty(InvestorModel.DOB))
                    {
                        InvestorModel.DOB = null;
                    }

                    strSql = "Insert into Investor" + nl;
                    strSql += "(FirstName,MiddleName,LastName,DOB,Address1,Address2,City,State,ZipCode,SSN,CurrentEmail,CurrentPhone,DrivingLicense,SocialSecCard,ProofOfCard,Status,CreatedBy,CreatedDate)" + nl;
                    strSql += "values(@FirstName,@MiddleName,@LastName,@DOB,@Address1,@Address2,@City,@State,@ZipCode,@SSN,@CurrentEmail,@CurrentPhone,@DrivingLicense,@SocialSecCard,@ProofOfCard,@Status,@CreatedBy,@CreatedDate);SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand();

                    cmd.Parameters.AddWithValue("@FirstName", InvestorModel.FirstName);


                    if (InvestorModel.MiddleName != null)
                    {
                        cmd.Parameters.AddWithValue("@MiddleName", InvestorModel.MiddleName);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MiddleName", string.Empty);
                    }

                    if (InvestorModel.CurrentPhone != null)
                    {
                        cmd.Parameters.AddWithValue("@CurrentPhone", InvestorModel.CurrentPhone);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CurrentPhone", string.Empty);
                    }

                    if (InvestorModel.DOB == "01/01/1900" || InvestorModel.DOB == "" || InvestorModel.DOB == null)
                    {
                        cmd.Parameters.Add("@DOB", SqlDbType.Date).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@DOB", InvestorModel.DOB);
                    }

                    cmd.Parameters.AddWithValue("@Address1", string.IsNullOrEmpty(InvestorModel.Address1) ? "" : InvestorModel.Address1);
                    cmd.Parameters.AddWithValue("@Address2", string.IsNullOrEmpty(InvestorModel.Address2) ? "" : InvestorModel.Address2);
                    cmd.Parameters.AddWithValue("@City", string.IsNullOrEmpty(InvestorModel.City) ? "" : InvestorModel.City);
                    cmd.Parameters.AddWithValue("@State", string.IsNullOrEmpty(InvestorModel.State) ? "" : InvestorModel.State);
                    cmd.Parameters.AddWithValue("@ZipCode", string.IsNullOrEmpty(InvestorModel.ZipCode) ? "" : InvestorModel.ZipCode);

                    if (InvestorModel.sDrivingLicense == null)
                    {
                        cmd.Parameters.AddWithValue("@DrivingLicense", string.Empty);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@DrivingLicense", InvestorModel.sDrivingLicense);
                    }
                    if (InvestorModel.sSocialSecCard == null)
                    {
                        cmd.Parameters.AddWithValue("@SocialSecCard", string.Empty);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SocialSecCard", InvestorModel.sSocialSecCard);
                    }
                    if (InvestorModel.sProofOfCard == null)
                    {
                        cmd.Parameters.AddWithValue("@ProofOfCard", string.Empty);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ProofOfCard", InvestorModel.sProofOfCard);
                    }
                    
                    cmd.Parameters.AddWithValue("@LastName", InvestorModel.LastName);
                    cmd.Parameters.AddWithValue("@SSN", encrySSN);
                    cmd.Parameters.AddWithValue("@CurrentEmail", InvestorModel.CurrentEmail);
                    cmd.Parameters.AddWithValue("@Status", InvestorModel.Status);
                    cmd.Parameters.AddWithValue("@CreatedBy", InvestorModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedDate", InvestorModel.CreatedDate);
                    cmd.CommandText = strSql;
                    int modified = Convert.ToInt32(objUtilities.ExecuteScalarCommand(cmd, true));
                    InvestorModel.InvestorId = Convert.ToInt32(modified);
                    if (InvestorModel.InvestorId != 0 && InvestorModel.InvestorId != null)
                    {

                        InvestorModel.sSocialSecCard = InvestorModel.InvestorId + "-" + InvestorModel.sSocialSecCard;
                        InvestorModel.sDrivingLicense = InvestorModel.InvestorId + "-" + InvestorModel.sDrivingLicense;
                        InvestorModel.sProofOfCard = InvestorModel.InvestorId + "-" + InvestorModel.sProofOfCard;
                        strSql = "Update Investor" + nl;
                        strSql += "set DrivingLicense=@DrivingLicense," + nl; ;
                        strSql += "SocialSecCard=@SocialSecCard,ProofOfCard=@ProofOfCard where InvestorId=@InvestorId" + nl;
                        cmd = new SqlCommand();
                        cmd.Parameters.AddWithValue("@InvestorId", InvestorModel.InvestorId);
                        cmd.Parameters.AddWithValue("@SocialSecCard", InvestorModel.sSocialSecCard);
                        cmd.Parameters.AddWithValue("@DrivingLicense", InvestorModel.sDrivingLicense);
                        cmd.Parameters.AddWithValue("@ProofOfCard", InvestorModel.sProofOfCard);
                        cmd.CommandText = strSql;
                        res = objUtilities.ExecuteInsertCommand(cmd, true);


                        if (res > 0)
                        {
                            strSql = "Insert into Users" + nl;
                            strSql += "(UserName,EmailAddress,Password,UserRole,Status,CreatedBy,CreatedDate,AgentClientId)" + nl;
                            strSql += "values(@UserName,@EmailAddress,@Password,@UserRole,@Status,@CreatedBy,@CreatedDate,@AgentClientId)" + nl;
                            cmd = new SqlCommand();
                            cmd.Parameters.AddWithValue("@AgentClientId", InvestorModel.InvestorId);
                            cmd.Parameters.AddWithValue("@UserName", InvestorModel.CurrentEmail);
                            cmd.Parameters.AddWithValue("@EmailAddress", InvestorModel.CurrentEmail);
                            cmd.Parameters.AddWithValue("@Password", encryPassw);
                            cmd.Parameters.AddWithValue("@UserRole", InvestorModel.UserRole);
                            cmd.Parameters.AddWithValue("@Status", InvestorModel.Status);
                            cmd.Parameters.AddWithValue("@CreatedBy", InvestorModel.CreatedBy);
                            cmd.Parameters.AddWithValue("@CreatedDate", InvestorModel.CreatedDate);
                            cmd.CommandText = strSql;
                            res = objUtilities.ExecuteInsertCommand(cmd, true);
                        }
                        if (res > 0)
                        {
                            res = (long)InvestorModel.InvestorId;
                            SessionData sd = new SessionData();
                            AgentData agentData = new AgentData();
                            string AdminEmail= sd.GetEmail();
                            common.SendMail(InvestorModel.CurrentEmail, "Registration successful.", "REGISTRATION", InvestorModel.CurrentEmail, "", "Investor", InvestorModel.Password, InvestorModel.FirstName);
                            common.SendMail(AdminEmail, "Registration successful.", "REGISTRATION", AdminEmail, "", "admin", "admin", InvestorModel.FirstName);
                        }
                    }
                    return res;
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }



        #region Insert AccountType
        public int InsertAccountType(AccountTypes objATypes)
        {
            try
            {
                strSql = "Insert into AccountTypes(AccountType,Status,AccountTypeDetails)values(@AccountType,@Status,@AccountTypeDetails)";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@AccountType", objATypes.AccountType);
                cmd.Parameters.AddWithValue("@Status", objATypes.Status);
                cmd.Parameters.AddWithValue("@AccountTypeDetails", objATypes.AccountTypeDetails);
                cmd.CommandText = strSql;
                res = Convert.ToInt32(objUtilities.ExecuteInsertCommand(cmd, true));
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        #endregion

        #region Edit Account Type
        public List<AccountTypes> GetAccountTypeEditById(string ATId)
        {

            List<AccountTypes> objATypesList = new List<AccountTypes>();
            try
            {
                strSql = "Select AccTypeId,AccountType,AccountTypeDetails,Status from AccountTypes where AccTypeId=" + ATId;
                DataTable dt = objUtilities.GetDataTable(strSql, true);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        AccountTypes ACTypes = new AccountTypes();
                        ACTypes.AccountType = row["AccountType"].ToString();
                        ACTypes.AccTypeId = Convert.ToInt32(row["AccTypeId"]);
                        ACTypes.AccountTypeDetails = row["AccountTypeDetails"].ToString();
                        ACTypes.Status = row["Status"].ToString();
                        objATypesList.Add(ACTypes);
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return objATypesList;
        }
        #endregion

        #region Update AccountType
        public int UpdateAccountType(AccountTypes objATypes)
        {
            try
            {
                strSql = "Update AccountTypes set AccountType=@AccountType,AccountTypeDetails=@AccountTypeDetails where  AccTypeId=@AccTypeId";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@AccTypeId", objATypes.AccTypeId);
                cmd.Parameters.AddWithValue("@AccountType", objATypes.AccountType);
               // cmd.Parameters.AddWithValue("@Status", objATypes.Status);
                cmd.Parameters.AddWithValue("@AccountTypeDetails", objATypes.AccountTypeDetails);
                cmd.CommandText = strSql;
                res = Convert.ToInt32(objUtilities.ExecuteInsertCommand(cmd, true));
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        #endregion

        #region Delete Account Type
        public int DeleteAccountType(string AccTypeId)
        {
            try
            {
                strSql = "Delete from AccountTypes where  AccTypeId=" + AccTypeId;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = strSql;
                res = Convert.ToInt32(objUtilities.ExecuteInsertCommand(cmd, true));
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }


        #endregion

    }
}
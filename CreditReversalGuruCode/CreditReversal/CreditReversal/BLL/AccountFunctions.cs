using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using CreditReversal.DAL;
using CreditReversal.Utilities;
using CreditReversal.Models;


namespace CreditReversal.BLL
{
	public class AccountFunctions
	{
		private Common common = new Common();
		private DBUtilities utilities = new DBUtilities();
		private DataTable dataTable = new DataTable();
		private SessionData sessionData = new SessionData();
        private RegistrationFunctions registrationFunctions = new RegistrationFunctions();
       

        public DataRow Login(string Email = "", string Password = "", int id = 0)
		{
			DataRow row = null;
			string encryptpassword = ""; string sql = string.Empty;
			try
			{
				encryptpassword = common.Encrypt(Password);
				if (id != 0)
				{
					sql = "select u.*,a.FirstName,a.LastName,a.BusinessName,a.TypeOfComp "
						+ " from Users u Inner Join Agent a on u.AgentClientId = a.AgentId where u.AgentClientId =" + id;
				}
				else
				{
					sql = "select * from Users where UserName='" + Email + "' and Password='" + encryptpassword + "'  and status=1";
				}
				dataTable = utilities.GetDataTable(sql);
				if (dataTable.Rows.Count > 0)
				{
					row = dataTable.Rows[0];
				}
			}
			catch (Exception ex) {  ex.insertTrace("");  }
			return row;
		}

		public int AddAgentAdmin(SignUp signUp)
		{
			bool status = false;
			object res = 0;
			bool userstatus = false;
			int id = 0;
			string sql = "";
			string comp = "";

			try
			{
				string firstname = string.Empty, lastname = string.Empty;
				string[] name = signUp.Name.Split(' ');
				if (name.Length > 1)
				{
					firstname = name[0];
					lastname = name[1];
				}
				else
				{
					firstname = name[0];
					lastname = null;
				}
				if (signUp.BusinessName == null)
				{
					comp = "1";
					signUp.BusinessName = firstname + " " + lastname;
					signUp.CompanyType = "I";

				}
				sql = "Insert into Agent(FirstName,LastName,PrimaryBusinessEmail,CreatedDate,BusinessName,TypeOfComp) values('" + firstname
					+ "','" + lastname + "','" + signUp.EmailAddress + "',GETDATE(),'" + signUp.BusinessName + "','" + comp
					+ "' ); Select Scope_Identity()";
				res = utilities.ExecuteScalar(sql, true);
				signUp.AgentClientId = res.ToString();
				userstatus = AddUser(signUp);
				if (userstatus)
				{
					string EncrptID = common.Encrypt(signUp.AgentClientId.ToString());
					String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
					string link = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/") + "Account/CompleteRegistration?id=" + EncrptID;
					bool mailstatus = common.SendMail(signUp.EmailAddress, link, signUp.Name);
					id = Convert.ToInt32(res.ToString());
				}

			}
			catch (Exception ex) {  ex.insertTrace("");  }

			return id;
		}
        public bool AddUser(SignUp signUp, string from = "")        {            bool status = false;            long res = 0;            string encryptpassword = common.Encrypt(signUp.Password);            string sql = "";            try            {                SqlCommand cmd = new SqlCommand();                sql = "Insert into Users(UserName,EmailAddress,Password,UserRole,Status,CreatedDate,AgentClientId)values(@UserName,@EmailAddress,@Password,@UserRole,@Status,GetDate(),@AgentClientId)";                cmd.CommandText = sql;                cmd.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(signUp.UserName) ? signUp.EmailAddress : signUp.UserName);                cmd.Parameters.AddWithValue("@EmailAddress", signUp.EmailAddress);                cmd.Parameters.AddWithValue("@Password", encryptpassword);                cmd.Parameters.AddWithValue("@UserRole", "agentadmin");                if (from == "Admin")                {                    cmd.Parameters.AddWithValue("@Status", "1");                }                else                {                    cmd.Parameters.AddWithValue("@Status", "0");                }                cmd.Parameters.AddWithValue("@AgentClientId", signUp.AgentClientId);                res = utilities.ExecuteInsertCommand(cmd, true);                if (res > 0)                {                    status = true;                }            }            catch (Exception ex) { ex.insertTrace(""); }            return status;        }
        //public bool AddUser(SignUp signUp)
        //{
        //	bool status = false;
        //	long res = 0;
        //	string encryptpassword = common.Encrypt(signUp.Password);
        //	string sql = "";
        //	try
        //	{
        //		SqlCommand cmd = new SqlCommand();
        //		sql = "Insert into Users(UserName,EmailAddress,Password,UserRole,Status,CreatedDate,AgentClientId) values(@UserName,@EmailAddress,@Password,@UserRole,@Status,GetDate(),@AgentClientId)";
        //		cmd.CommandText = sql;
        //		cmd.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(signUp.UserName) ? signUp.EmailAddress : signUp.UserName);
        //		cmd.Parameters.AddWithValue("@EmailAddress", signUp.EmailAddress);
        //		cmd.Parameters.AddWithValue("@Password", encryptpassword);
        //		cmd.Parameters.AddWithValue("@UserRole", "agentadmin");
        //		cmd.Parameters.AddWithValue("@status", 0);
        //		cmd.Parameters.AddWithValue("@AgentClientId", signUp.AgentClientId);
        //		res = utilities.ExecuteInsertCommand(cmd, true);
        //		if (res > 0)
        //		{
        //			status = true;
        //		}

        //	}
        //	catch (Exception ex) {  ex.insertTrace("");  }

        //	return status;
        //}
        public bool CheckUsernameexistorNot(string username = "")
        {
            bool status = false;
            string sql = "";
            try
            {
                if (username != "")
                {
                    sql = "select * from Users where UserName='" + username + "'";
                    DataTable dt = utilities.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }
        public Users EmailExitOrNot(string Email)
		{
			Users user = new Users();
			DataRow row = null;
			try
			{
				string query = "Select * from Users where EmailAddress = '" + Email + "'";

				row = utilities.GetDataRow(query);
				if (row != null)
				{
					user.UserName = row[1].ToString();
					string pass = row[3].ToString();
					if (pass != null)
					{
						user.Password = common.Decrypt(pass);
					}
				}
			}
			catch (Exception ex) {  ex.insertTrace("");  }
			return user;
		}
        public Users getUser(string username)
        {
            Users user = new Users();
            DataRow row = null;
            try
            {
                string query = "Select * from Users where username = '" + username + "'";

                row = utilities.GetDataRow(query);
                if (row != null)
                {
                    user.UserName = row[1].ToString();
                    string pass = row[3].ToString();
                    if (pass != null)
                    {
                        user.Password = common.Decrypt(pass);
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return user;
        }
        public Agent GetAgent(string id, string from = "")        {            Agent agent = new Agent();            string sql = "";            bool loginstatus = sessionData.GetLoginStatus();            DataRow dataRow = null;            try            {                if (from == "")                {                    if (!loginstatus)                    {                        sql = "select Top 1  A.*,AB.*,U.UserName from Agent A left Join AgentBilling AB on A.AgentId=AB.AgentId left Join Users U "                            + " on A.AgentId=U.AgentClientId where u.userrole='agentadmin' and  A.AgentId=" + id;                    }                    else                    {                        sql = "select Top 1  A.*,AB.*,U.UserName from Agent A left Join AgentBilling AB on A.AgentId=AB.AgentId left Join Users U "                            + " on AB.AgentId=U.AgentClientId where   u.userrole='agentadmin' and A.AgentId=" + id + " and isPrimary=1";                    }                }                if (from == "Admin")                {                    sql = "";                    sql = "select Top 1  A.*,AB.*,U.UserName from Agent A left Join AgentBilling AB on A.AgentId=AB.AgentId left Join Users U "                        + " on A.AgentId=U.AgentClientId where u.userrole='agentadmin' and  A.AgentId=" + id;                }                dataTable = utilities.GetDataTable(sql, true);                if (dataTable.Rows.Count > 0)                {                    dataRow = dataTable.Rows[0];                    string expirydate = common.Decrypt(dataRow["ExpiryDate"].ToString());                    string[] date = expirydate.Split('-');                    agent.AgentId = Convert.ToInt32(dataRow["AgentId"].ToString());                    agent.BusinessName = string.IsNullOrEmpty(dataRow["BusinessName"].ToString()) ? "" : dataRow["BusinessName"].ToString();                    agent.UserName = string.IsNullOrEmpty(dataRow["UserName"].ToString()) ? "" : dataRow["UserName"].ToString();                    agent.FirstName = string.IsNullOrEmpty(dataRow["FirstName"].ToString()) ? "" : dataRow["FirstName"].ToString();                    agent.LastName = string.IsNullOrEmpty(dataRow["LastName"].ToString()) ? "" : dataRow["LastName"].ToString();                    agent.MiddleName = string.IsNullOrEmpty(dataRow["MiddleName"].ToString()) ? "" : dataRow["MiddleName"].ToString(); ;                    agent.DBA = string.IsNullOrEmpty(dataRow["DBA"].ToString()) ? "" : dataRow["DBA"].ToString(); ;                    if (from == "")                    {                        int companytypeid = dataRow["TypeOfComp"] == null ? 0 : dataRow["TypeOfComp"].ToString().StringToInt(0);                        if (companytypeid > 0)                        {                            agent.TypeOfComp = registrationFunctions.GetCompanyTypeById(companytypeid);                        }                    }                    else if (from == "Admin")                    {                        agent.TypeOfComp = string.IsNullOrEmpty(dataRow["TypeOfComp"].ToString()) ? "" : dataRow["TypeOfComp"].ToString();                    }


                    agent.StateOfEncorp = string.IsNullOrEmpty(dataRow["StateOfEncorp"].ToString()) ? "" : dataRow["StateOfEncorp"].ToString();                    agent.StateOfIncorporationtext = string.IsNullOrEmpty(dataRow["StateOfIncorporationProof"].ToString()) ? "" : dataRow["StateOfIncorporationProof"].ToString();                    agent.DateOfEncorp = string.IsNullOrEmpty(dataRow["DateOfEncorp"].ToString()) ? "" : dataRow["DateOfEncorp"].ToString();                    agent.FedTaxIdentityNo = string.IsNullOrEmpty(dataRow["FedTaxIdentityNo"].ToString()) ? "" : dataRow["FedTaxIdentityNo"].ToString();                    agent.FedTaxIdentitytext = string.IsNullOrEmpty(dataRow["FedTaxIdentityProof"].ToString()) ? "" : dataRow["FedTaxIdentityProof"].ToString();                    agent.PricingPlan = dataRow["PricingPlan"].ToString().StringToInt(0);                    agent.PrimaryBusinessAdd1 = string.IsNullOrEmpty(dataRow["PrimaryBusinessAdd1"].ToString()) ? "" : dataRow["PrimaryBusinessAdd1"].ToString();                    agent.PrimaryBusinessEmail = string.IsNullOrEmpty(dataRow["PrimaryBusinessEmail"].ToString()) ? "" : dataRow["PrimaryBusinessEmail"].ToString();                    agent.PrimaryBusinessPhone = string.IsNullOrEmpty(dataRow["PrimaryBusinessPhone"].ToString()) ? "" : dataRow["PrimaryBusinessPhone"].ToString();                    agent.PrimaryBusinessState = string.IsNullOrEmpty(dataRow["PrimaryBusinessState"].ToString()) ? "" : dataRow["PrimaryBusinessState"].ToString(); ;                    agent.PrimaryBusinessAdd2 = string.IsNullOrEmpty(dataRow["PrimaryBusinessAdd2"].ToString()) ? "" : dataRow["PrimaryBusinessAdd2"].ToString();                    agent.PrimaryBusinessCity = string.IsNullOrEmpty(dataRow["PrimaryBusinessCity"].ToString()) ? "" : dataRow["PrimaryBusinessCity"].ToString();                    agent.PrimaryBusinessZip = string.IsNullOrEmpty(dataRow["PrimaryBusinessZip"].ToString()) ? "" : dataRow["PrimaryBusinessZip"].ToString();                    agent.BillingAdd1 = string.IsNullOrEmpty(dataRow["BillingAdd1"].ToString()) ? "" : dataRow["BillingAdd1"].ToString();                    agent.BillingAdd2 = string.IsNullOrEmpty(dataRow["BillingAdd2"].ToString()) ? "" : dataRow["BillingAdd2"].ToString();                    agent.BillingCity = string.IsNullOrEmpty(dataRow["BillingCity"].ToString()) ? "" : dataRow["BillingCity"].ToString();                    agent.BillingEmail = string.IsNullOrEmpty(dataRow["BillingEmail"].ToString()) ? "" : dataRow["BillingEmail"].ToString();                    agent.BillingPhone = string.IsNullOrEmpty(dataRow["BillingPhone"].ToString()) ? "" : dataRow["BillingPhone"].ToString();                    agent.BillingState = string.IsNullOrEmpty(dataRow["BillingState"].ToString()) ? "" : dataRow["BillingState"].ToString();                    agent.BillingZip = string.IsNullOrEmpty(dataRow["BillingZip"].ToString()) ? "" : dataRow["BillingZip"].ToString();                    agent.AgentBillingId = string.IsNullOrEmpty(dataRow["BillingType"].ToString()) ? "" : dataRow["BillingType"].ToString();                    agent.BillingType = string.IsNullOrEmpty(dataRow["BillingType"].ToString()) ? "" : dataRow["BillingType"].ToString();                    agent.CardType = string.IsNullOrEmpty(dataRow["CardType"].ToString()) ? "" : dataRow["CardType"].ToString();                    agent.CardNumber = string.IsNullOrEmpty(dataRow["CardNumber"].ToString()) ? "" : common.Decrypt(dataRow["CardNumber"].ToString());
                    if (date.Length > 1)
                    {
                        agent.ExpiryDate = string.IsNullOrEmpty(date[1]) ? "" : date[1];
                        agent.Month = string.IsNullOrEmpty(date[0]) ? "" : date[0];
                    }                    agent.BillingZipCode = string.IsNullOrEmpty(dataRow["BillingZipCode"].ToString()) ? "" : dataRow["BillingZipCode"].ToString();                    agent.CVV = string.IsNullOrEmpty(dataRow["CVV"].ToString()) ? "" : common.Decrypt(dataRow["CVV"].ToString());                    agent.DriversLicenseState = string.IsNullOrEmpty(dataRow["DriversLicenseState"].ToString()) ? "" : dataRow["DriversLicenseState"].ToString();                    agent.DriversLicenseNumber = string.IsNullOrEmpty(dataRow["DriversLicenseNumber"].ToString()) ? "" : dataRow["DriversLicenseNumber"].ToString();                    agent.DriversLicenseCopytext = string.IsNullOrEmpty(dataRow["DriversLicenseCopy"].ToString()) ? "" : dataRow["DriversLicenseCopy"].ToString();                    agent.IncChallengeInq = dataRow["IncChallengeInq"].ToString();                                    }            }            catch (Exception ex) { ex.insertTrace(""); }            return agent;        }
        //      public Agent GetAgent(string id)
        //{
        //	Agent agent = new Agent();
        //	string sql = "";
        //	bool loginstatus = sessionData.GetLoginStatus();
        //	DataRow dataRow = null;
        //	try
        //	{
        //		if (!loginstatus)
        //		{
        //			sql = "select Top 1  A.*,AB.*,U.UserName from Agent A left Join AgentBilling AB on A.AgentId=AB.AgentId left Join Users U "
        //                      + " on A.AgentId=U.AgentClientId where u.userrole='agentadmin' and  A.AgentId=" + id;
        //		}
        //		else
        //		{
        //			sql = "select Top 1  A.*,AB.*,U.UserName from Agent A left Join AgentBilling AB on A.AgentId=AB.AgentId left Join Users U "
        //                      + " on AB.AgentId=U.AgentClientId where   u.userrole='agentadmin' and A.AgentId=" + id + " and isPrimary=1";
        //		}
        //		dataTable = utilities.GetDataTable(sql, true);
        //		if (dataTable.Rows.Count > 0)
        //		{
        //			dataRow = dataTable.Rows[0];
        //			string expirydate = common.Decrypt(dataRow["ExpiryDate"].ToString());
        //			string[] date = expirydate.Split('-');

        //			agent.AgentId = Convert.ToInt32(dataRow["AgentId"].ToString());
        //			agent.BusinessName = string.IsNullOrEmpty(dataRow["BusinessName"].ToString()) ? "" : dataRow["BusinessName"].ToString();
        //			agent.UserName = string.IsNullOrEmpty(dataRow["UserName"].ToString()) ? "" : dataRow["UserName"].ToString();
        //			agent.FirstName = string.IsNullOrEmpty(dataRow["FirstName"].ToString()) ? "" : dataRow["FirstName"].ToString();
        //			agent.LastName = string.IsNullOrEmpty(dataRow["LastName"].ToString()) ? "" : dataRow["LastName"].ToString();
        //			agent.MiddleName = string.IsNullOrEmpty(dataRow["MiddleName"].ToString()) ? "" : dataRow["MiddleName"].ToString(); 
        //			agent.DBA = string.IsNullOrEmpty(dataRow["DBA"].ToString()) ? "" : dataRow["DBA"].ToString();
        //                  int companytypeid = dataRow["TypeOfComp"] == null ? 0 : dataRow["TypeOfComp"].ToString().StringToInt(0);
        //                  if (companytypeid > 0)
        //                  {
        //                      agent.TypeOfComp = registrationFunctions.GetCompanyTypeById(companytypeid);
        //                  }
        //                  agent.StateOfEncorp = string.IsNullOrEmpty(dataRow["StateOfEncorp"].ToString()) ? "" : dataRow["StateOfEncorp"].ToString();
        //			agent.StateOfIncorporationtext = dataRow["StateOfIncorporationProof"] == null ? "" : dataRow["StateOfIncorporationProof"].ToString();
        //			agent.DateOfEncorp = string.IsNullOrEmpty(dataRow["DateOfEncorp"].ToString()) ? "" : dataRow["DateOfEncorp"].ToString();
        //			agent.FedTaxIdentityNo = string.IsNullOrEmpty(dataRow["FedTaxIdentityNo"].ToString()) ? "" : dataRow["FedTaxIdentityNo"].ToString();
        //			agent.FedTaxIdentitytext = string.IsNullOrEmpty(dataRow["FedTaxIdentityProof"].ToString()) ? "" : dataRow["FedTaxIdentityProof"].ToString();
        //                  agent.DriversLicenseNumber = string.IsNullOrEmpty(dataRow["DriversLicenseNumber"].ToString()) ? "" : dataRow["DriversLicenseNumber"].ToString();
        //                  agent.DriversLicenseState = string.IsNullOrEmpty(dataRow["DriversLicenseState"].ToString()) ? "" : dataRow["DriversLicenseState"].ToString();
        //                  agent.DriversLicenseCopytext = string.IsNullOrEmpty(dataRow["DriversLicenseCopy"].ToString()) ? "" : dataRow["DriversLicenseCopy"].ToString();
        //                  agent.PricingPlan = dataRow["PricingPlan"].ToString().StringToInt(0);
        //                  agent.PrimaryBusinessAdd1 = string.IsNullOrEmpty(dataRow["PrimaryBusinessAdd1"].ToString()) ? "" : dataRow["PrimaryBusinessAdd1"].ToString();
        //			agent.PrimaryBusinessEmail = string.IsNullOrEmpty(dataRow["PrimaryBusinessEmail"].ToString()) ? "" : dataRow["PrimaryBusinessEmail"].ToString();
        //			agent.PrimaryBusinessPhone = string.IsNullOrEmpty(dataRow["PrimaryBusinessPhone"].ToString()) ? "" : dataRow["PrimaryBusinessPhone"].ToString();
        //			agent.PrimaryBusinessState = string.IsNullOrEmpty(dataRow["PrimaryBusinessState"].ToString()) ? "" : dataRow["PrimaryBusinessState"].ToString(); ;
        //			agent.PrimaryBusinessAdd2 = string.IsNullOrEmpty(dataRow["PrimaryBusinessAdd2"].ToString()) ? "" : dataRow["PrimaryBusinessAdd2"].ToString();
        //			agent.PrimaryBusinessCity = string.IsNullOrEmpty(dataRow["PrimaryBusinessCity"].ToString()) ? "" : dataRow["PrimaryBusinessCity"].ToString();
        //			agent.PrimaryBusinessZip = string.IsNullOrEmpty(dataRow["PrimaryBusinessZip"].ToString()) ? "" : dataRow["PrimaryBusinessZip"].ToString();
        //			agent.BillingAdd1 = string.IsNullOrEmpty(dataRow["BillingAdd1"].ToString()) ? "" : dataRow["BillingAdd1"].ToString();
        //			agent.BillingAdd2 = string.IsNullOrEmpty(dataRow["BillingAdd2"].ToString()) ? "" : dataRow["BillingAdd2"].ToString();
        //			agent.BillingCity = string.IsNullOrEmpty(dataRow["BillingCity"].ToString()) ? "" : dataRow["BillingCity"].ToString();
        //			agent.BillingEmail = string.IsNullOrEmpty(dataRow["BillingEmail"].ToString()) ? "" : dataRow["BillingEmail"].ToString();
        //			agent.BillingPhone = string.IsNullOrEmpty(dataRow["BillingPhone"].ToString()) ? "" : dataRow["BillingPhone"].ToString();
        //			agent.BillingState = string.IsNullOrEmpty(dataRow["BillingState"].ToString()) ? "" : dataRow["BillingState"].ToString();
        //			agent.BillingZip = string.IsNullOrEmpty(dataRow["BillingZip"].ToString()) ? "" : dataRow["BillingZip"].ToString();


        //			agent.AgentBillingId = string.IsNullOrEmpty(dataRow["BillingType"].ToString()) ? "" : dataRow["BillingType"].ToString();
        //			agent.BillingType = string.IsNullOrEmpty(dataRow["BillingType"].ToString()) ? "" : dataRow["BillingType"].ToString();
        //			agent.CardType = string.IsNullOrEmpty(dataRow["CardType"].ToString()) ? "" : dataRow["CardType"].ToString();
        //			agent.CardNumber = string.IsNullOrEmpty(dataRow["CardNumber"].ToString()) ? "" : common.Decrypt(dataRow["CardNumber"].ToString());
        //                  if(date.Length > 1)
        //                  {
        //                      agent.ExpiryDate = string.IsNullOrEmpty(date[1]) ? "" : date[1];
        //                      agent.Month = string.IsNullOrEmpty(date[0]) ? "" : date[0];
        //                  }
        //			agent.BillingZipCode = string.IsNullOrEmpty(dataRow["BillingZipCode"].ToString()) ? "" : dataRow["BillingZipCode"].ToString();
        //			agent.CVV = string.IsNullOrEmpty(dataRow["CVV"].ToString()) ? "" : common.Decrypt(dataRow["CVV"].ToString());


        //		}
        //	}
        //	catch (Exception ex) {  ex.insertTrace("");  }

        //	return agent;
        //}
        public bool GetUserStatus(string id = "")
		{
			bool status = false;
			string sql = "";
			try
			{
				sql = "select Status from Users where userrole='agentadmin' and AgentClientId=" + id;
				dataTable = utilities.GetDataTable(sql, true);
				if (dataTable.Rows.Count > 0)
				{
					DataRow row = dataTable.Rows[0];
					status = Convert.ToBoolean(row["Status"].ToString());
				}

			}
			catch (Exception ex) {  ex.insertTrace("");  }

			return status;
		}
        public int CheckUserExists(string id = "")
        {
            int res = 0;
            string sql = "";
            try
            {
                sql = "select UserId from Users where userrole='agentadmin' and AgentClientId=" + id;
                res = utilities.ExecuteScalar(sql, true).ConvertObjectToIntIfNotNull();

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        public long UpdateUserStatus(int AgentClientID = 0)
		{
			long res = 0;
			string sql = "";
			try
			{
				if (AgentClientID != 0)
				{
					sql = "update Users set Status=1 where AgentClientId=" + AgentClientID;
					res = utilities.ExecuteCommand(sql, true);

				}
			}
			catch (Exception ex) {  ex.insertTrace("");  }
			return res;
		}
        public bool UpdatePassword(string username, string Password)
        {
            bool status = false;
            long res = 0;
            string sql = "";
            string pass = common.Encrypt(Password);
            try
            {

                sql = "Update Users Set Password=@pass where UserName=@username";
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@pass", pass);
                cmd.Parameters.AddWithValue("username", username);
                cmd.CommandText = sql;
                res = utilities.ExecuteInsertCommand(cmd, true);
                if (res > 0)
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
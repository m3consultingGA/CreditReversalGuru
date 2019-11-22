using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CreditReversal.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CreditReversal.Utilities;
using CreditReversal.DAL;

namespace CreditReversal.BLL
{
    public class RegistrationFunctions
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private SqlConnection sqlcon;
        private DBUtilities utilities = new DBUtilities();
        private AgentData agentData = new AgentData();
        private Common common = new Common();
        string nl = '\n'.ToString();

        public bool AddAgent(Agent agent)        {            bool status = false;            int agentId = 0;            string query = "";            long res = 0;            int id = agent.AgentId;            string date = DateTime.Now.ToString("yyyy-MM-dd");            try            {
                //query = "Insert into Agent (BusinessName,FirstName,LastName,MiddleName,DBA,TypeOfComp,StateOfEncorp,StateOfIncorporationProof,PricingPlan,DateOfEncorp,FedTaxIdentityNo,FedTaxIdentityProof, " + nl;
                //query += "PrimaryBusinessAdd1,PrimaryBusinessAdd2,PrimaryBusinessCity,PrimaryBusinessState,PrimaryBusinessZip,PrimaryBusinessPhone, " + nl;
                //query += "PrimaryBusinessEmail,BillingAdd1,BillingAdd2,BillingCity,BillingState,BillingZip,BillingPhone,BillingEmail,CreatedBy,CreatedDate) " + nl;
                //query += "values(@BusinessName,@FirstName,@LastName,@MiddleName,@DBA,@TypeOfComp,@StateOfEncorp,@StateOfIncorporationProof,@PricingPlan@DateOfEncorp,@FedTaxIdentityNo,@FedTaxIdentityProof, " + nl;
                //query += "@PrimaryBusinessAdd1,@PrimaryBusinessAdd2,@PrimaryBusinessCity,@PrimaryBusinessState,@PrimaryBusinessZip,@PrimaryBusinessPhone, " + nl;
                //query += "@PrimaryBusinessEmail,@BillingAdd1,@BillingAdd2,@BillingCity,@BillingState,@BillingZip,@BillingPhone,@BillingEmail,@CreatedBy,@CreatedDate) " + nl;
                //query += "SELECT CAST(scope_identity() AS int)" + nl;

                query = "update Agent set BusinessName=@BusinessName,FirstName = @FirstName,LastName=@LastName, "
+ " MiddleName=@MiddleName,DBA=@DBA,TypeOfComp=@TypeOfComp,StateOfEncorp=@StateOfEncorp, "
+ " StateOfIncorporationProof=@StateOfIncorporationProof,DriversLicenseNumber=@DriversLicenseNumber,"
+ "DriversLicenseState=@DriversLicenseState,DriversLicenseCopy=@DriversLicenseCopy,"
+ "PricingPlan=@PricingPlan, "
+ " DateOfEncorp=@DateOfEncorp,FedTaxIdentityNo=@FedTaxIdentityNo,FedTaxIdentityProof=@FedTaxIdentityProof," + nl;                query += "PrimaryBusinessAdd1=@PrimaryBusinessAdd1,PrimaryBusinessAdd2=@PrimaryBusinessAdd2,PrimaryBusinessCity=@PrimaryBusinessCity,PrimaryBusinessState=@PrimaryBusinessState,PrimaryBusinessZip=@PrimaryBusinessZip,PrimaryBusinessPhone=@PrimaryBusinessPhone, " + nl;                query += "PrimaryBusinessEmail=@PrimaryBusinessEmail,BillingAdd1=@BillingAdd1,BillingAdd2=@BillingAdd2,BillingCity=@BillingCity,BillingState=@BillingState,BillingZip=@BillingZip,BillingPhone=@BillingPhone,BillingEmail=@BillingEmail where AgentId =" + id + nl;                SqlCommand cmd = new SqlCommand();                cmd.CommandText = query;                cmd.Parameters.AddWithValue("@BusinessName", string.IsNullOrEmpty(agent.BusinessName) ? (agent.FirstName + " " + agent.LastName) : agent.BusinessName);                cmd.Parameters.AddWithValue("@FirstName", string.IsNullOrEmpty(agent.FirstName) ? "" : agent.FirstName);                cmd.Parameters.AddWithValue("@LastName", string.IsNullOrEmpty(agent.LastName) ? "" : agent.LastName);                cmd.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(agent.MiddleName) ? "" : agent.MiddleName);                cmd.Parameters.AddWithValue("@DBA", string.IsNullOrEmpty(agent.DBA) ? "" : agent.DBA);                cmd.Parameters.AddWithValue("@TypeOfComp", string.IsNullOrEmpty(agent.TypeOfComp) ? "1" : agent.TypeOfComp);                cmd.Parameters.AddWithValue("@StateOfEncorp", string.IsNullOrEmpty(agent.StateOfEncorp) ? "" : agent.StateOfEncorp);                cmd.Parameters.AddWithValue("@StateOfIncorporationProof", string.IsNullOrEmpty(agent.StateOfIncorporationtext) ? "" : agent.StateOfIncorporationtext);                cmd.Parameters.AddWithValue("@DriversLicenseNumber", string.IsNullOrEmpty(agent.DriversLicenseNumber) ? "" : agent.DriversLicenseNumber);                cmd.Parameters.AddWithValue("@DriversLicenseState", string.IsNullOrEmpty(agent.DriversLicenseState) ? "" : agent.DriversLicenseState);                cmd.Parameters.AddWithValue("@DriversLicenseCopy", string.IsNullOrEmpty(agent.DriversLicenseCopytext) ? "" : agent.DriversLicenseCopytext);                cmd.Parameters.AddWithValue("@PricingPlan", agent.PricingPlan);                cmd.Parameters.AddWithValue("@DateOfEncorp", string.IsNullOrEmpty(agent.DateOfEncorp) ? "" : agent.DateOfEncorp);                cmd.Parameters.AddWithValue("@FedTaxIdentityNo", string.IsNullOrEmpty(agent.FedTaxIdentityNo) ? "" : agent.FedTaxIdentityNo);                cmd.Parameters.AddWithValue("@FedTaxIdentityProof", string.IsNullOrEmpty(agent.FedTaxIdentitytext) ? "" : agent.FedTaxIdentitytext);                cmd.Parameters.AddWithValue("@PrimaryBusinessAdd1", string.IsNullOrEmpty(agent.PrimaryBusinessAdd1) ? "" : agent.PrimaryBusinessAdd1);                cmd.Parameters.AddWithValue("@PrimaryBusinessAdd2", string.IsNullOrEmpty(agent.PrimaryBusinessAdd2) ? "" : agent.PrimaryBusinessAdd2);                cmd.Parameters.AddWithValue("@PrimaryBusinessCity", string.IsNullOrEmpty(agent.PrimaryBusinessCity) ? "" : agent.PrimaryBusinessCity);                cmd.Parameters.AddWithValue("@PrimaryBusinessEmail", string.IsNullOrEmpty(agent.PrimaryBusinessEmail) ? "" : agent.PrimaryBusinessEmail);                cmd.Parameters.AddWithValue("@PrimaryBusinessPhone", string.IsNullOrEmpty(agent.PrimaryBusinessPhone) ? "" : agent.PrimaryBusinessPhone);                cmd.Parameters.AddWithValue("@PrimaryBusinessState", string.IsNullOrEmpty(agent.PrimaryBusinessState) ? "" : agent.PrimaryBusinessState);                cmd.Parameters.AddWithValue("@PrimaryBusinessZip", string.IsNullOrEmpty(agent.PrimaryBusinessZip) ? "" : agent.PrimaryBusinessZip);                cmd.Parameters.AddWithValue("@BillingAdd1", string.IsNullOrEmpty(agent.BillingAdd1) ? "" : agent.BillingAdd1);                cmd.Parameters.AddWithValue("@BillingAdd2", string.IsNullOrEmpty(agent.BillingAdd2) ? "" : agent.BillingAdd2);                cmd.Parameters.AddWithValue("@BillingCity", string.IsNullOrEmpty(agent.BillingCity) ? "" : agent.BillingCity);                cmd.Parameters.AddWithValue("@BillingEmail", string.IsNullOrEmpty(agent.BillingEmail) ? "" : agent.BillingEmail);                cmd.Parameters.AddWithValue("@BillingPhone", string.IsNullOrEmpty(agent.BillingPhone) ? "" : agent.BillingPhone);                cmd.Parameters.AddWithValue("@BillingState", string.IsNullOrEmpty(agent.BillingState) ? "" : agent.BillingState);                cmd.Parameters.AddWithValue("@BillingZip", string.IsNullOrEmpty(agent.BillingZip) ? "" : agent.BillingZip);                cmd.Parameters.AddWithValue("@CreatedBy", agent.CreatedBy);                cmd.Parameters.AddWithValue("@CreatedDate", date);                res = utilities.ExecuteInsertCommand(cmd, true);                if (res == 1)                {                    status = true;                }            }            catch (Exception ex) { ex.insertTrace(""); }
            //return agentId;
            return status;        }


        public bool AddAgentFiles(Agent agent)
        {
            bool status = false;
            return status;
        }

        public bool AddAgentBilling(AgentBilling agentBilling)
        {
            bool status = false;
            try
            {
                status = agentData.AddBilling(agentBilling);
            }
          catch (Exception ex) {  ex.insertTrace("");  }
            return status;
        }

        public bool UpdateUser(SignUp signUp)
        {
            bool status = false;
            string Encryptpassword = common.Encrypt(signUp.Password);
            try
            {
                string sql = "update Users set Password='" + Encryptpassword
                    + "',status=1 where AgentClientId=" + signUp.AgentClientId;
                object res = utilities.ExecuteString(sql, true);
                status = true;
            }
          catch (Exception ex) {  ex.insertTrace("");  }
            return status;
        }
        public bool AddBillingInfo(AgentBilling agentBilling, string from = "")        {            bool status = false;
            AgentBillingTransactions agent = new AgentBillingTransactions();            Billing billing = new Billing();            DataTable dataTable = new DataTable();            string sql = "";            int AgentBillingId;            long res = 0;            try            {                sql = "select * from AgentBilling where AgentId=" + agentBilling.AgentId;                dataTable = utilities.GetDataTable(sql, true);                if (dataTable.Rows.Count == 0)                {                    string query = "Insert into AgentBilling (AgentId,BillingType,CardType,CardNumber,ExpiryDate,CVV,BillingZipCode,Status,CreatedBy,CreatedDate,isPrimary)" + nl;                    query += "values(@AgentId,@BillingType,@CardType,@CardNumber,@ExpiryDate,@CVV,@BillingZipCode,1,0,GETDATE(),1) " + nl;                    query += "SELECT CAST(scope_identity() AS int)" + nl;                    SqlCommand cmd = new SqlCommand(query);                    cmd.Parameters.AddWithValue("@AgentId", agentBilling.AgentId);                    cmd.Parameters.AddWithValue("@BillingType", string.IsNullOrEmpty(agentBilling.BillingType) ? "" : agentBilling.BillingType);                    cmd.Parameters.AddWithValue("@CardType", string.IsNullOrEmpty(agentBilling.CardType) ? "" : agentBilling.CardType);                    cmd.Parameters.AddWithValue("@CardNumber", string.IsNullOrEmpty(agentBilling.CardNumber) ? "" : common.Encrypt(agentBilling.CardNumber));                    cmd.Parameters.AddWithValue("@ExpiryDate", string.IsNullOrEmpty(agentBilling.ExpiryDate) ? "" : common.Encrypt(agentBilling.ExpiryDate));                    cmd.Parameters.AddWithValue("@CVV", string.IsNullOrEmpty(agentBilling.CVV) ? "" : common.Encrypt(agentBilling.CVV));                    cmd.Parameters.AddWithValue("@BillingZipCode", string.IsNullOrEmpty(agentBilling.BillingZipCode) ? "" : agentBilling.BillingZipCode);                    cmd.Parameters.AddWithValue("@CreatedBy", agentBilling.CreatedBy);                    AgentBillingId = utilities.ExecuteScalarCommand(cmd, true).objectToInt(0);                    if (AgentBillingId > 0)                    {
                        agent = GetBillingTransactionsById(agentBilling.AgentId);                        billing.AgentId = agentBilling.AgentId;                        billing.TransactionNo = agent.TransactionId;                        billing.CardType = agentBilling.CardType;                        billing.PaymentMethodId = AgentBillingId;                        billing.PaymentType = "CREDIT CARD";
                        billing.PaymentStatus = agent.Status;                        AddBillingSuccess(billing);                        status = true;                    }                }                else                {                    if (from == "")                    {                        string query = "update AgentBilling set BillingType=@BillingType,CardType=@CardType,CardNumber=@CardNumber,ExpiryDate=@ExpiryDate,CVV=@CVV,BillingZipCode=@BillingZipCode,Status=1,CreatedBy=@CreatedBy,CreatedDate=GETDATE() where isPrimary=1";                        SqlCommand cmd = new SqlCommand(query);                        cmd.Parameters.AddWithValue("@AgentId", agentBilling.AgentId);                        cmd.Parameters.AddWithValue("@BillingType", string.IsNullOrEmpty(agentBilling.BillingType) ? "" : agentBilling.BillingType);                        cmd.Parameters.AddWithValue("@CardType", string.IsNullOrEmpty(agentBilling.CardType) ? "" : agentBilling.CardType);                        cmd.Parameters.AddWithValue("@CardNumber", string.IsNullOrEmpty(agentBilling.CardNumber) ? "" : common.Encrypt(agentBilling.CardNumber));                        cmd.Parameters.AddWithValue("@ExpiryDate", string.IsNullOrEmpty(agentBilling.ExpiryDate) ? "" : common.Encrypt(agentBilling.ExpiryDate));                        cmd.Parameters.AddWithValue("@CVV", string.IsNullOrEmpty(agentBilling.CVV) ? "" : common.Encrypt(agentBilling.CVV));                        cmd.Parameters.AddWithValue("@BillingZipCode", string.IsNullOrEmpty(agentBilling.BillingZipCode) ? "" : agentBilling.BillingZipCode);                        cmd.Parameters.AddWithValue("@CreatedBy", agentBilling.CreatedBy);                        res = utilities.ExecuteInsertCommand(cmd, true);                        if (res > 0)                        {                            status = true;                        }                    }                    if (from == "Admin")                    {                        string query = "update AgentBilling set BillingType=@BillingType,CardType=@CardType,CardNumber=@CardNumber,ExpiryDate=@ExpiryDate,CVV=@CVV,BillingZipCode=@BillingZipCode,Status=1,CreatedBy=@CreatedBy,CreatedDate=GETDATE() where AgentId='" + agentBilling.AgentId + "'";                        SqlCommand cmd = new SqlCommand(query);                        cmd.Parameters.AddWithValue("@AgentId", agentBilling.AgentId);                        cmd.Parameters.AddWithValue("@BillingType", string.IsNullOrEmpty(agentBilling.BillingType) ? "" : agentBilling.BillingType);                        cmd.Parameters.AddWithValue("@CardType", string.IsNullOrEmpty(agentBilling.CardType) ? "" : agentBilling.CardType);                        cmd.Parameters.AddWithValue("@CardNumber", string.IsNullOrEmpty(agentBilling.CardNumber) ? "" : common.Encrypt(agentBilling.CardNumber));                        cmd.Parameters.AddWithValue("@ExpiryDate", string.IsNullOrEmpty(agentBilling.ExpiryDate) ? "" : common.Encrypt(agentBilling.ExpiryDate));                        cmd.Parameters.AddWithValue("@CVV", string.IsNullOrEmpty(agentBilling.CVV) ? "" : common.Encrypt(agentBilling.CVV));                        cmd.Parameters.AddWithValue("@BillingZipCode", string.IsNullOrEmpty(agentBilling.BillingZipCode) ? "" : agentBilling.BillingZipCode);                        cmd.Parameters.AddWithValue("@CreatedBy", agentBilling.CreatedBy);                        res = utilities.ExecuteInsertCommand(cmd, true);                        if (res > 0)                        {                            status = true;                        }                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }            return status;        }
        //public bool AddBillingInfo(AgentBilling agentBilling)
        //{
        //    bool status = false;
        //    DataTable dataTable = new DataTable();
        //    string sql = "";
        //    long res = 0;
        //    try
        //    {
        //        sql = "select * from AgentBilling where AgentId=" + agentBilling.AgentId;
        //        dataTable = utilities.GetDataTable(sql, true);

        //        if (dataTable.Rows.Count == 0)
        //        {
        //            string query = "Insert into AgentBilling (AgentId,BillingType,CardType,CardNumber,ExpiryDate,CVV,BillingZipCode,Status,CreatedBy,CreatedDate,isPrimary) values(@AgentId,@BillingType,@CardType,@CardNumber,@ExpiryDate,@CVV,@BillingZipCode,1,0,GETDATE(),1)";
        //            SqlCommand cmd = new SqlCommand(query);
        //            cmd.Parameters.AddWithValue("@AgentId", agentBilling.AgentId);
        //            cmd.Parameters.AddWithValue("@BillingType", string.IsNullOrEmpty(agentBilling.BillingType) ? "" : agentBilling.BillingType);
        //            cmd.Parameters.AddWithValue("@CardType", string.IsNullOrEmpty(agentBilling.CardType) ? "" : agentBilling.CardType);
        //            cmd.Parameters.AddWithValue("@CardNumber", string.IsNullOrEmpty(agentBilling.CardNumber) ? "" : common.Encrypt(agentBilling.CardNumber));
        //            cmd.Parameters.AddWithValue("@ExpiryDate", string.IsNullOrEmpty(agentBilling.ExpiryDate) ? "" : common.Encrypt(agentBilling.ExpiryDate));
        //            cmd.Parameters.AddWithValue("@CVV", string.IsNullOrEmpty(agentBilling.CVV) ? "" : common.Encrypt(agentBilling.CVV));
        //            cmd.Parameters.AddWithValue("@BillingZipCode", string.IsNullOrEmpty(agentBilling.BillingZipCode) ? "" : agentBilling.BillingZipCode);
        //            res = utilities.ExecuteInsertCommand(cmd, true);
        //            if (res > 0)
        //            {
        //                status = true;
        //            }
        //        }
        //        else
        //        {
        //            string query = "update AgentBilling set BillingType=@BillingType,CardType=@CardType,CardNumber=@CardNumber,ExpiryDate=@ExpiryDate,CVV=@CVV,BillingZipCode=@BillingZipCode,Status=1,CreatedBy=0,CreatedDate=GETDATE() where isPrimary=1";
        //            SqlCommand cmd = new SqlCommand(query);
        //            cmd.Parameters.AddWithValue("@AgentId", agentBilling.AgentId);
        //            cmd.Parameters.AddWithValue("@BillingType", string.IsNullOrEmpty(agentBilling.BillingType) ? "" : agentBilling.BillingType);
        //            cmd.Parameters.AddWithValue("@CardType", string.IsNullOrEmpty(agentBilling.CardType) ? "" : agentBilling.CardType);
        //            cmd.Parameters.AddWithValue("@CardNumber", string.IsNullOrEmpty(agentBilling.CardNumber) ? "" : common.Encrypt(agentBilling.CardNumber));
        //            cmd.Parameters.AddWithValue("@ExpiryDate", string.IsNullOrEmpty(agentBilling.ExpiryDate) ? "" : common.Encrypt(agentBilling.ExpiryDate));
        //            cmd.Parameters.AddWithValue("@CVV", string.IsNullOrEmpty(agentBilling.CVV) ? "" : common.Encrypt(agentBilling.CVV));
        //            cmd.Parameters.AddWithValue("@BillingZipCode", string.IsNullOrEmpty(agentBilling.BillingZipCode) ? "" : agentBilling.BillingZipCode);
        //            res = utilities.ExecuteInsertCommand(cmd, true);
        //            if (res > 0)
        //            {
        //                status = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex) {  ex.insertTrace("");  }

        //    return status;
        //}

        public bool checkCurrentPassword(string username="",string password="")
		{
			bool status = false;
			string encryptpassword = common.Encrypt(password);
			string sql = "";
			DataTable dataTable = new DataTable();
			try
			{
				sql = "select * from Users where UserName='"+username+ "' and Password='"+ encryptpassword + "'";
				dataTable = utilities.GetDataTable(sql,true);
				if(dataTable.Rows.Count>0)
				{
					status = true;
				}
			}catch (Exception ex) {  ex.insertTrace("");  }

			return status;
		}
        public List<Pricing> GetPricing()
        {
            List<Pricing> pricings = new List<Pricing>();

            try
            {
                string sql = "select PricingId,PricingType,SetupFee from Pricing";
                string type = "";
                string setupfee = "";
                DataTable dataTable = new DataTable();
                dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {

                        type = row["PricingType"].ToString();
                        setupfee = row["SetupFee"].ToString().PadUpto2();

                        pricings.Add(new Pricing
                        {
                            PricingId = row["PricingId"].ToString().StringToInt(0),
                            PricingType = type + " - $" + setupfee,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }

            return pricings;
        }

        public string GetCompanyTypeById(int id)
        {
            string CompanyType = "";

            try
            {
                string sql = "select CompanyType from CompanyTypes where CompanyTypeId=" + id;

                DataRow row = null;
                row = utilities.GetDataRow(sql);
                CompanyType = row["CompanyType"].ToString();

            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return CompanyType;
        }

        public List<CompanyTypes> GetCompanyTypes()
        {
            List<CompanyTypes> companyTypes = new List<CompanyTypes>();

            try
            {
                string sql = "select CompanyTypeId,CompanyType from CompanyTypes";
                DataTable dataTable = new DataTable();
                dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        companyTypes.Add(new CompanyTypes
                        {
                            CompanyTypeId = row["CompanyTypeId"].ToString().StringToInt(0),
                            CompanyType = row["CompanyType"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return companyTypes;
        }
        public List<States> GetStates()
        {
            List<States> states = new List<States>();

            try
            {
                string sql = "select StateCode,StateName from States";
                DataTable dataTable = new DataTable();

                dataTable = utilities.GetDataTable(sql, true);

                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        states.Add(new States
                        {
                            StateCode = row["StateCode"].ToString(),
                            StateName = row["StateName"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return states;
        }
        public int AddAgentNew(Agent agent)        {            bool status = false;            int agentId = 0;            string query = "";            long res = 0;            int id = agent.AgentId;            string date = DateTime.Now.ToString("yyyy-MM-dd");            try            {                using (sqlcon = new SqlConnection(connectionString))                {                    if (agent.Password != null)                    {                        agent.Password = common.Encrypt(agent.Password);                    }                    query = "Insert into Agent (BusinessName,FirstName,LastName,MiddleName,DBA,TypeOfComp,StateOfEncorp,StateOfIncorporationProof,PricingPlan,DateOfEncorp,FedTaxIdentityNo,FedTaxIdentityProof, " + nl;                    query += "PrimaryBusinessAdd1,PrimaryBusinessAdd2,PrimaryBusinessCity,PrimaryBusinessState,PrimaryBusinessZip,PrimaryBusinessPhone, " + nl;                    query += "PrimaryBusinessEmail,BillingAdd1,BillingAdd2,BillingCity,BillingState,BillingZip,BillingPhone,BillingEmail,CreatedBy,CreatedDate,DriversLicenseNumber,DriversLicenseState,DriversLicenseCopy) " + nl;                    query += "values(@BusinessName,@FirstName,@LastName,@MiddleName,@DBA,@TypeOfComp,@StateOfEncorp,@StateOfIncorporationProof,@PricingPlan,@DateOfEncorp,@FedTaxIdentityNo,@FedTaxIdentityProof, " + nl;                    query += "@PrimaryBusinessAdd1,@PrimaryBusinessAdd2,@PrimaryBusinessCity,@PrimaryBusinessState,@PrimaryBusinessZip,@PrimaryBusinessPhone, " + nl;                    query += "@PrimaryBusinessEmail,@BillingAdd1,@BillingAdd2,@BillingCity,@BillingState,@BillingZip,@BillingPhone,@BillingEmail,@CreatedBy,@CreatedDate,@DriversLicenseNumber,@DriversLicenseState,@DriversLicenseCopy) " + nl;                    query += "SELECT CAST(scope_identity() AS int)" + nl;                    SqlCommand cmd = new SqlCommand(query, sqlcon);                    if (sqlcon.State == ConnectionState.Closed)                    {                        sqlcon.Open();                    }                    cmd.CommandText = query;                    cmd.Parameters.AddWithValue("@BusinessName", string.IsNullOrEmpty(agent.BusinessName) ? (agent.FirstName + " " + agent.LastName) : agent.BusinessName);                    cmd.Parameters.AddWithValue("@FirstName", string.IsNullOrEmpty(agent.FirstName) ? "" : agent.FirstName);                    cmd.Parameters.AddWithValue("@LastName", string.IsNullOrEmpty(agent.LastName) ? "" : agent.LastName);                    cmd.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(agent.MiddleName) ? "" : agent.MiddleName);                    cmd.Parameters.AddWithValue("@DBA", string.IsNullOrEmpty(agent.DBA) ? "" : agent.DBA);                    cmd.Parameters.AddWithValue("@TypeOfComp", string.IsNullOrEmpty(agent.TypeOfComp) ? "SOLO PROP" : agent.TypeOfComp);                    cmd.Parameters.AddWithValue("@StateOfEncorp", string.IsNullOrEmpty(agent.StateOfEncorp) ? "" : agent.StateOfEncorp);                    cmd.Parameters.AddWithValue("@StateOfIncorporationProof", string.IsNullOrEmpty(agent.StateOfIncorporationtext) ? "" : agent.StateOfIncorporationtext);                    cmd.Parameters.AddWithValue("@PricingPlan", agent.PricingPlan);                    cmd.Parameters.AddWithValue("@DateOfEncorp", string.IsNullOrEmpty(agent.DateOfEncorp) ? "" : agent.DateOfEncorp);                    cmd.Parameters.AddWithValue("@FedTaxIdentityNo", string.IsNullOrEmpty(agent.FedTaxIdentityNo) ? "" : agent.FedTaxIdentityNo);                    cmd.Parameters.AddWithValue("@FedTaxIdentityProof", string.IsNullOrEmpty(agent.FedTaxIdentitytext) ? "" : agent.FedTaxIdentitytext);                    cmd.Parameters.AddWithValue("@PrimaryBusinessAdd1", string.IsNullOrEmpty(agent.PrimaryBusinessAdd1) ? "" : agent.PrimaryBusinessAdd1);                    cmd.Parameters.AddWithValue("@PrimaryBusinessAdd2", string.IsNullOrEmpty(agent.PrimaryBusinessAdd2) ? "" : agent.PrimaryBusinessAdd2);                    cmd.Parameters.AddWithValue("@PrimaryBusinessCity", string.IsNullOrEmpty(agent.PrimaryBusinessCity) ? "" : agent.PrimaryBusinessCity);                    cmd.Parameters.AddWithValue("@PrimaryBusinessEmail", string.IsNullOrEmpty(agent.PrimaryBusinessEmail) ? "" : agent.PrimaryBusinessEmail);                    cmd.Parameters.AddWithValue("@PrimaryBusinessPhone", string.IsNullOrEmpty(agent.PrimaryBusinessPhone) ? "" : agent.PrimaryBusinessPhone);                    cmd.Parameters.AddWithValue("@PrimaryBusinessState", string.IsNullOrEmpty(agent.PrimaryBusinessState) ? "" : agent.PrimaryBusinessState);                    cmd.Parameters.AddWithValue("@PrimaryBusinessZip", string.IsNullOrEmpty(agent.PrimaryBusinessZip) ? "" : agent.PrimaryBusinessZip);                    cmd.Parameters.AddWithValue("@BillingAdd1", string.IsNullOrEmpty(agent.BillingAdd1) ? "" : agent.BillingAdd1);                    cmd.Parameters.AddWithValue("@BillingAdd2", string.IsNullOrEmpty(agent.BillingAdd2) ? "" : agent.BillingAdd2);                    cmd.Parameters.AddWithValue("@BillingCity", string.IsNullOrEmpty(agent.BillingCity) ? "" : agent.BillingCity);                    cmd.Parameters.AddWithValue("@BillingEmail", string.IsNullOrEmpty(agent.BillingEmail) ? "" : agent.BillingEmail);                    cmd.Parameters.AddWithValue("@BillingPhone", string.IsNullOrEmpty(agent.BillingPhone) ? "" : agent.BillingPhone);                    cmd.Parameters.AddWithValue("@BillingState", string.IsNullOrEmpty(agent.BillingState) ? "" : agent.BillingState);                    cmd.Parameters.AddWithValue("@BillingZip", string.IsNullOrEmpty(agent.BillingZip) ? "" : agent.BillingZip);                    cmd.Parameters.AddWithValue("@CreatedBy", agent.CreatedBy);                    cmd.Parameters.AddWithValue("@CreatedDate", date);                    cmd.Parameters.AddWithValue("@DriversLicenseNumber", string.IsNullOrEmpty(agent.DriversLicenseNumber) ? "" : agent.DriversLicenseNumber);                    cmd.Parameters.AddWithValue("@DriversLicenseState", string.IsNullOrEmpty(agent.DriversLicenseState) ? "" : agent.DriversLicenseState);                    cmd.Parameters.AddWithValue("@DriversLicenseCopy", string.IsNullOrEmpty(agent.DriversLicenseCopytext) ? "" : agent.DriversLicenseCopytext);                    agentId = utilities.ExecuteScalarCommand(cmd, true).objectToInt(0);                    if (agentId > 0)                    {                        if (agent.FedTaxIdentitytext != null)                        {                            agent.FedTaxIdentitytext = "Agent-" + agentId + "-" + agent.FedTaxIdentitytext;                        }                        if (agent.StateOfIncorporationtext != null)                        {                            agent.StateOfIncorporationtext = "Agent-" + agentId + "-" + agent.StateOfIncorporationtext;                        }                        if (agent.DriversLicenseCopytext != null)                        {                            agent.DriversLicenseCopytext = "Agent-" + agentId + "-" + agent.DriversLicenseCopytext;                        }                        query = "";                        query = "Update Agent set FedTaxIdentityProof=@UFedTaxIdentityProof,StateOfIncorporationProof=@UStateOfIncorporationProof,DriversLicenseCopy=@DriversLicenseCopy where AgentId=@AgentId" + nl;                        SqlCommand cmd1 = new SqlCommand(query);                        cmd1.Parameters.AddWithValue("@AgentId", agentId);                        cmd1.Parameters.AddWithValue("@UFedTaxIdentityProof", string.IsNullOrEmpty(agent.FedTaxIdentitytext) ? "" : agent.FedTaxIdentitytext);                        cmd1.Parameters.AddWithValue("@UStateOfIncorporationProof", string.IsNullOrEmpty(agent.StateOfIncorporationtext) ? "" : agent.StateOfIncorporationtext);                        cmd1.Parameters.AddWithValue("@DriversLicenseCopy", string.IsNullOrEmpty(agent.DriversLicenseCopytext) ? "" : agent.DriversLicenseCopytext);                        res = utilities.ExecuteInsertCommand(cmd1, true);                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }
            //return agentId;
            return agentId;        }
        public bool AddAgentBillingTransactions(AgentBillingTransactions agent)        {            bool status = false;            long res = 0;            try            {                string sql = "Insert Into AgentBillingTransactions(AgentId,TransactionId,ResponseCode,MessageCode,Description,AuthorizeCode,ErrorCode,ErrorMessage,Status,IPAddress) values  (@AgentId,@TransactionId,@ResponseCode,@MessageCode,@Description,@AuthorizeCode,@ErrorCode,@ErrorMessage,@Status,@IPAddress)";                SqlCommand cmd = new SqlCommand();                cmd.Parameters.AddWithValue("@AgentId", agent.AgentId);                cmd.Parameters.AddWithValue("@TransactionId", string.IsNullOrEmpty(agent.TransactionId) ? "" : agent.TransactionId);                cmd.Parameters.AddWithValue("@ResponseCode", string.IsNullOrEmpty(agent.ResponseCode) ? "" : agent.ResponseCode);                cmd.Parameters.AddWithValue("@MessageCode", string.IsNullOrEmpty(agent.MessageCode) ? "" : agent.MessageCode);                cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(agent.Description) ? "" : agent.Description);                cmd.Parameters.AddWithValue("@AuthorizeCode", string.IsNullOrEmpty(agent.AuthorizeCode) ? "" : agent.AuthorizeCode);                cmd.Parameters.AddWithValue("@ErrorCode", string.IsNullOrEmpty(agent.ErrorCode) ? "" : agent.ErrorCode);                cmd.Parameters.AddWithValue("@ErrorMessage", string.IsNullOrEmpty(agent.ErrorMessage) ? "" : agent.ErrorMessage);                cmd.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(agent.Status) ? "" : agent.Status);                cmd.Parameters.AddWithValue("@IPAddress", string.IsNullOrEmpty(agent.IPAddress) ? "" : agent.IPAddress);                cmd.CommandText = sql;                res = utilities.ExecuteInsertCommand(cmd, true);                if (res == 1)                {                    status = true;                }            }            catch (Exception ex) { ex.insertTrace(""); }            return status;        }

        public string GetPricingById(int Id)        {            string SetupFee = string.Empty;            string PricingType = string.Empty;            string type = string.Empty;            DataRow row = null;            try            {                string sql = "Select * from Pricing where PricingId =" + Id;                row = utilities.GetDataRow(sql);                if (row != null)                {                    PricingType = row["PricingType"].ToString();                    SetupFee = row["SetupFee"].ToString().PadUpto2();                }                type = PricingType + " - $" + SetupFee;            }            catch (Exception ex)            {                ex.insertTrace("");            }            return type;        }
        public AgentBillingTransactions GetBillingTransactionsById(int BillingId)
        {
            AgentBillingTransactions agent = new AgentBillingTransactions();
            try
            {
                string query = "Select * from AgentBillingTransactions where status='SUCCESS' and AgentId =" + BillingId;
                DataTable dataTable = new DataTable();
                dataTable = utilities.GetDataTable(query, true);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    agent.TransactionId = row["TransactionId"].ToString();
                    agent.Status = row["Status"].ToString();
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return agent;
        }

        public bool AddBillingSuccess(Billing billing)
        {
            bool status = false;            long res = 0;            try            {
                string sql = "Insert Into Billing(AgentId,TransactionNo,CardType,PaymentMethodId,PaymentType,PaymentStatus,PaymentDate) values(@AgentId,@TransactionNo,@CardType,@PaymentMethodId,@PaymentType,@PaymentStatus,GETDATE())";
                SqlCommand cmd = new SqlCommand();

                cmd.Parameters.AddWithValue("@AgentId", billing.AgentId);                cmd.Parameters.AddWithValue("@TransactionNo", billing.TransactionNo);                cmd.Parameters.AddWithValue("@CardType", billing.CardType);                cmd.Parameters.AddWithValue("@PaymentMethodId", billing.PaymentMethodId);                cmd.Parameters.AddWithValue("@PaymentType", billing.PaymentType);                cmd.Parameters.AddWithValue("@PaymentStatus", billing.PaymentStatus);
                cmd.CommandText = sql;                res = utilities.ExecuteInsertCommand(cmd, true);                if (res == 1)                {                    status = true;                }
            }            catch (Exception ex) { ex.insertTrace(""); }            return status;
        }
    }
}
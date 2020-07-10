using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CreditReversal.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using CreditReversal.DAL;
using CreditReversal.Utilities;


namespace CreditReversal.DAL
{
    public class AgentData
    {
        Traces traces = new Traces();
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private SqlConnection sqlcon;

        DBUtilities utills = new DBUtilities();
        private Common common = new Common();
        //ExtensionMethods extension = new ExtensionMethods();
        string nl = '\n'.ToString();


        public int AddAgent(Agent agent)
        {
            int agentId = 0;
            string query = "";
            int id = agent.AgentId;
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                using (sqlcon = new SqlConnection(connectionString))
                {
                    if (id != 0)
                    {
                        query = "update Agent set BusinessName=@BusinessName,FirstName = @FirstName,LastName=@LastName,MiddleName=@MiddleName,DBA=@DBA,TypeOfComp=@TypeOfComp,StateOfEncorp=@StateOfEncorp,DateOfEncorp=@DateOfEncorp,FedTaxIdentityNo=@FedTaxIdentityNo, " + nl;
                        query += "PrimaryBusinessAdd1=@PrimaryBusinessAdd1,PrimaryBusinessAdd2=@PrimaryBusinessAdd2,PrimaryBusinessCity=@PrimaryBusinessCity,PrimaryBusinessState=@PrimaryBusinessState,PrimaryBusinessZip=@PrimaryBusinessZip,PrimaryBusinessPhone=@PrimaryBusinessPhone, " + nl;
                        query += "PrimaryBusinessEmail=@PrimaryBusinessEmail,BillingAdd1=@BillingAdd1,BillingAdd2=@BillingAdd2,BillingCity=@BillingCity,BillingState=@BillingState,BillingZip=@BillingZip,BillingPhone=@BillingPhone,BillingEmail=@BillingEmail where AgentId =" + id + nl;
                    }
                    else
                    {
                        query = "Insert into Agent (BusinessName,FirstName,LastName,MiddleName,DBA,TypeOfComp,StateOfEncorp,DateOfEncorp,FedTaxIdentityNo, " + nl;
                        query += "PrimaryBusinessAdd1,PrimaryBusinessAdd2,PrimaryBusinessCity,PrimaryBusinessState,PrimaryBusinessZip,PrimaryBusinessPhone, " + nl;
                        query += "PrimaryBusinessEmail,BillingAdd1,BillingAdd2,BillingCity,BillingState,BillingZip,BillingPhone,BillingEmail,CreatedBy,CreatedDate) " + nl;
                        query += "values(@BusinessName,@FirstName,@LastName,@MiddleName,@DBA,@TypeOfComp,@StateOfEncorp,@DateOfEncorp,@FedTaxIdentityNo, " + nl;
                        query += "@PrimaryBusinessAdd1,@PrimaryBusinessAdd2,@PrimaryBusinessCity,@PrimaryBusinessState,@PrimaryBusinessZip,@PrimaryBusinessPhone, " + nl;
                        query += "@PrimaryBusinessEmail,@BillingAdd1,@BillingAdd2,@BillingCity,@BillingState,@BillingZip,@BillingPhone,@BillingEmail,@CreatedBy,@CreatedDate) " + nl;
                        query += "SELECT CAST(scope_identity() AS int)" + nl;

                    }
                    SqlCommand cmd = new SqlCommand(query, sqlcon);

                    cmd.Parameters.AddWithValue("@BusinessName", agent.BusinessName);
                    cmd.Parameters.AddWithValue("@FirstName", string.IsNullOrEmpty(agent.FirstName) ? "" : agent.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", string.IsNullOrEmpty(agent.LastName) ? "" : agent.LastName);
                    cmd.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(agent.MiddleName) ? "" : agent.MiddleName);
                    cmd.Parameters.AddWithValue("@DBA", string.IsNullOrEmpty(agent.DBA) ? "" : agent.DBA);
                    cmd.Parameters.AddWithValue("@TypeOfComp", string.IsNullOrEmpty(agent.TypeOfComp) ? "" : agent.TypeOfComp);
                    cmd.Parameters.AddWithValue("@StateOfEncorp", string.IsNullOrEmpty(agent.StateOfEncorp) ? "" : agent.StateOfEncorp);
                    cmd.Parameters.AddWithValue("@DateOfEncorp", string.IsNullOrEmpty(agent.DateOfEncorp) ? "" : agent.DateOfEncorp);
                    cmd.Parameters.AddWithValue("@FedTaxIdentityNo", string.IsNullOrEmpty(agent.FedTaxIdentityNo) ? "" : agent.FedTaxIdentityNo);
                    cmd.Parameters.AddWithValue("@PrimaryBusinessAdd1", string.IsNullOrEmpty(agent.PrimaryBusinessAdd1) ? "" : agent.PrimaryBusinessAdd1);
                    cmd.Parameters.AddWithValue("@PrimaryBusinessAdd2", string.IsNullOrEmpty(agent.PrimaryBusinessAdd2) ? "" : agent.PrimaryBusinessAdd2);
                    cmd.Parameters.AddWithValue("@PrimaryBusinessCity", string.IsNullOrEmpty(agent.PrimaryBusinessCity) ? "" : agent.PrimaryBusinessCity);
                    cmd.Parameters.AddWithValue("@PrimaryBusinessEmail", string.IsNullOrEmpty(agent.PrimaryBusinessEmail) ? "" : agent.PrimaryBusinessEmail);
                    cmd.Parameters.AddWithValue("@PrimaryBusinessPhone", string.IsNullOrEmpty(agent.PrimaryBusinessPhone) ? "" : agent.PrimaryBusinessPhone);
                    cmd.Parameters.AddWithValue("@PrimaryBusinessState", string.IsNullOrEmpty(agent.PrimaryBusinessState) ? "" : agent.PrimaryBusinessState);
                    cmd.Parameters.AddWithValue("@PrimaryBusinessZip", string.IsNullOrEmpty(agent.PrimaryBusinessZip) ? "" : agent.PrimaryBusinessZip);
                    cmd.Parameters.AddWithValue("@BillingAdd1", string.IsNullOrEmpty(agent.BillingAdd1) ? "" : agent.BillingAdd1);
                    cmd.Parameters.AddWithValue("@BillingAdd2", string.IsNullOrEmpty(agent.BillingAdd2) ? "" : agent.BillingAdd2);
                    cmd.Parameters.AddWithValue("@BillingCity", string.IsNullOrEmpty(agent.BillingCity) ? "" : agent.BillingCity);
                    cmd.Parameters.AddWithValue("@BillingEmail", string.IsNullOrEmpty(agent.BillingEmail) ? "" : agent.BillingEmail);
                    cmd.Parameters.AddWithValue("@BillingPhone", string.IsNullOrEmpty(agent.BillingPhone) ? "" : agent.BillingPhone);
                    cmd.Parameters.AddWithValue("@BillingState", string.IsNullOrEmpty(agent.BillingState) ? "" : agent.BillingState);
                    cmd.Parameters.AddWithValue("@BillingZip", string.IsNullOrEmpty(agent.BillingZip) ? "" : agent.BillingZip);
                    if (id == 0)
                    {
                        cmd.Parameters.AddWithValue("@CreatedBy", agent.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedDate", date);
                    }

                    sqlcon.Open();
                    long res = 0;

                    agentId = (Int32)cmd.ExecuteScalar();
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }

            return agentId;
        }


        public bool AddUser(Users user)
        {

            bool status = false;

            string date = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                using (sqlcon = new SqlConnection(connectionString))
                {
                    string query = "Insert into Users (UserName,Password,EmailAddress,UserRole,CreatedBy,CreatedDate,AgentClientId) values(@UserName,@Password,@EmailAddress,@UserRole,@CreatedBy,@CreatedDate,@AgentClientId)";
                    SqlCommand cmd = new SqlCommand(query, sqlcon);
                    cmd.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(user.UserName) ? "" : user.UserName);
                    cmd.Parameters.AddWithValue("@EmailAddress", string.IsNullOrEmpty(user.EmailAddress) ? "" : user.EmailAddress);
                    cmd.Parameters.AddWithValue("@UserRole", string.IsNullOrEmpty(user.UserRole) ? "" : user.UserRole);
                    cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedDate", date);
                    cmd.Parameters.AddWithValue("@Password", string.IsNullOrEmpty(user.Password) ? "" : user.Password);
                    cmd.Parameters.AddWithValue("@AgentClientId", user.AgentClientId);
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                    status = true;
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }

            return status;
        }


        public List<Agent> GetAgent(int agentid = 0)        {            List<Agent> agent = new List<Agent>();            try            {                using (sqlcon = new SqlConnection(connectionString))                {                    string query = "Select * ,isnull(BusinessName,(isnull(FirstName,'')+ ' ' +isnull(MiddleName,'')+ ' ' +isnull(LastName,''))) as BName, "                        + " (isnull(FirstName,'')+ ' ' +isnull(MiddleName,'')+ ' ' +isnull(LastName,'')) as UserName from Agent  INNER JOIN CompanyTypes ON Agent.TypeOfComp = CompanyTypes.CompanyTypeId where Agent.status=1 ";                    if (agentid > 0)                    {                        query += " and agentid=" + agentid;                    }                    DataTable dt = utills.GetDataTable(query);                    foreach (DataRow row in dt.Rows)                    {                        Agent ag = new Agent();                        ag.AgentId = Convert.ToInt32(row["AgentId"]);                        ag.BusinessName = row["BName"].ToString();                        ag.FirstName = row["FirstName"].ToString();                        ag.LastName = row["LastName"].ToString();                        ag.UserName = row["UserName"].ToString();                        ag.MiddleName = row["MiddleName"].ToString();                        ag.DBA = row["DBA"].ToString();
                        ag.CompanyType= row["CompanyType"].ToString();
                        ag.TypeOfComp = row["TypeOfComp"].ToString();                        ag.StateOfEncorp = row["StateOfEncorp"].ToString();                        ag.DateOfEncorp = row["DateOfEncorp"].ToString();                        ag.FedTaxIdentityNo = row["FedTaxIdentityNo"].ToString();                        ag.PrimaryBusinessAdd1 = row["PrimaryBusinessAdd1"].ToString();                        ag.PrimaryBusinessEmail = row["PrimaryBusinessEmail"].ToString();                        ag.PrimaryBusinessPhone = row["PrimaryBusinessPhone"].ToString();                        ag.PrimaryBusinessState = row["PrimaryBusinessState"].ToString();                        ag.PrimaryBusinessAdd2 = row["PrimaryBusinessAdd2"].ToString();                        ag.PrimaryBusinessCity = row["PrimaryBusinessCity"].ToString();                        ag.PrimaryBusinessZip = row["PrimaryBusinessZip"].ToString();                        ag.BillingAdd1 = row["BillingAdd1"].ToString();                        ag.BillingAdd2 = row["BillingAdd2"].ToString();                        ag.BillingCity = row["BillingCity"].ToString();                        ag.BillingEmail = row["BillingEmail"].ToString();                        ag.BillingPhone = row["BillingPhone"].ToString();                        ag.BillingState = row["BillingState"].ToString();                        ag.BillingPhone = row["BillingZip"].ToString();                        if (ag.TypeOfComp == "1")                        {                            ag.checkClientExists = checkClientExists(ag.AgentId);                        }                        else                        {                            ag.checkStaffExists = checkStaffExists(ag.AgentId);                        }                        agent.Add(ag);                    }                }            }
            catch (Exception ex) { ex.insertTrace(""); }            return agent;        }
        public bool checkStaffExists(int AgentId)        {            bool status = false;            try            {                string query = "Select AgentStaffId from AgentStaff where AgentId=" + AgentId;                DataTable dt = new DataTable();                dt = utills.GetDataTable(query, true);                if (dt.Rows.Count > 0)                {                    status = true;                }            }            catch (Exception ex)            {                ex.insertTrace("");            }            return status;        }        public bool checkClientExists(int AgentId)        {            bool status = false;            try            {                string query = "Select ClientId from Client where AgentId=" + AgentId;                DataTable dt = new DataTable();                dt = utills.GetDataTable(query, true);                if (dt.Rows.Count > 0)                {                    status = true;                }            }            catch (Exception ex)            {                ex.insertTrace("");            }            return status;        }
        //public List<Agent> GetAgent(int agentid = 0)
        //{
        //    List<Agent> agent = new List<Agent>();
        //    try
        //    {
        //        using (sqlcon = new SqlConnection(connectionString))
        //        {
        //            string query = "Select * ,isnull(BusinessName,(isnull(FirstName,'')+ ' ' +isnull(MiddleName,'')+ ' ' +isnull(LastName,''))) as BName, "
        //                + " (isnull(FirstName,'')+ ' ' +isnull(MiddleName,'')+ ' ' +isnull(LastName,'')) as UserName from Agent where status=1 ";
        //            if (agentid > 0)
        //            {
        //                query += " and agentid=" + agentid;
        //            }
        //            DataTable dt = utills.GetDataTable(query);
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                Agent ag = new Agent();
        //                ag.AgentId = Convert.ToInt32(row["AgentId"]);
        //                ag.BusinessName = row["BName"].ToString();
        //                ag.FirstName = row["FirstName"].ToString();
        //                ag.LastName = row["LastName"].ToString();
        //                ag.UserName = row["UserName"].ToString();
        //                ag.MiddleName = row["MiddleName"].ToString();
        //                ag.DBA = row["DBA"].ToString();
        //                ag.TypeOfComp = row["TypeOfComp"].ToString();
        //                ag.StateOfEncorp = row["StateOfEncorp"].ToString();
        //                ag.DateOfEncorp = row["DateOfEncorp"].ToString();
        //                ag.FedTaxIdentityNo = row["FedTaxIdentityNo"].ToString();
        //                ag.PrimaryBusinessAdd1 = row["PrimaryBusinessAdd1"].ToString();
        //                ag.PrimaryBusinessEmail = row["PrimaryBusinessEmail"].ToString();
        //                ag.PrimaryBusinessPhone = row["PrimaryBusinessPhone"].ToString();
        //                ag.PrimaryBusinessState = row["PrimaryBusinessState"].ToString();
        //                ag.PrimaryBusinessAdd2 = row["PrimaryBusinessAdd2"].ToString();
        //                ag.PrimaryBusinessCity = row["PrimaryBusinessCity"].ToString();
        //                ag.PrimaryBusinessZip = row["PrimaryBusinessZip"].ToString();
        //                ag.BillingAdd1 = row["BillingAdd1"].ToString();
        //                ag.BillingAdd2 = row["BillingAdd2"].ToString();
        //                ag.BillingCity = row["BillingCity"].ToString();
        //                ag.BillingEmail = row["BillingEmail"].ToString();
        //                ag.BillingPhone = row["BillingPhone"].ToString();
        //                ag.BillingState = row["BillingState"].ToString();
        //                ag.BillingPhone = row["BillingZip"].ToString();
        //                agent.Add(ag);
        //            }

        //        }

        //    }
        //    catch (Exception ex) { ex.insertTrace(""); }
        //    return agent;
        //}

        public Agent EditAgent(string AgentID)
        {
            Agent agent = new Agent();
            try
            {
                using (sqlcon = new SqlConnection(connectionString))
                {
                    string query = "Select * from Agent where AgentId=" + AgentID;
                    SqlCommand cmd = new SqlCommand(query, sqlcon);
                    sqlcon.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        agent.BusinessName = reader["BusinessName"].ToString();
                        agent.FirstName = reader["FirstName"].ToString();
                        agent.LastName = reader["LastName"].ToString();
                        //agent.UserName = reader["UserName"].ToString();
                        agent.MiddleName = reader["MiddleName"].ToString();
                        agent.DBA = reader["DBA"].ToString();
                        agent.TypeOfComp = reader["TypeOfComp"].ToString();
                        agent.StateOfEncorp = reader["StateOfEncorp"].ToString();
                        agent.DateOfEncorp = reader["DateOfEncorp"].ToString();
                        agent.FedTaxIdentityNo = reader["FedTaxIdentityNo"].ToString();
                        agent.PrimaryBusinessAdd1 = reader["PrimaryBusinessAdd1"].ToString();
                        agent.PrimaryBusinessEmail = reader["PrimaryBusinessEmail"].ToString();
                        agent.PrimaryBusinessPhone = reader["PrimaryBusinessPhone"].ToString();
                        agent.PrimaryBusinessState = reader["PrimaryBusinessState"].ToString();
                        agent.PrimaryBusinessAdd2 = reader["PrimaryBusinessAdd2"].ToString();
                        agent.PrimaryBusinessCity = reader["PrimaryBusinessCity"].ToString();
                        agent.PrimaryBusinessZip = reader["PrimaryBusinessZip"].ToString();
                        agent.BillingAdd1 = reader["BillingAdd1"].ToString();
                        agent.BillingAdd2 = reader["BillingAdd2"].ToString();
                        agent.BillingCity = reader["BillingCity"].ToString();
                        agent.BillingEmail = reader["BillingEmail"].ToString();
                        agent.BillingPhone = reader["BillingPhone"].ToString();
                        agent.BillingState = reader["BillingState"].ToString();
                        agent.BillingZip = reader["BillingZip"].ToString();

                    }
                }


            }
            catch (Exception ex) { ex.insertTrace(""); }

            return agent;
        }


        public List<AgentStaff> GetStaff(string agentid = null, string staffid = null)
        {
            List<AgentStaff> staff = new List<AgentStaff>();

            try
            {
                using (sqlcon = new SqlConnection(connectionString))
                {
                    string query = "Select * ,(FirstName+ '   ' +LastName) as usernames from AgentStaff "
                        + " Where status=1 ";
                    if (!string.IsNullOrEmpty(agentid))
                    {
                        query += " and agentid=" + agentid;
                    }
                    else if (!string.IsNullOrEmpty(staffid))
                    {
                        query += " and agentstaffid=" + staffid;
                    }
                    DataTable dt = utills.GetDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        AgentStaff ag = new AgentStaff();
                        ag.AgentStaffId = Convert.ToInt32(row["AgentStaffId"]);
                        ag.FirstName = row["FirstName"].ToString();
                        ag.LastName = row["LastName"].ToString();
                        ag.MiddleName = row["MiddleName"].ToString();
                        ag.UserName = row["usernames"].ToString();
                        ag.PhoneNumber = row["ContactPhoneNo"].ToString();
                        ag.Email = row["EmailAddress"].ToString();
                        ag.CheckClientExit = CheckClientExit(ag.AgentStaffId);
                        ag.AgentId = Convert.ToInt32(row["AgentId"]);
                        staff.Add(ag);
                    }

                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return staff;
        }

        public int AddStaff(AgentStaff staff)
        {
            int staffID = 0;
            int id = staff.AgentStaffId;
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string query = "";
            try
            {
                using (sqlcon = new SqlConnection(connectionString))
                {
                    if (id != 0)
                    {
                        query = "Update AgentStaff Set FirstName=@FirstName,LastName=@LastName,MiddleName=@MiddleName,ContactPhoneNo=@ContactPhoneNo,EmailAddress=@EmailAddress where AgentStaffId =" + id;
                    }
                    else
                    {

                        query = "Insert into AgentStaff(FirstName,LastName,MiddleName,ContactPhoneNo,EmailAddress,AgentId,CreatedBy,CreatedDate) values(@FirstName,@LastName,@MiddleName,@ContactPhoneNo,@EmailAddress,@AgentId,@CreatedBy,@CreatedDate) SELECT CAST(scope_identity() AS int)";
                    }

                    SqlCommand cmd = new SqlCommand(query, sqlcon);
                    sqlcon.Open();
                    cmd.Parameters.AddWithValue("@FirstName", string.IsNullOrEmpty(staff.FirstName) ? "" : staff.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", string.IsNullOrEmpty(staff.LastName) ? "" : staff.LastName);
                    cmd.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(staff.MiddleName) ? "" : staff.MiddleName);
                    cmd.Parameters.AddWithValue("@ContactPhoneNo", string.IsNullOrEmpty(staff.PhoneNumber) ? "" : staff.PhoneNumber);
                    cmd.Parameters.AddWithValue("@EmailAddress", string.IsNullOrEmpty(staff.Email) ? "" : staff.Email);
                    if (id == 0)
                    {
                        cmd.Parameters.AddWithValue("@AgentId", staff.AgentId);
                        cmd.Parameters.AddWithValue("@CreatedBy", staff.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedDate", date);
                        staffID = (Int32)cmd.ExecuteScalar();
                    }
                    else
                    {
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return staffID;
        }

        public AgentStaff EditStaff(string staffId)
        {
            AgentStaff staff = new AgentStaff();

            try
            {
                using (sqlcon = new SqlConnection(connectionString))
                {
                    string query = "Select * from AgentStaff where AgentStaffId =" + staffId;
                    SqlCommand cmd = new SqlCommand(query, sqlcon);
                    sqlcon.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        staff.AgentStaffId = Convert.ToInt32(reader["AgentStaffId"]);
                        staff.FirstName = reader["FirstName"].ToString();
                        staff.LastName = reader["LastName"].ToString();
                        staff.MiddleName = reader["MiddleName"].ToString();
                        staff.PhoneNumber = reader["ContactPhoneNo"].ToString();
                        staff.Email = reader["EmailAddress"].ToString();
                    }

                }

            }
            catch (Exception ex) { ex.insertTrace(""); }


            return staff;
        }

        public Users EditUser(string Id, string UserRole)
        {
            Users user = new Users();
            try
            {
                using (sqlcon = new SqlConnection(connectionString))
                {
                    string query = "Select * from Users where UserRole ='" + UserRole + "' and AgentClientId =" + Id;
                    SqlCommand cmd = new SqlCommand(query, sqlcon);
                    sqlcon.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        user.UserName = reader["UserName"].ToString();
                        user.EmailAddress = reader["EmailAddress"].ToString();
                        string pass = reader["Password"].ToString();
                        user.Password = common.Decrypt(pass);
                        user.UserRole = reader["UserRole"].ToString();
                        user.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                        user.CreatedDate = reader["CreatedDate"].ToString();
                    }

                }



            }
            catch (Exception ex) { ex.insertTrace(""); }

            return user;
        }
        public bool DeleteAgentStaff(string Id)
        {
            bool status = false;
            long del = 0;
            try
            {

                string query = "Delete from AgentStaff where AgentStaffId =" + Id;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = query;
                del = utills.ExecuteInsertCommand(cmd, true);
                if (del == 1)
                {
                    // status = DeleteUserAgentStaff(Id);
                    string sql = "Delete from Users where UserRole= 'agentstaff' and AgentClientId =" + Id;

                    SqlCommand command = new SqlCommand();
                    command.CommandText = sql;
                    del = utills.ExecuteInsertCommand(command, true);
                    if (del == 1)
                    {
                        status = true;
                    }

                }


            }
            catch (Exception ex) { ex.insertTrace(""); }

            return status;
        }

        //Billing methods
        public bool AddBilling(AgentBilling agentbilling)        {            bool status = false;            long res = 0;            agentbilling.Status = "1";            if (agentbilling.CardNumber != null)            {                agentbilling.CardNumber = common.Encrypt(agentbilling.CardNumber);            }            if (agentbilling.ExpiryDate != null)            {                agentbilling.ExpiryDate = common.Encrypt(agentbilling.ExpiryDate);            }            if (agentbilling.CVV != null)            {                agentbilling.CVV = common.Encrypt(agentbilling.CVV);            }            try            {                using (sqlcon = new SqlConnection(connectionString))                {                    if (agentbilling.AgentBillingId != 0)                    {                        string query = "Update AgentBilling set BillingType=@BillingType,CardType=@CardType," + nl;                        query += "CardNumber=@CardNumber,ExpiryDate=@ExpiryDate,CVV=@CVV,Status=@Status,CreatedBy=@CreatedBy,CreatedDate=@CreatedDate," + nl;                        query += " BillingZipCode=@BillingZipCode, IsPrimary=@IsPrimary  where AgentBillingId=@AgentBillingId" + nl;


                        SqlCommand cmd = new SqlCommand(query, sqlcon);                        if (sqlcon.State == ConnectionState.Closed)                            sqlcon.Open();                        cmd.Parameters.AddWithValue("@AgentBillingId", agentbilling.AgentBillingId);                        cmd.Parameters.AddWithValue("@BillingType", string.IsNullOrEmpty(agentbilling.BillingType) ? "" : agentbilling.BillingType);                        cmd.Parameters.AddWithValue("@CardType", string.IsNullOrEmpty(agentbilling.CardType) ? "" : agentbilling.CardType);                        cmd.Parameters.AddWithValue("@CardNumber", string.IsNullOrEmpty(agentbilling.CardNumber) ? "" : agentbilling.CardNumber);                        cmd.Parameters.AddWithValue("@ExpiryDate", string.IsNullOrEmpty(agentbilling.ExpiryDate) ? "" : agentbilling.ExpiryDate);                        cmd.Parameters.AddWithValue("@CVV", string.IsNullOrEmpty(agentbilling.CVV) ? "" : agentbilling.CVV);                        cmd.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(agentbilling.Status) ? "" : agentbilling.Status);                        cmd.Parameters.AddWithValue("@CreatedBy", agentbilling.CreatedBy);                        cmd.Parameters.AddWithValue("@CreatedDate", agentbilling.CreatedDate);                        cmd.Parameters.AddWithValue("@BillingZipCode", string.IsNullOrEmpty(agentbilling.BillingZipCode) ? "" : agentbilling.BillingZipCode);                        cmd.Parameters.AddWithValue("@IsPrimary", agentbilling.IsPrimary);                        res = utills.ExecuteInsertCommand(cmd, true);                        if (res > 0)                        {                            if (agentbilling.IsPrimary != 0)                            {                                string strCmd = "Update AgentBilling set IsPrimary=0 Where(AgentId=@AgentId and AgentBillingId!=@AgentBillingId)";                                SqlCommand cmdUpdate = new SqlCommand(strCmd, sqlcon);
                                cmdUpdate.Parameters.AddWithValue("@AgentId", agentbilling.AgentId);                                cmdUpdate.Parameters.AddWithValue("@AgentBillingId", agentbilling.AgentBillingId);                                res = utills.ExecuteInsertCommand(cmdUpdate, true);                            }

                            status = true;

                        }                    }                    else                    {                        string query = "Insert into AgentBilling (AgentId,BillingType,CardType,CardNumber,ExpiryDate,CVV,Status,CreatedBy,CreatedDate,BillingZipCode,IsPrimary) " +                            "values(@AgentId,@BillingType,@CardType,@CardNumber,@ExpiryDate,@CVV,@Status,@CreatedBy,GETDATE(),@BillingZipCode,@IsPrimary);Select scope_Identity();";                        SqlCommand cmd = new SqlCommand(query, sqlcon);
                        if (sqlcon.State == ConnectionState.Closed)                            sqlcon.Open();                        cmd.Parameters.AddWithValue("@AgentId", agentbilling.AgentId);                        cmd.Parameters.AddWithValue("@BillingType", string.IsNullOrEmpty(agentbilling.BillingType) ? "" : agentbilling.BillingType);                        cmd.Parameters.AddWithValue("@CardType", string.IsNullOrEmpty(agentbilling.CardType) ? "" : agentbilling.CardType);                        cmd.Parameters.AddWithValue("@CardNumber", string.IsNullOrEmpty(agentbilling.CardNumber) ? "" : agentbilling.CardNumber);                        cmd.Parameters.AddWithValue("@ExpiryDate", string.IsNullOrEmpty(agentbilling.ExpiryDate) ? "" : agentbilling.ExpiryDate);                        cmd.Parameters.AddWithValue("@CVV", string.IsNullOrEmpty(agentbilling.CVV) ? "" : agentbilling.CVV);                        cmd.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(agentbilling.Status) ? "" : agentbilling.Status);                        cmd.Parameters.AddWithValue("@BillingZipCode", string.IsNullOrEmpty(agentbilling.BillingZipCode) ? "" : agentbilling.BillingZipCode);                        cmd.Parameters.AddWithValue("@CreatedBy", 0);                        cmd.Parameters.AddWithValue("@IsPrimary", agentbilling.IsPrimary);                        res = Convert.ToInt64(utills.ExecuteScalarCommand(cmd, true));                        if (res > 0)                        {                            if (agentbilling.IsPrimary != 0)                            {                                string strCmd = "Update AgentBilling set IsPrimary=0 Where AgentId=@AgentId and AgentBillingId!=" + res;                                SqlCommand cmdUpdate = new SqlCommand(strCmd, sqlcon);                                cmdUpdate.Parameters.AddWithValue("@AgentId", agentbilling.AgentId);
                                res = utills.ExecuteInsertCommand(cmdUpdate, true);                                                            }                                                           status = true;                        }                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }            return status;        }

        public AgentBilling GetAgentBilling(int AgentBillingId)
        {
            AgentBilling agentB = new AgentBilling();
            try
            {
                using (sqlcon = new SqlConnection(connectionString))
                {
                    string query = "Select * from AgentBilling where AgentBillingId=" + AgentBillingId;
                    SqlCommand cmd = new SqlCommand(query, sqlcon);
                    sqlcon.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        agentB.BillingType = reader["BillingType"].ConvertObjectToStringIfNotNull();
                        agentB.CardType = reader["CardType"].ConvertObjectToStringIfNotNull();
                        agentB.CardNumber = common.Decrypt(reader["CardNumber"].ConvertObjectToStringIfNotNull());
                        agentB.ExpiryDate = common.Decrypt(reader["ExpiryDate"].ConvertObjectToStringIfNotNull());
                        agentB.CVV = common.Decrypt(reader["CVV"].ConvertObjectToStringIfNotNull());
                        agentB.Status = reader["Status"].ConvertObjectToStringIfNotNull();
                        agentB.CreatedBy = reader["CreatedBy"].ConvertObjectToIntIfNotNull();
                        agentB.CreatedDate = reader["CreatedDate"].ConvertObjectToStringIfNotNull();
                        agentB.AgentId = reader["AgentId"].ConvertObjectToIntIfNotNull();
                        agentB.BillingZipCode = reader["BillingZipCode"].ConvertObjectToStringIfNotNull();
                        agentB.AgentBillingId = reader["AgentBillingId"].ConvertObjectToIntIfNotNull();
                        agentB.IsPrimary = reader["IsPrimary"].ConvertObjectToIntIfNotNull();
                    }
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }

            return agentB;
        }


        public List<AgentBilling> GetAgentBillings(int agentid = 0)
        {
            List<AgentBilling> AgentB = new List<AgentBilling>();
            try
            {
                string sql = "select * from AgentBilling where status=1 ";
                if (agentid > 0)
                {
                    sql += " and agentid=" + agentid;
                }
                DataTable dt = utills.GetDataTable(sql, true);
                foreach (DataRow row in dt.Rows)
                {
                    AgentBilling AB = new AgentBilling();
                    AB.AgentBillingId = row["AgentBillingId"].ConvertObjectToIntIfNotNull();
                    AB.BillingType = row["BillingType"].ConvertObjectToStringIfNotNull();
                    AB.CardType = row["CardType"].ConvertObjectToStringIfNotNull();
                    AB.CardNumber = common.Decrypt(row["CardNumber"].ConvertObjectToStringIfNotNull());
                    AB.ExpiryDate = common.Decrypt(row["ExpiryDate"].ConvertObjectToStringIfNotNull());
                    AB.CVV = common.Decrypt(row["CVV"].ConvertObjectToStringIfNotNull());
                    AB.Status = row["Status"].ConvertObjectToStringIfNotNull();
                    AB.CreatedBy = row["CreatedBy"].objectToInt(0);
                    AB.CreatedDate = row["CreatedDate"].ConvertObjectToStringIfNotNull();
                    AB.AgentId = row["AgentId"].ConvertObjectToIntIfNotNull();
                    AB.IsPrimary = row["IsPrimary"].ConvertObjectToIntIfNotNull();
                    AgentB.Add(AB);
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return AgentB;
        }

        public bool DeleteAgentBilling(int AgentBillingId)
        {
            bool status = false;
            long del = 0;
            try
            {
                string query = "Delete from AgentBilling where AgentBillingId =" + AgentBillingId + " and isnull(isPrimary,0)=0";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = query;
                del = utills.ExecuteInsertCommand(cmd, true);
                if (del == 1)
                {
                    status = true;
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }

            return status;
        }
        public bool DeleteAgent(string agentId, string from = "")        {            bool status = false;            long del = 0;            try            {                if (from == "Admin")                {                    string query = "Delete from Agent where AgentId =" + agentId;                    SqlCommand cmd = new SqlCommand();                    del = utills.ExecuteCommand(query, true);                    if (del == 1)                    {                        query = "";                        query = "Delete from AgentBilling where AgentId ='" + agentId + "'";                        del = utills.ExecuteCommand(query, true);                        if (del == 1 && from == "Admin")                        {                            query = "";                            query = "Delete from Users where UserRole= 'agentadmin' and AgentClientId =" + agentId;                            del = utills.ExecuteCommand(query, true);                            if (del == 1)                            {                                status = true;                            }                        }                    }                }                else                {                    string query = "Delete from Agent where AgentId =" + agentId;                    SqlCommand cmd = new SqlCommand();                    del = utills.ExecuteCommand(query, true);                    if (del == 1)                    {                        status = true;                    }                }


            }            catch (Exception ex) { ex.insertTrace(""); }            return status;        }
        public bool CheckClientExit(int AgentStaffId)        {            bool status = false;

            try            {                string query = "Select AgentStaffId from Client where AgentStaffId=" + AgentStaffId;                DataTable dt = new DataTable();                dt = utills.GetDataTable(query, true);                if (dt.Rows.Count > 0)                {                    status = true;                }            }            catch (Exception ex)            {                ex.insertTrace("");            }            return status;        }
        public bool AddAssignStaff(List<NewClient> client)        {            bool status = false;            int count = client.Count();            try            {                using (sqlcon = new SqlConnection(connectionString))                {                    for (int i = 0; i < count; i++)                    {                        string query = "Update Client set AgentStaffId=@agentStaffId where ClientId= @client";                        SqlCommand cmd = new SqlCommand(query, sqlcon);                        cmd.Parameters.AddWithValue("@agentStaffId", client[i].Staff);                        cmd.Parameters.AddWithValue("@client", client[i].ClientId);                        sqlcon.Open();                        cmd.ExecuteNonQuery();                        status = true;                        sqlcon.Close();                    }                }            }            catch (Exception ex)            {                ex.insertTrace("");            }            return status;        }
        public List<NewClient> GetClient(int id)        {            List<NewClient> newClients = new List<NewClient>();            try            {                string sql = "";

                sql = "select ClientId,FirstName,LastName,Convert(varchar(15),DOB,101)as DBirth, "+
                      "AgentStaffId,AgentId,Convert(varchar(15),CreatedDate,101)as CreatedDate  from client where status=1 and AgentId=" + id + " order by clientId DESC";

                DataTable dataTable = utills.GetDataTable(sql, true);                if (dataTable.Rows.Count > 0)                {                    foreach (DataRow row in dataTable.Rows)                    {
                        newClients.Add(new NewClient
                        {
                            ClientId = row["ClientId"].ToString().StringToInt(0),
                            Name = row["FirstName"].ToString() + " " + row["LastName"].ToString(),
                            DBirth = row["DBirth"].ToString(),
                            SignedUpDate = Convert.ToDateTime(row["CreatedDate"].ToString()),
                            AgentStaffId = Convert.ToInt32(row["AgentStaffId"]),
                            AgentId = Convert.ToInt32(row["AgentId"]),

                        });                    }                }            }            catch (Exception ex)            {                ex.insertTrace("");            }            return newClients;        }
        public List<CreditReportFiles> GetChallenges(string ClientId)        {            List<CreditReportFiles> creditreport = new List<CreditReportFiles>();            CreditReportFiles credit = new CreditReportFiles();            try            {                DataRow clientrow = null;                string name = string.Empty;                string[] agencies;                string query = "Select FirstName,MiddleName,LastName from Client Where ClientId=" + ClientId + "";                clientrow = utills.GetDataRow(query);                if (clientrow != null)                {                    name = clientrow[0].ToString() + clientrow[1].ToString() + clientrow[2].ToString();                }                string sql = "Select * from CreditReportFiles where ClientId=" + ClientId + " order by RoundType asc  ";                DataTable dt = utills.GetDataTable(sql);                if (dt.Rows.Count > 0)                {                                       foreach (DataRow row in dt.Rows)                    {   
                        credit = new CreditReportFiles();                        credit.ClientId = Convert.ToInt32(row["ClientId"]);                        credit.RoundType = row["RoundType"].ToString();                        credit.CRFilename =  row["CRFilename"].ToString();                        credit.CreateDate = row["CreateDate"].ToString();                        credit.ClientName = name;                        agencies = credit.CRFilename.Split('-');                        credit.CAgency = agencies[2];                        creditreport.Add(credit);                                          }                }



            }            catch (Exception ex) { ex.insertTrace(""); }            return creditreport;        }
    }
}
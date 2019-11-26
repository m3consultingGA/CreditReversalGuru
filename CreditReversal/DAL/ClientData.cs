using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CreditReversal.DAL;
using System.Data;
using System.Data.SqlClient;
using CreditReversal.Models;
using System.Configuration;
using CreditReversal.Utilities;
using System.Globalization;
using System.Text;

namespace CreditReversal.DAL
{
    public class ClientData
    {
        public SessionData sessionData = new SessionData();
        DBUtilities utils = new DBUtilities();
        Common common = new Common();
        IdentityIQData iQData = new IdentityIQData();
        string nl = '\n'.ToString();
        string sql = "";
        string sqlCon = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private SqlConnection sCon = new SqlConnection();
        public List<ClientModel> GetClients(string agentid = null, string staffid = null, string clientid = null)
        {
            List<ClientModel> Client = new List<ClientModel>();
            try
            {

                string sql = "select *,isnull(b.FirstName + ' ' + b.LastName, "
                        + " (select FirstName + ' ' + LastName from Agent where AgentId = c.AgentId)) "
                        + " as agent,c.FirstName + ' ' + c.LastName as staff from Client a left "
                        + " join Agent b on b.AgentId = isnull(a.AgentId, 0) left "
                        + " join AgentStaff c on c.AgentStaffId = isnull(a.AgentStaffId, 0) where a.Status = 1 ";
                if (!string.IsNullOrEmpty(agentid))
                {
                    sql += " and a.agentid=" + agentid;
                }
                else if (!string.IsNullOrEmpty(staffid))
                {
                    sql += " and a.agentstaffid=" + staffid;
                }
                else if (!string.IsNullOrEmpty(clientid))
                {
                    sql += " and a.clientid=" + clientid;
                }
                DataTable dt = utils.GetDataTable(sql, true);

                foreach (DataRow row in dt.Rows)
                {
                    ClientModel c = new ClientModel();
                    c.ClientId = row["ClientId"].ConvertObjectToIntIfNotNull();
                    c.FirstName = row["FirstName"].ConvertObjectToStringIfNotNull();
                    c.MiddleName = row["MiddleName"].ConvertObjectToStringIfNotNull();
                    c.LastName = row["LastName"].ConvertObjectToStringIfNotNull();
                    c.DOB = row["DOB"].ConvertObjectToStringIfNotNull();
                    c.SSN = row["SSN"].ConvertObjectToStringIfNotNull();
                    c.CurrentEmail = row["CurrentEmail"].ConvertObjectToStringIfNotNull();
                    c.CurrentPhone = row["CurrentPhone"].ConvertObjectToStringIfNotNull();
                    c.sDrivingLicense = row["DrivingLicense"].ConvertObjectToStringIfNotNull();
                    c.sSocialSecCard = row["SocialSecCard"].ConvertObjectToStringIfNotNull();
                    c.sProofOfCard = row["ProofOfCard"].ConvertObjectToStringIfNotNull();
                    c.Status = row["Status"].ConvertObjectToStringIfNotNull();
                    c.CreatedBy = row["CreatedBy"].objectToInt(0);
                    c.CreatedDate = row["CreatedDate"].ConvertObjectToStringIfNotNull();
                    c.AgentName = row["agent"].ConvertObjectToStringIfNotNull();
                    c.StaffName = row["staff"].ConvertObjectToStringIfNotNull();
                    Client.Add(c);
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Client;
        }


        public int GetAgentIdByStaff()
        {
            string staffid = sessionData.GetStaffId();
            int id = 0;

            try
            {
                string sql = "select AgentId from AgentStaff where AgentStaffId=" + staffid;
                DataRow row = utils.GetDataRow(sql);
                id = row["AgentId"].ToString().StringToInt(0);
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }

            return id;
        }
        public long CreateClient(ClientModel ClientModel)
        {
            long res = 0;
            string encryPassw = "";
            try
            {
                string role = sessionData.GetUserRole();
                if (role == "agentstaff")
                {
                    ClientModel.AgentId = GetAgentIdByStaff();
                }
                else if (role == "agentadmin")
                {
                    ClientModel.AgentId = sessionData.GetAgentId().StringToInt(0);
                }
                int? id = ClientModel.ClientId;
                DataTable dt = new DataTable();
                if (id != null)
                {
                    sql = "SELECT * from Client where ClientId='" + ClientModel.ClientId + "'";
                    dt = utils.GetDataTable(sql);
                }

                ClientModel.Password = Common.CreateRandomPassword();
                string encrySSN = common.Encrypt(ClientModel.SSN);

                string encryptidpassword = string.IsNullOrEmpty(ClientModel.IdPassword) ? "" : common.Encrypt(ClientModel.IdPassword);
                ClientModel.IdPassword = encryptidpassword;
                if (ClientModel.Password != null)
                {
                    encryPassw = common.Encrypt(ClientModel.Password);
                }

                if (dt.Rows.Count > 0)
                {
                    sql = "Update Client" + nl;
                    sql += "set FirstName=@FirstName,MiddleName=@MiddleName,LastName=@LastName,DOB=@DOB,Address1=@Address1,Address2=@Address2,City=@City,State=@State,ZipCode=@ZipCode,SSN=@SSN,CurrentEmail=@CurrentEmail,CurrentPhone=@CurrentPhone,DrivingLicense=@DrivingLicense," + nl; ;
                    sql += "SocialSecCard=@SocialSecCard,ProofOfCard=@ProofOfCard,Status=@Status,CreatedBy=@CreatedBy,CreatedDate=@CreatedDate where ClientId=@ClientId" + nl;
                    var cmd = new SqlCommand();
                    cmd.Parameters.AddWithValue("@ClientId", ClientModel.ClientId);
                    cmd.Parameters.AddWithValue("@FirstName", ClientModel.FirstName);
                    if (ClientModel.MiddleName != null)
                    {
                        cmd.Parameters.AddWithValue("@MiddleName", ClientModel.MiddleName);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MiddleName", string.Empty);
                    }

                    if (ClientModel.CurrentPhone != null)
                    {
                        cmd.Parameters.AddWithValue("@CurrentPhone", ClientModel.CurrentPhone);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CurrentPhone", string.Empty);
                    }

                    if (ClientModel.DOB == "01/01/1900" || ClientModel.DOB == "" || ClientModel.DOB == null)
                    {
                        cmd.Parameters.Add("@DOB", SqlDbType.Date).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@DOB", ClientModel.DOB);
                    }

                    cmd.Parameters.AddWithValue("@Address1", string.IsNullOrEmpty(ClientModel.Address1) ? "" : ClientModel.Address1);
                    cmd.Parameters.AddWithValue("@Address2", string.IsNullOrEmpty(ClientModel.Address2) ? "" : ClientModel.Address2);
                    cmd.Parameters.AddWithValue("@City", string.IsNullOrEmpty(ClientModel.City) ? "" : ClientModel.City);
                    cmd.Parameters.AddWithValue("@State", string.IsNullOrEmpty(ClientModel.State) ? "" : ClientModel.State);
                    cmd.Parameters.AddWithValue("@ZipCode", string.IsNullOrEmpty(ClientModel.ZipCode) ? "" : ClientModel.ZipCode);

                    if (ClientModel.sDrivingLicense == null)
                    {
                        cmd.Parameters.AddWithValue("@DrivingLicense", string.Empty);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@DrivingLicense", ClientModel.sDrivingLicense);
                    }
                    if (ClientModel.sSocialSecCard == null)
                    {
                        cmd.Parameters.AddWithValue("@SocialSecCard", string.Empty);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SocialSecCard", ClientModel.sSocialSecCard);
                    }
                    if (ClientModel.sProofOfCard == null)
                    {
                        cmd.Parameters.AddWithValue("@ProofOfCard", string.Empty);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ProofOfCard", ClientModel.sProofOfCard);
                    }

                    cmd.Parameters.AddWithValue("@LastName", ClientModel.LastName);
                    cmd.Parameters.AddWithValue("@SSN", encrySSN);
                    cmd.Parameters.AddWithValue("@CurrentEmail", ClientModel.CurrentEmail);
                    cmd.Parameters.AddWithValue("@Status", ClientModel.Status);
                    cmd.Parameters.AddWithValue("@CreatedBy", ClientModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedDate", ClientModel.CreatedDate);
                    cmd.CommandText = sql;
                    res = utils.ExecuteInsertCommand(cmd, true);
                    if (res > 0)
                    {
                        IdentityIQInfo identityIQInfo = new IdentityIQInfo();
                        identityIQInfo.ClientId = ClientModel.ClientId;
                        identityIQInfo.Question = string.IsNullOrEmpty(ClientModel.IdQuestion) ? "" : ClientModel.IdQuestion;
                        identityIQInfo.Answer = string.IsNullOrEmpty(ClientModel.IdAnswer) ? "" : ClientModel.IdAnswer;
                        identityIQInfo.UserName = string.IsNullOrEmpty(ClientModel.IdUserName) ? "" : ClientModel.IdUserName;
                        identityIQInfo.Password = string.IsNullOrEmpty(ClientModel.IdPassword) ? "" : ClientModel.IdPassword;

                        res = iQData.InsertIdentityIQInfo(identityIQInfo);

                    }
                    if (res > 0)
                    {
                        sql = "Update Users" + nl;
                        sql += "set EmailAddress=@EmailAddress," + nl;
                        sql += "UserRole=@UserRole,Status=@Status,CreatedBy=@CreatedBy,CreatedDate=@CreatedDate where UserRole=@UserRole and AgentClientId=@AgentClientId" + nl;
                        cmd = new SqlCommand();
                        cmd.Parameters.AddWithValue("@AgentClientId", ClientModel.ClientId);
                        cmd.Parameters.AddWithValue("@EmailAddress", ClientModel.CurrentEmail);
                        cmd.Parameters.AddWithValue("@UserRole", ClientModel.UserRole);
                        cmd.Parameters.AddWithValue("@Status", ClientModel.Status);
                        cmd.Parameters.AddWithValue("@CreatedBy", ClientModel.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedDate", ClientModel.CreatedDate);
                        cmd.CommandText = sql;
                        res = utils.ExecuteInsertCommand(cmd, true);
                        res = (long)ClientModel.ClientId;
                    }
                    return res;
                }
                else
                {
                    if (string.IsNullOrEmpty(ClientModel.DOB))
                    {
                        ClientModel.DOB = null;
                    }


                    sql = "Insert into Client" + nl;
                    sql += "(FirstName,MiddleName,LastName,DOB,Address1,Address2,City,State,ZipCode,SSN,CurrentEmail,CurrentPhone,DrivingLicense,SocialSecCard,ProofOfCard,Status,CreatedBy,CreatedDate,AgentId,AgentStaffId)" + nl;
                    sql += "values(@FirstName,@MiddleName,@LastName,@DOB,@Address1,@Address2,@City,@State,@ZipCode,@SSN,@CurrentEmail,@CurrentPhone,@DrivingLicense,@SocialSecCard,@ProofOfCard,@Status,@CreatedBy,@CreatedDate,@AgentId,@AgentStaffId);SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand();

                    cmd.Parameters.AddWithValue("@FirstName", ClientModel.FirstName);


                    if (ClientModel.MiddleName != null)
                    {
                        cmd.Parameters.AddWithValue("@MiddleName", ClientModel.MiddleName);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MiddleName", string.Empty);
                    }

                    if (ClientModel.CurrentPhone != null)
                    {
                        cmd.Parameters.AddWithValue("@CurrentPhone", ClientModel.CurrentPhone);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CurrentPhone", string.Empty);
                    }

                    if (ClientModel.DOB == "01/01/1900" || ClientModel.DOB == "" || ClientModel.DOB == null)
                    {
                        cmd.Parameters.Add("@DOB", SqlDbType.Date).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@DOB", ClientModel.DOB);
                    }

                    cmd.Parameters.AddWithValue("@Address1", string.IsNullOrEmpty(ClientModel.Address1) ? "" : ClientModel.Address1);
                    cmd.Parameters.AddWithValue("@Address2", string.IsNullOrEmpty(ClientModel.Address2) ? "" : ClientModel.Address2);
                    cmd.Parameters.AddWithValue("@City", string.IsNullOrEmpty(ClientModel.City) ? "" : ClientModel.City);
                    cmd.Parameters.AddWithValue("@State", string.IsNullOrEmpty(ClientModel.State) ? "" : ClientModel.State);
                    cmd.Parameters.AddWithValue("@ZipCode", string.IsNullOrEmpty(ClientModel.ZipCode) ? "" : ClientModel.ZipCode);

                    if (ClientModel.sDrivingLicense == null)
                    {
                        cmd.Parameters.AddWithValue("@DrivingLicense", string.Empty);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@DrivingLicense", ClientModel.sDrivingLicense);
                    }
                    if (ClientModel.sSocialSecCard == null)
                    {
                        cmd.Parameters.AddWithValue("@SocialSecCard", string.Empty);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SocialSecCard", ClientModel.sSocialSecCard);
                    }
                    if (ClientModel.sProofOfCard == null)
                    {
                        cmd.Parameters.AddWithValue("@ProofOfCard", string.Empty);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ProofOfCard", ClientModel.sProofOfCard);
                    }

                    cmd.Parameters.AddWithValue("@AgentId", ClientModel.AgentId);
                    cmd.Parameters.AddWithValue("@AgentStaffId", ClientModel.AgentStaffId);
                    cmd.Parameters.AddWithValue("@LastName", ClientModel.LastName);
                    cmd.Parameters.AddWithValue("@SSN", encrySSN);
                    cmd.Parameters.AddWithValue("@CurrentEmail", ClientModel.CurrentEmail);
                    cmd.Parameters.AddWithValue("@Status", ClientModel.Status);
                    cmd.Parameters.AddWithValue("@CreatedBy", ClientModel.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedDate", ClientModel.CreatedDate);
                    cmd.CommandText = sql;
                    int modified = Convert.ToInt32(utils.ExecuteScalarCommand(cmd, true));
                    ClientModel.AgentClientId = Convert.ToInt32(modified);
                    if (ClientModel.AgentClientId != 0 && ClientModel.AgentClientId != null)
                    {
                        //
                        ClientModel.sSocialSecCard = ClientModel.AgentClientId + "-" + ClientModel.sSocialSecCard;
                        ClientModel.sDrivingLicense = ClientModel.AgentClientId + "-" + ClientModel.sDrivingLicense;
                        ClientModel.sProofOfCard = ClientModel.AgentClientId + "-" + ClientModel.sProofOfCard;
                        sql = "Update Client" + nl;
                        sql += "set DrivingLicense=@DrivingLicense," + nl; ;
                        sql += "SocialSecCard=@SocialSecCard,ProofOfCard=@ProofOfCard where ClientId=@ClientId" + nl;
                        cmd = new SqlCommand();
                        cmd.Parameters.AddWithValue("@ClientId", ClientModel.AgentClientId);
                        cmd.Parameters.AddWithValue("@SocialSecCard", ClientModel.sSocialSecCard);
                        cmd.Parameters.AddWithValue("@DrivingLicense", ClientModel.sDrivingLicense);
                        cmd.Parameters.AddWithValue("@ProofOfCard", ClientModel.sProofOfCard);
                        cmd.CommandText = sql;
                        res = utils.ExecuteInsertCommand(cmd, true);
                        //

                        IdentityIQInfo identityIQInfo = new IdentityIQInfo();
                        identityIQInfo.ClientId = modified;
                        identityIQInfo.Question = ClientModel.IdQuestion;
                        identityIQInfo.Answer = ClientModel.IdAnswer;
                        identityIQInfo.UserName = ClientModel.IdUserName;
                        identityIQInfo.Password = ClientModel.IdPassword;
                        if (identityIQInfo.Question != null && identityIQInfo.Answer != null)
                        {
                            res = iQData.InsertIdentityIQInfo(identityIQInfo);
                        }
                        if (res > 0)
                        {
                            sql = "Insert into Users" + nl;
                            sql += "(UserName,EmailAddress,Password,UserRole,Status,CreatedBy,CreatedDate,AgentClientId)" + nl;
                            sql += "values(@UserName,@EmailAddress,@Password,@UserRole,@Status,@CreatedBy,@CreatedDate,@AgentClientId)" + nl;
                            cmd = new SqlCommand();
                            cmd.Parameters.AddWithValue("@AgentClientId", ClientModel.AgentClientId);
                            cmd.Parameters.AddWithValue("@UserName", ClientModel.CurrentEmail);
                            cmd.Parameters.AddWithValue("@EmailAddress", ClientModel.CurrentEmail);
                            cmd.Parameters.AddWithValue("@Password", encryPassw);
                            cmd.Parameters.AddWithValue("@UserRole", ClientModel.UserRole);
                            cmd.Parameters.AddWithValue("@Status", ClientModel.Status);
                            cmd.Parameters.AddWithValue("@CreatedBy", ClientModel.CreatedBy);
                            cmd.Parameters.AddWithValue("@CreatedDate", ClientModel.CreatedDate);
                            cmd.CommandText = sql;
                            res = utils.ExecuteInsertCommand(cmd, true);
                        }
                        if (res > 0)
                        {
                            res = (long)ClientModel.AgentClientId;
                            AgentData agentData = new AgentData();
                            List<Agent> agent = agentData.GetAgent(ClientModel.AgentId);
                            common.SendMail(ClientModel.CurrentEmail, "Registration successful.", "REGISTRATION", ClientModel.CurrentEmail, "", "user", ClientModel.Password, ClientModel.FirstName);
                            common.SendMail(agent[0].PrimaryBusinessEmail, "Registration successful.", "REGISTRATION", agent[0].FirstName, "", "agentadmin", "", ClientModel.FirstName);
                        }
                    }
                    return res;
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }

        public bool CheckSSNExistorNot(string SSN = "", string ClientId = "")
        {
            bool status = false;
            try
            {
                SSN = common.Encrypt(SSN);

                if (ClientId != "" && SSN != "")
                {
                    sql = "select * from Client where SSN='" + SSN + "' and ClientId !='" + ClientId + "'";
                    DataTable dt = utils.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    sql = "select * from Client where SSN='" + SSN + "'";
                    DataTable dt = utils.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        return true;
                    }
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }

        public bool CheckUsernameexistorNot(string EmailAddress = "", int ClientId = 0)
        {
            bool status = false;
            try
            {
                if (EmailAddress != "" && ClientId == 0)
                {
                    sql = "select * from Users where UserName='" + EmailAddress + "'";
                    DataTable dt = utils.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    sql = "select * from Users where UserName='" + EmailAddress + "' and AgentClientId !='" + ClientId + "'";
                    DataTable dt = utils.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        return true;
                    }

                }


            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }


        public ClientModel GetClient(string ClientId)
        {
            ClientModel res = new ClientModel();
            try
            {
                string sql = "SELECT c.FirstName,c.MiddleName,c.LastName,Convert(varchar(15),c.DOB,101)as DOB,c.Address1,c.Address2,c.City,c.State,c.ZipCode,u.Password,c.SSN,c.CurrentEmail,c.CurrentPhone,c.DrivingLicense,c.SocialSecCard,c.ProofOfCard,i.Question,i.Answer,i.UserName as uname,i.Password as passwd,c.AgentId,c.AgentStaffId FROM Client c " + nl;
                sql += "JOIN Users u ON c.ClientId = u.AgentClientId Left Join IdentityIqInformation i ON c.ClientId=i.ClientId" + nl;
                sql += "WHERE c.ClientId= '" + ClientId + "' and u.UserRole='client'";
                DataTable dt = utils.GetDataTable(sql, true);
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

                    res.IdQuestion = row["Question"].ConvertObjectToStringIfNotNull();
                    res.IdAnswer = row["Answer"].ConvertObjectToStringIfNotNull();
                    res.IdUserName = row["uname"].ConvertObjectToStringIfNotNull();
                    res.IdPassword = common.Decrypt(row["passwd"].ConvertObjectToStringIfNotNull());
                    res.AgentId = row["AgentId"].ConvertObjectToIntIfNotNull();
                    res.AgentStaffId = row["AgentStaffId"].ConvertObjectToIntIfNotNull();
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return res;
        }

        public long DeleteClient(string ClientId)
        {
            long res = 0;
            try
            {
                if (ClientId != "" || ClientId != null)
                {
                    sql = "Delete from Client where ClientId='" + ClientId + "'";
                    var cmd1 = new SqlCommand();
                    cmd1.CommandText = sql;
                    res = utils.ExecuteInsertCommand(cmd1, true);
                    if (res > 0)
                    {
                        sql = "Delete from [Users] where AgentClientId='" + ClientId + "'";
                        var cmd = new SqlCommand();
                        cmd.CommandText = sql;
                        res = utils.ExecuteInsertCommand(cmd, true);
                    }

                    return res;
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }

        //public string AddCreditReport(List<CreditReportItems> credit, string clientId,string mode="",string round="")
        //{
        //    string ReportId = ""; string sql = string.Empty; string roundtype =string.Empty;
        //     string[] agencyname = { "EQUIFAX", "EXPERIAN", "TRANSUNION" };        //    int i = 0;
        //    long res = 0;
        //    try
        //    {        //        if(mode == "reset")
        //        {
        //            sql = "delete from CreditReportItemChallenges Where CredRepItemsId in "
        //              + " (select CredRepItemsId from CreditReportItems where CredRepItemsId in "
        //             + " (Select CreditReportId from CreditReport where ClientId =" + clientId +" and RoundType = '"+ round + "')) ";
        //            utils.ExecuteString(sql, true);
        //            sql = "delete from CreditReportItems Where CredReportId in "
        //             + " (Select CreditReportId from CreditReport where  ClientId =" + clientId + " and RoundType = '" + round + "') ";
        //            utils.ExecuteString(sql, true);
        //            sql = "delete from CreditReport where  ClientId =" + clientId + " and RoundType = '" + round + "'";
        //            utils.ExecuteString(sql, true);
        //        }        //        sql = "Select Count(CreditReportId) from CreditReport where AgencyName='EQUIFAX' and ClientId=" + clientId;        //        int count = utils.ExecuteScalar(sql, true).ConvertObjectToIntIfNotNull();        //        if(count == 0)
        //        {
        //            roundtype = "First Round";
        //         }        //        else if (count == 1)
        //        {
        //            roundtype = "Second Round";
        //        }
        //        else if (count == 2)
        //        {
        //            roundtype = "Third Round";
        //        }        //        for (i = 0; i < 3; i++)        //        {        //             sql = "Insert Into CreditReport(ClientId,DateReportPulls,AgencyName,RoundType) "        //            + " values(@ClientId,GETDATE(),@AgencyName,@RoundType);SELECT CAST(scope_identity() AS int)";        //            SqlCommand cmd = new SqlCommand();        //            cmd.CommandText = sql;        //            cmd.Parameters.AddWithValue("@ClientId", clientId);        //            cmd.Parameters.AddWithValue("@AgencyName", agencyname[i]);
        //            cmd.Parameters.AddWithValue("@RoundType", roundtype);        //            cmd.CommandText = sql;        //            res = Convert.ToInt64(utils.ExecuteScalarCommand(cmd, true));        //            int id = (int)res;        //            if (id != 0)        //            {        //                AddCreditReportItems(credit, id, agencyname[i]);        //            }        //        }

        //        //sql = "Insert Into CreditReport(ClientId,DateReportPulls,AgencyName) "
        //        //    + " values(@ClientId,GETDATE(),@AgencyName);SELECT CAST(scope_identity() AS int)";
        //        //cmd = new SqlCommand();
        //        //cmd.CommandText = sql;
        //        //cmd.Parameters.AddWithValue("@ClientId", clientId);
        //        //cmd.Parameters.AddWithValue("@AgencyName", "EXPERIAN");
        //        //cmd.CommandText = sql;
        //        //res = Convert.ToInt64(utils.ExecuteScalarCommand(cmd, true));

        //        //sql = "Insert Into CreditReport(ClientId,DateReportPulls,AgencyName) "
        //        //    + " values(@ClientId,GETDATE(),@AgencyName);SELECT CAST(scope_identity() AS int)";
        //        //cmd = new SqlCommand();
        //        //cmd.CommandText = sql;
        //        //cmd.Parameters.AddWithValue("@ClientId", clientId);
        //        //cmd.Parameters.AddWithValue("@AgencyName", "TRANSUNION");
        //        //cmd.CommandText = sql;
        //        //res = Convert.ToInt64(utils.ExecuteScalarCommand(cmd, true));

        //        ReportId = res.ToString();

        //    }
        //    catch (Exception ex) { ex.insertTrace(""); }

        //    return ReportId;
        //}
        public bool AddCreditReportItems(List<CreditReportItems> credit, int Id, string agency,int sno)        {            bool status = false;            try            {                int count = credit.Count();                for (int i = 0; i < count; i++)                {                    string sql = "Insert Into CreditReportItems(CredReportId,MerchantName,AccountId,OpenDate,"                        + " CurrentBalance,HighestBalance,Status,MonthlyPayment,LastReported,Agency,sno) values(@CredReportId,@MerchantName,@AccountId,"                        + " @OpenDate,@CurrentBalance,@HighestBalance,@Status,@MonthlyPayment,@LastReported,@Agency,"+ sno +")";                    SqlCommand cmd = new SqlCommand();                    cmd.CommandText = sql;                    cmd.Parameters.AddWithValue("@CredReportId", Id);                    cmd.Parameters.AddWithValue("@Agency", agency);                    cmd.Parameters.AddWithValue("@MerchantName", credit[i].MerchantName);                    cmd.Parameters.AddWithValue("@AccountId", credit[i].AccountId);                    cmd.Parameters.AddWithValue("@OpenDate", credit[i].OpenDate);                    cmd.Parameters.AddWithValue("@CurrentBalance", credit[i].CurrentBalance);                    cmd.Parameters.AddWithValue("@HighestBalance", credit[i].HighestBalance);                    cmd.Parameters.AddWithValue("@MonthlyPayment", "$800");                    cmd.Parameters.AddWithValue("@LastReported", "08/2019");                    if (agency == "TRANSUNION")
                    {
                        cmd.Parameters.AddWithValue("@Status", "Delinquent");
                    }                    else
                    {
                        cmd.Parameters.AddWithValue("@Status", credit[i].Status);
                    }


                    utils.ExecuteInsertCommand(cmd, true);                    status = true;                }
            }            catch (Exception ex) { ex.insertTrace(""); }            return status;        }
        public int getsno(string id)
        {
            int sno = 0;
            try
            {
                
                sql = "Select max(isnull(sno,1)) sno from CreditReport where AgencyName='EQUIFAX' and ClientId=" + id;
                try
                {
                    sno = utils.ExecuteScalar(sql, true).ConvertObjectToIntIfNotNull();
                    sno = sno + 1;
                }
                catch (Exception ex)
                {
                    sno = 1;
                }
            }
            catch (Exception)
            {}
            return sno;
        }
        public int getsnofromitems(string id,string round)
        {
            int sno = 0;
            try
            {

                sql = "Select max(sno) from CreditReportItemChallenges where ClientId =" + id + " and RoundType = '" + round + "' ";
                try
                {
                    sno = utils.ExecuteScalar(sql, true).ConvertObjectToIntIfNotNull();
                }
                catch (Exception ex)
                {
                    sno = 1;   
                }
                if(sno == 0)
                {
                    sno = 1;
                }

            }
            catch (Exception)
            { }
            return sno;
        }
        public string AddCreditReport(List<AccountHistory> credit, List<Inquires> inquires, List<MonthlyPayStatusHistory> monthlyPayStatusHistoryDetails, string clientId, string mode = "", string round = "")        {            string ReportId = ""; string sql = string.Empty; string roundtype = string.Empty;            List<AccountHistory> AccountHistory = new List<AccountHistory>();            string AgentId = HttpContext.Current.Session["UserId"].ToString();            string[] agencyname = { "EQUIFAX", "EXPERIAN", "TRANSUNION" };            int i = 0;            long res = 0;
            List<Inquires> Inquires = new List<Inquires>();            try            {

                int sno = getsno(clientId);                if (mode == "Edit")
                {
                    sql = "Select Count(CreditReportId) from CreditReport where AgencyName='EQUIFAX' and ClientId=" + clientId;
                    int count = utils.ExecuteScalar(sql, true).ConvertObjectToIntIfNotNull();
                    if (count == 0)
                    {
                        roundtype = "First Round";
                    }
                    else if (count == 1)
                    {
                        roundtype = "Second Round";
                    }
                    else if (count == 2)
                    {
                        roundtype = "Third Round";
                    }                }                else
                {
                    roundtype = round;
                }                for (i = 0; i < 3; i++)                {
                    sql = "Insert Into CreditReport(ClientId,DateReportPulls,AgencyName,RoundType,CreatedBy,CreatedDate,sno) "
                   + " values(@ClientId,GETDATE(),@AgencyName,@RoundType,@CreatedBy,getdate(),"+ sno +");SELECT CAST(scope_identity() AS int)";                    SqlCommand cmd = new SqlCommand();                    cmd.CommandText = sql;                    cmd.Parameters.AddWithValue("@ClientId", clientId);                    cmd.Parameters.AddWithValue("@AgencyName", agencyname[i]);                    cmd.Parameters.AddWithValue("@RoundType", roundtype);                    cmd.Parameters.AddWithValue("@CreatedBy", AgentId);                    cmd.CommandText = sql;                    res = Convert.ToInt64(utils.ExecuteScalarCommand(cmd, true));                    int id = (int)res;                    if (id != 0)                    {                        AccountHistory = credit.Where(x => x.Agency.ToUpper() == agencyname[i]).ToList();                        AddCreditReportItems(AccountHistory, id, agencyname[i], roundtype,sno);                        Inquires = inquires.Where(x => x.CreditBureau.ToUpper() == agencyname[i]).ToList();                        AddCreditInquiries(Inquires, id, AgentId, roundtype,sno);                    }                }
                ReportId = res.ToString();                long result=  InsertPaymentHistory(roundtype, clientId.StringToLong(0), monthlyPayStatusHistoryDetails,sno);            }            catch (Exception ex) {                 ex.insertTrace("");             }            return ReportId;        }

        public string RefreshCreditReport(List<AccountHistory> credit, List<Inquires> inquires, List<MonthlyPayStatusHistory> monthlyPayStatusHistoryDetails, string clientId, string mode = "", string round = "")        {            string ReportId = ""; string sql = string.Empty; string roundtype = string.Empty;            List<AccountHistory> AccountHistory = new List<AccountHistory>();            string AgentId = HttpContext.Current.Session["UserId"].ToString();            string[] agencyname = { "EQUIFAX", "EXPERIAN", "TRANSUNION" };            int i = 0;            long res = 0;
            List<Inquires> Inquires = new List<Inquires>();

            int sno = getsno(clientId);            try            {                if (mode == "reset")                {                    //sql = "delete from CreditReportItemChallenges Where CredRepItemsId in "                    //  + " (select CredRepItemsId from CreditReportItems where CredReportId in "                    // + " (Select CreditReportId from CreditReport where ClientId =" + clientId + " and RoundType = '" + round + "')) ";                    //utils.ExecuteString(sql, true);
                    //sql = "delete from CreditReportItemChallenges Where CreditInqId in "
                    // + " (select CreditInqId from CreditInquiries where CredReportId in "
                    //+ " (Select CreditReportId from CreditReport where ClientId =" + clientId + " and RoundType = '" + round + "')) ";                    //utils.ExecuteString(sql, true);                    sql = "delete from CreditReportItems Where CredReportId in "                     + " (Select CreditReportId from CreditReport where  ClientId =" + clientId + " and RoundType = '" + round + "') ";                    utils.ExecuteString(sql, true);

                    sql = "delete from CreditInquiries Where CreditReportId in "
                    //+ "  (select CredRepItemsId from CreditReportItems where CredReportId in "
                    + " (Select CreditReportId from CreditReport where  ClientId =" + clientId + " and RoundType = '" + round + "') ";                    utils.ExecuteString(sql, true);
                    sql = "delete from PaymentHistory Where   ClientId =" + clientId + " and RoundType = '" + round + "' ";                    utils.ExecuteString(sql, true);
                    //sql = "delete from CreditReportFiles Where   ClientId =" + clientId + " and RoundType = '" + round + "' ";                    //utils.ExecuteString(sql, true);
                }

                for (i = 0; i < 3; i++)
                {
                    sql = "select CreditReportId from  CreditReport where ClientId=@ClientId and RoundType=@RoundType and AgencyName=@AgencyName";

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@ClientId", clientId);
                    cmd.Parameters.AddWithValue("@AgencyName", agencyname[i]);
                    cmd.Parameters.AddWithValue("@RoundType", round);
                    cmd.CommandText = sql;
                    res = Convert.ToInt64(utils.ExecuteScalarCommand(cmd, true));
                    int id = (int)res;                    if (id != 0)                    {                        AccountHistory = credit.Where(x => x.Agency.ToUpper() == agencyname[i]).ToList();                        AddCreditReportItems(AccountHistory, id, agencyname[i], round, sno);                        Inquires = inquires.Where(x => x.CreditBureau.ToUpper() == agencyname[i]).ToList();                        AddCreditInquiries(Inquires, id, AgentId, round, sno);                    }


                }
                ReportId = res.ToString();                long result = InsertPaymentHistory(round, clientId.StringToLong(0), monthlyPayStatusHistoryDetails, sno);            }            catch (Exception ex) { ex.insertTrace(""); }            return ReportId;        }
        public string RefreshCreditReportBKUP(List<AccountHistory> credit, List<Inquires> inquires , List<MonthlyPayStatusHistory> monthlyPayStatusHistoryDetails,string clientId, string mode = "", string round = "")        {            string ReportId = ""; string sql = string.Empty; string roundtype = string.Empty;            List<AccountHistory> AccountHistory = new List<AccountHistory>();            string AgentId = HttpContext.Current.Session["UserId"].ToString();            string[] agencyname = { "EQUIFAX", "EXPERIAN", "TRANSUNION" };            int i = 0;            long res = 0;
            List<Inquires> Inquires = new List<Inquires>();

            int sno = getsno(clientId);            try            {                if (mode == "reset")                {                    sql = "delete from CreditReportItemChallenges Where CredRepItemsId in "                      + " (select CredRepItemsId from CreditReportItems where CredReportId in "                     + " (Select CreditReportId from CreditReport where ClientId =" + clientId + " and RoundType = '" + round + "')) ";                    utils.ExecuteString(sql, true);
                    sql = "delete from CreditReportItemChallenges Where CreditInqId in "
                     + " (select CreditInqId from CreditInquiries where CredReportId in "
                    + " (Select CreditReportId from CreditReport where ClientId =" + clientId + " and RoundType = '" + round + "')) ";                    utils.ExecuteString(sql, true);                    sql = "delete from CreditReportItems Where CredReportId in "                     + " (Select CreditReportId from CreditReport where  ClientId =" + clientId + " and RoundType = '" + round + "') ";                    utils.ExecuteString(sql, true);

                    sql = "delete from CreditInquiries Where CreditReportId in "
                      //+ "  (select CredRepItemsId from CreditReportItems where CredReportId in "
                    + " (Select CreditReportId from CreditReport where  ClientId =" + clientId + " and RoundType = '" + round + "') ";                    utils.ExecuteString(sql, true);

                    sql = "delete from PaymentHistory Where   ClientId =" + clientId + " and RoundType = '" + round + "' ";                    utils.ExecuteString(sql, true);
                    sql = "delete from CreditReportFiles Where   ClientId =" + clientId + " and RoundType = '" + round + "' ";                    utils.ExecuteString(sql, true);
                    //sql = "delete from CreditReport where  ClientId =" + clientId + " and RoundType = '" + round + "'";
                    //utils.ExecuteString(sql, true);


                }                //sql = "Select Count(CreditReportId) from CreditReport where AgencyName='EQUIFAX' and ClientId=" + clientId;                //int count = utils.ExecuteScalar(sql, true).ConvertObjectToIntIfNotNull();              
                for(i=0; i<3;i++)
                {
                    sql = "select CreditReportId from  CreditReport where ClientId=@ClientId and RoundType=@RoundType and AgencyName=@AgencyName";

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@ClientId", clientId);
                    cmd.Parameters.AddWithValue("@AgencyName", agencyname[i]);
                    cmd.Parameters.AddWithValue("@RoundType", round);                    
                     cmd.CommandText = sql;
                    res = Convert.ToInt64(utils.ExecuteScalarCommand(cmd, true));
                    int id = (int)res;                    if (id != 0)                    {                        AccountHistory = credit.Where(x => x.Agency.ToUpper() == agencyname[i]).ToList();                        AddCreditReportItems(AccountHistory, id, agencyname[i],round,sno);                        Inquires = inquires.Where(x => x.CreditBureau.ToUpper() == agencyname[i]).ToList();                        AddCreditInquiries(Inquires, id, AgentId,round,sno);                    }

                   
                }
                ReportId = res.ToString();                long result = InsertPaymentHistory(round, clientId.StringToLong(0), monthlyPayStatusHistoryDetails,sno);            }            catch (Exception ex) { ex.insertTrace(""); }            return ReportId;        }        public long InsertPaymentHistory(string round,long clientid, List<MonthlyPayStatusHistory> monthlyPayStatusHistoryDetails,int sno)
        {
            long res = 0;
            try
            {
                if(monthlyPayStatusHistoryDetails.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for(int i=0; i<monthlyPayStatusHistoryDetails.Count;i++)
                    {
                        sb.AppendFormat("Insert into PaymentHistory(Agency,Merchant,AccountNo,PHDate,PHStatus,ClientId,RoundType,sno) "
                            + " values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7})",
                            monthlyPayStatusHistoryDetails[i].Agency, monthlyPayStatusHistoryDetails[i].Bank, 
                            monthlyPayStatusHistoryDetails[i].AccountNo, monthlyPayStatusHistoryDetails[i].atdate,
                            monthlyPayStatusHistoryDetails[i].atstatus,clientid,round,sno);
                    }
                    res= utils.ExecuteString(sb.ToString(), true);
                }
            }
            catch (Exception ex)
            { string msg = ex.Message; }
            return res;
        }        public bool checkDateReport(string clientId)
        {
            bool status = false;
            object row = null;
            string date = DateTime.Now.ToString("MM/dd/yyyy").Replace('-', '/');
            try
            {
                //string sql = "select * from CreditReport where ClientId ='" + clientId + "' and convert(varchar,DateReportPulls,101)='" + date + "'";
                string sql = "select CreditReportId as pulldate from CreditReport "
                + " where ClientId  ='" + clientId + "' and GETDATE() between DateReportPulls and DATEADD(DAY,32,DateReportPulls)";                SqlCommand cmd = new SqlCommand();
                row = utils.ExecuteScalar(sql, true);

                if (row.ConvertObjectToIntIfNotNull() == 0)
                {
                    status = true;
                }
                // cmd.ExecuteReader();
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }        public bool AddCreditReportItems(List<AccountHistory> credit, int Id, string agency,string round,int sno)        {
            string AgentId = HttpContext.Current.Session["UserId"].ToString();            bool status = false;            try            {                int count = credit.Count();                for (int i = 0; i < count; i++)                {                    string sql = "Insert Into CreditReportItems(CredReportId,MerchantName,AccountId,OpenDate,"                        + " CurrentBalance,HighestBalance,Status,MonthlyPayment,LastReported,Agency,CreatedBy, "                        + " CreatedDate,negativeitems,RoundType,sno) values(@CredReportId,@MerchantName,@AccountId,"                        + " @OpenDate,@CurrentBalance,@HighestBalance,@Status,@MonthlyPayment,@LastReported,@Agency, "                        + " @CreatedBy,getdate(),@negativeitems,@RoundType,"+ sno +")";                    SqlCommand cmd = new SqlCommand();                    cmd.CommandText = sql;                    cmd.Parameters.AddWithValue("@CredReportId", Id);                    cmd.Parameters.AddWithValue("@Agency", agency);                    cmd.Parameters.AddWithValue("@MerchantName", credit[i].Bank);                    cmd.Parameters.AddWithValue("@AccountId", credit[i].Account);                    cmd.Parameters.AddWithValue("@OpenDate", credit[i].DateOpened);                    cmd.Parameters.AddWithValue("@CurrentBalance", credit[i].Balance);                    cmd.Parameters.AddWithValue("@HighestBalance", credit[i].HighCredit);                    cmd.Parameters.AddWithValue("@MonthlyPayment", credit[i].MonthlyPayment);                    cmd.Parameters.AddWithValue("@LastReported", credit[i].LastReported);                    cmd.Parameters.AddWithValue("@CreatedBy", AgentId);
                    cmd.Parameters.AddWithValue("@negativeitems", credit[i].negativeitems);
                    cmd.Parameters.AddWithValue("@RoundType", round);

                    //if (agency == "TRANSUNION")
                    //{
                    //    cmd.Parameters.AddWithValue("@Status", "Delinquent");
                    //}
                    //else
                    //{
                    cmd.Parameters.AddWithValue("@Status", credit[i].PaymentStatus);                  //  }


                    utils.ExecuteInsertCommand(cmd, true);                    status = true;                }
            }            catch (Exception ex) {                 ex.insertTrace("");             }            return status;        }


        public bool AddCreditInquiries(List<Inquires> inquires, int id, string AgentId,string round,int sno)        {            bool status = false;
            // List<Inquires> Inquires = new List<Inquires>();
            int count = inquires.Count();            try            {                for (int i = 0; i < count; i++)                {                    string sql = "Insert Into CreditInquiries (CreditReportId,CreditorName,TypeOfBusiness,DateOfInquiry, "                        + " Agency,CreatedDate,CreatedBy,RoundType,sno) values (@CreditReportId,@CreditorName,@TypeOfBusiness,@DateOfInquiry, "                        + " @Agency,GETDATE(),@CreatedBy,@RoundType,"+ sno +")";                    SqlCommand cmd = new SqlCommand();                    cmd.Parameters.AddWithValue("@CreditReportId", id);                    cmd.Parameters.AddWithValue("@CreditorName", inquires[i].CreditorName);                    cmd.Parameters.AddWithValue("@TypeOfBusiness", inquires[i].TypeofBusiness);                    cmd.Parameters.AddWithValue("@DateOfInquiry", inquires[i].Dateofinquiry);                    cmd.Parameters.AddWithValue("@Agency", inquires[i].CreditBureau);                    cmd.Parameters.AddWithValue("@CreatedBy", AgentId);                    cmd.Parameters.AddWithValue("@RoundType", round);                    cmd.CommandText = sql;                    utils.ExecuteInsertCommand(cmd, true);                    status = true;                }            }            catch (Exception ex) { ex.insertTrace(""); }            return status;        }

        public string checkLastDateReport(string clientId)        {            string status = string.Empty;
            // object row = null;

            DataTable dt = new DataTable();
            // string date = DateTime.Now.ToString("MM/dd/yyyy").Replace('-', '/');
            try            {

                string sql = "select top 1 max(CreditReportId) as pulldate  from CreditReport where ClientId ='" + clientId + "' having cast(Max(DateReportPulls) as date) <= (select DATEADD(day,-30, cast(GETDATE() as date)))";                SqlCommand cmd = new SqlCommand();                cmd.CommandText = sql;                dt = utils.ExecuteCommand(cmd, true);                if (dt.Rows.Count > 0)                {                    status = "True";
                    //foreach (DataRow row in dt.Rows)
                    //{
                    //    status = row["RoundType"].ToString();
                    //}

                }

                // cmd.ExecuteReader();
            }            catch (Exception ex) { ex.insertTrace(""); }            return status;        }

        public string[] GetRoundType(string clientId)        {            string roundtype = string.Empty;            string date = string.Empty;            string[] Getroud = new string[2];

            try            {                string sql = "Select top 1  RoundType, convert(varchar(15),DateReportPulls,101) as pulldate from CreditReport where ClientId = '" + clientId + "' order by CreditReportId desc";                DataRow row = utils.GetDataRow(sql);                roundtype = row["RoundType"].ToString();                date = row["pulldate"].ToString();                Getroud[0] = roundtype;                Getroud[1] = date;            }            catch (Exception ex) { ex.insertTrace(""); }            return Getroud;        }
        public List<CreditReportItems> GetCreditReportItems(string id)        {            DataTable dt = new DataTable();
            List<CreditReportItems> creditReportItems = new List<CreditReportItems>();            try            {                string sql = "Select * from CreditReportItems where CredRepItemsId=" + id;                dt = utils.GetDataTable(sql,true);                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        creditReportItems.Add(new CreditReportItems
                        {
                            CredRepItemsId = Convert.ToInt32(row["CredRepItemsId"].ToString()),
                            MerchantName = row["MerchantName"].ToString(),
                            AccountId = row["AccountId"].ToString(),
                            OpenDate = row["OpenDate"].ToString(),
                            CurrentBalance = row["CurrentBalance"].ToString(),
                            HighestBalance = row["HighestBalance"].ToString(),
                            Status = row["Status"].ToString(),
                            Agency = row["Agency"].ToString(),
                            RoundType = row["RoundType"].ToString()
                        });
                    }
                }            }            catch (Exception ex) { ex.insertTrace(""); }            return creditReportItems;        }
        public List<Inquires> GetInquires(string id)        {            DataTable dt = new DataTable();
            List<Inquires> inquires = new List<Inquires>();            try            {                string sql = "Select * from CreditInquiries where CreditInqId=" + id;                dt = utils.GetDataTable(sql, true);                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        inquires.Add(new Inquires
                        {
                            CreditInqId = row["CreditInqId"].ToString(),
                            CreditorName = row["CreditorName"].ToString(),
                            CreditBureau = row["Agency"].ToString(),
                        });
                    }
                }            }            catch (Exception ex) { ex.insertTrace(""); }            return inquires;        }
    }

}
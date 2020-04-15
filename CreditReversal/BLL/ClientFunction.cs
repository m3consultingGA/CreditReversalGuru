using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CreditReversal.Models;
using CreditReversal.DAL;
using CreditReversal.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace CreditReversal.BLL
{
    public class ClientFunction
    {
        private DataTable dataTable = new DataTable();
        private Common common = new Common();
        private DBUtilities utilities = new DBUtilities();
        ClientData cd = new ClientData();
        public List<ClientModel> GetClients(string agentid = null, string staffid = null, string clientid = null)
        {
            List<ClientModel> objClient = new
                 List<ClientModel>();
            try
            {
                objClient = cd.GetClients(agentid, staffid, clientid);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return objClient;
        }

        public long CreateClient(ClientModel ClientModel)
        {
            ClientModel clientModel = new ClientModel();
            long userstatus = 0;
            try
            {
                userstatus = cd.CreateClient(ClientModel);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return userstatus;
        }


        public ClientModel GetClient(string ClientId)
        {
            ClientModel objClient = new ClientModel();
            try
            {

                objClient = cd.GetClient(ClientId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return objClient;
        }



        public bool CheckSSNExistorNot(string SSN = "", string ClientId = "")
        {
            bool status = false;
            try
            {
                if (SSN != "")
                {
                    status = cd.CheckSSNExistorNot(SSN, ClientId);
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }

        public bool CheckUsernameexistorNot(string CurrentEmail = "", int ClientId = 0)
        {
            bool status = false;
            try
            {
                if (CurrentEmail != "")
                {
                    status = cd.CheckUsernameexistorNot(CurrentEmail, ClientId);
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }


        public long DeleteClient(string ClientId)
        {
            ClientModel objClient = new ClientModel();
            long userstatus = 0;
            try
            {
                userstatus = cd.DeleteClient(ClientId);
                return userstatus;
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return userstatus;
        }
        public List<CreditReportItems> GetCreditReportItems(int id)
        {
            List<CreditReportItems> creditReportItems = new List<CreditReportItems>();
            string sql = "";
            try
            {
                //    sql = "select CRI.* from CreditReport CR INNER JOIN CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId where CR.ClientId=" + id;
                sql = "select CRI.*,CR.*,CRC.* from CreditReport CR INNER JOIN CreditReportItems CRI ON"
                    + " CR.CreditReportId = CRI.CredReportId LEFT JOIN CreditReportItemChallenges CRC"
                    + " ON CRI.CredRepItemsId = CRC.CredRepItemsId where CRC.CredRepItemsId is null and CR.ClientId=" + id;

                dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        creditReportItems.Add(new CreditReportItems
                        {
                            CredRepItemsId = Convert.ToInt32(row["CredRepItemsId"].ToString()),
                            MerchantName = row["MerchantName"].ToString(),
                            AccountId = row["AccountId"].ToString(),
                            OpenDate = row["OpenDate"].ToString(),
                            // OpenDate = row["OpenDate"].ToString().stringToCultureInfoDateTime().ToShortDateString(),
                            CurrentBalance = row["CurrentBalance"].ToString(),
                            HighestBalance = row["HighestBalance"].ToString(),
                            DateReportPulls = row["DateReportPulls"].ToString(),
                            // DateReportPulls = row["DateReportPulls"].ToString().stringToCultureInfoDateTime().ToShortDateString(),
                            Status = row["Status"].ToString(),
                            Agency = row["AgencyName"].ToString(),
                            Challenge = row["ChallengeText"].ToString(),
                            RoundType = row["RoundType"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return creditReportItems;
        }
        public List<Challenge> GetChallengeMasters(int agentid = 0, int staffid = 0)
        {
            string sql = "";
            List<Challenge> challengeMasters = new List<Challenge>();

            try
            {
                sql = "select 'NO CHALLENGE' AS ChallengeText UNION select ChallengeText from ChallengeMaster "
                    + " union select ChallengeText from AgentGenChallenges where status=1 ";
                if (agentid > 0)
                {
                    sql += " and AgentId=" + agentid;
                }
                if (staffid > 0)
                {
                    sql += " and staffid=" + agentid;
                }

                dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        challengeMasters.Add(new Challenge
                        {
                            ChallengeText = row["ChallengeText"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return challengeMasters;
        }
        public bool AddChallenge(CreditReportItems credit, string AgentId = "", string staffId = "")        {            long res = 0;            try            {
                string ChallengeText = credit.Challenge;                string sql = "Select ChallengeText from ChallengeMaster where ChallengeText = '" + ChallengeText + "'";                dataTable = utilities.GetDataTable(sql, true);                if (dataTable.Rows.Count == 0)                {                    string query = "insert into AgentGenChallenges(AgentId,StaffId,ChallengeLevel,ChallengeText,CreatedDate,Status) values(@AgentId,@StaffId,@ChallengeLevel,@ChallengeText,GetDate(),@Status)";                    SqlCommand command = new SqlCommand();                    command.CommandText = query;                    command.Parameters.AddWithValue("@StaffId", string.IsNullOrEmpty(staffId) ? "" : staffId);                    command.Parameters.AddWithValue("@AgentId", string.IsNullOrEmpty(AgentId) ? "" : AgentId);                    command.Parameters.AddWithValue("@ChallengeLevel", 1);                    command.Parameters.AddWithValue("@ChallengeText", ChallengeText);                    command.Parameters.AddWithValue("@Status", 1);                    res = utilities.ExecuteInsertCommand(command, true);                }            }            catch (Exception ex) { /*ex.insertTrace("");*/ }            return true;        }
        public List<CreditReportItems> ReportItemChallenges(int id, string agency = null)
        {
            List<CreditReportItems> creditReportItems = new List<CreditReportItems>();
            string sql = "";
            try
            {
                // sql = "select CRI.* from CreditReport CR INNER JOIN CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId where CR.ClientId='"+id + "'INNER JOIN CreditReportItemChallenges CRC ON CRC.CredRepItemsId = CRI.CredRepItemsId";
                sql = "select CRC.ChallengeText,CRI.Status as CRCstatus,CR.DateReportPulls ,crc.createddate,cr.ClientId,cr.AgencyName,crc.RoundType,CRC.MerchantName,CRC.AccountId  "
                    + " from CreditReport CR INNER JOIN CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId "
                    + " LEFT JOIN CreditReportItemChallenges CRC ON CRI.AccountId = CRC.AccountId and CRI.Agency=CRC.Agency and CRC.clientid='" + +id + "' "
                    + " where CRC.CredRepItemsId is Not null and CR.ClientId=" + id;
                if (!string.IsNullOrEmpty(agency))
                {
                    sql += " and AgencyName='" + agency.ToUpper() + "'";
                }
                dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        creditReportItems.Add(new CreditReportItems
                        {

                            MerchantName = row["MerchantName"].ToString(),                            AccountId = row["AccountId"].ToString(),                            RoundType = row["RoundType"].ToString(),                            Agency = row["AgencyName"].ToString(),
                            Agent = getAgentName(row["ClientId"].ConvertObjectToIntIfNotNull())

                            //CredRepItemsId = Convert.ToInt32(row["CredRepItemsId"].ToString()),
                            //MerchantName = row["MerchantName"].ToString(),
                            //AccountId = row["AccountId"].ToString(),
                            //OpenDate = row["OpenDate"].ToString().stringToCultureInfoDateTime().ToShortDateString(),
                            //CurrentBalance = row["CurrentBalance"].ToString(),
                            //HighestBalance = row["HighestBalance"].ToString(),
                            //Status = row["CRCstatus"].ToString(),
                            //Challenge = row["ChallengeText"].ToString(),
                            //DatePulls = row["DateReportPulls"].ToString().stringToCultureInfoDateTime().ToShortDateString(),
                            //ChallengeCreatedDate = row["createddate"].ToString().stringToCultureInfoDateTime().ToShortDateString(),
                            //Agency = row["AgencyName"].ToString(),
                            //Agent = getAgentName(row["ClientId"].ConvertObjectToIntIfNotNull())
                        });
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return creditReportItems;
        }
        public string getAgentName(int clientid)
        {
            string agent = string.Empty;
            try
            {
                AgentFunction agentFunction = new AgentFunction();
                ClientModel client = GetClient(clientid.ToString());
                if (client != null)
                {
                    if (client.AgentId > 0)
                    {
                        Agent agentDetails = agentFunction.GetAgent(client.AgentId)[0];
                        agent = agentDetails.FirstName + " " + agentDetails.LastName;
                    }
                    if (client.AgentStaffId > 0)
                    {
                        AgentStaff staffDetails = agentFunction.GetStaff(null, client.AgentStaffId.ToString())[0];
                        agent = staffDetails.FirstName + " " + staffDetails.LastName;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return agent;
        }

        //public string AddCreditReport(List<CreditReportItems> credit, string clientId,string mode="",string round="")
        //{
        //    string ReportId = "";
        //    //object res = 0;
        //    bool status = false;
        //    try
        //    {
        //        status = true;//cd.checkDateReport(clientId);

        //        if (status == true)
        //        {
        //            ReportId = cd.AddCreditReport(credit, clientId,mode,round);
        //        }
        //        else
        //        {
        //            ReportId = "report";
        //        }
        //    }
        //    catch (Exception ex) { ex.insertTrace(""); }

        //    return ReportId;
        //}
        public string GetAgentName(string ClientId)
        {
            string AgentName = string.Empty;
            DataRow row = null;
            try
            {
                string query = "select ags.FirstName from Client as Cl INNER JOIN AgentStaff as ags on Cl.AgentStaffId = ags.AgentStaffId where Cl.ClientId=" + ClientId;
                SqlCommand cmd = new SqlCommand();
                row = utilities.GetDataRow(query);
                if (row != null)
                {
                    AgentName = row[0].ToString();
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return AgentName;
        }
        public string GetAgentNameNew(string ClientId)
        {
            string AgentName = string.Empty;
            DataRow row = null;
            try
            {
                string query = "select ags.FirstName from Client as c inner join Agent as ags on c.AgentId=ags.AgentId where ClientId='" + ClientId + "'";
                SqlCommand cmd = new SqlCommand();
                row = utilities.GetDataRow(query);
                if (row != null)
                {
                    AgentName = row[0].ToString();
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return AgentName;
        }
        public List<AccountHistory> GetAccountHistory(string html)
        {
            List<AccountHistory> accountHistories = new List<AccountHistory>();

            List<string> Banks = common.GetbanksFormhtml(html);

            DataSet dataSet = common.GetDataSetFromHtml(html, "rpt_content_table rpt_content_header rpt_table4column ng-scope", "AccountHistory");
            dataSet = AddBankColumnToDataSet(dataSet, Banks);

            foreach (DataTable dt in dataSet.Tables)
            {
                foreach (DataRow row in dt.Rows)
                {
                    accountHistories.Add(new AccountHistory
                    {
                        Account = string.IsNullOrEmpty(row["Account"].ToString()) ? "-" : row["Account"].ToString(),
                        AccountType = string.IsNullOrEmpty(row["AccountType"].ToString()) ? "-" : row["AccountType"].ToString(),
                        AccountTypeDetail = string.IsNullOrEmpty(row["AccountTypeDetail"].ToString()) ? "-" : row["AccountTypeDetail"].ToString(),
                        BureauCode = string.IsNullOrEmpty(row["BureauCode"].ToString()) ? "-" : row["BureauCode"].ToString(),
                        AccountStatus = string.IsNullOrEmpty(row["AccountStatus"].ToString()) ? "-" : row["AccountStatus"].ToString(),
                        MonthlyPayment = string.IsNullOrEmpty(row["MonthlyPayment"].ToString()) ? "-" : row["MonthlyPayment"].ToString(),
                        DateOpened = string.IsNullOrEmpty(row["DateOpened"].ToString()) ? "-" : row["DateOpened"].ToString(),
                        Balance = string.IsNullOrEmpty(row["Balance"].ToString()) ? "-" : row["Balance"].ToString(),
                        NoofMonths_terms = string.IsNullOrEmpty(row["No.ofMonths(terms)"].ToString()) ? "-" : row["No.ofMonths(terms)"].ToString(),
                        HighCredit = string.IsNullOrEmpty(row["HighCredit"].ToString()) ? "-" : row["HighCredit"].ToString(),
                        CreditLimit = string.IsNullOrEmpty(row["CreditLimit"].ToString()) ? "-" : row["CreditLimit"].ToString(),
                        PastDue = string.IsNullOrEmpty(row["PastDue"].ToString()) ? "-" : row["PastDue"].ToString(),
                        PaymentStatus = string.IsNullOrEmpty(row["PaymentStatus"].ToString()) ? "-" : row["PaymentStatus"].ToString(),
                        LastReported = string.IsNullOrEmpty(row["LastReported"].ToString()) ? "-" : row["LastReported"].ToString(),
                        Comments = string.IsNullOrEmpty(row["Comments"].ToString()) ? "-" : row["Comments"].ToString(),
                        DateLastActive = string.IsNullOrEmpty(row["DateLastActive"].ToString()) ? "-" : row["DateLastActive"].ToString(),
                        DateofLastPayment = string.IsNullOrEmpty(row["DateofLastPayment"].ToString()) ? "-" : row["DateofLastPayment"].ToString(),
                        Bank = row["Bank"].ToString(),
                        Agency = row["Agency"].ToString()
                    });
                }

            }
            return accountHistories;
        }

        public List<Inquires> GetInquiresFromHtml(string html)
        {
            List<Inquires> inquires = new List<Inquires>();
            try
            {
                DataSet ds = common.GetDataSetFromHtml(html, "rpt_content_table rpt_content_header rpt_content_contacts ng-scope", "Inquires");

                DataTable dt = ds.Tables[0];

                foreach (DataRow row in dt.Rows)
                {
                    inquires.Add(new Inquires
                    {
                        CreditorName = row["Creditor Name"].ToString(),
                        TypeofBusiness = row["Type of Business"].ToString(),
                        Dateofinquiry = row["Date of inquiry"].ToString(),
                        CreditBureau = row["Credit Bureau"].ToString(),
                    });
                }
            }
            catch (Exception ex)
            {

            }
            return inquires;
        }

        public DataSet AddBankColumnToDataSet(DataSet dataSet, List<string> Banks)
        {
            DataSet ds = new DataSet();
            int dataSetlen = dataSet.Tables.Count;
            int bankslen = Banks.Count();
            int i = 0;
            if (dataSetlen == bankslen)
            {
                foreach (DataTable dt in dataSet.Tables)
                {
                    DataRow row = dt.NewRow();
                    row["Type"] = "Bank";
                    row["TransUnion"] = Banks[i];
                    row["Experian"] = Banks[i];
                    row["Equifax"] = Banks[i];
                    dt.Rows.Add(row);

                    DataRow Agencydata = dt.NewRow();
                    Agencydata["Type"] = "Agency";
                    Agencydata["TransUnion"] = "TransUnion";
                    Agencydata["Experian"] = "Experian";
                    Agencydata["Equifax"] = "Equifax";
                    dt.Rows.Add(Agencydata);

                    int count = 0;
                    DataTable table = new DataTable();
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        string str = dataRow["Type"].ToString().Replace(" ", "");
                        str = str.Replace(":", "");
                        str = str.Replace("#", "");
                        str = str.Replace("-", "");
                        table.Columns.Add(str.Trim());

                        int columncount = table.Columns.Count;
                        if (columncount == 1)
                        {
                            table.Rows.Add(dataRow["TransUnion"].ToString());
                            table.Rows.Add(dataRow["Experian"].ToString());
                            table.Rows.Add(dataRow["Equifax"].ToString());
                        }
                        else
                        {
                            table.Rows[0].SetField(str.Trim(), dataRow["TransUnion"].ToString());
                            table.Rows[1].SetField(str.Trim(), dataRow["Experian"].ToString());
                            table.Rows[2].SetField(str.Trim(), dataRow["Equifax"].ToString());
                        }

                        count++;

                    }
                    ds.Tables.Add(table);
                    i++;
                }
            }

            return ds;
        }
        public string RefreshCreditReport(List<AccountHistory> credit, List<Inquires> inquires, List<MonthlyPayStatusHistory> monthlyPayStatusHistoryDetails, string clientId, string mode = "", string round = "")
        {
            string ReportId = "";
            //object res = 0;
            bool status = true;
            try
            {
                //status = cd.checkDateReport(clientId);

                if (status == true)
                {
                    ReportId = cd.RefreshCreditReport(credit, inquires, monthlyPayStatusHistoryDetails, clientId, mode, round);
                    //  ReportId = cd.AddCreditReport(credit, inquires, monthlyPayStatusHistoryDetails, clientId, mode, round);
                }
                else
                {
                    ReportId = "report";
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return ReportId;
        }

        public string AddCreditReport(List<AccountHistory> credit, List<Inquires> inquires, List<MonthlyPayStatusHistory> monthlyPayStatusHistoryDetails, string clientId, string mode = "", string from = "")        {            string ReportId = "";
            //object res = 0;
            bool status = false;            string[] round = null;            string roundtype = string.Empty;            try            {                roundtype = cd.checkLastDateReport(clientId);                round = cd.GetRoundType(clientId);                status = cd.checkDateReport(clientId);                if (status == true || roundtype != "")                {                    if (round[0] != "Third Round")                    {                        ReportId = cd.AddCreditReport(credit, inquires, monthlyPayStatusHistoryDetails, clientId, mode);                        DBUtilities dBUtilities = new DBUtilities();                        string sql = "UPDATE CreditReportData set CreditReportId=" + ReportId + " Where CreditReportId is null";                        dBUtilities.ExecuteString(sql, true);                    }                }                else                {                    ReportId = "report";                }            }            catch (Exception ex) { ex.insertTrace(""); }            return ReportId;        }
        public bool AddReportItemChallenges(CreditReportItems credit, int sno, int clientid)
        {
            try
            {
                //
                object ChallengeText = "";                
                string sql2 = "Select AccTypeId from AccountTypes where AccountType = '" + credit.AccountType + "'";
                
                object AccountTypeId = utilities.ExecuteScalar(sql2, true);                if (AccountTypeId != null && sno != 0)
                {
                    sql2 = "";
                    sql2 = "Select ChallengeText from ChallengeMaster where AccountTypeId = '" + AccountTypeId + "' and ChallengeLevel='" + sno + "'";
                    ChallengeText = utilities.ExecuteScalar(sql2, true);

                    if (ChallengeText==null) {                       
                        sql2 = "";
                        sql2 = "Select max(ChallengeLevel) from ChallengeMaster where AccountTypeId = '"+ AccountTypeId + "' ";
                        var  Challengelevelid = utilities.ExecuteScalar(sql2, true);
                        sql2 = "";
                        sql2 = "Select ChallengeText from ChallengeMaster where AccountTypeId = '" + AccountTypeId + "' and ChallengeLevel='" + Challengelevelid + "'";
                        ChallengeText = utilities.ExecuteScalar(sql2, true);
                    }
                }
                //


                CreditReportItems creditReportItems = cd.GetCreditReportItems(credit.CredRepItemsId.ToString())[0];
                string sql = string.Empty;
                sql = "Insert Into CreditReportItemChallenges (CredRepItemsId,ChallengeText,Status,MerchantName,AccountId,Agency,RoundType,sno,clientid) "
                    + " values(@CredRepItemsId,@ChallengeText,@Status,@MerchantName,@AccountId,@Agency,@RoundType," + sno + "," + clientid + ")";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@CredRepItemsId", credit.CredRepItemsId);
                
                credit.Challenge = ChallengeText.ToString();
                if (sno == 1)
                {
                    creditReportItems.RoundType = "Round-"+ sno;
                }
                else if (sno > 1) { creditReportItems.RoundType = "Round-"+ sno; }
               
                cmd.Parameters.AddWithValue("@ChallengeText", credit.Challenge);
                cmd.Parameters.AddWithValue("@Status", creditReportItems.Status);
                cmd.Parameters.AddWithValue("@MerchantName", creditReportItems.MerchantName);
                cmd.Parameters.AddWithValue("@AccountId", creditReportItems.AccountId);
                cmd.Parameters.AddWithValue("@Agency", creditReportItems.Agency);
                cmd.Parameters.AddWithValue("@RoundType", creditReportItems.RoundType);

                utilities.ExecuteInsertCommand(cmd, true);
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return true;
        }
        public bool AddReportItemInquiriesChallenges(Inquires Inquires, string round, int sno, int clientid)        {
            object ChallengeText = "";
            try            {

                //
                     
                string sql2 = "Select AccTypeId from AccountTypes where AccountType = '" + Inquires.AccountType + "'";               
                object AccountTypeId = utilities.ExecuteScalar(sql2, true);                if (AccountTypeId != null && sno != 0)
                {
                    sql2 = "";
                    sql2 = "Select ChallengeText from ChallengeMaster where AccountTypeId = '" + AccountTypeId + "' and ChallengeLevel='" + sno + "'";
                    ChallengeText = utilities.ExecuteScalar(sql2, true);

                    if (ChallengeText == null)
                    {
                        sql2 = "";
                        sql2 = "Select max(ChallengeLevel) from ChallengeMaster where AccountTypeId = '" + AccountTypeId + "' ";
                        var Challengelevelid = utilities.ExecuteScalar(sql2, true);
                        sql2 = "";
                        sql2 = "Select ChallengeText from ChallengeMaster where AccountTypeId = '" + AccountTypeId + "' and ChallengeLevel='" + Challengelevelid + "'";
                        ChallengeText = utilities.ExecuteScalar(sql2, true);
                    }
                }
                //

                Inquires cinquires = cd.GetInquires(Inquires.CreditInqId)[0];

                string sql = string.Empty;
                sql = "Insert Into CreditReportItemChallenges (CreditInqId,ChallengeText,Status,MerchantName,Agency,RoundType,sno,clientid,DateOfInquiry) "
                    + " values(@CreditInqId,@ChallengeText,@Status,@MerchantName,@Agency,@RoundType," + sno + "," + clientid + ",@DateOfInquiry)";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
               
                    Inquires.ChallengeText = ChallengeText.ToString();
                if (sno == 1)
                {
                    Inquires.RoundType = "Round-" + sno;
                }
                else if (sno > 1) { Inquires.RoundType = "Round-" + sno; }
                
               
                cmd.Parameters.AddWithValue("@CreditInqId", Inquires.CreditInqId);
                cmd.Parameters.AddWithValue("@ChallengeText", Inquires.ChallengeText);
                cmd.Parameters.AddWithValue("@Status", "");
                cmd.Parameters.AddWithValue("@MerchantName", cinquires.CreditorName);
                cmd.Parameters.AddWithValue("@Agency", cinquires.CreditBureau);
                cmd.Parameters.AddWithValue("@RoundType", Inquires.RoundType);
                cmd.Parameters.AddWithValue("@DateOfInquiry", cinquires.Dateofinquiry);
                utilities.ExecuteInsertCommand(cmd, true);            }            catch (Exception ex) { ex.insertTrace(""); }            return true;        }
        public bool AddInquiresChallenge(Inquires credit, string AgentId = "", string staffId = "")        {            long res = 0;            try            {                string ChallengeText = credit.ChallengeText;                string sql = "Select ChallengeText from ChallengeMaster where ChallengeText = '" + ChallengeText + "'";                dataTable = utilities.GetDataTable(sql, true);                if (dataTable.Rows.Count == 0)                {                    string query = "insert into AgentGenChallenges(AgentId,StaffId,ChallengeLevel,ChallengeText,CreatedDate,Status) values(@AgentId,@StaffId,@ChallengeLevel,@ChallengeText,GetDate(),@Status)";                    SqlCommand command = new SqlCommand();                    command.CommandText = query;                    command.Parameters.AddWithValue("@StaffId", string.IsNullOrEmpty(staffId) ? "" : staffId);                    command.Parameters.AddWithValue("@AgentId", string.IsNullOrEmpty(AgentId) ? "" : AgentId);                    command.Parameters.AddWithValue("@ChallengeLevel", 1);                    command.Parameters.AddWithValue("@ChallengeText", ChallengeText);                    command.Parameters.AddWithValue("@Status", 1);                    res = utilities.ExecuteInsertCommand(command, true);                }            }            catch (Exception ex) { /*ex.insertTrace("");*/ }            return true;        }
        public List<CreditReportFiles> GetCreditReportsFilesByround(string id, string Round = "")        {            List<CreditReportFiles> CreditReportFiles = new List<CreditReportFiles>();            string sql = "";            try            {                sql = "select* from CreditReportFiles where ClientId = '" + id + "' and RoundType = '" + Round + "'";                dataTable = utilities.GetDataTable(sql, true);                if (dataTable.Rows.Count > 0)                {                    foreach (DataRow row in dataTable.Rows)                    {                        CreditReportFiles.Add(new CreditReportFiles                        {                            CreditRepFileId = row["CreditRepFileId"].ToString(),                            RoundType = row["RoundType"].ToString(),                            ClientId = Convert.ToInt32(row["ClientId"].ToString()),                            CRFilename = row["CRFilename"].ToString()
                        });                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }            return CreditReportFiles;        }
        public List<Inquires> ReportItemInquiresChallenges(int id, string agency = null)        {            List<Inquires> Inquires = new List<Inquires>();            string sql = "";            try            {
                //sql = "select DISTINCT CI.CreditReportId,CI.CreditorName,CI.TypeOfBusiness,CRC.RoundType,CI.Agency"
                //+ " from CreditReport CR INNER JOIN CreditInquiries CI  ON CR.CreditReportId = CI.CreditReportId "
                //+ " LEFT JOIN CreditReportItemChallenges CRC  ON CI.CreditorName = CRC.MerchantName "
                //+ " where CRC.CreditInqId is Not null and CR.ClientId='"+id+"' ORDER BY CI.CreditorName ";

                sql = "select DISTINCT CI.CreditReportId,CI.CreditorName,CI.TypeOfBusiness,CRC.RoundType,CI.Agency"
                + " from CreditReportItemChallenges CRC INNER JOIN CreditInquiries CI  ON CRC.CreditInqId = CI.CreditInqId "
                + " where CRC.CreditInqId is Not null and CRC.ClientId = '"+id+"' ORDER BY CI.CreditorName ";
                if (!string.IsNullOrEmpty(agency))                {                    sql += " and AgencyName='" + agency.ToUpper() + "'";                }                dataTable = utilities.GetDataTable(sql, true);                if (dataTable.Rows.Count > 0)                {                    foreach (DataRow row in dataTable.Rows)                    {                        Inquires.Add(new Inquires                        {                            CreditorName = row["CreditorName"].ToString(),                            TypeofBusiness = row["TypeofBusiness"].ToString(),                            RoundType = row["RoundType"].ToString(),                            CreditBureau = row["Agency"].ToString(),                            
                            //ChallengeText = row["ChallengeText"].ToString(),
                        });                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }            return Inquires;        }
        public Agent GetAgentAddressById(string AgentId)
        {
            string AgentName = string.Empty;
            Agent agent = new Agent();
            DataRow row = null;
            try
            {
                string query = "select PrimaryBusinessAdd1,PrimaryBusinessAdd2,PrimaryBusinessCity,PrimaryBusinessState,PrimaryBusinessZip from Agent where AgentId='" + AgentId + "'";
                SqlCommand cmd = new SqlCommand();
                row = utilities.GetDataRow(query);
                if (row != null)
                {
                    agent.PrimaryBusinessAdd1 = row[0].ToString();
                    agent.PrimaryBusinessAdd2 = row[1].ToString();
                    agent.PrimaryBusinessCity = row[2].ToString();
                    agent.PrimaryBusinessState = row[3].ToString();
                    agent.PrimaryBusinessZip = row[4].ToString();
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return agent;
        }
    }
}
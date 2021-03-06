﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CreditReversal.Models;
using CreditReversal.DAL;
using CreditReversal.Utilities;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

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
                    //+ " ON CRI.CredRepItemsId = CRC.CredRepItemsId where CRC.CredRepItemsId is null and CR.ClientId=" + id;
                    + " ON CRI.CredRepItemsId = CRC.CredRepItemsId where CR.ClientId=" + id;

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
                            RoundType = row["RoundType"].ToString(),
                            LoanStatus = row["LoanStatus"].ToString(),
                            PastDueDays = row["PastDueDays"].ConvertObjectToIntIfNotNull()
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

        public bool AddChallenge(CreditReportItems credit, string AgentId = "", string staffId = "")
        {
            long res = 0;
            try
            {
                string ChallengeText = credit.Challenge;
                if (!string.IsNullOrEmpty(ChallengeText))
                {
                    string sql = "Select ChallengeText from ChallengeMaster where ChallengeText = '" + ChallengeText + "'";
                    dataTable = utilities.GetDataTable(sql, true);
                    if (dataTable.Rows.Count == 0)
                    {
                        string query = "insert into AgentGenChallenges(AgentId,StaffId,ChallengeLevel,ChallengeText,CreatedDate,Status) values(@AgentId,@StaffId,@ChallengeLevel,@ChallengeText,GetDate(),@Status)";
                        SqlCommand command = new SqlCommand();
                        command.CommandText = query;
                        command.Parameters.AddWithValue("@StaffId", string.IsNullOrEmpty(staffId) ? "" : staffId);
                        command.Parameters.AddWithValue("@AgentId", string.IsNullOrEmpty(AgentId) ? "" : AgentId);
                        command.Parameters.AddWithValue("@ChallengeLevel", 1);
                        command.Parameters.AddWithValue("@ChallengeText", string.IsNullOrEmpty(ChallengeText) ? "" : ChallengeText);
                        command.Parameters.AddWithValue("@Status", 1);
                        res = utilities.ExecuteInsertCommand(command, true);
                    }
                }
            }
            catch (Exception ex) { /*ex.insertTrace("");*/ }

            return true;
        }
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

                            MerchantName = row["MerchantName"].ToString(),
                            AccountId = row["AccountId"].ToString(),
                            RoundType = row["RoundType"].ToString(),
                            Agency = row["AgencyName"].ToString(),
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
        public string RefreshCreditReport(List<AccountHistory> credit, List<Inquires> inquires,
            List<MonthlyPayStatusHistory> monthlyPayStatusHistoryDetails, string clientId, string mode = "", string round = "", List<PublicRecord> publicRecords = null)
        {
            string ReportId = "";
            //object res = 0;
            bool status = true;
            try
            {
                //status = cd.checkDateReport(clientId);

                if (status == true)
                {
                    ReportId = cd.RefreshCreditReport(credit, inquires, monthlyPayStatusHistoryDetails, clientId, mode, round, publicRecords);
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

        public string AddCreditReport(List<AccountHistory> credit, List<Inquires> inquires,
            List<MonthlyPayStatusHistory> monthlyPayStatusHistoryDetails, string clientId, string mode = "", string from = "", List<PublicRecord> publicRecords = null)
        {
            string ReportId = "";
            //object res = 0;
            bool status = false;

            string[] round = null;
            string roundtype = string.Empty;
            try
            {
                roundtype = cd.checkLastDateReport(clientId);

                round = cd.GetRoundType(clientId);

                status = cd.checkDateReport(clientId);

                if (status == true || roundtype != "")
                {
                    if (round[0] != "Third Round")
                    {
                        ReportId = cd.AddCreditReport(credit, inquires, monthlyPayStatusHistoryDetails, clientId, mode, "", publicRecords);
                        DBUtilities dBUtilities = new DBUtilities();
                        string sql = "UPDATE CreditReportData set CreditReportId=" + ReportId + " Where CreditReportId is null";
                        dBUtilities.ExecuteString(sql, true);
                    }

                }
                else
                {
                    ReportId = "report";
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return ReportId;
        }
        public bool AddReportItemChallenges(CreditReportItems credit, int sno, int clientid, string PrevItem = null)
        {
            string sql = string.Empty; string AccountType = credit.AccountType;
            try
            {
                int PrevNo = sno;
                if (!string.IsNullOrEmpty(PrevItem))
                {
                    if (Convert.ToBoolean(PrevItem))
                    {
                        PrevNo++;
                    }
                }

                CreditReportItems creditReportItems = cd.GetCreditReportItems(credit.CredRepItemsId.ToString())[0];
                //var opendate1 = creditReportItems.OpenDate.MMDDYYStringToDateTime("MM/dd/yyyy");

                if (credit.PastDueDays > 0 && credit.AccountType.ToUpper() == "EDUCATION" && credit.LoanStatus.ToUpper() == "DEFERMENT")
                {
                    //   PrevNo = PrevNo + 3;
                    AccountType = "Education Deferment";
                }
                //if (credit.AccountType.ToUpper() == "MEDICAL")
                //{
                //    decimal balamt = -1.00m;
                //    string bal = creditReportItems.CurrentBalance;
                //    var opendate = creditReportItems.OpenDate.MMDDYYStringToDateTime("MM/dd/yyyy");
                //    var year = DateTime.Now.Year - opendate.Year;
                //    bal = bal.Replace("$", "");
                //    balamt = Convert.ToDecimal(bal);
                //    if (balamt == 0 && year < 2)
                //    {
                //        AccountType = "Medical Zero Balance";
                //        //     PrevNo = PrevNo + 3;
                //    }
                //    if (year >= 2)
                //    {
                //        AccountType = "Medical Outdated";
                //        //  PrevNo = PrevNo + 5;
                //    }
                //}
                object ChallengeText = "";
                string sql2 = "Select AccTypeId from AccountTypes where upper(AccountType) = '" + AccountType.ToUpper() + "'";
                object AccountTypeId = utilities.ExecuteScalar(sql2, true);
                if (AccountTypeId != null && sno != 0)
                {
                    sql2 = "";
                    sql2 = "Select ChallengeText from ChallengeMaster where AccountTypeId = '" + AccountTypeId + "' and ChallengeLevel='" + PrevNo + "'";
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
                //if agent gave own challenge
                if (string.IsNullOrEmpty(ChallengeText.ToString()))
                {
                    ChallengeText = AccountType;
                }

                sql = "Insert Into CreditReportItemChallenges (CredRepItemsId,ChallengeText,Status,MerchantName,AccountId, "
                    + " Agency,RoundType,sno,clientid,LoanStatus,PastDueDays,DateOfInquiry) "
                    + " values(@CredRepItemsId,@ChallengeText,@Status,@MerchantName,@AccountId,@Agency,@RoundType,"
                    + sno + "," + clientid + ",@LoanStatus,@PastDueDays,@DateOfInquiry)";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@DateOfInquiry", string.IsNullOrEmpty(creditReportItems.OpenDate) ? "" : creditReportItems.OpenDate);
                cmd.Parameters.AddWithValue("@CredRepItemsId", credit.CredRepItemsId);
                cmd.Parameters.AddWithValue("@LoanStatus", string.IsNullOrEmpty(credit.LoanStatus) ? "" : credit.LoanStatus);
                cmd.Parameters.AddWithValue("@PastDueDays", credit.PastDueDays);

                credit.Challenge = ChallengeText.ToString();
                if (sno == 1)
                {
                    creditReportItems.RoundType = "Round-" + sno;
                }
                else if (sno > 1) { creditReportItems.RoundType = "Round-" + sno; }
                cmd.Parameters.AddWithValue("@ChallengeText", string.IsNullOrEmpty(credit.Challenge) ? "" : credit.Challenge);
                cmd.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(creditReportItems.Status) ? "" : creditReportItems.Status);
                cmd.Parameters.AddWithValue("@MerchantName", string.IsNullOrEmpty(creditReportItems.MerchantName) ? creditReportItems.DispMerchantName : creditReportItems.MerchantName);
                cmd.Parameters.AddWithValue("@AccountId", string.IsNullOrEmpty(creditReportItems.AccountId) ? "" : creditReportItems.AccountId);
                cmd.Parameters.AddWithValue("@Agency", string.IsNullOrEmpty(creditReportItems.Agency) ? "" : creditReportItems.Agency);
                cmd.Parameters.AddWithValue("@RoundType", string.IsNullOrEmpty(creditReportItems.RoundType) ? "" : creditReportItems.RoundType);

                utilities.ExecuteInsertCommand(cmd, true);
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return true;
        }
        public bool AddReportItemPRChallenges(PublicRecord publicRecord, string round, int sno, int clientid, string previtem = null)
        {
            object ChallengeText = "";
            try
            {
                int prevno = sno;
                if (!string.IsNullOrEmpty(previtem))
                {
                    if (Convert.ToBoolean(previtem))
                    {
                        prevno++;
                    }
                }
                //

                string sql2 = "Select AccTypeId from AccountTypes where AccountType = '" + publicRecord.AccountType + "'";
                object AccountTypeId = utilities.ExecuteScalar(sql2, true);
                if (AccountTypeId != null && sno != 0)
                {
                    sql2 = "";
                    sql2 = "Select ChallengeText from ChallengeMaster where AccountTypeId = '" + AccountTypeId + "' and ChallengeLevel='" + prevno + "'";
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
                //if agent gave own challenge
                if (string.IsNullOrEmpty(ChallengeText.ToString()))
                {
                    ChallengeText = publicRecord.AccountType;
                }

                PublicRecord cinquires = cd.GetPublicRecords(publicRecord.PublicRecordId)[0];

                string sql = string.Empty;
                sql = "Insert Into CreditReportItemChallenges (PublicRecordId,ChallengeText,Status,MerchantName,Agency,RoundType,sno,clientid,DateOfInquiry) "
                    + " values(@PublicRecordId,@ChallengeText,@Status,@MerchantName,@Agency,@RoundType," + sno + "," + clientid + ",@DateOfInquiry)";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;

                publicRecord.ChallengeText = ChallengeText.ToString();
                if (sno == 1)
                {
                    publicRecord.RoundType = "Round-" + sno;
                }
                else if (sno > 1) { publicRecord.RoundType = "Round-" + sno; }
                cmd.Parameters.AddWithValue("@PublicRecordId", publicRecord.PublicRecordId);
                cmd.Parameters.AddWithValue("@ChallengeText", string.IsNullOrEmpty(publicRecord.ChallengeText) ? "" : publicRecord.ChallengeText);
                cmd.Parameters.AddWithValue("@Status", "");
                cmd.Parameters.AddWithValue("@MerchantName", string.IsNullOrEmpty(cinquires.atcourtName) ? "" : cinquires.atcourtName);
                cmd.Parameters.AddWithValue("@Agency", string.IsNullOrEmpty(cinquires.atbureau) ? "" : cinquires.atbureau);
                cmd.Parameters.AddWithValue("@RoundType", publicRecord.RoundType);
                cmd.Parameters.AddWithValue("@DateOfInquiry", string.IsNullOrEmpty(cinquires.atdateFiled) ? "" : cinquires.atdateFiled);
                utilities.ExecuteInsertCommand(cmd, true);
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return true;
        }
        public bool AddReportItemInquiriesChallenges(Inquires Inquires, string round, int sno, int clientid, string previtem = null)
        {
            object ChallengeText = "";
            try
            {
                int prevno = sno;
                if (!string.IsNullOrEmpty(previtem))
                {
                    if (Convert.ToBoolean(previtem))
                    {
                        prevno++;
                    }
                }
                string sql2 = "Select AccTypeId from AccountTypes where AccountType = '" + Inquires.AccountType + "'";
                object AccountTypeId = utilities.ExecuteScalar(sql2, true);
                if (AccountTypeId != null && sno != 0)
                {
                    sql2 = "";
                    sql2 = "Select ChallengeText from ChallengeMaster where AccountTypeId = '" + AccountTypeId + "' and ChallengeLevel='" + prevno + "'";
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
                //if agent gave own challenge
                if (string.IsNullOrEmpty(ChallengeText.ToString()))
                {
                    ChallengeText = Inquires.AccountType;
                }

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


                cmd.Parameters.AddWithValue("@CreditInqId", string.IsNullOrEmpty(Inquires.CreditInqId) ? "0" : Inquires.CreditInqId);
                cmd.Parameters.AddWithValue("@ChallengeText", Inquires.ChallengeText);
                cmd.Parameters.AddWithValue("@Status", "");
                cmd.Parameters.AddWithValue("@MerchantName", string.IsNullOrEmpty(cinquires.CreditorName) ? "" : cinquires.CreditorName);
                cmd.Parameters.AddWithValue("@Agency", string.IsNullOrEmpty(cinquires.CreditBureau) ? "" : cinquires.CreditBureau);
                cmd.Parameters.AddWithValue("@RoundType", Inquires.RoundType);
                cmd.Parameters.AddWithValue("@DateOfInquiry", string.IsNullOrEmpty(cinquires.Dateofinquiry) ? "" : cinquires.Dateofinquiry);
                utilities.ExecuteInsertCommand(cmd, true);
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return true;
        }
        public bool AddInquiresChallenge(Inquires credit, string AgentId = "", string staffId = "")
        {
            long res = 0;
            try
            {
                string ChallengeText = credit.ChallengeText;
                string sql = "Select ChallengeText from ChallengeMaster where ChallengeText = '" + ChallengeText + "'";
                dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count == 0)
                {
                    string query = "insert into AgentGenChallenges(AgentId,StaffId,ChallengeLevel,ChallengeText,CreatedDate,Status) values(@AgentId,@StaffId,@ChallengeLevel,@ChallengeText,GetDate(),@Status)";
                    SqlCommand command = new SqlCommand();
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@StaffId", string.IsNullOrEmpty(staffId) ? "" : staffId);
                    command.Parameters.AddWithValue("@AgentId", string.IsNullOrEmpty(AgentId) ? "" : AgentId);
                    command.Parameters.AddWithValue("@ChallengeLevel", 1);
                    command.Parameters.AddWithValue("@ChallengeText", ChallengeText);
                    command.Parameters.AddWithValue("@Status", 1);
                    res = utilities.ExecuteInsertCommand(command, true);
                }
            }
            catch (Exception ex) { /*ex.insertTrace("");*/ }

            return true;
        }
        public List<CreditReportFiles> GetCreditReportsFilesByround(string id, string Round = "")
        {
            List<CreditReportFiles> CreditReportFiles = new List<CreditReportFiles>();
            string sql = "";
            try
            {
                sql = "select* from CreditReportFiles where ClientId = '" + id + "' and RoundType = '" + Round + "'";
                dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        CreditReportFiles.Add(new CreditReportFiles
                        {
                            CreditRepFileId = row["CreditRepFileId"].ToString(),
                            RoundType = row["RoundType"].ToString(),
                            ClientId = Convert.ToInt32(row["ClientId"].ToString()),
                            CRFilename = row["CRFilename"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return CreditReportFiles;
        }
        public List<Inquires> ReportItemInquiresChallenges(int id, string agency = null)
        {
            List<Inquires> Inquires = new List<Inquires>();
            string sql = "";
            try
            {
                //sql = "select DISTINCT CI.CreditReportId,CI.CreditorName,CI.TypeOfBusiness,CRC.RoundType,CI.Agency"
                //+ " from CreditReport CR INNER JOIN CreditInquiries CI  ON CR.CreditReportId = CI.CreditReportId "
                //+ " LEFT JOIN CreditReportItemChallenges CRC  ON CI.CreditorName = CRC.MerchantName "
                //+ " where CRC.CreditInqId is Not null and CR.ClientId='"+id+"' ORDER BY CI.CreditorName ";

                sql = "select DISTINCT CI.CreditReportId,CI.CreditorName,CI.TypeOfBusiness,CRC.RoundType,CI.Agency"
                + " from CreditReportItemChallenges CRC INNER JOIN CreditInquiries CI  ON CRC.CreditInqId = CI.CreditInqId "
                + " where CRC.CreditInqId is Not null and CRC.ClientId = '" + id + "' ORDER BY CI.CreditorName ";


                if (!string.IsNullOrEmpty(agency))
                {
                    sql += " and AgencyName='" + agency.ToUpper() + "'";
                }
                dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Inquires.Add(new Inquires
                        {
                            CreditorName = row["CreditorName"].ToString(),
                            TypeofBusiness = row["TypeofBusiness"].ToString(),
                            RoundType = row["RoundType"].ToString(),
                            CreditBureau = row["Agency"].ToString(),

                            //ChallengeText = row["ChallengeText"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Inquires;
        }
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

        public CreditReportData GetCreditReportItemsbyReading(IdentityIQInfo IdentityIQInfo)
        {
            List<AccountHistory> accountHistories = new List<AccountHistory>();
            List<Inquires> inquires = new List<Inquires>();
            List<PublicRecord> publicRecords = new List<PublicRecord>();
            CreditReportData creditReportData = new CreditReportData();


            try
            {
                sbrowser sb = new sbrowser();
                string username = IdentityIQInfo.UserName;
                string Password = IdentityIQInfo.Password;
                string SecurityAnswer = IdentityIQInfo.Answer;

                CreditReport cr = sb.pullcredit(username, Password, SecurityAnswer, IdentityIQInfo.ClientId.ToString());
                if(!string.IsNullOrEmpty(cr.errMsg))
                {
                    creditReportData.errMsg = cr.errMsg;
                    return creditReportData;
                }

                List<MonthlyPayStatus> monthlyPayStatusEQ = new List<MonthlyPayStatus>();
                List<MonthlyPayStatus> monthlyPayStatusTU = new List<MonthlyPayStatus>();
                List<MonthlyPayStatus> monthlyPayStatusEX = new List<MonthlyPayStatus>();
                monthlyPayStatusEQ = cr.monthlyPayStatusEQ;
                monthlyPayStatusEX = cr.monthlyPayStatusEX;
                monthlyPayStatusTU = cr.monthlyPayStatusTU;



                string date = "";
                string[] strr;
                string year = "";
                string month = "";
                string dat = "";
                string FormatedDate = "";
                try
                {
                    foreach (var ach in cr.TransUnionParsed)
                    {
                        DateTime aDate = DateTime.Now;
                        AccountHistory ah = new AccountHistory();
                        ah.TradeLineName = ach.commonName;
                        ah.DispMerchantName = ah.Bank = ach.atcreditorName.Replace("&", " And ");

                        if (ach.CollectionTrade != null)
                        {
                            ah.DispMerchantName = !string.IsNullOrEmpty(ach.CollectionTrade.atoriginalCreditor) ? (ach.atcreditorName
                                 + " (Original Creditor: " + ach.CollectionTrade.atoriginalCreditor.Replace("&", " And ") + ")") : ach.atcreditorName;
                        }
                        ah.Bank = string.IsNullOrEmpty(ach.atcreditorName) ? ah.DispMerchantName : ach.atcreditorName.Replace("&", " And ");
                        ah.Account = ach.ataccountNumber;
                        ah.AccountStatus = ach.OpenClosed.atabbreviation;
                        var remark = ach.Remark;
                        if (remark != null)
                        {
                            try
                            {
                                try
                                {
                                    var data1 = JsonConvert.DeserializeObject<Remark>(remark.ToString());
                                    if (data1 != null)
                                    {
                                        ah.AccountComments = data1.RemarkCode.atdescription;
                                    }
                                }
                                catch (Exception)
                                { }

                                if (string.IsNullOrEmpty(ah.AccountComments))
                                {
                                    var data = JsonConvert.DeserializeObject<List<Remark>>(remark.ToString());
                                    if (data != null)
                                    {
                                        string remDesc = string.Empty;
                                        for (int i = 0; i < data.Count; i++)
                                        {
                                            if (string.IsNullOrEmpty(remDesc))
                                            {
                                                remDesc = data[i].RemarkCode.atdescription;
                                            }
                                            else
                                            {
                                                remDesc = remDesc + ", " + data[i].RemarkCode.atdescription;
                                            }

                                        }
                                        ah.AccountComments = remDesc;
                                    }
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                        ah.Agency = ach.atbureau;
                        ah.AccountCondition = ach.AccountCondition.atdescription;
                        try
                        {
                            ah.AccountType = ach.GrantedTrade.CreditType.atabbreviation; //AccountType
                            ah.AccountTypeDetail = ach.GrantedTrade.AccountType.atdescription; //AccountTypeDetail 
                        }
                        catch (Exception)
                        {
                            ah.AccountType = ach.IndustryCode != null ? ach.IndustryCode.atabbreviation : "NA";
                            ah.AccountTypeDetail = ach.IndustryCode.atabbreviation;
                        }
                        if (ah.AccountType == "Unknown")
                        {
                            ah.AccountType = ach.AccountCondition.atdescription;
                            ah.AccountTypeDetail = ach.AccountCondition.atdescription;
                        }
                        //Date formating
                        date = ach.atdateOpened;
                        strr = date.Split('-');
                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.DateOpened = FormatedDate;

                        double balance = Convert.ToDouble(ach.atcurrentBalance);
                        string bal = balance.DoubleToStringIfNotNull();
                        ah.Balance = "$" + bal;

                        double HighCredit = Convert.ToDouble(ach.athighBalance);
                        string HighCre = HighCredit.DoubleToStringIfNotNull();
                        ah.HighCredit = "$" + HighCre;

                        GrantedTrade gt = ach.GrantedTrade;
                        double MonthlyPayment = 0;
                        if (gt != null)
                        {
                            MonthlyPayment = Convert.ToDouble(ach.GrantedTrade.atmonthlyPayment);
                        }

                        string MonthlyPay = MonthlyPayment.DoubleToStringIfNotNull();
                        ah.MonthlyPayment = "$" + MonthlyPay;

                        date = ach.atdateReported;
                        strr = date.Split('-');
                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.LastReported = FormatedDate;
                        ah.PaymentStatus = ach.PayStatus.atdescription;
                        var payStatus = monthlyPayStatusTU.FirstOrDefault(x => x.commonName == ach.commonName);
                        ah.negativeitems = payStatus != null ? payStatus.NegitiveItemsCount : 0;
                        accountHistories.Add(ah);
                    }
                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                }
                try
                {
                    foreach (var ach in cr.ExperianParsed)
                    {
                        TradeLineParsed trBank = new TradeLineParsed();
                        string BankName = string.Empty;
                        var trData = cr.TransUnionParsed;
                        if (trData.Count > 0)
                        {
                             trBank = trData.FirstOrDefault(x => x.commonName.Trim() == ach.commonName.Trim());
                            if (trBank != null)
                            {
                                if (!string.IsNullOrEmpty(trBank.atcreditorName))
                                    BankName = trBank.atcreditorName.Replace("&", " And ");
                                if (trBank.CollectionTrade != null)
                                {
                                    BankName = !string.IsNullOrEmpty(trBank.CollectionTrade.atoriginalCreditor) ? (trBank.atcreditorName
                                    + " (Original Creditor: " + trBank.CollectionTrade.atoriginalCreditor.Replace("&", " And ") + ")") : trBank.atcreditorName;
                                }
                            }
                        }
                        DateTime aDate = DateTime.Now;
                        AccountHistory ah = new AccountHistory();
                        if (string.IsNullOrEmpty(BankName))
                        {
                            ah.DispMerchantName = ach.atcreditorName.Replace("&", " And ");
                            if (ach.CollectionTrade != null)
                            {
                                ah.DispMerchantName = !string.IsNullOrEmpty(ach.CollectionTrade.atoriginalCreditor) ? (ach.atcreditorName
                                 + " (Original Creditor: " + ach.CollectionTrade.atoriginalCreditor.Replace("&", " And ") + ")") : ach.atcreditorName;
                            }
                        }
                        else
                        {
                            ah.DispMerchantName = BankName;
                        }
                        ah.TradeLineName = ach.commonName;
                        ah.Bank = string.IsNullOrEmpty(ach.atcreditorName) ? ah.DispMerchantName : ach.atcreditorName.Replace("&", " And ");
                        ah.Account = ach.ataccountNumber;
                        ah.AccountStatus = ach.OpenClosed.atabbreviation;
                        var remark = ach.Remark;
                        if (remark != null)
                        {
                            try
                            {
                                try
                                {
                                    var data1 = JsonConvert.DeserializeObject<Remark>(remark.ToString());
                                    if (data1 != null)
                                    {
                                        ah.AccountComments = data1.RemarkCode.atdescription;
                                    }
                                }
                                catch (Exception)
                                { }

                                if (string.IsNullOrEmpty(ah.AccountComments))
                                {
                                    var data = JsonConvert.DeserializeObject<List<Remark>>(remark.ToString());
                                    if (data != null)
                                    {
                                        string remDesc = string.Empty;
                                        for (int i = 0; i < data.Count; i++)
                                        {
                                            if (string.IsNullOrEmpty(remDesc))
                                            {
                                                remDesc = data[i].RemarkCode.atdescription;
                                            }
                                            else
                                            {
                                                remDesc = remDesc + ", " + data[i].RemarkCode.atdescription;
                                            }

                                        }
                                        ah.AccountComments = remDesc;
                                    }
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                        ah.AccountCondition = ach.AccountCondition.atdescription;
                        ah.Agency = ach.atbureau;
                        try
                        {
                            ah.AccountType = ach.GrantedTrade.CreditType.atabbreviation; //AccountType
                            ah.AccountTypeDetail = ach.GrantedTrade.AccountType.atdescription; //AccountTypeDetail 
                        }
                        catch (Exception)
                        {
                            ah.AccountType = ach.IndustryCode != null ? ach.IndustryCode.atabbreviation : "NA";
                            ah.AccountTypeDetail = ach.IndustryCode.atabbreviation;
                        }
                        try
                        {
                            if (trBank != null)
                            {
                                if (string.IsNullOrEmpty(ah.AccountType))
                                {
                                    ah.AccountTypeDetail = trBank.GrantedTrade.CreditType.atdescription;
                                }
                                if (string.IsNullOrEmpty(ah.AccountTypeDetail))
                                {
                                    ah.AccountTypeDetail = trBank.GrantedTrade.AccountType.atdescription;
                                }
                            }
                        }
                        catch (Exception)
                        { }
                        if (ah.AccountType == "Unknown")
                        {
                            ah.AccountType = ach.AccountCondition.atdescription;
                            ah.AccountTypeDetail = ach.AccountCondition.atdescription;
                        }
                        //Date formating
                        date = ach.atdateOpened;
                        strr = date.Split('-');
                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.DateOpened = FormatedDate;

                        double balance = Convert.ToDouble(ach.atcurrentBalance);
                        string bal = balance.DoubleToStringIfNotNull();
                        ah.Balance = "$" + bal;

                        double HighCredit = Convert.ToDouble(ach.athighBalance);
                        string HighCre = HighCredit.DoubleToStringIfNotNull();
                        ah.HighCredit = "$" + HighCre;

                        GrantedTrade gt = ach.GrantedTrade;
                        double MonthlyPayment = 0;
                        if (gt != null)
                        {
                            MonthlyPayment = Convert.ToDouble(ach.GrantedTrade.atmonthlyPayment);
                        }
                        string MonthlyPay = MonthlyPayment.DoubleToStringIfNotNull();
                        ah.MonthlyPayment = "$" + MonthlyPay;

                        date = ach.atdateReported;
                        strr = date.Split('-');
                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.LastReported = FormatedDate;
                        ah.PaymentStatus = ach.PayStatus.atdescription;
                        var payStatus = monthlyPayStatusEX.FirstOrDefault(x => x.commonName == ach.commonName );
                        ah.negativeitems = payStatus != null ? payStatus.NegitiveItemsCount : 0;
                        accountHistories.Add(ah);
                    }

                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                }
                try
                {
                    foreach (var ach in cr.EquifaxParsed)
                    {
                        TradeLineParsed trBank = new TradeLineParsed();
                        string BankName = string.Empty;
                        var trData = cr.TransUnionParsed;
                        if (trData.Count > 0)
                        {
                             trBank = trData.FirstOrDefault(x => x.commonName.Trim() == ach.commonName.Trim());
                            if (trBank != null)
                            {
                                if (!string.IsNullOrEmpty(trBank.atcreditorName))
                                    BankName = trBank.atcreditorName.Replace("&", " And ");
                                if (trBank.CollectionTrade != null)
                                {
                                    BankName = !string.IsNullOrEmpty(trBank.CollectionTrade.atoriginalCreditor) ? (trBank.atcreditorName
                                   + " (Original Creditor: " + trBank.CollectionTrade.atoriginalCreditor.Replace("&", " And ") + ")") : trBank.atcreditorName;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(BankName))
                        {
                            var exData = cr.ExperianParsed;
                            var exBank = exData.FirstOrDefault(x => x.commonName.Trim() == ach.commonName.Trim());
                            if (exBank != null)
                            {
                                if (!string.IsNullOrEmpty(exBank.atcreditorName))
                                    BankName = exBank.atcreditorName.Replace("&", " And ");
                                if (exBank.CollectionTrade != null)
                                {
                                    BankName = !string.IsNullOrEmpty(exBank.CollectionTrade.atoriginalCreditor) ? (exBank.atcreditorName
                                    + " (Original Creditor: " + exBank.CollectionTrade.atoriginalCreditor.Replace("&", " And ") + ")") : exBank.atcreditorName;
                                }
                            }
                        }
                        
                        DateTime aDate = DateTime.Now;
                        AccountHistory ah = new AccountHistory();
                        if (string.IsNullOrEmpty(BankName))
                        {
                            ah.DispMerchantName = ach.atcreditorName.Replace("&", " And ");
                            if (ach.CollectionTrade != null)
                            {
                                ah.DispMerchantName = !string.IsNullOrEmpty(ach.CollectionTrade.atoriginalCreditor) ? (ach.atcreditorName
                                    + " (Original Creditor: " + ach.CollectionTrade.atoriginalCreditor.Replace("&", " And ") + ")") : ach.atcreditorName;
                            }
                        }
                        else
                        {
                            ah.DispMerchantName = BankName;
                        }
                        ah.TradeLineName = ach.commonName;
                        ah.Bank = string.IsNullOrEmpty(ach.atcreditorName) ? ah.DispMerchantName : ach.atcreditorName.Replace("&", " And ");

                        ah.Account = ach.ataccountNumber;
                        ah.AccountStatus = ach.OpenClosed.atabbreviation;
                        ah.AccountCondition = ach.AccountCondition.atdescription;
                        var remark = ach.Remark;
                        if (remark != null)
                        {
                            try
                            {
                                try
                                {
                                    var data1 = JsonConvert.DeserializeObject<Remark>(remark.ToString());
                                    if (data1 != null)
                                    {
                                        ah.AccountComments = data1.RemarkCode.atdescription;
                                    }
                                }
                                catch (Exception)
                                {}

                                if (string.IsNullOrEmpty(ah.AccountComments))
                                {
                                    var data = JsonConvert.DeserializeObject<List<Remark>>(remark.ToString());
                                    if (data != null)
                                    {
                                        string remDesc = string.Empty;
                                        for (int i = 0; i < data.Count; i++)
                                        {
                                            if (string.IsNullOrEmpty(remDesc))
                                            {
                                                remDesc = data[i].RemarkCode.atdescription;
                                            }
                                            else
                                            {
                                                remDesc = remDesc + ", " + data[i].RemarkCode.atdescription;
                                            }

                                        }
                                        ah.AccountComments = remDesc;
                                    }
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                        ah.Agency = ach.atbureau;
                        try
                        {
                            ah.AccountType = ach.GrantedTrade.CreditType.atabbreviation; //AccountType
                            ah.AccountTypeDetail = ach.GrantedTrade.AccountType.atdescription; //AccountTypeDetail 
                            

                        }
                        catch (Exception)
                        {
                            ah.AccountType = ach.IndustryCode != null ? ach.IndustryCode.atabbreviation : "NA";
                            ah.AccountTypeDetail = ach.IndustryCode.atabbreviation;
                        }

                        try
                        {
                            if (trBank != null)
                            {
                                if (string.IsNullOrEmpty(ah.AccountType))
                                {
                                    ah.AccountTypeDetail = trBank.GrantedTrade.CreditType.atdescription;
                                }
                                if (string.IsNullOrEmpty(ah.AccountTypeDetail))
                                {
                                    ah.AccountTypeDetail = trBank.GrantedTrade.AccountType.atdescription;
                                }
                            }
                        }
                        catch (Exception)
                        {}

                        if (ah.AccountType == "Unknown")
                        {
                            ah.AccountType = ach.AccountCondition.atdescription;
                            ah.AccountTypeDetail = ach.AccountCondition.atdescription;
                        }
                        //Date formating
                        date = ach.atdateOpened;
                        strr = date.Split('-');
                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.DateOpened = FormatedDate;

                        double balance = Convert.ToDouble(ach.atcurrentBalance);
                        string bal = balance.DoubleToStringIfNotNull();
                        ah.Balance = "$" + bal;

                        double HighCredit = Convert.ToDouble(ach.athighBalance);
                        string HighCre = HighCredit.DoubleToStringIfNotNull();
                        ah.HighCredit = "$" + HighCre;

                        GrantedTrade gt = ach.GrantedTrade;
                        double MonthlyPayment = 0;
                        if (gt != null)
                        {
                            MonthlyPayment = Convert.ToDouble(ach.GrantedTrade.atmonthlyPayment);
                        }
                        string MonthlyPay = MonthlyPayment.DoubleToStringIfNotNull();
                        ah.MonthlyPayment = "$" + MonthlyPay;

                        date = ach.atdateReported;
                        strr = date.Split('-');

                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.LastReported = FormatedDate;
                        ah.PaymentStatus = ach.PayStatus.atdescription;
                        var payStatus = monthlyPayStatusEQ.FirstOrDefault(x => x.commonName == ach.commonName );
                        ah.negativeitems = payStatus != null ? payStatus.NegitiveItemsCount : 0;
                        accountHistories.Add(ah);
                    }
                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                }
                try
                {
                    foreach (var ach in cr.inquiries)
                    {

                        Inquires inquires1 = new Inquires();
                        inquires1.CreditBureau = ach.Inquiry.atbureau;
                        inquires1.CreditorName = ach.Inquiry.atsubscriberName;
                        inquires1.Dateofinquiry = ach.Inquiry.atinquiryDate;
                        inquires1.TypeofBusiness = ach.Inquiry.IndustryCode.atabbreviation;
                        inquires.Add(inquires1);
                    }
                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                }
                try
                {
                    foreach (var ach in cr.PublicRecords)
                    {
                        PublicRecord publicRecord = new PublicRecord();
                        publicRecord.atcourtName = ach.atcourtName;
                        publicRecord.AccountType = ach.Type.atdescription;
                        publicRecord.atdateFiled = ach.atdateFiled;
                        publicRecord.atbureau = ach.atbureau;
                        publicRecords.Add(publicRecord);
                    }
                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                }

                creditReportData.AccHistory = accountHistories;
                creditReportData.inquiryDetails = inquires;
                creditReportData.monthlyPayStatusHistoryDetails = cr.monthlyPayStatusHistoryList;
                creditReportData.PublicRecords = publicRecords;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return creditReportData;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CreditReversal.DAL;
using CreditReversal.Models;
using CreditReversal.Utilities;
using System.Data.SqlClient;
using System.Text;

namespace CreditReversal.BLL
{
    public class DashboardFunctions
    {
        private ClientData clientdata = new ClientData();
        private DBUtilities utilities = new DBUtilities();
        public SessionData sessionData = new SessionData();
        Common common = new Common();
        string nl = '\n'.ToString();
        public int GetTotalStaff(string from = "", string AgentId = null)
        {
            int count = 0;
            DataRow row;
            string sql = "";
            try
            {
                if (from != "")
                {
                    switch (from)
                    {
                        case "admin":
                            sql = "select count(AgentStaffId) as AgentStaffCount from AgentStaff";
                            row = utilities.GetDataRow(sql);
                            count = Convert.ToInt32(row["AgentStaffCount"].ToString());
                            return count;
                        case "agentadmin":
                            sql = "select count(AgentStaffId) as AgentStaffCount from AgentStaff where AgentId=" + AgentId;
                            row = utilities.GetDataRow(sql);
                            count = Convert.ToInt32(row["AgentStaffCount"].ToString());
                            return count;
                        case "agentstaff":
                            break;
                        case "client":
                            break;


                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return count;
        }

        public int GetTotalClients(string from = "", string agentid = null, string staffid = null)
        {
            int count = 0;
            DataRow row;
            string sql = "";
            try
            {
                if (from != "")
                {
                    switch (from)
                    {
                        case "admin":
                            sql = "select count(ClientId) as ClientCount from Client";
                            row = utilities.GetDataRow(sql);
                            count = Convert.ToInt32(row["ClientCount"].ToString());
                            break;
                        case "agentadmin":
                            sql = "select count(ClientId) as ClientCount from Client where status=1 ";
                            if (!string.IsNullOrEmpty(agentid))
                            {
                                sql += " and agentid=" + agentid;

                            }
                            row = utilities.GetDataRow(sql);
                            count = Convert.ToInt32(row["ClientCount"].ToString());
                            return count;

                        case "staff":
                            sql = "select count(ClientId) as ClientCount from Client where status=1 ";

                            if (!string.IsNullOrEmpty(staffid))
                            {
                                sql += " and agentstaffid=" + staffid;
                            }
                            row = utilities.GetDataRow(sql);
                            count = Convert.ToInt32(row["ClientCount"].ToString());
                            return count;
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return count;
        }

        public int GetTotalAgents(string from = "")
        {
            int count = 0;
            DataRow row;
            string sql = "";

            try
            {
                if (from != "")
                {
                    switch (from)
                    {
                        case "admin":
                            sql = "select count(UserId) as AgentCount from Users where userrole='agentadmin' and status=1";
                            row = utilities.GetDataRow(sql);
                            count = Convert.ToInt32(row["AgentCount"].ToString());
                            break;
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return count;
        }
        //public int GetCountChallenges(int id,string agency=null)
        //{
        //    string sql = "";
        //    DataRow row;
        //    int count = 0;
        //    try
        //    {
        //        // sql = "select CRI.* from CreditReport CR INNER JOIN CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId where CR.ClientId='"+id + "'INNER JOIN CreditReportItemChallenges CRC ON CRC.CredRepItemsId = CRI.CredRepItemsId";
        //        sql = "Select Count (CRC.CrdRepItemChallengeId) as ChallengeCount from CreditReport CR INNER JOIN "
        //            +" CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId "
        //            +" LEFT JOIN CreditReportItemChallenges CRC ON CRI.CredRepItemsId = CRC.CredRepItemsId where CR.ClientId=" + id;
        //        if (!string.IsNullOrEmpty(agency))
        //        {
        //            sql += " and AgencyName='" + agency.ToUpper() + "'";
        //        }
        //        row = utilities.GetDataRow(sql);
        //        count = count = Convert.ToInt32(row["ChallengeCount"].ToString());
        //    }
        //    catch (Exception ex) { ex.insertTrace(""); }

        //    return count;
        //}


        public List<CreditReportChallenges> GetChallenges(int id, string agency = null)        {            List<CreditReportChallenges> CRC = new List<CreditReportChallenges>();            string sql = "";            DataTable dt;            DataRow row;            int count = 0;            try            {                sql = "Select AgencyName,CRC.Status,Count(CRC.Status) as ChallengeCount from CreditReport CR " +                      " INNER JOIN CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId  LEFT JOIN  " +                      " CreditReportItemChallenges CRC ON CRI.CredRepItemsId = CRC.CredRepItemsId  " +                      " where CR.ClientId = '" + id + "' and CRC.Status is not null " +                     " Group by AgencyName,CRC.Status";
                //sql = "Select Count(CRC.Status) as TotalStatus,Count (CRC.CrdRepItemChallengeId) as ChallengeCount from CreditReport CR INNER JOIN "
                //    + " CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId "
                //    +" LEFT JOIN CreditReportItemChallenges CRC ON CRI.CredRepItemsId = CRC.CredRepItemsId where CR.ClientId=" + id;
                if (!string.IsNullOrEmpty(agency))                {                    sql += " and AgencyName='" + agency.ToUpper() + "'";                }                dt = utilities.GetDataTable(sql);                foreach (DataRow dr in dt.Rows)
                {
                    CreditReportChallenges crc = new CreditReportChallenges();
                    crc.AgencyName = dr["AgencyName"].ToString();
                    crc.Status = dr["Status"].ToString();
                    crc.Count = Convert.ToInt32(dr["ChallengeCount"].ToString());
                    CRC.Add(crc);

                }
                //row = utilities.GetDataRow(sql);
                // count = Convert.ToInt32(row["ChallengeCount"].ToString());

            }            catch (Exception ex) { ex.insertTrace(""); }            return CRC;        }

        public ClientModel GetClient(string ClientId)        {            ClientModel res = new ClientModel();            try            {                string sql = "SELECT c.FirstName,c.MiddleName,c.LastName,Convert(varchar(15),c.DOB,101)as DOB,u.Password,c.SSN,c.CurrentEmail,c.CurrentPhone,c.DrivingLicense,c.SocialSecCard,c.ProofOfCard,Convert(varchar(15),u.CreatedDate,101)as CreatedDate,c.Address1,c.Address2,c.City,c.State,c.ZipCode,i.Question,i.Answer,i.UserName as uname,i.Password as passwd,c.AgentId,c.AgentStaffId FROM Client c " + nl;                sql += "JOIN Users u ON c.ClientId = u.AgentClientId Left Join IdentityIqInformation i ON c.ClientId=i.ClientId" + nl;                sql += "WHERE c.ClientId= '" + ClientId + "' and u.UserRole='client'";                DataTable dt = utilities.GetDataTable(sql, true);                if (dt.Rows.Count > 0)                {                    DataRow row = dt.Rows[0];

                    res.FirstName = string.IsNullOrEmpty(row["FirstName"].ConvertObjectToStringIfNotNull()) ? "" : row["FirstName"].ConvertObjectToStringIfNotNull();                    res.MiddleName = string.IsNullOrEmpty(row["MiddleName"].ConvertObjectToStringIfNotNull()) ? "" : row["MiddleName"].ConvertObjectToStringIfNotNull();                    res.LastName = row["LastName"].ConvertObjectToStringIfNotNull();                    res.FullName = res.FirstName + " " + res.LastName;                    res.DOB = row["DOB"].ConvertObjectToStringIfNotNull();                    string ssn = common.Decrypt(row["SSN"].ConvertObjectToStringIfNotNull());                    res.MaskedSSN = common.GetMaskSSN(ssn);                    res.SSN = common.Decrypt(row["SSN"].ConvertObjectToStringIfNotNull());                    res.CurrentEmail = row["CurrentEmail"].ConvertObjectToStringIfNotNull();                    res.CurrentPhone = row["CurrentPhone"].ConvertObjectToStringIfNotNull();                    res.sDrivingLicense = row["DrivingLicense"].ConvertObjectToStringIfNotNull();                    res.sSocialSecCard = row["SocialSecCard"].ConvertObjectToStringIfNotNull();                    res.sProofOfCard = row["ProofOfCard"].ConvertObjectToStringIfNotNull();                    res.Password = common.Decrypt(row["Password"].ConvertObjectToStringIfNotNull());                    res.IdQuestion = row["Question"].ConvertObjectToStringIfNotNull();                    res.IdAnswer = row["Answer"].ConvertObjectToStringIfNotNull();                    res.IdUserName = row["uname"].ConvertObjectToStringIfNotNull();                    res.IdPassword = common.Decrypt(row["passwd"].ConvertObjectToStringIfNotNull());                    res.AgentId = row["AgentId"].ConvertObjectToIntIfNotNull();                    res.AgentStaffId = row["AgentStaffId"].ConvertObjectToIntIfNotNull();                    res.CreatedDate = row["CreatedDate"].ConvertObjectToStringIfNotNull();                    res.Address1 = row["Address1"].ConvertObjectToStringIfNotNull();                    res.Address2 = row["Address2"].ConvertObjectToStringIfNotNull();                    res.City = row["City"].ConvertObjectToStringIfNotNull();                    res.State = row["State"].ConvertObjectToStringIfNotNull();                    res.ZipCode = row["ZipCode"].ConvertObjectToStringIfNotNull();                    if (res.Address2 != "")                    {                        res.FullAddress = res.Address1 + ", " + res.Address2;                    }                    else
                    {                        res.FullAddress = res.Address1;                    }                    if (res.ZipCode != "" && res.State != "")                    {                        res.FullAddress2 = res.City + ", " + res.State + ", " + res.ZipCode;                    }                    else                    {                        res.FullAddress2 = res.City;                    }


                    if (res.CreatedDate != null && res.CreatedDate != "")                    {                        DateTime date = Convert.ToDateTime(res.CreatedDate);                        int month = date.Month;                        string cmonth = "";                        string cdate = "";                        if (month < 10)                        {                            cmonth = "0" + month.ToString();                        }                        else                        {                            cmonth = month.ToString();                        }                        int year = date.Year + 1;                        int dte = date.Day;                        if (dte < 10)                        {                            cdate = "0" + dte.ToString();                        }                        else                        {                            cdate = dte.ToString();                        }                        res.ServiceExperiDate = cmonth + "/" + cdate + "/" + year;

                    }

                }            }            catch (Exception ex)            {                ex.insertTrace("");            }            return res;        }


        public CreditReportItems GetCreditFile(string ClientId)
        {
            CreditReportItems res = new CreditReportItems();
            try
            {
                string sql = "Select cr.RoundType, min(Convert(varchar(15),cr.DateReportPulls,101)) as FirstDate , max(Convert(varchar(15),cr.DateReportPulls,101)) PullDate, max(Convert(varchar(15),crc.CreatedDate,101)) ChallengeDate ," + nl;                sql += "CRC.Status,Count(CRI.CredRepItemsId) as NegativeItemsCount, " + nl;                sql += "Convert(varchar(15),DATEADD(DAY, 30, max(cr.DateReportPulls)),101) NextActionDate from CreditReport CR" + nl;
                sql += "INNER JOIN  CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId  LEFT JOIN" + nl;
                sql += "CreditReportItemChallenges CRC ON CRI.CredRepItemsId = CRC.CredRepItemsId" + nl;
                sql += "where CR.ClientId = '" + ClientId + "'  and CRC.Status is not null" + nl;                sql += "Group by  CRC.Status,cr.RoundType";

                DataTable dt = utilities.GetDataTable(sql, true);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    res.FirstDate = row["FirstDate"].ConvertObjectToStringIfNotNull();
                    res.DatePulls = row["PullDate"].ConvertObjectToStringIfNotNull();
                    res.ChallengeCreatedDate = row["ChallengeDate"].ConvertObjectToStringIfNotNull();
                    res.Status = row["Status"].ConvertObjectToStringIfNotNull();
                    res.CredRepItemsId = row["NegativeItemsCount"].ConvertObjectToIntIfNotNull();
                    res.DateReportPulls = row["NextActionDate"].ConvertObjectToStringIfNotNull();
                    res.NegativeItemsCount = getNagativeItemsCount(ClientId, row["RoundType"].ToString());
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return res;
        }

        public string getNagativeItemsCount(string ClientId,string round)
        {
            string items = "";
            try            {

                //string sql = "SELECT  COUNT(negativeitems) FROM CreditReportItems where  negativeitems !=0 and CredReportId in " +
                //    "(select CredReportId from CreditReport where ClientId=" + ClientId + ")";
                string sql = "select distinct AccountNo, Agency from PaymentHistory "
                 +" where ClientId =" + ClientId +" and RoundType = '"+ round + "' and(PHStatus != 'C' AND PHStatus != 'U' AND PHStatus != ' ' and "
                 +" PHStatus != 7 and PHStatus != 9)";
                DataTable dt = utilities.GetDataTable(sql);
                items = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return items;
        }







        #region Credit Report Summary        public List<CreditItems> GetCreditReportChallenges(int id, string agency = null)        {            List<CreditReportItems> EQUIFAXCRI = new List<CreditReportItems>();            List<CreditReportItems> EXPERIANCRI = new List<CreditReportItems>();            List<CreditReportItems> TRANSUNIONCRI = new List<CreditReportItems>();            List<CreditReportItems> CRI = new List<CreditReportItems>();

            List<CreditItems> CreditItems = new List<CreditItems>();            string sql = "";            DataTable dt;            try            {                sql = "SELECT ( STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status)" +                     " FROM CreditReportItems US join CreditReport cr on US.CredRepItemsId=cr.CreditReportId where Agency='EQUIFAX' and CR.ClientId='" + id + "'" +                     "FOR XML PATH('')), 1, 1, ''))" +                    "as 'EQUIFAX' ," +                    "STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status)" +                    " FROM CreditReportItems US join CreditReport cr on US.CredRepItemsId=cr.CreditReportId where Agency='EXPERIAN' and CR.ClientId='" + id + "'" +                    "FOR XML PATH('')), 1, 1, '')" +                    " as 'EXPERIAN'," +                    "(SELECT STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status)" +                     " FROM CreditReportItems US join CreditReport cr on US.CredRepItemsId=cr.CreditReportId where Agency='TRANSUNION' and CR.ClientId='" + id + "'" +                     " FOR XML PATH('')), 1, 1, ''))" +                    "as   'TRANSUNION'";                dt = utilities.GetDataTable(sql);                for (int r = 0; r < dt.Rows.Count; r++)                {

                    string EQUIFAX = dt.Rows[r]["EQUIFAX"].ToString();                    string TRANSUNION = dt.Rows[r]["TRANSUNION"].ToString();                    string EXPERIAN = dt.Rows[r]["EXPERIAN"].ToString();                    string[] strTRANSUNION = TRANSUNION.Split('^');                    string newstrTRANSUNION = strTRANSUNION.ToString();                    string[] strEQUIFAX = EQUIFAX.Split('^');                    string[] strEXPERIAN = EXPERIAN.Split('^');                    for (int j = 0; j < strTRANSUNION.Count(); j++)                    {                        string[] strTRANSUNION1 = strTRANSUNION[j].Split('~');                        string[] strEQUIFAX1 = strEQUIFAX[j].Split('~');                        string[] strEXPERIAN1 = strEXPERIAN[j].Split('~');                        for (int i = 0; i < 8; i++)                        {                            CreditItems CreditItem = new CreditItems();                            CreditItem.Heading = getHeadings()[i];                            CreditItem.EQUIFAX = strEQUIFAX1[i].ToString();                            CreditItem.EXPERIAN = strEXPERIAN1[i].ToString();                            CreditItem.TRANSUNION = strTRANSUNION1[i].ToString();                            if (i == 7)
                            {                                CreditItem.Empty = "Empty";                            }

                            CreditItems.Add(CreditItem);                        }                    }

                }



            }            catch (Exception ex) { ex.insertTrace(""); }            return CreditItems;        }
        public List<string> getHeadings()        {            List<string> Headings = new List<string>();            Headings.Add("Merchant Name/Account");            Headings.Add("Account #");            Headings.Add("Date Opened");            Headings.Add("High Credit");            Headings.Add("Balance");            Headings.Add("Monthly Payment");            Headings.Add("Last Reported");            Headings.Add("Payment Status");            Headings.Add("Challenge");            Headings.Add("CreditreportId");            Headings.Add("Negativeitems");            Headings.Add("");            return Headings;        }

        #endregion

        public bool GetCreditReportStatus(string clientID)
        {
            bool res = false;
            try
            {
                string sql = "select * from CreditReport where ClientId='" + clientID + "'";
                DataTable dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }

            return res;
        }

        public bool GetIdentityStatus(string clientID)
        {
            bool res = false;
            try
            {
                string sql = "select * from IdentityIqInformation where ClientId='" + clientID + "' and Question!='' ";
                DataTable dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }

            return res;
        }
        public List<NewClient> GetActiveClients(int id)
        {
            List<NewClient> newClients = new List<NewClient>();
            CommonModel commonModel = new CommonModel();
            string role = sessionData.GetUserRole();
            try
            {
                string sql = "";
                if (role == "agentadmin")
                {
                    sql = "select * from client where status=1 and AgentId='" + id + "' order by clientId DESC";
                }
                else if (role == "agentstaff")
                {
                    sql = "select * from client where status=1 and AgentStaffId='" + id + "' order by clientId DESC";
                }


                DataTable dataTable = utilities.GetDataTable(sql, true);
                int creditreportcount = 0;
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        bool status = GetCreditReportStatus(row["ClientId"].ToString());
                        if (status)
                        {
                            NewClient client = new NewClient();
                            client.ClientId = row["ClientId"].ToString().StringToInt(0);
                            client.Name = row["FirstName"].ToString() + " " + row["LastName"].ToString();
                            client.DOB = Convert.ToDateTime(row["DOB"].ToString());
                            commonModel = GetCreditReportCount(row["ClientId"].ToString());
                            creditreportcount = commonModel.count;
                            switch (creditreportcount)
                            {
                                case 1:
                                    client.CurrentStatus = "First Round";
                                    client.NextAction = "Second Round";
                                    break;
                                case 2:
                                    client.CurrentStatus = "Second Round";
                                    client.NextAction = "Third Round";
                                    break;
                                case 3:
                                    client.CurrentStatus = "Third Round";
                                    client.NextAction = "";
                                    break;
                                case 0:
                                    client.CurrentStatus = "";
                                    client.NextAction = "";
                                    break;

                            }
                            if (creditreportcount > 0)
                            {
                                DateTime date = commonModel.Date;
                                client.DueDate = date.AddDays(30);
                            }
                            else
                            {
                                client.DueDate = null;
                            }

                            newClients.Add(client);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }

            return newClients;
        }

        public CommonModel GetCreditReportCount(string clientID)
        {
            CommonModel commonModel = new CommonModel();
            try
            {
                string sql = "select count(*) as total, max(DateReportPulls) as datereport from CreditReport where ClientId='" + clientID + "' and AgencyName='EQUIFAX'";
                DataRow row = utilities.GetDataRow(sql);
                commonModel.count = row["total"].ToString().StringToInt(0);
                commonModel.Date = Convert.ToDateTime(row["datereport"].ToString());
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }

            return commonModel;
        }
        public List<NewClient> GetClients(int id)
        {
            List<NewClient> newClients = new List<NewClient>();
            string role = sessionData.GetUserRole();
            int AgentId = clientdata.GetAgentIdByStaff(); //GetAgentIdByStaff();
            try
            {
                string sql = "";
                if (role == "agentadmin")
                {
                    sql = "select * from client where status=1 and AgentId=" + id + " order by clientId DESC";
                }
                else if (role == "agentstaff")
                {
                    sql = "select * from client where status=1 and (AgentStaffId=" + id + " Or AgentId=" + AgentId + ") order by clientId DESC";
                }
                DataTable dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        bool status = GetCreditReportStatus(row["ClientId"].ToString());
                        if (!status)
                        {
                            bool res = GetIdentityStatus(row["ClientId"].ToString());
                            newClients.Add(new NewClient
                            {
                                ClientId = row["ClientId"].ToString().StringToInt(0),
                                Name = row["FirstName"].ToString() + " " + row["LastName"].ToString(),
                                DOB = Convert.ToDateTime(row["DOB"].ToString()),
                                SignedUpDate = Convert.ToDateTime(row["CreatedDate"].ToString()),
                                AgentStaffId = Convert.ToInt32(row["AgentStaffId"]),                                AgentId = Convert.ToInt32(row["AgentId"]),
                                IdentityStatus = res
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }

            return newClients;
        }




        #region AgentNamefrmCId        public List<CreditReportItems> GetAgentNamefrmCId(string Cid)        {            List<CreditReportItems> CRI = new List<CreditReportItems>();            string sql = "";            DataTable dt;            try            {                sql = "Select top 1 cr.ClientId,cr.RoundType,a.AgentId,isnull(a.FirstName, '') + ' ' + isnull(a.LastName, '') as Fullname,Convert(varchar(15),cr.DateReportPulls,101)as  DatePulls " +                             "from CreditReport cr  inner join Client c on cr.ClientId = c.ClientId left join agent a on " +                             "a.AgentId = c.AgentId where c.ClientId ='" + Cid + "'";                dt = utilities.GetDataTable(sql);                foreach (DataRow dr in dt.Rows)                {                    CreditReportItems cri = new CreditReportItems();                    cri.DatePulls = dr["DatePulls"].ToString();                    cri.Agent = dr["Fullname"].ToString();                    cri.RoundType = dr["RoundType"].ToString();                    CRI.Add(cri);                }            }            catch (Exception ex)            {                System.Diagnostics.Debug.WriteLine(ex.Message);            }            return CRI;        }        #endregion
        public List<ClientModel> GetStaffsByAgent(string agentID)        {            List<ClientModel> Staffs = new List<ClientModel>();            DataRow row1;            try            {                string sql = "select * from AgentStaff where AgentId='" + agentID + "'";                DataTable dataTable = utilities.GetDataTable(sql, true);                if (dataTable.Rows.Count > 0)                {                    foreach (DataRow row in dataTable.Rows)                    {
                        int AgentStaffId = row["AgentStaffId"].ToString().StringToInt(0);
                        string sql2 = "select count(ClientId) as StaffClientsCount from Client where AgentStaffId='" + AgentStaffId + "'";
                        row1 = utilities.GetDataRow(sql2);                        Staffs.Add(new ClientModel                        {
                            StaffName = row["FirstName"].ToString() + " " + row["LastName"].ToString(),                            AgentStaffId = row["AgentStaffId"].ToString().StringToInt(0),                            ClientsCount = row1["StaffClientsCount"].ToString()


                        });
                    }                }            }            catch (Exception ex)            {                ex.insertTrace("");            }            return Staffs;        }
        public long DeleteClient(string ClientId)        {            long res = 0;            try            {                if (ClientId != "" || ClientId != null)                {                    string sql = "Delete from Client where ClientId='" + ClientId + "'";                    var cmd1 = new SqlCommand();                    cmd1.CommandText = sql;                    res = utilities.ExecuteInsertCommand(cmd1, true);                    if (res > 0)                    {                        sql = "Delete from [Users] where AgentClientId='" + ClientId + "'";                        var cmd = new SqlCommand();                        cmd.CommandText = sql;                        res = utilities.ExecuteInsertCommand(cmd, true);                    }                    return res;                }            }            catch (Exception ex) { ex.insertTrace(""); }            return res;        }
        public List<ActiveClientsBar> GetActiveClientsByStatus(string round = "", string agentid = "")
        {
            List<ActiveClientsBar> activeClients = new List<ActiveClientsBar>();
            List<ActiveClientsBar> clients = new List<ActiveClientsBar>();
            List<ActiveClientsBar> clientsbarmodel = new List<ActiveClientsBar>();
            string role = sessionData.GetUserRole();

            try
            {
                string sql = "";
                if (role == "agentadmin")
                {
                    sql = "Select COUNT(a.ClientId) as ClienstCount, MONTH(a.DateReportPulls) as Month,"
                          + "a.RoundType as Round "
                          + "from CreditReport a, Agent b, Client c "
                          + "where b.AgentId = c.AgentId and a.ClientId = c.ClientId "
                          + "and a.AgencyName = 'EQUIFAX' AND b.AgentId =" + agentid + " AND a.RoundType = '" + round + "' "
                          + "Group by MONTH(a.DateReportPulls),a.RoundType ";
                }
                else if (role == "agentstaff")
                {
                    sql = "Select COUNT(a.ClientId) as ClienstCount, MONTH(a.DateReportPulls) as Month,"
                          + "a.RoundType as Round "
                          + "from CreditReport a, AgentStaff b, Client c "
                          + "where b.AgentStaffId = c.AgentStaffId and a.ClientId = c.ClientId "
                          + "and a.AgencyName = 'EQUIFAX' AND c.AgentStaffId =" + agentid + " AND a.RoundType = '" + round + "' "
                          + "Group by MONTH(a.DateReportPulls),a.RoundType ";

                }

                DataTable dataTable = utilities.GetDataTable(sql, true);

                foreach (DataRow row in dataTable.Rows)
                {
                    ActiveClientsBar clientbar = new ActiveClientsBar();
                    clientbar.clients = row["ClienstCount"].ToString().StringToInt(0);
                    clientbar.Month = row["Month"].ToString().StringToInt(0);
                    activeClients.Add(clientbar);
                }
                IEnumerable<int> months = activeClients.Select(x => x.Month).ToList();
                IEnumerable<int> missing = GetMissing(months);
                foreach (int val in missing)
                {
                    activeClients.Add(new ActiveClientsBar
                    {
                        Month = val,
                        clients = 0
                    });
                }

                clients = activeClients.OrderBy(x => x.Month).ToList();



            }
            catch (Exception ex)
            {

            }
            return clients.OrderBy(x => x.Month).ToList();
        }
        public IEnumerable<int> GetMissing(IEnumerable<int> values)
        {
            HashSet<int> myRange = new HashSet<int>(Enumerable.Range(1, 12));
            myRange.ExceptWith(values);
            return myRange;
        }

        public List<CreditItems> GetCreditReportChallengesAgentBKUP(int id, string agency = null)        {            List<CreditItems> CreditItems = new List<CreditItems>();            string sql = "";            DataTable dt;            try            {
                //sql = "SELECT ( STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status='Delinquent' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId))" +
                //    " FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='EQUIFAX' and CR.ClientId='" + id + "'" +
                //    "FOR XML PATH('')), 1, 1, ''))" +
                //   "as 'EQUIFAX' ," +
                //   "STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status='Delinquent' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId))" +
                //   " FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='EXPERIAN' and CR.ClientId='" + id + "'" +
                //   "FOR XML PATH('')), 1, 1, '')" +
                //   " as 'EXPERIAN'," +
                //   "(SELECT STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status='Delinquent' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId))" +
                //    " FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='TRANSUNION' and CR.ClientId='" + id + "'" +
                //    " FOR XML PATH('')), 1, 1, ''))" +
                //   "as   'TRANSUNION'";

                //sql = "SELECT ( STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status='Delinquent' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId))" +
                //   " FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='EQUIFAX' and CR.ClientId='" + id + "'" +
                //   " and cast(cr.DateReportPulls as date) in (select cast(Max(DateReportPulls) as date) as pulldate  from CreditReport where ClientId = '" + id + "')" +
                //   " FOR XML PATH('')), 1, 1, '')) " +
                //  " as 'EQUIFAX' ," +
                //  " STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status='Delinquent' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId))" +
                //  " FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='EXPERIAN' and CR.ClientId='" + id + "'" +
                //  " and cast(cr.DateReportPulls as date) in (select cast(Max(DateReportPulls) as date) as pulldate  from CreditReport where ClientId = '" + id + "')" +
                //  " FOR XML PATH('')), 1, 1, '')" +
                //  " as 'EXPERIAN'," +
                //  " (SELECT STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status='Delinquent' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId))" +
                //   " FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='TRANSUNION' and CR.ClientId='" + id + "'" +
                //   " and cast(cr.DateReportPulls as date) in (select cast(Max(DateReportPulls) as date) as pulldate  from CreditReport where ClientId = '" + id + "')" +
                //   " FOR XML PATH('')), 1, 1, ''))" +
                //  " as 'TRANSUNION'";


                sql = "SELECT ( STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status LIKE '%LATE%' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId))" +
" FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='EQUIFAX' and CR.ClientId='" + id + "'" +
" and cast(cr.DateReportPulls as date) in (select cast(Max(DateReportPulls) as date) as pulldate  from CreditReport where ClientId = '" + id + "')" +
" FOR XML PATH('')), 1, 1, '')) " +
" as 'EQUIFAX' ," +
" STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status LIKE '%LATE%' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId))" +
" FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='EXPERIAN' and CR.ClientId='" + id + "'" +
" and cast(cr.DateReportPulls as date) in (select cast(Max(DateReportPulls) as date) as pulldate  from CreditReport where ClientId = '" + id + "')" +
" FOR XML PATH('')), 1, 1, '')" +
" as 'EXPERIAN'," +
" (SELECT STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status LIKE '%LATE%' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId))" +
" FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='TRANSUNION' and CR.ClientId='" + id + "'" +
" and cast(cr.DateReportPulls as date) in (select cast(Max(DateReportPulls) as date) as pulldate  from CreditReport where ClientId = '" + id + "')" +
" FOR XML PATH('')), 1, 1, ''))" +
" as 'TRANSUNION'";                dt = utilities.GetDataTable(sql);                for (int r = 0; r < dt.Rows.Count; r++)                {                    string EQUIFAX = dt.Rows[r]["EQUIFAX"].ToString();                    string TRANSUNION = dt.Rows[r]["TRANSUNION"].ToString();                    string EXPERIAN = dt.Rows[r]["EXPERIAN"].ToString();                    string[] strTRANSUNION = TRANSUNION.Split('^');                    string newstrTRANSUNION = strTRANSUNION.ToString();                    string[] strEQUIFAX = EQUIFAX.Split('^');                    string[] strEXPERIAN = EXPERIAN.Split('^');                    for (int j = 0; j < strTRANSUNION.Count(); j++)                    {                        string[] strTRANSUNION1 = strTRANSUNION[j].Split('~');                        string[] strEQUIFAX1 = strEQUIFAX[j].Split('~');                        string[] strEXPERIAN1 = strEXPERIAN[j].Split('~');                        for (int i = 0; i < 10; i++)                        {                            CreditItems CreditItem = new CreditItems();                            CreditItem.Heading = getHeadings()[i];                            CreditItem.EQUIFAX = strEQUIFAX1[i].ToString();                            CreditItem.EXPERIAN = strEXPERIAN1[i].ToString();                            CreditItem.TRANSUNION = strTRANSUNION1[i].ToString();                            if (i == 8)                            {                                CreditItem.Empty = "Empty";                            }                            CreditItems.Add(CreditItem);                        }                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }            return CreditItems;        }
        public List<CreditReportItems> GetCreditReportChallengesAgentById(List<CreditReportItems> credit, int id)        {            List<CreditReportItems> CreditItems = new List<CreditReportItems>();            string sql = "";            DataTable dt;            try            {                for (int i = 0; i < credit.Count; i++)                {                    sql = "select cr.MerchantName,cr.AccountId,cr.OpenDate,cr.HighestBalance,cr.CurrentBalance,cr.MonthlyPayment,cr.LastReported,";                    sql += "cr.Agency,crc.ChallengeText,cr.Status,cr.CredRepItemsId from CreditReportItemChallenges as crc inner join CreditReportItems as cr on crc.CredRepItemsId = cr.CredRepItemsId ";                    sql += "inner join CreditReport as c on c.CreditReportId = cr.CredReportId ";                    sql += "where c.ClientId = '" + id + "' and crc.CredRepItemsId='" + credit[i].CredRepItemsId + "'";                    sql += "order by cr.Agency";                    dt = utilities.GetDataTable(sql);                    foreach (DataRow row in dt.Rows)                    {                        CreditReportItems clientbar = new CreditReportItems();                        clientbar.MerchantName = row["MerchantName"].ToString();                        clientbar.AccountId = row["AccountId"].ToString();                        clientbar.OpenDate = row["OpenDate"].ToString();                        clientbar.HighestBalance = row["HighestBalance"].ToString();                        clientbar.CurrentBalance = row["CurrentBalance"].ToString();                        clientbar.MonthlyPayment = row["MonthlyPayment"].ToString();                        clientbar.LastReported = row["LastReported"].ToString();                        clientbar.Agency = row["Agency"].ToString();                        clientbar.ChallengeText = row["ChallengeText"].ToString();                        clientbar.Status = row["Status"].ToString();                        clientbar.CredRepItemsId = row["CredRepItemsId"].ToString().StringToInt(0);                        CreditItems.Add(clientbar);                    }                }

            }            catch (Exception ex) { ex.insertTrace(""); }            return CreditItems;        }

        public List<CreditItems> GetCreditReportChallengesAgent(int id, string agency = null, string role = "")        {            List<CreditItems> CreditItems = new List<CreditItems>();            string sql = "";            DataTable dt;            DataTable dt1;            try            {
                string addnagitiveitems = "", payhistory = "";
                //payhistory = " isnull((SELECT ' 30 Days Late in ' + convert(varchar,max(convert(datetime,a.phdate)),101)   as pdate FROM PaymentHistory a, "
                //    + " CreditReportItems b, CreditReport c where a.AccountNo = b.AccountId and a.Agency = b.Agency and a.Merchant = b.MerchantName "
                //    + " and b.CredReportId = c.CreditReportId and a.ClientId = c.ClientId and a.RoundType = c.RoundType and a.Agency = c.AgencyName "
                //    + " and(PHStatus != 'C' AND PHStatus != 'U' AND PHStatus != ' ' and PHStatus != 7) and a.AccountNo = US.AccountId  "; 

                payhistory = " isnull((SELECT top 1 convert(varchar,(PHStatus*30)) +' Days late in ' + convert(varchar,FORMAT(max(convert(datetime,a.phdate)), 'MMM-yyyy'),101)   as pdate "
                    +" FROM PaymentHistory a, CreditReportItems b, CreditReport c where a.AccountNo = b.AccountId and a.Agency = b.Agency and a.Merchant = b.MerchantName "
                   + " and b.CredReportId = c.CreditReportId and a.ClientId = c.ClientId and a.RoundType = c.RoundType and a.Agency = c.AgencyName "
                   + " and(PHStatus != 'C' AND PHStatus != 'U' AND PHStatus != ' ' and PHStatus != 7 and PHStatus != 9) and a.AccountNo = US.AccountId  ";
                if (role != "client")
                {
                    addnagitiveitems = "and isnull(US.negativeitems,0) > 0 ";
                }
                else
                {
                    addnagitiveitems = "";
                }


                sql = "SELECT ( STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status LIKE '%LATE%' THEN '1' ELSE '0' END))  +'~'+ Convert(varchar,CredRepItemsId) +'~'+ Convert(varchar,negativeitems) +'~'+ " + payhistory + " and a.Agency='Equifax' group by PayHistoryId, PHStatus order by PayHistoryId asc),''))" +
                      " FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='EQUIFAX' and CR.ClientId='" + id + "'" + addnagitiveitems +

                //if (role== "agentadmin") {
                //  sql += "  and US.negativeitems > 0 "+
                //}

                " and cast(cr.DateReportPulls as date) in (select cast(Max(DateReportPulls) as date) as pulldate  from CreditReport where ClientId = '" + id + "')" +
                      " FOR XML PATH('')), 1, 1, '')) " +
                      " as 'EQUIFAX' ," +
                      " STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status LIKE '%LATE%' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId) +'~'+ Convert(varchar,negativeitems) +'~'+ " + payhistory + " and a.Agency='Experian' group by PayHistoryId, PHStatus order by PayHistoryId asc),''))" +
                      " FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='EXPERIAN' and CR.ClientId='" + id + "'" + addnagitiveitems +
                      //if (role== "agentadmin") {
                      //  sql += "  and US.negativeitems > 0 "+
                      //}


                      " and cast(cr.DateReportPulls as date) in (select cast(Max(DateReportPulls) as date) as pulldate  from CreditReport where ClientId = '" + id + "')" +
                      " FOR XML PATH('')), 1, 1, '')" +
                      " as 'EXPERIAN'," +
                      " (SELECT STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status +'~'+   Convert (varchar,Isnull((Select ChallengeText from CreditReportItemChallenges where CredRepItemsId=US.CredRepItemsId),CASE WHEN Status LIKE '%LATE%' THEN '1' ELSE '0' END)) +'~'+ Convert(varchar,CredRepItemsId) +'~'+ Convert(varchar,negativeitems) +'~'+ " + payhistory + " and a.Agency='TransUnion' group by PayHistoryId, PHStatus order by PayHistoryId asc),''))" +
                      " FROM CreditReportItems US join CreditReport cr on US.CredReportId=cr.CreditReportId where Agency='TRANSUNION' and CR.ClientId='" + id + "'" + addnagitiveitems +
                      //"  and US.negativeitems > 0 " +
                      " and cast(cr.DateReportPulls as date) in (select cast(Max(DateReportPulls) as date) as pulldate  from CreditReport where ClientId = '" + id + "')" +
                      " FOR XML PATH('')), 1, 1, ''))" +
                      " as 'TRANSUNION'";                dt = utilities.GetDataTable(sql);                sql = "";                sql = "select distinct accountId from[CreditRepair].[dbo].[CreditReportItems] where AccountId !='-'";                dt1 = utilities.GetDataTable(sql);                string value = "";                for (int r = 0; r < dt.Rows.Count; r++)                {                    string EQUIFAX = dt.Rows[r]["EQUIFAX"].ToString();                    string TRANSUNION = dt.Rows[r]["TRANSUNION"].ToString();                    string EXPERIAN = dt.Rows[r]["EXPERIAN"].ToString();                    string[] strTRANSUNION = TRANSUNION.Split('^');                    string newstrTRANSUNION = strTRANSUNION.ToString();                    string[] strEQUIFAX = EQUIFAX.Split('^');                    string[] strEXPERIAN = EXPERIAN.Split('^');

                    List<AccountHistory> achquifax = new List<AccountHistory>();                    List<AccountHistory> achexperian = new List<AccountHistory>();                    List<AccountHistory> achtransunion = new List<AccountHistory>();                    if (TRANSUNION != "")
                    {

                        for (int k = 0; k < strTRANSUNION.Length; k++)
                        {
                            string[] strTRANS1 = strTRANSUNION[k].Split('~');
                            AccountHistory ah = new AccountHistory();
                            ah.Bank = strTRANS1[0];
                            ah.Account = strTRANS1[1];
                            ah.DateOpened = strTRANS1[2];
                            ah.HighCredit = strTRANS1[3];
                            ah.Balance = strTRANS1[4];
                            ah.MonthlyPayment = strTRANS1[5];
                            ah.LastReported = strTRANS1[6];
                            ah.PaymentStatus = strTRANS1[7] + "~" + strTRANS1[10] + "~" + strTRANS1[11];
                            ah.Comments = strTRANS1[8];
                            ah.PastDue = strTRANS1[9];
                            ah.negativeitems = strTRANS1[10].StringToInt(0);
                            achtransunion.Add(ah);

                        }                    }                    if (EQUIFAX != "")
                    {
                        for (int k = 0; k < strEQUIFAX.Length; k++)
                        {
                            string[] strEQUIFAX1 = strEQUIFAX[k].Split('~');
                            AccountHistory ah = new AccountHistory();
                            ah.Bank = strEQUIFAX1[0];
                            ah.Account = strEQUIFAX1[1];
                            ah.DateOpened = strEQUIFAX1[2];
                            ah.HighCredit = strEQUIFAX1[3];
                            ah.Balance = strEQUIFAX1[4];
                            ah.MonthlyPayment = strEQUIFAX1[5];
                            ah.LastReported = strEQUIFAX1[6];
                            ah.PaymentStatus = strEQUIFAX1[7] + "~" + strEQUIFAX1[10] + "~" + strEQUIFAX1[11];
                            ah.Comments = strEQUIFAX1[8];
                            ah.PastDue = strEQUIFAX1[9];
                            ah.negativeitems = strEQUIFAX1[10].StringToInt(0);
                            achquifax.Add(ah);

                        }
                    }                    if (EXPERIAN != "")
                    {
                        for (int l = 0; l < strEXPERIAN.Length; l++)
                        {

                            string[] strEXPERIAN1 = strEXPERIAN[l].Split('~');
                            AccountHistory ah = new AccountHistory();
                            ah.Bank = strEXPERIAN1[0];
                            ah.Account = strEXPERIAN1[1];
                            ah.DateOpened = strEXPERIAN1[2];
                            ah.HighCredit = strEXPERIAN1[3];
                            ah.Balance = strEXPERIAN1[4];
                            ah.MonthlyPayment = strEXPERIAN1[5];
                            ah.LastReported = strEXPERIAN1[6];
                            ah.PaymentStatus = strEXPERIAN1[7] + "~" + strEXPERIAN1[10] + "~" + strEXPERIAN1[11];
                            ah.Comments = strEXPERIAN1[8];
                            ah.PastDue = strEXPERIAN1[9];
                            ah.negativeitems = strEXPERIAN1[10].StringToInt(0);
                            achexperian.Add(ah);
                        }
                    }                    for (int x = 0; x < dt1.Rows.Count; x++)                    {                        string accountid = dt1.Rows[x]["accountId"].ToString();
                        for (int i = 0; i < 10; i++)                        {                            CreditItems CreditItem = new CreditItems();                            CreditItem.Heading = getHeadings()[i];                            if (CreditItem.Heading == "Merchant Name/Account")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.Bank;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.Bank;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.Bank;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.Heading == "Account #")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.Account;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.Account;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.Account;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.Heading == "Date Opened")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.DateOpened;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.DateOpened;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.DateOpened;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.Heading == "High Credit")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.HighCredit;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.HighCredit;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.HighCredit;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.Heading == "Balance")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.Balance;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.Balance;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.Balance;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.Heading == "Monthly Payment")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.MonthlyPayment;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.MonthlyPayment;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.MonthlyPayment;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.Heading == "Last Reported")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.LastReported;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.LastReported;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.LastReported;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.Heading == "Payment Status")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.PaymentStatus;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.PaymentStatus;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.PaymentStatus;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.Heading == "Challenge")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.Comments;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.Comments;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.Comments;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.Heading == "CreditreportId")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.PastDue;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.PastDue;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.PastDue;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.Heading == "Negativeitems")                            {                                var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);                                if (equifax != null)                                {                                    CreditItem.EQUIFAX = equifax.PastDue;                                }                                else                                {                                    CreditItem.EQUIFAX = "-";                                }                                var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);                                if (transunio != null)                                {                                    CreditItem.TRANSUNION = transunio.PastDue;                                }                                else                                {                                    CreditItem.TRANSUNION = "-";                                }                                var experian = achexperian.FirstOrDefault(t => t.Account == accountid);                                if (experian != null)                                {                                    CreditItem.EXPERIAN = experian.PastDue;                                }                                else                                {                                    CreditItem.EXPERIAN = "-";                                }                            }                            if (CreditItem.EXPERIAN == "-" && CreditItem.TRANSUNION == "-" && CreditItem.EQUIFAX == "-")                            {

                            }                            else
                            {                                CreditItems.Add(CreditItem);                            }

                        }                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }            return CreditItems;        }
        public List<Inquires> GetCreditReportInquiresAgent(int id, string agency = null, string role = "")        {            List<Inquires> Inquires = new List<Inquires>();            string sql = "";            DataTable dt;            DataTable dt1;            try            {                sql = "SELECT (" +                " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfBusiness + '~' + CONVERT(varchar,CONVERT(datetime,DateOfInquiry,101),103)" +                " + '~' + isnull(Convert(varchar, ((Select ChallengeText from CreditReportItemChallenges" +                " where CreditInqId = us.CreditInqId))), '-') + '~' + Convert(varchar, US.CreditInqId))" +                " FROM" +                " CreditInquiries US join CreditReport cr on US.CreditReportId = cr.CreditReportId" +                " where Agency = 'EQUIFAX' and CR.ClientId = '" + id + "'" +                " FOR XML PATH('')), 1, 1, '')) " +                " as 'EQUIFAX'," +                " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfBusiness + '~' + CONVERT(varchar,CONVERT(datetime,DateOfInquiry,101),103)" +                " + '~' + isnull(Convert(varchar, ((Select ChallengeText from CreditReportItemChallenges" +                " where CreditInqId = us.CreditInqId))), '-') + '~' + Convert(varchar, US.CreditInqId))" +                " FROM" +                " CreditInquiries US join CreditReport cr on US.CreditReportId = cr.CreditReportId" +                " where Agency = 'EXPERIAN' and CR.ClientId = '" + id + "'" +                " FOR XML PATH('')), 1, 1, '')" +                " as 'EXPERIAN'," +                " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfBusiness + '~' + CONVERT(varchar,CONVERT(datetime,DateOfInquiry,101),103)" +                " + '~' + isnull(Convert(varchar, ((Select ChallengeText from CreditReportItemChallenges" +                " where CreditInqId = us.CreditInqId))), '-') + '~' + Convert(varchar, US.CreditInqId))" +                " FROM" +                " CreditInquiries US join CreditReport cr on US.CreditReportId = cr.CreditReportId" +                " where Agency = 'TRANSUNION' and CR.ClientId = '" + id + "'" +                " FOR XML PATH('')), 1, 1, '')" +                " as 'TRANSUNION'";                dt = utilities.GetDataTable(sql);                sql = "";                sql = "select distinct CreditorName from CreditInquiries";                dt1 = utilities.GetDataTable(sql);                for (int r = 0; r < dt.Rows.Count; r++)                {                    string EQUIFAX = dt.Rows[r]["EQUIFAX"].ToString();                    string TRANSUNION = dt.Rows[r]["TRANSUNION"].ToString();                    string EXPERIAN = dt.Rows[r]["EXPERIAN"].ToString();                    string[] strTRANSUNION = TRANSUNION.Split('^');                    string newstrTRANSUNION = strTRANSUNION.ToString();                    string[] strEQUIFAX = EQUIFAX.Split('^');                    string[] strEXPERIAN = EXPERIAN.Split('^');                    List<Inquires> inqquifax = new List<Inquires>();                    List<Inquires> inqexperian = new List<Inquires>();                    List<Inquires> inqtransunion = new List<Inquires>();                    if (TRANSUNION != "")                    {                        for (int k = 0; k < strTRANSUNION.Length; k++)                        {                            string[] strTRANS1 = strTRANSUNION[k].Split('~');                            Inquires inq = new Inquires();                            inq.CreditorName = strTRANS1[0];                            inq.TypeofBusiness = strTRANS1[1];                            inq.Dateofinquiry = strTRANS1[2];                            inq.ChallengeText = strTRANS1[3];                            inq.CreditInqId = strTRANS1[4];                            inqtransunion.Add(inq);                        }                    }                    if (EQUIFAX != "")                    {                        for (int k = 0; k < strEQUIFAX.Length; k++)                        {                            string[] strEQUIFAX1 = strEQUIFAX[k].Split('~');                            Inquires inq = new Inquires();                            inq.CreditorName = strEQUIFAX1[0];                            inq.TypeofBusiness = strEQUIFAX1[1];                            inq.Dateofinquiry = strEQUIFAX1[2];                            inq.ChallengeText = strEQUIFAX1[3];                            inq.CreditInqId = strEQUIFAX1[4];                            inqquifax.Add(inq);                        }                    }                    if (EXPERIAN != "")                    {                        for (int l = 0; l < strEXPERIAN.Length; l++)                        {                            string[] strEXPERIAN1 = strEXPERIAN[l].Split('~');                            Inquires inq = new Inquires();                            inq.CreditorName = strEXPERIAN1[0];                            inq.TypeofBusiness = strEXPERIAN1[1];                            inq.Dateofinquiry = strEXPERIAN1[2];                            inq.ChallengeText = strEXPERIAN1[3];                            inq.CreditInqId = strEXPERIAN1[4];                            inqexperian.Add(inq);                        }                    }                    for (int x = 0; x < dt1.Rows.Count; x++)                    {
                        //string accountid = dt1.Rows[x]["accountId"].ToString();
                        for (int i = 0; i < 5; i++)                        {                            Inquires Inq = new Inquires();                            Inq.Heading = getHeadingsForInquires()[i];                            if (Inq.Heading == "Creditor Name")                            {

                                //var equifax = inqquifax.FirstOrDefault(t => t.Account == accountid);
                                if (inqquifax[x].CreditorName != null)                                {                                    Inq.EQUIFAX = inqquifax[x].CreditorName;                                }                                else                                {                                    Inq.EQUIFAX = "-";                                }

                                //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                if (inqtransunion[x].CreditorName != null)                                {                                    Inq.TRANSUNION = inqtransunion[x].CreditorName;                                }                                else                                {                                    Inq.TRANSUNION = "-";                                }

                                //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                if (inqexperian[x].CreditorName != null)                                {                                    Inq.EXPERIAN = inqexperian[x].CreditorName;                                }                                else                                {                                    Inq.EXPERIAN = "-";                                }                            }                            if (Inq.Heading == "Type Of Business")                            {

                                //var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);
                                if (inqquifax[x].TypeofBusiness != null && inqquifax[x].TypeofBusiness != "")                                {                                    Inq.EQUIFAX = inqquifax[x].TypeofBusiness;                                }                                else                                {                                    Inq.EQUIFAX = "-";                                }

                                //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                if (inqtransunion[x].TypeofBusiness != null && inqtransunion[x].TypeofBusiness != "")                                {                                    Inq.TRANSUNION = inqtransunion[x].TypeofBusiness;                                }                                else                                {                                    Inq.TRANSUNION = "-";                                }

                                //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                if (inqexperian[x].TypeofBusiness != null && inqexperian[x].TypeofBusiness != "")                                {                                    Inq.EXPERIAN = inqexperian[x].TypeofBusiness;                                }                                else                                {                                    Inq.EXPERIAN = "-";                                }                            }                            if (Inq.Heading == "Date Of Inquiry")                            {                                if (inqquifax[x].Dateofinquiry != null)                                {                                    Inq.EQUIFAX = inqquifax[x].Dateofinquiry;                                }                                else                                {                                    Inq.EQUIFAX = "-";                                }

                                //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                if (inqtransunion[x].Dateofinquiry != null)                                {                                    Inq.TRANSUNION = inqtransunion[x].Dateofinquiry;                                }                                else                                {                                    Inq.TRANSUNION = "-";                                }

                                //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                if (inqexperian[x].Dateofinquiry != null)                                {                                    Inq.EXPERIAN = inqexperian[x].Dateofinquiry;                                }                                else                                {                                    Inq.EXPERIAN = "-";                                }                            }                            if (Inq.Heading == "Challenge")                            {                                if (inqquifax[x].ChallengeText != null && inqquifax[x].ChallengeText != "-")                                {                                    Inq.EQUIFAX = inqquifax[x].ChallengeText;                                }                                else                                {                                    Inq.EQUIFAX = "-";                                }

                                //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                if (inqtransunion[x].ChallengeText != null && inqtransunion[x].ChallengeText != "-")                                {                                    Inq.TRANSUNION = inqtransunion[x].ChallengeText;                                }                                else                                {                                    Inq.TRANSUNION = "-";                                }

                                //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                if (inqexperian[x].ChallengeText != null && inqexperian[x].ChallengeText != "-")                                {                                    Inq.EXPERIAN = inqexperian[x].ChallengeText;                                }                                else                                {                                    Inq.EXPERIAN = "-";                                }                            }                            if (Inq.Heading == "CreditInqId")                            {                                if (inqquifax[x].CreditInqId != null)                                {                                    Inq.EQUIFAX = inqquifax[x].CreditInqId;                                }                                else                                {                                    Inq.EQUIFAX = "-";                                }

                                //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                if (inqtransunion[x].CreditInqId != null)                                {                                    Inq.TRANSUNION = inqtransunion[x].CreditInqId;                                }                                else                                {                                    Inq.TRANSUNION = "-";                                }

                                //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                if (inqexperian[x].CreditInqId != null)                                {                                    Inq.EXPERIAN = inqexperian[x].CreditInqId;                                }                                else                                {                                    Inq.EXPERIAN = "-";                                }                            }


                            //if (Inq.EXPERIAN == "-" && Inq.TRANSUNION == "-" && Inq.EQUIFAX == "-")
                            //{

                            //}
                            //else
                            //{
                            Inquires.Add(Inq);
                            //}

                        }                    }                }            }            catch (Exception ex) { ex.insertTrace(""); }            return Inquires;        }
        public List<string> getHeadingsForInquires()        {            List<string> Headings = new List<string>();            Headings.Add("Creditor Name");            Headings.Add("Type Of Business");            Headings.Add("Date Of Inquiry");
            Headings.Add("Challenge");
            Headings.Add("CreditInqId");            return Headings;        }
        public List<Inquires> GetInquiriesChallengesAgentById(List<Inquires> credit, int id)        {            List<Inquires> Inquires = new List<Inquires>();            string sql = "";            DataTable dt;            try            {                for (int i = 0; i < credit.Count; i++)                {                    sql = "select ci.CreditorName,ci.TypeOfBusiness,ci.DateOfInquiry,ci.Agency, ";                    sql += "crc.ChallengeText from CreditReportItemChallenges as crc inner join CreditInquiries as ci on crc.CreditInqId = ci.CreditInqId ";                    sql += "inner join CreditReport as c on c.CreditReportId = ci.CreditReportId ";                    sql += "where c.ClientId = '" + id + "' and crc.CreditInqId ='" + credit[i].CreditInqId + "'";                    sql += "order by ci.Agency";                    dt = utilities.GetDataTable(sql);                    foreach (DataRow row in dt.Rows)                    {                        Inquires Inq = new Inquires();                        Inq.CreditorName = row["CreditorName"].ToString();                        Inq.TypeofBusiness = row["TypeofBusiness"].ToString();                        Inq.Dateofinquiry = row["Dateofinquiry"].ToString();                        Inq.CreditBureau = row["Agency"].ToString();                        Inq.ChallengeText = row["ChallengeText"].ToString();                        Inquires.Add(Inq);                    }                }

            }            catch (Exception ex) { ex.insertTrace(""); }            return Inquires;        }
        public long updatechallengefilepath(List<CreditReportFiles> files,int sno)        {            long res = 0;            try            {                StringBuilder sb = new StringBuilder();                for (int i = 0; i < files.Count; i++)                {                    sb.AppendFormat(" insert into CreditReportFiles(CRFilename,RoundType,ClientId,sno) "                        +" values('{0}','{1}',{2},{3})", files[i].CRFilename, files[i].RoundType, files[i].ClientId, sno);                }                var cmd1 = new SqlCommand();                cmd1.CommandText = sb.ToString();                res = utilities.ExecuteInsertCommand(cmd1, true);                return res;            }            catch (Exception ex) { ex.insertTrace(""); }            return res;        }
    }
}
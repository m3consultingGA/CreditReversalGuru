using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using CreditReversal.DAL;
using CreditReversal.Models;
using CreditReversal.Utilities;
using System.Data.SqlClient;
using System.Text;
using System.Globalization;

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


        public List<CreditReportChallenges> GetChallenges(int id, string agency = null)
        {
            List<CreditReportChallenges> CRC = new List<CreditReportChallenges>();
            string sql = "";
            DataTable dt;
            DataRow row;
            int count = 0;
            try
            {
                sql = "Select AgencyName,CRC.Status,Count(CRC.Status) as ChallengeCount from CreditReport CR " +
                      " INNER JOIN CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId  LEFT JOIN  " +
                      " CreditReportItemChallenges CRC ON CRI.CredRepItemsId = CRC.CredRepItemsId  " +
                      " where CR.ClientId = '" + id + "' and CRC.Status is not null " +
                     " Group by AgencyName,CRC.Status";
                //sql = "Select Count(CRC.Status) as TotalStatus,Count (CRC.CrdRepItemChallengeId) as ChallengeCount from CreditReport CR INNER JOIN "
                //    + " CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId "
                //    +" LEFT JOIN CreditReportItemChallenges CRC ON CRI.CredRepItemsId = CRC.CredRepItemsId where CR.ClientId=" + id;
                if (!string.IsNullOrEmpty(agency))
                {
                    sql += " and AgencyName='" + agency.ToUpper() + "'";
                }
                dt = utilities.GetDataTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    CreditReportChallenges crc = new CreditReportChallenges();
                    crc.AgencyName = dr["AgencyName"].ToString();
                    crc.Status = dr["Status"].ToString();
                    crc.Count = Convert.ToInt32(dr["ChallengeCount"].ToString());
                    CRC.Add(crc);

                }
                //row = utilities.GetDataRow(sql);
                // count = Convert.ToInt32(row["ChallengeCount"].ToString());

            }
            catch (Exception ex) { ex.insertTrace(""); }

            return CRC;
        }

        public ClientModel GetClient(string ClientId)
        {
            ClientModel res = new ClientModel();
            try
            {
                string sql = "SELECT c.FirstName,c.MiddleName,c.LastName,Convert(varchar(15),c.DOB,101)as DOB,u.Password,c.SSN,c.CurrentEmail,c.CurrentPhone,c.DrivingLicense,c.SocialSecCard,c.ProofOfCard,Convert(varchar(15),u.CreatedDate,101)as CreatedDate,c.Address1,c.Address2,c.City,c.State,c.ZipCode,i.Question,i.Answer,i.UserName as uname,i.Password as passwd,c.AgentId,c.AgentStaffId FROM Client c " + nl;
                sql += "JOIN Users u ON c.ClientId = u.AgentClientId Left Join IdentityIqInformation i ON c.ClientId=i.ClientId" + nl;
                sql += "WHERE c.ClientId= '" + ClientId + "' and u.UserRole='client'";
                DataTable dt = utilities.GetDataTable(sql, true);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    res.FirstName = string.IsNullOrEmpty(row["FirstName"].ConvertObjectToStringIfNotNull()) ? "" : row["FirstName"].ConvertObjectToStringIfNotNull();
                    res.MiddleName = string.IsNullOrEmpty(row["MiddleName"].ConvertObjectToStringIfNotNull()) ? "" : row["MiddleName"].ConvertObjectToStringIfNotNull();
                    res.LastName = row["LastName"].ConvertObjectToStringIfNotNull();
                    res.FullName = res.FirstName + " " + res.LastName;
                    res.DOB = row["DOB"].ConvertObjectToStringIfNotNull();
                    string ssn = common.Decrypt(row["SSN"].ConvertObjectToStringIfNotNull());
                    res.MaskedSSN = common.GetMaskSSN(ssn);
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
                    res.CreatedDate = row["CreatedDate"].ConvertObjectToStringIfNotNull();
                    res.Address1 = row["Address1"].ConvertObjectToStringIfNotNull();
                    res.Address2 = row["Address2"].ConvertObjectToStringIfNotNull();
                    res.City = row["City"].ConvertObjectToStringIfNotNull();
                    res.State = row["State"].ConvertObjectToStringIfNotNull();
                    res.ZipCode = row["ZipCode"].ConvertObjectToStringIfNotNull();
                    if (res.Address2 != "")
                    {
                        res.FullAddress = res.Address1 + ", " + res.Address2;
                    }
                    else
                    {
                        res.FullAddress = res.Address1;
                    }
                    if (res.ZipCode != "" && res.State != "")
                    {
                        res.FullAddress2 = res.City + ", " + res.State + ", " + res.ZipCode;
                    }
                    else
                    {
                        res.FullAddress2 = res.City;
                    }


                    if (res.CreatedDate != null && res.CreatedDate != "")
                    {
                        DateTime date = Convert.ToDateTime(res.CreatedDate);
                        int month = date.Month;
                        string cmonth = "";
                        string cdate = "";
                        if (month < 10)
                        {
                            cmonth = "0" + month.ToString();
                        }
                        else
                        {
                            cmonth = month.ToString();
                        }
                        int year = date.Year + 1;
                        int dte = date.Day;
                        if (dte < 10)
                        {
                            cdate = "0" + dte.ToString();
                        }
                        else
                        {
                            cdate = dte.ToString();
                        }
                        res.ServiceExperiDate = cmonth + "/" + cdate + "/" + year;

                    }

                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return res;
        }


        public CreditReportItems GetCreditFile(string ClientId)
        {
            CreditReportItems res = new CreditReportItems();
            try
            {
                string sql = "Select crc.RoundType, min(Convert(varchar(15),cr.DateReportPulls,101)) as FirstDate , max(Convert(varchar(15),cr.DateReportPulls,101)) PullDate, max(Convert(varchar(15),crc.CreatedDate,101)) ChallengeDate ," + nl;
                sql += "CRC.Status,Count(CRI.CredRepItemsId) as NegativeItemsCount, " + nl;
                sql += "Convert(varchar(15),DATEADD(DAY, 30, max(cr.DateReportPulls)),101) NextActionDate, " + nl;
                sql += "Convert(varchar(15),DATEADD(DAY, 90, max(cr.DateReportPulls)),101) NextCRGDate from CreditReport CR" + nl;
                sql += "INNER JOIN  CreditReportItems CRI ON CR.CreditReportId = CRI.CredReportId  LEFT JOIN" + nl;
                sql += "CreditReportItemChallenges CRC ON CRI.AccountId = CRC.AccountId" + nl;
                sql += "where CR.ClientId = '" + ClientId + "'  and CRC.Status is not null" + nl;
                sql += "Group by  CRC.Status,crc.RoundType";

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
                    res.NextCRGDate = row["NextCRGDate"].ConvertObjectToStringIfNotNull();
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return res;
        }

        public string getNagativeItemsCount(string ClientId, string round)
        {
            string items = "0";
            try
            {

                //string sql = "SELECT  COUNT(negativeitems) FROM CreditReportItems where  negativeitems !=0 and CredReportId in " +
                //    "(select CredReportId from CreditReport where ClientId=" + ClientId + ")";
                //string sql = "select distinct AccountNo, Agency from PaymentHistory "
                // + " where ClientId =" + ClientId + " and RoundType = '" + round + "' and(PHStatus != 'C' AND PHStatus != 'U' AND PHStatus != ' ' and "
                // + " PHStatus != 7 and PHStatus != 9)";

                //string sql = "SELECT DISTINCT AccountId,Agency from CreditReportItems "
                //+ " WHERE isnull(negativeitems, 0) > 0 AND CredReportId IN(SELECT CreditReportId FROM CreditReport WHERE ClientId =" + ClientId + ") ";

                string sql = "Select Count(*) as itemscount from CreditReportItemChallenges cri, CreditReportItems cr, CreditReport c "
                   + " where cr.CredRepItemsId = cri.CredRepItemsId and c.CreditReportId = cr.CredReportId "
                   + " and c.ClientId =" + ClientId + " and cri.RoundType ='" + round + "'";
                items = utilities.ExecuteScalar(sql, true).ToString();

            }
            catch (Exception ex)
            {
                items = "0";
                ex.insertTrace("");
            }
            return items;
        }
        #region Credit Report Summary
        public List<CreditItems> GetCreditReportChallenges(int id, string agency = null)
        {
            List<CreditReportItems> EQUIFAXCRI = new List<CreditReportItems>();
            List<CreditReportItems> EXPERIANCRI = new List<CreditReportItems>();
            List<CreditReportItems> TRANSUNIONCRI = new List<CreditReportItems>();
            List<CreditReportItems> CRI = new List<CreditReportItems>();

            List<CreditItems> CreditItems = new List<CreditItems>();
            string sql = "";
            DataTable dt;

            try
            {
                sql = "SELECT ( STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status)" +
                     " FROM CreditReportItems US join CreditReport cr on US.CredRepItemsId=cr.CreditReportId where Agency='EQUIFAX' and CR.ClientId='" + id + "'" +
                     "FOR XML PATH('')), 1, 1, ''))" +
                    "as 'EQUIFAX' ," +
                    "STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status)" +
                    " FROM CreditReportItems US join CreditReport cr on US.CredRepItemsId=cr.CreditReportId where Agency='EXPERIAN' and CR.ClientId='" + id + "'" +
                    "FOR XML PATH('')), 1, 1, '')" +
                    " as 'EXPERIAN'," +
                    "(SELECT STUFF((SELECT '^ ' + ( MerchantName + '~'+ AccountId+'~'+OpenDate+'~'+ HighestBalance +'~'+ CurrentBalance +'~'+ MonthlyPayment +'~'+ LastReported+ '~'+Status)" +
                     " FROM CreditReportItems US join CreditReport cr on US.CredRepItemsId=cr.CreditReportId where Agency='TRANSUNION' and CR.ClientId='" + id + "'" +
                     " FOR XML PATH('')), 1, 1, ''))" +
                    "as   'TRANSUNION'";

                dt = utilities.GetDataTable(sql);


                for (int r = 0; r < dt.Rows.Count; r++)
                {

                    string EQUIFAX = dt.Rows[r]["EQUIFAX"].ToString();
                    string TRANSUNION = dt.Rows[r]["TRANSUNION"].ToString();
                    string EXPERIAN = dt.Rows[r]["EXPERIAN"].ToString();
                    string[] strTRANSUNION = TRANSUNION.Split('^');
                    string newstrTRANSUNION = strTRANSUNION.ToString();

                    string[] strEQUIFAX = EQUIFAX.Split('^');
                    string[] strEXPERIAN = EXPERIAN.Split('^');
                    for (int j = 0; j < strTRANSUNION.Count(); j++)
                    {
                        string[] strTRANSUNION1 = strTRANSUNION[j].Split('~');
                        string[] strEQUIFAX1 = strEQUIFAX[j].Split('~');
                        string[] strEXPERIAN1 = strEXPERIAN[j].Split('~');

                        for (int i = 0; i < 8; i++)
                        {
                            CreditItems CreditItem = new CreditItems();
                            CreditItem.Heading = getHeadings()[i];
                            CreditItem.EQUIFAX = strEQUIFAX1[i].ToString();
                            CreditItem.EXPERIAN = strEXPERIAN1[i].ToString();
                            CreditItem.TRANSUNION = strTRANSUNION1[i].ToString();
                            if (i == 7)
                            {
                                CreditItem.Empty = "Empty";
                            }

                            CreditItems.Add(CreditItem);
                        }
                    }

                }



            }
            catch (Exception ex) { ex.insertTrace(""); }

            return CreditItems;
        }
        public List<string> getHeadings()
        {
            List<string> Headings = new List<string>();
            Headings.Add("Merchant Name/Account");  //0
            Headings.Add("Account #"); //1
            Headings.Add("Account Type"); //2
            Headings.Add("Account Type Details"); //3
            Headings.Add("Date Opened"); //4
            Headings.Add("High Credit"); //5
            Headings.Add("Balance"); //6
            Headings.Add("Monthly Payment"); //6
            Headings.Add("Last Reported"); //7
            Headings.Add("LoanStatus"); //8
            Headings.Add("PastDueDays"); //9
            Headings.Add("Payment Status"); //10
            Headings.Add("Challenge"); //11
            Headings.Add("CreditreportId"); //12
            Headings.Add("Negativeitems"); //13
            //Headings.Add("Negativeitems");
            return Headings;
        }

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

        public DataTable GetCreditReport(string clientID)
        {
            bool res = false;
            DataTable dataTable = new DataTable();
            try
            {
                string sql = "select top 1.* from CreditReport where ClientId='" + clientID + "'";
                dataTable = utilities.GetDataTable(sql, true);

            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }

            return dataTable;
        }



        public bool GetIdentityStatus(string clientID)
        {
            bool res = false;
            try
            {
                string sql = "select * from IdentityIqInformation where ClientId=" + clientID + "  and isnull(UserName,'') != ''";
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
        public List<NewClient> GetActiveClientsnew(int id)
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

                DateTime duedate = new DateTime();
                DataTable dataTable = utilities.GetDataTable(sql, true);

                if (dataTable.Rows.Count > 0)
                {
                    sql = "select top 1.* from CreditReport ";
                    DataTable dataTable1 = utilities.GetDataTable(sql, true);
                    string rund = "";

                    foreach (DataRow row in dataTable.Rows)
                    {

                        bool status = GetCreditReportStatus(row["ClientId"].ToString());
                        DataTable dt = GetCreditReport(row["ClientId"].ToString());
                        if (dt.Rows.Count > 0)
                        {
                            NewClient client = new NewClient();
                            client.ClientId = row["ClientId"].ToString().StringToInt(0);
                            client.Name = row["FirstName"].ToString() + " " + row["LastName"].ToString();
                            client.DOB = Convert.ToDateTime(row["DOB"].ToString());

                            rund = dt.Rows[0]["RoundType"].ToString();
                            duedate = Convert.ToDateTime(dt.Rows[0]["DateReportPulls"].ToString());

                            if (rund == "First Round")
                            {
                                client.NextAction = "Second Round";
                                client.CurrentStatus = "First Round";
                            }
                            if (rund == "Second Round")
                            {
                                client.NextAction = "Third Round";
                                client.CurrentStatus = "Second Round";
                            }
                            if (rund == "Third Round")
                            {
                                client.NextAction = "";
                                client.CurrentStatus = "Third Round";
                            }
                            if (rund == "Third Round")
                            {
                                client.DueDate = null;
                            }
                            else
                            {
                                DateTime date = duedate;
                                client.DueDate = date.AddDays(30);
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
                                AgentStaffId = Convert.ToInt32(row["AgentStaffId"]),
                                AgentId = Convert.ToInt32(row["AgentId"]),
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








        #region AgentNamefrmCId
        public List<CreditReportItems> GetAgentNamefrmCId(string Cid)
        {
            List<CreditReportItems> CRI = new List<CreditReportItems>();
            string sql = "";
            DataTable dt;
            try
            {
                sql = "Select top 1 cr.ClientId,cr.RoundType,a.AgentId,isnull(a.FirstName, '') + ' ' + isnull(a.LastName, '') as Fullname,Convert(varchar(15),cr.DateReportPulls,101)as  DatePulls " +
                             "from CreditReport cr  inner join Client c on cr.ClientId = c.ClientId left join agent a on " +
                             "a.AgentId = c.AgentId where c.ClientId ='" + Cid + "'";
                dt = utilities.GetDataTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    CreditReportItems cri = new CreditReportItems();
                    cri.DatePulls = dr["DatePulls"].ToString();
                    cri.Agent = dr["Fullname"].ToString();
                    cri.RoundType = dr["RoundType"].ToString();
                    CRI.Add(cri);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return CRI;
        }
        #endregion
        public List<ClientModel> GetStaffsByAgent(string agentID)
        {
            List<ClientModel> Staffs = new List<ClientModel>();
            DataRow row1;
            try
            {
                string sql = "select * from AgentStaff where AgentId='" + agentID + "'";
                DataTable dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int AgentStaffId = row["AgentStaffId"].ToString().StringToInt(0);
                        string sql2 = "select count(ClientId) as StaffClientsCount from Client where AgentStaffId='" + AgentStaffId + "'";
                        row1 = utilities.GetDataRow(sql2);

                        Staffs.Add(new ClientModel
                        {
                            StaffName = row["FirstName"].ToString() + " " + row["LastName"].ToString(),
                            AgentStaffId = row["AgentStaffId"].ToString().StringToInt(0),
                            ClientsCount = row1["StaffClientsCount"].ToString()


                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }

            return Staffs;
        }
        public long DeleteClient(string ClientId)
        {
            long res = 0;
            try
            {
                if (ClientId != "" || ClientId != null)
                {
                    string sql = "Delete from Client where ClientId='" + ClientId + "'";
                    var cmd1 = new SqlCommand();
                    cmd1.CommandText = sql;
                    res = utilities.ExecuteInsertCommand(cmd1, true);
                    if (res > 0)
                    {
                        sql = "Delete from [Users] where AgentClientId='" + ClientId + "'";
                        var cmd = new SqlCommand();
                        cmd.CommandText = sql;
                        res = utilities.ExecuteInsertCommand(cmd, true);
                    }

                    return res;
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
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
        public List<CreditReportItems> GetCreditReportChallengesAgentById(List<CreditReportItems> credit, int id)
        {
            List<CreditReportItems> CreditItems = new List<CreditReportItems>();
            string sql = "";
            DataTable dt;
            try
            {
                for (int i = 0; i < credit.Count; i++)
                {
                    sql = "select cr.MerchantName,cr.AccountId,cr.OpenDate,cr.HighestBalance,cr.CurrentBalance,cr.MonthlyPayment,cr.LastReported,crc.RoundType, ";
                    sql += "cr.Agency,crc.ChallengeText,cr.Status,cr.CredRepItemsId,cr.AccountTypeDetails from "
                        + " CreditReportItemChallenges as crc inner join CreditReportItems as cr on crc.CredRepItemsId = cr.CredRepItemsId ";
                    sql += "inner join CreditReport as c on c.CreditReportId = cr.CredReportId ";
                    sql += "where c.ClientId = '" + id + "' and crc.sno=(select max(sno) from CreditReportItemChallenges"
                        + " where  Agency=crc.Agency and  AccountId=crc.AccountId and clientid=crc.clientid) and  crc.CredRepItemsId='" + credit[i].CredRepItemsId + "'";
                    sql += "order by cr.Agency";

                    dt = utilities.GetDataTable(sql);
                    foreach (DataRow row in dt.Rows)
                    {
                        CreditReportItems clientbar = new CreditReportItems();
                        clientbar.MerchantName = row["MerchantName"].ToString();
                        clientbar.AccountId = row["AccountId"].ToString();
                        clientbar.OpenDate = row["OpenDate"].ToString();
                        clientbar.HighestBalance = row["HighestBalance"].ToString();
                        clientbar.CurrentBalance = row["CurrentBalance"].ToString();
                        clientbar.MonthlyPayment = row["MonthlyPayment"].ToString();
                        clientbar.LastReported = row["LastReported"].ToString();
                        clientbar.Agency = row["Agency"].ToString();
                        clientbar.ChallengeText = row["ChallengeText"].ToString();
                        clientbar.Status = row["Status"].ToString();
                        clientbar.RoundType = row["RoundType"].ToString();
                        clientbar.CredRepItemsId = row["CredRepItemsId"].ToString().StringToInt(0);
                        clientbar.AccountType = credit[i].AccountType;
                        clientbar.AccountTypeDetail = row["AccountTypeDetails"].ToString();
                        CreditItems.Add(clientbar);
                    }
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return CreditItems;
        }
        public string getAccountType(string clientid, string accountid)
        {
            string res = "";
            try
            {
                string sql = "select AccountTypeDetails from CreditReportItems a, CreditReport b "
                  + " where a.CredReportId = b.CreditReportId and isnull(AccountTypeDetails,'')!= '' AND "
                  + " Agency = 'EQUIFAX' and accountid = '" + accountid + "' and b.ClientId =" + clientid
                  + " union "
                  + " select AccountTypeDetails from CreditReportItems a, CreditReport b "
                  + " where a.CredReportId = b.CreditReportId and isnull(AccountTypeDetails,'')!= '' AND "
                  + " Agency = 'EXPERIAN' and accountid = '" + accountid + "' and b.ClientId =" + clientid
                 + "  union "
                  + " select AccountTypeDetails from CreditReportItems a, CreditReport b "
                 + "  where a.CredReportId = b.CreditReportId and isnull(AccountTypeDetails,'')!= '' AND "
                 + " Agency = 'TRANSUNION' and accountid = '" + accountid + "' and b.ClientId =" + clientid;
                DataTable dt = utilities.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    res = dt.Rows[0]["AccountTypeDetails"].ToString();
                }
            }
            catch (Exception ex)
            { }
            return res;
        }

        public string getChallengeTextPrev(int clientid, string accountno, string agency, int sno)
        {
            string res = "";
            try
            {
                sno = sno - 1;
                string sql = " Select top 1 RoundType+'-'+convert(varchar,ChallengeText) from CreditReportItemChallenges where "
                    + " AccountId='" + accountno.Trim() + "' and Agency='" + agency + "' and clientid=" + clientid + " and sno=" + sno
                     + " order by CrdRepItemChallengeId desc";
                object obj = utilities.ExecuteScalar(sql, true);
                if (obj != null)
                {
                    res = obj.ToString();
                }
            }
            catch (Exception ex)
            { string msg = ex.Message; res = ""; }
            return res;
        }
        public string getINQChallengeTextPrev(int clientid, string merchant, string agency, int sno, string Dateofinquiry)
        {
            string res = "";
            try
            {


                //DateTime date = DateTime.ParseExact(Dateofinquiry, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //string x = Dateofinquiry.ToString("yyyy-MM-dd",0);

                sno = sno - 1;
                string sql = " Select top 1 RoundType+'-'+convert(varchar,ChallengeText) from CreditReportItemChallenges where "
                    + " MerchantName='" + merchant.Trim() + "' and Agency='" + agency + "' and clientid=" + clientid
                    + " and convert(varchar ,convert( datetime , DateOfInquiry,101 ),101) ='" + Dateofinquiry + "' and sno=" + sno
                     + " order by CrdRepItemChallengeId desc";
                object obj = utilities.ExecuteScalar(sql, true);
                if (obj != null)
                {
                    res = obj.ToString();
                }
            }
            catch (Exception ex)
            { string msg = ex.Message; res = ""; }
            return res;
        }
        public string SetStatusForMedical(string AccountType, string AccountTypeDetails, string status = "", string comments = "", string dateopened = "",string balamt="")
        {
            string res = string.Empty;
            try
            {

                if (AccountType.ToUpper().Contains("MEDICAL") || AccountTypeDetails.ToUpper().Contains("MEDICAL")
                    || AccountType.ToUpper().Contains("HEALTH") || AccountTypeDetails.ToUpper().Contains("HEALTH")
                    || comments.ToUpper().Contains("HEALTH") || comments.ToUpper().Contains("HEALTH")
                    || comments.ToUpper().Contains("MEDICAL") || comments.ToUpper().Contains("MEDICAL"))
                {
                    if(!string.IsNullOrEmpty(balamt))
                    {
                        balamt = balamt.Replace("$", "");
                        if (balamt.StringToDouble(0) == 0)
                        {
                         return   res = "Medical Zero Balance. " + status;
                        }
                    }
                    var opendate = dateopened.MMDDYYStringToDateTime("MM/dd/yyyy");
                    var year = DateTime.Now.Year - opendate.Year;
                    if (year >= 2)
                    {
                        res  = " Outdated 2+ years.";
                    }
                    else
                    {
                        res = status;
                    }

                }
                else
                {
                    res = status;
                }
            }
            catch (Exception ex)
            { }
            return res;
        }
        public List<CreditItems> GetCreditReportChallengesAgent(int id, string agency = null, string role = "")
        {

            List<CreditItems> CreditItems = new List<CreditItems>();
            string sql = "";
            DataTable dt;
            DataTable dt1;
            try
            {
                ClientData clientData = new ClientData();
               DataSet ds= clientdata.GetCreditReportItems(id.ToString(), role);
                dt = ds.Tables[0];
                dt1 = ds.Tables[1];

                List<AccountHistory> acHistory = new List<AccountHistory>();
                List<AccountHistory> achquifax = new List<AccountHistory>();
                List<AccountHistory> achexperian = new List<AccountHistory>();
                List<AccountHistory> achtransunion = new List<AccountHistory>();
                for (int r = 0; r < dt.Rows.Count; r++)
                {

                    string EQUIFAX = dt.Rows[r]["EQUIFAX"].ToString();
                    string TRANSUNION = dt.Rows[r]["TRANSUNION"].ToString();
                    string EXPERIAN = dt.Rows[r]["EXPERIAN"].ToString();
                    string[] strTRANSUNION = TRANSUNION.Split('^');
                    string newstrTRANSUNION = strTRANSUNION.ToString();

                    string[] strEQUIFAX = EQUIFAX.Split('^');
                    string[] strEXPERIAN = EXPERIAN.Split('^');
                    

                    if (TRANSUNION != "")
                    {

                        for (int k = 0; k < strTRANSUNION.Length; k++)
                        {
                            string[] strTRANS1 = strTRANSUNION[k].Split('~');
                            AccountHistory ah = new AccountHistory();
                            ah.Agency = "TRANSUNION";
                            ah.Bank = strTRANS1[0];
                            ah.Account = strTRANS1[1];
                            ah.AccountType = strTRANS1[2];
                            ah.AccountTypeDetail = string.IsNullOrEmpty(strTRANS1[3]) ? getAccountType(id.ToString(), strTRANS1[1]) : strTRANS1[3];
                            ah.DateOpened = strTRANS1[4];
                            ah.HighCredit = strTRANS1[5];
                            ah.Balance = strTRANS1[6];
                            ah.MonthlyPayment = strTRANS1[7];
                            ah.LastReported = strTRANS1[8];
                            ah.negativeitems = (strTRANS1[9] == "Coll/Chargeoff" || strTRANS1[9] == "Collection/Chargeoff") ? 1 : strTRANS1[12].StringToInt(0);
                            string challengestatus = "";
                            if (strTRANS1[10] == "0" || strTRANS1[10].Trim() == "")
                            {
                               // challengestatus = getChallengeTextPrev(id, strTRANS1[1], "TRANSUNION", sno);
                            }
                            else
                            {
                                challengestatus = strTRANS1[10];
                            }
                            string pstatus = strTRANS1[9] + "~" + strTRANS1[12] + "~" + strTRANS1[13];


                            ah.PaymentStatus = SetStatusForMedical(strTRANS1[2], strTRANS1[3], pstatus, "", strTRANS1[4], strTRANS1[6]);
                            ah.Comments = string.IsNullOrEmpty(challengestatus) ? strTRANS1[10] : challengestatus;
                            // ah.Comments = ah.Comments + "^" + (string.IsNullOrEmpty(ah.AccountTypeDetail) ? ah.AccountType : ah.AccountTypeDetail) + "^" + strTRANS1[9];
                            ah.Comments = ah.Comments + "^" + strTRANS1[14];
                            ah.PastDue = strTRANS1[11];
                            ah.TradeLineName = strTRANS1[15];
                            ah.ArgumentName = strTRANS1[14];
                            ah.LoanStatus = "";
                            ah.PastDueDays = "";
                            achtransunion.Add(ah);

                        }
                    }
                    if (EQUIFAX != "")
                    {
                        for (int k = 0; k < strEQUIFAX.Length; k++)
                        {
                            string[] strEQUIFAX1 = strEQUIFAX[k].Split('~');
                            AccountHistory ah = new AccountHistory();
                            ah.Agency = "EQUIFAX";
                            ah.Bank = strEQUIFAX1[0];
                            ah.Account = strEQUIFAX1[1];
                            ah.AccountType = strEQUIFAX1[2];
                            // ah.AccountTypeDetail = strEQUIFAX1[3];
                            ah.AccountTypeDetail = string.IsNullOrEmpty(strEQUIFAX1[3]) ? getAccountType(id.ToString(), strEQUIFAX1[1]) : strEQUIFAX1[3];
                            ah.DateOpened = strEQUIFAX1[4];
                            ah.HighCredit = strEQUIFAX1[5];
                            ah.Balance = strEQUIFAX1[6];
                            ah.MonthlyPayment = strEQUIFAX1[7];
                            ah.LastReported = strEQUIFAX1[8];
                            string challengestatus = "";
                            if (strEQUIFAX1[10] == "0" || strEQUIFAX1[10].Trim() == "")
                            {
                            //    challengestatus = getChallengeTextPrev(id, strEQUIFAX1[1], "EQUIFAX", sno);
                            }
                            else
                            {
                                challengestatus = strEQUIFAX1[10];
                            }
                            string pstatus = strEQUIFAX1[9] + "~" + strEQUIFAX1[12] + "~" + strEQUIFAX1[13];
                            ah.PaymentStatus = SetStatusForMedical(strEQUIFAX1[2], strEQUIFAX1[3], pstatus, "", strEQUIFAX1[4], strEQUIFAX1[6]);
                            ah.Comments = string.IsNullOrEmpty(challengestatus) ? strEQUIFAX1[10] : challengestatus;
                           // ah.Comments = ah.Comments + "^" + (string.IsNullOrEmpty(ah.AccountTypeDetail) ? ah.AccountType : ah.AccountTypeDetail) + "^" + strEQUIFAX1[9];
                            ah.Comments = ah.Comments + "^" + strEQUIFAX1[14];
                            ah.PastDue = strEQUIFAX1[11];
                            ah.negativeitems = (strEQUIFAX1[9] == "Coll/Chargeoff" || strEQUIFAX1[9] == "Collection/Chargeoff") ? 1 : strEQUIFAX1[12].StringToInt(0);
                            ah.LoanStatus = "";
                            ah.PastDueDays = "";
                            ah.TradeLineName = strEQUIFAX1[15];
                            ah.ArgumentName = strEQUIFAX1[14];
                            achquifax.Add(ah);

                        }
                    }
                    if (EXPERIAN != "")
                    {
                        for (int l = 0; l < strEXPERIAN.Length; l++)
                        {

                            string[] strEXPERIAN1 = strEXPERIAN[l].Split('~');
                            AccountHistory ah = new AccountHistory();
                            ah.Agency = "EXPERIAN";
                            ah.Bank = strEXPERIAN1[0];
                            ah.Account = strEXPERIAN1[1];
                            ah.AccountType = strEXPERIAN1[2];
                            // ah.AccountTypeDetail = strEXPERIAN1[3];
                            ah.AccountTypeDetail = string.IsNullOrEmpty(strEXPERIAN1[3]) ? getAccountType(id.ToString(), strEXPERIAN1[1]) : strEXPERIAN1[3];
                            ah.DateOpened = strEXPERIAN1[4];
                            ah.HighCredit = strEXPERIAN1[5];
                            ah.Balance = strEXPERIAN1[6];
                            ah.MonthlyPayment = strEXPERIAN1[7];
                            ah.LastReported = strEXPERIAN1[8];
                            string challengestatus = "";
                            if (strEXPERIAN1[10] == "0" || strEXPERIAN1[10].Trim() == "")
                            {
                              //  challengestatus = getChallengeTextPrev(id, strEXPERIAN1[1], "EXPERIAN", sno);
                            }
                            else
                            {
                                challengestatus = strEXPERIAN1[10];
                            }
                            ah.Comments = string.IsNullOrEmpty(challengestatus) ? strEXPERIAN1[10] : challengestatus;
                            //ah.Comments = ah.Comments + "^" + (string.IsNullOrEmpty(ah.AccountTypeDetail) ? ah.AccountType : ah.AccountTypeDetail) + "^" + strEXPERIAN1[9];
                            ah.Comments = ah.Comments + "^" + strEXPERIAN1[14];
                            string pstatus = strEXPERIAN1[9] + "~" + strEXPERIAN1[12] + "~" + strEXPERIAN1[13];
                            ah.PaymentStatus = SetStatusForMedical(strEXPERIAN1[2], strEXPERIAN1[3], pstatus, "", strEXPERIAN1[4], strEXPERIAN1[6]);
                            ah.PastDue = strEXPERIAN1[11];
                            ah.negativeitems = (strEXPERIAN1[9] == "Coll/Chargeoff" || strEXPERIAN1[9] == "Collection/Chargeoff") ? 1 : strEXPERIAN1[12].StringToInt(0);
                            ah.LoanStatus = "";
                            ah.PastDueDays = "";
                            ah.TradeLineName = strEXPERIAN1[15];
                            ah.ArgumentName = strEXPERIAN1[14];
                            achexperian.Add(ah);
                        }
                    }

                }
                    acHistory.AddRange(achquifax); acHistory.AddRange(achexperian); acHistory.AddRange(achtransunion);
                    acHistory = acHistory.OrderBy(x => x.Bank).ThenBy(x => x.Account).ToList();
                    achquifax = achquifax.OrderBy(x => x.Bank).ThenBy(x => x.Account).ToList();
                    achexperian = achexperian.OrderBy(x => x.Bank).ThenBy(x => x.Account).ToList();
                    achtransunion = achtransunion.OrderBy(x => x.Bank).ThenBy(x => x.Account).ToList();
                    for (int x = 0; x < dt1.Rows.Count; x++)
                    {
                        //string accountid = dt1.Rows[x]["AccountId"].ToString();
                        //string Bank = dt1.Rows[x]["DispMerchantName"].ToString();
                        //string DateOpened = dt1.Rows[x]["OpenDate"].ToString();

                    string TradeLineName = dt1.Rows[x]["TradeLineName"].ToString();

                    for (int i = 0; i < 14; i++)
                        {

                            CreditItems CreditItem = new CreditItems();

                            CreditItem.Heading = getHeadings()[i];
                            var equifax = achquifax.FirstOrDefault(t => t.TradeLineName.Trim() == TradeLineName.Trim() );
                            var transunio = achtransunion.FirstOrDefault(t => t.TradeLineName.Trim() == TradeLineName.Trim());
                        var experian = achexperian.FirstOrDefault(t => t.TradeLineName.Trim() == TradeLineName.Trim());

                        if (CreditItem.Heading == "Merchant Name/Account")
                            {
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.Bank;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.Bank;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.Bank;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "Account #")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.Account;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.Account;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.Account;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "Account Type")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.AccountType;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.AccountType;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.AccountType;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "Account Type Details")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.AccountTypeDetail;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.AccountTypeDetail;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.AccountTypeDetail;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "Date Opened")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.DateOpened;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.DateOpened;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.DateOpened;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "High Credit")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.HighCredit;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.HighCredit;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.HighCredit;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "Balance")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.Balance;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }


                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.Balance;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.Balance;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "Monthly Payment")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.MonthlyPayment;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.MonthlyPayment;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.MonthlyPayment;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "Last Reported")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.LastReported;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.LastReported;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.LastReported;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "Payment Status")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.PaymentStatus;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.PaymentStatus;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.PaymentStatus;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "Challenge")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.Comments;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.Comments;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.Comments;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "CreditreportId")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.PastDue;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.PastDue;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.PastDue;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "Negativeitems")
                            {

                               
                                if (equifax != null)
                                {
                                    CreditItem.EQUIFAX = equifax.PastDue;
                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    CreditItem.TRANSUNION = transunio.PastDue;
                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    CreditItem.EXPERIAN = experian.PastDue;
                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }
                            }
                            if (CreditItem.Heading == "LoanStatus" || CreditItem.Heading == "PastDueDays")
                            {
                               
                                if (equifax != null)
                                {
                                    if (equifax.AccountType.ToUpper().Contains("EDUCATION") || equifax.AccountTypeDetail.ToUpper().Contains("EDUCATION")
                                    || equifax.ArgumentName.ToUpper().Contains("EDUCATION"))
                                    {
                                        CreditItem.EQUIFAX = "EDUCATION";
                                    }
                                    else
                                    {
                                        CreditItem.EQUIFAX = "";
                                    }

                                }
                                else
                                {
                                    CreditItem.EQUIFAX = "-";
                                }

                              
                                if (transunio != null)
                                {
                                    if (transunio.AccountType.ToUpper().Contains("EDUCATION") || transunio.AccountTypeDetail.ToUpper().Contains("EDUCATION")
                                      || transunio.ArgumentName.ToUpper().Contains("EDUCATION"))
                                {
                                        CreditItem.TRANSUNION = "EDUCATION";
                                    }
                                    else
                                    {
                                        CreditItem.TRANSUNION = "";
                                    }

                                }
                                else
                                {
                                    CreditItem.TRANSUNION = "-";
                                }

                               
                                if (experian != null)
                                {
                                    if (experian.AccountType.ToUpper().Contains("EDUCATION") || experian.AccountTypeDetail.ToUpper().Contains("EDUCATION")
                                     || experian.ArgumentName.ToUpper().Contains("EDUCATION"))
                                {
                                        CreditItem.EXPERIAN = "EDUCATION";
                                    }
                                    else
                                    {
                                        CreditItem.EXPERIAN = "";
                                    }

                                }
                                else
                                {
                                    CreditItem.EXPERIAN = "-";
                                }


                            }
                            if (CreditItem.EXPERIAN == "-" && CreditItem.TRANSUNION == "-" && CreditItem.EQUIFAX == "-")
                            {

                            }
                            else
                            {
                                CreditItems.Add(CreditItem);
                            }
                        }
                    }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return CreditItems;
        }
        public List<PublicRecord> GetCreditReportPublicRecordsAgent(int id, string agency = null, string role = "")
        {

            List<PublicRecord> publicRecords = new List<PublicRecord>();
            string sql = "";
            DataTable dt;
            DataTable dt1;
            try
            {
                ClientData clientData = new ClientData();
                DataSet ds = clientdata.GetCreditReportPublicRecords(id.ToString(), role);
                dt = ds.Tables[0];
                dt1 = ds.Tables[1];
                for (int r = 0; r < dt.Rows.Count; r++)
                {

                    string EQUIFAX = dt.Rows[r]["EQUIFAX"].ToString();
                    string TRANSUNION = dt.Rows[r]["TRANSUNION"].ToString();
                    string EXPERIAN = dt.Rows[r]["EXPERIAN"].ToString();
                    string[] strTRANSUNION = TRANSUNION.Split('^');
                    string newstrTRANSUNION = strTRANSUNION.ToString();

                    string[] strEQUIFAX = EQUIFAX.Split('^');
                    string[] strEXPERIAN = EXPERIAN.Split('^');

                    List<PublicRecord> inqquifax = new List<PublicRecord>();
                    List<PublicRecord> inqexperian = new List<PublicRecord>();
                    List<PublicRecord> inqtransunion = new List<PublicRecord>();

                    if (TRANSUNION != "")
                    {
                        for (int k = 0; k < strTRANSUNION.Length; k++)
                        {
                            string[] strTRANS1 = strTRANSUNION[k].Split('~');
                            PublicRecord inq = new PublicRecord();
                            inq.atcourtName = strTRANS1[0].Replace("&amp;", "");
                            inq.AccountType = strTRANS1[1];
                            inq.atdateFiled = strTRANS1[2];
                            inq.ChallengeStatus = strTRANS1[3];
                            inq.ChallengeText = strTRANS1[4];
                            inq.PublicRecordId = strTRANS1[5];
                            if (inq.ChallengeText != "NO CHALLENGE")
                            {
                                inqtransunion.Add(inq);
                            }
                        }
                    }
                    if (EQUIFAX != "")
                    {
                        for (int k = 0; k < strEQUIFAX.Length; k++)
                        {
                            string[] strEQUIFAX1 = strEQUIFAX[k].Split('~');
                            PublicRecord inq = new PublicRecord();
                            inq.atcourtName = strEQUIFAX1[0].Replace("&amp;", "");
                            inq.AccountType = strEQUIFAX1[1];
                            inq.atdateFiled = strEQUIFAX1[2];
                            inq.ChallengeStatus = strEQUIFAX1[3];
                            inq.ChallengeText = strEQUIFAX1[4];
                            inq.PublicRecordId = strEQUIFAX1[5];
                            if (inq.ChallengeText != "NO CHALLENGE")
                            {
                                inqquifax.Add(inq);
                            }
                        }
                    }
                    if (EXPERIAN != "")
                    {
                        for (int l = 0; l < strEXPERIAN.Length; l++)
                        {
                            string[] strEXPERIAN1 = strEXPERIAN[l].Split('~');
                            PublicRecord inq = new PublicRecord();
                            inq.atcourtName = strEXPERIAN1[0].Replace("&amp;", "");
                            inq.AccountType = strEXPERIAN1[1];
                            inq.atdateFiled = strEXPERIAN1[2];
                            inq.ChallengeStatus = strEXPERIAN1[3];
                            inq.ChallengeText = strEXPERIAN1[4];
                            inq.PublicRecordId = strEXPERIAN1[5];
                            if (inq.ChallengeText != "NO CHALLENGE")
                            {
                                inqexperian.Add(inq);
                            }
                        }
                    }
                    for (int x = 0; x < dt1.Rows.Count; x++)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            PublicRecord Inq = new PublicRecord();
                            Inq.Heading = getHeadingsForPublicRecords()[i];
                            if (Inq.Heading == "Account Name")
                            {
                                try
                                {
                                    if (inqquifax[x].atcourtName != null)
                                    {
                                        Inq.EQUIFAX = inqquifax[x].atcourtName;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                try
                                {
                                    if (inqtransunion[x].atcourtName != null)
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].atcourtName;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }

                                try
                                {
                                    if (inqexperian[x].atcourtName != null)
                                    {
                                        Inq.EXPERIAN = inqexperian[x].atcourtName;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }

                            }
                            if (Inq.Heading == "Account Type")
                            {

                                //var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);
                                try
                                {
                                    if (inqquifax[x].AccountType != null && inqquifax[x].AccountType != "")
                                    {
                                        Inq.EQUIFAX = inqquifax[x].AccountType;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }

                                try
                                {
                                    //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                    if (inqtransunion[x].AccountType != null && inqtransunion[x].AccountType != "")
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].AccountType;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }

                                try
                                {
                                    //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                    if (inqexperian[x].AccountType != null && inqexperian[x].AccountType != "")
                                    {
                                        Inq.EXPERIAN = inqexperian[x].AccountType;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }

                            }
                            if (Inq.Heading == "Date")
                            {
                                try
                                {
                                    if (inqquifax[x].atdateFiled != null)
                                    {
                                        Inq.EQUIFAX = inqquifax[x].atdateFiled;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }

                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    if (inqtransunion[x].atdateFiled != null)
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].atdateFiled;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }


                                try
                                {
                                    if (inqexperian[x].atdateFiled != null)
                                    {
                                        Inq.EXPERIAN = inqexperian[x].atdateFiled;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                            }

                            if (Inq.Heading == "Challenge Status")
                            {
                                try
                                {
                                    if (inqquifax[x].ChallengeStatus != null)
                                    {
                                        Inq.EQUIFAX = inqquifax[x].ChallengeStatus;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }
                                }
                                catch (Exception)
                                { }


                                try
                                {
                                    if (inqtransunion[x].ChallengeStatus != null)
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].ChallengeStatus;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                { }


                                try
                                {
                                    if (inqexperian[x].ChallengeStatus != null)
                                    {
                                        Inq.EXPERIAN = inqexperian[x].ChallengeStatus;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                            }
                            if (Inq.Heading == "Challenge")
                            {
                                try
                                {
                                    if (inqquifax[x].ChallengeText != null && inqquifax[x].ChallengeText != "-")
                                    {
                                        Inq.EQUIFAX = inqquifax[x].ChallengeText;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }
                                }
                                catch (Exception)
                                { }


                                try
                                {
                                    //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                    if (inqtransunion[x].ChallengeText != null && inqtransunion[x].ChallengeText != "-")
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].ChallengeText;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                    if (inqexperian[x].ChallengeText != null && inqexperian[x].ChallengeText != "-")
                                    {
                                        Inq.EXPERIAN = inqexperian[x].ChallengeText;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                            }
                            if (Inq.Heading == "PublicRecordId")
                            {
                                try
                                {
                                    if (inqquifax[x].PublicRecordId != null)
                                    {
                                        Inq.EQUIFAX = inqquifax[x].PublicRecordId;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                    if (inqtransunion[x].PublicRecordId != null)
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].PublicRecordId;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                    if (inqexperian[x].PublicRecordId != null)
                                    {
                                        Inq.EXPERIAN = inqexperian[x].PublicRecordId;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                            }

                            if (Inq.EXPERIAN == null && Inq.TRANSUNION == null && Inq.EQUIFAX == null)
                            { }
                            else
                            {
                                publicRecords.Add(Inq);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return publicRecords;
        }
        //public List<PublicRecord> GetCreditReportPublicRecordsAgent(int id, string agency = null, string role = "")
        //{

        //    List<PublicRecord> publicRecords = new List<PublicRecord>();
        //    string sql = "";
        //    DataTable dt;
        //    DataTable dt1;
        //    try
        //    {
        //        int sno = 0;
        //        sql = "select top 1 sno from PublicRecords where   "
        //       + " CreditReportId in (Select CreditReportId from CreditReport where  ClientId =" + id + ")  order by PublicRecordId desc";
        //        try
        //        {
        //            sno = utilities.ExecuteScalar(sql, true).ConvertObjectToIntIfNotNull();
        //        }
        //        catch (Exception)
        //        { }

        //        sql = " Select max(a.sno) from CreditReportItemChallenges a,PublicRecords b, CreditReport c   where "
        //            //+ "  a.PublicRecordId=b.PublicRecordId and b.CreditReportId=c.CreditReportId and c.ClientID=" + id;
        //            + "  a.MerchantName=b.CreditorName and UPPER(a.agency)=UPPER(b.agency)  and a.ClientID=" + id
        //            + " and a.PublicRecordId > 0  and b.CreditReportId=c.CreditReportId and c.ClientID=" + id;
        //        long val = 0;
        //        try
        //        {
        //            object _obj = utilities.ExecuteScalar(sql, true);

        //            val = _obj != null ? long.Parse(_obj.ToString()) : 0;
        //        }
        //        catch (Exception ex)
        //        { }

        //        long val1 = 0;
        //        sql = " Select  max(a.sno) from CreditReportItemChallenges a,CreditReportItems b, CreditReport c   where "
        //        //+ " a.CredRepItemsId=b.CredRepItemsId and b.CredReportId=c.CreditReportId and c.ClientID=" + id;
        //        + " a.AccountId=b.AccountId and a.Agency=b.Agency and a.CredRepItemsId > 0 and a.ClientID=c.ClientID "
        //        + " and b.CredReportId=c.CreditReportId and c.ClientID=" + id;
        //        //+ "  b.CredReportId=c.CreditReportId and c.ClientID=" + id;

        //        try
        //        {
        //            object _obj = utilities.ExecuteScalar(sql, true);
        //            val1 = _obj != null ? long.Parse(_obj.ToString()) : 0;
        //        }
        //        catch (Exception ex)
        //        { }

        //        if(val != val1)
        //        {
        //            return null;
        //        }

        //        sql = "SELECT (";

        //        if (val > 0)
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfPR + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfPR,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges "
        //        + " where PublicRecordId=US.PublicRecordId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfPR order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.PublicRecordId))" +
        //        " FROM" +
        //        " PublicRecords US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        "  join CreditReportItemChallenges cri ON cri.MerchantName=US.CreditorName and UPPER(cri.agency)=UPPER(US.agency)  and CRI.ClientID=" + id
        //        + " and CRI.PublicRecordId > 0  and cri.sno =" + val +
        //        " where US.Agency = 'EQUIFAX' and CR.ClientId = '" + id + "' and us.sno=" + sno
        //        + " FOR XML PATH('')), 1, 1, '')) " +
        //        " as 'EQUIFAX',";
        //        }
        //        else
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfPR + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfPR,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges "
        //        + " where PublicRecordId=US.PublicRecordId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfPR order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.PublicRecordId))" +
        //        " FROM" +
        //        " PublicRecords US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        " where US.Agency = 'EQUIFAX' and CR.ClientId = '" + id + "' and us.sno=" + sno
        //        + " FOR XML PATH('')), 1, 1, '')) " +
        //        " as 'EQUIFAX',";
        //        }

        //        if (val > 0)
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfPR + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfPR,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges "
        //        + " where PublicRecordId=US.PublicRecordId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfPR order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.PublicRecordId))" +
        //        " FROM" +
        //        " PublicRecords US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        "  join CreditReportItemChallenges cri ON cri.MerchantName=US.CreditorName and UPPER(cri.agency)=UPPER(US.agency)   and CRI.ClientID=" + id
        //        + " and CRI.PublicRecordId > 0  and cri.sno =" + val +
        //        " where US.Agency = 'EXPERIAN' and CR.ClientId = '" + id + "' and us.sno=" + sno
        //       + " FOR XML PATH('')), 1, 1, '')" +
        //        " as 'EXPERIAN',";
        //        }
        //        else
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfPR + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfPR,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges "
        //        + " where PublicRecordId=US.PublicRecordId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.PublicRecordId order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.PublicRecordId))" +
        //        " FROM" +
        //        " PublicRecords US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        " where US.Agency = 'EXPERIAN' and CR.ClientId = '" + id + "' and us.sno=" + sno
        //       + " FOR XML PATH('')), 1, 1, '')" +
        //        " as 'EXPERIAN',";
        //        }


        //        if (val > 0)
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfPR + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfPR,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges  "
        //        + " where PublicRecordId=US.PublicRecordId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfPR order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.PublicRecordId))" +
        //        " FROM" +
        //        " PublicRecords US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        "  join CreditReportItemChallenges cri ON cri.MerchantName=US.CreditorName and UPPER(cri.agency)=UPPER(US.agency)   and CRI.ClientID=" + id
        //        + " and CRI.PublicRecordId > 0  and cri.sno =" + val +
        //        "  where US.Agency = 'TRANSUNION' and CR.ClientId = '" + id + "' and us.sno=" + sno +
        //        " FOR XML PATH('')), 1, 1, '')" +
        //        " as 'TRANSUNION'";
        //        }
        //        else
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfPR + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfPR,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges  "
        //        + " where PublicRecordId=US.PublicRecordId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfPR order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.PublicRecordId))" +
        //        " FROM" +
        //        " PublicRecords US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        "  where US.Agency = 'TRANSUNION' and CR.ClientId = '" + id + "' and us.sno=" + sno +
        //        " FOR XML PATH('')), 1, 1, '')" +
        //        " as 'TRANSUNION'";
        //        }



        //        dt = utilities.GetDataTable(sql);
        //        sql = "";
        //        sql = "select distinct CreditorName from PublicRecords ci join  CreditReport c on ci.CreditReportId=c.CreditReportId "
        //            + " and ClientId =" + id + " and ci.sno=" + sno + " left join "
        //            + " CreditReportItemChallenges cri on  cri.MerchantName=ci.CreditorName and UPPER(cri.agency)=UPPER(ci.agency)   and CRI.ClientID=" + id
        //            + " and CRI.PublicRecordId > 0   and cri.sno =" + sno;

        //        dt1 = utilities.GetDataTable(sql);

        //        for (int r = 0; r < dt.Rows.Count; r++)
        //        {

        //            string EQUIFAX = dt.Rows[r]["EQUIFAX"].ToString();
        //            string TRANSUNION = dt.Rows[r]["TRANSUNION"].ToString();
        //            string EXPERIAN = dt.Rows[r]["EXPERIAN"].ToString();
        //            string[] strTRANSUNION = TRANSUNION.Split('^');
        //            string newstrTRANSUNION = strTRANSUNION.ToString();

        //            string[] strEQUIFAX = EQUIFAX.Split('^');
        //            string[] strEXPERIAN = EXPERIAN.Split('^');

        //            List<PublicRecord> inqquifax = new List<PublicRecord>();
        //            List<PublicRecord> inqexperian = new List<PublicRecord>();
        //            List<PublicRecord> inqtransunion = new List<PublicRecord>();

        //            if (TRANSUNION != "")
        //            {
        //                for (int k = 0; k < strTRANSUNION.Length; k++)
        //                {
        //                    string[] strTRANS1 = strTRANSUNION[k].Split('~');
        //                    PublicRecord inq = new PublicRecord();
        //                    inq.atcourtName = strTRANS1[0].Replace("&amp;", "");
        //                    inq.AccountType = strTRANS1[1];
        //                    inq.atdateFiled = strTRANS1[2];
        //                    inq.ChallengeStatus = strTRANS1[3];
        //                    inq.ChallengeText = strTRANS1[4];
        //                    inq.PublicRecordId = strTRANS1[5];
        //                    if (inq.ChallengeText != "NO CHALLENGE")
        //                    {
        //                        inqtransunion.Add(inq);
        //                    }
        //                }
        //            }
        //            if (EQUIFAX != "")
        //            {
        //                for (int k = 0; k < strEQUIFAX.Length; k++)
        //                {
        //                    string[] strEQUIFAX1 = strEQUIFAX[k].Split('~');
        //                    PublicRecord inq = new PublicRecord();
        //                    inq.atcourtName = strEQUIFAX1[0].Replace("&amp;", "");
        //                    inq.AccountType = strEQUIFAX1[1];
        //                    inq.atdateFiled = strEQUIFAX1[2];
        //                    inq.ChallengeStatus = strEQUIFAX1[3];
        //                    inq.ChallengeText = strEQUIFAX1[4];
        //                    inq.PublicRecordId = strEQUIFAX1[5];
        //                    if (inq.ChallengeText != "NO CHALLENGE")
        //                    {
        //                        inqquifax.Add(inq);
        //                    }
        //                }
        //            }
        //            if (EXPERIAN != "")
        //            {
        //                for (int l = 0; l < strEXPERIAN.Length; l++)
        //                {
        //                    string[] strEXPERIAN1 = strEXPERIAN[l].Split('~');
        //                    PublicRecord inq = new PublicRecord();
        //                    inq.atcourtName = strEXPERIAN1[0].Replace("&amp;", "");
        //                    inq.AccountType = strEXPERIAN1[1];
        //                    inq.atdateFiled = strEXPERIAN1[2];
        //                    inq.ChallengeStatus = strEXPERIAN1[3];
        //                    inq.ChallengeText = strEXPERIAN1[4];
        //                    inq.PublicRecordId = strEXPERIAN1[5];
        //                    if (inq.ChallengeText != "NO CHALLENGE")
        //                    {
        //                        inqexperian.Add(inq);
        //                    }
        //                }
        //            }
        //            for (int x = 0; x < dt1.Rows.Count; x++)
        //            {
        //                for (int i = 0; i < 6; i++)
        //                {
        //                    PublicRecord Inq = new PublicRecord();
        //                    Inq.Heading = getHeadingsForPublicRecords()[i];
        //                    if (Inq.Heading == "Account Name")
        //                    {
        //                        try
        //                        {
        //                            if (inqquifax[x].atcourtName != null)
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].atcourtName;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {

        //                        }

        //                        try
        //                        {
        //                            if (inqtransunion[x].atcourtName != null)
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].atcourtName;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }

        //                        try
        //                        {
        //                            if (inqexperian[x].atcourtName != null)
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].atcourtName;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }

        //                    }
        //                    if (Inq.Heading == "Account Type")
        //                    {

        //                        //var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);
        //                        try
        //                        {
        //                            if (inqquifax[x].AccountType != null && inqquifax[x].AccountType != "")
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].AccountType;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }

        //                        try
        //                        {
        //                            //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqtransunion[x].AccountType != null && inqtransunion[x].AccountType != "")
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].AccountType;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }

        //                        try
        //                        {
        //                            //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqexperian[x].AccountType != null && inqexperian[x].AccountType != "")
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].AccountType;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }

        //                    }
        //                    if (Inq.Heading == "Date")
        //                    {
        //                        try
        //                        {
        //                            if (inqquifax[x].atdateFiled != null)
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].atdateFiled;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }

        //                        }
        //                        catch (Exception)
        //                        { }

        //                        try
        //                        {
        //                            if (inqtransunion[x].atdateFiled != null)
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].atdateFiled;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }


        //                        try
        //                        {
        //                            if (inqexperian[x].atdateFiled != null)
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].atdateFiled;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                    }

        //                    if (Inq.Heading == "Challenge Status")
        //                    {
        //                        try
        //                        {
        //                            if (inqquifax[x].ChallengeStatus != null)
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].ChallengeStatus;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }


        //                        try
        //                        {
        //                            if (inqtransunion[x].ChallengeStatus != null)
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].ChallengeStatus;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }


        //                        try
        //                        {
        //                            if (inqexperian[x].ChallengeStatus != null)
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].ChallengeStatus;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                    }
        //                    if (Inq.Heading == "Challenge")
        //                    {
        //                        try
        //                        {
        //                            if (inqquifax[x].ChallengeText != null && inqquifax[x].ChallengeText != "-")
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].ChallengeText;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }


        //                        try
        //                        {
        //                            //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqtransunion[x].ChallengeText != null && inqtransunion[x].ChallengeText != "-")
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].ChallengeText;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                        try
        //                        {
        //                            //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqexperian[x].ChallengeText != null && inqexperian[x].ChallengeText != "-")
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].ChallengeText;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                    }
        //                    if (Inq.Heading == "PublicRecordId")
        //                    {
        //                        try
        //                        {
        //                            if (inqquifax[x].PublicRecordId != null)
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].PublicRecordId;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                        try
        //                        {
        //                            //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqtransunion[x].PublicRecordId != null)
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].PublicRecordId;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                        try
        //                        {
        //                            //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqexperian[x].PublicRecordId != null)
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].PublicRecordId;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                    }

        //                    if (Inq.EXPERIAN == null && Inq.TRANSUNION == null && Inq.EQUIFAX == null)
        //                    { }
        //                    else
        //                    {
        //                        publicRecords.Add(Inq);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex) { ex.insertTrace(""); }

        //    return publicRecords;
        //}
        public List<Inquires> GetCreditReportInquiresAgent(int id, string agency = null, string role = "")
        {

            List<Inquires> Inquires = new List<Inquires>();
            string sql = "";
            DataTable dt;
            DataTable dt1;
            try
            {

                ClientData clientData = new ClientData();
                DataSet ds = clientdata.GetCreditReportInquires(id.ToString(), role);
                dt = ds.Tables[0];
                dt1 = ds.Tables[1];
                List<Inquires> inqquifax = new List<Inquires>();
                List<Inquires> inqexperian = new List<Inquires>();
                List<Inquires> inqtransunion = new List<Inquires>();
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    string EQUIFAX = dt.Rows[r]["EQUIFAX"].ToString();
                    string TRANSUNION = dt.Rows[r]["TRANSUNION"].ToString();
                    string EXPERIAN = dt.Rows[r]["EXPERIAN"].ToString();
                    string[] strTRANSUNION = TRANSUNION.Split('^');
                    string newstrTRANSUNION = strTRANSUNION.ToString();
                    string[] strEQUIFAX = EQUIFAX.Split('^');
                    string[] strEXPERIAN = EXPERIAN.Split('^');

                    if (TRANSUNION != "")
                    {
                        for (int k = 0; k < strTRANSUNION.Length; k++)
                        {
                            string[] strTRANS1 = strTRANSUNION[k].Split('~');
                            Inquires inq = new Inquires();
                            inq.CreditorName = strTRANS1[0].Replace("&amp;", "");
                            inq.TypeofBusiness = strTRANS1[1];
                            inq.Dateofinquiry = strTRANS1[2];
                            inq.ChallengeStatus = strTRANS1[3];
                            inq.ChallengeText = strTRANS1[4];
                            inq.CreditInqId = strTRANS1[5];
                            if (inq.ChallengeText != "NO CHALLENGE")
                            {
                                inqtransunion.Add(inq);
                            }
                        }
                    }
                    if (EQUIFAX != "")
                    {
                        for (int k = 0; k < strEQUIFAX.Length; k++)
                        {
                            string[] strEQUIFAX1 = strEQUIFAX[k].Split('~');
                            Inquires inq = new Inquires();
                            inq.CreditorName = strEQUIFAX1[0].Replace("&amp;", "");
                            inq.TypeofBusiness = strEQUIFAX1[1];
                            inq.Dateofinquiry = strEQUIFAX1[2];
                            inq.ChallengeStatus = strEQUIFAX1[3];
                            inq.ChallengeText = strEQUIFAX1[4];
                            inq.CreditInqId = strEQUIFAX1[5];
                            if (inq.ChallengeText != "NO CHALLENGE")
                            {
                                inqquifax.Add(inq);
                            }
                        }
                    }
                    if (EXPERIAN != "")
                    {
                        for (int l = 0; l < strEXPERIAN.Length; l++)
                        {
                            string[] strEXPERIAN1 = strEXPERIAN[l].Split('~');
                            Inquires inq = new Inquires();
                            inq.CreditorName = strEXPERIAN1[0].Replace("&amp;", "");
                            inq.TypeofBusiness = strEXPERIAN1[1];
                            inq.Dateofinquiry = strEXPERIAN1[2];
                            inq.ChallengeStatus = strEXPERIAN1[3];
                            inq.ChallengeText = strEXPERIAN1[4];
                            inq.CreditInqId = strEXPERIAN1[5];
                            if (inq.ChallengeText != "NO CHALLENGE")
                            {
                                inqexperian.Add(inq);
                            }
                        }
                    }
                }
                 for (int x = 0; x < dt1.Rows.Count; x++)
                    {
                        //string accountid = dt1.Rows[x]["accountId"].ToString();
                        for (int i = 0; i < 6; i++)
                        {
                            Inquires Inq = new Inquires();
                            Inq.Heading = getHeadingsForInquires()[i];
                            if (Inq.Heading == "Creditor Name")
                            {
                                try
                                {
                                    if (inqquifax[x].CreditorName != null)
                                    {
                                        Inq.EQUIFAX = inqquifax[x].CreditorName;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                try
                                {
                                    if (inqtransunion[x].CreditorName != null)
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].CreditorName;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }

                                try
                                {
                                    if (inqexperian[x].CreditorName != null)
                                    {
                                        Inq.EXPERIAN = inqexperian[x].CreditorName;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }

                            }
                            if (Inq.Heading == "Type Of Business")
                            {

                                //var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);
                                try
                                {
                                    if (inqquifax[x].TypeofBusiness != null && inqquifax[x].TypeofBusiness != "")
                                    {
                                        Inq.EQUIFAX = inqquifax[x].TypeofBusiness;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }

                                try
                                {
                                    //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                    if (inqtransunion[x].TypeofBusiness != null && inqtransunion[x].TypeofBusiness != "")
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].TypeofBusiness;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }

                                try
                                {
                                    //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                    if (inqexperian[x].TypeofBusiness != null && inqexperian[x].TypeofBusiness != "")
                                    {
                                        Inq.EXPERIAN = inqexperian[x].TypeofBusiness;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }

                            }
                            if (Inq.Heading == "Date Of Inquiry")
                            {
                                try
                                {
                                    if (inqquifax[x].Dateofinquiry != null)
                                    {
                                        Inq.EQUIFAX = inqquifax[x].Dateofinquiry;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }

                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    if (inqtransunion[x].Dateofinquiry != null)
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].Dateofinquiry;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                {

                                }


                                try
                                {
                                    if (inqexperian[x].Dateofinquiry != null)
                                    {
                                        Inq.EXPERIAN = inqexperian[x].Dateofinquiry;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                            }

                            if (Inq.Heading == "Challenge Status")
                            {
                                try
                                {
                                    if (inqquifax[x].ChallengeStatus != null)
                                    {
                                        Inq.EQUIFAX = inqquifax[x].ChallengeStatus;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }
                                }
                                catch (Exception)
                                { }


                                try
                                {
                                    if (inqtransunion[x].ChallengeStatus != null)
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].ChallengeStatus;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                { }


                                try
                                {
                                    if (inqexperian[x].ChallengeStatus != null)
                                    {
                                        Inq.EXPERIAN = inqexperian[x].ChallengeStatus;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                            }
                            if (Inq.Heading == "Challenge")
                            {
                                try
                                {
                                    if (inqquifax[x].ChallengeText != null && inqquifax[x].ChallengeText != "-")
                                    {
                                        Inq.EQUIFAX = inqquifax[x].ChallengeText;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }
                                }
                                catch (Exception)
                                { }


                                try
                                {
                                    //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                    if (inqtransunion[x].ChallengeText != null && inqtransunion[x].ChallengeText != "-")
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].ChallengeText;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                    if (inqexperian[x].ChallengeText != null && inqexperian[x].ChallengeText != "-")
                                    {
                                        Inq.EXPERIAN = inqexperian[x].ChallengeText;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                            }
                            if (Inq.Heading == "CreditInqId")
                            {
                                try
                                {
                                    if (inqquifax[x].CreditInqId != null)
                                    {
                                        Inq.EQUIFAX = inqquifax[x].CreditInqId;
                                    }
                                    else
                                    {
                                        Inq.EQUIFAX = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
                                    if (inqtransunion[x].CreditInqId != null)
                                    {
                                        Inq.TRANSUNION = inqtransunion[x].CreditInqId;
                                    }
                                    else
                                    {
                                        Inq.TRANSUNION = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                                try
                                {
                                    //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
                                    if (inqexperian[x].CreditInqId != null)
                                    {
                                        Inq.EXPERIAN = inqexperian[x].CreditInqId;
                                    }
                                    else
                                    {
                                        Inq.EXPERIAN = "-";
                                    }
                                }
                                catch (Exception)
                                { }

                            }


                            //if (Inq.EXPERIAN == "-" && Inq.TRANSUNION == "-" && Inq.EQUIFAX == "-")
                            //{

                            //}
                            //else
                            //{

                            //}

                            if (Inq.EXPERIAN == null && Inq.TRANSUNION == null && Inq.EQUIFAX == null)
                            {

                            }
                            else
                            {
                                Inquires.Add(Inq);
                            }

                        }
                    }
                
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return Inquires;
        }
        //public List<Inquires> GetCreditReportInquiresAgent(int id, string agency = null, string role = "")
        //{

        //    List<Inquires> Inquires = new List<Inquires>();
        //    string sql = "";
        //    DataTable dt;
        //    DataTable dt1;
        //    try
        //    {
        //        int sno = 0;
        //        sql = "select top 1 sno from CreditInquiries where   "
        //       + " CreditReportId in (Select CreditReportId from CreditReport where  ClientId =" + id + ")  order by CreditInqId desc";
        //        try
        //        {
        //            sno = utilities.ExecuteScalar(sql, true).ConvertObjectToIntIfNotNull();
        //        }
        //        catch (Exception)
        //        { }

        //        sql = " Select max(a.sno) from CreditReportItemChallenges a,CreditInquiries b, CreditReport c   where "
        //            + "   a.MerchantName=b.CreditorName and UPPER(a.agency)=UPPER(b.agency) and a.CreditInqId > 0  and a.ClientID=" + id
        //            + " and b.CreditReportId=c.CreditReportId and c.ClientID=" + id;
        //        long val = 0;
        //        try
        //        {
        //            object _obj = utilities.ExecuteScalar(sql, true);

        //            val = _obj != null ? long.Parse(_obj.ToString()) : 0;
        //        }
        //        catch (Exception ex)
        //        { }


        //        sql = "SELECT (";

        //        if (val > 0)
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfBusiness + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfInquiry,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges "
        //        + " where CreditInqId=US.CreditInqId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfInquiry order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.CreditInqId))" +
        //        " FROM" +
        //        " CreditInquiries US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        "  join CreditReportItemChallenges cri ON   CRI.MerchantName=US.CreditorName and UPPER(CRI.agency)=UPPER(US.agency)  and CRI.ClientID=" + id
        //        + " and US.CreditInqId > 0 and cri.sno =" + val +
        //        " where US.Agency = 'EQUIFAX' and CR.ClientId = '" + id + "' and us.sno=" + sno
        //        + " FOR XML PATH('')), 1, 1, '')) " +
        //        " as 'EQUIFAX',";
        //        }
        //        else
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfBusiness + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfInquiry,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges "
        //        + " where CreditInqId=US.CreditInqId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfInquiry order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.CreditInqId))" +
        //        " FROM" +
        //        " CreditInquiries US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        " where US.Agency = 'EQUIFAX' and CR.ClientId = '" + id + "' and us.sno=" + sno
        //        + " FOR XML PATH('')), 1, 1, '')) " +
        //        " as 'EQUIFAX',";
        //        }

        //        if (val > 0)
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfBusiness + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfInquiry,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges "
        //        + " where CreditInqId=US.CreditInqId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfInquiry order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.CreditInqId))" +
        //        " FROM" +
        //        " CreditInquiries US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        "  join CreditReportItemChallenges cri ON   CRI.MerchantName=US.CreditorName and UPPER(CRI.agency)=UPPER(US.agency)  and CRI.ClientID=" + id
        //        + " and US.CreditInqId > 0 and cri.sno =" + val +
        //        " where US.Agency = 'EXPERIAN' and CR.ClientId = '" + id + "' and us.sno=" + sno
        //       + " FOR XML PATH('')), 1, 1, '')" +
        //        " as 'EXPERIAN',";
        //        }
        //        else
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfBusiness + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfInquiry,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges "
        //        + " where CreditInqId=US.CreditInqId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfInquiry order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.CreditInqId))" +
        //        " FROM" +
        //        " CreditInquiries US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        " where US.Agency = 'EXPERIAN' and CR.ClientId = '" + id + "' and us.sno=" + sno
        //       + " FOR XML PATH('')), 1, 1, '')" +
        //        " as 'EXPERIAN',";
        //        }


        //        if (val > 0)
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfBusiness + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfInquiry,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges  "
        //        + " where CreditInqId=US.CreditInqId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfInquiry order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.CreditInqId))" +
        //        " FROM" +
        //        " CreditInquiries US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        "  join CreditReportItemChallenges cri ON   CRI.MerchantName=US.CreditorName and UPPER(CRI.agency)=UPPER(US.agency)  and CRI.ClientID=" + id
        //        + " and US.CreditInqId > 0 and cri.sno =" + val +
        //        "  where US.Agency = 'TRANSUNION' and CR.ClientId = '" + id + "' and us.sno=" + sno +
        //        " FOR XML PATH('')), 1, 1, '')" +
        //        " as 'TRANSUNION'";
        //        }
        //        else
        //        {
        //            sql += " STUFF((SELECT '^ ' + (CreditorName + '~' + TypeOfBusiness + '~' + CONVERT(varchar,CONVERT(datetime,US.DateOfInquiry,101),101)" +
        //        " + '~' + isnull(Convert(varchar, ((select top 1 RoundType from CreditReportItemChallenges  "
        //        + " where CreditInqId=US.CreditInqId))), '-') +'~'+ isnull(Convert(varchar, ((Select top 1 RoundType+'-'+convert(varchar,ChallengeText) "
        //        + " from CreditReportItemChallenges where  MerchantName=US.CreditorName and AccountId is null and Agency=us.Agency and clientid =" + id + " and"
        //        + " DateOfInquiry=US.DateOfInquiry order by CrdRepItemChallengeId desc))), '-') + '~' + Convert(varchar, US.CreditInqId))" +
        //        " FROM" +
        //        " CreditInquiries US join CreditReport cr on US.CreditReportId = cr.CreditReportId " +
        //        "  where US.Agency = 'TRANSUNION' and CR.ClientId = '" + id + "' and us.sno=" + sno +
        //        " FOR XML PATH('')), 1, 1, '')" +
        //        " as 'TRANSUNION'";
        //        }



        //        dt = utilities.GetDataTable(sql);
        //        sql = "";
        //        //sql = "select distinct CreditorName from CreditInquiries where  "
        //        //+ " CreditReportId in (Select CreditReportId from CreditReport where  ClientId =" + id + ") and sno=" + sno
        //        //+ " and CreditInqId in (select CreditInqId from CreditReportItemChallenges where sno="+ sno+")";


        //        sql = "select distinct CreditorName from CreditInquiries ci join  CreditReport c on ci.CreditReportId=c.CreditReportId "
        //            + " and ClientId =" + id + " and ci.sno=" + sno + " left join CreditReportItemChallenges cri on  "
        //            + " CRI.MerchantName=CI.CreditorName and UPPER(CRI.agency)=UPPER(CI.agency)  and CRI.ClientID=" + id
        //            + " and CI.CreditInqId > 0 and cri.sno =" + sno;

        //        dt1 = utilities.GetDataTable(sql);

        //        for (int r = 0; r < dt.Rows.Count; r++)
        //        {

        //            string EQUIFAX = dt.Rows[r]["EQUIFAX"].ToString();
        //            string TRANSUNION = dt.Rows[r]["TRANSUNION"].ToString();
        //            string EXPERIAN = dt.Rows[r]["EXPERIAN"].ToString();
        //            string[] strTRANSUNION = TRANSUNION.Split('^');
        //            string newstrTRANSUNION = strTRANSUNION.ToString();

        //            string[] strEQUIFAX = EQUIFAX.Split('^');
        //            string[] strEXPERIAN = EXPERIAN.Split('^');

        //            List<Inquires> inqquifax = new List<Inquires>();
        //            List<Inquires> inqexperian = new List<Inquires>();
        //            List<Inquires> inqtransunion = new List<Inquires>();

        //            if (TRANSUNION != "")
        //            {
        //                for (int k = 0; k < strTRANSUNION.Length; k++)
        //                {
        //                    string[] strTRANS1 = strTRANSUNION[k].Split('~');
        //                    Inquires inq = new Inquires();
        //                    inq.CreditorName = strTRANS1[0].Replace("&amp;", "");
        //                    inq.TypeofBusiness = strTRANS1[1];
        //                    inq.Dateofinquiry = strTRANS1[2];
        //                    inq.ChallengeStatus = strTRANS1[3];
        //                    inq.ChallengeText = strTRANS1[4];
        //                    inq.CreditInqId = strTRANS1[5];
        //                    if (inq.ChallengeText != "NO CHALLENGE")
        //                    {
        //                        inqtransunion.Add(inq);
        //                    }
        //                }
        //            }
        //            if (EQUIFAX != "")
        //            {
        //                for (int k = 0; k < strEQUIFAX.Length; k++)
        //                {
        //                    string[] strEQUIFAX1 = strEQUIFAX[k].Split('~');
        //                    Inquires inq = new Inquires();
        //                    inq.CreditorName = strEQUIFAX1[0].Replace("&amp;", "");
        //                    inq.TypeofBusiness = strEQUIFAX1[1];
        //                    inq.Dateofinquiry = strEQUIFAX1[2];
        //                    inq.ChallengeStatus = strEQUIFAX1[3];
        //                    inq.ChallengeText = strEQUIFAX1[4];
        //                    inq.CreditInqId = strEQUIFAX1[5];
        //                    if (inq.ChallengeText != "NO CHALLENGE")
        //                    {
        //                        inqquifax.Add(inq);
        //                    }
        //                }
        //            }
        //            if (EXPERIAN != "")
        //            {
        //                for (int l = 0; l < strEXPERIAN.Length; l++)
        //                {
        //                    string[] strEXPERIAN1 = strEXPERIAN[l].Split('~');
        //                    Inquires inq = new Inquires();
        //                    inq.CreditorName = strEXPERIAN1[0].Replace("&amp;", "");
        //                    inq.TypeofBusiness = strEXPERIAN1[1];
        //                    inq.Dateofinquiry = strEXPERIAN1[2];
        //                    inq.ChallengeStatus = strEXPERIAN1[3];
        //                    inq.ChallengeText = strEXPERIAN1[4];
        //                    inq.CreditInqId = strEXPERIAN1[5];
        //                    if (inq.ChallengeText != "NO CHALLENGE")
        //                    {
        //                        inqexperian.Add(inq);
        //                    }
        //                }
        //            }
        //            for (int x = 0; x < dt1.Rows.Count; x++)
        //            {
        //                //string accountid = dt1.Rows[x]["accountId"].ToString();
        //                for (int i = 0; i < 6; i++)
        //                {
        //                    Inquires Inq = new Inquires();
        //                    Inq.Heading = getHeadingsForInquires()[i];
        //                    if (Inq.Heading == "Creditor Name")
        //                    {
        //                        try
        //                        {
        //                            if (inqquifax[x].CreditorName != null)
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].CreditorName;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {

        //                        }

        //                        try
        //                        {
        //                            if (inqtransunion[x].CreditorName != null)
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].CreditorName;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }

        //                        try
        //                        {
        //                            if (inqexperian[x].CreditorName != null)
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].CreditorName;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }

        //                    }
        //                    if (Inq.Heading == "Type Of Business")
        //                    {

        //                        //var equifax = achquifax.FirstOrDefault(t => t.Account == accountid);
        //                        try
        //                        {
        //                            if (inqquifax[x].TypeofBusiness != null && inqquifax[x].TypeofBusiness != "")
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].TypeofBusiness;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }

        //                        try
        //                        {
        //                            //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqtransunion[x].TypeofBusiness != null && inqtransunion[x].TypeofBusiness != "")
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].TypeofBusiness;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }

        //                        try
        //                        {
        //                            //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqexperian[x].TypeofBusiness != null && inqexperian[x].TypeofBusiness != "")
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].TypeofBusiness;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }

        //                    }
        //                    if (Inq.Heading == "Date Of Inquiry")
        //                    {
        //                        try
        //                        {
        //                            if (inqquifax[x].Dateofinquiry != null)
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].Dateofinquiry;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }

        //                        }
        //                        catch (Exception)
        //                        { }

        //                        try
        //                        {
        //                            if (inqtransunion[x].Dateofinquiry != null)
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].Dateofinquiry;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        {

        //                        }


        //                        try
        //                        {
        //                            if (inqexperian[x].Dateofinquiry != null)
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].Dateofinquiry;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                    }

        //                    if (Inq.Heading == "Challenge Status")
        //                    {
        //                        try
        //                        {
        //                            if (inqquifax[x].ChallengeStatus != null)
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].ChallengeStatus;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }


        //                        try
        //                        {
        //                            if (inqtransunion[x].ChallengeStatus != null)
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].ChallengeStatus;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }


        //                        try
        //                        {
        //                            if (inqexperian[x].ChallengeStatus != null)
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].ChallengeStatus;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                    }
        //                    if (Inq.Heading == "Challenge")
        //                    {
        //                        try
        //                        {
        //                            if (inqquifax[x].ChallengeText != null && inqquifax[x].ChallengeText != "-")
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].ChallengeText;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }


        //                        try
        //                        {
        //                            //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqtransunion[x].ChallengeText != null && inqtransunion[x].ChallengeText != "-")
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].ChallengeText;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                        try
        //                        {
        //                            //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqexperian[x].ChallengeText != null && inqexperian[x].ChallengeText != "-")
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].ChallengeText;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                    }
        //                    if (Inq.Heading == "CreditInqId")
        //                    {
        //                        try
        //                        {
        //                            if (inqquifax[x].CreditInqId != null)
        //                            {
        //                                Inq.EQUIFAX = inqquifax[x].CreditInqId;
        //                            }
        //                            else
        //                            {
        //                                Inq.EQUIFAX = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                        try
        //                        {
        //                            //var transunio = achtransunion.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqtransunion[x].CreditInqId != null)
        //                            {
        //                                Inq.TRANSUNION = inqtransunion[x].CreditInqId;
        //                            }
        //                            else
        //                            {
        //                                Inq.TRANSUNION = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                        try
        //                        {
        //                            //var experian = achexperian.FirstOrDefault(t => t.Account == accountid);
        //                            if (inqexperian[x].CreditInqId != null)
        //                            {
        //                                Inq.EXPERIAN = inqexperian[x].CreditInqId;
        //                            }
        //                            else
        //                            {
        //                                Inq.EXPERIAN = "-";
        //                            }
        //                        }
        //                        catch (Exception)
        //                        { }

        //                    }


        //                    //if (Inq.EXPERIAN == "-" && Inq.TRANSUNION == "-" && Inq.EQUIFAX == "-")
        //                    //{

        //                    //}
        //                    //else
        //                    //{

        //                    //}

        //                    if (Inq.EXPERIAN == null && Inq.TRANSUNION == null && Inq.EQUIFAX == null)
        //                    {

        //                    }
        //                    else
        //                    {
        //                        Inquires.Add(Inq);
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex) { ex.insertTrace(""); }

        //    return Inquires;
        //}

        public List<string> getHeadingsForInquires()
        {
            List<string> Headings = new List<string>();
            Headings.Add("Creditor Name");
            Headings.Add("Type Of Business");
            Headings.Add("Date Of Inquiry");
            Headings.Add("Challenge Status");
            Headings.Add("Challenge");
            Headings.Add("CreditInqId");
            return Headings;
        }

        public List<string> getHeadingsForPublicRecords()
        {
            List<string> Headings = new List<string>();
            Headings.Add("Account Name");
            Headings.Add("Account Type");
            Headings.Add("Date");
            Headings.Add("Challenge Status");
            Headings.Add("Challenge");
            Headings.Add("PublicRecordId");
            return Headings;
        }
        //public List<Inquires> GetInquiriesChallengesAgentById(List<Inquires> credit, int id)
        //{
        //    List<Inquires> Inquires = new List<Inquires>();
        //    string sql = "";
        //    DataTable dt;
        //    try
        //    {
        //        for (int i = 0; i < credit.Count; i++)
        //        {
        //            sql = "select ci.CreditorName,ci.TypeOfBusiness,ci.DateOfInquiry,ci.Agency,crc.RoundType, ";
        //            sql += "crc.ChallengeText from CreditReportItemChallenges as crc inner join CreditInquiries as ci on crc.CreditInqId = ci.CreditInqId ";
        //            sql += "inner join CreditReport as c on c.CreditReportId = ci.CreditReportId ";
        //            sql += "where c.ClientId = '" + id + "' and crc.sno=(select max(sno) from CreditReportItemChallenges where CreditInqId=crc.CreditInqId) and crc.CreditInqId ='" + credit[i].CreditInqId + "'";
        //            sql += "order by ci.Agency";

        //            dt = utilities.GetDataTable(sql);
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                Inquires Inq = new Inquires();
        //                Inq.CreditorName = row["CreditorName"].ToString();
        //                Inq.TypeofBusiness = string.IsNullOrEmpty(row["TypeofBusiness"].ToString()) ? "-" : row["TypeofBusiness"].ToString();
        //                Inq.Dateofinquiry = row["Dateofinquiry"].ToString();
        //                Inq.CreditBureau = row["Agency"].ToString();
        //                Inq.ChallengeText = row["ChallengeText"].ToString();
        //                Inq.RoundType = row["RoundType"].ToString();
        //                Inq.AccountType = credit[i].AccountType;
        //                Inquires.Add(Inq);
        //            }
        //        }
        //    }
        //    catch (Exception ex) { ex.insertTrace(""); }
        //    return Inquires;
        //}

        public List<CreditReportItems> GetInquiriesChallengesAgentById(List<Inquires> credit, int id)
        {
            List<CreditReportItems> Inquires = new List<CreditReportItems>();
            string sql = "";
            DataTable dt;
            try
            {
                for (int i = 0; i < credit.Count; i++)
                {
                    sql = "select ci.CreditorName,ci.TypeOfBusiness,ci.DateOfInquiry,ci.Agency,crc.RoundType, ";
                    sql += "crc.ChallengeText from CreditReportItemChallenges as crc inner join CreditInquiries as ci on crc.CreditInqId = ci.CreditInqId ";
                    sql += "inner join CreditReport as c on c.CreditReportId = ci.CreditReportId ";
                    sql += "where c.ClientId = '" + id + "' and crc.sno=(select max(sno) from CreditReportItemChallenges where CreditInqId=crc.CreditInqId) and crc.CreditInqId ='" + credit[i].CreditInqId + "'";
                    sql += "order by ci.Agency";

                    dt = utilities.GetDataTable(sql);
                    foreach (DataRow row in dt.Rows)
                    {
                        CreditReportItems Inq = new CreditReportItems();
                        //Inq.CreditorName = row["CreditorName"].ToString();
                        //Inq.TypeofBusiness = string.IsNullOrEmpty(row["TypeofBusiness"].ToString()) ? "-" : row["TypeofBusiness"].ToString();
                        //Inq.Dateofinquiry = row["Dateofinquiry"].ToString();
                        //Inq.CreditBureau = row["Agency"].ToString();
                        Inq.MerchantName = row["CreditorName"].ToString();
                        Inq.AccountType = string.IsNullOrEmpty(row["TypeofBusiness"].ToString()) ? "-" : row["TypeofBusiness"].ToString();
                        Inq.OpenDate = row["Dateofinquiry"].ToString();
                        Inq.Agency = row["Agency"].ToString();
                        Inq.ChallengeText = row["ChallengeText"].ToString();
                        Inq.RoundType = row["RoundType"].ToString();
                        Inq.AccountType = credit[i].AccountType;
                        Inquires.Add(Inq);
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Inquires;
        }
        public List<CreditReportItems> GetPRChallengesAgentById(List<PublicRecord> credit, int id)
        {
            List<CreditReportItems> CreditItems = new List<CreditReportItems>();
            string sql = "";
            DataTable dt;
            try
            {
                for (int i = 0; i < credit.Count; i++)
                {
                    sql = "select ci.CreditorName,ci.TypeOfPR,ci.DateOfPR,ci.Agency,crc.RoundType, ci.PublicRecordId, ";
                    sql += "crc.ChallengeText from CreditReportItemChallenges as crc inner join publicrecords as ci on crc.PublicRecordId = ci.PublicRecordId ";
                    sql += "inner join CreditReport as c on c.CreditReportId = ci.CreditReportId ";
                    sql += "where c.ClientId = '" + id + "' and crc.sno=(select max(sno) from CreditReportItemChallenges"
                        + " where PublicRecordId=crc.PublicRecordId) and crc.PublicRecordId ='" + credit[i].PublicRecordId + "'";
                    sql += "order by ci.Agency";

                    dt = utilities.GetDataTable(sql);
                    foreach (DataRow row in dt.Rows)
                    {
                        //PublicRecord Inq = new PublicRecord();
                        //Inq.atcourtName = row["CreditorName"].ToString();
                        //Inq.AccountType = string.IsNullOrEmpty(row["TypeOfPR"].ToString()) ? "-" : row["TypeOfPR"].ToString();
                        //Inq.atdateFiled = row["DateOfPR"].ToString();
                        //Inq.atbureau = row["Agency"].ToString();
                        //Inq.ChallengeText = row["ChallengeText"].ToString();
                        //Inq.RoundType = row["RoundType"].ToString();
                        //Inq.AccountType = credit[i].AccountType;
                        //Inquires.Add(Inq);

                        CreditReportItems clientbar = new CreditReportItems();
                        clientbar.AccountId = row["TypeOfPR"].ToString();
                        clientbar.MerchantName = row["CreditorName"].ToString();
                        clientbar.OpenDate = row["DateOfPR"].ToString();
                        clientbar.Agency = row["Agency"].ToString();
                        clientbar.ChallengeText = row["ChallengeText"].ToString();
                        clientbar.RoundType = row["RoundType"].ToString();
                        clientbar.CredRepItemsId = row["PublicRecordId"].ToString().StringToInt(0);
                        clientbar.AccountType = credit[i].AccountType;
                        clientbar.AccountTypeDetail = credit[i].AccountType;
                        CreditItems.Add(clientbar);
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return CreditItems;
        }
        public long updatechallengefilepath(List<CreditReportFiles> files, int sno)
        {
            long res = 0;
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < files.Count; i++)
                {
                    sb.AppendFormat(" insert into CreditReportFiles(CRFilename,RoundType,ClientId,sno,isManual) "
                        + " values('{0}','{1}',{2},{3},{4})", files[i].CRFilename, files[i].RoundType, files[i].ClientId, sno, files[i].isManual);
                }
                var cmd1 = new SqlCommand();
                cmd1.CommandText = sb.ToString();
                res = utilities.ExecuteInsertCommand(cmd1, true);
                return res;
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        public long UpdateCreditReportFileIsMoved(string file)
        {
            long res = 0;
            try
            {
                string sql = "UPDATE CreditReportFiles SET isMoved=1 where CRFilename='" + file + "'";
                res = utilities.ExecuteString(sql, true);
                return res;
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }

        public List<Agent> GetAgentBillings()
        {
            List<Agent> objAgent = new List<Agent>();
            string strStatus = "";
            try
            {
                string sql = "Select a.AgentId, a.BusinessName as AgencyName,isnull((select CompanyType from CompanyTypes where CompanyTypeId = a.TypeOfComp),'')as TypeofCompany, " +
                            "isnull((select Count(AgentStaffId) from AgentStaff where AgentId = a.AgentId and status = 1),'')as NumberofStaff, " +
                            "isnull(Convert(varchar(15), a.createddate, 101), '') as MemberSince, " +
                            "isnull((select Count(AgentId) from Client  where AgentId = a.AgentId),'')as RegisteredClients, " +
                            "isnull((isnull(a.FirstName, '') + ' ' + isnull(a.LastName, '')), '') as PrimaryUser, " +
                            "isnull(a.BillingEmail, '')as BMail ,isnull(a.BillingPhone, '') as BPhone, " +
                            "isnull((select Count(cr.ClientId) as ActiveClients from CreditReport cr , Client c where  cr.ClientId = c.ClientId and c.AgentId = a.AgentId and " +
                            "cr.AgencyName = 'EQUIFAX' and cr.RoundType = 'First Round'),'')as ActiveClients, " +
                            "isnull((select(case when u.Status = '1' then 'Active' else 'Inactive' end)from users u where AgentClientId = a.AgentId and UserRole = 'agentadmin'),'') as AgencyStatus, " +
                            "isnull((select PricingType from Pricing where PricingId = a.PricingPlan),'')as PricingPlan, " +
                            "isnull((select top 1 BillingType from AgentBilling where AgentId=a.AgentId),'')as BillingType, " +
                            "isnull((select(case BillingType" +
                            " when 'Monthly'     then(select top 1 Convert(varchar(15), DATEADD(MONTH, 1, PaymentDate), 101) from Billing where AgentId = a.AgentId and PaymentStatus ='SUCCESS') " +
                            " when 'Quarterly'   then(select top 1 Convert(varchar(15), DATEADD(MONTH, 3, PaymentDate), 101) from Billing where AgentId = a.AgentId and PaymentStatus ='SUCCESS') " +
                            " when 'Half-Yearly' then(select top 1  Convert(varchar(15), DATEADD(MONTH, 6, PaymentDate), 101) from Billing where AgentId = a.AgentId and PaymentStatus ='SUCCESS') " +
                            " when 'Annually'    then(select top 1  Convert(varchar(15), DATEADD(YEAR, 1, PaymentDate), 101)  from Billing where AgentId = a.AgentId and PaymentStatus ='SUCCESS') " +
                            " else '' end)), '') as NextBillingDate ," +
                            "isnull((select(case BillingType" +
                            " when 'Monthly'     then(select top 1 Convert(varchar(15), DATEADD(MONTH, 1, CreatedDate), 101) from AgentBilling where AgentId = a.AgentId) " +
                            " when 'Quarterly'   then(select top 1 Convert(varchar(15), DATEADD(MONTH, 3, CreatedDate), 101) from AgentBilling where AgentId = a.AgentId) " +
                            " when 'Half-Yearly' then(select top 1  Convert(varchar(15), DATEADD(MONTH, 6, CreatedDate), 101) from AgentBilling where AgentId = a.AgentId) " +
                            " when 'Annually'    then(select top 1  Convert(varchar(15), DATEADD(YEAR, 1, CreatedDate), 101)  from AgentBilling where AgentId = a.AgentId) " +
                            " else '' end)), '') as Nextcreateddate from Agent a,AgentBilling ab where ab.isPrimary = 1 and a.AgentId = ab.AgentId";
                DataTable dt = utilities.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Agent mAgent = new Agent();
                        mAgent.AgentId = Convert.ToInt32(dr["AgentId"].ToString());
                        mAgent.BusinessName = dr["AgencyName"].ToString();
                        mAgent.TypeOfComp = dr["TypeofCompany"].ToString();
                        mAgent.NumberofStaff = dr["NumberofStaff"].ToString();
                        mAgent.SCreatedDate = dr["MemberSince"].ToString();
                        mAgent.RegisteredClients = dr["RegisteredClients"].ToString();
                        mAgent.PrimaryUser = dr["PrimaryUser"].ToString();
                        mAgent.BillingEmail = dr["BMail"].ToString();
                        mAgent.BillingPhone = dr["BPhone"].ToString();
                        mAgent.ActiveClients = dr["ActiveClients"].ToString();

                        strStatus = AgentStatus(Convert.ToString(mAgent.AgentId));
                        mAgent.Status = strStatus;
                        // mAgent.Status = dr["AgencyStatus"].ToString();
                        mAgent.PricingPlans = dr["PricingPlan"].ToString();
                        mAgent.BillingType = dr["BillingType"].ToString();
                        //mAgent.NextBillingDate = dr["NextBillingDate"].ToString();

                        string next = dr["NextBillingDate"].ToString();
                        if (next != "")
                        {
                            mAgent.NextBillingDate = dr["NextBillingDate"].ToString();
                        }
                        else
                        {
                            mAgent.NextBillingDate = dr["Nextcreateddate"].ToString();
                        }

                        objAgent.Add(mAgent);
                    }
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return objAgent;
        }
        public string AgentStatus(string agentId)
        {
            string sql = "", res = "";
            bool flag = true;

            try
            {
                sql = "select status from Users where UserRole='agentadmin' and AgentClientId='" + agentId + "'";
                res = utilities.ExecuteScalar(sql, flag).ToString();
                if (res == "False")
                {
                    return "InActive";
                }
                else
                {
                    sql = "Select Convert(varchar(15), max(paymentdate), 101) as PaymentDate,PaymentStatus " +
                        "from Billing where AgentId = '" + agentId + "'  group by PaymentStatus";

                    DataTable dt = utilities.GetDataTable(sql, flag);
                    if (dt.Rows.Count > 0)
                    {
                        res = dt.Rows[0]["PaymentStatus"].ToString();
                        if (res != "")
                        {
                            if (res.ToUpper() == "FAILURE")
                            {
                                return "Suspended";
                            }
                            else
                            {
                                return "Success";
                            }
                        }
                        else
                        {
                            return "Pending";
                        }
                    }
                    else
                    {
                        return "Pending";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return res;
        }

        public List<string> GetChallengeTextByAccountTypeAndRound(string Accounttype)
        {
            List<string> AccountType = new List<string>();
            try
            {
                bool flag = true;
                string sql = "SELECT AccTypeId,AccountType FROM AccountTypes WHERE AccountType LIKE '%" + Accounttype + "%';";

                DataTable dt = utilities.GetDataTable(sql, flag);
                foreach (DataRow dr in dt.Rows)
                {
                    string accounttype = "";
                    int accounttypeid = 0;
                    accounttypeid = Convert.ToInt32(dr["AccTypeId"]);
                    accounttype = dr["AccountType"].ToString();
                    string sql1 = "SELECT ChallengeText FROM ChallengeMaster WHERE AccountTypeId='" + accounttypeid + "'";
                    DataTable dt2 = utilities.GetDataTable(sql1, flag);
                    if (dt2.Rows.Count > 0)
                    {
                        AccountType.Add(accounttype);
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return AccountType;
        }

        public List<string> GetAccountTypes()
        {
            List<string> AccountType = new List<string>();
            try
            {
                bool flag = true;
                string sql = "SELECT AccTypeId,AccountType FROM AccountTypes";

                DataTable dt = utilities.GetDataTable(sql, flag);
                foreach (DataRow dr in dt.Rows)
                {
                    string accounttype = "";
                    int accounttypeid = 0;
                    accounttypeid = Convert.ToInt32(dr["AccTypeId"]);
                    accounttype = dr["AccountType"].ToString();
                    string sql1 = "SELECT ChallengeText FROM ChallengeMaster WHERE AccountTypeId='" + accounttypeid + "'";
                    DataTable dt2 = utilities.GetDataTable(sql1, flag);
                    if (dt2.Rows.Count > 0)
                    {
                        AccountType.Add(accounttype);
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return AccountType;
        }


    }
}
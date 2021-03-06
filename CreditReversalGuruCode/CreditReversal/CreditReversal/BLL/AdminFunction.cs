﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using CreditReversal.DAL;
using CreditReversal.Models;
using CreditReversal.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace CreditReversal.BLL
{
    public class AdminFunction
    {
        private DBUtilities dbutilities = new DBUtilities();


        #region Challenge Functions


        public List<Challenge> Getchallange()
        {
            List<Challenge> challenge = new List<Challenge>();
            try
            {
                //string query = "Select ChallengeId,ChallengeLevel,ChallengeText,isnull(AccountTypeId,0)as  AccountTypeId,  " +
                //               "(select top 1 isnull(AccountType, '') from AccountTypes "+
                //               "where AccountTypeId = cm.AccountTypeId and Status = 1) as AccountType,Status " +
                //               "from ChallengeMaster cm where cm.Status = 1";

                string query= "Select ChallengeId,ChallengeLevel,ChallengeText, isnull(AccountTypeId, 0) as AccountTypeId,"+
                    "at.AccountType,cm.Status from ChallengeMaster cm inner join AccountTypes at " +
                    "on cm.AccountTypeId = at.AccTypeId  and cm.Status = 1";

                DataTable dt = dbutilities.GetDataTable(query, true);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Challenge cha = new Challenge();
                        cha.ChallengeText = row["ChallengeText"].ToString();
                        cha.ChallengeId = Convert.ToInt32(row["ChallengeId"]);
                        cha.ChallengeLevel = row["ChallengeLevel"].ToString();
                        cha.AccTypeId = Convert.ToInt32(row["AccountTypeId"].ToString());
                        cha.AccountType = row["AccountType"].ToString();                            
                        cha.Status = row["Status"].ToString();
                        challenge.Add(cha);
                    }
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return challenge;
        }

        public bool AddChallenge(Challenge challange)
        {
            int Id = challange.ChallengeId;
            bool status = false;
            string query = "";
            long res = 0;
            try
            {
                if (Id != 0)
                {
                    query = "Update ChallengeMaster Set ChallengeLevel=@ChallengeLevel,ChallengeText=@ChallengeText,AccountTypeId=@AccountTypeId where ChallengeId =" + Id;
                }
                else
                {
                    query = "Insert Into ChallengeMaster(ChallengeLevel,ChallengeText,Status,AccountTypeId) values(@ChallengeLevel,@ChallengeText,@Status,@AccountTypeId)";
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@ChallengeLevel", string.IsNullOrEmpty(challange.ChallengeLevel) ? "" : challange.ChallengeLevel);
                cmd.Parameters.AddWithValue("@ChallengeText", string.IsNullOrEmpty(challange.ChallengeText) ? "" : challange.ChallengeText);
                if (Id != 0)
                {
                 
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Status", challange.Status);
                }
                cmd.Parameters.AddWithValue("@AccountTypeId", challange.AccTypeId);
                cmd.CommandText = query;
                res = dbutilities.ExecuteInsertCommand(cmd, true);
                if (res > 0)
                {
                    status = true;

                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }

        public Challenge EditChallenge(string ChallengeId)
        {
            Challenge challenge = new Challenge();
            string query = "";
            DataRow row = null;
            try
            {
                query = "Select * from ChallengeMaster where ChallengeId = " + ChallengeId;
                SqlCommand cmd = new SqlCommand();

                row = dbutilities.GetDataRow(query);
                challenge.ChallengeId = Convert.ToInt32(row[0]);
                challenge.ChallengeLevel = row[1].ToString();
                challenge.ChallengeText = row[2].ToString();                
                challenge.Status = row[3].ToString();
                challenge.AccTypeId = Convert.ToInt32(row[4]);

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return challenge;
        }

        public bool DeleteChallenge(string ChallengeId)
        {
            bool status = false;
            string query = "";
            long res = 0;
            try
            {
                query = "Delete from ChallengeMaster where ChallengeId = " + ChallengeId;
                res = dbutilities.ExecuteCommand(query, true);
                if (res > 0)
                {
                    status = true;
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }
        #endregion


        #region Pricing Functions

        private DataTable dataTable = new DataTable();
        public List<Pricing> GetPricings(string id = "")
        {
            List<Pricing> pricings = new List<Pricing>();
            string sql = "";

            try
            {
                if (id != "")
                    sql = "select * from Pricing where PricingId=" + id;
                else
                    sql = "select * from Pricing";

                dataTable = dbutilities.GetDataTable(sql);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        pricings.Add(new Pricing
                        {
                            PricingId = Convert.ToInt32(row["PricingId"].ToString()),
                            PricingType = row["PricingType"].ToString(),
                            LogoText = row["Logo"].ToString(),
                            SetupFee = Convert.ToDecimal(row["SetupFee"].ToString().PadUpto2()),
                            PricePerMonth = Convert.ToDecimal(row["PricePerMonth"].ToString().PadUpto2()),
                            AdditionalAgent = Convert.ToDecimal(row["AdditionalAgent"].ToString().PadUpto2()),
                        });
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return pricings;
        }
        public bool AddPricing(Pricing pricing, bool EditFlag)
        {
            bool status = false;
            long res = 0;
            string sql = "";
            try
            {
                SqlCommand cmd = new SqlCommand();
                if (EditFlag)
                {
                    sql = "Update Pricing set PricingType=@PricingType";
                    if (pricing.Logo != null)
                    {
                        sql += ",Logo=@Logo";
                    }
                    sql += ",SetupFee=@SetupFee,PricePerMonth=@PricePerMonth,AdditionalAgent=@AdditionalAgent where PricingId=@PricingId";
                }
                else
                {
                    sql = "Insert into Pricing(PricingType,Logo,SetupFee,PricePerMonth,AdditionalAgent,Status,CreatedBy,CreatedDate) values(@PricingType,@Logo,@SetupFee,@PricePerMonth,@AdditionalAgent,@Status,@CreatedBy,GetDate())";
                }

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@PricingType", pricing.PricingType);
                if (pricing.Logo != null)
                {
                    cmd.Parameters.AddWithValue("@Logo", pricing.LogoText);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Logo", System.DBNull.Value);
                }
                cmd.Parameters.AddWithValue("@SetupFee", pricing.SetupFee);
                cmd.Parameters.AddWithValue("@PricePerMonth", pricing.PricePerMonth);
                cmd.Parameters.AddWithValue("@AdditionalAgent", pricing.AdditionalAgent);
                if (!EditFlag)
                {
                    cmd.Parameters.AddWithValue("@CreatedBy", 1);
                    cmd.Parameters.AddWithValue("@status", 1);
                }
                else
                    cmd.Parameters.AddWithValue("@PricingId", pricing.PricingId);

                res = dbutilities.ExecuteInsertCommand(cmd, true);


                if (res > 0)
                {
                    status = true;
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }
        public bool Delete(int id)
        {
            bool status = false;
            long res = 0;
            try
            {
                string sql = "Delete from Pricing where PricingId=" + id;

                res = dbutilities.ExecuteCommand(sql, true);
                if (res > 0)
                {
                    status = true;
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return status;
        }
        public bool GetTypeStatus(string pricetype)
        {
            bool status = false;
            string sql = "";
            try
            {
                sql = "select * from Pricing where PricingType='" + pricetype + "'";
                dataTable = dbutilities.GetDataTable(sql);
                if (dataTable.Rows.Count > 0)
                {
                    status = true;
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return status;
        }
        #endregion


        #region CompanyType Functions
        AdminData objCTData = new AdminData();


        #region Insert Company Type
        public int InsertCompanyType(CompanyTypes objCTypes)
        {
            int res = 0;
            try
            {
                res = objCTData.InsertCompanTYpe(objCTypes);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        #endregion

        #region Get Company Type
        public List<CompanyTypes> GetCompanyType()
        {
            List<CompanyTypes> objCTypes = new List<CompanyTypes>();
            try
            {
                objCTypes = objCTData.GetCompanyType();
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return objCTypes;
        }
        #endregion

        #region Check Company Type
        public bool CheckCompanyType(string CType)
        {
            bool result = false;
            try
            {
                result = objCTData.CheckCompanyType(CType);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return result;
        }
        #endregion

        #region Edit CompanyTypes
        public List<CompanyTypes> GetCompanyTypeEditById(string CTId)
        {
            List<CompanyTypes> objCTypes = new List<CompanyTypes>();
            try
            {
                objCTypes = objCTData.GetCompanyTypeEditById(CTId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return objCTypes;
        }


        #endregion

        #region Update Company Type
        public int UpdateCompanyType(CompanyTypes objCTypes)
        {
            int res = 0;
            try
            {
                res = objCTData.UpdateCompanyType(objCTypes);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        #endregion

        #region Delete Company Type
        public int DeleteCompanyType(string CompanyTypeId)
        {
            int res = 0;
            try
            {
                res = objCTData.DeleteCompanyType(CompanyTypeId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        #endregion

        #region Get Company Type        public List<Agent> GetBilling()        {            List<Agent> objCTypes = new List<Agent>();            try            {                objCTypes = objCTData.GetBilling();            }            catch (Exception ex) { ex.insertTrace(""); }            return objCTypes;        }
        #endregion

        #endregion

        #region Letter Functions


        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private SqlConnection sqlcon;

        private DBUtilities utilities = new DBUtilities();
        string query = string.Empty;
        DataTable dt = new DataTable();

        public List<LetterTemplate> GetLetterTemplate()
        {
            List<LetterTemplate> letter = new List<LetterTemplate>();
            LetterTemplate lt = new LetterTemplate();
            try
            {
                query = "Select * from LetterTemplate";
                dt = utilities.GetDataTable(query, true);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        letter.Add(new LetterTemplate
                        {
                            LetterId = Convert.ToInt32(row["LetterId"]),
                            LetterName = row["LetterName"].ToString(),
                            LetterText = row["LetterText"].ToString(),
                            isPrimary = Convert.ToBoolean(row["isPrimary"])
                        });
                    }
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return letter;
        }

        public LetterTemplate EditLetter(int letterId)
        {
            LetterTemplate letter = new LetterTemplate();
            try
            {
                query = "Select * from LetterTemplate where LetterId=" + letterId;
                dt = utilities.GetDataTable(query, true);
                foreach (DataRow row in dt.Rows)
                {
                    letter.LetterId = Convert.ToInt32(row["LetterId"]);
                    letter.LetterName = row["LetterName"].ToString();
                    letter.LetterText = row["LetterText"].ToString();
                    letter.isPrimary = Convert.ToBoolean(row["isPrimary"]);
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return letter;
        }

        public bool AddLetter(LetterTemplate letter)
        {
            bool status = false;
            int Id = letter.LetterId;
            object res = string.Empty;

            //long res = 0;
            try
            {
                using (sqlcon = new SqlConnection(connectionString))
                {
                    if (Id == 0)
                    {
                        query = "Insert Into LetterTemplate(LetterName,LetterText,isPrimary)values(@LetterName,@LetterText,@isPrimary); SELECT CAST(scope_identity() AS int)";
                    }
                    else
                    {
                        query = "Update LetterTemplate set LetterName=@LetterName,LetterText=@LetterText,isPrimary=@isPrimary where LetterId =" + Id;
                    }
                    SqlCommand cmd = new SqlCommand(query, sqlcon);
                    cmd.Parameters.AddWithValue("@LetterName", letter.LetterName);
                    cmd.Parameters.AddWithValue("@LetterText", letter.LetterText);
                    cmd.Parameters.AddWithValue("@isPrimary", letter.isPrimary);
                    sqlcon.Open();
                    if (Id == 0)
                    {
                        status = checklettername(letter.LetterName);
                        if (status == true)
                        {
                            int id = (Int32)cmd.ExecuteScalar();
                            if (letter.isPrimary == true)
                            {
                                isprimary(id);
                            }

                        }

                    }
                    else
                    {
                        if (letter.isPrimary == true)
                        {
                            isprimary(Id);
                        }
                        res = utilities.ExecuteInsertCommand(cmd, true);

                        status = true;
                    }

                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }

        public bool DeleteLetter(int letterId)
        {
            bool status = false;
            long res = 0;
            try
            {
                query = "Delete from  LetterTemplate where LetterId=" + letterId;
                res = utilities.ExecuteCommand(query, true);
                status = true;

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }

        public bool checklettername(string name)
        {
            bool status = false;
            DataRow row = null;
            try
            {
                query = "Select * from LetterTemplate where LetterName= '" + name + "'";
                row = utilities.GetDataRow(query);
                if (row == null)
                {
                    status = true;
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return status;
        }

        public void isprimary(int Id)
        {
            long res = 0;
            try
            {
                query = "Update  LetterTemplate Set isPrimary=@isPrimary where LetterId <>" + Id;
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@isPrimary", false);
                cmd.CommandText = query;
                res = utilities.ExecuteInsertCommand(cmd, true);
            }
            catch (Exception ex) { ex.insertTrace(""); }
        }


        #endregion


        #region Insert Service Settings
        public int InsertServiceSettings(ServiceSettings servicesettings)
        {
            int res = 0;
            try
            {
                res = objCTData.InsertServiceSettings(servicesettings);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        #endregion

        #region Get Service Settings
        public ServiceSettings GetServiceSettings()
        {
            ServiceSettings res = new ServiceSettings();
            try
            {

                res = objCTData.GetServiceSettings();
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        #endregion


        public long CreateInvestor(Investor InvestorModel)
        {
            Investor clientModel = new Investor();
            long userstatus = 0;
            try
            {
                userstatus = objCTData.CreateInvestor(InvestorModel);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return userstatus;
        }

        public Investor GetInvestor(string InvestorId)
        {
            Investor objInvestor = new Investor();
            try
            {

                objInvestor = objCTData.GetInvestor(InvestorId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return objInvestor;
        }

        public List<Investor> GetInvestors()
        {
            List<Investor> objInvestor = new
                 List<Investor>();
            try
            {
                objInvestor = objCTData.GetInvestors();
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return objInvestor;
        }

        public long DeleteInvestor(string InvestorId)
        {
            Investor objInvestor = new Investor();
            long userstatus = 0;
            try
            {
                userstatus = objCTData.DeleteInvestor(InvestorId);
                return userstatus;
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return userstatus;
        }

        #region Insert Account Type
        public int InsertAccountTypes(AccountTypes objATypes)
        {
            int res = 0;
            try
            {
                res = objCTData.InsertAccountType(objATypes);
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return res;
        }
        #endregion

        #region Get AccountTpes
        public List<AccountTypes> GetAccountTypes()
        {
            List<AccountTypes> accountTypes = new List<AccountTypes>();
            try
            {
                string query = "Select AccTypeId,AccountType,AccountTypeDetails,Status from AccountTypes where Status=1";

                DataTable dt = dbutilities.GetDataTable(query, true);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        AccountTypes ACTypes = new AccountTypes();
                        ACTypes.AccountType = row["AccountType"].ToString();
                        ACTypes.AccTypeId = Convert.ToInt32(row["AccTypeId"]);
                        ACTypes.AccountTypeDetails = row["AccountTypeDetails"].ToString();
                        ACTypes.Status= row["Status"].ToString();
                        accountTypes.Add(ACTypes);
                    }
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return accountTypes;
        }
        #endregion

        #region Edit AccountType
        public List<AccountTypes> GetAccountTypeEditById(string ATId)
        {
            List<AccountTypes> objATypes = new List<AccountTypes>();
            try
            {
                objATypes = objCTData.GetAccountTypeEditById(ATId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return objATypes;
        }


        #endregion

        #region Update Company Type
        public int UpdateAccountType(AccountTypes objATypes)
        {
            int res = 0;
            try
            {
                res = objCTData.UpdateAccountType(objATypes);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        #endregion

        #region Delete Account Type
        public int DeleteAccountType(string AccTypeId)
        {
            int res = 0;
            try
            {
                res = objCTData.DeleteAccountType(AccTypeId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return res;
        }
        #endregion

        #region Get Challenge Order
        public List<ChallengeOrders> GetChallengeOrders()
        {
            List<ChallengeOrders> COrder = new List<ChallengeOrders>();
            try
            {
                string query = "Select RowId,OrderId,OrderName,ChallengeFileName,OrderStatus, " +
                               "Convert(varchar(15),OrderDate,101)as RDate,Status from ChallengeOrders where Status = 1";

                DataTable dt = dbutilities.GetDataTable(query, true);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ChallengeOrders COrders = new ChallengeOrders();
                        COrders.OrderId = row["OrderId"].ToString();
                        COrders.OrderName =row["OrderName"].ToString();
                        COrders.ChallengeFileName = row["ChallengeFileName"].ToString();
                        COrders.OrderStatus = row["OrderStatus"].ToString();
                        COrders.OrderDate = row["RDate"].ToString();
                        COrders.Status = row["Status"].ToString();
                        COrder.Add(COrders);
                    }
                }
            }

            catch (Exception ex) { ex.insertTrace(""); }
            return COrder;
        }
        #endregion
    }
}
﻿using System;
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

        public bool AddAgent(Agent agent)
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
                + " DateOfEncorp=@DateOfEncorp,FedTaxIdentityNo=@FedTaxIdentityNo,FedTaxIdentityProof=@FedTaxIdentityProof," + nl;
                // cmd.Parameters.AddWithValue("@CreatedDate", date);
                res = utilities.ExecuteInsertCommand(cmd, true);
            //return agentId;
            return status;


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
            catch (Exception ex) { ex.insertTrace(""); }
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
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }
        public bool AddBillingInfo(AgentBilling agentBilling, string from = "")
            AgentBillingTransactions agent = new AgentBillingTransactions();
                        agent = GetBillingTransactionsById(agentBilling.AgentId);
                        billing.PaymentStatus = agent.Status;
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

        public bool checkCurrentPassword(string username = "", string password = "")
        {
            bool status = false;
            string encryptpassword = common.Encrypt(password);
            string sql = "";
            DataTable dataTable = new DataTable();
            try
            {
                sql = "select * from Users where UserName='" + username + "' and Password='" + encryptpassword + "'";
                dataTable = utilities.GetDataTable(sql, true);
                if (dataTable.Rows.Count > 0)
                {
                    status = true;
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

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
        public int AddAgentNew(Agent agent)
            //return agentId;
            return agentId;
        public bool AddAgentBillingTransactions(AgentBillingTransactions agent)

        public string GetPricingById(int Id)

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
            bool status = false;
                string sql = "Insert Into Billing(AgentId,TransactionNo,CardType,PaymentMethodId,PaymentType,PaymentStatus,PaymentDate) values(@AgentId,@TransactionNo,@CardType,@PaymentMethodId,@PaymentType,@PaymentStatus,GETDATE())";
                SqlCommand cmd = new SqlCommand();

                cmd.Parameters.AddWithValue("@AgentId", billing.AgentId);
                cmd.CommandText = sql;
            }
        }
    }
}
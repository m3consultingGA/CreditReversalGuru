using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
	public class Agent
    {
        public string pricing { get; set; }
        //public string DriversLicenseCopyText { get; set; }
        public string Agentstatus { get; set; }
        public HttpPostedFileBase SocialSecCard { get; set; }
        public string SocialSecCardtext { get; set; }
        public string SocialSecCardNo { get; set; }
        public bool checkStaffExists { get; set; }
        public bool checkClientExists { get; set; }
        
        public int AgentId { get; set; }
		public string BusinessName { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string MiddleName { get; set; }
		public string DBA { get; set; }
		public string TypeOfComp { get; set; }
		public string CompanyType { get; set; }
		public string StateOfEncorp { get; set; }
		public HttpPostedFileBase StateOfIncorporationProof { get;set;}
		public string StateOfIncorporationtext { get;set;}
        public string DateOfEncorp { get; set; }
        public string FedTaxIdentityNo { get; set; }
		public HttpPostedFileBase FedTaxIdentityProof { get; set; }
		public string FedTaxIdentitytext { get; set; }
		public string DriversLicenseNumber { get; set; }
		public string DriversLicenseState { get; set; }
		public HttpPostedFileBase DriversLicenseCopy { get; set; }
		public string DriversLicenseCopytext { get; set; }
		public int PricingPlan { get; set; }
		public string PrimaryBusinessAdd1 { get; set; }
        public string PrimaryBusinessEmail { get; set; }
        public string PrimaryBusinessPhone { get; set; }
        public string PrimaryBusinessState { get; set; }
        public string PrimaryBusinessAdd2 { get; set; }
        public string PrimaryBusinessCity { get; set; }
        public string PrimaryBusinessZip { get; set; }
        public string BillingAdd1 { get; set; }
        public string BillingAdd2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingEmail { get; set; }
        public string BillingPhone { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Password { get; set; }

		public string AgentBillingId { get; set; }
		public string BillingType { get; set; }
		public string CardType { get; set; }
		public string CardNumber { get; set; }
		public string ExpiryDate { get; set; }
		public string Month { get; set; }
		public string CVV { get; set; }
		public string BillingZipCode { get; set; }
		public string Status { get; set; }

        public string TransactionNo { get; set; }
        public string PaymentMethodId { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentDate { get; set; }
        public string SCreatedDate { get; set; }        public string NumberofStaff { get; set; }        public string NextBillingDate { get; set; }        public string RegisteredClients { get; set; }        public string PrimaryUser { get; set; }        public string ActiveClients { get; set; }        public string PricingPlans { get; set; }

    }
    public class AgentStaff
    {
        public int AgentStaffId { get; set; }
        public int AgentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Password { get; set; }
        public bool CheckClientExit { get; set; }
        
    }

    public class AgentBilling
    {
        public int AgentBillingId { get; set; }  
		public int AgentId { get; set; }
		public int PricingPlan { get; set; }
		public string BillingType { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
		public string BillingZipCode { get; set; }
		public string Status { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int IsPrimary { get; set; }
    }

    public class AgentBillingTransactions
    {
        public int AgentId { get; set; }
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; }
        public string MessageCode { get; set; }
        public string Description { get; set; }
        public string AuthorizeCode { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Status { get; set; }
        public string IPAddress { get; set; }
    }
    public class Billing    {        public int BillingId { get; set; }        public int AgentId { get; set; }        public string TransactionNo { get; set; }        public string CardType { get; set; }        public int PaymentMethodId { get; set; }        public string PaymentType { get; set; }        public string PaymentStatus { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class Investor
    {
        public string InvestorsCount { get; set; }
        public int? InvestorId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string SSN { get; set; }
        public string MaskedSSN { get; set; }
        public string CurrentEmail { get; set; }
        public string CurrentPhone { get; set; }

        public HttpPostedFileBase FDrivingLicense { get; set; }
        public HttpPostedFileBase FSocialSecCard { get; set; }
        public HttpPostedFileBase FProofOfCard { get; set; }

        public string sDrivingLicense { get; set; }
        public string sSocialSecCard { get; set; }
        public string sProofOfCard { get; set; }

        public string Status { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
        public int? AgentInvestorModelId { get; set; }
        public string getdate { get; set; }
        public int AgentId { get; set; }
        public int AgentStaffId { get; set; }

        public string AgentName { get; set; }
        public string StaffName { get; set; }

        public long? IdentityIqId { get; set; }
        public long? IdInvestorModelId { get; set; }
        public string IdQuestion { get; set; }
        public string IdAnswer { get; set; }
        public string IdUserName { get; set; }
        public string IdPassword { get; set; }
        public string CfmPassword { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string FullAddress { get; set; }
        public string FullAddress2 { get; set; }
        public string ServiceExperiDate { get; set; }
        public string Mode { get; set; }
    }
}
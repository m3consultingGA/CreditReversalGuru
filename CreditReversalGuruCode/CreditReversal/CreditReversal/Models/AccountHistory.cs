using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class Remark
    {
        public string atcustomRemark { get; set; }
        public RemarkCode RemarkCode { get; set; }
    }
    public class RemarkCode
    {
        public string atabbreviation { get; set; }
        public string atdescription { get; set; }
    }
    public class AccountHistory
    {
        public string AccountComments { get; set; }
        public string AccountCondition { get; set; }
        public string ChallengeText { get; set; }
        public string Account { get; set; }
        public string AccountType { get; set; }       
        public string AccountTypeDetail { get; set; }       
        public string BureauCode { get; set; }
        public string AccountStatus { get; set; }
        public string MonthlyPayment { get; set; }
        public string DateOpened { get; set; }
        public string Balance { get; set; }
        public string NoofMonths_terms { get; set; }
        public string HighCredit { get; set; }
        public string CreditLimit { get; set; }
        public string PastDue { get; set; }
        public string PaymentStatus { get; set; }
        public string LastReported { get; set; }
        public string Comments { get; set; }
        public string Accounttransferredorsold { get; set; }
        public string DateLastActive { get; set; }
        public string DateofLastPayment { get; set; }
		public string Bank { get; set; }
		public string Agency { get; set; }
        public int negativeitems { get; set; }
        public string LoanStatus { get; set; }
        public string PastDueDays { get; set; }
    }
}
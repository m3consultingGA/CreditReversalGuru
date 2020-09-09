using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class CreditReportData
    {
        public List<AccountHistory> AccHistory { get; set; }
        public List<Inquires> inquiryDetails { get; set; }
        public List<MonthlyPayStatusHistory> monthlyPayStatusHistoryDetails { get; set; }
        public List<PublicRecord> PublicRecords { get; set; }
    }
    
    public class CreditReport
    {
        public List<TradeLine> Equifax { get; set; }
        public List<TradeLine> Experian { get; set; }
        public List<TradeLine> TransUnion { get; set; }
        public List<InquiryPartition> inquiries { get; set; }
        public List<MonthlyPayStatus> monthlyPayStatusEQ { get; set; }
        public List<MonthlyPayStatus> monthlyPayStatusTU { get; set; }
        public List<MonthlyPayStatus> monthlyPayStatusEX { get; set; }
        public List<MonthlyPayStatusHistory> monthlyPayStatusHistoryList { get; set; }
        public List<PublicRecord> PublicRecords { get; set; }
    }
    public class MonthlyPayStatusHistory
    {
        public string Agency { get; set; }
        public string Bank { get; set; }
        public string AccountNo { get; set; }
        public string atdate { get; set; }
        public string atstatus { get; set; }
    }
    public class MonthlyPayStatus
    {
        public string Agency { get; set; }
        public string Bank { get; set; }
        public string AccountNo { get; set; }
        public string atmonthsReviewed { get; set; }
        public string atmonthlyPayment { get; set; }
        public string atlate90Count { get; set; }
        public string atlate60Count { get; set; }
        public string atlate30Count { get; set; }
        public string atdateLastPayment { get; set; }
        public string attermMonths { get; set; }
        public string atcollateral { get; set; }
        public string atamountPastDue { get; set; }
        public string atworstPatStatusCount { get; set; }
        public int NegitiveItemsCount { get; set; }
        public List<MonthlyPayStatu> monthlyPayStatusEQ { get; set; }
        public List<MonthlyPayStatu> monthlyPayStatusTU { get; set; }
        public List<MonthlyPayStatu> monthlyPayStatusEX { get; set; }
    }
}
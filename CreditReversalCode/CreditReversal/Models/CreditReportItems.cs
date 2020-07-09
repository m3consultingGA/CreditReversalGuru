﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class CreditReportItems
    {
        public string ChallengeText { get; set; }
        public int? CredRepItemsId { get; set; }
        public int? CredReportId { get; set; }
        public string MerchantName { get; set; }
        public string AccountId { get; set; }
        public string AccountType { get; set; }
        public string AccountTypeDetail { get; set; }
        public DateTime OpenDateTime { get; set; }
        public string OpenDate { get; set; }
        public string CurrentBalance { get; set; }
        public string HighestBalance { get; set; }
        public string Status { get; set; }
        public string Challenge { get; set; }
        public string DateReportPulls { get; set; }
        public string Agent { get; set; }
        public string Agency { get; set; }
        public string ChallengeCreatedDate { get; set; }
        public string MonthlyPayment { get; set; }
        public string RoundType { get; set; }
        public string FirstDate { get; set; }
        public string NegativeItemsCount { get; set; }
    }

    public class CreditReportChallenges
        public string filename { get; set; }

    public class CreditItems

    public class CreditsModel
    {
        public string Bureau { get; set; }
        public string BureauCode { get; set; }
        public string DateOpen { get; set; }
        public string HighLimit { get; set; }
        public string MonthlyPayment { get; set; }
        public string AccountBalance { get; set; }
        public string LastReported { get; set; }
        public string AccountStatus { get; set; }
        public string AmountPastDue { get; set; }
        public string Account { get; set; }
        public string AccountNumber { get; set; }
    }
}
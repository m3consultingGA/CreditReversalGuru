using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class ServiceSettings
    {

        public int? ConfigId { get; set; }

        public string DBConnection { get; set; }
        public string SendGridAPIKey { get; set; }
        public string Fromemail { get; set; }
        public string AuthApiLoginId { get; set; }
        public string ApiTransactionKey { get; set; }
        public string SecretKey { get; set; }
        public string AuthEnvironment { get; set; }
        public string Days14MailLine1 {get;set;}
        public string Days14MailLine2 {get;set;}
        public string Days7MailLine1 {get;set;}
        public string Days7MailLine2 {get;set;}
        public string PaymentSuccessLine1 { get;set;}
        public string PaymentSuccessLine2 { get;set;}
        public string PaymentFailureLine1 { get;set;}
        public string PaymentFailureLine2 { get;set;}
        public string NextAttemptMailLine1 { get;set;}
        public string NextAttemptMailLine2 { get;set;}
        public string SecondPaymentFailureLine1 { get; set; }
        public string SecondPaymentFailureLine2 { get; set; }

        public string ChallengesPath { get; set; }
        public string ChallengesPathResult { get; set; }
        public string MailXStreamURL { get; set; }
        public string MXUserid { get; set; }
        public string MXPassword { get; set; }
        public string MXTemplate { get; set; }

    }
}
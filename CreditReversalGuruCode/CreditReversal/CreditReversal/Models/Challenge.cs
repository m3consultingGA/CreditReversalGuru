using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class Challenge
    {
        public int ChallengeId { get; set; }
        public string ChallengeLevel { get; set; }
        public string ChallengeText { get; set; }
        public int AccTypeId { get; set; }
        public string AccountType { get; set; }
        public string Status { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
	public class Inquires
	{
		public string CreditorName { get; set; }
		public string TypeofBusiness { get; set; }
		public string Dateofinquiry { get; set; }
		public string CreditBureau { get; set; }
		public string Heading { get; set; }
		public string CreditInqId { get; set; }
		public string ChallengeText { get; set; }

        public string EQUIFAX { get; set; }
        public string EXPERIAN { get; set; }
        public string TRANSUNION { get; set; }
        public string RoundType { get; set; }
        public string ChallengeStatus { get; set; }
    }
}
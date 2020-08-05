using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class CreditReportFiles
    {
        public string RoundType { get; set; }
        public int ClientId { get; set; }
        public string CRFilename { get; set; }
        public string CreateDate { get; set; }
        public string CreditRepFileId { get; set; }
        public string ClientName { get; set; }
        public string CAgency { get; set; }
        public int isAutoChallenges { get; set; }
        public string mode { get; set; }
    }
}
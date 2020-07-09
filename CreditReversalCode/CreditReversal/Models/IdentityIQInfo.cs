using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class IdentityIQInfo
    {
        public long? IdentityIqId { get; set; }
        public long? ClientId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CfmPassword { get; set; }
    }
}
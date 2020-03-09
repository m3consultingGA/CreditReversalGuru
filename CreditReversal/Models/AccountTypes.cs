using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class AccountTypes
    {
        public int AccTypeId { get; set; }
        public string AccountType { get; set; }
        public string Status { get; set; }
        public string AccountTypeDetails { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class ChallengeOrders
    {
        public long RowId { get; set; }
        public string OrderId { get; set; }
        public string OrderName { get; set; }       
        public string ChallengeFileName { get; set; }
        public string OrderStatus { get; set; }
        public string OrderDate { get; set; }
        public string Status { get; set; }
    }
}
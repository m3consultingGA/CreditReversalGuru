using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class Trace
    {
        public long TraceId { get; set; }
        public string Exception { get; set; }
        public string Error { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class CompanyTypes
    {
        public int CompanyTypeId { get; set; }
        public string CompanyType { get; set; }
        public string Status { get; set; }
        public bool IsIndividual { get; set; }
    }
}
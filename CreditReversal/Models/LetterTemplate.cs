using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class LetterTemplate
    {
        public int LetterId { get; set; }
        public string LetterName { get; set; }
        public string LetterText { get; set; }
        public bool isPrimary { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
	public class SignUp
	{
		public string EmailAddress { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
        public string AgentClientId { get; set; }
        public string BusinessName { get; set; }
        public string CompanyType { get; set; }
    }
}
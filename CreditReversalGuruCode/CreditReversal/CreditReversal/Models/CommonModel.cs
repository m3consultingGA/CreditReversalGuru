using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
	public class CommonModel
	{
		public int count { get; set; }
		public DateTime Date { get; set; } 
	}

	public class NewClient
	{
		public int ClientId { get; set; }
		public string Name { get; set; }
		public DateTime DOB { get; set; }
		public DateTime SignedUpDate { get; set; }
		public string CurrentStatus { get; set; }
		public string NextAction { get; set; }
		public DateTime? DueDate { get; set; }
		public bool IdentityStatus { get; set; }
        public int AgentStaffId { get; set; }        public int AgentId { get; set; }        public string Staff { get; set; }
        public string DBirth { get; set; }
		public string EncryptKey { get; set; }
		public string UrlKey { get; set; }
    }

	public class ActiveClientsBar
	{
		public int clients { get;set;}
		public int Month { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
	public class States
	{
		public int StateId { get; set; }
		public string StateCode { get; set; }
		public string StateName { get; set; }
		public bool Active { get; set; }
	}
}
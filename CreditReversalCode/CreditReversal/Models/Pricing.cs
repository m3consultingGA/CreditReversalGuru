using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
	public class Pricing
	{
		public int PricingId { get; set; }
		public string PricingType { get; set; }
		public string LogoText { get; set; }
		public HttpPostedFileBase Logo { get; set; }
		public decimal SetupFee { get; set; }
		public decimal PricePerMonth { get; set; }
		public decimal AdditionalAgent { get; set; }
		public int Status { get; set; }
		public int CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
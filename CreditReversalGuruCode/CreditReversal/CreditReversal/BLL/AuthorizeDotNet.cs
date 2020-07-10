using System.Collections.Generic;
namespace CreditReversal.BLL
{
    public class AuthorizeDotNetModel
    {
        public string custId { get; set; } //cust number
        public CustomerBillingInfoModel customerBillingInfo { get; set; }
        public CustomerOrderInformationModel customerOrderInfo { get; set; }
        public CustomerShippingInformationModel customerShippingInfo { get; set; }
        public CustomerAdditionalInformationModel customerAdditionalinfo { get; set; }
        public List<LineItemsModel> customerLineItems { get; set; }
        public CreditCardDetailsModel creditCardDetails { get; set; }
    }
    public class CustomerBillingInfoModel
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string EmailAddress { get; set; }
    }
    public class CustomerShippingInformationModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
    public class CustomerAdditionalInformationModel
    {
        public double Tax { get; set; }
        public double Duty { get; set; }
        public double Freight { get; set; }
        public bool TaxExempt { get; set; }
        public string PONumber { get; set; }
    }
    public class CustomerOrderInformationModel
    {
        public string InVoice { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string paymentMethod { get; set; }
        public string TransactionType { get; set; }
    }
    public class LineItemsModel
    {
        public string Item { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Unitprice { get; set; }
        public string ItemTotal { get; set; }
    }
    public class CreditCardDetailsModel
    {
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string ExpDate { get; set; }
        public string CardCode { get; set; }
        public string CardType { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string ShippingCharge { get; set; }
        public string TotalTax { get; set; }
        public string FinalPrice { get; set; }
        public bool TermsAndConditionsChckbx { get; set; }
    }
    public class InvoiceDetailsModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrderNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string PurchasedDate { get; set; }
        public double SubTotal { get; set; }
        public double ShippingCost { get; set; }
        public double Total { get; set; }
        public string AutherizationCode { get; set; }
        public string TransactionNumber { get; set; }
        public string ShippingTaxes { get; set; }
    }
}

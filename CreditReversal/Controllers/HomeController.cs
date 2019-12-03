using CreditReversal.BLL;
using CreditReversal.Models;
using CreditReversal.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net.Http;
using CreditReversal.DAL;
using System.Text;

namespace CreditReversal.Controllers
{
    public class HomeController : Controller
    {
        private AccountFunctions functions = new AccountFunctions();
        private SessionData sessionData = new SessionData();
        private Common common = new Common();
        public string GetIpAddress()
        {
            string ipAddress = "";
            try
            {
                string strHostName = Dns.GetHostName();
                IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);

                IPAddress[] addr = ipEntry.AddressList;
                foreach (IPAddress ipa in addr)
                {
                    if (ipa.AddressFamily.ToString() == "InterNetwork")
                    {
                        ipAddress = ipa.ToString();
                    }
                }
                string hostName = Convert.ToString(ipEntry.HostName);
                return ipAddress;
            }
            catch (Exception e)
            {

            }
            return ipAddress;
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string UserName, string Password)
        {
            try
            {

                DataRow row = functions.Login(UserName, Password);
                string role = "";
                if (row != null)
                {
                    role = row["UserRole"].ToString();
                    Session["UserId"] = row["UserId"];
                    Session["UserName"] = row["UserName"];
                    Session["Name"] = row["UserName"];
                    Session["EmailAddress"] = row["EmailAddress"];
                    Session["UserRole"] = row["UserRole"];
                    Session["Status"] = row["Status"];
                    Session["CreatedBy"] = row["CreatedBy"];
                    Session["CreatedDate"] = row["CreatedDate"];
                    Session["AgentClientId"] = row["AgentClientId"];
                    if (row["UserRole"].ToString() == "agentadmin")
                    {
                        Session["AgentId"] = row["AgentClientId"];
                        AgentFunction agentFunction = new AgentFunction();
                        Agent agent = agentFunction.GetAgent(row["AgentClientId"].ConvertObjectToIntIfNotNull())[0];
                        if (agent != null)
                        {
                            Session["AgentType"] = agent.TypeOfComp;
                            Session["Name"] = agent.FirstName + " " + agent.LastName;
                        }

                    }
                    else if (row["UserRole"].ToString() == "agentstaff")
                    {
                        AgentFunction agentFunction = new AgentFunction();
                        AgentStaff agentStaff = agentFunction.GetStaff(null, Session["AgentClientId"].ToString())[0];
                        if (agentStaff != null)
                        {
                            Session["Name"] = agentStaff.FirstName + " " + agentStaff.LastName;
                        }
                        Session["StaffId"] = row["AgentClientId"];
                    }
                    else if (row["UserRole"].ToString() == "client")
                    {
                        ClientFunction clientFunction = new ClientFunction();
                        ClientModel clientModel = clientFunction.GetClients(null, null, Session["AgentClientId"].ToString())[0];
                        if (clientModel != null)
                        {
                            Session["Name"] = clientModel.FirstName + " " + clientModel.LastName;
                        }
                        Session["ClientId"] = row["AgentClientId"];
                    }

                    switch (role)
                    {
                        case "client":
                            return RedirectToAction("Client", "Dashboard");

                        case "agentstaff":
                            return RedirectToAction("AgentStaff", "Dashboard");

                        case "agentadmin":
                            return RedirectToAction("Agent", "Dashboard");

                        case "admin":
                            return RedirectToAction("Admin", "Dashboard");
                        case "investor":                            return RedirectToAction("Admin", "Dashboard");
                    }
                }
                else
                {
                    TempData["LoginError"] = "Invalid username or password";
                    return RedirectToAction("Index", "Home"); //TODO Changed Home page
                    //Response.Redirect("/Account/SignIn");
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return View();
        }
        public ActionResult HowItWorks()
        {
            //SendMails();
            return View();
        }
        public int SendMails()
        {
            int res = 0;
            try
            {
                string sql = " SELECT c.FirstName+' '+c.LastName agentname,c.BillingEmail, a.AgentId, "
                  +" max(a.PaymentDate) paydate,b.BillingType FROM Billing a, AgentBilling b,Agent c"
                  + " where a.PaymentMethodId = b.AgentBillingId and c.AgentId = b.AgentId "
                  + " group by c.BillingEmail, a.AgentId,b.BillingType,FirstName,LastName";
                DBUtilities utilities = new DBUtilities();
                DataTable dt = utilities.GetDataTable(sql);
                StringBuilder sb14 = new StringBuilder(); StringBuilder sb7 = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime currentdate = DateTime.Now.Date;
                    DateTime nextdue = DateTime.Now.Date;
                    DateTime paydate =  Convert.ToDateTime(dt.Rows[i]["paydate"].ToString());
                    string BillingType = dt.Rows[i]["BillingType"].ToString();
                    if (BillingType.ToUpper() == "MONTHLY")
                    {
                        nextdue = paydate.AddMonths(1);
                    }
                    else if (BillingType.ToUpper() == "Quarterly")
                    {
                        nextdue = paydate.AddMonths(4);
                    }
                    else if (BillingType.ToUpper() == "Half-Yearly")
                    {
                        nextdue = paydate.AddMonths(6);
                    }
                    else if (BillingType.ToUpper() == "Annually")
                    {
                        nextdue = paydate.AddMonths(12);
                    }
                   if(nextdue.AddDays(-14) == currentdate)
                    {
                        sb14.Append(dt.Rows[i]["agentname"].ToString() + "~" + dt.Rows[i]["BillingEmail"].ToString() + "~14^");
                    }
                    else if (nextdue.AddDays(-7) == currentdate)
                    {
                        sb7.Append(dt.Rows[i]["agentname"].ToString() + "~" + dt.Rows[i]["BillingEmail"].ToString() + "~7^");
                    }
                }
                string[] str14C = sb14.ToString().Split('^');
                
                for(int i=1;i< str14C.Length;i++)
                {
                    string[] str14T = str14C[i].Split('~');
                    string agent = str14T[0]; string email = str14T[1]; string mode = str14T[2];
                    //send mails
                }
            }
            catch (Exception ex)
            { }
            return res;
        }
        public int AgentPayments()
        {
            int res = 0;
            try
            {
                string sql = " SELECT p.SetupFee,b.CardNumber,b.CardType,b.CVV,b.ExpiryDate , "
                  +" c.FirstName+' '+c.LastName agentname,c.BillingEmail, a.AgentId, "
                  + " max(a.PaymentDate) paydate,b.BillingType FROM Billing a, AgentBilling b,Agent c"
                  + " where a.PaymentMethodId = b.AgentBillingId and c.AgentId = b.AgentId "
                  + " group by c.BillingEmail, a.AgentId,b.BillingType,FirstName,LastName, "
                  +" b.CardNumber,b.CardType,b.CVV,b.ExpiryDate , p.SetupFee";
                DBUtilities utilities = new DBUtilities();
                DataTable dt = utilities.GetDataTable(sql);
                StringBuilder sb14 = new StringBuilder(); StringBuilder sb7 = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime currentdate = DateTime.Now.Date;
                    DateTime nextdue = DateTime.Now.Date;
                    DateTime paydate = Convert.ToDateTime(dt.Rows[i]["paydate"].ToString());
                    string BillingType = dt.Rows[i]["BillingType"].ToString();
                    if (BillingType.ToUpper() == "MONTHLY")
                    {
                        nextdue = paydate.AddMonths(1);
                    }
                    else if (BillingType.ToUpper() == "Quarterly")
                    {
                        nextdue = paydate.AddMonths(4);
                    }
                    else if (BillingType.ToUpper() == "Half-Yearly")
                    {
                        nextdue = paydate.AddMonths(6);
                    }
                    else if (BillingType.ToUpper() == "Annually")
                    {
                        nextdue = paydate.AddMonths(12);
                    }
                    if (nextdue == currentdate)
                    {
                        sb14.Append(dt.Rows[i]["agentname"].ToString() + "~" + dt.Rows[i]["BillingEmail"].ToString() + "~14^");
                    }
                   
                }
                string[] str14C = sb14.ToString().Split('^');

                for (int i = 1; i < str14C.Length; i++)
                {
                    string[] str14T = str14C[i].Split('~');
                    string agent = str14T[0]; string email = str14T[1]; string mode = str14T[2];
                    //send mails
                }
            }
            catch (Exception ex)
            { }
            return res;
        }
        public ActionResult ContactUs()
        {

            ////var response = AuthPayment.Pay(500);
            //AuthorizeDotNetModel parentModel = new AuthorizeDotNetModel();
            //CreditCardDetailsModel creditCardDetails = new CreditCardDetailsModel();
            //creditCardDetails.CardNumber = "4111111111111111";
            //creditCardDetails.ExpDate = "1028";
            //creditCardDetails.CardCode = "123";
            //parentModel.creditCardDetails = creditCardDetails;

            //CustomerBillingInfoModel customerBillingInfo = new CustomerBillingInfoModel();
            //customerBillingInfo.FirstName = "John";
            //customerBillingInfo.LastName = "Doe";
            //customerBillingInfo.Address = "123 My St";
            //customerBillingInfo.City = "OurTown";
            //customerBillingInfo.ZipCode = "98004";
            //customerBillingInfo.Country = "USA";
            //customerBillingInfo.State = "";
            //customerBillingInfo.CompanyName = "";
            //customerBillingInfo.EmailAddress = "";
            //customerBillingInfo.PhoneNumber = "";
            //customerBillingInfo.Fax = "";
            //parentModel.customerBillingInfo = customerBillingInfo;

            //List<LineItemsModel> lineItems = new List<LineItemsModel>();
            //LineItemsModel customerLineItems = new LineItemsModel();
            //customerLineItems.Item = "1";
            //customerLineItems.ItemName = "CreditReversal";
            //customerLineItems.Description = "CreditReversalGuru";
            //customerLineItems.Quantity = Convert.ToInt32(1);
            //customerLineItems.Unitprice = Convert.ToDouble(500.00);
            //lineItems.Add(customerLineItems);
            //parentModel.customerLineItems = lineItems;

            //CustomerOrderInformationModel customerOrderInfo = new CustomerOrderInformationModel();
            //customerOrderInfo.InVoice = "";
            //customerOrderInfo.Description = "";
            //parentModel.customerOrderInfo = customerOrderInfo;

            //CustomerShippingInformationModel customerShippingInfo = new CustomerShippingInformationModel();
            //customerShippingInfo.FirstName = "Satish";
            //customerShippingInfo.LastName = "Y";
            //customerShippingInfo.Address = "Madhurawada";
            //customerShippingInfo.City = "Visakhapatnam";
            //customerShippingInfo.ZipCode = "530041";
            //customerShippingInfo.Country = "USA";
            //customerShippingInfo.State = "";
            //customerShippingInfo.CompanyName = "";
            //parentModel.customerShippingInfo = customerShippingInfo;

            //var response = (dynamic)null;
            //response = AuthPayment.AuthorizeandCaptureTransaction(new Decimal(500.00), parentModel, "");
            //if (response != null)
            //{
            //    AgentBillingTransactions agentBillingTransactions = new AgentBillingTransactions();
            //    agentBillingTransactions.IPAddress = response.IPAddress;
            //    if (response.Status == "SUCCESS")
            //    {
            //        ViewBag.Msg = "Payment Success.";
            //        agentBillingTransactions.TransactionId = response.TransactionId;
            //        agentBillingTransactions.ResponseCode = response.ResponseCode;
            //        agentBillingTransactions.MessageCode = response.MessageCode;
            //        agentBillingTransactions.Description = response.Description;
            //        agentBillingTransactions.AuthorizeCode = response.AuthorizeCode;
            //        agentBillingTransactions.Status = response.Status;
            //    }
            //    else
            //    {
            //        string errormsg = "";
            //        string ErrorCode = response.ErrorCode != null ? response.ErrorCode : "";
            //        string ErrorText = response.ErrorMessage != null ? response.ErrorMessage : "";
            //        string msgInvalidCard = "your card issuer due to " + "<b>\"invalid credit card number\"</b>",
            //            msgInvalidCVV = "your card issuer due to " + "<b>\"invalid CVV number\"</b>",
            //            msgCardExpired = "your card issuer due to " + "<b>\"expiration of your credit card\"</b>",
            //            msgInvalidExpirationDate = "your card issuer due to " + "<b>\"invalid credit card expiration date\"</b>",
            //            msgPaymentGateway = "payment gateway issues";
            //        //Payment failure mail send to customer
            //        if (ErrorCode == "8") //The credit card has expired
            //        {
            //            errormsg = msgCardExpired;
            //        }
            //        else if (ErrorCode == "6" || ErrorCode == "37" || ErrorText.ToUpper().Contains("CARDNUMBER")) //The credit card number is invalid.
            //        {
            //            errormsg = msgInvalidCard;
            //        }
            //        else if (ErrorCode == "7" || ErrorText.ToUpper().Contains("EXPIRATIONDATE")) //The credit card expiration date is invalid.
            //        {
            //            errormsg = msgInvalidExpirationDate;
            //        }
            //        else if (ErrorCode == "11") //A duplicate transaction has been submitted.
            //        {
            //            errormsg = msgPaymentGateway;
            //        }
            //        else if (ErrorCode == "13") //The merchant API Login ID is invalid or the account is inactive.
            //        {
            //            errormsg = msgPaymentGateway;
            //        }
            //        else if (ErrorCode == "16") //The transaction was not found.
            //        {
            //            errormsg = msgPaymentGateway;
            //        }
            //        else if (ErrorCode == "17" || ErrorCode == "28") //The merchant does not accept this type of credit card.
            //        {
            //            errormsg = msgInvalidCard;
            //        }
            //        else if (ErrorCode == "19" || ErrorCode == "23" || ErrorCode == "25" || ErrorCode == "26") //An error occurred during processing. Please try again in 5 minutes..
            //        {
            //            errormsg = msgPaymentGateway;
            //        }
            //        else if (ErrorCode == "35") //An error occurred during processing.Call Merchant Service Provider.
            //        {
            //            errormsg = msgPaymentGateway;
            //        }
            //        else if (ErrorCode == "44" || ErrorCode == "65" || ErrorCode == "78" || ErrorText.ToUpper().Contains("CARDCODE")) //Invalid CVV.
            //        {
            //            errormsg = msgInvalidCVV;
            //        }
            //        string errMsg = "Your payment was declined  by " + errormsg;
            //        errMsg = errMsg.Replace("<b>", " "); errMsg = errMsg.Replace("</b>", " ");
            //        ViewBag.Msg = errMsg;
            //        agentBillingTransactions.ErrorCode = response.ErrorCode;
            //        agentBillingTransactions.ErrorMessage = response.ErrorMessage;
            //        agentBillingTransactions.Status = response.Status;
            //    }
            //}
            return View();
        }

        public ActionResult Signout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult NewPage()
        {
            string url = "https://www.w3schools.com/";
            HttpClient client = new HttpClient(); string result = string.Empty;
            using (HttpResponseMessage response = client.GetAsync(url).Result)
            {
                using (HttpContent content = response.Content)
                {
                    result = content.ReadAsStringAsync().Result;
                }
            }

            //var html = new System.Net.WebClient().DownloadString("https://www.identityiq.com/login.aspx");
            System.IO.File.WriteAllText(Server.MapPath("~/Content/sample2.html"), result);
            return View();
        }
    }
}
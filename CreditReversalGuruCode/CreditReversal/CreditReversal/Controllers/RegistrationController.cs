using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CreditReversal.Models;
using CreditReversal.BLL;
using CreditReversal.Utilities;
using System.IO;
using System.Net;

namespace CreditReversal.Controllers
{
    public class RegistrationController : Controller
    {
        private AccountFunctions accountFunctions = new AccountFunctions();
        private AccountController accountController = new AccountController();
        private RegistrationFunctions functions = new RegistrationFunctions();
        private SessionData sessionData = new SessionData();
        private Common common = new Common();

        public ActionResult Thankyou()
        {
            return View();
        }
        // GET: Registration
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Agent()
        {
            int AgentClientId = sessionData.GetAgentClientId();
            Session["UserId"] = sessionData.GetAgentClientId();
            if (AgentClientId == 0)
            {
                Response.Redirect("/Account/SignIn");
            }
            Agent agent = new Agent();

            agent = accountFunctions.GetAgent(AgentClientId.ToString());
            agent.Agentstatus = "AgentStatus";

            ViewBag.years = common.GetYears();
            List<Pricing> pricings = functions.GetPricing();
            ViewBag.pricing = pricings;
            string str = string.Join(", ", pricings.Select(x => x.PricingType).ToArray());
            ViewBag.pricingstring = str;
            List<CompanyTypes> companyTypes = functions.GetCompanyTypes();
            string comstr = string.Join(", ", companyTypes.Where(y=>y.CompanyType!= "SOLO PROP").Select(x => x.CompanyType).ToArray());
            ViewBag.companystring = comstr;
            ViewBag.companytypes = companyTypes;
            ViewBag.Dasboard = sessionData.getDasboard();
            ViewBag.states = functions.GetStates();
            ViewBag.Agentstatus = "AgentStatus";
            return View(agent);
        }
        //[Authorization]
        [HttpPost]
        public ActionResult Agent(Agent agent)
        {

            string pay = string.Empty;
            bool loginstatus = sessionData.GetLoginStatus();
            bool status = false;
            int Agentclient = sessionData.GetAgentClientId();
            AgentBilling agentBilling = new AgentBilling();

            try
            {
                agent.AgentId = Agentclient;
                if (agent.FedTaxIdentityProof != null)
                {
                    common.DeleteFile(agent.FedTaxIdentitytext);
                    string TaxIdentity = Path.GetFileName(agent.FedTaxIdentityProof.FileName);
                    string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + TaxIdentity);
                    agent.FedTaxIdentityProof.SaveAs(physicalPath);
                    agent.FedTaxIdentitytext = "Agent-" + agent.AgentId + "-" + TaxIdentity;
                }
                if (agent.StateOfIncorporationProof != null)
                {
                    common.DeleteFile(agent.StateOfIncorporationtext);
                    string ImageName = Path.GetFileName(agent.StateOfIncorporationProof.FileName);
                    string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + ImageName);
                    agent.StateOfIncorporationProof.SaveAs(physicalPath);
                    agent.StateOfIncorporationtext = "Agent-" + agent.AgentId + "-" + ImageName;
                }
                if (agent.DriversLicenseCopy != null)
                {
                    common.DeleteFile(agent.DriversLicenseCopytext);
                    string ImageName = Path.GetFileName(agent.DriversLicenseCopy.FileName);
                    string physicalPath = Server.MapPath("~/documents/" + "AgentDriversLicense-" + agent.AgentId + "-" + ImageName);
                    agent.DriversLicenseCopy.SaveAs(physicalPath);
                    agent.DriversLicenseCopytext = "AgentDriversLicense-" + agent.AgentId + "-" + ImageName;
                }

                string role = string.IsNullOrEmpty(sessionData.GetUserRole()) ? string.Empty : sessionData.GetUserRole().ToLower();
                if (role == "agentadmin")
                {
                    pay = "Success";
                }
                else
                {
                    pay = Payment(agent);
                }

                if (pay == "Success")
                {
                    agent.AgentId = Agentclient;

                    status = functions.AddAgent(agent);

                    if (status)
                    {
                        agentBilling.AgentBillingId = agent.AgentBillingId.StringToInt(0);
                        agentBilling.AgentId = agent.AgentId;
                        agentBilling.BillingType = agent.BillingType;
                        agentBilling.CardType = agent.CardType;
                        agentBilling.CardNumber = agent.CardNumber;
                        agentBilling.ExpiryDate = agent.Month + "-" + agent.ExpiryDate;
                        agentBilling.BillingZipCode = agent.BillingZipCode;
                        agentBilling.CVV = agent.CVV;

                        SignUp signUp = new SignUp();

                        bool agentbillingstatus = functions.AddBillingInfo(agentBilling);
                        if (agentbillingstatus)
                        {
                            accountFunctions.UpdateUserStatus(Agentclient);
                            if (agent.Password != null)
                            {
                                signUp.UserName = agent.UserName;
                                signUp.Password = agent.Password;
                                signUp.AgentClientId = agent.AgentId.ToString();
                                functions.UpdateUser(signUp);
                            }

                            if (loginstatus)
                            {
                                return RedirectToAction("Agent", "Dashboard");
                            }
                            else
                            {
                                Users user = accountFunctions.getUser(agent.UserName);
                                common.SendMail(agent.UserName, "Agent Registration", "AgentRegistration", agent.FirstName, "", "", user.Password);
                                return RedirectToAction("Thankyou", "Registration");
                            }
                        }
                    }
                }
                else
                {

                    int id = agent.TypeOfComp.StringToInt(0);

                    agent.TypeOfComp = functions.GetCompanyTypeById(id);

                    //string Plan = functions.GetPricingById(agent.PricingPlan);
                    //agent.PricingPlan = Plan.StringToInt(0);

                    //int AgentClientId = sessionData.GetAgentClientId();
                    //agent = accountFunctions.GetAgent(AgentClientId.ToString());

                    ViewBag.years = common.GetYears();
                    ViewBag.pricing = functions.GetPricing();
                    ViewBag.companytypes = functions.GetCompanyTypes();
                    ViewBag.Dasboard = sessionData.getDasboard();
                    ViewBag.states = functions.GetStates();
                    TempData["errormsg"] = pay;
                    ViewBag.errormsg = pay;
                    // return View(agent);


                }


            }
            catch (Exception ex) { ex.insertTrace(""); }

            return View(agent);
        }

        public ActionResult ValidatePassword(string UserName = "", string Password = "")
        {
            bool status = false;
            if (UserName != "" && Password != "")
            {
                status = functions.checkCurrentPassword(UserName, Password);
            }
            return Json(status);
        }
        public string Payment(Agent agent)
        {
            try
            {
                //var response = AuthPayment.Pay(500);
                AuthorizeDotNetModel parentModel = new AuthorizeDotNetModel();
                CreditCardDetailsModel creditCardDetails = new CreditCardDetailsModel();
                creditCardDetails.CardNumber = agent.CardNumber;

                //   string r = agent.CardNumber;
                string expdate = agent.Month + agent.ExpiryDate;
                creditCardDetails.ExpDate = expdate;
                //creditCardDetails.ExpiryMonth = agent.Month;
                creditCardDetails.CardCode = agent.CVV;
                parentModel.creditCardDetails = creditCardDetails;

                CustomerBillingInfoModel customerBillingInfo = new CustomerBillingInfoModel();
                customerBillingInfo.FirstName = agent.FirstName;
                customerBillingInfo.LastName = agent.LastName;
                customerBillingInfo.Address = agent.PrimaryBusinessAdd1;
                customerBillingInfo.City = agent.PrimaryBusinessCity;
                customerBillingInfo.ZipCode = agent.PrimaryBusinessZip;
                //customerBillingInfo.Country = agent.b
                customerBillingInfo.State = agent.PrimaryBusinessState;
                customerBillingInfo.CompanyName = agent.CompanyType;
                customerBillingInfo.EmailAddress = agent.PrimaryBusinessEmail;
                customerBillingInfo.PhoneNumber = agent.PrimaryBusinessPhone;
                customerBillingInfo.Fax = agent.FedTaxIdentityNo;
                parentModel.customerBillingInfo = customerBillingInfo;

                List<LineItemsModel> lineItems = new List<LineItemsModel>();
                LineItemsModel customerLineItems = new LineItemsModel();
                customerLineItems.Item = "1";
                customerLineItems.ItemName = "CreditReversal";
                customerLineItems.Description = "CreditReversalGuru";
                customerLineItems.Quantity = Convert.ToInt32(1);
                //string price = functions.GetPricingById(agent.PricingPlan);
                //double plan = Convert.ToDouble(price);
                //customerLineItems.Unitprice = Convert.ToDouble(price);

                string price = functions.GetPricingById(agent.PricingPlan);
                string[] strtemp = price.Split('$');
                string pricedouble = strtemp[1];
                double plan = Convert.ToDouble(pricedouble);

                lineItems.Add(customerLineItems);
                parentModel.customerLineItems = lineItems;

                CustomerOrderInformationModel customerOrderInfo = new CustomerOrderInformationModel();
                customerOrderInfo.InVoice = "";
                customerOrderInfo.Description = "";
                parentModel.customerOrderInfo = customerOrderInfo;

                CustomerShippingInformationModel customerShippingInfo = new CustomerShippingInformationModel();
                customerShippingInfo.FirstName = agent.FirstName;
                customerShippingInfo.LastName = agent.LastName;
                customerShippingInfo.Address = agent.PrimaryBusinessAdd1;
                customerShippingInfo.City = agent.PrimaryBusinessCity;
                customerShippingInfo.ZipCode = agent.PrimaryBusinessZip;
                //customerShippingInfo.Country = "";
                customerShippingInfo.State = agent.PrimaryBusinessState;
                customerShippingInfo.CompanyName = agent.CompanyType;
                parentModel.customerShippingInfo = customerShippingInfo;

                var response = (dynamic)null;
                response = AuthPayment.AuthorizeandCaptureTransaction(new Decimal(plan), parentModel, "");

                if (response != null)
                {
                    AgentBillingTransactions agentBillingTransactions = new AgentBillingTransactions();

                    agentBillingTransactions.IPAddress = GetIpAddress();

                    agentBillingTransactions.AgentId = agent.AgentId;
                    if (response.Status == "SUCCESS")
                    {
                        ViewBag.Msg = "Success";
                        agentBillingTransactions.TransactionId = response.TransactionId;
                        agentBillingTransactions.ResponseCode = response.ResponseCode;
                        agentBillingTransactions.MessageCode = response.MessageCode;
                        agentBillingTransactions.Description = response.Description;
                        agentBillingTransactions.AuthorizeCode = response.AuthorizeCode;
                        agentBillingTransactions.Status = response.Status;

                    }
                    else
                    {
                        string errormsg = "";
                        string ErrorCode = response.ErrorCode != null ? response.ErrorCode : "";
                        string ErrorText = response.ErrorMessage != null ? response.ErrorMessage : "";
                        string msgInvalidCard = "Your card issuer due to " + "<b>\" Invalid credit card number. \"</b>",
                         msgInvalidCVV = "Your card issuer due to " + "<b>\" Invalid CVV number. \"</b>",
                         msgCardExpired = "Your card issuer due to " + "<b>\" Expiration of your credit card. \"</b>",
                         msgInvalidExpirationDate = "Your card issuer due to " + "<b>\" Invalid credit card expiration date. \"</b>",
                         msgPaymentGateway = " Payment gateway issues";
                        //Payment failure mail send to customer
                        if (ErrorCode == "8") //The credit card has expired
                        {
                            errormsg = msgCardExpired;
                        }
                        else if (ErrorCode == "6" || ErrorCode == "37" || ErrorText.ToUpper().Contains("CARDNUMBER")) //The credit card number is invalid.
                        {
                            errormsg = msgInvalidCard;
                        }
                        else if (ErrorCode == "7" || ErrorText.ToUpper().Contains("EXPIRATIONDATE")) //The credit card expiration date is invalid.
                        {
                            errormsg = msgInvalidExpirationDate;
                        }
                        else if (ErrorCode == "11") //A duplicate transaction has been submitted.
                        {
                            errormsg = msgPaymentGateway;
                        }
                        else if (ErrorCode == "13") //The merchant API Login ID is invalid or the account is inactive.
                        {
                            errormsg = msgPaymentGateway;
                        }
                        else if (ErrorCode == "16") //The transaction was not found.
                        {
                            errormsg = msgPaymentGateway;
                        }
                        else if (ErrorCode == "17" || ErrorCode == "28") //The merchant does not accept this type of credit card.
                        {
                            errormsg = msgInvalidCard;
                        }
                        else if (ErrorCode == "19" || ErrorCode == "23" || ErrorCode == "25" || ErrorCode == "26") //An error occurred during processing. Please try again in 5 minutes..
                        {
                            errormsg = msgPaymentGateway;
                        }
                        else if (ErrorCode == "35") //An error occurred during processing.Call Merchant Service Provider.
                        {
                            errormsg = msgPaymentGateway;
                        }
                        else if (ErrorCode == "44" || ErrorCode == "65" || ErrorCode == "78" || ErrorText.ToUpper().Contains("CARDCODE")) //Invalid CVV.
                        {
                            errormsg = msgInvalidCVV;
                        }
                        string errMsg = "Payment was declined. " + errormsg;
                        errMsg = errMsg.Replace("<b>", " "); errMsg = errMsg.Replace("</b>", " ");
                        ViewBag.Msg = errMsg.Replace("&quot;", " ");
                        agentBillingTransactions.ErrorCode = response.ErrorCode;
                        agentBillingTransactions.ErrorMessage = response.ErrorMessage;
                        agentBillingTransactions.Status = response.Status;
                    }

                    functions.AddAgentBillingTransactions(agentBillingTransactions);

                }
                return ViewBag.Msg;
            }
            catch (Exception e)
            {
                e.insertTrace("");
            }
            return"";
        }
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
        public ActionResult Failure()
        {
            return View();
        }
    }
}
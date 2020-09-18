using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CreditReversal.Models;
using CreditReversal.BLL;
using CreditReversal.Utilities;
using System.IO;

namespace CreditReversal.Controllers
{[Authorization]
    public class AgentController : Controller
    {
        private DashboardFunctions DashFunction = new DashboardFunctions();
        private RegistrationFunctions functions = new RegistrationFunctions();
        private AccountFunctions accountFunctions = new AccountFunctions();

        public SessionData sessionData = new SessionData();
        private AgentFunction agentfunction = new AgentFunction();
        private Common common = new Common();

      
        // GET: Agent

        public ActionResult Thankyou()
        {
            
            return View();
        }
        public ActionResult Index()
        {
            List<Agent> agent = new List<Agent>();
            try
            {
                agent = agentfunction.GetAgent();
                ViewBag.agent = agent;
                ViewBag.Dasboard = sessionData.getDasboard();
            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return View();
        }
        public ActionResult AddAgent()
        {
            ViewBag.Dasboard = sessionData.getDasboard();
            return View();
        }

        [HttpPost]
        public ActionResult AddAgent(Agent agent)
        {
            agent.CreatedBy = sessionData.GetUserID().StringToInt(0);
            int status = 0;
            bool userstatus = false;
            Users user = new Users();
            user.UserName = agent.UserName;
            string pass = common.Encrypt(agent.Password);
            user.Password = pass;
            user.EmailAddress = agent.PrimaryBusinessEmail;
            user.UserRole = "agentadmin";
            user.CreatedBy = sessionData.GetUserID().StringToInt(0);
            try
            {
                status = agentfunction.AddAgent(agent);
                user.AgentClientId = status;
                if (status != 0)
                {
                    userstatus = agentfunction.AddUser(user);
                }

            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return Json(userstatus);
        }

        public JsonResult EditAgent(string AgentID)
        {
            Agent agent = new Agent();
            Users user = new Users();
            string Id = AgentID;
            string UserRole = "agentadmin";
            try
            {
                agent = agentfunction.EditAgent(AgentID);
                user = agentfunction.EditUser(Id, UserRole);
                agent.UserName = user.UserName;
                agent.Password = user.Password;

            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return Json(agent);
        }
        [Authorization]
        public ActionResult AddStaff()
        {
            List<AgentStaff> staff = new List<AgentStaff>();
            try
            {
                string role = sessionData.GetUserRole();
                if (role != "admin")
                {
                    staff = agentfunction.GetStaff(sessionData.GetAgentId(), sessionData.GetStaffId());
                }
                else
                {
                    staff = agentfunction.GetStaff();
                }
                ViewBag.staff = staff;

                ViewBag.Dasboard = sessionData.getDasboard();
            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return View();
        }

        [HttpPost]
        public ActionResult AddStaff(AgentStaff staff)
        {
            List<AgentStaff> agstaff = new List<AgentStaff>();
            staff.AgentId = sessionData.GetAgentId().StringToInt(0);
            staff.CreatedBy = sessionData.GetUserID().StringToInt(0);

            Users user = new Users();
            bool userstatus = false;
            user.UserName = staff.Email;

            string Random = Common.CreateRandomPassword();

            string pass = common.Encrypt(Random);

            user.Password = pass;
            user.EmailAddress = staff.Email;
            user.UserRole = "agentstaff";
            user.CreatedBy = sessionData.GetUserID().StringToInt(0);
            int status = 0;

            try
            {
                agstaff = agentfunction.GetStaff(sessionData.GetAgentId(), sessionData.GetStaffId());
                status = agentfunction.AddStaff(staff);
                user.AgentClientId = status;
                if (status != 0)
                {
                    ViewBag.staff = agstaff;
                    userstatus = agentfunction.AddUser(user);

                    string subject = "Staff Registration";
                    string type = "Staff-Registration";
                    string Email = user.EmailAddress;
                    string name = staff.FirstName;
                    if (userstatus == true)
                    {
                        common.SendMail(Email, subject, type, name, "", "", Random);
                    }
                }

                ViewBag.Dasboard = sessionData.getDasboard();
            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return Json(userstatus);
        }

        public JsonResult EditStaff(string staffId)
        {
            AgentStaff staff = new AgentStaff();
            Users user = new Users();
            string Id = staffId;
            string UserRole = "agentstaff";
            try
            {
                staff = agentfunction.EditStaff(staffId);
                user = agentfunction.EditUser(Id, UserRole);
                staff.UserName = user.UserName;
                staff.Password = user.Password;

            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return Json(staff);
        }
        public JsonResult DeleteAgentStaff(string Id)
        {
            bool status = false;
            try
            {
                status = agentfunction.DeleteAgentStaff(Id);

            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return Json(status);
        }


        //Agent Billing
        public ActionResult AddBilling()
        {
            ViewBag.Dasboard = sessionData.getDasboard();
            ViewBag.Years = common.GetYears();
            return View();
        }

        [HttpPost]
        public ActionResult AddBilling(AgentBilling agentbilling)
        {
            bool userstatus = false;
            try
            {
                agentbilling.CreatedBy = sessionData.GetUserID().StringToInt(0);
                agentbilling.AgentId = sessionData.GetAgentId().StringToInt(0);
                agentbilling.CreatedDate = System.DateTime.Now.ConvertObjectToStringIfNotNull();
                agentbilling.Status = "1";
                userstatus = agentfunction.AddBilling(agentbilling);
            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return Json(userstatus);
        }


        public JsonResult GetAgentBilling(int AgentBillingId)
        {
            AgentBilling agentB = new AgentBilling();            
            try
            {
                agentB = agentfunction.GetAgentBilling(AgentBillingId);                
            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return Json(agentB);
        }

        public ActionResult BillingList()
        {
            List<AgentBilling> agentB = new List<AgentBilling>();
            try
            {
                agentB = agentfunction.GetAgentBillings(sessionData.GetAgentId().StringToInt(0));
                ViewBag.AgentBilling = agentB;
                ViewBag.Dasboard = sessionData.getDasboard();
            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return View();
        }

        public JsonResult DeleteAgentBilling(int AgentBillingId)
        {
            bool status = false;
            try
            {
                status = agentfunction.DeleteAgentBilling(AgentBillingId);

            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return Json(status);
        }
        public JsonResult DeleteAgent(string agentId)
        {
            bool status = false;
            try
            {
                string from = "Admin";
                status = agentfunction.DeleteAgent(agentId, from);

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(status);
        }
        [HttpPost]
        public ActionResult AgentById(string AgentId)
        {

            Agent agent = new Agent();
            try
            {
                string from = "Admin";
                agent = accountFunctions.GetAgent(AgentId, from);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            ViewBag.years = common.GetYears();
            ViewBag.pricing = functions.GetPricing();
            return Json(agent);
            //return View(agent);
        }


        [HttpGet]
        public ActionResult AddNewAgent()
        {
            try
            {
                ViewBag.States = functions.GetStates();
                ViewBag.companytypes = functions.GetCompanyTypes();
                ViewBag.years = common.GetYears();
                ViewBag.pricing = functions.GetPricing();
                ViewBag.Dasboard = sessionData.getDasboard();
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return View();
        }

        [HttpPost]
        public ActionResult AddNewAgent(Agent agent)
        {
            bool status = false;
            bool loginstatus = sessionData.GetLoginStatus();
            int Agentclient = sessionData.GetAgentClientId();
            AgentBilling agentBilling = new AgentBilling();
            Agent agente = new Agent();

            try
            {
                if (agent.AgentId != 0)
                {
                    string from = "Admin";
                    agente = accountFunctions.GetAgent(agent.AgentId.ToString(), from);


                    if (agent.FedTaxIdentityProof != null)
                    {
                        common.DeleteFile(agent.FedTaxIdentitytext);
                        string TaxIdentity = Path.GetFileName(agent.FedTaxIdentityProof.FileName);
                        string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + TaxIdentity);
                        agent.FedTaxIdentityProof.SaveAs(physicalPath);
                        agent.FedTaxIdentitytext = "Agent-" + agent.AgentId + "-" + TaxIdentity;
                    }
                    else
                    {
                        agent.FedTaxIdentitytext = agente.FedTaxIdentitytext;
                    }
                    if (agent.StateOfIncorporationProof != null)
                    {
                        common.DeleteFile(agent.StateOfIncorporationtext);
                        string ImageName = Path.GetFileName(agent.StateOfIncorporationProof.FileName);
                        string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + ImageName);
                        agent.StateOfIncorporationProof.SaveAs(physicalPath);
                        agent.StateOfIncorporationtext = "Agent-" + agent.AgentId + "-" + ImageName;
                    }
                    else
                    {
                        agent.StateOfIncorporationtext = agente.StateOfIncorporationtext;
                    }
                    if (agent.SocialSecCard != null)
                    {
                        common.DeleteFile(agent.SocialSecCardtext);
                        string TaxIdentity = Path.GetFileName(agent.SocialSecCard.FileName);
                        string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + TaxIdentity);
                        agent.SocialSecCard.SaveAs(physicalPath);
                        agent.FedTaxIdentitytext = "Agent-" + agent.AgentId + "-" + TaxIdentity;
                        agent.FedTaxIdentityNo = agent.SocialSecCardNo;
                    }
                    else
                    {
                        agent.SocialSecCardtext = agente.FedTaxIdentitytext;
                    }
                    if (agent.DriversLicenseCopy != null)
                    {
                        common.DeleteFile(agent.SocialSecCardtext);
                        string TaxIdentity = Path.GetFileName(agent.DriversLicenseCopy.FileName);
                        string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + TaxIdentity);
                        agent.DriversLicenseCopy.SaveAs(physicalPath);
                        agent.DriversLicenseCopytext = "Agent-" + agent.AgentId + "-" + TaxIdentity;

                    }
                    else
                    {
                        agent.DriversLicenseCopytext = agente.DriversLicenseCopytext;
                    }
                    agent.PrimaryBusinessEmail = agente.PrimaryBusinessEmail;
                    agent.UserName = agente.PrimaryBusinessEmail;
                    agent.CreatedBy = sessionData.GetUserID().StringToInt(0);
                    status = functions.AddAgent(agent);
                    if (status)
                    {
                        string year = agent.ExpiryDate;
                        year = year.Substring(2, 2);

                        agentBilling.AgentBillingId = agent.AgentBillingId.StringToInt(0);
                        agentBilling.AgentId = agent.AgentId;
                        agentBilling.BillingType = agent.BillingType;
                        agentBilling.CardType = agent.CardType;
                        agentBilling.CardNumber = agent.CardNumber;
                        agentBilling.ExpiryDate = agent.Month + "-" + year;
                        agentBilling.BillingZipCode = agent.BillingZipCode;
                        agentBilling.CVV = agent.CVV;

                        SignUp signUp = new SignUp();

                        if (agent.AgentId > 0)
                        {
                            if (agent.FedTaxIdentityProof != null)
                            {
                                common.DeleteFile(agent.FedTaxIdentitytext);
                                string TaxIdentity = Path.GetFileName(agent.FedTaxIdentityProof.FileName);
                                string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + TaxIdentity);
                                agent.FedTaxIdentityProof.SaveAs(physicalPath);

                            }
                            if (agent.StateOfIncorporationProof != null)
                            {
                                common.DeleteFile(agent.StateOfIncorporationtext);
                                string ImageName = Path.GetFileName(agent.StateOfIncorporationProof.FileName);
                                string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + ImageName);
                                agent.StateOfIncorporationProof.SaveAs(physicalPath);

                            }
                            if (agent.SocialSecCard != null)
                            {
                                common.DeleteFile(agent.StateOfIncorporationtext);
                                string ImageName = Path.GetFileName(agent.SocialSecCard.FileName);
                                string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + ImageName);
                                agent.SocialSecCard.SaveAs(physicalPath);

                            }
                            if (agent.DriversLicenseCopy != null)
                            {
                                common.DeleteFile(agent.DriversLicenseCopytext);
                                string ImageName = Path.GetFileName(agent.DriversLicenseCopy.FileName);
                                string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + ImageName);
                                agent.DriversLicenseCopy.SaveAs(physicalPath);

                            }
                        }

                        string fromA = "Admin";
                        bool agentbillingstatus = functions.AddBillingInfo(agentBilling, fromA);
                        if (agentbillingstatus)
                        {
                            if (agent.Password != null)
                            {
                                signUp.UserName = agente.UserName;
                                signUp.Password = agente.Password;
                                signUp.AgentClientId = agent.AgentId.ToString();
                                functions.UpdateUser(signUp);
                            }
                            return RedirectToAction("Index", "Agent");
                        }
                    }
                }
                else
                {
                    agent.AgentId = Agentclient;
                    if (agent.FedTaxIdentityProof != null)
                    {
                        common.DeleteFile(agent.FedTaxIdentitytext);
                        string TaxIdentity = Path.GetFileName(agent.FedTaxIdentityProof.FileName);
                        agent.FedTaxIdentitytext = TaxIdentity;
                    }
                    if (agent.StateOfIncorporationProof != null)
                    {
                        common.DeleteFile(agent.StateOfIncorporationtext);
                        string ImageName = Path.GetFileName(agent.StateOfIncorporationProof.FileName);
                        agent.StateOfIncorporationtext = ImageName;
                    }
                    if (agent.SocialSecCard != null)
                    {
                        common.DeleteFile(agent.StateOfIncorporationtext);
                        string ImageName = Path.GetFileName(agent.SocialSecCard.FileName);
                        agent.SocialSecCardtext = ImageName;
                        agent.FedTaxIdentityNo = agent.SocialSecCardNo;
                        agent.FedTaxIdentitytext = ImageName;
                    }
                    if (agent.DriversLicenseCopy != null)
                    {
                        common.DeleteFile(agent.DriversLicenseCopytext);
                        string ImageName = Path.GetFileName(agent.DriversLicenseCopy.FileName);
                        agent.DriversLicenseCopytext = ImageName;
                    }
                    agent.UserName = agent.PrimaryBusinessEmail;
                    agent.CreatedBy = sessionData.GetUserID().StringToInt(0);


                    agent.AgentId = functions.AddAgentNew(agent);
                    if (agent.AgentId > 0)
                    {
                        string year = agent.ExpiryDate;
                        year = year.Substring(2, 2);

                        agentBilling.AgentId = agent.AgentId;
                        agentBilling.BillingType = agent.BillingType;
                        agentBilling.CardType = agent.CardType;
                        agentBilling.CardNumber = agent.CardNumber;
                        agentBilling.ExpiryDate = agent.Month + "-" + year;
                        agentBilling.BillingZipCode = agent.BillingZipCode;
                        agentBilling.CVV = agent.CVV;
                        agentBilling.CreatedBy = sessionData.GetUserID().StringToInt(0);

                        SignUp signUp = new SignUp();

                        if (agent.AgentId > 0)
                        {
                            if (agent.FedTaxIdentityProof != null)
                            {
                                common.DeleteFile(agent.FedTaxIdentitytext);
                                string TaxIdentity = Path.GetFileName(agent.FedTaxIdentityProof.FileName);
                                string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + TaxIdentity);
                                agent.FedTaxIdentityProof.SaveAs(physicalPath);

                            }
                            if (agent.StateOfIncorporationProof != null)
                            {
                                common.DeleteFile(agent.StateOfIncorporationtext);
                                string ImageName = Path.GetFileName(agent.StateOfIncorporationProof.FileName);
                                string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + ImageName);
                                agent.StateOfIncorporationProof.SaveAs(physicalPath);

                            }
                            if (agent.SocialSecCard != null)
                            {
                                common.DeleteFile(agent.StateOfIncorporationtext);
                                string ImageName = Path.GetFileName(agent.SocialSecCard.FileName);
                                string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + ImageName);
                                agent.SocialSecCard.SaveAs(physicalPath);

                            }
                            if (agent.DriversLicenseCopy != null)
                            {
                                common.DeleteFile(agent.DriversLicenseCopytext);
                                string ImageName = Path.GetFileName(agent.DriversLicenseCopy.FileName);
                                string physicalPath = Server.MapPath("~/documents/" + "Agent-" + agent.AgentId + "-" + ImageName);
                                agent.DriversLicenseCopy.SaveAs(physicalPath);

                            }
                        }
                        bool agentbillingstatus = functions.AddBillingInfo(agentBilling);
                        if (agentbillingstatus)
                        {
                            signUp.UserName = agent.UserName;
                            signUp.Password = Common.CreateRandomPassword();
                            signUp.AgentClientId = agent.AgentId.ToString();
                            signUp.EmailAddress = agent.UserName;
                            string from = "Admin";
                            accountFunctions.AddUser(signUp, from);

                            // Users user = accountFunctions.getUser(agent.UserName);
                            common.SendMail(agent.UserName, "Agent Registration", "AgentRegistration", agent.FirstName, "", "", signUp.Password);
                            return RedirectToAction("Index", "Agent");

                        }
                    }
                }


            }
            catch (Exception ex) { ex.insertTrace(""); }

            return View();
        }

        [HttpPost]
        public JsonResult CheckUsernameexistorNot(string CurrentEmail, int ClientId = 0)
        {
            bool res = false;
            try
            {
                res = agentfunction.CheckUsernameexistorNot(CurrentEmail, ClientId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(res);

        }
        public ActionResult AssignStaff()
        {
            try
            {
                ViewBag.Dasboard = sessionData.getDasboard();
                string agentid = sessionData.GetAgentId();
                ViewBag.Clients = agentfunction.GetClient(agentid.StringToInt(0));
                ViewBag.staff = agentfunction.GetStaff(agentid);

            }
            catch (Exception ex) { ex.insertTrace(""); }


            return View();
        }


        public JsonResult AddAssignStaff(List<NewClient> client)
        {
            bool status = false;
            try
            {
                status = agentfunction.AddAssignStaff(client);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(status);
        }
        public ActionResult Challenges(string ClientId = "")
        {
            List<CreditReportFiles> creditreport = new List<CreditReportFiles>();
            List<CreditReportFiles> creditreportAH = new List<CreditReportFiles>();
            List<CreditReportFiles> creditreportINQ = new List<CreditReportFiles>();
            List<CreditReportFiles> creditreportPR = new List<CreditReportFiles>();
            if (ClientId != "")
            {
                //PublicRe //Account-
                creditreport = agentfunction.GetChallenges(ClientId);
                creditreportAH = creditreport.Where(x => x.mode == "Account-").ToList();
                ViewBag.creditreportfile = creditreportAH;

                //creditreportPR = creditreport.Where(x => x.mode == "PublicRe").ToList();
                //ViewBag.creditreportfilePR = creditreportPR;

                int AgentClientId = sessionData.GetAgentClientId();
                Agent agent = new Agent();
                AccountFunctions _accFunctions = new AccountFunctions();
                agent = _accFunctions.GetAgent(AgentClientId.ToString());
                bool checkIncInq = false;
                if (agent != null)
                {
                    checkIncInq = string.IsNullOrEmpty(agent.IncChallengeInq) ? false : Convert.ToBoolean(agent.IncChallengeInq);
                }

                if (checkIncInq)
                {
                    creditreportINQ = creditreport.Where(x => x.mode == "Inquires").ToList();
                    ViewBag.creditreportfileInq = creditreportINQ;
                }
                
                ViewBag.Dasboard = sessionData.getDasboard();
            }

            return View();
        }


    }
}
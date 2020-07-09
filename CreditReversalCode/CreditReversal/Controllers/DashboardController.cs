using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CreditReversal.Utilities;
using CreditReversal.BLL;
using CreditReversal.Models;
using CreditReversal.DAL;

namespace CreditReversal.Controllers
{
    [Authorization]
    [RoutePrefix("dashboard")]
    public class DashboardController : Controller
    {
        private SessionData sessionData = new SessionData();
        private DashboardFunctions functions = new DashboardFunctions();
        ClientFunction cfunction = new ClientFunction();
        ClientData CData = new ClientData();
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }
        [Route("client")]
        public ActionResult Client()
        {
            ClientModel ClientModel = new ClientModel();
            string ClientId = "";
            string[] round = null;
            try
            {
                ClientId = sessionData.GetClientId();
                ViewBag.ClientModel = functions.GetClient(ClientId);
                round = CData.GetRoundType(ClientId);
                ViewBag.CreditReportItems = functions.GetCreditFile(ClientId);
                ViewBag.Round = round[0];
                //ViewBag.TUcount = functions.GetCountChallenges(Convert.ToInt32(ClientId), "TransUnion");
                //ViewBag.EXcount = functions.GetCountChallenges(Convert.ToInt32(ClientId), "Experian");
                ViewBag.Dasboard = sessionData.getDasboard();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return View();
        }
        [Route("agent")]
        public ActionResult Agent()
        {
            string agentID = sessionData.GetAgentId();

            ViewBag.Dasboard = sessionData.getDasboard();
            ViewBag.TotalStaff = functions.GetTotalStaff("agentadmin", sessionData.GetAgentId());
            ViewBag.Clients = functions.GetClients(agentID.StringToInt(0));
            ViewBag.ActiveClients = functions.GetActiveClientsnew(agentID.StringToInt(0));
            // ViewBag.ActiveClients = functions.GetActiveClients(agentID.StringToInt(0));

            //Staff By Agent And Client By Staff
            ViewBag.TotalStaffAndClients = functions.GetStaffsByAgent(agentID);

            ViewBag.ActiveRoundOne = functions.GetActiveClientsByStatus("First Round", agentID).ToArray();
            ViewBag.ActiveRoundTwo = functions.GetActiveClientsByStatus("Second Round", agentID).ToArray();
            ViewBag.ActiveRoundThree = functions.GetActiveClientsByStatus("Third Round", agentID).ToArray();
            return View();
        }

        [Route("admin")]
        public ActionResult Admin()
        {
            ViewBag.Dasboard = sessionData.getDasboard();
            //ViewBag.TotalStaff = functions.GetTotalStaff("admin");
            //ViewBag.TotalClients = functions.GetTotalClients("admin");
            //ViewBag.TotalAgents = functions.GetTotalAgents("admin");
            ViewBag.GetBillingDates = functions.GetAgentBillings();
            return View();
        }

        public ActionResult AgentClients()
        {
            ViewBag.Dasboard = sessionData.getDasboard();
            string agent = string.Empty;
            ClientData clientData = new ClientData();
            if(Request["agent"] != null)
            {
                agent = Request["agent"].ToString();
            }
            ViewBag.GetClients= clientData.GetClientsByAgent(agent);
            return View();
        }
        [Route("staff")]
        public ActionResult AgentStaff()
        {
            string staffid = sessionData.GetStaffId();            int Id = Convert.ToInt32(staffid);            ViewBag.staffId = Id;
            ViewBag.Dasboard = sessionData.getDasboard();
			ViewBag.Clients = functions.GetClients(staffid.StringToInt(0));
			ViewBag.ActiveClients = functions.GetActiveClients(staffid.StringToInt(0));
			ViewBag.ActiveRoundOne = functions.GetActiveClientsByStatus("First Round", staffid).ToArray();
			ViewBag.ActiveRoundTwo = functions.GetActiveClientsByStatus("Second Round", staffid).ToArray();
			ViewBag.ActiveRoundThree = functions.GetActiveClientsByStatus("Third Round", staffid).ToArray();
			return View();
        }

        public ActionResult CreditReport()        {            string ClientId = "", CName = "";            string[] round = null;            try            {                ClientId = sessionData.GetClientId();                if (Session["Name"] != null)                {                    CName = Session["Name"].ToString();                }
                //List<CreditItems> CreditItems = functions.GetCreditReportChallenges(Convert.ToInt32(ClientId), null);
                //ViewBag.CreditItems = CreditItems;

                DashboardFunctions functions = new DashboardFunctions();                List<CreditItems> challenges = functions.GetCreditReportChallengesAgent(Convert.ToInt32(ClientId), null);                ViewBag.challenges = challenges;                ViewBag.Cname = CName;                round = CData.GetRoundType(ClientId);                List<CreditReportItems> creditReportItems = cfunction.GetCreditReportItems(Convert.ToInt32(ClientId));                if (creditReportItems.Count > 0)                {                    ViewBag.DateReportPulls = round[1];                    ViewBag.CreditReportItems = creditReportItems;
                    //ViewBag.Round = creditReportItems.First().RoundType;
                    ViewBag.Round = round[0];                }                ViewBag.AgentName = cfunction.getAgentName(Convert.ToInt32(ClientId));
                //string fullname = string.Empty;
                //if (Session["Name"] != null)
                //{
                //    fullname = Session["Name"].ToString();
                //}
                //var names = fullname.Split(' ');
                //string name = names[0];
                //ViewBag.name = name;
                //ViewBag.AgentName = fullname;



                //ViewBag.EQUIFAX = challenges.Where(x => x.Agency == "EQUIFAX").FirstOrDefault();
                //ViewBag.TRANSUNION = challenges.Where(x => x.Agency == "TRANSUNION").FirstOrDefault();
                //ViewBag.EXPERIAN = challenges.Where(x => x.Agency == "EXPERIAN").FirstOrDefault();
                //List<CreditReportItems> agent = functions.GetAgentNamefrmCId(ClientId);
                //ViewBag.AgentName = agent.First().Agent;
                //ViewBag.DatePulls = agent.First().DatePulls;
                //ViewBag.Round = agent.First().RoundType;
                //ViewBag.CName = CName;
                //ViewBag.piechart = challenges.Where(x => x.Status == "Deleted" && x.Status == "Repaired");
                //ViewBag.totcount = challenges.Sum(x => x.Count);


                //ViewBag.Deletedcount = challenges.Where(x => x.Status == "Deleted").Sum(x => x.Count);
                //ViewBag.Repairedcount = challenges.Where(x => x.Status == "Repaired").Sum(x => x.Count);
                //ViewBag.Pendingcount = challenges.Where(x => x.Status == "Pending").Sum(x => x.Count);
                //ViewBag.Disputecount = challenges.Where(x => x.Status == "Dispute").Sum(x => x.Count);


                //AgnecyWise Total count 

                //ViewBag.EQUIFAXcount = challenges.Where(x => x.AgencyName == "EQUIFAX").Sum(x => x.Count);
                //ViewBag.TransUnioncount = challenges.Where(x => x.AgencyName == "TRANSUNION").Sum(x => x.Count);
                //ViewBag.Experiancount = challenges.Where(x => x.AgencyName == "EXPERIAN").Sum(x => x.Count);


                //Agnecy status Deleted  Wisecount

                //ViewBag.EQUIFAXDeletedcount = challenges.Where(x => x.AgencyName == "EQUIFAX" && x.Status == "Deleted").Sum(x => x.Count);
                //ViewBag.TransUnionDeletedcount = challenges.Where(x => x.AgencyName == "TRANSUNION" && x.Status == "Deleted").Sum(x => x.Count);
                //ViewBag.ExperianDeletedcount = challenges.Where(x => x.AgencyName == "EXPERIAN" && x.Status == "Deleted").Sum(x => x.Count);

                //Agnecy status Repaired  Wisecount

                //ViewBag.EQUIFAXRepairedcount = challenges.Where(x => x.AgencyName == "EQUIFAX" && x.Status == "Repaired").Sum(x => x.Count);
                //ViewBag.TransUnionRepairedcount = challenges.Where(x => x.AgencyName == "TRANSUNION" && x.Status == "Repaired").Sum(x => x.Count);
                //ViewBag.ExperianRepairedcount = challenges.Where(x => x.AgencyName == "EXPERIAN" && x.Status == "Repaired").Sum(x => x.Count);

                //Agnecy status Pending  Wisecount
                //ViewBag.EQUIFAXPendingcount = challenges.Where(x => x.AgencyName == "EQUIFAX" && x.Status == "Pending").Sum(x => x.Count);
                //ViewBag.TransUnionPendingcount = challenges.Where(x => x.AgencyName == "TRANSUNION" && x.Status == "Pending").Sum(x => x.Count);
                //ViewBag.ExperianPendingcount = challenges.Where(x => x.AgencyName == "EXPERIAN" && x.Status == "Pending").Sum(x => x.Count);




                //Agnecy status Dispute  Wisecount

                //ViewBag.EQUIFAXDisputecount = challenges.Where(x => x.AgencyName == "EQUIFAX" && x.Status == "Dispute").Sum(x => x.Count);
                //ViewBag.TransUnionDisputecount = challenges.Where(x => x.AgencyName == "TRANSUNION" && x.Status == "Dispute").Sum(x => x.Count);
                //ViewBag.ExperianDisputecount = challenges.Where(x => x.AgencyName == "EXPERIAN" && x.Status == "Dispute").Sum(x => x.Count);





                //ViewBag.TUcount = functions.GetCountChallenges(Convert.ToInt32(ClientId), "TransUnion");
                //ViewBag.EXcount = functions.GetCountChallenges(Convert.ToInt32(ClientId), "Experian");
                ViewBag.Dasboard = sessionData.getDasboard();            }            catch (Exception ex)            {                System.Diagnostics.Debug.WriteLine(ex.Message);            }            return View();        }
        //public ActionResult CreditReport()
        //{
        //    string ClientId = "",CName="";
        //    try
        //    {
        //        ClientId = sessionData.GetClientId();
        //        if (Session["Name"] != null)
        //        {
        //            CName = Session["Name"].ToString();
        //        }
        //        List<CreditReportItems> challenges = functions.GetCreditReportChallenges(Convert.ToInt32(ClientId), null);

        //        ViewBag.EQUIFAX = challenges.Where(x => x.Agency == "EQUIFAX").FirstOrDefault();
        //        ViewBag.TRANSUNION = challenges.Where(x => x.Agency == "TRANSUNION").FirstOrDefault();
        //        ViewBag.EXPERIAN = challenges.Where(x => x.Agency == "EXPERIAN").FirstOrDefault();
        //        List<CreditReportItems> agent = functions.GetAgentNamefrmCId(ClientId);
        //        ViewBag.AgentName = agent.First().Agent;
        //        ViewBag.DatePulls = agent.First().DatePulls;
        //        ViewBag.Round = agent.First().RoundType;
        //        ViewBag.CName = CName;
        //        //ViewBag.piechart = challenges.Where(x => x.Status == "Deleted" && x.Status == "Repaired");
        //        //ViewBag.totcount = challenges.Sum(x => x.Count);


        //        //ViewBag.Deletedcount = challenges.Where(x => x.Status == "Deleted").Sum(x => x.Count);
        //        //ViewBag.Repairedcount = challenges.Where(x => x.Status == "Repaired").Sum(x => x.Count);
        //        //ViewBag.Pendingcount = challenges.Where(x => x.Status == "Pending").Sum(x => x.Count);
        //        //ViewBag.Disputecount = challenges.Where(x => x.Status == "Dispute").Sum(x => x.Count);


        //        //AgnecyWise Total count 

        //        //ViewBag.EQUIFAXcount = challenges.Where(x => x.AgencyName == "EQUIFAX").Sum(x => x.Count);
        //        //ViewBag.TransUnioncount = challenges.Where(x => x.AgencyName == "TRANSUNION").Sum(x => x.Count);
        //        //ViewBag.Experiancount = challenges.Where(x => x.AgencyName == "EXPERIAN").Sum(x => x.Count);


        //        //Agnecy status Deleted  Wisecount

        //        //ViewBag.EQUIFAXDeletedcount = challenges.Where(x => x.AgencyName == "EQUIFAX" && x.Status == "Deleted").Sum(x => x.Count);
        //        //ViewBag.TransUnionDeletedcount = challenges.Where(x => x.AgencyName == "TRANSUNION" && x.Status == "Deleted").Sum(x => x.Count);
        //        //ViewBag.ExperianDeletedcount = challenges.Where(x => x.AgencyName == "EXPERIAN" && x.Status == "Deleted").Sum(x => x.Count);

        //        //Agnecy status Repaired  Wisecount

        //        //ViewBag.EQUIFAXRepairedcount = challenges.Where(x => x.AgencyName == "EQUIFAX" && x.Status == "Repaired").Sum(x => x.Count);
        //        //ViewBag.TransUnionRepairedcount = challenges.Where(x => x.AgencyName == "TRANSUNION" && x.Status == "Repaired").Sum(x => x.Count);
        //        //ViewBag.ExperianRepairedcount = challenges.Where(x => x.AgencyName == "EXPERIAN" && x.Status == "Repaired").Sum(x => x.Count);

        //        //Agnecy status Pending  Wisecount
        //        //ViewBag.EQUIFAXPendingcount = challenges.Where(x => x.AgencyName == "EQUIFAX" && x.Status == "Pending").Sum(x => x.Count);
        //        //ViewBag.TransUnionPendingcount = challenges.Where(x => x.AgencyName == "TRANSUNION" && x.Status == "Pending").Sum(x => x.Count);
        //        //ViewBag.ExperianPendingcount = challenges.Where(x => x.AgencyName == "EXPERIAN" && x.Status == "Pending").Sum(x => x.Count);




        //        //Agnecy status Dispute  Wisecount

        //        //ViewBag.EQUIFAXDisputecount = challenges.Where(x => x.AgencyName == "EQUIFAX" && x.Status == "Dispute").Sum(x => x.Count);
        //        //ViewBag.TransUnionDisputecount = challenges.Where(x => x.AgencyName == "TRANSUNION" && x.Status == "Dispute").Sum(x => x.Count);
        //        //ViewBag.ExperianDisputecount = challenges.Where(x => x.AgencyName == "EXPERIAN" && x.Status == "Dispute").Sum(x => x.Count);





        //        //ViewBag.TUcount = functions.GetCountChallenges(Convert.ToInt32(ClientId), "TransUnion");
        //        //ViewBag.EXcount = functions.GetCountChallenges(Convert.ToInt32(ClientId), "Experian");
        //        ViewBag.Dasboard = sessionData.getDasboard();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.Message);
        //    }
        //    return View();

        //}
        [HttpPost]
        public ActionResult DeleteClient(string ClientId)
        {
            try
            {
                long res = functions.DeleteClient(ClientId);
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index", "Client");

        }
    }
}
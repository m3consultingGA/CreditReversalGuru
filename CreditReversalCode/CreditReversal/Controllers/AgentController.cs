﻿using System;
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
            //return View(agent);
        }


        [HttpGet]

                            // Users user = accountFunctions.getUser(agent.UserName);
                            common.SendMail(agent.UserName, "Agent Registration", "AgentRegistration", agent.FirstName, "", "", signUp.Password);

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
                ViewBag.Dasboard = sessionData.getDasboard();
        public ActionResult Challenges(string ClientId = "")


    }
}
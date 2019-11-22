using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CreditReversal.BLL;
using CreditReversal.Utilities;
using CreditReversal.Models;

namespace CreditReversal.Controllers
{
    public class AccountController : Controller
    {
        private RegistrationFunctions Regfunction = new RegistrationFunctions();
        // GET: Account

        private AccountFunctions functions = new AccountFunctions();
        private SessionData sessionData = new SessionData();
        private Common common = new Common();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SignIn()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SignIn(string UserName, string Password)
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
                        if(agent != null)
                        {
                            Session["AgentType"] = agent.TypeOfComp;
                            Session["Name"] = agent.FirstName + " " + agent.LastName;
                        }
                        
                    }
                    else if (row["UserRole"].ToString() == "agentstaff")
                    {
                        AgentFunction agentFunction = new AgentFunction();
                        AgentStaff agentStaff =  agentFunction.GetStaff(Session["AgentClientId"].ToString())[0];
                        if(agentStaff != null)
                        {
                            Session["Name"] = agentStaff.FirstName + " " + agentStaff.LastName;
                        }
                        Session["StaffId"] = row["AgentClientId"];
                    }
                    else if (row["UserRole"].ToString() == "client")
                    {
                        ClientFunction clientFunction = new ClientFunction();
                        ClientModel clientModel = clientFunction.GetClients(null, null, Session["AgentClientId"].ToString())[0];
                        if(clientModel != null)
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

        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(SignUp signUp)
        {
            try
            {
                int res = functions.AddAgentAdmin(signUp);
                if (res > 0)
                {
                    Session["EmailAddress"] = signUp.EmailAddress;
                    Session["Name"] = signUp.Name;
                    Session["UserName"] = string.IsNullOrEmpty(signUp.UserName) ? signUp.EmailAddress : signUp.UserName;
                    Session["AgentClientId"] = res;
                    Session["BusinessName"] = signUp.BusinessName;
                    Session["CompanyType"] = string.IsNullOrEmpty(signUp.CompanyType) ? null : "I";
                    return RedirectToAction("Agent", "Registration");
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return View();
        }

        public void SignOut()
        {

        }
        public JsonResult CheckUsernameexistorNot(string username)        {            bool status = false;            try            {                status = functions.CheckUsernameexistorNot(username);                if (status == true)                {                    ViewBag.errormsg = "User Name Already Exists";                }            }           catch (Exception ex) {  ex.insertTrace("");  }            return Json(status);        }
        public JsonResult EmailExitOrNot(string Email)        {            bool status = false;            Users user = new Users();            try            {                user = functions.EmailExitOrNot(Email);

                string subject = "Forget Password";
                string type = "Lost_Password";
                if (user.UserName != null)
                {
                    status = true;
                    common.SendMail(Email, subject, type, user.UserName, "", "", user.Password);
                }

            }           catch (Exception ex) {  ex.insertTrace("");  }            return Json(status);        }
        public ActionResult CompleteRegistration(string id = "")        {            string deCryptID = common.Decrypt(id);            int res = functions.CheckUserExists(deCryptID);            if(res > 0)
            {
                bool status = functions.GetUserStatus(deCryptID);
                if (status)
                {
                    Response.Redirect("/Error?msg=1");
                }
                else
                {
                    Session["AgentClientId"] = deCryptID;
                    Session["UserId"] = deCryptID;
                    return RedirectToAction("Agent", "Registration");

                }
            }            else
            {
                Response.Redirect("/Error");
            }            return View();        }
        public ActionResult ChangePassword()        {
            ViewBag.Dasboard = sessionData.getDasboard();            return View();        }        public JsonResult Current_Password(string Password)        {            string username = sessionData.GetUserName();            bool status = false;            try            {                status = Regfunction.checkCurrentPassword(username, Password);            }            catch (Exception ex)            {                ex.insertTrace("");            }            return Json(status);        }        public JsonResult UpdatePassword(string Password)        {            bool status = false;            string username = sessionData.GetUserName();            try            {                status = functions.UpdatePassword(username, Password);            }            catch (Exception ex)            {                ex.insertTrace("");            }            return Json(status);        }
    }
}
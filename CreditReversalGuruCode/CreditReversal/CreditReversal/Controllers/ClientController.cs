using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CreditReversal.Models;
using CreditReversal.BLL;
using System.IO;
using System.Data;
using CreditReversal.Utilities;
using Persits.PDF;
using System.Dynamic;
using CreditReversal.DAL;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CreditReversal.Controllers
{
    [Authorization]
    public class ClientController : Controller
    {
        string ClientId = string.Empty;
        public SessionData sessionData = new SessionData();
        IdentityIQFunction IQfunction = new IdentityIQFunction();
        Common objCommon = new Common();
        ClientFunction cfunction = new ClientFunction();
        DashboardFunctions functions = new DashboardFunctions();
        ClientData CData = new ClientData();
        RegistrationFunctions registrationfunc = new RegistrationFunctions();
        public object PageLoadEvent { get; private set; }

        [HttpGet]
        public ActionResult IdentityIQInfo()
        {
            IdentityIQInfo IQInfo = new IdentityIQInfo();
            try
            {
                ClientId = sessionData.GetClientId();
                IQInfo = IQfunction.GetIdentityIQInfo(ClientId);
                ViewBag.Dasboard = sessionData.getDasboard();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return View(IQInfo);
        }



        [HttpPost]
        public ActionResult IdentityIQInfo(IdentityIQInfo IQInfoModel)
        {
            long res = 0;
            IdentityIQInfo info = new IdentityIQInfo();
            try
            {
                ClientId = sessionData.GetClientId();

                info = IQfunction.GetIdentityIQInfo(ClientId);
                string id = info.IdentityIqId.ToString();
                if (id == "")
                {
                    string pass = objCommon.Encrypt(IQInfoModel.Password);
                    IQInfoModel.Password = pass;
                    IQInfoModel.ClientId = Convert.ToInt64(ClientId);
                    res = IQfunction.InsertIdetityIQInfo(IQInfoModel);
                    if (res > 0)
                    {
                        ViewBag.Success = res;
                        TempData["Message"] = "Identity IQ Information Created Successfully";
                    }
                }
                else
                {
                    IQInfoModel.IdentityIqId = info.IdentityIqId;
                    IQInfoModel.ClientId = info.ClientId;
                    string pass = objCommon.Encrypt(IQInfoModel.Password);
                    IQInfoModel.Password = pass;
                    res = IQfunction.UpdateIdetityIQInfo(IQInfoModel);
                    if (res > 0)
                    {
                        ViewBag.Update = res;
                        TempData["Message"] = "Identity IQ Information Updated Successfully";
                    }
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return View(info);
        }

        [HttpGet]
        public ActionResult CreditItems(string ClientId, string from = "")
        {
            string role = sessionData.GetUserRole();
            string[] round = null;
            try
            {
                ClientModel clientModel = cfunction.GetClient(ClientId);
                if (clientModel != null)
                {
                    ViewBag.Cname = clientModel.FullName;
                }
                string fullname = string.Empty;
                if (Session["Name"] != null)
                {
                    fullname = Session["Name"].ToString();
                }
                var names = fullname.Split(' ');
                string name = names[0];
                ViewBag.name = name;
                ViewBag.ClientName = fullname;

                ViewBag.AgentName = cfunction.getAgentName(Convert.ToInt32(ClientId));
                string agentid = sessionData.GetAgentId();
                string staffid = sessionData.GetStaffId();
                round = CData.GetRoundType(ClientId);

                List<CreditReportItems> creditReportItems = cfunction.GetCreditReportItems(Convert.ToInt32(ClientId));
                if (creditReportItems.Count > 0)
                {
                    ViewBag.DateReportPulls = round[1];
                    ViewBag.CreditReportItems = creditReportItems;
                    //ViewBag.Round = creditReportItems.First().RoundType;
                    ViewBag.Round = round[0];

                    string dateInString = round[1];
                    DateTime startDate = DateTime.Parse(dateInString);
                    DateTime expiryDate = startDate.AddDays(30);
                    DateTime dexpiryDate = Convert.ToDateTime(expiryDate.ToString("MM/dd/yyyy").Replace("-", "/"));
                    if (DateTime.Now.Date >= dexpiryDate)
                    {
                        ViewBag.Status = "true";
                    }
                }

                ViewBag.challengeMasters = cfunction.GetChallengeMasters(agentid.StringToInt(0), staffid.StringToInt(0));
                ViewBag.Dasboard = sessionData.getDasboard();
                //List<CreditReportItems> creditReportItemChallenges = cfunction.ReportItemChallenges(Convert.ToInt32(ClientId));
                //if (creditReportItemChallenges.Count > 0)
                //{
                //    ViewBag.ReportItemChallenges = creditReportItemChallenges;
                //    ViewBag.Challenge = creditReportItemChallenges.Where(x => x.Agency == "TRANSUNION").First().Challenge;
                //}
                ViewBag.Role = role;
                /////////////////////////////////////////
                DashboardFunctions functions = new DashboardFunctions();
                List<CreditItems> challenges = functions.GetCreditReportChallengesAgent(Convert.ToInt32(ClientId), null, role);
                List<string> AccountTypes = functions.GetAccountTypes();
                ViewBag.challenges = challenges;
                ViewBag.AllAccountTypes = AccountTypes;

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
                    ViewBag.checkIncInq = "1";
                }

                List<Inquires> Inquires = functions.GetCreditReportInquiresAgent(Convert.ToInt32(ClientId), null, role);
                ViewBag.Inquires = Inquires.Count == 0 ? null : Inquires;
                List<PublicRecord> publicRecords = functions.GetCreditReportPublicRecordsAgent(Convert.ToInt32(ClientId), null, role);
                ViewBag.PublicRecords = publicRecords.Count == 0 ? null : publicRecords;
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return View();
        }
        // GET: Client
        public ActionResult Index(string Id = null)
        {
            try
            {
                bool status = sessionData.GetLoginStatus();
                if (status)
                {
                    List<ClientModel> models = new List<ClientModel>();
                    if (Id != null)
                    {
                        string AgentId = Id;
                        models = cfunction.GetClients(AgentId);
                    }
                    else
                    {
                        models = cfunction.GetClients(sessionData.GetAgentId(), sessionData.GetStaffId(), sessionData.GetClientId());
                    }
                    //models = cfunction.GetClients(sessionData.GetAgentId(), sessionData.GetStaffId(), sessionData.GetClientId());
                    ViewBag.Models = models;
                    ViewBag.Dasboard = sessionData.getDasboard();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home"); //TODO Changed Home page
                }


            }
            catch (Exception ex) { ex.insertTrace(""); }
            return View();
        }
        [HttpGet]
        public ActionResult CreateClient(string ClientId = "", string Mode = "", string from = "")
        {
            TempData["from"] = from;
            string staffid = sessionData.GetStaffId();
            int Id = Convert.ToInt32(staffid);

            ViewBag.staffId = Id;
            ClientModel clientModel = new ClientModel(); try
            {
                ViewBag.Dasboard = sessionData.getDasboard();
                ViewBag.States = registrationfunc.GetStates();
                // ClientId = Request.QueryString["ClientId"];
                Session["ClientId"] = ClientId;
                //Mode = Request.QueryString["Mode"];
                if (ClientId != "0")
                {
                    if (Mode == "Edit" || Mode == "Profile" || Mode == "View" || Mode == "frmDashboard")
                    {
                        clientModel = cfunction.GetClient(ClientId);
                        clientModel.ClientId = ClientId.StringToInt(0);

                    }
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return View(clientModel);
        }

        [HttpPost]
        public ActionResult CreateClient(ClientModel clientModel)
        {
            ClientModel client = new ClientModel(); string filename = string.Empty; string path = string.Empty;
            string role = sessionData.GetUserRole();
            string from = TempData["from"] != null ? TempData["from"].ToString() : string.Empty;
            long res = 0;
            try
            {
                if (clientModel.FProofOfCard != null)
                {
                    var ext = Path.GetExtension(clientModel.FProofOfCard.FileName);
                    string PFileName = string.Empty;
                    if (ext.ToUpper() == ".PDF")
                    {
                        PFileName = Path.GetFileName(clientModel.FProofOfCard.FileName);
                        filename = "Client-ProofOfCard-" + PFileName;
                        path = Server.MapPath("~/documents/" + filename);
                        clientModel.FProofOfCard.SaveAs(path);
                        clientModel.sProofOfCard = filename;
                    }
                    else
                    {
                        PFileName = ResizeImage(clientModel.FProofOfCard);
                        clientModel.sProofOfCard = PFileName;
                    }

                }
                if (clientModel.FDrivingLicense != null)
                {
                    string DFileName = Path.GetFileName(clientModel.FDrivingLicense.FileName);
                    filename = "Client-DrivingLicense-" + DFileName;
                    path = Server.MapPath("~/documents/" + filename);
                    clientModel.FDrivingLicense.SaveAs(path);
                    clientModel.sDrivingLicense = filename;
                }
                if (clientModel.FSocialSecCard != null)
                {
                    string SFileName = Path.GetFileName(clientModel.FSocialSecCard.FileName);
                    filename = "Client-SocialSecCard-" + SFileName;
                    path = Server.MapPath("~/documents/" + filename);
                    clientModel.FSocialSecCard.SaveAs(path);
                    clientModel.sSocialSecCard = filename;
                }
                clientModel.AgentId = sessionData.GetAgentId().StringToInt(0);
                clientModel.AgentStaffId = sessionData.GetStaffId().StringToInt(0);
                clientModel.Status = "1";
                clientModel.CreatedBy = sessionData.GetUserID().StringToInt(0);
                clientModel.UserRole = "client";
                clientModel.CreatedDate = System.DateTime.Now.ToShortDateString();
                res = cfunction.CreateClient(clientModel);
                if (res > 0)
                {
                    if (from == "db")
                    {
                        if (role == "agentadmin")
                        {
                            return RedirectToAction("Agent", "Dashboard");
                        }
                        else if (role == "agentstaff")
                        {
                            return RedirectToAction("staff", "Dashboard");
                        }
                        else if (role == "client")
                        {
                            return RedirectToAction("Client", "Dashboard");
                        }
                    }
                    else
                    {
                        if (role == "agentadmin")
                        {
                            return RedirectToAction("Agent", "Dashboard");
                        }
                        else if (role == "agentstaff")
                        {
                            return RedirectToAction("staff", "Dashboard");
                        }
                        else if (role == "client")
                        {
                            return RedirectToAction("Client", "Dashboard");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return View(clientModel);
        }


        //[HttpPost]
        //public ActionResult CreateNewClient(FormCollection form, HttpPostedFileBase[] file)
        //{
        //    ClientModel client = new ClientModel();
        //    string[] SplitFile;
        //    long res = 0;
        //    HttpPostedFileBase SocialSecCard = null;
        //    HttpPostedFileBase DrivingLicense = null;
        //    HttpPostedFileBase ProofOfCard = null;
        //    ClientModel clientM = new ClientModel();
        //    try
        //    {
        //        string isSocialSecCard = form["isSocialSecCard"];
        //        string isDrivingLicense = form["isDrivingLicense"];
        //        string isProofOfCard = form["isProofOfCard"];

        //        client.ClientId = form["ClientId"].StringToInt(0);
        //        client.FirstName = form["FirstName"];
        //        client.LastName = form["LastName"];
        //        client.MiddleName = form["MiddleName"];
        //        client.CurrentEmail = form["CurrentEmail"];
        //        client.CurrentPhone = form["CurrentPhone"];
        //        client.DOB = form["DOB"];
        //        client.SSN = form["SSN"];

        //        if (client.FirstName != "")
        //        {

        //            if (client.ClientId != 0)
        //            {
        //                clientM = cfunction.GetClient(client.ClientId.ToString());
        //            }
        //            if (Request.Files.Count == 1)
        //            {
        //                if (isDrivingLicense == "0" && isSocialSecCard != "0" && isProofOfCard == "0")
        //                {
        //                    SocialSecCard = Request.Files[0];
        //                }
        //                if (isDrivingLicense != "0" && isSocialSecCard == "0" && isProofOfCard == "0")
        //                {
        //                    DrivingLicense = Request.Files[0];

        //                }
        //                if (isDrivingLicense == "0" && isSocialSecCard == "0" && isProofOfCard != "0")
        //                {
        //                    ProofOfCard = Request.Files[0];
        //                }
        //            }
        //            if (Request.Files.Count == 2)
        //            {
        //                if (isDrivingLicense != "0" && isSocialSecCard != "0" && isProofOfCard == "0")
        //                {
        //                    SocialSecCard = Request.Files[0];
        //                    DrivingLicense = Request.Files[1];

        //                }
        //                if (isDrivingLicense != "0" && isSocialSecCard == "0" && isProofOfCard != "0")
        //                {
        //                    DrivingLicense = Request.Files[0];
        //                    ProofOfCard = Request.Files[1];

        //                }
        //                if (isDrivingLicense == "0" && isSocialSecCard != "0" && isProofOfCard != "0")
        //                {
        //                    SocialSecCard = Request.Files[0];
        //                    ProofOfCard = Request.Files[1];

        //                }
        //            }
        //            if (Request.Files.Count == 3)
        //            {
        //                SocialSecCard = Request.Files[0];
        //                DrivingLicense = Request.Files[1];
        //                ProofOfCard = Request.Files[2];
        //            }


        //            var _PFileName = "";
        //            var _DFileName = "";
        //            var _SFileName = "";
        //            if (ProofOfCard == null)
        //            {
        //                SplitFile = clientM.sProofOfCard.Split('-');
        //                client.sProofOfCard = SplitFile[1];
        //            }
        //            else
        //            {

        //                client.sProofOfCard = ProofOfCard.FileName;
        //            }


        //            if (DrivingLicense == null)
        //            {
        //                SplitFile = clientM.sDrivingLicense.Split('-');
        //                client.sDrivingLicense = SplitFile[1];
        //            }
        //            else
        //            {

        //                client.sDrivingLicense = DrivingLicense.FileName;
        //            }
        //            if (SocialSecCard == null)
        //            {
        //                SplitFile = clientM.sSocialSecCard.Split('-');
        //                client.sSocialSecCard = SplitFile[1];
        //            }
        //            else
        //            {

        //                client.sSocialSecCard = SocialSecCard.FileName;
        //            }


        //            client.AgentId = sessionData.GetAgentId().StringToInt(0);
        //            client.AgentStaffId = sessionData.GetStaffId().StringToInt(0);
        //            client.Status = "1";
        //            client.CreatedBy = sessionData.GetUserID().StringToInt(0);
        //            client.UserRole = "client";
        //            client.CreatedDate = System.DateTime.Now.ToShortDateString();
        //            res = cfunction.CreateClient(client);
        //            if (res > 0)
        //            {
        //                if (ProofOfCard != null)
        //                {
        //                    _PFileName = Path.GetFileName(res + "-" + ProofOfCard.FileName);
        //                    string _path = Path.Combine(Server.MapPath("~/documents"), _PFileName);
        //                    ProofOfCard.SaveAs(_path);
        //                }

        //                if (DrivingLicense != null)
        //                {
        //                    _DFileName = Path.GetFileName(res + "-" + DrivingLicense.FileName);
        //                    string _path = Path.Combine(Server.MapPath("~/documents"), _DFileName);
        //                    DrivingLicense.SaveAs(_path);
        //                }

        //                if (SocialSecCard != null)
        //                {
        //                    _SFileName = Path.GetFileName(res + "-" + SocialSecCard.FileName);
        //                    string _path = Path.Combine(Server.MapPath("~/documents"), _SFileName);
        //                    SocialSecCard.SaveAs(_path);
        //                }

        //                //return Json(res);
        //            }


        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return Json(res);
        //}

        [HttpPost]
        public JsonResult CheckSSNExistorNot(string SSN)
        {
            bool res = false;
            try
            {
                string ClientId = "";
                if (Session["ClientId"] != null)
                {
                    ClientId = Session["ClientId"].ToString();
                }

                res = cfunction.CheckSSNExistorNot(SSN, ClientId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(res);

        }


        [HttpPost]
        public JsonResult CheckUsernameexistorNot(string CurrentEmail, int ClientId = 0)
        {
            bool res = false;
            try
            {
                res = cfunction.CheckUsernameexistorNot(CurrentEmail, ClientId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(res);

        }

        [HttpPost]
        public ActionResult DeleteClient(string ClientId)
        {
            try
            {
                long res = cfunction.DeleteClient(ClientId);
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index", "Client");

        }

        public string RenderViewAsString(string viewName, object model)
        {
            // create a string writer to receive the HTML code
            StringWriter stringWriter = new StringWriter();

            // get the view to render
            ViewEngineResult viewResult = ViewEngines.Engines.FindView(ControllerContext,
viewName, null);
            // create a context to render a view based on a model
            ViewContext viewContext = new ViewContext(
ControllerContext,
viewResult.View,
new ViewDataDictionary(model),
new TempDataDictionary(),
stringWriter
);

            // render the view to a HTML code
            viewResult.View.Render(viewContext, stringWriter);

            // return the HTML code
            return stringWriter.ToString();
        }




        //public JsonResult ClientChallengeform(List<CreditReportItems> credit, int Id, string[] values)
        //{
        //    string date = string.Empty; bool status = false;
        //    int sno = 0;
        //    try
        //    {
        //        //
        //        if (!string.IsNullOrEmpty(Session["AgentId"].ToString()))
        //        {
        //            string agentid = Session["AgentId"].ToString();
        //            Agent agentAdddress = cfunction.GetAgentAddressById(agentid);
        //            Session["AgentAddress"] = agentAdddress;
        //        }

        //        //

        //        date = DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString()
        //        + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
        //        List<CreditReportItems> crediteTRANSUNION = new List<CreditReportItems>();
        //        List<CreditReportItems> crediteEXPERIAN = new List<CreditReportItems>();
        //        List<CreditReportItems> crediteEQUIFAX = new List<CreditReportItems>();
        //        List<CreditReportFiles> filenames = new List<CreditReportFiles>();

        //        int count = 0;
        //        if (credit != null)
        //        {
        //            count = credit.Count();
        //        }
        //        int i = 0;
        //        byte[] pdfBuffer = null;

        //        string filepath = "", filename = "";
        //        try
        //        {
        //            ClientData cd = new ClientData();

        //            if (credit != null)
        //            {

        //                for (i = 0; i < count; i++)
        //                {
        //                    sno = cd.getsnofromitems(Id.ToString(), credit[i].RoundType);
        //                    cfunction.AddReportItemChallenges(credit[i], sno, Id);
        //                    dynamic model = new ExpandoObject();
        //                    string AgentId = sessionData.GetAgentId();
        //                    string staffId = sessionData.GetStaffId();
        //                    string fullname = "";

        //                    cfunction.AddChallenge(credit[i], AgentId, staffId);
        //                    ViewBag.CreditReportItems = credit[i];

        //                    model.clientcredit = credit[i];

        //                    if (Session["Name"] != null)
        //                    {
        //                        fullname = Session["Name"].ToString();
        //                    }
        //                    var names = fullname.Split(' ');
        //                    string name = names[0];
        //                    int clientid = Id;
        //                    int? Report = credit[i].CredRepItemsId;

        //                }
        //            }
        //            List<CreditReportItems> credite = functions.GetCreditReportChallengesAgentById(credit, Id);

        //            string client = Id.ToString();

        //            ClientModel clientModel = cfunction.GetClient(client);
        //            Session["ClientAddress"] = clientModel;
        //            List<string> StringList = new List<string>();
        //            StringList.Add(clientModel.FirstName);
        //            StringList.Add(clientModel.LastName);
        //            StringList.Add(values[2]);
        //            StringList.Add(values[3]);
        //            StringList.Add(clientModel.SSN);


        //            for (int k = 0; k < credite.Count; k++)
        //            {
        //                CreditReportItems cr = new CreditReportItems();

        //                if (credite[k].Agency == "TRANSUNION" && credite[k].ChallengeText != "NO CHALLENGE")
        //                {
        //                    cr.MerchantName = credite[k].MerchantName;
        //                    cr.AccountId = credite[k].AccountId;
        //                    cr.OpenDate = credite[k].OpenDate;
        //                    cr.HighestBalance = credite[k].HighestBalance;
        //                    cr.CurrentBalance = credite[k].CurrentBalance;
        //                    cr.MonthlyPayment = credite[k].MonthlyPayment;
        //                    cr.LastReported = credite[k].LastReported;
        //                    cr.Agency = credite[k].Agency;
        //                    cr.ChallengeText = credite[k].ChallengeText;
        //                    cr.Status = credite[k].Status;
        //                    cr.CredRepItemsId = credite[k].CredRepItemsId;
        //                    cr.RoundType = credite[k].RoundType;
        //                    cr.AccountType = credite[k].AccountType;
        //                    crediteTRANSUNION.Add(cr);
        //                }
        //                if ("EXPERIAN" == credite[k].Agency && credite[k].ChallengeText != "NO CHALLENGE")
        //                {
        //                    cr.MerchantName = credite[k].MerchantName;
        //                    cr.AccountId = credite[k].AccountId;
        //                    cr.OpenDate = credite[k].OpenDate;
        //                    cr.HighestBalance = credite[k].HighestBalance;
        //                    cr.CurrentBalance = credite[k].CurrentBalance;
        //                    cr.MonthlyPayment = credite[k].MonthlyPayment;
        //                    cr.LastReported = credite[k].LastReported;
        //                    cr.Agency = credite[k].Agency;
        //                    cr.ChallengeText = credite[k].ChallengeText;
        //                    cr.Status = credite[k].Status;
        //                    cr.CredRepItemsId = credite[k].CredRepItemsId;
        //                    cr.RoundType = credite[k].RoundType;
        //                    cr.AccountType = credite[k].AccountType;                           
        //                    crediteEXPERIAN.Add(cr);
        //                }
        //                if ("EQUIFAX" == credite[k].Agency && credite[k].ChallengeText != "NO CHALLENGE")
        //                {
        //                    cr.MerchantName = credite[k].MerchantName;
        //                    cr.AccountId = credite[k].AccountId;
        //                    cr.OpenDate = credite[k].OpenDate;
        //                    cr.HighestBalance = credite[k].HighestBalance;
        //                    cr.CurrentBalance = credite[k].CurrentBalance;
        //                    cr.MonthlyPayment = credite[k].MonthlyPayment;
        //                    cr.LastReported = credite[k].LastReported;
        //                    cr.Agency = credite[k].Agency;
        //                    cr.ChallengeText = credite[k].ChallengeText;
        //                    cr.Status = credite[k].Status;
        //                    cr.CredRepItemsId = credite[k].CredRepItemsId;
        //                    cr.RoundType = credite[k].RoundType;
        //                    cr.AccountType = credite[k].AccountType;
        //                    crediteEQUIFAX.Add(cr);
        //                }

        //            }
        //            PdfManager objPdf = new PdfManager();
        //            PdfDocument objDocTRANSUNION = objPdf.CreateDocument();
        //            objDocTRANSUNION.Title = "CreditReversalGuru";
        //            objDocTRANSUNION.Creator = "CreditReversalGuru";
        //            PdfFont objFont = objDocTRANSUNION.Fonts["Helvetica"];

        //            PdfDocument objDocEXPERIAN = objPdf.CreateDocument();
        //            PdfDocument objDocEQUIFAX = objPdf.CreateDocument();
        //            MemoryStream pdfStream = new MemoryStream();
        //            filenames = new List<CreditReportFiles>();
        //            if (crediteTRANSUNION.Count > 0)
        //            {
        //                StringList.Add("TRANSUNION");
        //                Session["values"] = StringList;
        //                objDocTRANSUNION = objPdf.CreateDocument();
        //                dynamic model = new ExpandoObject();
        //                int trcount = crediteTRANSUNION.Count - 1;
        //                model.clientcredit = crediteTRANSUNION;
        //                string htmlToConvert = RenderViewAsString("ClientChallengeform", model);
        //                //objDocTRANSUNION.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=34;TopMargin=34;BottomMargin=34; hyperlinks=true; drawbackground=true");
        //                objDocTRANSUNION.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=54;TopMargin=54;BottomMargin=54; hyperlinks=true; drawbackground=true");
        //                filename = "Challenge-Account-TRANSUNION" + "-" + values[0] + "-" + values[1] + "-" + crediteTRANSUNION[trcount].RoundType + "-" + date + ".pdf";
        //                filename = filename.Replace(" ", "");
        //                filepath = Server.MapPath("~/Documents/Challenge/Challenge-Account-TRANSUNION" + "-" + values[0] + "-" + values[1] + "-" + crediteTRANSUNION[trcount].RoundType + "-" + date + ".pdf");
        //                // filepath = Server.MapPath("~/Documents/Challenge/Challenge-TRANSUNION.pdf"); // + "-" + values[0] + "-" + values[1] + "-" + DateTime.Now.ToShortDateString() + "-" + values[3] + ".pdf");
        //                filepath = filepath.Replace(" ", "");
        //                objDocTRANSUNION.Save(filepath, false);
        //                pdfBuffer = System.IO.File.ReadAllBytes(filepath);
        //                System.IO.File.WriteAllBytes(filepath, pdfBuffer);
        //                objDocTRANSUNION.Close();

        //                CreditReportFiles files = new CreditReportFiles();
        //                //files.RoundType = values[3];
        //                files.RoundType = crediteTRANSUNION[trcount].RoundType;
        //                files.CRFilename = filename;
        //                files.ClientId = Id;
        //                filenames.Add(files);
        //            }
        //            if (crediteEXPERIAN.Count > 0)
        //            {
        //                if (StringList.Count == 5)
        //                {
        //                    StringList.Add("EXPERIAN");
        //                }
        //                else
        //                {
        //                    StringList[5] = "";
        //                    StringList[5] = "EXPERIAN";
        //                }

        //                Session["values"] = StringList;
        //                objDocEXPERIAN = objPdf.CreateDocument();
        //                dynamic model = new ExpandoObject();
        //                int excount = crediteEXPERIAN.Count - 1;
        //                model.clientcredit = crediteEXPERIAN;
        //                string htmlToConvert = RenderViewAsString("ClientChallengeform", model);
        //                //objDocEXPERIAN.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=34;TopMargin=34;BottomMargin=34; hyperlinks=true; drawbackground=true");
        //                objDocEXPERIAN.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=54;TopMargin=54;BottomMargin=54; hyperlinks=true; drawbackground=true");
        //                filename = "Challenge-Account-EXPERIAN" + "-" + values[0] + "-" + values[1] + "-" + crediteEXPERIAN[excount].RoundType + "-" + date + ".pdf";
        //                filename = filename.Replace(" ", "");
        //                filepath = Server.MapPath("~/Documents/Challenge/Challenge-Account-EXPERIAN" + "-" + values[0] + "-" + values[1] + "-" + crediteEXPERIAN[excount].RoundType + "-" + date + ".pdf");
        //                //filepath = Server.MapPath("~/Documents/Challenge/Challenge-EXPERIAN.pdf");
        //                filepath = filepath.Replace(" ", "");
        //                objDocEXPERIAN.Save(filepath, false);
        //                pdfBuffer = System.IO.File.ReadAllBytes(filepath);
        //                System.IO.File.WriteAllBytes(filepath, pdfBuffer);
        //                objDocEXPERIAN.Close();

        //                CreditReportFiles files = new CreditReportFiles();
        //                //files.RoundType = values[3];
        //                files.RoundType = crediteEXPERIAN[excount].RoundType;
        //                files.CRFilename = filename;
        //                files.ClientId = Id;
        //                filenames.Add(files);
        //            }
        //            if (crediteEQUIFAX.Count > 0)
        //            {
        //                if (StringList.Count == 5)
        //                {
        //                    StringList.Add("EQUIFAX");
        //                }
        //                else
        //                {
        //                    StringList[5] = "";
        //                    StringList[5] = "EQUIFAX";
        //                }
        //                Session["values"] = StringList;
        //                objDocEQUIFAX = objPdf.CreateDocument();
        //                dynamic model = new ExpandoObject();
        //                int eqcount = crediteEQUIFAX.Count - 1;
        //                model.clientcredit = crediteEQUIFAX;
        //                string htmlToConvert = RenderViewAsString("ClientChallengeform", model);

        //                //objDocEQUIFAX.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=34;TopMargin=34;BottomMargin=34; hyperlinks=true; drawbackground=true");
        //                objDocEQUIFAX.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=54;TopMargin=54;BottomMargin=54; hyperlinks=true; drawbackground=true");

        //                filename = "Challenge-Account-EQUIFAX" + "-" + values[0] + "-" + values[1] + "-" + crediteEQUIFAX[eqcount].RoundType + "-" + date + ".pdf";
        //                filename = filename.Replace(" ", "");
        //                filepath = Server.MapPath("~/Documents/Challenge/Challenge-Account-EQUIFAX" + "-" + values[0] + "-" + values[1] + "-" + crediteEQUIFAX[eqcount].RoundType + "-" + date + ".pdf");
        //                //filepath = Server.MapPath("~/Documents/Challenge/Challenge-EQUIFAX.pdf");
        //                filepath = filepath.Replace(" ", "");
        //                objDocEQUIFAX.Save(filepath, false);
        //                pdfBuffer = System.IO.File.ReadAllBytes(filepath);
        //                System.IO.File.WriteAllBytes(filepath, pdfBuffer);
        //                objDocEQUIFAX.Close();

        //                CreditReportFiles files = new CreditReportFiles();
        //                //files.RoundType = values[3];
        //                files.RoundType = crediteEQUIFAX[eqcount].RoundType;
        //                files.CRFilename = filename;
        //                files.ClientId = Id;
        //                filenames.Add(files);
        //            }
        //            if (filenames.Count > 0)
        //            {
        //                functions.updatechallengefilepath(filenames, sno);
        //            }
        //            status = true;
        //        }

        //        catch (Exception ex)
        //        {
        //            status = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    { string msg = ex.Message; }
        //    return Json(status);

        //}
        public ActionResult ClientChallenges(string agency = null)
        {
            string ClientId = string.Empty;
            string Agentname = string.Empty;
            string AgentnameNew = string.Empty;
            try
            {
                ClientId = sessionData.GetClientId();
                Agentname = cfunction.GetAgentName(ClientId);
                AgentnameNew = cfunction.getAgentName(Convert.ToInt32(ClientId));
                ViewBag.name = Agentname;
                ViewBag.Agentname = AgentnameNew;
                ViewBag.Dasboard = sessionData.getDasboard();
                ViewBag.CreditReportItems = cfunction.ReportItemChallenges(ClientId.StringToInt(0), agency);
                ViewBag.CreditReportInquiresItems = cfunction.ReportItemInquiresChallenges(ClientId.StringToInt(0), agency);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return View();
        }
        public JsonResult AddCreditReport(string clientId, string mode = "", string round = "")
        {
            string ReportId = "";
            try
            {
                List<AccountHistory> accountHistories = new List<AccountHistory>();
                List<Inquires> inquires = new List<Inquires>();
                List<PublicRecord> publicRecords = new List<PublicRecord>();
                IdentityIQInfo IdentityIQInfo = new IdentityIQInfo();

                IdentityIQInfo = IQfunction.CheckIdentityIQInfo(clientId);
                CreditReportData tuple = GetCreditReportItemsbyReading(IdentityIQInfo);

                if (string.IsNullOrEmpty(tuple.errMsg))
                {
                    accountHistories = tuple.AccHistory;
                    inquires = tuple.inquiryDetails;
                    publicRecords = tuple.PublicRecords;
                    ReportId = cfunction.RefreshCreditReport(accountHistories, inquires, tuple.monthlyPayStatusHistoryDetails, clientId, mode, round, publicRecords);
                }
                else
                {
                    return Json(0);
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return Json(ReportId);
        }
        public JsonResult CheckIdentityIQInfo(long clientId)
        {
            bool status = false;
            try
            {
                status = IQfunction.CheckIdentityIQInfo(clientId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(status);
        }

        public ActionResult PullDetails()
        {
            return View();
        }

        public ActionResult Loadhtml()
        {
            List<AccountHistory> accountHistories = new List<AccountHistory>();
            List<Inquires> inquires = new List<Inquires>();
            string html = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Content/sample.html"));

            accountHistories = cfunction.GetAccountHistory(html);
            inquires = cfunction.GetInquiresFromHtml(html);
            ViewBag.inquires = inquires;
            return View(accountHistories);
        }

        //public JsonResult GetActiveCLientStatus(string ClientId)
        //{
        //	bool status = false;

        //	status = cfunction.GetActiveClientStatus(ClientId);

        //	return Json(status);
        //}
        public ActionResult CreditPullFirstTime(string clientId, string from = "", string mode = "")
        {
            //bool status = false;
            string ReportId = "";
            string role = Session["UserRole"].ToString();
            IdentityIQInfo objidentity = new IdentityIQInfo();
            try
            {

                bool checkclientexist = functions.GetCreditReportStatus(clientId);
                if (checkclientexist && mode == "Edit")
                {
                    return RedirectToAction("CreditItems", "Client", new { @ClientId = clientId, @from = from, @mode = mode });
                }
                objidentity = IQfunction.CheckIdentityIQInfo(clientId);

                if (objidentity.Password != null && objidentity.UserName != null && objidentity.Answer != null)
                {
                    List<AccountHistory> accountHistories = new List<AccountHistory>();
                    List<Inquires> inquires = new List<Inquires>();
                    List<PublicRecord> publicRecords = new List<PublicRecord>();
                    CreditReportData tuple = GetCreditReportItemsbyReading(objidentity);

                    if (string.IsNullOrEmpty(tuple.errMsg))
                    {
                        accountHistories = tuple.AccHistory;
                        inquires = tuple.inquiryDetails;
                        publicRecords = tuple.PublicRecords;

                        ReportId = cfunction.AddCreditReport(accountHistories, inquires, tuple.monthlyPayStatusHistoryDetails, clientId, mode, "", publicRecords);
                        return RedirectToAction("CreditItems", "Client", new { @ClientId = clientId, @from = from, @mode = mode });
                    }
                    else
                    {
                        TempData["errmsg"] = tuple.errMsg;

                        if (role == "agentstaff")
                        {
                            return RedirectToAction("AgentStaff", "dashboard");
                        }
                        if (role == "agentadmin")
                        {
                            return RedirectToAction("Agent", "dashboard");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errmsg"] = "error.";
                if (role == "agentstaff")
                {
                    return RedirectToAction("AgentStaff", "dashboard");
                }
                if (role == "agentadmin")
                {
                    return RedirectToAction("Agent", "dashboard");
                }
            }
            string user = "Agent";
            if (role == "agentstaff")
            { user = "AgentStaff"; }
            return RedirectToAction(user, "dashboard");
        }
        public ActionResult CreditPull(string clientId, string from = "", string mode = "")
        {
            string role = Session["UserRole"].ToString();
            IdentityIQInfo objidentity = new IdentityIQInfo();
            try
            {
                return RedirectToAction("CreditItems", "Client", new { @ClientId = clientId, @from = from, @mode = mode });
            }
            catch (Exception ex)
            { }

            if (role == "agentstaff")
            { return RedirectToAction("AgentStaff", "dashboard"); }
            else
            {
                return RedirectToAction("Agent", "dashboard");
            }

        }

        public CreditReportData GetCreditReportItemsbyReading(IdentityIQInfo IdentityIQInfo)
        {
            CreditReportData creditReportData = new CreditReportData();
            try
            {
                ClientFunction clientFunction = new ClientFunction();
                creditReportData = clientFunction.GetCreditReportItemsbyReading(IdentityIQInfo);
            }
            catch (Exception ex)
            { ex.insertTrace(""); }
            return creditReportData;
        }

        public JsonResult ClientChallengeform(List<CreditReportItems> credit, int Id, string[] values, List<PublicRecord> publicRecords,
            List<Inquires> inquires)
        {
            string status = "0";
            int sno = 0;
            //return Json(status);
            if (credit == null && publicRecords == null && inquires == null)
            {
                return Json(status);
            }
            try
            {
                string client = Id.ToString();
                ClientModel clientModel = cfunction.GetClient(client);
                if (!string.IsNullOrEmpty(Session["AgentId"].ToString()))
                {
                    string agentid = Session["AgentId"].ToString();
                    Agent agentAdddress = cfunction.GetAgentAddressById(agentid);
                    Session["AgentAddress"] = agentAdddress;
                }
                int count = 0;
                if (credit != null)
                {
                    count = credit.Count();
                }
                int i = 0;

                try
                {
                    ClientData cd = new ClientData();
                    if (credit != null)
                    {
                        sno = cd.getsnofromitems(Id.ToString(), credit[0].RoundType, "AH");
                        for (i = 0; i < count; i++)
                        {
                            cfunction.AddReportItemChallenges(credit[i], sno, Id, clientModel.isPrevItemInLastYear);
                            dynamic model = new ExpandoObject();
                            string AgentId = sessionData.GetAgentId();
                            string staffId = sessionData.GetStaffId();
                            string fullname = "";

                            cfunction.AddChallenge(credit[i], AgentId, staffId);
                            ViewBag.CreditReportItems = credit[i];

                            model.clientcredit = credit[i];

                            if (Session["Name"] != null)
                            {
                                fullname = Session["Name"].ToString();
                            }
                            var names = fullname.Split(' ');
                            string name = names[0];
                            int clientid = Id;
                            int? Report = credit[i].CredRepItemsId;
                        }
                    }
                    List<CreditReportItems> credite = functions.GetCreditReportChallengesAgentById(credit, Id);
                    List<CreditReportItems> EQCreditItems = credite.Where(x => x.Agency.ToUpper() == "EQUIFAX").ToList();
                    List<CreditReportItems> EXCreditItems = credite.Where(x => x.Agency.ToUpper() == "EXPERIAN").ToList();
                    List<CreditReportItems> TUCreditItems = credite.Where(x => x.Agency.ToUpper() == "TRANSUNION").ToList();



                    if (publicRecords != null)
                    {
                        int res = ClientChallengePRform(publicRecords, Id, values);
                        if (res > 0)
                        {
                            List<CreditReportItems> PublicRecords = functions.GetPRChallengesAgentById(publicRecords, Id);
                            List<CreditReportItems> eqPR = PublicRecords.Where(x => x.Agency.ToUpper() == "EQUIFAX").ToList();
                            List<CreditReportItems> exPR = PublicRecords.Where(x => x.Agency.ToUpper() == "EXPERIAN").ToList();
                            List<CreditReportItems> tuPR = PublicRecords.Where(x => x.Agency.ToUpper() == "TRANSUNION").ToList();
                            if (eqPR.Count > 0)
                            {
                                EQCreditItems.AddRange(eqPR);
                            }
                            if (exPR.Count > 0)
                            {
                                EXCreditItems.AddRange(exPR);
                            }
                            if (tuPR.Count > 0)
                            {
                                TUCreditItems.AddRange(tuPR);
                            }
                        }
                    }

                    if (inquires != null)
                    {
                        int resInq = ClientChallengeInquiresform(inquires, Id, values);
                        if (resInq > 0)
                        {
                            List<CreditReportItems> Inquires = functions.GetInquiriesChallengesAgentById(inquires, Id);
                            List<CreditReportItems> eqINQ = Inquires.Where(x => x.Agency.ToUpper() == "EQUIFAX").ToList();
                            List<CreditReportItems> exINQ = Inquires.Where(x => x.Agency.ToUpper() == "EXPERIAN").ToList();
                            List<CreditReportItems> tuINQ = Inquires.Where(x => x.Agency.ToUpper() == "TRANSUNION").ToList();
                            if (eqINQ.Count > 0)
                            {
                                EQCreditItems.AddRange(eqINQ);
                            }
                            if (exINQ.Count > 0)
                            {
                                EXCreditItems.AddRange(exINQ);
                            }
                            if (tuINQ.Count > 0)
                            {
                                TUCreditItems.AddRange(tuINQ);
                            }
                        }
                    }
                    string file1 = "", file2 = "", file3 = "";
                    if (EQCreditItems.Count > 0)
                    {
                        file1 = CreateChallengeLetters(EQCreditItems, clientModel, values, Id, sno);
                    }
                    if (EXCreditItems.Count > 0)
                    {
                        file2 = CreateChallengeLetters(EXCreditItems, clientModel, values, Id, sno);
                    }
                    if (TUCreditItems.Count > 0)
                    {
                        file3 = CreateChallengeLetters(TUCreditItems, clientModel, values, Id, sno);
                    }

                    if (string.IsNullOrEmpty(file1) && string.IsNullOrEmpty(file2) && string.IsNullOrEmpty(file3))
                    {
                        status = "1";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(file1))
                        {
                            status = file1;
                        }
                        if (!string.IsNullOrEmpty(file2))
                        {
                            status = status + "^" + file2;
                        }
                        if (!string.IsNullOrEmpty(file3))
                        {
                            status = status + "^" + file3;
                        }
                        status = status.TrimStart('^'); status = status.TrimEnd('^');
                        Session["files"] = status;
                    }


                }
                catch (Exception ex)
                {
                    status = "0";
                }
            }
            catch (Exception ex)
            { string msg = ex.Message; }
            return Json(status);
        }

        public string CreateChallengeLetters(List<CreditReportItems> credite, ClientModel clientModel, string[] values, int Id, int sno)
        {
            string res = "";
            try
            {
                string date = DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString()
                 + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                byte[] pdfBuffer = null;
                string filepath = "", filename = "";
                List<CreditReportFiles> filenames = new List<CreditReportFiles>();
                Session["ClientAddress"] = clientModel;
                List<string> StringList = new List<string>();

                try
                {
                    StringList.Add(clientModel.FirstName);
                    StringList.Add(clientModel.LastName);
                    StringList.Add(values[2]);
                    StringList.Add(values[3]);
                    StringList.Add(clientModel.SSN);
                }
                catch (Exception)
                { }

                List<CreditReportItems> crItems = new List<CreditReportItems>();

                for (int k = 0; k < credite.Count; k++)
                {
                    CreditReportItems cr = new CreditReportItems();
                    if (credite[k].ChallengeText != "NO CHALLENGE")
                    {
                        cr.MerchantName = credite[k].MerchantName;
                        cr.AccountId = credite[k].AccountId;
                        cr.OpenDate = credite[k].OpenDate;
                        cr.HighestBalance = credite[k].HighestBalance;
                        cr.CurrentBalance = credite[k].CurrentBalance;
                        cr.MonthlyPayment = credite[k].MonthlyPayment;
                        cr.LastReported = credite[k].LastReported;
                        cr.Agency = credite[k].Agency;
                        cr.ChallengeText = credite[k].ChallengeText;
                        cr.Status = credite[k].Status;
                        cr.CredRepItemsId = credite[k].CredRepItemsId;
                        cr.RoundType = credite[k].RoundType;
                        if (credite[k].AccountType == credite[k].ChallengeText)
                        {
                            credite[k].AccountType = credite[k].AccountTypeDetail;
                        }
                        cr.AccountType = credite[k].AccountType;
                        crItems.Add(cr);
                    }
                }

                AgentData agentData = new AgentData();
                int _checkExists = agentData.checkAutoChallenges(crItems[0].RoundType, crItems[0].Agency, Id.ToString(), "Account");

                PdfManager objPdf = new PdfManager();
                PdfDocument objDoc = objPdf.CreateDocument();
                objDoc.Title = "CreditReversalGuru";
                objDoc.Creator = "CreditReversalGuru";
                PdfFont objFont = objDoc.Fonts["Helvetica"];
                MemoryStream pdfStream = new MemoryStream();
                filenames = new List<CreditReportFiles>();
                if (crItems.Count > 0)
                {

                    StringList.Add(crItems[0].Agency);
                    Session["values"] = StringList;
                    objDoc = objPdf.CreateDocument();
                    dynamic model = new ExpandoObject();
                    model.clientcredit = crItems;
                    string htmlToConvert = RenderViewAsString("ClientChallengeform", model);
                    objDoc.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=54;TopMargin=54;BottomMargin=54; hyperlinks=true; drawbackground=true");
                    filename = "Challenge-Account-" + crItems[0].Agency + "-" + values[0] + "-" + values[1] + "-" + crItems[0].RoundType + "-" + date + ".pdf";
                    filename = filename.Replace(" ", "");
                    if (_checkExists == 0)
                    {
                        filepath = Server.MapPath("~/Documents/Challenge/Challenge-Account-" + crItems[0].Agency + "-" + values[0] + "-" + values[1]
                        + "-" + crItems[0].RoundType + "-" + date + ".pdf");
                    }
                    else
                    {
                        filepath = Server.MapPath("~/Documents/AutoPrint/Challenge-Account-" + crItems[0].Agency + "-" + values[0] + "-" + values[1]
                        + "-" + crItems[0].RoundType + "-" + date + ".pdf");
                    }

                    filepath = filepath.Replace(" ", "");

                    try
                    {
                        if (!string.IsNullOrEmpty(clientModel.sProofOfCard))
                        {
                            try
                            {
                                if (clientModel.sProofOfCard.ToUpper().Contains(".PDF"))
                                {
                                    byte[] bytes = null; PdfDocument objDoc2 = null;
                                    var doc = objPdf.OpenDocument(Server.MapPath("~/Documents/" + clientModel.sProofOfCard));
                                    bytes = doc.SaveToMemory();
                                    objDoc2 = objPdf.OpenDocument(bytes);
                                    objDoc.AppendDocument(objDoc2);
                                }
                                else
                                {
                                    byte[] bytes = null;
                                    bytes = pdfBuffer = System.IO.File.ReadAllBytes(Server.MapPath("~/Documents/" + clientModel.sProofOfCard));
                                    PdfImage objImage = objDoc.OpenImage(bytes);
                                    PdfPage objPage = objDoc.Pages.Add();
                                    PdfParam objParam = objPdf.CreateParam();
                                    objPage.Canvas.DrawImage(objImage, "x=100,y=400");
                                }
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    catch (Exception ex)
                    { }
                    objDoc.Save(filepath, false);
                    objDoc.Close();

                    CreditReportFiles files = new CreditReportFiles();
                    files.RoundType = crItems[0].RoundType;
                    files.CRFilename = filename;
                    files.ClientId = Id;
                    if (_checkExists == 0)
                    {
                        files.isManual = 1;
                    }
                    filenames.Add(files);
                }
                if (filenames.Count > 0)
                {
                    functions.updatechallengefilepath(filenames, sno);
                }
                StringList.Clear();

                //int _checkExists = agentData.checkAutoChallenges(crItems[0].RoundType, crItems[0].Agency, Id.ToString(), "Account");
                if (_checkExists == 0)
                {
                    res = filename;
                }
            }
            catch (Exception ex)
            { }
            return res;
        }

        //public JsonResult ClientChallengePRformBKUP(List<PublicRecord> credit, int Id, string[] values)
        //{

        //    string client = Id.ToString();
        //    ClientModel clientModel = cfunction.GetClient(client);
        //    int sno = 0;

        //    string status = "";
        //    int count = 0;
        //    if (credit != null)
        //    {
        //        count = credit.Count();
        //    }
        //    int i = 0;

        //    try
        //    {
        //        ClientData cd = new ClientData();

        //        if (credit != null)
        //        {
        //            sno = cd.getsnofromitems(Id.ToString(), credit[0].RoundType, "PR");

        //            for (i = 0; i < count; i++)
        //            {
        //                cfunction.AddReportItemPRChallenges(credit[i], values[3], sno, Id);
        //                dynamic model = new ExpandoObject();
        //                string AgentId = sessionData.GetAgentId();
        //                string staffId = sessionData.GetStaffId();
        //                string fullname = "";

        //              //  cfunction.AddInquiresChallenge(credit[i], AgentId, staffId);
        //                ViewBag.CreditReportItems = credit[i];

        //                model.clientcredit = credit[i];

        //                if (Session["Name"] != null)
        //                {
        //                    fullname = Session["Name"].ToString();
        //                }
        //                var names = fullname.Split(' ');
        //                string name = names[0];
        //                int clientid = Id;
        //                string Report = credit[i].PublicRecordId;

        //            }
        //        }
        //        List<PublicRecord> PublicRecords = functions.GetPRChallengesAgentById(credit, Id);

        //        List<PublicRecord> eqInquires = PublicRecords.Where(x => x.atbureau.ToUpper() == "EQUIFAX").ToList();
        //        List<PublicRecord> exInquires = PublicRecords.Where(x => x.atbureau.ToUpper() == "EXPERIAN").ToList();
        //        List<PublicRecord> tuInquires = PublicRecords.Where(x => x.atbureau.ToUpper() == "TRANSUNION").ToList();

        //        string file1 = "", file2 = "", file3 = "";
        //        if (eqInquires.Count > 0)
        //        {
        //            file1 = CreateChallengeLettersForPR(eqInquires, Id, values, clientModel, sno);
        //        }
        //        if (exInquires.Count > 0)
        //        {
        //            file2 = CreateChallengeLettersForPR(exInquires, Id, values, clientModel, sno);
        //        }
        //        if (tuInquires.Count > 0)
        //        {
        //            file3 = CreateChallengeLettersForPR(tuInquires, Id, values, clientModel, sno);
        //        }


        //        if (string.IsNullOrEmpty(file1) && string.IsNullOrEmpty(file2) && string.IsNullOrEmpty(file3))
        //        {
        //            status = "1";
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(file1))
        //            {
        //                status = file1;
        //            }
        //            if (!string.IsNullOrEmpty(file2))
        //            {
        //                status = status + "^" + file2;
        //            }
        //            if (!string.IsNullOrEmpty(file3))
        //            {
        //                status = status + "^" + file3;
        //            }
        //            status = status.TrimStart('^'); status = status.TrimEnd('^');
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        status = "0";
        //    }
        //    return Json(status);
        //}
        public int ClientChallengePRform(List<PublicRecord> credit, int Id, string[] values)
        {
            int res = 0;
            string client = Id.ToString();
            ClientModel clientModel = cfunction.GetClient(client);
            int sno = 0;

            string status = "";
            int count = 0;
            if (credit != null)
            {
                count = credit.Count();
            }
            int i = 0;

            try
            {
                ClientData cd = new ClientData();

                if (credit != null)
                {
                    sno = cd.getsnofromitems(Id.ToString(), credit[0].RoundType, "PR");
                    for (i = 0; i < count; i++)
                    {
                        cfunction.AddReportItemPRChallenges(credit[i], "", sno, Id);
                    }
                }
                res = 1;
            }
            catch (Exception ex)
            {
                res = 0;
            }
            return res;
            //return Json(status);
        }
        public string CreateChallengeLettersForPR(List<PublicRecord> Inquires, int Id, string[] values, ClientModel clientModel, int sno)
        {
            string res = "";
            try
            {
                List<PublicRecord> InquiresAllAgencies = new List<PublicRecord>();
                List<CreditReportFiles> filenames = new List<CreditReportFiles>();
                byte[] pdfBuffer = null;
                PdfManager objPdf = new PdfManager();
                PdfDocument objDoc = objPdf.CreateDocument();
                MemoryStream pdfStream = new MemoryStream();
                string filepath = "", filename = "";

                string date = DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString()
                   + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                List<string> StringList = new List<string>();
                Session["ClientAddress"] = clientModel;
                StringList.Add(clientModel.FirstName);
                StringList.Add(clientModel.LastName);
                StringList.Add(values[2]);
                StringList.Add(values[3]);
                StringList.Add(clientModel.SSN);

                for (int k = 0; k < Inquires.Count; k++)
                {
                    PublicRecord cr = new PublicRecord();
                    if (Inquires[k].ChallengeText != "NO CHALLENGE")
                    {
                        cr.atcourtName = Inquires[k].atcourtName;
                        cr.AccountType = Inquires[k].AccountType;
                        cr.atdateFiled = Inquires[k].atdateFiled;
                        cr.atbureau = Inquires[k].atbureau;
                        if (Inquires[k].AccountType == Inquires[k].ChallengeText)
                        {
                            Inquires[k].AccountType = Inquires[k].AccountType; //Need to fix
                        }
                        cr.ChallengeText = Inquires[k].ChallengeText;
                        cr.RoundType = Inquires[k].RoundType;
                        cr.AccountType = Inquires[k].AccountType;
                        InquiresAllAgencies.Add(cr);
                    }
                }
                filenames = new List<CreditReportFiles>();
                if (InquiresAllAgencies.Count > 0)
                {
                    StringList.Add(Inquires[0].atbureau);
                    Session["values"] = StringList;
                    objDoc = objPdf.CreateDocument();
                    dynamic model = new ExpandoObject();
                    int trcount = InquiresAllAgencies.Count - 1;
                    model.clientcredit = InquiresAllAgencies;
                    string htmlToConvert = RenderViewAsString("ClientPRChallengeform", model);
                    objDoc.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=54;TopMargin=54;BottomMargin=54; hyperlinks=true; drawbackground=true");
                    filename = "Challenge-PublicRecords-" + Inquires[0].atbureau + "-" + values[0] + "-" + values[1] + "-" + InquiresAllAgencies[0].RoundType + "-" + date + ".pdf";
                    filename = filename.Replace(" ", "");
                    filepath = Server.MapPath("~/Documents/Challenge/Challenge-PublicRecords-" + Inquires[0].atbureau + "-"
                        + values[0] + "-" + values[1] + "-" + InquiresAllAgencies[0].RoundType + "-" + date + ".pdf");
                    filepath = filepath.Replace(" ", "");

                    if (!string.IsNullOrEmpty(clientModel.sProofOfCard))
                    {
                        try
                        {
                            byte[] bytes = null;
                            bytes = pdfBuffer = System.IO.File.ReadAllBytes(Server.MapPath("~/Documents/" + clientModel.sProofOfCard));
                            PdfImage objImage = objDoc.OpenImage(bytes);
                            PdfPage objPage = objDoc.Pages.Add();
                            PdfParam objParam = objPdf.CreateParam();
                            objPage.Canvas.DrawImage(objImage, "x=100,y=400");
                        }
                        catch (Exception)
                        { }

                    }
                    objDoc.Save(filepath, false);
                    objDoc.Close();

                    CreditReportFiles files = new CreditReportFiles();
                    files.RoundType = InquiresAllAgencies[trcount].RoundType;
                    files.CRFilename = filename;
                    files.ClientId = Id;
                    filenames.Add(files);
                }
                if (filenames.Count > 0)
                {
                    functions.updatechallengefilepath(filenames, sno);
                }
                StringList.Clear();
                AgentData agentData = new AgentData();
                int _checkExists = agentData.checkAutoChallenges(InquiresAllAgencies[0].RoundType, Inquires[0].atbureau, Id.ToString(), "PR");
                if (_checkExists == 0)
                {
                    res = filename;
                }
            }
            catch (Exception ex)
            { }
            return res;
        }
        public int ClientChallengeInquiresform(List<Inquires> credit, int Id, string[] values)
        {
            int res = 0;
            string client = Id.ToString();
            ClientModel clientModel = cfunction.GetClient(client);
            int sno = 0;
            int count = 0;
            if (credit != null)
            {
                count = credit.Count();
            }
            int i = 0;

            try
            {
                ClientData cd = new ClientData();

                if (credit != null)
                {
                    sno = cd.getsnofromitems(Id.ToString(), credit[0].RoundType, "INQ");
                    for (i = 0; i < count; i++)
                    {
                        cfunction.AddReportItemInquiriesChallenges(credit[i], "", sno, Id);
                        //string AgentId = sessionData.GetAgentId();
                        //string staffId = sessionData.GetStaffId();
                        //cfunction.AddInquiresChallenge(credit[i], AgentId, staffId);
                        //ViewBag.CreditReportItems = credit[i];
                    }
                }
                //List<Inquires> Inquires = functions.GetInquiriesChallengesAgentById(credit, Id);
                //List<Inquires> eqInquires = Inquires.Where(x => x.CreditBureau.ToUpper() == "EQUIFAX").ToList();
                //List<Inquires> exInquires = Inquires.Where(x => x.CreditBureau.ToUpper() == "EXPERIAN").ToList();
                //List<Inquires> tuInquires = Inquires.Where(x => x.CreditBureau.ToUpper() == "TRANSUNION").ToList();

                res = 1;
            }
            catch (Exception ex)
            {
                res = 0;
            }
            return res;
        }
        //public JsonResult ClientChallengeInquiresform(List<Inquires> credit, int Id, string[] values)
        //{

        //    string client = Id.ToString();
        //    ClientModel clientModel = cfunction.GetClient(client);
        //    int sno = 0;

        //    string status = "";
        //    int count = 0;
        //    if (credit != null)
        //    {
        //        count = credit.Count();
        //    }
        //    int i = 0;

        //    try
        //    {
        //        ClientData cd = new ClientData();

        //        if (credit != null)
        //        {
        //            sno = cd.getsnofromitems(Id.ToString(), credit[0].RoundType, "INQ");
        //            for (i = 0; i < count; i++)
        //            {
        //                cfunction.AddReportItemInquiriesChallenges(credit[i], "", sno, Id);
        //                dynamic model = new ExpandoObject();
        //                string AgentId = sessionData.GetAgentId();
        //                string staffId = sessionData.GetStaffId();
        //                string fullname = "";

        //                cfunction.AddInquiresChallenge(credit[i], AgentId, staffId);
        //                ViewBag.CreditReportItems = credit[i];

        //                model.clientcredit = credit[i];

        //                if (Session["Name"] != null)
        //                {
        //                    fullname = Session["Name"].ToString();
        //                }
        //                var names = fullname.Split(' ');
        //                string name = names[0];
        //                int clientid = Id;
        //                string Report = credit[i].CreditInqId;

        //            }
        //        }
        //        List<Inquires> Inquires = functions.GetInquiriesChallengesAgentById(credit, Id);

        //        List<Inquires> eqInquires = Inquires.Where(x => x.CreditBureau.ToUpper() == "EQUIFAX").ToList();
        //        List<Inquires> exInquires = Inquires.Where(x => x.CreditBureau.ToUpper() == "EXPERIAN").ToList();
        //        List<Inquires> tuInquires = Inquires.Where(x => x.CreditBureau.ToUpper() == "TRANSUNION").ToList();

        //        string file1 = "", file2 = "", file3 = "";
        //        if (eqInquires.Count > 0)
        //        {
        //            file1 = CreateChallengeLettersForInq(eqInquires, Id, values, clientModel, sno);
        //        }
        //        if (exInquires.Count > 0)
        //        {
        //            file2 = CreateChallengeLettersForInq(exInquires, Id, values, clientModel, sno);
        //        }
        //        if (tuInquires.Count > 0)
        //        {
        //            file3 = CreateChallengeLettersForInq(tuInquires, Id, values, clientModel, sno);
        //        }


        //        if (string.IsNullOrEmpty(file1) && string.IsNullOrEmpty(file2) && string.IsNullOrEmpty(file3))
        //        {
        //            status = "1";
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(file1))
        //            {
        //                status = file1;
        //            }
        //            if (!string.IsNullOrEmpty(file2))
        //            {
        //                status = status + "^" + file2;
        //            }
        //            if (!string.IsNullOrEmpty(file3))
        //            {
        //                status = status + "^" + file3;
        //            }
        //            status = status.TrimStart('^'); status = status.TrimEnd('^');
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        status = "0";
        //    }
        //    return Json(status);
        //}

        public string CreateChallengeLettersForInq(List<Inquires> Inquires, int Id, string[] values, ClientModel clientModel, int sno)
        {
            string res = "";
            try
            {
                List<Inquires> InquiresAllAgencies = new List<Inquires>();
                List<CreditReportFiles> filenames = new List<CreditReportFiles>();
                byte[] pdfBuffer = null;
                PdfManager objPdf = new PdfManager();
                PdfDocument objDoc = objPdf.CreateDocument();
                MemoryStream pdfStream = new MemoryStream();
                string filepath = "", filename = "";

                string date = DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString()
                   + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                List<string> StringList = new List<string>();
                Session["ClientAddress"] = clientModel;
                StringList.Add(clientModel.FirstName);
                StringList.Add(clientModel.LastName);
                StringList.Add(values[2]);
                StringList.Add(values[3]);
                StringList.Add(clientModel.SSN);

                for (int k = 0; k < Inquires.Count; k++)
                {
                    Inquires cr = new Inquires();
                    if (Inquires[k].ChallengeText != "NO CHALLENGE")
                    {
                        cr.CreditorName = Inquires[k].CreditorName;
                        cr.TypeofBusiness = Inquires[k].TypeofBusiness;
                        cr.Dateofinquiry = Inquires[k].Dateofinquiry;
                        cr.CreditBureau = Inquires[k].CreditBureau;
                        if (Inquires[k].AccountType == Inquires[k].ChallengeText)
                        {
                            Inquires[k].AccountType = Inquires[k].AccountTypeDetails; //Need to fix
                        }
                        cr.ChallengeText = Inquires[k].ChallengeText;
                        cr.RoundType = Inquires[k].RoundType;
                        cr.AccountType = Inquires[k].AccountType;
                        InquiresAllAgencies.Add(cr);
                    }
                }
                filenames = new List<CreditReportFiles>();
                if (InquiresAllAgencies.Count > 0)
                {
                    StringList.Add(Inquires[0].CreditBureau);
                    Session["values"] = StringList;
                    objDoc = objPdf.CreateDocument();
                    dynamic model = new ExpandoObject();
                    int trcount = InquiresAllAgencies.Count - 1;
                    model.clientcredit = InquiresAllAgencies;
                    string htmlToConvert = RenderViewAsString("ClientInquiriesChallengeform", model);
                    objDoc.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=54;TopMargin=54;BottomMargin=54; hyperlinks=true; drawbackground=true");
                    filename = "Challenge-Inquires-" + Inquires[0].CreditBureau + "-" + values[0] + "-" + values[1] + "-" + InquiresAllAgencies[0].RoundType + "-" + date + ".pdf";
                    filename = filename.Replace(" ", "");
                    filepath = Server.MapPath("~/Documents/Challenge/Challenge-Inquires-" + Inquires[0].CreditBureau + "-"
                        + values[0] + "-" + values[1] + "-" + InquiresAllAgencies[0].RoundType + "-" + date + ".pdf");
                    filepath = filepath.Replace(" ", "");

                    try
                    {
                        if (!string.IsNullOrEmpty(clientModel.sProofOfCard))
                        {
                            try
                            {
                                if (clientModel.sProofOfCard.ToUpper().Contains(".PDF"))
                                {
                                    byte[] bytes = null; PdfDocument objDoc2 = null;
                                    var doc = objPdf.OpenDocument(Server.MapPath("~/Documents/" + clientModel.sProofOfCard));
                                    bytes = doc.SaveToMemory();
                                    objDoc2 = objPdf.OpenDocument(bytes);
                                    objDoc.AppendDocument(objDoc2);
                                }
                                else
                                {
                                    byte[] bytes = null;
                                    bytes = pdfBuffer = System.IO.File.ReadAllBytes(Server.MapPath("~/Documents/" + clientModel.sProofOfCard));
                                    PdfImage objImage = objDoc.OpenImage(bytes);
                                    PdfPage objPage = objDoc.Pages.Add();
                                    PdfParam objParam = objPdf.CreateParam();
                                    objPage.Canvas.DrawImage(objImage, "x=100,y=400");
                                }
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    catch (Exception ex)
                    { }
                    objDoc.Save(filepath, false);
                    objDoc.Close();


                    CreditReportFiles files = new CreditReportFiles();
                    files.RoundType = InquiresAllAgencies[trcount].RoundType;
                    files.CRFilename = filename;
                    files.ClientId = Id;
                    filenames.Add(files);
                }
                if (filenames.Count > 0)
                {
                    functions.updatechallengefilepath(filenames, sno);
                }
                StringList.Clear();
                AgentData agentData = new AgentData();
                int _checkExists = agentData.checkAutoChallenges(InquiresAllAgencies[0].RoundType, Inquires[0].CreditBureau, Id.ToString(), "Inquires");
                if (_checkExists == 0)
                {
                    res = filename;
                }
            }
            catch (Exception ex)
            { }
            return res;
        }

        //public JsonResult ClientChallengeInquiresform(List<Inquires> credit, int Id, string[] values)
        //{
        //    string date = string.Empty;
        //    int sno = 0;
        //    date = DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString()
        //        + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
        //    List<Inquires> InquiresTRANSUNION = new List<Inquires>();
        //    List<Inquires> InquiresEXPERIAN = new List<Inquires>();
        //    List<Inquires> InquiresEQUIFAX = new List<Inquires>();

        //    List<CreditReportFiles> filenames = new List<CreditReportFiles>();
        //    bool status = false;
        //    int count = 0;
        //    if (credit != null)
        //    {
        //        count = credit.Count();
        //    }
        //    int i = 0;
        //    byte[] pdfBuffer = null;
        //    PdfManager objPdf = new PdfManager();
        //    PdfDocument objDocTRANSUNION = objPdf.CreateDocument();
        //    PdfDocument objDocEXPERIAN = objPdf.CreateDocument();
        //    PdfDocument objDocEQUIFAX = objPdf.CreateDocument();
        //    MemoryStream pdfStream = new MemoryStream();
        //    string filepath = "", filename = "";
        //    try
        //    {
        //        ClientData cd = new ClientData();

        //        if (credit != null)
        //        {

        //            for (i = 0; i < count; i++)
        //            {
        //                sno = cd.getsnofromitems(Id.ToString(), credit[i].RoundType);
        //                cfunction.AddReportItemInquiriesChallenges(credit[i], values[3], sno, Id);
        //                dynamic model = new ExpandoObject();
        //                string AgentId = sessionData.GetAgentId();
        //                string staffId = sessionData.GetStaffId();
        //                string fullname = "";

        //                cfunction.AddInquiresChallenge(credit[i], AgentId, staffId);
        //                ViewBag.CreditReportItems = credit[i];

        //                model.clientcredit = credit[i];

        //                if (Session["Name"] != null)
        //                {
        //                    fullname = Session["Name"].ToString();
        //                }
        //                var names = fullname.Split(' ');
        //                string name = names[0];
        //                int clientid = Id;
        //                string Report = credit[i].CreditInqId;

        //            }
        //        }
        //        List<Inquires> Inquires = functions.GetInquiriesChallengesAgentById(credit, Id);

        //        string client = Id.ToString();
        //        ClientModel clientModel = cfunction.GetClient(client);

        //        List<string> StringList = new List<string>();
        //        StringList.Add(clientModel.FirstName);
        //        StringList.Add(clientModel.LastName);
        //        StringList.Add(values[2]);
        //        StringList.Add(values[3]);
        //        StringList.Add(clientModel.SSN);


        //        for (int k = 0; k < Inquires.Count; k++)
        //        {
        //            Inquires cr = new Inquires();

        //            if (Inquires[k].CreditBureau == "TransUnion" && Inquires[k].ChallengeText != "NO CHALLENGE")
        //            {
        //                cr.CreditorName = Inquires[k].CreditorName;
        //                cr.TypeofBusiness = Inquires[k].TypeofBusiness;
        //                cr.Dateofinquiry = Inquires[k].Dateofinquiry;
        //                cr.CreditBureau = Inquires[k].CreditBureau;
        //                cr.ChallengeText = Inquires[k].ChallengeText;
        //                cr.RoundType = Inquires[k].RoundType;
        //                cr.AccountType = Inquires[k].AccountType;
        //                InquiresTRANSUNION.Add(cr);
        //            }
        //            if ("Experian" == Inquires[k].CreditBureau && Inquires[k].ChallengeText != "NO CHALLENGE")
        //            {
        //                cr.CreditorName = Inquires[k].CreditorName;
        //                cr.TypeofBusiness = Inquires[k].TypeofBusiness;
        //                cr.Dateofinquiry = Inquires[k].Dateofinquiry;
        //                cr.CreditBureau = Inquires[k].CreditBureau;
        //                cr.ChallengeText = Inquires[k].ChallengeText;
        //                cr.RoundType = Inquires[k].RoundType;
        //                cr.AccountType = Inquires[k].AccountType;
        //                InquiresEXPERIAN.Add(cr);
        //            }
        //            if ("Equifax" == Inquires[k].CreditBureau && Inquires[k].ChallengeText != "NO CHALLENGE")
        //            {
        //                cr.CreditorName = Inquires[k].CreditorName;
        //                cr.TypeofBusiness = Inquires[k].TypeofBusiness;
        //                cr.Dateofinquiry = Inquires[k].Dateofinquiry;
        //                cr.CreditBureau = Inquires[k].CreditBureau;
        //                cr.ChallengeText = Inquires[k].ChallengeText;
        //                cr.RoundType = Inquires[k].RoundType;
        //                cr.AccountType = Inquires[k].AccountType;
        //                InquiresEQUIFAX.Add(cr);
        //            }

        //        }

        //        filenames = new List<CreditReportFiles>();

        //        if (InquiresTRANSUNION.Count > 0)
        //        {
        //            StringList.Add("TRANSUNION");
        //            Session["values"] = StringList;
        //            objDocTRANSUNION = objPdf.CreateDocument();
        //            dynamic model = new ExpandoObject();
        //            int trcount = InquiresTRANSUNION.Count - 1;
        //            model.clientcredit = InquiresTRANSUNION;
        //            string htmlToConvert = RenderViewAsString("ClientInquiriesChallengeform", model);
        //            objDocTRANSUNION.ImportFromUrl(htmlToConvert, "scale=0.8; hyperlinks=true; drawbackground=true");
        //            filename = "Challenge-Inquires-TRANSUNION" + "-" + values[0] + "-" + values[1] + "-" + InquiresTRANSUNION[trcount].RoundType + "-" + date + ".pdf";
        //            filename = filename.Replace(" ", "");
        //            filepath = Server.MapPath("~/Documents/Challenge/Challenge-Inquires-TRANSUNION" + "-" + values[0] + "-" + values[1] + "-" + InquiresTRANSUNION[trcount].RoundType + "-" + date + ".pdf");
        //            // filepath = Server.MapPath("~/Documents/Challenge/Challenge-TRANSUNION.pdf"); // + "-" + values[0] + "-" + values[1] + "-" + DateTime.Now.ToShortDateString() + "-" + values[3] + ".pdf");
        //            filepath = filepath.Replace(" ", "");
        //            objDocTRANSUNION.Save(filepath, false);
        //            pdfBuffer = System.IO.File.ReadAllBytes(filepath);
        //            System.IO.File.WriteAllBytes(filepath, pdfBuffer);
        //            objDocTRANSUNION.Close();

        //            CreditReportFiles files = new CreditReportFiles();
        //            files.RoundType = InquiresTRANSUNION[trcount].RoundType;
        //            files.CRFilename = filename;
        //            files.ClientId = Id;
        //            filenames.Add(files);
        //        }


        //        if (InquiresEXPERIAN.Count > 0)
        //        {
        //            if (StringList.Count == 5)
        //            {
        //                StringList.Add("EXPERIAN");
        //            }
        //            else
        //            {
        //                StringList[5] = "";
        //                StringList[5] = "EXPERIAN";
        //            }

        //            Session["values"] = StringList;
        //            objDocEXPERIAN = objPdf.CreateDocument();
        //            dynamic model = new ExpandoObject();
        //            int excount = InquiresEXPERIAN.Count - 1;
        //            model.clientcredit = InquiresEXPERIAN;
        //            string htmlToConvert = RenderViewAsString("ClientInquiriesChallengeform", model);
        //            objDocEXPERIAN.ImportFromUrl(htmlToConvert, "scale=0.8; hyperlinks=true; drawbackground=true");
        //            filename = "Challenge-Inquires-EXPERIAN" + "-" + values[0] + "-" + values[1] + "-" + InquiresEXPERIAN[excount].RoundType + "-" + date + ".pdf";
        //            filename = filename.Replace(" ", "");
        //            filepath = Server.MapPath("~/documents/Challenge/Challenge-Inquires-EXPERIAN" + "-" + values[0] + "-" + values[1] + "-" + InquiresEXPERIAN[excount].RoundType + "-" + date + ".pdf");
        //            //filepath = Server.MapPath("~/Documents/Challenge/Challenge-EXPERIAN.pdf");
        //            filepath = filepath.Replace(" ", "");
        //            objDocEXPERIAN.Save(filepath, false);
        //            pdfBuffer = System.IO.File.ReadAllBytes(filepath);
        //            System.IO.File.WriteAllBytes(filepath, pdfBuffer);
        //            objDocEXPERIAN.Close();

        //            CreditReportFiles files = new CreditReportFiles();
        //            files.RoundType = InquiresEXPERIAN[excount].RoundType;
        //            files.CRFilename = filename;
        //            files.ClientId = Id;
        //            filenames.Add(files);
        //        }

        //        if (InquiresEQUIFAX.Count > 0)
        //        {
        //            if (StringList.Count == 4)
        //            {
        //                StringList.Add("EQUIFAX");
        //            }
        //            else
        //            {
        //                StringList[5] = "";
        //                StringList[5] = "EQUIFAX";
        //            }
        //            Session["values"] = StringList;
        //            objDocEQUIFAX = objPdf.CreateDocument();
        //            dynamic model = new ExpandoObject();
        //            int eqcount = InquiresEQUIFAX.Count - 1;
        //            model.clientcredit = InquiresEQUIFAX;
        //            string htmlToConvert = RenderViewAsString("ClientInquiriesChallengeform", model);

        //            objDocEQUIFAX.ImportFromUrl(htmlToConvert, "scale=0.8; hyperlinks=true; drawbackground=true");
        //            filename = "Challenge-Inquires-EQUIFAX" + "-" + values[0] + "-" + values[1] + "-" + InquiresEQUIFAX[eqcount].RoundType + "-" + date + ".pdf";
        //            filename = filename.Replace(" ", "");
        //            filepath = Server.MapPath("~/documents/Challenge/Challenge-Inquires-EQUIFAX" + "-" + values[0] + "-" + values[1] + "-" + InquiresEQUIFAX[eqcount].RoundType + "-" + date + ".pdf");
        //            //filepath = Server.MapPath("~/Documents/Challenge/Challenge-EQUIFAX.pdf");
        //            filepath = filepath.Replace(" ", "");
        //            objDocEQUIFAX.Save(filepath, false);
        //            pdfBuffer = System.IO.File.ReadAllBytes(filepath);
        //            System.IO.File.WriteAllBytes(filepath, pdfBuffer);
        //            objDocEQUIFAX.Close();

        //            CreditReportFiles files = new CreditReportFiles();
        //            files.RoundType = InquiresEQUIFAX[eqcount].RoundType;
        //            files.CRFilename = filename;
        //            files.ClientId = Id;
        //            filenames.Add(files);
        //        }

        //        if (filenames.Count > 0)
        //        {
        //            functions.updatechallengefilepath(filenames, sno);
        //        }
        //        status = true;

        //    }

        //    catch (Exception ex)
        //    {
        //        status = true;
        //    }
        //    return Json(status);

        //}


        [HttpGet]
        public JsonResult GetChallengeTextByAccountTypeAndRound(string Accounttype)
        {

            List<string> AccountType = functions.GetChallengeTextByAccountTypeAndRound(Accounttype);

            return Json(AccountType, JsonRequestBehavior.AllowGet);

        }

        public string ResizeImage(HttpPostedFileBase fileToUpload)
        {
            string name = Path.GetFileNameWithoutExtension(fileToUpload.FileName);
            var ext = Path.GetExtension(fileToUpload.FileName);
            string myfile = "Client-ProofOfCard-" + name + ext;



            try
            {
                using (Image image = Image.FromStream(fileToUpload.InputStream, true, false))
                {
                    var path = Path.Combine(Server.MapPath("~/documents"), myfile);
                    try
                    {
                        var height = image.Height; var width = image.Width;
                        //Size can be change according to your requirement 
                        float thumbWidth = 500F;
                        float thumbHeight = 500F;
                        //calculate  image  size
                        if (image.Width > image.Height)
                        {
                            thumbHeight = ((float)image.Height / image.Width) * thumbWidth;
                        }
                        else
                        {
                            thumbWidth = ((float)image.Width / image.Height) * thumbHeight;
                        }



                        int actualthumbWidth = Convert.ToInt32(Math.Floor(thumbWidth));
                        int actualthumbHeight = Convert.ToInt32(Math.Floor(thumbHeight));
                        var thumbnailBitmap = new Bitmap(actualthumbWidth, actualthumbHeight);
                        var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
                        thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                        thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                        thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        var imageRectangle = new Rectangle(0, 0, actualthumbWidth, actualthumbHeight);
                        thumbnailGraph.DrawImage(image, imageRectangle);
                        var ms = new MemoryStream();
                        thumbnailBitmap.Save(path, ImageFormat.Jpeg);
                        ms.Position = 0;
                        GC.Collect();
                        thumbnailGraph.Dispose();
                        thumbnailBitmap.Dispose();
                        image.Dispose();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return myfile;
        }
        public ActionResult ChallengeInfo()
        {
            ViewBag.Dasboard = sessionData.getDasboard();
            return View();
        }



        [HttpPost]
        public ActionResult ChallengeInfo(string strFiles = "")
        {
            int res = 0;
            try
            {
                DashboardFunctions functions = new DashboardFunctions();
                ViewBag.Dasboard = sessionData.getDasboard();
                if (!string.IsNullOrEmpty(strFiles))
                {
                    string sourcePath = Server.MapPath("~/documents/Challenge/");
                    string destPath = Server.MapPath("~/documents/AutoPrint/");
                    string[] files = strFiles.Split('^');
                    foreach (string s in files)
                    {
                        System.IO.File.Copy(sourcePath + s, destPath + s, true);
                        functions.UpdateCreditReportFileIsMoved(s);
                    }
                    
                    
                    res = 1;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}
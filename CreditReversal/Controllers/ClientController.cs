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

        [HttpGet]        public ActionResult CreditItems(string ClientId, string from = "")        {            string role = sessionData.GetUserRole();            string[] round = null;            try            {                ClientModel clientModel = cfunction.GetClient(ClientId);                if (clientModel != null)                {                    ViewBag.Cname = clientModel.FullName;                }                string fullname = string.Empty;                if (Session["Name"] != null)                {                    fullname = Session["Name"].ToString();                }                var names = fullname.Split(' ');                string name = names[0];                ViewBag.name = name;                ViewBag.ClientName = fullname;                ViewBag.AgentName = cfunction.getAgentName(Convert.ToInt32(ClientId));                string agentid = sessionData.GetAgentId();                string staffid = sessionData.GetStaffId();                round = CData.GetRoundType(ClientId);                List<CreditReportItems> creditReportItems = cfunction.GetCreditReportItems(Convert.ToInt32(ClientId));                if (creditReportItems.Count > 0)                {                    ViewBag.DateReportPulls = round[1];                    ViewBag.CreditReportItems = creditReportItems;
                    //ViewBag.Round = creditReportItems.First().RoundType;
                    ViewBag.Round = round[0];                    string dateInString = round[1];                    DateTime startDate = DateTime.Parse(dateInString);                    DateTime expiryDate = startDate.AddDays(30);                    DateTime dexpiryDate = Convert.ToDateTime(expiryDate.ToString("MM/dd/yyyy").Replace("-", "/"));                    if (DateTime.Now.Date >= dexpiryDate)                    {                        ViewBag.Status = "true";                    }                }                ViewBag.challengeMasters = cfunction.GetChallengeMasters(agentid.StringToInt(0), staffid.StringToInt(0));                ViewBag.Dasboard = sessionData.getDasboard();
                //List<CreditReportItems> creditReportItemChallenges = cfunction.ReportItemChallenges(Convert.ToInt32(ClientId));
                //if (creditReportItemChallenges.Count > 0)
                //{
                //    ViewBag.ReportItemChallenges = creditReportItemChallenges;
                //    ViewBag.Challenge = creditReportItemChallenges.Where(x => x.Agency == "TRANSUNION").First().Challenge;
                //}
                ViewBag.Role = role;
                /////////////////////////////////////////
                DashboardFunctions functions = new DashboardFunctions();                List<CreditItems> challenges = functions.GetCreditReportChallengesAgent(Convert.ToInt32(ClientId), null, role);                ViewBag.challenges = challenges;
                List<Inquires> Inquires = functions.GetCreditReportInquiresAgent(Convert.ToInt32(ClientId), null, role);                ViewBag.Inquires = Inquires;

            }            catch (Exception ex) { ex.insertTrace(""); }            return View();        }
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
            ClientModel client = new ClientModel();
            string role = sessionData.GetUserRole();
            string from = TempData["from"] != null ? TempData["from"].ToString() : string.Empty;
            long res = 0;
            try
            {
                //if (Session["ClientId"] != null)
                //{
                //	client = cfunction.GetClient(Session["ClientId"].ToString());
                //	clientModel.ClientId = Convert.ToInt32(Session["ClientId"].ToString());					
                //}
                if (clientModel.FProofOfCard != null)
                {
                    string PFileName = Path.GetFileName(clientModel.FProofOfCard.FileName);
                    string _path = Server.MapPath("~/documents/" + "Client-" + clientModel.ClientId + "-" + PFileName);
                    clientModel.FProofOfCard.SaveAs(_path);
                    //clientModel.sProofOfCard = PFileName;
                    if (clientModel.ClientId != null)
                    {
                        clientModel.sProofOfCard = clientModel.ClientId + "-" + PFileName;
                    }
                    else
                    {
                        clientModel.sProofOfCard = PFileName;
                    }
                }

                if (clientModel.FDrivingLicense != null)
                {
                    string DFileName = Path.GetFileName(clientModel.FDrivingLicense.FileName);
                    string path = Server.MapPath("~/documents/" + "Client-" + clientModel.ClientId + "-" + DFileName);
                    clientModel.FDrivingLicense.SaveAs(path);
                    //clientModel.sDrivingLicense = DFileName;
                    if (clientModel.ClientId !=null)
                    {
                        clientModel.sDrivingLicense = clientModel.ClientId + "-" + DFileName;
                    } 
                    else
                    {
                        clientModel.sDrivingLicense = DFileName;
                    }
                    
                }

                if (clientModel.FSocialSecCard != null)
                {
                    string SFileName = Path.GetFileName(clientModel.FSocialSecCard.FileName);
                    string path = Server.MapPath("~/documents/" + "Client-" + clientModel.ClientId + "-" + SFileName);
                    clientModel.FSocialSecCard.SaveAs(path);
                    //clientModel.sSocialSecCard = SFileName;
                    if (clientModel.ClientId != null)
                    {
                        clientModel.sSocialSecCard = clientModel.ClientId + "-" + SFileName;
                    }
                    else
                    {
                        clientModel.sSocialSecCard = SFileName;
                    }
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
        public JsonResult ClientChallengeform(List<CreditReportItems> credit, int Id, string[] values)
        {
            string date = string.Empty; bool status = false;

            try
            {
                //
                if (!string.IsNullOrEmpty(Session["AgentId"].ToString()))
                {
                    string agentid = Session["AgentId"].ToString();
                    Agent agentAdddress = cfunction.GetAgentAddressById(agentid);
                    Session["AgentAddress"] = agentAdddress;
                }

                //

                date = DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString()
                + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                List<CreditReportItems> crediteTRANSUNION = new List<CreditReportItems>();
                List<CreditReportItems> crediteEXPERIAN = new List<CreditReportItems>();
                List<CreditReportItems> crediteEQUIFAX = new List<CreditReportItems>();
                List<CreditReportFiles> filenames = new List<CreditReportFiles>();

                int count = 0;
                if (credit != null)
                {
                    count = credit.Count();
                }
                int i = 0;
                byte[] pdfBuffer = null;

                string filepath = "", filename = "";
                try
                {
                    ClientData cd = new ClientData();
                    int sno = cd.getsnofromitems(Id.ToString(), values[3]);
                    if (credit != null)
                    {

                        for (i = 0; i < count; i++)
                        {

                            cfunction.AddReportItemChallenges(credit[i], sno, Id);
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

                    string client = Id.ToString();

                    ClientModel clientModel = cfunction.GetClient(client);
                    Session["ClientAddress"] = clientModel;
                    List<string> StringList = new List<string>();
                    StringList.Add(clientModel.FirstName);
                    StringList.Add(clientModel.LastName);
                    StringList.Add(values[2]);
                    StringList.Add(values[3]);
                    StringList.Add(clientModel.SSN);

                    for (int k = 0; k < credite.Count; k++)
                    {
                        CreditReportItems cr = new CreditReportItems();

                        if (credite[k].Agency == "TRANSUNION" && credite[k].ChallengeText != "NO CHALLENGE")
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
                            crediteTRANSUNION.Add(cr);
                        }
                        if ("EXPERIAN" == credite[k].Agency && credite[k].ChallengeText != "NO CHALLENGE")
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
                            // var round = credite[k].CredRepItemsId.Split(' ');
                            crediteEXPERIAN.Add(cr);
                        }
                        if ("EQUIFAX" == credite[k].Agency && credite[k].ChallengeText != "NO CHALLENGE")
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
                            crediteEQUIFAX.Add(cr);
                        }

                    }
                    PdfManager objPdf = new PdfManager();
                    PdfDocument objDocTRANSUNION = objPdf.CreateDocument();
                    objDocTRANSUNION.Title = "CreditReversalGuru";
                    objDocTRANSUNION.Creator = "CreditReversalGuru";
                    PdfFont objFont = objDocTRANSUNION.Fonts["Helvetica"];

                    PdfDocument objDocEXPERIAN = objPdf.CreateDocument();
                    PdfDocument objDocEQUIFAX = objPdf.CreateDocument();
                    MemoryStream pdfStream = new MemoryStream();
                    filenames = new List<CreditReportFiles>();
                    if (crediteTRANSUNION.Count > 0)
                    {
                        StringList.Add("TRANSUNION");
                        Session["values"] = StringList;
                        objDocTRANSUNION = objPdf.CreateDocument();
                        dynamic model = new ExpandoObject();
                        int trcount = crediteTRANSUNION.Count - 1;
                        model.clientcredit = crediteTRANSUNION;
                        string htmlToConvert = RenderViewAsString("ClientChallengeform", model);
                        //objDocTRANSUNION.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=34;TopMargin=34;BottomMargin=34; hyperlinks=true; drawbackground=true");
                        objDocTRANSUNION.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=54;TopMargin=54;BottomMargin=54; hyperlinks=true; drawbackground=true");
                        filename = "Challenge-Account-TRANSUNION" + "-" + values[0] + "-" + values[1] + "-" + crediteTRANSUNION[trcount].RoundType + "-" + date + ".pdf";
                        filename = filename.Replace(" ", "");
                        filepath = Server.MapPath("~/Documents/Challenge/Challenge-Account-TRANSUNION" + "-" + values[0] + "-" + values[1] + "-" + crediteTRANSUNION[trcount].RoundType + "-" + date + ".pdf");
                        // filepath = Server.MapPath("~/Documents/Challenge/Challenge-TRANSUNION.pdf"); // + "-" + values[0] + "-" + values[1] + "-" + DateTime.Now.ToShortDateString() + "-" + values[3] + ".pdf");
                        filepath = filepath.Replace(" ", "");
                        objDocTRANSUNION.Save(filepath, false);
                        pdfBuffer = System.IO.File.ReadAllBytes(filepath);
                        System.IO.File.WriteAllBytes(filepath, pdfBuffer);
                        objDocTRANSUNION.Close();

                        CreditReportFiles files = new CreditReportFiles();
                        //files.RoundType = values[3];
                        files.RoundType = crediteTRANSUNION[trcount].RoundType;
                        files.CRFilename = filename;
                        files.ClientId = Id;
                        filenames.Add(files);
                    }
                    if (crediteEXPERIAN.Count > 0)
                    {
                        if (StringList.Count == 5)
                        {
                            StringList.Add("EXPERIAN");
                        }
                        else
                        {
                            StringList[5] = "";
                            StringList[5] = "EXPERIAN";
                        }

                        Session["values"] = StringList;
                        objDocEXPERIAN = objPdf.CreateDocument();
                        dynamic model = new ExpandoObject();
                        int excount = crediteEXPERIAN.Count - 1;
                        model.clientcredit = crediteEXPERIAN;
                        string htmlToConvert = RenderViewAsString("ClientChallengeform", model);
                        //objDocEXPERIAN.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=34;TopMargin=34;BottomMargin=34; hyperlinks=true; drawbackground=true");
                        objDocEXPERIAN.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=54;TopMargin=54;BottomMargin=54; hyperlinks=true; drawbackground=true");
                        filename = "Challenge-Account-EXPERIAN" + "-" + values[0] + "-" + values[1] + "-" + crediteEXPERIAN[excount].RoundType + "-" + date + ".pdf";
                        filename = filename.Replace(" ", "");
                        filepath = Server.MapPath("~/Documents/Challenge/Challenge-Account-EXPERIAN" + "-" + values[0] + "-" + values[1] + "-" + crediteEXPERIAN[excount].RoundType + "-" + date + ".pdf");
                        //filepath = Server.MapPath("~/Documents/Challenge/Challenge-EXPERIAN.pdf");
                        filepath = filepath.Replace(" ", "");
                        objDocEXPERIAN.Save(filepath, false);
                        pdfBuffer = System.IO.File.ReadAllBytes(filepath);
                        System.IO.File.WriteAllBytes(filepath, pdfBuffer);
                        objDocEXPERIAN.Close();

                        CreditReportFiles files = new CreditReportFiles();
                        //files.RoundType = values[3];
                        files.RoundType = crediteEXPERIAN[excount].RoundType;
                        files.CRFilename = filename;
                        files.ClientId = Id;
                        filenames.Add(files);
                    }
                    if (crediteEQUIFAX.Count > 0)
                    {
                        if (StringList.Count == 5)
                        {
                            StringList.Add("EQUIFAX");
                        }
                        else
                        {
                            StringList[5] = "";
                            StringList[5] = "EQUIFAX";
                        }
                        Session["values"] = StringList;
                        objDocEQUIFAX = objPdf.CreateDocument();
                        dynamic model = new ExpandoObject();
                        int eqcount = crediteEQUIFAX.Count - 1;
                        model.clientcredit = crediteEQUIFAX;
                        string htmlToConvert = RenderViewAsString("ClientChallengeform", model);

                        //objDocEQUIFAX.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=34;TopMargin=34;BottomMargin=34; hyperlinks=true; drawbackground=true");
                        objDocEQUIFAX.ImportFromUrl(htmlToConvert, "LeftMargin=54;RightMargin=54;TopMargin=54;BottomMargin=54; hyperlinks=true; drawbackground=true");

                        filename = "Challenge-Account-EQUIFAX" + "-" + values[0] + "-" + values[1] + "-" + crediteEQUIFAX[eqcount].RoundType + "-" + date + ".pdf";
                        filename = filename.Replace(" ", "");
                        filepath = Server.MapPath("~/Documents/Challenge/Challenge-Account-EQUIFAX" + "-" + values[0] + "-" + values[1] + "-" + crediteEQUIFAX[eqcount].RoundType + "-" + date + ".pdf");
                        //filepath = Server.MapPath("~/Documents/Challenge/Challenge-EQUIFAX.pdf");
                        filepath = filepath.Replace(" ", "");
                        objDocEQUIFAX.Save(filepath, false);
                        pdfBuffer = System.IO.File.ReadAllBytes(filepath);
                        System.IO.File.WriteAllBytes(filepath, pdfBuffer);
                        objDocEQUIFAX.Close();

                        CreditReportFiles files = new CreditReportFiles();
                        //files.RoundType = values[3];
                        files.RoundType = crediteEQUIFAX[eqcount].RoundType;
                        files.CRFilename = filename;
                        files.ClientId = Id;
                        filenames.Add(files);
                    }
                    if (filenames.Count > 0)
                    {
                        functions.updatechallengefilepath(filenames, sno);
                    }
                    status = true;
                }

                catch (Exception ex)
                {
                    status = true;
                }
            }
            catch (Exception ex)
            { string msg = ex.Message; }
            return Json(status);

        }
        public ActionResult ClientChallenges(string agency = null)        {            string ClientId = string.Empty;            string Agentname = string.Empty;            string AgentnameNew = string.Empty;            try            {                ClientId = sessionData.GetClientId();                Agentname = cfunction.GetAgentName(ClientId);                AgentnameNew = cfunction.getAgentName(Convert.ToInt32(ClientId));                ViewBag.name = Agentname;                ViewBag.Agentname = AgentnameNew;                ViewBag.Dasboard = sessionData.getDasboard();                ViewBag.CreditReportItems = cfunction.ReportItemChallenges(ClientId.StringToInt(0), agency);                ViewBag.CreditReportInquiresItems = cfunction.ReportItemInquiresChallenges(ClientId.StringToInt(0), agency);            }            catch (Exception ex)            {                System.Diagnostics.Debug.WriteLine(ex.Message);            }            return View();        }
        public JsonResult AddCreditReport(string clientId, string mode = "", string round = "")        {            string ReportId = "";            try            {                List<AccountHistory> accountHistories = new List<AccountHistory>();                List<Inquires> inquires = new List<Inquires>();                IdentityIQInfo IdentityIQInfo = new IdentityIQInfo();                IdentityIQInfo = IQfunction.CheckIdentityIQInfo(clientId);                CreditReportData tuple = GetCreditReportItemsbyReading(IdentityIQInfo);
                if (tuple.AccHistory.Count > 0 && tuple.inquiryDetails.Count > 0)
                {
                    accountHistories = tuple.AccHistory;
                    inquires = tuple.inquiryDetails;
                    try
                    {
                        //List<CreditReportFiles> files = cfunction.GetCreditReportsFilesByround(clientId, round);
                        //if (files.Count > 0)
                        //{
                        //    foreach (CreditReportFiles file in files)
                        //    {
                        //        // Check if file exists with its full path    
                        //        string filepath = Server.MapPath("~/documents/Challenge/" + file.CRFilename);
                        //        if (System.IO.File.Exists(filepath))
                        //        {
                        //            // If file found, delete it    
                        //            System.IO.File.Delete(filepath);
                        //        }
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    { string msg = ex.Message; }

                    ReportId = cfunction.RefreshCreditReport(accountHistories, inquires, tuple.monthlyPayStatusHistoryDetails, clientId, mode, round);
                }
                else
                {
                    return Json(0);
                }


            }            catch (Exception ex) { ex.insertTrace(""); }            return Json(ReportId);        }
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
                    CreditReportData tuple = GetCreditReportItemsbyReading(objidentity);
                    //string ds = "";
                    if (tuple.AccHistory.Count > 0 && tuple.inquiryDetails.Count > 0)
                    {
                        accountHistories = tuple.AccHistory;
                        inquires = tuple.inquiryDetails;
                        ReportId = cfunction.AddCreditReport(accountHistories, inquires, tuple.monthlyPayStatusHistoryDetails, clientId, mode);
                        return RedirectToAction("CreditItems", "Client", new { @ClientId = clientId, @from = from, @mode = mode });
                    }
                    else
                    {
                        if (role == "agentstaff")
                        {
                            TempData["from"] = "InvalidIdentityIQ";
                            return RedirectToAction("AgentStaff", "dashboard");
                        }
                        if (role == "agentadmin")
                        {
                            TempData["from"] = "InvalidIdentityIQ";
                            return RedirectToAction("Agent", "dashboard");
                        }

                    }
                }
                else
                {
                    if (role == "agentstaff")
                    {
                        TempData["from"] = "error";
                        return RedirectToAction("AgentStaff", "dashboard");
                    }
                    if (role == "agentadmin")
                    {
                        TempData["from"] = "error";
                        return RedirectToAction("Agent", "dashboard");
                    }

                }
            }
            catch (Exception ex)
            {
                if (role == "agentstaff")
                {
                    TempData["from"] = "error";
                    return RedirectToAction("AgentStaff", "dashboard");
                }
                if (role == "agentadmin")
                {
                    TempData["from"] = "error";
                    return RedirectToAction("Agent", "dashboard");
                }
            }
            string user = "Agent";
            if (role == "agentstaff")
            { user = "AgentStaff"; }
           return RedirectToAction(user, "dashboard");
        }
        public ActionResult CreditPull(string clientId, string from = "", string mode = "")        {
            string role = Session["UserRole"].ToString();
            IdentityIQInfo objidentity = new IdentityIQInfo();            try            {
                return RedirectToAction("CreditItems", "Client", new { @ClientId = clientId, @from = from, @mode = mode });            }            catch (Exception ex)
            { }

            if (role == "agentstaff")
            { return RedirectToAction("AgentStaff", "dashboard"); }
            else
            {
                return RedirectToAction("Agent", "dashboard");
            }

        }
        public ActionResult CreditPullBKUP(string clientId, string from = "", string mode = "")        {
            //bool status = false;
            string ReportId = "";            IdentityIQInfo objidentity = new IdentityIQInfo();            try            {                ClientData cd = new ClientData();
                string roundtype = cd.checkLastDateReport(clientId);                bool status = cd.checkDateReport(clientId);                bool checkclientexist = functions.GetCreditReportStatus(clientId);                if (status == true || roundtype != "")
                { }                else
                {
                    if (checkclientexist && mode == "Edit")
                    {
                        return RedirectToAction("CreditItems", "Client", new { @ClientId = clientId, @from = from, @mode = mode });
                    }
                }

                objidentity = IQfunction.CheckIdentityIQInfo(clientId);                if (objidentity.Password != null && objidentity.UserName != null && objidentity.Answer != null)                {                    List<AccountHistory> accountHistories = new List<AccountHistory>();                    List<Inquires> inquires = new List<Inquires>();                    CreditReportData tuple = GetCreditReportItemsbyReading(objidentity);
                    //string ds = "";
                    if (tuple.AccHistory.Count > 0 && tuple.inquiryDetails.Count > 0)                    {                        accountHistories = tuple.AccHistory;                        inquires = tuple.inquiryDetails;                        ReportId = cfunction.AddCreditReport(accountHistories, inquires, tuple.monthlyPayStatusHistoryDetails, clientId, mode);                        return RedirectToAction("CreditItems", "Client", new { @ClientId = clientId, @from = from, @mode = mode });                    }                    else
                    {                        TempData["from"] = "InvalidIdentityIQ";                        return RedirectToAction("Agent", "dashboard");                    }
                }                else                {                    TempData["from"] = "error";                    return RedirectToAction("Agent", "dashboard");                }            }            catch (Exception ex)
            {                TempData["from"] = "error";                return RedirectToAction("Agent", "dashboard");            }


        }

        //public Tuple<List<AccountHistory>, List<Inquires>> GetCreditReportItemsbyReading(IdentityIQInfo IdentityIQInfo)
        public CreditReportData GetCreditReportItemsbyReading(IdentityIQInfo IdentityIQInfo)
        {            List<AccountHistory> accountHistories = new List<AccountHistory>();            List<Inquires> inquires = new List<Inquires>();            CreditReportData creditReportData = new CreditReportData();

            try            {                sbrowser sb = new sbrowser();
                //string username = "georgecole622@msn.com";
                //string Password = "211665gc";
                //string SecurityAnswer = "4344";
                string username = IdentityIQInfo.UserName;                string Password = IdentityIQInfo.Password;                string SecurityAnswer = IdentityIQInfo.Answer;                CreditReport cr = sb.pullcredit(username, Password, SecurityAnswer);


                List<MonthlyPayStatus> monthlyPayStatusEQ = new List<MonthlyPayStatus>();
                List<MonthlyPayStatus> monthlyPayStatusTU = new List<MonthlyPayStatus>();
                List<MonthlyPayStatus> monthlyPayStatusEX = new List<MonthlyPayStatus>();
                monthlyPayStatusEQ = cr.monthlyPayStatusEQ;
                monthlyPayStatusEX = cr.monthlyPayStatusEX;
                monthlyPayStatusTU = cr.monthlyPayStatusTU;



                string date = "";
                string[] strr;
                string year = "";
                string month = "";
                string dat = "";
                string FormatedDate = "";
                try
                {
                    foreach (var ach in cr.TransUnion)
                    {
                        DateTime aDate = DateTime.Now;
                        AccountHistory ah = new AccountHistory();
                        ah.Bank = ach.atcreditorName;
                        ah.Account = ach.ataccountNumber;
                        ah.AccountStatus = ach.OpenClosed.atabbreviation;
                        ah.Agency = ach.atbureau;
                        ah.AccountType = ach.GrantedTrade.CreditType.atabbreviation; //AccountType
                        ah.AccountTypeDetail = ach.GrantedTrade.AccountType.atdescription; //AccountTypeDetail 
                        //Date formating
                        date = ach.atdateOpened;
                        strr = date.Split('-');
                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.DateOpened = FormatedDate;

                        double balance = Convert.ToDouble(ach.atcurrentBalance);
                        string bal = balance.DoubleToStringIfNotNull();
                        ah.Balance = "$" + bal;

                        double HighCredit = Convert.ToDouble(ach.athighBalance);
                        string HighCre = HighCredit.DoubleToStringIfNotNull();
                        ah.HighCredit = "$" + HighCre;

                        GrantedTrade gt = ach.GrantedTrade;
                        double MonthlyPayment = 0;
                        if (gt != null)
                        {
                            MonthlyPayment = Convert.ToDouble(ach.GrantedTrade.atmonthlyPayment);
                        }

                        string MonthlyPay = MonthlyPayment.DoubleToStringIfNotNull();
                        ah.MonthlyPayment = "$" + MonthlyPay;

                        date = ach.atdateReported;
                        strr = date.Split('-');
                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.LastReported = FormatedDate;
                        ah.PaymentStatus = ach.PayStatus.atabbreviation;
                        var payStatus = monthlyPayStatusTU.FirstOrDefault(x => x.AccountNo == ach.ataccountNumber);
                        ah.negativeitems = payStatus != null ? payStatus.NegitiveItemsCount : 0;
                        accountHistories.Add(ah);
                    }
                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                }
                try
                {
                    foreach (var ach in cr.Experian)
                    {
                        DateTime aDate = DateTime.Now;
                        AccountHistory ah = new AccountHistory();
                        ah.Bank = ach.atcreditorName;
                        ah.Account = ach.ataccountNumber;
                        ah.AccountStatus = ach.OpenClosed.atabbreviation;
                        ah.Agency = ach.atbureau;
                        ah.AccountType = ach.GrantedTrade.CreditType.atabbreviation; //AccountType
                        ah.AccountTypeDetail = ach.GrantedTrade.AccountType.atdescription; //AccountTypeDetail 
                        //Date formating
                        date = ach.atdateOpened;
                        strr = date.Split('-');
                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.DateOpened = FormatedDate;

                        double balance = Convert.ToDouble(ach.atcurrentBalance);
                        string bal = balance.DoubleToStringIfNotNull();
                        ah.Balance = "$" + bal;

                        double HighCredit = Convert.ToDouble(ach.athighBalance);
                        string HighCre = HighCredit.DoubleToStringIfNotNull();
                        ah.HighCredit = "$" + HighCre;

                        GrantedTrade gt = ach.GrantedTrade;
                        double MonthlyPayment = 0;
                        if (gt != null)
                        {
                            MonthlyPayment = Convert.ToDouble(ach.GrantedTrade.atmonthlyPayment);
                        }
                        string MonthlyPay = MonthlyPayment.DoubleToStringIfNotNull();
                        ah.MonthlyPayment = "$" + MonthlyPay;

                        date = ach.atdateReported;
                        strr = date.Split('-');
                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.LastReported = FormatedDate;
                        ah.PaymentStatus = ach.PayStatus.atabbreviation;
                        var payStatus = monthlyPayStatusEX.FirstOrDefault(x => x.AccountNo == ach.ataccountNumber);
                        ah.negativeitems = payStatus != null ? payStatus.NegitiveItemsCount : 0;
                        accountHistories.Add(ah);
                    }

                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                }
                try
                {
                    foreach (var ach in cr.Equifax)
                    {
                        DateTime aDate = DateTime.Now;
                        AccountHistory ah = new AccountHistory();
                        ah.Bank = ach.atcreditorName;
                        ah.Account = ach.ataccountNumber;
                        ah.AccountStatus = ach.OpenClosed.atabbreviation;
                        ah.Agency = ach.atbureau;
                        ah.AccountType = ach.GrantedTrade.CreditType.atabbreviation; //AccountType
                        ah.AccountTypeDetail = ach.GrantedTrade.AccountType.atdescription; //AccountTypeDetail 
                        //Date formating
                        date = ach.atdateOpened;
                        strr = date.Split('-');
                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.DateOpened = FormatedDate;

                        double balance = Convert.ToDouble(ach.atcurrentBalance);
                        string bal = balance.DoubleToStringIfNotNull();
                        ah.Balance = "$" + bal;

                        double HighCredit = Convert.ToDouble(ach.athighBalance);
                        string HighCre = HighCredit.DoubleToStringIfNotNull();
                        ah.HighCredit = "$" + HighCre;

                        GrantedTrade gt = ach.GrantedTrade;
                        double MonthlyPayment = 0;
                        if (gt != null)
                        {
                            MonthlyPayment = Convert.ToDouble(ach.GrantedTrade.atmonthlyPayment);
                        }
                        string MonthlyPay = MonthlyPayment.DoubleToStringIfNotNull();
                        ah.MonthlyPayment = "$" + MonthlyPay;

                        date = ach.atdateReported;
                        strr = date.Split('-');

                        year = strr[0];
                        month = strr[1];
                        dat = strr[2];
                        FormatedDate = month + "/" + dat + "/" + year;
                        ah.LastReported = FormatedDate;
                        ah.PaymentStatus = ach.PayStatus.atabbreviation;
                        var payStatus = monthlyPayStatusEQ.FirstOrDefault(x => x.AccountNo == ach.ataccountNumber);
                        ah.negativeitems = payStatus != null ? payStatus.NegitiveItemsCount : 0;
                        accountHistories.Add(ah);
                    }
                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                }
                try
                {
                    foreach (var ach in cr.inquiries)
                    {
                        Inquires inquires1 = new Inquires();
                        inquires1.CreditBureau = ach.Inquiry.atbureau;
                        inquires1.CreditorName = ach.Inquiry.atsubscriberName;
                        inquires1.Dateofinquiry = ach.Inquiry.atinquiryDate;
                        inquires1.TypeofBusiness = ach.Inquiry.IndustryCode.atabbreviation;
                        inquires.Add(inquires1);
                    }
                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                }


                creditReportData.AccHistory = accountHistories;
                creditReportData.inquiryDetails = inquires;
                creditReportData.monthlyPayStatusHistoryDetails = cr.monthlyPayStatusHistoryList;
            }            catch (Exception ex)            {
                string msg = ex.Message;
            }            return creditReportData;        }
        public JsonResult ClientChallengeInquiresform(List<Inquires> credit, int Id, string[] values)        {            string date = string.Empty;            date = DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString()                + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();            List<Inquires> InquiresTRANSUNION = new List<Inquires>();            List<Inquires> InquiresEXPERIAN = new List<Inquires>();            List<Inquires> InquiresEQUIFAX = new List<Inquires>();            List<CreditReportFiles> filenames = new List<CreditReportFiles>();            bool status = false;            int count = 0;            if (credit != null)            {                count = credit.Count();            }            int i = 0;            byte[] pdfBuffer = null;            PdfManager objPdf = new PdfManager();            PdfDocument objDocTRANSUNION = objPdf.CreateDocument();            PdfDocument objDocEXPERIAN = objPdf.CreateDocument();            PdfDocument objDocEQUIFAX = objPdf.CreateDocument();            MemoryStream pdfStream = new MemoryStream();            string filepath = "", filename = "";            try            {
                ClientData cd = new ClientData();
                int sno = cd.getsnofromitems(Id.ToString(), values[3]);                if (credit != null)                {                    for (i = 0; i < count; i++)                    {                        cfunction.AddReportItemInquiriesChallenges(credit[i], values[3], sno, Id);                        dynamic model = new ExpandoObject();                        string AgentId = sessionData.GetAgentId();                        string staffId = sessionData.GetStaffId();                        string fullname = "";                        cfunction.AddInquiresChallenge(credit[i], AgentId, staffId);                        ViewBag.CreditReportItems = credit[i];                        model.clientcredit = credit[i];                        if (Session["Name"] != null)                        {                            fullname = Session["Name"].ToString();                        }                        var names = fullname.Split(' ');                        string name = names[0];                        int clientid = Id;                        string Report = credit[i].CreditInqId;                    }                }                List<Inquires> Inquires = functions.GetInquiriesChallengesAgentById(credit, Id);                string client = Id.ToString();                ClientModel clientModel = cfunction.GetClient(client);                List<string> StringList = new List<string>();                StringList.Add(clientModel.FirstName);                StringList.Add(clientModel.LastName);                StringList.Add(values[2]);                StringList.Add(values[3]);                StringList.Add(clientModel.SSN);                for (int k = 0; k < Inquires.Count; k++)                {                    Inquires cr = new Inquires();                    if (Inquires[k].CreditBureau == "TransUnion" && Inquires[k].ChallengeText != "NO CHALLENGE")                    {                        cr.CreditorName = Inquires[k].CreditorName;                        cr.TypeofBusiness = Inquires[k].TypeofBusiness;                        cr.Dateofinquiry = Inquires[k].Dateofinquiry;                        cr.CreditBureau = Inquires[k].CreditBureau;                        cr.ChallengeText = Inquires[k].ChallengeText;                        cr.RoundType = Inquires[k].RoundType;                        InquiresTRANSUNION.Add(cr);                    }                    if ("Experian" == Inquires[k].CreditBureau && Inquires[k].ChallengeText != "NO CHALLENGE")                    {                        cr.CreditorName = Inquires[k].CreditorName;                        cr.TypeofBusiness = Inquires[k].TypeofBusiness;                        cr.Dateofinquiry = Inquires[k].Dateofinquiry;                        cr.CreditBureau = Inquires[k].CreditBureau;                        cr.ChallengeText = Inquires[k].ChallengeText;
                        cr.RoundType = Inquires[k].RoundType;                        InquiresEXPERIAN.Add(cr);                    }                    if ("Equifax" == Inquires[k].CreditBureau && Inquires[k].ChallengeText != "NO CHALLENGE")                    {                        cr.CreditorName = Inquires[k].CreditorName;                        cr.TypeofBusiness = Inquires[k].TypeofBusiness;                        cr.Dateofinquiry = Inquires[k].Dateofinquiry;                        cr.CreditBureau = Inquires[k].CreditBureau;                        cr.ChallengeText = Inquires[k].ChallengeText;
                        cr.RoundType = Inquires[k].RoundType;                        InquiresEQUIFAX.Add(cr);                    }                }                filenames = new List<CreditReportFiles>();                if (InquiresTRANSUNION.Count > 0)                {                    StringList.Add("TRANSUNION");                    Session["values"] = StringList;                    objDocTRANSUNION = objPdf.CreateDocument();                    dynamic model = new ExpandoObject();                    int trcount = InquiresTRANSUNION.Count - 1;                    model.clientcredit = InquiresTRANSUNION;                    string htmlToConvert = RenderViewAsString("ClientInquiriesChallengeform", model);                    objDocTRANSUNION.ImportFromUrl(htmlToConvert, "scale=0.8; hyperlinks=true; drawbackground=true");                    filename = "Challenge-Inquires-TRANSUNION" + "-" + values[0] + "-" + values[1] + "-" + InquiresTRANSUNION[trcount].RoundType + "-" + date + ".pdf";
                    filename = filename.Replace(" ", "");                    filepath = Server.MapPath("~/Documents/Challenge/Challenge-Inquires-TRANSUNION" + "-" + values[0] + "-" + values[1] + "-" + InquiresTRANSUNION[trcount].RoundType + "-" + date + ".pdf");
                    // filepath = Server.MapPath("~/Documents/Challenge/Challenge-TRANSUNION.pdf"); // + "-" + values[0] + "-" + values[1] + "-" + DateTime.Now.ToShortDateString() + "-" + values[3] + ".pdf");
                    filepath = filepath.Replace(" ", "");                    objDocTRANSUNION.Save(filepath, false);                    pdfBuffer = System.IO.File.ReadAllBytes(filepath);                    System.IO.File.WriteAllBytes(filepath, pdfBuffer);                    objDocTRANSUNION.Close();                    CreditReportFiles files = new CreditReportFiles();                    files.RoundType = InquiresTRANSUNION[trcount].RoundType;                    files.CRFilename = filename;                    files.ClientId = Id;                    filenames.Add(files);                }                if (InquiresEXPERIAN.Count > 0)                {                    if (StringList.Count == 5)                    {                        StringList.Add("EXPERIAN");                    }                    else                    {                        StringList[5] = "";                        StringList[5] = "EXPERIAN";                    }                    Session["values"] = StringList;                    objDocEXPERIAN = objPdf.CreateDocument();                    dynamic model = new ExpandoObject();                    int excount = InquiresEXPERIAN.Count - 1;                    model.clientcredit = InquiresEXPERIAN;                    string htmlToConvert = RenderViewAsString("ClientInquiriesChallengeform", model);                    objDocEXPERIAN.ImportFromUrl(htmlToConvert, "scale=0.8; hyperlinks=true; drawbackground=true");                    filename = "Challenge-Inquires-EXPERIAN" + "-" + values[0] + "-" + values[1] + "-" + InquiresEXPERIAN[excount].RoundType + "-" + date + ".pdf";
                    filename = filename.Replace(" ", "");                    filepath = Server.MapPath("~/documents/Challenge/Challenge-Inquires-EXPERIAN" + "-" + values[0] + "-" + values[1] + "-" + InquiresEXPERIAN[excount].RoundType + "-" + date + ".pdf");
                    //filepath = Server.MapPath("~/Documents/Challenge/Challenge-EXPERIAN.pdf");
                    filepath = filepath.Replace(" ", "");                    objDocEXPERIAN.Save(filepath, false);                    pdfBuffer = System.IO.File.ReadAllBytes(filepath);                    System.IO.File.WriteAllBytes(filepath, pdfBuffer);                    objDocEXPERIAN.Close();                    CreditReportFiles files = new CreditReportFiles();                    files.RoundType = InquiresEXPERIAN[excount].RoundType;                    files.CRFilename = filename;                    files.ClientId = Id;                    filenames.Add(files);                }                if (InquiresEQUIFAX.Count > 0)                {                    if (StringList.Count == 4)                    {                        StringList.Add("EQUIFAX");                    }                    else                    {                        StringList[5] = "";                        StringList[5] = "EQUIFAX";                    }                    Session["values"] = StringList;                    objDocEQUIFAX = objPdf.CreateDocument();                    dynamic model = new ExpandoObject();                    int eqcount = InquiresEQUIFAX.Count - 1;                    model.clientcredit = InquiresEQUIFAX;                    string htmlToConvert = RenderViewAsString("ClientInquiriesChallengeform", model);                    objDocEQUIFAX.ImportFromUrl(htmlToConvert, "scale=0.8; hyperlinks=true; drawbackground=true");                    filename = "Challenge-Inquires-EQUIFAX" + "-" + values[0] + "-" + values[1] + "-" + InquiresEQUIFAX[eqcount].RoundType + "-" + date + ".pdf";
                    filename = filename.Replace(" ", "");                    filepath = Server.MapPath("~/documents/Challenge/Challenge-Inquires-EQUIFAX" + "-" + values[0] + "-" + values[1] + "-" + InquiresEQUIFAX[eqcount].RoundType + "-" + date + ".pdf");
                    //filepath = Server.MapPath("~/Documents/Challenge/Challenge-EQUIFAX.pdf");
                    filepath = filepath.Replace(" ", "");                    objDocEQUIFAX.Save(filepath, false);                    pdfBuffer = System.IO.File.ReadAllBytes(filepath);                    System.IO.File.WriteAllBytes(filepath, pdfBuffer);                    objDocEQUIFAX.Close();                    CreditReportFiles files = new CreditReportFiles();                    files.RoundType = InquiresEQUIFAX[eqcount].RoundType;                    files.CRFilename = filename;                    files.ClientId = Id;                    filenames.Add(files);                }                if (filenames.Count > 0)                {                    functions.updatechallengefilepath(filenames, sno);                }                status = true;            }            catch (Exception ex)            {                status = true;            }            return Json(status);        }
    }
}
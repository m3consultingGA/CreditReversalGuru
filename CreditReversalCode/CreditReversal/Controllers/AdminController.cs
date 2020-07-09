using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using CreditReversal.Models;
using CreditReversal.DAL;
using CreditReversal.BLL;
using CreditReversal.Utilities;

namespace CreditReversal.Controllers
{
    [Authorization]
    public class AdminController : Controller
    {
        AdminFunction objAdminfunction = new AdminFunction();
        SessionData objSData = new SessionData();


        public SessionData sessionData = new SessionData();
        IdentityIQFunction IQfunction = new IdentityIQFunction();
        Common objCommon = new Common();
        ClientFunction cfunction = new ClientFunction();
        DashboardFunctions functions = new DashboardFunctions();
        ClientData CData = new ClientData();

        int res = 0;
        string strCType = string.Empty;
        bool result = false;

        // GET: Admin
        public ActionResult Index()
        {
            try
            {
                ViewBag.Dasboard = objSData.getDasboard();
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return View();
        }

        #region Display 
        [HttpGet]
        public ActionResult AddCompanyTypes()
        {
            List<CompanyTypes> objCTList = new List<CompanyTypes>();
            try
            {
                ViewBag.Dasboard = objSData.getDasboard();
                objCTList = objAdminfunction.GetCompanyType();
                ViewBag.CTList = objCTList;

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return View();
        }
        #endregion

        #region Add CompanyType
        [HttpPost]
        public JsonResult AddCompanyTypes(CompanyTypes objCompTypes)
        {
            try
            {
                res = objAdminfunction.InsertCompanyType(objCompTypes);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(res);
        }
        #endregion

        #region Check CompanyType
        public JsonResult CheckCompanyType(string CompanyType)
        {
            try
            {
                strCType = CompanyType;
                result = objAdminfunction.CheckCompanyType(strCType);

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(result);
        }
        #endregion        

        #region Update
        public JsonResult UpdateCompanyTypes(CompanyTypes objCompTypes)
        {

            try
            {
                res = objAdminfunction.UpdateCompanyType(objCompTypes);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(res);

        }
        #endregion

        #region Edit
        public JsonResult CompanyTypeEditById(string CTId)
        {
            List<CompanyTypes> objCTypes = new List<CompanyTypes>();
            try
            {
                objCTypes = objAdminfunction.GetCompanyTypeEditById(CTId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(objCTypes);
        }
        #endregion

        public JsonResult DeleteCompanyTypeById(string CompanyTypeId)
        {
            try
            {
                res = objAdminfunction.DeleteCompanyType(CompanyTypeId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(res);
        }

        public ActionResult Billing()        {            try            {                List<Agent> agent = objAdminfunction.GetBilling();                ViewBag.Agent = agent;            }            catch (Exception ex) { ex.insertTrace(""); }            return View();        }


        #region Letters

        // GET: Letters
        public ActionResult Letter()
        {
            // LetterTemplate letter = new LetterTemplate();
            try
            {
                List<LetterTemplate> letter = new List<LetterTemplate>();
                letter = objAdminfunction.GetLetterTemplate();
                ViewBag.letter = letter;
                ViewBag.Dasboard = objSData.getDasboard();
            }

            catch (Exception ex) { ex.insertTrace(""); }
            return View();
        }
        public JsonResult EditLetter(int letterId)
        {
            LetterTemplate letter = new LetterTemplate();
            try
            {
                letter = objAdminfunction.EditLetter(letterId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(letter);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddLetter(LetterTemplate letter)
        {
            bool status = false;
            try
            {
                status = objAdminfunction.AddLetter(letter);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(status);
        }

        public JsonResult DeleteLetter(int letterId)
        {
            bool status = false;
            try
            {
                status = objAdminfunction.DeleteLetter(letterId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(status);
        }
        #endregion


        #region Pricing

        [HttpGet]
        public ActionResult Pricing()
        {
            List<Pricing> pricings = new List<Pricing>();
            try
            {
                pricings = objAdminfunction.GetPricings();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

            }
            return View(pricings);
        }
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(Pricing pricing)
        {
            string ImageName = "";
            string physicalPath = "";
            bool status = false;
            try
            {
                if (pricing.Logo != null)
                {
                    ImageName = Path.GetFileName(pricing.Logo.FileName);
                    physicalPath = Server.MapPath("~/documents/pricing/" + pricing.PricingType + "-" + ImageName);
                    pricing.Logo.SaveAs(physicalPath);
                    pricing.LogoText = pricing.PricingType + "-" + ImageName;
                }
                status = objAdminfunction.AddPricing(pricing, false);
                if (status)
                {
                    Response.Redirect("/Admin/Pricing");
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return View();
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            List<Pricing> pricings = new List<Pricing>();
            try
            {
                pricings = objAdminfunction.GetPricings(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

            }
            return View(pricings.FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Edit(Pricing pricing)
        {
            string ImageName = "";
            string physicalPath = "";
            bool status = false;
            try
            {
                if (pricing.Logo != null)
                {
                    ImageName = Path.GetFileName(pricing.Logo.FileName);
                    physicalPath = Server.MapPath("~/documents/pricing/" + pricing.PricingType + "-" + ImageName);
                    pricing.Logo.SaveAs(physicalPath);
                    pricing.LogoText = pricing.PricingType + "-" + ImageName;
                }

                status = objAdminfunction.AddPricing(pricing, true);
                if (status)
                {
                    return RedirectToAction("Pricing", "Admin");
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return View();
        }
        public void Delete(int id)
        {
            bool status;
            try
            {
                status = objAdminfunction.Delete(id);
                if (status)
                {
                    Response.Redirect("/Admin/Pricing");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);

            }


        }
        public JsonResult GetTypeStatus(string PricingType = "")
        {
            bool status = false;
            try
            {
                if (PricingType != "")
                {
                    status = objAdminfunction.GetTypeStatus(PricingType);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return Json(status);
        }
        #endregion

        #region Challenge
        // GET: Challenge
        public ActionResult Challenge()
        {
            List<Challenge> challange = new List<Challenge>();
            List<AccountTypes> accountType = new List<AccountTypes>();

            try
            {

                challange = objAdminfunction.Getchallange();
                accountType = objAdminfunction.GetAccountTypes();
                ViewBag.challange = challange;
                ViewBag.accTypes = accountType;
                ViewBag.Dasboard = objSData.getDasboard();

            }
            catch (Exception ex) { ex.insertTrace(""); }

            return View();
        }

        public JsonResult AddChallenge(Challenge challange)
        {
            bool status = false;
            try
            {
                status = objAdminfunction.AddChallenge(challange);
            }
            catch (Exception ex) { ex.insertTrace(""); }


            return Json(1);
        }

        public JsonResult EditChallenge(string ChallengeId)
        {
            Challenge challenge = new Challenge();
            try
            {
                challenge = objAdminfunction.EditChallenge(ChallengeId);
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return Json(challenge);
        }

        public JsonResult DeleteChallenge(string ChallengeId)
        {
            bool status = false;
            try
            {
                status = objAdminfunction.DeleteChallenge(ChallengeId);
            }
            catch (Exception ex) { ex.insertTrace(""); }

            return Json(status);
        }
        #endregion


        public ActionResult ServiceSettings()        {            try            {
                ViewBag.Dasboard = objSData.getDasboard();            }            catch (Exception ex) { ex.insertTrace(""); }            return View();        }

        [HttpPost]
        public JsonResult AddServiceSettings(ServiceSettings servicesettings)        {            try            {
                res = objAdminfunction.InsertServiceSettings(servicesettings);            }            catch (Exception ex) { ex.insertTrace(""); }            return Json(res);        }


        [HttpPost]
        public JsonResult GetServiceSettings()        {            ServiceSettings res = new ServiceSettings();            try            {
                res = objAdminfunction.GetServiceSettings();            }            catch (Exception ex) { ex.insertTrace(""); }            return Json(res);        }


        [HttpGet]
        public ActionResult CreateInvestor(string InvestorId = "", string Mode = "", string from = "")
        {
            TempData["from"] = from;
            string staffid = sessionData.GetStaffId();
            int Id = Convert.ToInt32(staffid);

            ViewBag.staffId = Id;
            Investor InvestorModel = new Investor();
            try
            {
                ViewBag.Dasboard = sessionData.getDasboard();

                if (InvestorId != "0")
                {
                    if (Mode == "Edit")
                    {
                        InvestorModel = objAdminfunction.GetInvestor(InvestorId);
                        InvestorModel.InvestorId = InvestorId.StringToInt(0);
                    }
                }

            }
            catch (Exception ex) { ex.insertTrace(""); }
            return View(InvestorModel);
        }

        //Createinvestor
        [HttpPost]
        public ActionResult CreateInvestor(Investor InvestorModel)
        {
            Investor Investor = new Investor();
            string role = sessionData.GetUserRole();
            string from = TempData["from"] != null ? TempData["from"].ToString() : string.Empty;
            long res = 0;
            try
            {

                if (InvestorModel.FProofOfCard != null)
                {
                    string PFileName = Path.GetFileName(InvestorModel.FProofOfCard.FileName);
                    string _path = Server.MapPath("~/documents/" + "Investor-" + InvestorModel.InvestorId + "-" + PFileName);
                    InvestorModel.FProofOfCard.SaveAs(_path);

                    if (InvestorModel.InvestorId != null)
                    {
                        InvestorModel.sProofOfCard = InvestorModel.InvestorId + "-" + PFileName;
                    }
                    else
                    {
                        InvestorModel.sProofOfCard = PFileName;
                    }
                }

                if (InvestorModel.FDrivingLicense != null)
                {
                    string DFileName = Path.GetFileName(InvestorModel.FDrivingLicense.FileName);
                    string path = Server.MapPath("~/documents/" + "Investor-" + InvestorModel.InvestorId + "-" + DFileName);
                    InvestorModel.FDrivingLicense.SaveAs(path);

                    if (InvestorModel.InvestorId != null)
                    {
                        InvestorModel.sDrivingLicense = InvestorModel.InvestorId + "-" + DFileName;
                    }
                    else
                    {
                        InvestorModel.sDrivingLicense = DFileName;
                    }
                }

                if (InvestorModel.FSocialSecCard != null)
                {
                    string SFileName = Path.GetFileName(InvestorModel.FSocialSecCard.FileName);
                    string path = Server.MapPath("~/documents/" + "Investor-" + InvestorModel.InvestorId + "-" + SFileName);
                    InvestorModel.FSocialSecCard.SaveAs(path);
                    if (InvestorModel.InvestorId != null)
                    {
                        InvestorModel.sSocialSecCard = InvestorModel.InvestorId + "-" + SFileName;
                    }
                    else
                    {
                        InvestorModel.sSocialSecCard = SFileName;
                    }


                }



                InvestorModel.Status = "1";
                InvestorModel.CreatedBy = sessionData.GetUserID().StringToInt(0);
                InvestorModel.UserRole = "investor";
                InvestorModel.CreatedDate = System.DateTime.Now.ToShortDateString();
                res = objAdminfunction.CreateInvestor(InvestorModel);
                if (res > 0)
                {
                    if (role == "admin")
                    {
                        return RedirectToAction("Investors", "Admin");
                    }
                }


            }
            catch (Exception ex)
            {
                ex.insertTrace("");
            }
            return View(InvestorModel);
        }


        public ActionResult Investors()        {            try            {
                ViewBag.Dasboard = objSData.getDasboard();
                ViewBag.Investors = objAdminfunction.GetInvestors();            }            catch (Exception ex) { ex.insertTrace(""); }            return View();        }

        [HttpPost]        public JsonResult DeleteInvestor(string InvestorId)        {            long res = 0;            try            {
                res = objAdminfunction.DeleteInvestor(InvestorId);            }            catch (Exception ex)            {            }            return Json(res);        }
        /// <summary>
        /// Account Type
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountType()
        {
            List<AccountTypes> acctTypes = new List<AccountTypes>();
            try
            {
                acctTypes = objAdminfunction.GetAccountTypes();
                ViewBag.acctTypes = acctTypes;
                ViewBag.Dashboard = objSData.getDasboard();

            }
            catch (Exception ex) { ex.insertTrace(""); }

            return View();
        }

        [HttpPost]
        public JsonResult AddAccountType(AccountTypes ACTypes)
        {            try            {
                res = objAdminfunction.InsertAccountTypes(ACTypes);            }            catch (Exception ex) { ex.insertTrace(""); }            return Json(res);


        }

        #region Account Edit
        public JsonResult AccountTypeEditById(string ATId)
        {
            List<AccountTypes> objATypes = new List<AccountTypes>();
            try
            {
                objATypes = objAdminfunction.GetAccountTypeEditById(ATId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(objATypes);
        }
        #endregion

        #region Update Account Type
        [HttpPost]
        public JsonResult UpdateAccountTypes(AccountTypes objATypes)
        {
            try
            {
                res = objAdminfunction.UpdateAccountType(objATypes);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(res);

        }
        #endregion

        #region Delete Account Type
        [HttpPost]
        public JsonResult DeleteAccountTypesById(string AccTypeId)
        {
            try
            {
                res = objAdminfunction.DeleteAccountType(AccTypeId);
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return Json(res);

        }
        #endregion

        #region Challenge Orders
        public ActionResult ChallengeOrders()
        {
            List<ChallengeOrders> corders = new List<ChallengeOrders>();
            try
            {
                corders = objAdminfunction.GetChallengeOrders();
                ViewBag.COrders = corders;
                ViewBag.Dashboard = objSData.getDasboard();

            }
            catch (Exception ex) { ex.insertTrace(""); }

            return View();
        }
        #endregion
    }
}
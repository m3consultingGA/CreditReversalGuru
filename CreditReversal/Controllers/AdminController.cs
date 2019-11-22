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

            try
            {

                challange = objAdminfunction.Getchallange();
                ViewBag.challange = challange;
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBrowser;
using SimpleBrowser.Elements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using CreditReversal.Models;
using CreditReversal.BLL;

namespace CreditReversal.Controllers
{
    public class SimpleController : Controller
    {
        // GET: Simple
        public ActionResult Index()
        {
            ViewBag.transunion =TempData["transunion"];
            ViewBag.equifax = TempData["equifax"];
            ViewBag.experian = TempData["experian"];
            ViewBag.InquiryPartition = TempData["InquiryPartition"];

            return View();
        }
        public ActionResult pullcredit()
        {
            //Browser browser = new Browser();

            //// we'll fake the user agent for websites that alter their content for unrecognised browsers
            //browser.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/534.10";

            //// browse to GitHub
            //browser.Navigate("https://www.identityiq.com/login.aspx");
            //browser.Find("txtUsername").Value = "georgecole622@msn.com";
            //browser.Find("txtPassword").Value = "211665gc";
            ////  browser.Find(ElementType.Button, "name", "imgBtnLogin").Click();
            //browser.Find("imgBtnLogin").Click();
            //var html = browser.CurrentHtml;

            //browser.Navigate("https://www.identityiq.com/SecurityQuestions.aspx");
            //browser.Find("FBfbforcechangesecurityanswer_txtSecurityAnswer").Value = "4344";
            //browser.Find("FBfbforcechangesecurityanswer_ibtSubmit").Click();
            //var html1 = browser.CurrentHtml;

            //browser.Navigate("https://www.identityiq.com/CreditReport.aspx");
            //// browser.Find("imgDownloadAction").Click();
            ////browser.Find("a", new { @class="imgDownloadAction" }).Click();
            //// browser.Find("ucCreditReport_ImgSecureAlacarte").Click();

            //var val = browser.Find("div", FindBy.Id, "divReprtOuter");

            //var html3 = browser.CurrentHtml;

            //browser.Navigate("https://www.identityiq.com/CreditReport.aspx");
            //var html4 = browser.CurrentHtml;

            //var value = browser.Find("input", new { id = "reportUrl" }).Value;
            //var prevValue = value;
            //value = prevValue;
            //// value = value.Replace("&xsl=CC2IDENTITYIQ_GENERIC_JSON", " ");
            //browser.Navigate(value);
            //var html2 = browser.CurrentHtml;
            ////    var list = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(html2);
            //// var items = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<report>>(html2);
            //html2 = html2.Replace("JSON_CALLBACK(", "");
            //html2 = html2.TrimEnd(')', ' ');
            //html2 = html2.Replace("$", "dollar");
            //html2 = html2.Replace("@", "at");
            //html2 = html2.Replace("{ \"", " { ");
            //html2 = html2.Replace("\" :", " : ");
            //html2 = html2.Replace(", \"", " , ");
            //var data = JsonConvert.DeserializeObject<dynamic>(html2);
            //browser.Close();
            //RootObject rootObj = JsonConvert.DeserializeObject<RootObject>(html2);
            ////TU  EQ  EXP
            //List<TradeLinePartition> tdl = new List<TradeLinePartition>();
            //List<InquiryPartition> InquiryPartition = new List<InquiryPartition>();
            //List<TradeLine> trdlines = new List<TradeLine>();
            //List<TradeLine> transunion = new List<TradeLine>();
            //List<TradeLine> equifax = new List<TradeLine>();
            //List<TradeLine>  experian= new List<TradeLine>();


            //tdl.AddRange(rootObj.BundleComponents.BundleComponent[6].TrueLinkCreditReportType.TradeLinePartition);
            //InquiryPartition.AddRange(rootObj.BundleComponents.BundleComponent[6].TrueLinkCreditReportType.InquiryPartition);
            //for (int i = 0; i < tdl.Count; i++)
            //{
            //    string str = tdl[i].Tradeline.ToString();
            //    string name = string.Empty;
            //    if (str.Substring(0,1).Contains("["))
            //    {
            //        name = "{ TradeLine: " + str + " }";
            //    }
            //    else
            //    {
            //        name = "{ TradeLine: [ " + str + " ] }";
            //    }

            //    //str = str.Replace("[{", "TradeLine: [ {");
            //    try
            //    {
            //        ChildModel td = JsonConvert.DeserializeObject<ChildModel>(name);
            //        //List<TradeLine> td1 = JsonConvert.DeserializeObject<List<TradeLine>>(name);
            //        //List<TradeLine> trdlines = new List<TradeLine>();
            //        trdlines.AddRange(td.TradeLine);


            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }


            //}

            //if (trdlines.Count>0)
            //{
            //    transunion = new List<TradeLine>();
            //    equifax = new List<TradeLine>();
            //    experian = new List<TradeLine>();
            //    var trcount = trdlines.Where(x => x.atbureau == "TransUnion");
            //    var Equifaxcount = trdlines.Where(x => x.atbureau == "Equifax");
            //    var Experiancount = trdlines.Where(x => x.atbureau == "Experian");
            //    if (trcount.Count()>0)
            //    {
            //        transunion.AddRange(trcount);
            //    }
            //    if (Equifaxcount.Count() > 0)
            //    {
            //        equifax.AddRange(Equifaxcount);
            //    }
            //    if (Experiancount.Count() > 0)
            //    {
            //        experian.AddRange(Experiancount);
            //    }

            //    TempData["transunion"] = transunion;
            //    TempData["equifax"] = equifax;
            //    TempData["experian"] = experian;
            //    TempData["InquiryPartition"] = InquiryPartition;



            //}

            ////var resultObjects = AllChildren(JObject.Parse(html2));
            ////int count = resultObjects.Count();
            ////resultObjects = resultObjects.Where(c => c.Type == JTokenType.Array && c.Path.Contains("TradeLine"));
            ////count = resultObjects.Count();




            ////resultObjects = resultObjects.Where(x => x.Contains("atsubscriberCode")).ToList();

            ////resultObjects = resultObjects.First(c => c.Type == JTokenType.Array && c.Path.Contains("Tradeline")).Children<JObject>();


            ////foreach (JObject result in resultObjects)
            ////{
            ////    foreach (JProperty property in result.Properties())
            ////    {
            ////        // do something with the property belonging to result
            ////    }
            ////}

            sbrowser sb = new sbrowser();
           // sb.pullcredit();
            return RedirectToAction("Index");

        }
    }
}
using Newtonsoft.Json;
using SimpleBrowser;
using CreditReversal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CreditReversal.Utilities;
using CreditReversal.DAL;
using System.Data.SqlClient;

namespace CreditReversal.BLL
{
    public class sbrowser
    {
        
        public CreditReport pullcredit(string Username, string Password, string SecurityAnswer, string clientid)
        {
            CreditReport cr = new CreditReport();
            try
            {
                Browser browser = new Browser();

                // we'll fake the user agent for websites that alter their content for unrecognised browsers
                browser.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/534.10";

                // browse to GitHub
                browser.Navigate("https://www.identityiq.com/login.aspx");
                browser.Find("txtUsername").Value = Username; // "georgecole622@msn.com";
                browser.Find("txtPassword").Value = Password; // "211665gc";
                //  browser.Find(ElementType.Button, "name", "imgBtnLogin").Click();
                browser.Find("imgBtnLogin").Click();
                var html = browser.CurrentHtml;

                browser.Navigate("https://www.identityiq.com/SecurityQuestions.aspx");
                browser.Find("FBfbforcechangesecurityanswer_txtSecurityAnswer").Value = SecurityAnswer; // "4344";
                browser.Find("FBfbforcechangesecurityanswer_ibtSubmit").Click();
                var html1 = browser.CurrentHtml;
                browser.Navigate("https://www.identityiq.com/CreditReport.aspx");
                var val = browser.Find("div", FindBy.Id, "divReprtOuter");
                var html3 = browser.CurrentHtml;
                browser.Navigate("https://www.identityiq.com/CreditReport.aspx");
                var html4 = browser.CurrentHtml;
                var value = browser.Find("input", new { id = "reportUrl" }).Value;
                var prevValue = value;
                value = prevValue;
                browser.Navigate(value);
                var html2 = browser.CurrentHtml;
                html2 = html2.Replace("JSON_CALLBACK(", "");
                html2 = html2.TrimEnd(')', ' ');
                html2 = html2.Replace("$", "dollar");
                html2 = html2.Replace("@", "at");
                html2 = html2.Replace("{ \"", " { ");
                html2 = html2.Replace("\" :", " : ");
                html2 = html2.Replace(", \"", " , ");
                var data = JsonConvert.DeserializeObject<dynamic>(html2);
                browser.Close();

                //need to comment
                //string html2 = System.IO.File.ReadAllText(@"E:\\CreditReportData-Sep142020.txt");
                //html2 = html2.Replace("JSON_CALLBACK(", "");
                //html2 = html2.TrimEnd(')', ' ');
                //html2 = html2.Replace("$", "dollar");
                //html2 = html2.Replace("@", "at");
                //html2 = html2.Replace("{ \"", " { ");
                //html2 = html2.Replace("\" :", " : ");
                //html2 = html2.Replace(", \"", " , ");

                DBUtilities utilities = new DBUtilities();
                SessionData sessionData = new SessionData();
                try
                {
                    string jsondata = html2;
                    jsondata = jsondata.Replace("'", "\"");
                    string sql = "Insert into CreditReportData(ClientId,AgentId,JsonData) "
                    + " Values(@ClientId,@AgentId,@JsonData)";
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@ClientId", clientid);
                    cmd.Parameters.AddWithValue("@AgentId", sessionData.GetUserID());
                    cmd.Parameters.AddWithValue("@JsonData", jsondata);
                    utilities.ExecuteInsertCommand(cmd, true);
                }
                catch (Exception)
                { }

                RootObject rootObj = JsonConvert.DeserializeObject<RootObject>(html2);
                //TU  EQ  EXP
                List<TradeLinePartition> tdl = new List<TradeLinePartition>();
                List<InquiryPartition> InquiryPartition = new List<InquiryPartition>();
                List<TradeLine> trdlines = new List<TradeLine>();
                List<TradeLine> transunion = new List<TradeLine>();
                List<TradeLine> equifax = new List<TradeLine>();
                List<TradeLine> experian = new List<TradeLine>();
                List<PublicRecord> PublicRecords = new List<PublicRecord>();

                try
                {
                    tdl.AddRange(rootObj.BundleComponents.BundleComponent[6].TrueLinkCreditReportType.TradeLinePartition);
                }
                catch (Exception)
                {}

                try
                {
                    InquiryPartition.AddRange(rootObj.BundleComponents.BundleComponent[6].TrueLinkCreditReportType.InquiryPartition);
                }
                catch (Exception)
                {}

                try
                {
                    PublicRecords.AddRange(rootObj.BundleComponents.BundleComponent[6].TrueLinkCreditReportType.PulblicRecordPartition.PublicRecord);
                }
                catch (Exception)
                {}
                for (int i = 0; i < tdl.Count; i++)
                {
                    string str = tdl[i].Tradeline.ToString();
                    string name = string.Empty;
                    if (str.Substring(0, 1).Contains("["))
                    {
                        name = "{ TradeLine: " + str + " }";
                    }
                    else
                    {
                        name = "{ TradeLine: [ " + str + " ] }";
                    }

                    //str = str.Replace("[{", "TradeLine: [ {");
                    try
                    {
                        ChildModel td = JsonConvert.DeserializeObject<ChildModel>(name);
                        trdlines.AddRange(td.TradeLine);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                if (trdlines.Count > 0)
                {

                    transunion = new List<TradeLine>();
                    equifax = new List<TradeLine>();
                    experian = new List<TradeLine>();
                    var trcount = trdlines.Where(x => x.atbureau == "TransUnion");
                    var Equifaxcount = trdlines.Where(x => x.atbureau == "Equifax");
                    var Experiancount = trdlines.Where(x => x.atbureau == "Experian");
                    if (trcount.Count() > 0)
                    {
                        transunion.AddRange(trcount);
                    }
                    if (Equifaxcount.Count() > 0)
                    {
                        equifax.AddRange(Equifaxcount);
                    }
                    if (Experiancount.Count() > 0)
                    {
                        experian.AddRange(Experiancount);
                    }
                }

                cr.Equifax = equifax;
                cr.Experian = experian;
                cr.TransUnion = transunion;
                cr.inquiries = InquiryPartition;
                cr.PublicRecords = PublicRecords;

                List<MonthlyPayStatus> monthlyPayStatusEQ = new List<MonthlyPayStatus>();
                List<MonthlyPayStatus> monthlyPayStatusTU = new List<MonthlyPayStatus>();
                List<MonthlyPayStatus> monthlyPayStatusEX = new List<MonthlyPayStatus>();

                monthlyPayStatusEQ = new List<MonthlyPayStatus>();
                monthlyPayStatusTU = new List<MonthlyPayStatus>();
                monthlyPayStatusEX = new List<MonthlyPayStatus>();
                List<MonthlyPayStatusHistory> monthlyPayStatusHistoryList = new List<MonthlyPayStatusHistory>();
                MonthlyPayStatusHistory monthlyPayStatusHistory = new MonthlyPayStatusHistory();

                MonthlyPayStatus monthlyPayStatusEQItem = new MonthlyPayStatus();
                try
                {
                    for (int i = 0; i < equifax.Count; i++)
                    {
                        monthlyPayStatusEQItem = new MonthlyPayStatus();
                        monthlyPayStatusEQItem.Agency = equifax[i].atbureau;
                        monthlyPayStatusEQItem.Bank = equifax[i].atcreditorName;
                        monthlyPayStatusEQItem.AccountNo = equifax[i].ataccountNumber;
                        GrantedTrade gt = equifax[i].GrantedTrade;
                        if (gt != null)
                        {
                            monthlyPayStatusEQItem.atmonthsReviewed = gt.atmonthsReviewed;
                            monthlyPayStatusEQItem.atmonthlyPayment = gt.atmonthlyPayment;
                            monthlyPayStatusEQItem.atlate90Count = gt.atlate90Count;
                            monthlyPayStatusEQItem.atlate60Count = gt.atlate60Count;
                            monthlyPayStatusEQItem.atlate30Count = gt.atlate30Count;
                            monthlyPayStatusEQItem.atdateLastPayment = gt.atdateLastPayment;
                            monthlyPayStatusEQItem.attermMonths = gt.attermMonths;
                            monthlyPayStatusEQItem.atcollateral = gt.atcollateral;
                            monthlyPayStatusEQItem.atamountPastDue = gt.atamountPastDue;
                            monthlyPayStatusEQItem.atworstPatStatusCount = gt.atworstPatStatusCount;

                            monthlyPayStatusEQItem.NegitiveItemsCount = gt.atlate90Count.StringToInt(0)
                                + gt.atlate60Count.StringToInt(0) + gt.atlate30Count.StringToInt(0);
                            try
                            {
                                monthlyPayStatusEQItem.monthlyPayStatusEQ = new List<MonthlyPayStatu>();
                                var monthPayStatusEQ = gt.PayStatusHistory.MonthlyPayStatus;
                                monthlyPayStatusEQItem.monthlyPayStatusEQ = monthPayStatusEQ;
                                for (int j = 0; j < monthPayStatusEQ.Count; j++)
                                {
                                    monthlyPayStatusHistory = new MonthlyPayStatusHistory();
                                    monthlyPayStatusHistory.Agency = monthlyPayStatusEQItem.Agency;
                                    monthlyPayStatusHistory.Bank = monthlyPayStatusEQItem.Bank;
                                    monthlyPayStatusHistory.AccountNo = monthlyPayStatusEQItem.AccountNo;
                                    monthlyPayStatusHistory.atdate = monthPayStatusEQ[j].atdate;
                                    monthlyPayStatusHistory.atstatus = monthPayStatusEQ[j].atstatus;
                                    monthlyPayStatusHistoryList.Add(monthlyPayStatusHistory);
                                }
                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                            }
                        }
                        monthlyPayStatusEQ.Add(monthlyPayStatusEQItem);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }




                MonthlyPayStatus monthlyPayStatusTUItem = new MonthlyPayStatus();
                try
                {
                    for (int i = 0; i < transunion.Count; i++)
                    {
                        monthlyPayStatusTUItem = new MonthlyPayStatus();
                        monthlyPayStatusTUItem.Agency = transunion[i].atbureau;
                        monthlyPayStatusTUItem.Bank = transunion[i].atcreditorName;
                        monthlyPayStatusTUItem.AccountNo = transunion[i].ataccountNumber;
                        GrantedTrade gt = transunion[i].GrantedTrade;
                        if (gt != null)
                        {
                            monthlyPayStatusTUItem.atmonthsReviewed = gt.atmonthsReviewed;
                            monthlyPayStatusTUItem.atmonthlyPayment = gt.atmonthlyPayment;
                            monthlyPayStatusTUItem.atlate90Count = gt.atlate90Count;
                            monthlyPayStatusTUItem.atlate60Count = gt.atlate60Count;
                            monthlyPayStatusTUItem.atlate30Count = gt.atlate30Count;
                            monthlyPayStatusTUItem.atdateLastPayment = gt.atdateLastPayment;
                            monthlyPayStatusTUItem.attermMonths = gt.attermMonths;
                            monthlyPayStatusTUItem.atcollateral = gt.atcollateral;
                            monthlyPayStatusTUItem.atamountPastDue = gt.atamountPastDue;
                            monthlyPayStatusTUItem.atworstPatStatusCount = gt.atworstPatStatusCount;
                            monthlyPayStatusTUItem.NegitiveItemsCount = gt.atlate90Count.StringToInt(0)
                               + gt.atlate60Count.StringToInt(0) + gt.atlate30Count.StringToInt(0);
                            try
                            {
                                monthlyPayStatusTUItem.monthlyPayStatusTU = new List<MonthlyPayStatu>();
                                var monthPayStatusTU = gt.PayStatusHistory != null ? gt.PayStatusHistory.MonthlyPayStatus : new List<MonthlyPayStatu>();
                                monthlyPayStatusTUItem.monthlyPayStatusTU = monthPayStatusTU;
                                for (int j = 0; j < monthPayStatusTU.Count; j++)
                                {
                                    monthlyPayStatusHistory = new MonthlyPayStatusHistory();
                                    monthlyPayStatusHistory.Agency = monthlyPayStatusTUItem.Agency;
                                    monthlyPayStatusHistory.Bank = monthlyPayStatusTUItem.Bank;
                                    monthlyPayStatusHistory.AccountNo = monthlyPayStatusTUItem.AccountNo;
                                    monthlyPayStatusHistory.atdate = monthPayStatusTU[j].atdate;
                                    monthlyPayStatusHistory.atstatus = monthPayStatusTU[j].atstatus;
                                    monthlyPayStatusHistoryList.Add(monthlyPayStatusHistory);
                                }
                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                            }
                        }


                        monthlyPayStatusTU.Add(monthlyPayStatusTUItem);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }



                MonthlyPayStatus monthlyPayStatusEXItem = new MonthlyPayStatus();
                try
                {
                    for (int i = 0; i < experian.Count; i++)
                    {
                        monthlyPayStatusEXItem = new MonthlyPayStatus();
                        monthlyPayStatusEXItem.Agency = experian[i].atbureau;
                        monthlyPayStatusEXItem.Bank = experian[i].atcreditorName;
                        monthlyPayStatusEXItem.AccountNo = experian[i].ataccountNumber;
                        GrantedTrade gt = experian[i].GrantedTrade;
                        if (gt != null)
                        {
                            monthlyPayStatusEXItem.atmonthsReviewed = gt.atmonthsReviewed;
                            monthlyPayStatusEXItem.atmonthlyPayment = gt.atmonthlyPayment;
                            monthlyPayStatusEXItem.atlate90Count = gt.atlate90Count;
                            monthlyPayStatusEXItem.atlate60Count = gt.atlate60Count;
                            monthlyPayStatusEXItem.atlate30Count = gt.atlate30Count;
                            monthlyPayStatusEXItem.atdateLastPayment = gt.atdateLastPayment;
                            monthlyPayStatusEXItem.attermMonths = gt.attermMonths;
                            monthlyPayStatusEXItem.atcollateral = gt.atcollateral;
                            monthlyPayStatusEXItem.atamountPastDue = gt.atamountPastDue;
                            monthlyPayStatusEXItem.atworstPatStatusCount = gt.atworstPatStatusCount;
                            monthlyPayStatusEXItem.NegitiveItemsCount = gt.atlate90Count.StringToInt(0)
                               + gt.atlate60Count.StringToInt(0) + gt.atlate30Count.StringToInt(0);
                            try
                            {
                                monthlyPayStatusEXItem.monthlyPayStatusEX = new List<MonthlyPayStatu>();
                                var monthPayStatusEX = gt.PayStatusHistory.MonthlyPayStatus;
                                monthlyPayStatusEXItem.monthlyPayStatusEX = monthPayStatusEX;
                                for (int j = 0; j < monthPayStatusEX.Count; j++)
                                {
                                    monthlyPayStatusHistory = new MonthlyPayStatusHistory();
                                    monthlyPayStatusHistory.Agency = monthlyPayStatusEXItem.Agency;
                                    monthlyPayStatusHistory.Bank = monthlyPayStatusEXItem.Bank;
                                    monthlyPayStatusHistory.AccountNo = monthlyPayStatusEXItem.AccountNo;
                                    monthlyPayStatusHistory.atdate = monthPayStatusEX[j].atdate;
                                    monthlyPayStatusHistory.atstatus = monthPayStatusEX[j].atstatus;
                                    monthlyPayStatusHistoryList.Add(monthlyPayStatusHistory);
                                }

                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                            }
                        }
                        monthlyPayStatusEX.Add(monthlyPayStatusEXItem);

                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }


                cr.monthlyPayStatusEQ = monthlyPayStatusEQ;
                cr.monthlyPayStatusEX = monthlyPayStatusEX;
                cr.monthlyPayStatusTU = monthlyPayStatusTU;
                cr.monthlyPayStatusHistoryList = monthlyPayStatusHistoryList;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return cr;
        }

        public static object pullcreditTesting()
        {
            CreditReport cr = new CreditReport();
            try
            {
                string html2 = System.IO.File.ReadAllText(@"E:\\CreditReportData09082020.txt");
                html2 = html2.Replace("JSON_CALLBACK(", "");
                html2 = html2.TrimEnd(')', ' ');
                html2 = html2.Replace("$", "dollar");
                html2 = html2.Replace("@", "at");
                html2 = html2.Replace("{ \"", " { ");
                html2 = html2.Replace("\" :", " : ");
                html2 = html2.Replace(", \"", " , ");

                var data = JsonConvert.DeserializeObject<dynamic>(html2);
                RootObject rootObj = JsonConvert.DeserializeObject<RootObject>(html2);
                //TU  EQ  EXP
                List<TradeLinePartition> tdl = new List<TradeLinePartition>();
                List<InquiryPartition> InquiryPartition = new List<InquiryPartition>();
                List<TradeLine> trdlines = new List<TradeLine>();
                List<TradeLine> transunion = new List<TradeLine>();
                List<TradeLine> equifax = new List<TradeLine>();
                List<TradeLine> experian = new List<TradeLine>();

                tdl.AddRange(rootObj.BundleComponents.BundleComponent[6].TrueLinkCreditReportType.TradeLinePartition);
                //InquiryPartition.AddRange(rootObj.BundleComponents.BundleComponent[6].TrueLinkCreditReportType.InquiryPartition);
                for (int i = 0; i < tdl.Count; i++)
                {
                    string str = tdl[i].Tradeline.ToString();
                    string name = string.Empty;
                    if (str.Substring(0, 1).Contains("["))
                    {
                        name = "{ TradeLine: " + str + " }";
                    }
                    else
                    {
                        name = "{ TradeLine: [ " + str + " ] }";
                    }

                    //str = str.Replace("[{", "TradeLine: [ {");
                    try
                    {
                        ChildModel td = JsonConvert.DeserializeObject<ChildModel>(name);
                        trdlines.AddRange(td.TradeLine);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                if (trdlines.Count > 0)
                {

                    transunion = new List<TradeLine>();
                    equifax = new List<TradeLine>();
                    experian = new List<TradeLine>();
                    var trcount = trdlines.Where(x => x.atbureau == "TransUnion");
                    var Equifaxcount = trdlines.Where(x => x.atbureau == "Equifax");
                    var Experiancount = trdlines.Where(x => x.atbureau == "Experian");
                    if (trcount.Count() > 0)
                    {
                        transunion.AddRange(trcount);
                    }
                    if (Equifaxcount.Count() > 0)
                    {
                        equifax.AddRange(Equifaxcount);
                    }
                    if (Experiancount.Count() > 0)
                    {
                        experian.AddRange(Experiancount);
                    }
                }

                cr.Equifax = equifax;
                cr.Experian = experian;
                cr.TransUnion = transunion;



                List<MonthlyPayStatus> monthlyPayStatusEQ = new List<MonthlyPayStatus>();
                List<MonthlyPayStatus> monthlyPayStatusTU = new List<MonthlyPayStatus>();
                List<MonthlyPayStatus> monthlyPayStatusEX = new List<MonthlyPayStatus>();

                monthlyPayStatusEQ = new List<MonthlyPayStatus>();
                monthlyPayStatusTU = new List<MonthlyPayStatus>();
                monthlyPayStatusEX = new List<MonthlyPayStatus>();
                List<MonthlyPayStatusHistory> monthlyPayStatusHistoryList = new List<MonthlyPayStatusHistory>();
                MonthlyPayStatusHistory monthlyPayStatusHistory = new MonthlyPayStatusHistory();

                MonthlyPayStatus monthlyPayStatusEQItem = new MonthlyPayStatus();
                try
                {
                    for (int i = 0; i < equifax.Count; i++)
                    {
                        monthlyPayStatusEQItem = new MonthlyPayStatus();
                        monthlyPayStatusEQItem.Agency = equifax[i].atbureau;
                        monthlyPayStatusEQItem.Bank = equifax[i].atcreditorName;
                        monthlyPayStatusEQItem.AccountNo = equifax[i].ataccountNumber;
                        GrantedTrade gt = equifax[i].GrantedTrade;
                        if (gt != null)
                        {
                            monthlyPayStatusEQItem.atmonthsReviewed = gt.atmonthsReviewed;
                            monthlyPayStatusEQItem.atmonthlyPayment = gt.atmonthlyPayment;
                            monthlyPayStatusEQItem.atlate90Count = gt.atlate90Count;
                            monthlyPayStatusEQItem.atlate60Count = gt.atlate60Count;
                            monthlyPayStatusEQItem.atlate30Count = gt.atlate30Count;
                            monthlyPayStatusEQItem.atdateLastPayment = gt.atdateLastPayment;
                            monthlyPayStatusEQItem.attermMonths = gt.attermMonths;
                            monthlyPayStatusEQItem.atcollateral = gt.atcollateral;
                            monthlyPayStatusEQItem.atamountPastDue = gt.atamountPastDue;
                            monthlyPayStatusEQItem.atworstPatStatusCount = gt.atworstPatStatusCount;

                            monthlyPayStatusEQItem.NegitiveItemsCount = gt.atlate90Count.StringToInt(0)
                                + gt.atlate60Count.StringToInt(0) + gt.atlate30Count.StringToInt(0);
                            try
                            {
                                monthlyPayStatusEQItem.monthlyPayStatusEQ = new List<MonthlyPayStatu>();
                                var monthPayStatusEQ = gt.PayStatusHistory.MonthlyPayStatus;
                                monthlyPayStatusEQItem.monthlyPayStatusEQ = monthPayStatusEQ;
                                for (int j = 0; j < monthPayStatusEQ.Count; j++)
                                {
                                    monthlyPayStatusHistory = new MonthlyPayStatusHistory();
                                    monthlyPayStatusHistory.Agency = monthlyPayStatusEQItem.Agency;
                                    monthlyPayStatusHistory.Bank = monthlyPayStatusEQItem.Bank;
                                    monthlyPayStatusHistory.AccountNo = monthlyPayStatusEQItem.AccountNo;
                                    monthlyPayStatusHistory.atdate = monthPayStatusEQ[j].atdate;
                                    monthlyPayStatusHistory.atstatus = monthPayStatusEQ[j].atstatus;
                                    monthlyPayStatusHistoryList.Add(monthlyPayStatusHistory);
                                }
                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                            }
                        }
                        monthlyPayStatusEQ.Add(monthlyPayStatusEQItem);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }




                MonthlyPayStatus monthlyPayStatusTUItem = new MonthlyPayStatus();
                try
                {
                    for (int i = 0; i < transunion.Count; i++)
                    {
                        monthlyPayStatusTUItem = new MonthlyPayStatus();
                        monthlyPayStatusTUItem.Agency = transunion[i].atbureau;
                        monthlyPayStatusTUItem.Bank = transunion[i].atcreditorName;
                        monthlyPayStatusTUItem.AccountNo = transunion[i].ataccountNumber;
                        GrantedTrade gt = transunion[i].GrantedTrade;
                        if (gt != null)
                        {
                            monthlyPayStatusTUItem.atmonthsReviewed = gt.atmonthsReviewed;
                            monthlyPayStatusTUItem.atmonthlyPayment = gt.atmonthlyPayment;
                            monthlyPayStatusTUItem.atlate90Count = gt.atlate90Count;
                            monthlyPayStatusTUItem.atlate60Count = gt.atlate60Count;
                            monthlyPayStatusTUItem.atlate30Count = gt.atlate30Count;
                            monthlyPayStatusTUItem.atdateLastPayment = gt.atdateLastPayment;
                            monthlyPayStatusTUItem.attermMonths = gt.attermMonths;
                            monthlyPayStatusTUItem.atcollateral = gt.atcollateral;
                            monthlyPayStatusTUItem.atamountPastDue = gt.atamountPastDue;
                            monthlyPayStatusTUItem.atworstPatStatusCount = gt.atworstPatStatusCount;
                            monthlyPayStatusTUItem.NegitiveItemsCount = gt.atlate90Count.StringToInt(0)
                               + gt.atlate60Count.StringToInt(0) + gt.atlate30Count.StringToInt(0);
                            try
                            {
                                monthlyPayStatusTUItem.monthlyPayStatusTU = new List<MonthlyPayStatu>();
                                var monthPayStatusTU = gt.PayStatusHistory != null ? gt.PayStatusHistory.MonthlyPayStatus : new List<MonthlyPayStatu>();
                                monthlyPayStatusTUItem.monthlyPayStatusTU = monthPayStatusTU;
                                for (int j = 0; j < monthPayStatusTU.Count; j++)
                                {
                                    monthlyPayStatusHistory = new MonthlyPayStatusHistory();
                                    monthlyPayStatusHistory.Agency = monthlyPayStatusTUItem.Agency;
                                    monthlyPayStatusHistory.Bank = monthlyPayStatusTUItem.Bank;
                                    monthlyPayStatusHistory.AccountNo = monthlyPayStatusTUItem.AccountNo;
                                    monthlyPayStatusHistory.atdate = monthPayStatusTU[j].atdate;
                                    monthlyPayStatusHistory.atstatus = monthPayStatusTU[j].atstatus;
                                    monthlyPayStatusHistoryList.Add(monthlyPayStatusHistory);
                                }
                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                            }
                        }


                        monthlyPayStatusTU.Add(monthlyPayStatusTUItem);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }



                MonthlyPayStatus monthlyPayStatusEXItem = new MonthlyPayStatus();
                try
                {
                    for (int i = 0; i < experian.Count; i++)
                    {
                        monthlyPayStatusEXItem = new MonthlyPayStatus();
                        monthlyPayStatusEXItem.Agency = experian[i].atbureau;
                        monthlyPayStatusEXItem.Bank = experian[i].atcreditorName;
                        monthlyPayStatusEXItem.AccountNo = experian[i].ataccountNumber;
                        GrantedTrade gt = experian[i].GrantedTrade;
                        if (gt != null)
                        {
                            monthlyPayStatusEXItem.atmonthsReviewed = gt.atmonthsReviewed;
                            monthlyPayStatusEXItem.atmonthlyPayment = gt.atmonthlyPayment;
                            monthlyPayStatusEXItem.atlate90Count = gt.atlate90Count;
                            monthlyPayStatusEXItem.atlate60Count = gt.atlate60Count;
                            monthlyPayStatusEXItem.atlate30Count = gt.atlate30Count;
                            monthlyPayStatusEXItem.atdateLastPayment = gt.atdateLastPayment;
                            monthlyPayStatusEXItem.attermMonths = gt.attermMonths;
                            monthlyPayStatusEXItem.atcollateral = gt.atcollateral;
                            monthlyPayStatusEXItem.atamountPastDue = gt.atamountPastDue;
                            monthlyPayStatusEXItem.atworstPatStatusCount = gt.atworstPatStatusCount;
                            monthlyPayStatusEXItem.NegitiveItemsCount = gt.atlate90Count.StringToInt(0)
                               + gt.atlate60Count.StringToInt(0) + gt.atlate30Count.StringToInt(0);
                            try
                            {
                                monthlyPayStatusEXItem.monthlyPayStatusEX = new List<MonthlyPayStatu>();
                                var monthPayStatusEX = gt.PayStatusHistory.MonthlyPayStatus;
                                monthlyPayStatusEXItem.monthlyPayStatusEX = monthPayStatusEX;
                                for (int j = 0; j < monthPayStatusEX.Count; j++)
                                {
                                    monthlyPayStatusHistory = new MonthlyPayStatusHistory();
                                    monthlyPayStatusHistory.Agency = monthlyPayStatusEXItem.Agency;
                                    monthlyPayStatusHistory.Bank = monthlyPayStatusEXItem.Bank;
                                    monthlyPayStatusHistory.AccountNo = monthlyPayStatusEXItem.AccountNo;
                                    monthlyPayStatusHistory.atdate = monthPayStatusEX[j].atdate;
                                    monthlyPayStatusHistory.atstatus = monthPayStatusEX[j].atstatus;
                                    monthlyPayStatusHistoryList.Add(monthlyPayStatusHistory);
                                }

                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                            }
                        }
                        monthlyPayStatusEX.Add(monthlyPayStatusEXItem);

                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }


                cr.monthlyPayStatusEQ = monthlyPayStatusEQ;
                cr.monthlyPayStatusEX = monthlyPayStatusEX;
                cr.monthlyPayStatusTU = monthlyPayStatusTU;
                cr.monthlyPayStatusHistoryList = monthlyPayStatusHistoryList;

                try
                {
                    InquiryPartition.AddRange(rootObj.BundleComponents.BundleComponent[6].TrueLinkCreditReportType.InquiryPartition);
                    cr.inquiries = InquiryPartition;
                }
                catch (Exception ex)
                {

                    string msg = ex.Message;
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return cr;
        }
    }
}
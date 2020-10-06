using CreditReversal.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace CreditReversal.BLL
{
    public class PayPalPayment
    {
        public Hashtable  Payment(decimal amount, AuthorizeDotNetModel parentModel, string ipAddress)
        {
            Hashtable htResponse = new Hashtable();
            string res = "";
            //API Credentials (3-token)
            string strUsername = "";
            string strPassword = "";
            string strSignature = "";
            string strNVPSandboxServer = "";
            string strNVPLiveServer = "";
            string strAPIVersion = "60.0";
            string strPaymentMethod = ""; //DoDirectPayment
            string strEnvironment = "";

            Common common = new Common();
            DataTable dt = common.getSettings();

            if (dt.Rows.Count > 0)
            {
                strUsername = dt.Rows[0]["PayPalAPIUserName"].ToString();
                strPassword = dt.Rows[0]["PayPalAPIPassword"].ToString();
                strSignature = dt.Rows[0]["PayPalAPISignature"].ToString();
                strNVPSandboxServer = dt.Rows[0]["PayPalSandBoxServer"].ToString();
                strNVPLiveServer = dt.Rows[0]["PayPalLiveServer"].ToString();
                strAPIVersion = dt.Rows[0]["PayPalAPIVersion"].ToString();
                strPaymentMethod = dt.Rows[0]["PayPalPaymentMethod"].ToString();
                strEnvironment = dt.Rows[0]["PayPalEnvironment"].ToString();
            }

            string strCredentials = "USER=" + strUsername + "&PWD=" + strPassword + "&SIGNATURE=" + strSignature;
            //SandBox server Url - https://api-3t.paypal.com/nv
            //Live server Url - https://api.paypal.com/nvp
            //strNVPSandboxServer  represents server url
            
            //sId represents the stateId
            //NY - NewYork

            //grandTotal represents the shopping cart total 
            string strNVP = "METHOD=" + strPaymentMethod +
                        "&VERSION=" + strAPIVersion +
                        "&PWD=" + strPassword +
                        "&USER=" + strUsername +
                        "&SIGNATURE=" + strSignature +
                        "&PAYMENTACTION=Sale" +
                        "&IPADDRESS=" + ipAddress +
                        "&RETURNFMFDETAILS=0" +
                        "&CREDITCARDTYPE=" + parentModel.creditCardDetails.CardType + //"Visa" +
                        "&ACCT=" + parentModel.creditCardDetails.CardNumber + //"4111111111111111" +
                        "&EXPDATE=" + parentModel.creditCardDetails.ExpDate +
                        "&CVV2=" + parentModel.creditCardDetails.CardCode + //111 +
                        "&STARTDATE=" +
                        "&ISSUENUMBER=" +
                        "&EMAIL=" + parentModel.customerBillingInfo.EmailAddress + //satishyellapu.m3@gmail.com" +
                        //the following  represents the billing details
                        "&FIRSTNAME=" + parentModel.customerBillingInfo.FirstName + //billingFirstName +
                        "&LASTNAME=" + parentModel.customerBillingInfo.LastName  + //billingLastName +
                        "&STREET=" + parentModel.customerBillingInfo.Address + //billingAddress1 +
                        "&STREET2=" + "" +
                        "&CITY=" + parentModel.customerBillingInfo.City + // "Vizag" +
                        "&STATE=" + parentModel.customerBillingInfo.State + //"GA" +
                        "&COUNTRYCODE=US" +
                        "&ZIP=" + parentModel.customerBillingInfo.ZipCode + // "530041" +
                        "&AMT=" +  amount.ToString().PadUpto2() + // "100.00" +//orderdetails.GrandTotal.ToString("0.0")+
                        "&CURRENCYCODE=USD" +
                        "&DESC=CreditReversalGuru" +
                        "&INVNUM=" + "";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                //Create web request and web response objects, make sure you using the correct server (sandbox/live)
                HttpWebRequest wrWebRequest = (strEnvironment.ToUpper() == "SANDBOX" 
                    ?  (HttpWebRequest)WebRequest.Create(strNVPSandboxServer) : (HttpWebRequest)WebRequest.Create(strNVPLiveServer));
                wrWebRequest.Method = "POST";
                StreamWriter requestWriter = new StreamWriter(wrWebRequest.GetRequestStream());
                requestWriter.Write(strNVP);
                requestWriter.Close();

                // Get the response.
                HttpWebResponse hwrWebResponse = (HttpWebResponse)wrWebRequest.GetResponse();
                StreamReader responseReader = new StreamReader(wrWebRequest.GetResponse().GetResponseStream());

                //and read the response
                string responseData = responseReader.ReadToEnd();
                responseReader.Close();

                // string result = Server.UrlDecode(responseData);
                string result = responseData;

                string[] arrResult = result.Split('&');
                
                string[] responseItemArray;
                foreach (string responseItem in arrResult)
                {
                    responseItemArray = responseItem.Split('=');
                    htResponse.Add(responseItemArray[0], responseItemArray[1]);
                }
                return htResponse;
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }
            return htResponse;
        }
    }
}
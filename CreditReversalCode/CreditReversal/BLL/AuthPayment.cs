using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using System.Dynamic;
using System.Web.Configuration;
using CreditReversal.DAL;
using System.Data;
using CreditReversal.Utilities;

namespace CreditReversal.BLL
{
    public class AuthPayment
    {
       
        public static ANetApiResponse Pay(decimal amount)
        {

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = "69QD4mbp",
                ItemElementName = ItemChoiceType.transactionKey,
                Item = "44d37Zb73aCNmBAL",
            };

            var creditCard = new creditCardType
            {
                cardNumber = "4111111111111111",
                expirationDate = "1028",
                cardCode = "123"
            };

            var billingAddress = new customerAddressType
            {
                firstName = "John",
                lastName = "Doe",
                address = "123 My St",
                city = "OurTown",
                zip = "98004"
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            // Add line Items
            var lineItems = new lineItemType[2];
            lineItems[0] = new lineItemType { itemId = "1", name = "t-shirt", quantity = 2, unitPrice = new Decimal(15.00) };
            lineItems[1] = new lineItemType { itemId = "2", name = "snowboard", quantity = 1, unitPrice = new Decimal(450.00) };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card

                amount = amount,
                payment = paymentType,
                billTo = billingAddress,
                lineItems = lineItems
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                        Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                        Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                        Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                    }
                    else
                    {
                        Console.WriteLine("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed Transaction.");
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Null Response.");
            }

            return response;
        }
        public static bool AuthorizationSettings()
        {
            try
            {
                string ApiLoginId = "";//WebConfigurationManager.AppSettings["ApiLoginId"];
                string ApiTransactionKey = "";// WebConfigurationManager.AppSettings["ApiTransactionKey"];
                string SecretKey = "";// WebConfigurationManager.AppSettings["SecretKey"];
                string Environment = "";// WebConfigurationManager.AppSettings["Environment"];

               // DBUtilities dBUtilities = new DBUtilities();
                  Common common = new Common();
                  DataTable dt = common.getSettings();

                if (dt.Rows.Count > 0)
                {
                    ApiLoginId = dt.Rows[0]["AuthApiLoginId"].ToString();
                    ApiTransactionKey = dt.Rows[0]["ApiTransactionKey"].ToString();
                    SecretKey = dt.Rows[0]["SecretKey"].ToString();
                    Environment = dt.Rows[0]["AuthEnvironment"].ToString();
                }


                ApiOperationBase<AuthorizeNet.Api.Contracts.V1.ANetApiRequest, AuthorizeNet.Api.Contracts.V1.ANetApiResponse>.RunEnvironment = Environment.ToUpper() == "SANDBOX" ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                // define the merchant information (authentication / transaction id)
                ApiOperationBase<AuthorizeNet.Api.Contracts.V1.ANetApiRequest, AuthorizeNet.Api.Contracts.V1.ANetApiResponse>.MerchantAuthentication = new AuthorizeNet.Api.Contracts.V1.merchantAuthenticationType()
                {
                    name = ApiLoginId,
                    ItemElementName = AuthorizeNet.Api.Contracts.V1.ItemChoiceType.transactionKey,
                    Item = ApiTransactionKey
                };
            }
            catch (Exception e)
            {

                return false;
            }
            return true;

        }
        #region Authorize and Capture Type Transaction
        // We are implementing Authorize and Capture transaction type by one customize method.In this, each transaction will be approved as well as automatically captured and
        //settlement for this transaction will happen with all transactions together within 24 hours as a batch process. One Unique 'TransactionId' will be given for each transaction
        //once it is approved.
        public static dynamic AuthorizeandCaptureTransaction(decimal amount, AuthorizeDotNetModel parentModel, string ipAddress)
        {
            dynamic retval = new ExpandoObject();
            var creditCard = new AuthorizeNet.Api.Contracts.V1.creditCardType();
            var billingAddress = new AuthorizeNet.Api.Contracts.V1.customerAddressType();
            var lineItems = new AuthorizeNet.Api.Contracts.V1.lineItemType[0];
            var order = new AuthorizeNet.Api.Contracts.V1.orderType();
            var ship = new AuthorizeNet.Api.Contracts.V1.nameAndAddressType();
            var tax = new AuthorizeNet.Api.Contracts.V1.extendedAmountType();
            var duty = new AuthorizeNet.Api.Contracts.V1.extendedAmountType();
            var shipping = new AuthorizeNet.Api.Contracts.V1.extendedAmountType();
            try
            {
                AuthorizationSettings();
                if (parentModel.creditCardDetails != null)
                {
                    creditCard = new AuthorizeNet.Api.Contracts.V1.creditCardType
                    {
                        cardNumber = parentModel.creditCardDetails.CardNumber,//"4111111111111111",
                        expirationDate = parentModel.creditCardDetails.ExpDate,
                        cardCode = parentModel.creditCardDetails.CardCode
                    };
                }
                if (parentModel.customerBillingInfo != null)
                {
                    billingAddress = new AuthorizeNet.Api.Contracts.V1.customerAddressType
                    {
                        firstName = GetValidString(parentModel.customerBillingInfo.FirstName, 50),
                        lastName = GetValidString(parentModel.customerBillingInfo.LastName, 50),
                        address = GetValidString(parentModel.customerBillingInfo.Address, 60),
                        city = GetValidString(parentModel.customerBillingInfo.City, 40),
                        zip = GetValidString(parentModel.customerBillingInfo.ZipCode, 20),
                        country = GetValidString(parentModel.customerBillingInfo.Country, 60),
                        state = GetValidString(parentModel.customerBillingInfo.State, 40),
                        company = GetValidString(parentModel.customerBillingInfo.CompanyName, 50),
                        email = parentModel.customerBillingInfo.EmailAddress,
                        phoneNumber = parentModel.customerBillingInfo.PhoneNumber,
                        faxNumber = parentModel.customerBillingInfo.Fax
                    };
                }
                //standard api call to retrieve response
                var paymentType = new AuthorizeNet.Api.Contracts.V1.paymentType { Item = creditCard };
                // Add line Items
                if (parentModel.customerLineItems.Count > 0)
                {
                    lineItems = new AuthorizeNet.Api.Contracts.V1.lineItemType[parentModel.customerLineItems.Count];
                    for (int i = 0; i < parentModel.customerLineItems.Count; i++)
                    {
                        lineItems[i] = new AuthorizeNet.Api.Contracts.V1.lineItemType
                        {
                            //itemId = parentModel.customerLineItems[i].Item,
                            //name = parentModel.customerLineItems[i].ItemName,
                            //description = parentModel.customerLineItems[i].Description,
                            itemId = GetValidString(parentModel.customerLineItems[i].Item, 31),
                            name = GetValidString(parentModel.customerLineItems[i].ItemName, 31),
                            description = GetValidString(parentModel.customerLineItems[i].Description, 255),
                            quantity = new Decimal(parentModel.customerLineItems[i].Quantity),
                            unitPrice = new Decimal(parentModel.customerLineItems[i].Unitprice)
                        };
                    }
                }
                if (parentModel.customerOrderInfo != null)
                {
                    order = new AuthorizeNet.Api.Contracts.V1.orderType
                    {
                        invoiceNumber = parentModel.customerOrderInfo.InVoice,
                        description = parentModel.customerOrderInfo.Description
                    };
                }
                if (parentModel.customerShippingInfo != null)
                {
                    ship = new AuthorizeNet.Api.Contracts.V1.nameAndAddressType
                    {
                        firstName = GetValidString(parentModel.customerShippingInfo.FirstName, 50),
                        lastName = GetValidString(parentModel.customerShippingInfo.LastName, 50),
                        address = GetValidString(parentModel.customerShippingInfo.Address, 60),
                        city = GetValidString(parentModel.customerShippingInfo.City, 40),
                        state = GetValidString(parentModel.customerShippingInfo.State, 40),
                        zip = GetValidString(parentModel.customerShippingInfo.ZipCode, 20),
                        company = GetValidString(parentModel.customerShippingInfo.CompanyName, 50),
                        country = GetValidString(parentModel.customerShippingInfo.Country, 60),
                    };
                }
                if (parentModel.customerAdditionalinfo != null)
                {
                    tax = new AuthorizeNet.Api.Contracts.V1.extendedAmountType
                    {
                        amount = new Decimal(parentModel.customerAdditionalinfo.Tax)
                    };
                    duty = new AuthorizeNet.Api.Contracts.V1.extendedAmountType
                    {
                        amount = new Decimal(parentModel.customerAdditionalinfo.Duty)
                    };
                    shipping = new AuthorizeNet.Api.Contracts.V1.extendedAmountType
                    {
                        amount = new Decimal(parentModel.customerAdditionalinfo.Freight)
                    };
                }
                var usefields = new AuthorizeNet.Api.Contracts.V1.userField[2];
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                    {
                        usefields[i] = new AuthorizeNet.Api.Contracts.V1.userField
                        {
                            name = "IP",
                            value = ipAddress
                        };
                    }
                    else
                    {
                        usefields[i] = new AuthorizeNet.Api.Contracts.V1.userField
                        {
                            name = "Date",
                            value = parentModel.customerAdditionalinfo != null ? parentModel.customerAdditionalinfo.PONumber : ""
                        };
                    }
                }
                var transactionRequest = new AuthorizeNet.Api.Contracts.V1.transactionRequestType
                {
                    transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card
                    order = order,
                    amount = amount,
                    payment = paymentType,
                    billTo = billingAddress,
                    lineItems = lineItems,
                    shipTo = ship,
                    poNumber = parentModel.customerAdditionalinfo != null ? parentModel.customerAdditionalinfo.PONumber : "",
                    taxExemptSpecified = true,
                    taxExempt = false,
                    tax = tax,
                    duty = duty,
                    shipping = shipping,
                    customer = new AuthorizeNet.Api.Contracts.V1.customerDataType
                    {
                        id = parentModel.custId,
                        email = billingAddress.email,
                        typeSpecified = false
                    },
                    //  userFields = usefields
                };
                var request = new AuthorizeNet.Api.Contracts.V1.createTransactionRequest { transactionRequest = transactionRequest };
                // instantiate the contoller that will call the service
                var controller = new createTransactionController(request);
                controller.Execute();
                // get the response from the service (errors contained if any)
                var response = controller.GetApiResponse();
                //validate
                if (response != null)
                {
                    if (response.messages.resultCode == AuthorizeNet.Api.Contracts.V1.messageTypeEnum.Ok)
                    {
                        if (response.transactionResponse.messages != null)
                        {
                            retval.Status = "SUCCESS";
                            retval.TransactionId = response.transactionResponse.transId;
                            retval.ResponseCode = response.transactionResponse.responseCode;
                            retval.MessageCode = response.transactionResponse.messages[0].code;
                            retval.Description = response.transactionResponse.messages[0].description;
                            retval.AuthorizeCode = response.transactionResponse.authCode;
                            //The code in the comment indicates we can send mail to customer abot transaction so that we can get the response that
                            // Mail sent to customer. But the customer Mail Address can't  be appeared in 'Merchant Mail' .
                            //var sendrequest = new AuthorizeNet.Api.Contracts.V1.sendCustomerTransactionReceiptRequest();
                            //sendrequest.transId = response.transactionResponse.transId;
                            //sendrequest.customerEmail = transactionRequest.billTo.email;
                            //AuthorizeNet.Api.Contracts.V1.sendCustomerTransactionReceiptRequest req = sendrequest;
                            //var sendController = new sendCustomerTransactionReceiptController(req);
                            //sendController.Execute();
                            //var sendResponse = sendController.GetApiResponse();
                            //if(sendResponse!=null)
                            //{
                            //    if(sendResponse.messages.resultCode==AuthorizeNet.Api.Contracts.V1.messageTypeEnum.Ok)
                            //    {
                            //        retval.MailStatus = "SUCCESS";
                            //        retval.MailMessage = sendResponse.messages.message[0].text;
                            //        retval.MailMessageCode = sendResponse.messages.message[0].code;
                            //    }
                            //    else
                            //    {
                            //        retval.MailStatus = "FAILURE";
                            //        retval.MailMessage = "UnSuccessful Mail Can't Be Send To Customer";
                            //    }
                            //}
                            //else
                            //{
                            //    retval.MailStatus = "FAILURE";
                            //    retval.MailMessage = "UnSuccessful Mail Can't Be Send To Customer";
                            //}
                        }
                        else
                        {
                            if (response.transactionResponse.errors != null)
                            {
                                retval.Status = "FAILURE";
                                retval.ErrorCode = response.transactionResponse.errors[0].errorCode;
                                retval.ErrorMessage = response.transactionResponse.errors[0].errorText;
                            }
                        }
                    }
                    else
                    {
                        if (response.transactionResponse != null && response.transactionResponse.errors != null)
                        {
                            retval.Status = "FAILURE";
                            retval.ErrorCode = response.transactionResponse.errors[0].errorCode;
                            retval.ErrorMessage = response.transactionResponse.errors[0].errorText;
                        }
                        else
                        {
                            retval.Status = "FAILURE";
                            retval.ErrorCode = response.messages.message[0].code;
                            retval.ErrorMessage = response.messages.message[0].text;
                        }
                    }
                }
                else
                {
                    var respon = controller.GetErrorResponse();
                    retval.Status = "FAILURE";
                    retval.ErrorCode = "Null Reponse Occured " + respon.messages.message[0].code;
                    retval.ErrorMessage = respon.messages.message[0].text;
                }
            }
            catch (Exception e)
            {
                retval.Status = "FAILURE";
                retval.ErrorCode = "Exception : " + e.Message + e.InnerException + " occured. ";
                retval.ErrorMessage = "UnSuccessful Transaction";
            }
            return retval;
        }
        #endregion
        #region To Refund the Transaction by 'TransactionId' and 'Amount' Specified
        // By this method, we can refund the transaction amount tp customer by giving 'TransactionId' and 'Amount' specified as input.
        public static dynamic RefundTheTransaction(decimal TransactionAmount, string TransactionID, AuthorizeDotNetModel parentModel)
        {
            dynamic retval = new ExpandoObject();
            try
            {
                AuthorizationSettings();
                var creditCard = new AuthorizeNet.Api.Contracts.V1.creditCardType
                {
                    cardNumber = parentModel.creditCardDetails.CardNumber,
                    expirationDate = parentModel.creditCardDetails.ExpDate
                };
                //standard api call to retrieve response
                var paymentType = new AuthorizeNet.Api.Contracts.V1.paymentType { Item = creditCard };
                var transactionRequest = new AuthorizeNet.Api.Contracts.V1.transactionRequestType
                {
                    transactionType = transactionTypeEnum.refundTransaction.ToString(),    // refund type
                    payment = paymentType,
                    amount = TransactionAmount,
                    refTransId = TransactionID
                };
                var request = new AuthorizeNet.Api.Contracts.V1.createTransactionRequest { transactionRequest = transactionRequest };
                // instantiate the contoller that will call the service
                var controller = new createTransactionController(request);
                controller.Execute();
                // get the response from the service (errors contained if any)
                var response = controller.GetApiResponse();
                //validate
                if (response != null)
                {
                    if (response.messages.resultCode == AuthorizeNet.Api.Contracts.V1.messageTypeEnum.Ok)
                    {
                        if (response.transactionResponse.messages != null)
                        {
                            retval.Status = "SUCCESS";
                            retval.TransactionId = response.transactionResponse.transId;
                            retval.ResponseCode = response.transactionResponse.responseCode;
                            retval.MessageCode = response.transactionResponse.messages[0].code;
                            retval.Description = response.transactionResponse.messages[0].description;
                            retval.SuccessAuthorizeCode = response.transactionResponse.authCode;
                        }
                        else
                        {
                            if (response.transactionResponse.errors != null)
                            {
                                retval.Status = "FAILURE";
                                retval.ErrorCode = response.transactionResponse.errors[0].errorCode;
                                retval.ErrorMessage = response.transactionResponse.errors[0].errorText;
                            }
                        }
                    }
                    else
                    {
                        if (response.transactionResponse != null && response.transactionResponse.errors != null)
                        {
                            retval.Status = "FAILURE";
                            retval.ErrorCode = response.transactionResponse.errors[0].errorCode;
                            retval.ErrorMessage = response.transactionResponse.errors[0].errorText;
                        }
                        else
                        {
                            retval.Status = "FAILURE";
                            retval.ErrorCode = response.messages.message[0].code;
                            retval.ErrorMessage = response.messages.message[0].code;
                        }
                    }
                }
                else
                {
                    retval.Status = "FAILURE";
                    retval.ErrorCode = "Null Reponse Occured";
                    retval.ErrorMessage = "UnSuccessful Operation";
                }
                try
                {
                    if (retval.ErrorMessage == "The sum of credits against the referenced transaction would exceed original debit amount.")
                    {
                        retval.ErrorMessage = "Payment already refunded.";
                    }
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
            catch (Exception e)
            {
                retval.Status = "FAILURE";
                retval.ErrorCode = "Exception : " + e.Message + e.InnerException + " occured. ";
                retval.ErrorMessage = "UnSuccessful Operation";
            }
            return retval;
        }
        #endregion
        #region To Get Each Transaction Detail Based On 'TransactionId'
        //With this method, we will get all details of particular transaction regarding  type, status amount and date by giving 'TransactionId' as input.
        public static dynamic GetTransactionDetails(string transactionId)
        {
            dynamic retval = new ExpandoObject();
            try
            {
                AuthorizationSettings();
                var request = new AuthorizeNet.Api.Contracts.V1.getTransactionDetailsRequest { transId = transactionId };
                // instantiate the controller that will call the service
                var controller = new getTransactionDetailsController(request);
                controller.Execute();
                // get the response from the service (errors contained if any)
                var response = controller.GetApiResponse();
                if (response != null && response.messages.resultCode == AuthorizeNet.Api.Contracts.V1.messageTypeEnum.Ok)
                {
                    if (response.transaction == null)
                    {
                        retval.Status = "SUCCESS";
                        retval.Message = "NO Details Available for this TransactionId : " + transactionId;
                        return retval;
                    }
                    retval.Status = "SUCCESS";
                    retval.TransactionId = response.transaction.transId;
                    retval.TransactionType = response.transaction.transactionType;
                    retval.TransactionStatus = response.transaction.transactionStatus;
                    retval.TransactionAuthorizeAmount = response.transaction.authAmount;
                    retval.TransactionSettleAmount = response.transaction.settleAmount;
                    retval.Item = response.transaction.payment.Item;
                    retval.Message = string.Empty;
                }
                else if (response != null)
                {
                    retval.Status = "FAILURE";
                    retval.Message = "Error : " + response.messages.message[0].code + response.messages.message[0].text;

                    retval.ErrorCode = string.Empty;
                    retval.ErrorMessage = string.Empty;
                    retval.TransactionId = string.Empty;
                    retval.TransactionType = string.Empty;
                    retval.TransactionStatus = string.Empty;
                    retval.TransactionAuthorizeAmount = string.Empty;
                    retval.TransactionSettleAmount = string.Empty;
                    retval.Item = string.Empty;
                }
            }
            catch (Exception e)
            {
                retval.Status = "FAILURE";
                retval.ErrorCode = "Exception : " + e.Message + e.InnerException + " occured. ";
                retval.ErrorMessage = "UnSuccessful Response";

                retval.TransactionId = string.Empty;
                retval.TransactionType = string.Empty;
                retval.TransactionStatus = string.Empty;
                retval.TransactionAuthorizeAmount = string.Empty;
                retval.TransactionSettleAmount = string.Empty;
                retval.Item = string.Empty;
                retval.Message = string.Empty;
            }
            return retval;
        }
        #endregion
        public static dynamic VoidTransaction(string TransactionID)
        {
            dynamic retval = new ExpandoObject();
            try
            {
                AuthorizationSettings();
                var transactionRequest = new AuthorizeNet.Api.Contracts.V1.transactionRequestType
                {
                    transactionType = transactionTypeEnum.voidTransaction.ToString(),    // refund type
                    refTransId = TransactionID
                };
                var request = new AuthorizeNet.Api.Contracts.V1.createTransactionRequest { transactionRequest = transactionRequest };
                // instantiate the controller that will call the service
                var controller = new createTransactionController(request);
                controller.Execute();
                // get the response from the service (errors contained if any)
                var response = controller.GetApiResponse();
                // validate response
                if (response != null)
                {
                    retval.Status = "FAILURE";
                    retval.resultCode = response.messages.resultCode;
                    if (response.messages.resultCode == AuthorizeNet.Api.Contracts.V1.messageTypeEnum.Ok)
                    {
                        if (response.transactionResponse.messages != null)
                        {
                            if (response.transactionResponse.messages[0].description == "This transaction has been approved.")
                            {
                                retval.Status = "SUCCESS";
                                retval.TransactionId = response.transactionResponse.transId;
                                retval.responseCode = response.transactionResponse.responseCode;
                                retval.code = response.transactionResponse.messages[0].code;
                                retval.description = response.transactionResponse.messages[0].description;
                                retval.authCode = response.transactionResponse.authCode;
                            }
                            else
                            {
                                retval.TransactionId = response.transactionResponse.transId;
                                retval.responseCode = response.transactionResponse.responseCode;
                                retval.errorCode = response.transactionResponse.messages[0].code;
                                retval.errorText = response.transactionResponse.messages[0].description;
                                retval.authCode = response.transactionResponse.authCode;
                            }
                        }
                        else
                        {
                            //  Console.WriteLine("Failed Transaction.");
                            if (response.transactionResponse.errors != null)
                            {
                                retval.errorCode = response.transactionResponse.errors[0].errorCode;
                                retval.errorText = response.transactionResponse.errors[0].errorText;
                                //Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                                //Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                            }
                        }
                    }
                    else
                    {
                        // Console.WriteLine("Failed Transaction.");
                        if (response.transactionResponse != null && response.transactionResponse.errors != null)
                        {
                            retval.errorCode = response.transactionResponse.errors[0].errorCode;
                            retval.errorText = response.transactionResponse.errors[0].errorText;
                            //Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            //Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                        else
                        {
                            retval.errorCode = response.messages.message[0].code;
                            retval.errorText = response.messages.message[0].text;
                            //Console.WriteLine("Error Code: " + response.messages.message[0].code);
                            //Console.WriteLine("Error message: " + response.messages.message[0].text);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                retval.Status = "FAILURE";
                retval.ErrorCode = "Exception : " + e.Message + e.InnerException + " occured. ";
                retval.errorText = "UnSuccessful Transaction";
            }
            return retval;
        }
        public static string GetValidString(string value, int reqLength)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Length > reqLength)
                    {
                        return value.Substring(0, reqLength);
                    }
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
            return value;
        }
    }
}

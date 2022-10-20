using Newtonsoft.Json.Linq;
using SPP_APISERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPP_APISERVER.Controllers
{
    public class paymentController : Controller
    {
        // GET: payment
        public string serectKey = "xUHfoPq35RGAHSJvuNc4AfR3YJ6RsTHG";
        ResponseService responseService = new ResponseService();
        TTPDatabaseDataContext db = new TTPDatabaseDataContext();
        HandleDatabase handleDatabase = new HandleDatabase();
        GetDatabase getDatabase = new GetDatabase();
        CryptoService crypto = new CryptoService();
        public ActionResult checkpayment()
        {
            string param = Request.QueryString["payment"].ToString();
            param = param.Replace(" ", "+");
            string uri = crypto.DecryptUri(param, serectKey);
            string[] listUri = uri.Split(new char[] { '&' });
            string[] uri0 = listUri[0].Split(new char[] { '=' });
            string[] uri1 = listUri[1].Split(new char[] { '=' });
            string[] uri2 = listUri[2].Split(new char[] { '=' });
            string[] uri3 = listUri[3].Split(new char[] { '=' });
            string[] uri4 = listUri[4].Split(new char[] { '=' });
            ViewBag.transId = uri0[1];
            ViewBag.amount = uri1[1];
            ViewBag.shopId = uri2[1];
            ViewBag.oderInfo = uri3[1];
            ViewBag.responseTime = uri4[1];
            return View();
        }
        [HttpPost]
        public ActionResult checkpayment(FormCollection collection)
        {
            string username = collection["Username"];
            string password = collection["Password"];
            string transId = collection["transId"];
            string amount = collection["amount"];
            string responseTime = crypto.DatetimeToTimestamp(DateTime.Now);
            Buyer buyer = getDatabase.GetBuyer(username);
            Merchant merchant = getDatabase.GetMerchantWithTransId(transId);
            OderRequest oderRequest = getDatabase.GetOderRequest(transId);
            // Check transId
            if (oderRequest == null)
            {
                OrderPaymentReceipt oderPaymen = new OrderPaymentReceipt()
                {
                    transId = transId,
                    amount = amount,
                    statusCode = "1",
                    responseTime = responseTime,
                    signature = ""
                };
                string messageHash =
                        "transId=" + transId +
                        "&amount=" + amount +
                        "&statusCode=1&responseTime=" + responseTime
                    ;
                string signature = crypto.signSHA256(messageHash, serectKey);

                // Object TPP send to Merchant
                JObject message = new JObject
                    {
                        { "transId", transId },
                        { "amount", amount },
                        { "statusCode", "1" },
                        { "responseTime", responseTime },
                        { "signature", signature }
                    };

                string OK = responseService.sendOrderPaymentReceipt(merchant.notifyUrl.ToString().Trim(), message.ToString());

                string resultUrl = merchant.resultUrl + "?" + messageHash + "&signature=" + signature; // params sends Buyer

                oderPaymen.signature = signature;
                oderPaymen.statusCode = "1";
                handleDatabase.InsertOderPaymentReceiptWithObj(oderPaymen);

                return Redirect(resultUrl);
            }
            if (buyer == null || merchant == null)
            {
                return View();
            }

            //
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                transId = transId,
                amount = amount,
                buyerId = buyer.buyerId,
                responseTime = responseTime,
                signature = "",
            };
            OrderPaymentReceipt oderPaymentReceipt = new OrderPaymentReceipt()
            {
                transId = transId,
                amount = amount,
                statusCode = "1",
                responseTime = responseTime,
                signature = ""
            };
            try
            {
                string messagepayment =
                        "transId=" + transId +
                        "&amount=" + amount +
                        "&buyerId=" + buyer.buyerId +
                        "&responseTime=" + responseTime
                    ;
                string signaturepayment = crypto.signSHA256(messagepayment, serectKey);
                paymentRequest.signature = signaturepayment;
                handleDatabase.InsertPaymentRequestWithObj(paymentRequest);
                int checkin = handleDatabase.Checkin(username, password, buyer.amount);
                if (checkin == 0)
                {
                    handleDatabase.Payment(username, transId, amount);
                    string messageHash = 
                        "transId=" + transId +
                        "&amount=" + amount +
                        "&statusCode=0&responseTime=" + responseTime
                    ;
                    string signature = crypto.signSHA256(messageHash, serectKey);

                    // Object TPP send to Merchant
                    JObject message = new JObject
                    {
                        { "transId", transId },
                        { "amount", amount },
                        { "statusCode", "0" },
                        { "responseTime", responseTime },
                        { "signature", signature }
                    };

                    string test = responseService.sendOrderPaymentReceipt(merchant.notifyUrl.ToString(), message.ToString());

                    string resultUrl = merchant.resultUrl + "?" + messageHash + "&signature=" + signature; // params sends Buyer

                    oderPaymentReceipt.signature = signature;
                    oderPaymentReceipt.statusCode = "0";
                    handleDatabase.InsertOderPaymentReceiptWithObj(oderPaymentReceipt);

                    return Redirect(resultUrl);
                }
                if (checkin == 1)
                {
                    // Wrong Password or Username not exist
                    string messageHash =
                        "transId=" + transId +
                        "&amount=" + amount +
                        "&statusCode=1&responseTime=" + responseTime
                    ;
                    string signature = crypto.signSHA256(messageHash, serectKey);

                    // Object TPP send to Merchant
                    JObject message = new JObject
                    {
                        { "transId", transId },
                        { "amount", amount },
                        { "statusCode", "1" },
                        { "responseTime", responseTime },
                        { "signature", signature }
                    };

                    string OK = responseService.sendOrderPaymentReceipt(merchant.notifyUrl.ToString().Trim(), message.ToString());

                    string resultUrl = merchant.resultUrl + "?" + messageHash + "&signature=" + signature; // params sends Buyer

                    oderPaymentReceipt.signature = signature;
                    oderPaymentReceipt.statusCode = "1";
                    handleDatabase.InsertOderPaymentReceiptWithObj(oderPaymentReceipt);

                    return Redirect(resultUrl);
                }
                if (checkin == -1)
                {
                    // Not Enough Monry
                    ViewBag.message = "Not Enough Monry";
                    return View();
                }
                if (checkin == -2)
                {
                    // Error System
                    ViewBag.message = "Error System";
                    return View();
                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.message = "Error System";
                return View();
            }
        }
    }
}
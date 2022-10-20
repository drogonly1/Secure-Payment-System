using Newtonsoft.Json.Linq;
using SPP_APISERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPP_APISERVER.Controllers
{
    public class HomeController : Controller
    {
        public string serectKey = "xUHfoPq35RGAHSJvuNc4AfR3YJ6RsTHG";
        ResponseService responseService = new ResponseService();
        TTPDatabaseDataContext db = new TTPDatabaseDataContext();
        HandleDatabase handleDatabase = new HandleDatabase();
        GetDatabase getDatabase = new GetDatabase();
        CryptoService crypto = new CryptoService();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult merchant()
        {
            IEnumerable<Merchant> merchants = db.Merchants;
            return View(merchants);
        }
        public ActionResult EDIT(string id)
        {
            Merchant merchant = getDatabase.GetMerchant(id);
            if (merchant == null)
            {
                return RedirectToAction("merchant", "Home");
            }
            else
            {
                return View(merchant);
            }
        }
        [HttpPost]
        public ActionResult EDIT(Merchant merchant)
        {
            Merchant merchant1 = db.Merchants.FirstOrDefault(x => x.shopId.Trim() == merchant.shopId.Trim());
            merchant1.Address = merchant.Address;
            //merchant1.serectKey = merchant.serectKey;
            merchant1.amount = merchant.amount;
            //merchant1.accessKey = merchant.accessKey;
            merchant1.notifyUrl = merchant.notifyUrl;
            merchant1.resultUrl =  merchant.resultUrl;

            db.SubmitChanges();
            return RedirectToAction("merchant", "Home");
        }
        public ActionResult pay()
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
        // Controller handle for Buyer payment with TTP
        [HttpPost]
        public ActionResult pay(FormCollection collection)
        {
            string una = collection["Username"];
            string  pwa = collection["Password"];
            string transId = collection["transId"];
            string amount = collection["amount"];
            string responseTime = crypto.DatetimeToTimestamp(DateTime.Now);
            // Create payment table
            Buyer buyer = getDatabase.GetBuyer(una);
            string paymentHash = "transId=" + transId +
                "&amount=" + amount +
                "&buyerId=" + buyer.buyerId +
                "&responseTime=" + responseTime;
                ;
            string paymentSignature = crypto.signSHA256(paymentHash, serectKey);
            handleDatabase.InsertPaymentRequest(transId, amount, buyer.buyerId, responseTime, paymentSignature);
            //Create Oder Payment Receipt


            //Checkin payment
            int checkin = handleDatabase.Checkin(una, pwa, collection["amount"].ToString());
            Merchant merchant = getDatabase.GetMerchantWithTransId(transId);
            if (checkin == 0)
            {
                handleDatabase.Payment(una, transId, collection["amount"]);
                string messageHash = "transId=" + collection["transId"] +
                "&amount=" + collection["amount"] +
                "&statusCode=0";
                string signature = crypto.signSHA256(messageHash, serectKey);

                // Object TPP send to Merchant
                JObject message = new JObject
                {
                    { "transId", collection["transId"] },
                    { "amount", collection["amount"] },
                    { "statusCode", "0" },
                    { "signature", signature }
                };
                //Create Oder Payment Receipt
                string odpayCHash = "transId=" + transId +
                "&amount=" + amount +
                "&statusCode=0" +
                "&responseTime=" + responseTime;
                ;
                string sigOdpayHash = crypto.signSHA256(odpayCHash, serectKey);
                handleDatabase.InsertOderPaymentReceipt(transId, amount, "0", responseTime, sigOdpayHash);
                // TTP Sends Merchant a Transaction Receipt 
                responseService.sendOrderPaymentReceipt(merchant.notifyUrl, message.ToString());
                // TTP Sends Buyer a Transaction Receipt 
                string resultUrl = merchant.resultUrl + "?" + messageHash + "&signature=" + signature; // params sends Buyer
                return Redirect(resultUrl);
            }
            else
            {
                string messageHash = "transId=" + collection["transId"] +
                "&amount=" + collection["amount"] +
                "&statusCode=1";
                string signature = crypto.signSHA256(messageHash, serectKey);

                JObject message = new JObject
                {
                    { "transId", collection["transId"] },
                    { "amount", collection["amount"] },
                    { "statusCode", "1" },
                    { "signature", signature }
                };
                //Create Oder Payment Receipt
                string odpayCHash = "transId=" + transId +
                "&amount=" + amount +
                "&statusCode=1" +
                "&responseTime=" + responseTime;
                ;
                string sigOdpayHash = crypto.signSHA256(odpayCHash, serectKey);
                handleDatabase.InsertOderPaymentReceipt(transId, amount, "1", responseTime, sigOdpayHash);
                // TTP Sends Merchant a Transaction Receipt 
                responseService.sendOrderPaymentReceipt(merchant.notifyUrl, message.ToString());
                // TTP Sends Buyer a Transaction Receipt 
                string resultUrl = merchant.resultUrl + "?" + messageHash + "&signature=" + signature;
                return Redirect(resultUrl);
            }
        }
        // Payment Infomation save on TTP SERVER
        public ActionResult payment()
        {
            string param = Request.QueryString.ToString();
            ViewBag.transId = param;
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}

using Newtonsoft.Json.Linq;
using SPP_APISERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SPP_APISERVER.Controllers
{
    public class ErrorController : ApiController
    {
        public string serectKey = "xUHfoPq35RGAHSJvuNc4AfR3YJ6RsTHG";
        ResponseService responseService = new ResponseService();
        TTPDatabaseDataContext db = new TTPDatabaseDataContext();
        HandleDatabase handleDatabase = new HandleDatabase();
        GetDatabase getDatabase = new GetDatabase();
        CryptoService crypto = new CryptoService();
        // GET: api/Error
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Error/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Error
        public IHttpActionResult Post(string transId, string amount)
        {
            string responseTime = crypto.DatetimeToTimestamp(DateTime.Now);
            Merchant merchant = getDatabase.GetMerchantWithTransId(transId);
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
            OrderPaymentReceipt oderPaymentReceipt = new OrderPaymentReceipt()
            {
                transId = transId,
                amount = amount,
                statusCode = "1",
                responseTime = responseTime,
                signature = signature
            };
            handleDatabase.InsertOderPaymentReceiptWithObj(oderPaymentReceipt);

            return Redirect(resultUrl);
        }

        // PUT: api/Error/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Error/5
        public void Delete(int id)
        {
        }
    }
}

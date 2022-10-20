using SPP_APISERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SPP_APISERVER.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        GetDatabase getDatabase = new GetDatabase();
        CryptoService crypto = new CryptoService();
        HandleDatabase handle = new HandleDatabase();
        private string serectKey = "xUHfoPq35RGAHSJvuNc4AfR3YJ6RsTHG";
        public IEnumerable<OderRequest> Get()
        {
            return getDatabase.GetOderRequest();
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public ConfirmRequest Post([FromBody] OderRequest oderRequest)
        {
            ConfirmRequest confirm = new ConfirmRequest()
            {
                transId = oderRequest.transId,
                payUrl = "",
                statusCode = "1",
                signature = ""
            };
            try
            {
                string signature = oderRequest.signature;
                string rawHash = "transId=" + oderRequest.transId +
                        "&amount=" + oderRequest.amount +
                        "&shopId=" + oderRequest.shopId +
                        "&oderInfo=" + oderRequest.oderInfo +
                        "&responseTime=" + oderRequest.responseTime
                        ;
                oderRequest.signature = crypto.signSHA256(rawHash, serectKey);
                if (oderRequest.signature == signature)
                {
                    bool checkOderPayment = handle.InsertOderRequestWithObj(oderRequest);
                    if (checkOderPayment)
                    {
                        string url3des = rawHash + "&signature=" + signature;
                        string rawHash1 = "transId=" + oderRequest.transId +
                        "&statusCode=0" +
                        "&payUrl=" + oderRequest.oderInfo
                        ;
                        string signature1 = crypto.signSHA256(rawHash1, serectKey);
                        handle.InsertConfirmOderRequest(oderRequest.transId, "0", "https://demopay.xyz/payment/checkpayment?payment=" + crypto.EncryptUri(url3des, serectKey), signature1);

                        confirm.payUrl = "https://demopay.xyz/payment/checkpayment?payment=" + crypto.EncryptUri(url3des, serectKey);
                        confirm.statusCode = "0";
                        confirm.signature = signature1;

                        return confirm;
                    }
                    else
                    {
                        string rawHash2 = "transId=" + oderRequest.transId +
                         "&statusCode=1" +
                         "&payUrl="
                         ;
                        string signature2 = crypto.signSHA256(rawHash2, serectKey);

                        confirm.signature = signature2;
                        return confirm;
                    }
                }
                else
                {
                    string rawHash2 = "transId=" + oderRequest.transId +
                        "&statusCode=1" +
                        "&payUrl="
                        ;
                    string signature2 = crypto.signSHA256(rawHash2, serectKey);

                    confirm.signature = signature2;
                    return confirm; ;
                }
            }
            catch (Exception) { return null; }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}

using Newtonsoft.Json;
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
    public class PayController : ApiController
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0);
        TTPDatabaseDataContext db = new TTPDatabaseDataContext();
        GetDatabase getDatabase = new GetDatabase();
        HandleDatabase handleDatabase = new HandleDatabase();
        public string cipher = "";
        public string serectKey = "xUHfoPq35RGAHSJvuNc4AfR3YJ6RsTHG";
        CryptoService crypto = new CryptoService();
        ResponseService responseService = new ResponseService();
        CryptoRSA rsa = new CryptoRSA();

        // GET: api/Pay
        //public IEnumerable<string> Get()
        //{
        //    string encrypt = crypto.EncryptUri("transId=aloo&amount=1000&shopId=MCID01&oderInfo=123&responseTime=1655139465&signature=3852bf4feb8630a78afba27f8b14143b650af46538fb6c04fc13334fc3a27a47", serectKey);
        //    return new string[] { encrypt, crypto.DecryptUri(encrypt, serectKey) };
        //}

        // GET: api/Pay/5
        public string Get()
        {
            return "OK";
        }
        [HttpPost]
        public bool Insert(OderRequest oderRequest)
        {
            string signature = oderRequest.signature;
            string rawHash = "transId=" + oderRequest.transId +
                    "&amount=" + oderRequest.amount +
                    "&shopId=" + oderRequest.shopId +
                    "&oderInfo=" + oderRequest.oderInfo +
                    "&responseTime=" + oderRequest.responseTime
                    ;
            oderRequest.signature = crypto.signSHA256(rawHash, serectKey);
            if(oderRequest.signature == signature)
            {
                //handleDatabase.InsertOderRequestWithObj(oderRequest);
                return true;
            }
            return false;
        }

        // POST: api/Pay
        public ConfirmRequest PostOrderRequest(OderRequest oderRequest)
        {
            try
            {
                ConfirmRequest confirm = new ConfirmRequest();
                string signature = oderRequest.signature;
                ConfirmOderRequest confirmOderRequest = new ConfirmOderRequest();
                string rawHash = "transId=" + oderRequest.transId +
                    "&amount=" + oderRequest.amount +
                    "&shopId=" + oderRequest.shopId +
                    "&oderInfo=" + oderRequest.oderInfo +
                    "&responseTime=" + oderRequest.responseTime
                    ;
                // Create URL Payment for buyer
                string signatureUri = crypto.signSHA256(rawHash, serectKey);
                string rawHashUri = rawHash + "&signature=" + signatureUri;
                // 3DES
                rawHashUri = crypto.DecryptUri(rawHashUri, serectKey);

                oderRequest.signature = crypto.signSHA256(rawHash, serectKey);
                if (oderRequest.signature == signature)
                {
                    // succsess case of signature verification
                    string url = "https://tomoca.click/Home/pay?payment=" + rawHashUri;
                    confirmOderRequest.transId = oderRequest.transId;
                    confirmOderRequest.payUrl = url;
                    confirmOderRequest.statusCode = "0";
                    string confirmHash = "transId=" + oderRequest.transId +
                    "&statusCode=0" +
                    "&payUrl=" + url;
                    confirmOderRequest.signature = crypto.signSHA256(confirmHash, serectKey);
                    // Create response for Merchant
                    confirm = handleDatabase.CreateConfirm(confirmOderRequest.transId, confirmOderRequest.statusCode, confirmOderRequest.payUrl);

                    // Insert Oder Request and Confirm payment to database
                    bool checkOderRequest = handleDatabase.InsertOderRequest(oderRequest.transId, oderRequest.amount, oderRequest.shopId, oderRequest.oderInfo, oderRequest.responseTime, oderRequest.signature);
                    bool confirmOder;

                    if (checkOderRequest)
                    {
                        confirmOder = handleDatabase.InsertConfirmOderRequestWithObj(confirmOderRequest);
                        if (confirmOder == false)
                        {
                            // Can't insert to database for Oder Request or Confirm payment 
                            confirm = handleDatabase.CreateConfirm(oderRequest.transId, "101", "");
                            return confirm;
                        }
                        // Insert succsess
                        return confirm;
                    }
                    else
                    {
                        confirm = handleDatabase.CreateConfirm(oderRequest.transId, "101", "");
                        return confirm;
                    }
                }
                else
                {
                    // faild case of signature verification
                    confirmOderRequest.transId = oderRequest.transId;
                    confirmOderRequest.payUrl = "";
                    confirmOderRequest.statusCode = "1";
                    string confirmHash = "transId=" + oderRequest.transId +
                    "&statusCode=1" + oderRequest.signature +
                    "&payUrl=";
                    confirmOderRequest.signature = crypto.signSHA256(confirmHash, serectKey);

                    confirm = handleDatabase.CreateConfirm(oderRequest.transId, confirmOderRequest.statusCode, "");

                    bool checkOderRequest = handleDatabase.InsertOderRequestWithObj(oderRequest);
                    bool confirmOder;

                    if (checkOderRequest)
                    {
                        confirmOder = handleDatabase.InsertConfirmOderRequestWithObj(confirmOderRequest);
                        if (confirmOder == false)
                        {
                            //Can't insert to database for Oder Request or Confirm payment 
                            confirm = handleDatabase.CreateConfirm(oderRequest.transId, "102", "");
                            return confirm;
                        }
                        //Insert succsess
                        return confirm;
                    }
                    else
                    {
                        confirm = handleDatabase.CreateConfirm(oderRequest.transId, "102", "");
                        return confirm;
                    }
                }
            }
            catch (Exception) { return null; }
        }
        public void Put([FromBody] OderRequest oderRequest)
        {
            db.OderRequests.InsertOnSubmit(oderRequest);
            db.SubmitChanges();
        }
    }
}

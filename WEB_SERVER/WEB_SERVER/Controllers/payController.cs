using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WEB_SERVER.Models;

namespace WEB_SERVER.Controllers
{
    public class payController : ApiController
    {
        // POST: api/pay
        Merchant_DBDataContext db = new Merchant_DBDataContext();
        CryptoService crypto = new CryptoService();
        public string serectKey = "xUHfoPq35RGAHSJvuNc4AfR3YJ6RsTHG";
        public string Post(OrderPaymentReceipt orderPaymentReceipt)
        {
            // Handle result payment...

            Payment pay = new Payment()
            {
                transId = orderPaymentReceipt.transId,
                statusCode = orderPaymentReceipt.statusCode,
                responseTime = orderPaymentReceipt.responseTime,
                signature = "",
            };
            string messageHash =
                        "transId=" + orderPaymentReceipt.transId +
                        "&amount=" + orderPaymentReceipt.amount +
                        "&statusCode=" + orderPaymentReceipt.statusCode +
                        "&responseTime=" + orderPaymentReceipt.responseTime
                    ;
            string signature = crypto.signSHA256(messageHash, serectKey);
            pay.signature = signature;
            
            if (signature == orderPaymentReceipt.signature)
            {
                pay.signature = orderPaymentReceipt.signature;
            }
            if(orderPaymentReceipt.statusCode == "0")
            {
                // handle succses payment 
                db.Payments.InsertOnSubmit(pay);
                db.SubmitChanges();
            }
            else
            {
                // handle faild payment
            }
            return "OK";
        }
    }
}

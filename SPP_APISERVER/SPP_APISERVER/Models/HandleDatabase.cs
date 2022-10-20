using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPP_APISERVER.Models
{
    public class HandleDatabase
    {
        readonly TTPDatabaseDataContext db = new TTPDatabaseDataContext();
        readonly CryptoService crypto = new CryptoService();
        private string serectKey = "xUHfoPq35RGAHSJvuNc4AfR3YJ6RsTHG";
        // Buyer checkin

        public int Checkin(string username, string password, string amount)
        {
            try
            {
                Buyer buyer = db.Buyers.FirstOrDefault(x => x.Username.Trim() == username);
                if (buyer == null)
                {
                    // Username not exist
                    return 1;
                }
                else
                {
                    if(buyer.Password.Trim() == password)
                    {
                        if (long.Parse(buyer.amount.Trim()) >= long.Parse(amount))
                        {
                            // Enough money
                            return 0;
                        }
                        else
                        {
                            // Not enough money
                            return -1;
                        }
                    }
                    else
                    {
                        // Wrong password
                        return 1;
                    }
                }
            }
            catch (Exception)
            {
                return -2;
            }
        }
        public bool Payment(string username, string transId, string amount)
        {
            try
            {
                Buyer buyer = db.Buyers.FirstOrDefault(x => x.Username.Trim() == username);
                OderRequest oreq = db.OderRequests.FirstOrDefault(x => x.transId.Trim() == transId);
                Merchant merchant = db.Merchants.FirstOrDefault(x => x.shopId.Trim() == oreq.shopId);
                buyer.amount = (long.Parse(buyer.amount.Trim()) - long.Parse(amount)).ToString();
                db.SubmitChanges();
                merchant.amount = (long.Parse(merchant.amount.Trim()) + long.Parse(amount)).ToString();
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        // EDIT
        public bool EditMerchant(string id, string amount, string resultUrl, string notifyUrl)
        {
            try
            {
                Merchant merchant = db.Merchants.FirstOrDefault(x => x.shopId.Trim() == id);
                merchant.amount = amount;
                merchant.resultUrl = resultUrl;
                merchant.notifyUrl = notifyUrl;
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        // Insert with object
        public bool InsertOderRequestWithObj(OderRequest oderRequest)
        {
            try
            {
                db.OderRequests.InsertOnSubmit(oderRequest);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertConfirmOderRequestWithObj(ConfirmOderRequest confirmOderRequest)
        {
            try
            {
                db.ConfirmOderRequests.InsertOnSubmit(confirmOderRequest);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertPaymentRequestWithObj(PaymentRequest paymentRequest)
        {
            try
            {
                db.PaymentRequests.InsertOnSubmit(paymentRequest);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertOderPaymentReceiptWithObj(OrderPaymentReceipt order)
        {
            try
            {
                db.OrderPaymentReceipts.InsertOnSubmit(order);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Insert with params
        public bool InsertOderRequest(string transId, string amount, string shopId, string oderInfo, string responseTime, string signature)
        {
            try
            {
                OderRequest obj = new OderRequest
                {
                    transId = transId,
                    amount = amount,
                    shopId = shopId,
                    oderInfo = oderInfo,
                    responseTime = responseTime,
                    signature = signature
                };
                db.OderRequests.InsertOnSubmit(obj);
                db.SubmitChanges();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool InsertPaymentRequest(string transId, string amount, string buyerId, string responseTime, string signature)
        {
            try
            {
                PaymentRequest obj = new PaymentRequest();
                PaymentRequest checkId = db.PaymentRequests.LastOrDefault();
                if (checkId == null)
                {
                    obj.paymentId = 1;
                    obj.transId = transId;
                    obj.amount = amount;
                    obj.responseTime = responseTime;
                    obj.buyerId = buyerId;
                    obj.signature = signature;
                    db.PaymentRequests.InsertOnSubmit(obj);
                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    int getId = checkId.paymentId++;
                    obj.paymentId = getId;
                    obj.transId = transId;
                    obj.amount = amount;
                    obj.responseTime = responseTime;
                    obj.buyerId = buyerId;
                    obj.signature = signature;
                    db.PaymentRequests.InsertOnSubmit(obj);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertConfirmOderRequest(string transId, string statusCode, string payUrl, string signature)
        {
            try
            {
                ConfirmOderRequest obj = new ConfirmOderRequest
                {
                    transId = transId,
                    statusCode = statusCode,
                    payUrl = payUrl,
                    signature = signature
                };
                db.ConfirmOderRequests.InsertOnSubmit(obj);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertOderPaymentReceipt(string transId, string amount, string statusCode, string responseTime, string signature)
        {
            try
            {
                OrderPaymentReceipt obj = new OrderPaymentReceipt
                {
                    transId = transId,
                    amount = amount,
                    responseTime = responseTime,
                    statusCode = statusCode,
                    signature = signature
                };
                db.OrderPaymentReceipts.InsertOnSubmit(obj);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteTransaction(string transId)
        {
            try
            {
                ConfirmOderRequest cor = db.ConfirmOderRequests.FirstOrDefault(x => x.transId == transId);
                OrderPaymentReceipt opr = db.OrderPaymentReceipts.FirstOrDefault(x => x.transId == transId);
                PaymentRequest pmr = db.PaymentRequests.FirstOrDefault(x => x.transId == transId);
                OderRequest odr = db.OderRequests.FirstOrDefault(x => x.transId == transId);
                if(cor != null)
                    db.ConfirmOderRequests.DeleteOnSubmit(cor);
                if (opr != null)
                    db.OrderPaymentReceipts.DeleteOnSubmit(opr);
                if (pmr !=null)
                    db.PaymentRequests.DeleteOnSubmit(pmr);
                if (odr != null)
                    db.OderRequests.DeleteOnSubmit(odr);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        // Create
        public ConfirmRequest CreateConfirm(string transId, string status, string payUrl)
        {
            string confirmHash = "transId=" + transId +
                "&statusCode="+ status +
                "&payUrl=" + payUrl;
            string hash = crypto.signSHA256(confirmHash, serectKey);
            ConfirmRequest confirm = new ConfirmRequest
            {
                transId = transId,
                statusCode = status,
                payUrl = payUrl,
                signature = hash
            };
            return confirm;
        }
    }
}
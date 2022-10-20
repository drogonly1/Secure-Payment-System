using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB_SERVER.Models
{
    public class HandleDatabase
    {
        Merchant_DBDataContext db = new Merchant_DBDataContext();
        IEnumerable<Product> products;
        IEnumerable<OderRequest> oderRequests;
        IEnumerable<Payment> payments;
        // INSERT DATA
        public bool PayProduct(string productId, int count)
        {
            try
            {
                Product product = GetProduct(productId);
                product.Count = product.Count - count;
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertProduc(string productId, string Name, string Amount, int Count, string image)
        {
            try
            {
                Product product = new Product()
                {
                    productId = productId,
                    Name = Name,
                    Amount = Amount,
                    Count = Count,
                    image = image
                };
                db.Products.InsertOnSubmit(product);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertOderRequest(string transId, string amount, string shopId, string oderInfo, string responseTime, string signature, string producId)
        {
            try
            {
                OderRequest oderRequest = new OderRequest()
                {
                    transId = transId,
                    amount = amount,
                    shopId = shopId,
                    oderInfo = oderInfo,
                    responseTime = responseTime,
                    signature = signature,
                    productId = producId
                };
                db.OderRequests.InsertOnSubmit(oderRequest);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertPayment(string transId, string statusCode, string responseTime, string signature)
        {
            try
            {
                Payment payment = new Payment() { 
                    transId = transId,
                    statusCode = statusCode,
                    responseTime = responseTime,
                    signature = signature
                };
                db.Payments.InsertOnSubmit(payment);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertPayUrl(string transId, string payUrl)
        {
            try
            {
                GetUrlPayment getUrl = new GetUrlPayment()
                {
                    transId = transId,
                    payUrl = payUrl
                };
                db.GetUrlPayments.InsertOnSubmit(getUrl);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        // GET ONE
        public Product GetProduct(string productId)
        {
            Product product = db.Products.FirstOrDefault(x => x.productId.Trim() == productId);
            return product;
        }
        // GET DATAS
        public IEnumerable<Product> GetProducts()
        {
            try
            {
                products = db.Products.ToList();
                return products;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public IEnumerable<OderRequest> GetOderRequests()
        {
            try
            {
                oderRequests = db.OderRequests.ToList();
                return oderRequests;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public IEnumerable<Payment> GetPayments()
        {
            try
            {
                payments = db.Payments.ToList();
                return payments;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
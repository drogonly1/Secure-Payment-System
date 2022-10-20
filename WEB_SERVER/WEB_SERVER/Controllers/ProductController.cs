using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_SERVER.Models;

namespace WEB_SERVER.Controllers
{
    public class ProductController : Controller
    {
        HandleDatabase handle = new HandleDatabase();
        public string serectKey = "xUHfoPq35RGAHSJvuNc4AfR3YJ6RsTHG";
        CryptoService crypto = new CryptoService();
        // GET: Product
        public ActionResult item()
        {
            IEnumerable<Product> products = handle.GetProducts();
            return View(products);
        }
        public ActionResult payment(string id)
        {
            if (Request.QueryString.ToString() == null)
            {
                return RedirectToAction("item", "Product");
            }
            else
            {
                if (Request.QueryString["id"] == null)
                {
                    return RedirectToAction("item", "Product");
                }
                else
                {
                    Product product = handle.GetProduct(id);
                    if (product == null)
                    {
                        return RedirectToAction("item", "Product");
                    }
                    else
                    {
                        ViewBag.productId = id;
                        return View(product);
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult paymentInfo(FormCollection formCollection)
        {
            string url = "https://demopay.xyz/api/values";
            //string url = "https://localhost:44327/api/values";
            string time = crypto.DatetimeToTimestamp(DateTime.Now);
            string transId = formCollection["transId"];
            string amount = formCollection["amount"];
            string productId = formCollection["productId"];
            Product product = handle.GetProduct(productId);

            try
            {
                string rawHash = "transId=" + transId +
                "&amount=" + product.Amount.Trim() +
                "&shopId=" + "MCID01" +
                "&oderInfo=" + product.Name.Trim() + "-SL1" +
                "&responseTime=" + time
                ;
                string signature = crypto.signSHA256(rawHash, serectKey);
                handle.PayProduct(productId, 1);
                bool check = handle.InsertOderRequest(transId, product.Amount, "MCID01", product.Name + "-SL1", time, signature, productId);
                JObject message = new JObject
                {
                    { "transId", transId },
                    { "amount", product.Amount.Trim() },
                    { "shopId", "MCID01" },
                    { "oderInfo", product.Name.Trim()+"-SL1" },
                    { "responseTime", time },
                    { "signature", signature }
                };

                string responseFromTPP = PMR.sendPaymentRequest(url, message.ToString());
                JObject jmessage = JObject.Parse(responseFromTPP);
                if (jmessage.GetValue("payUrl").ToString() == "")
                {
                    return Redirect("https://localhost:44337/Product/payment?id=" + product.productId);
                }
                else
                {
                    handle.InsertPayUrl(transId, jmessage.GetValue("payUrl").ToString());
                    return Redirect(jmessage.GetValue("payUrl").ToString());
                }
            }
            catch (Exception)
            {
                return Redirect("https://localhost:44337/Product/payment?id=" + product.productId);
            }
        }
    }
}
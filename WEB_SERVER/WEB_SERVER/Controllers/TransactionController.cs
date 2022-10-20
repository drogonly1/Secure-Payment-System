using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_SERVER.Models;

namespace WEB_SERVER.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        HandleDatabase handle = new HandleDatabase();
        CryptoService crypto = new CryptoService();
        public string serectKey = "xUHfoPq35RGAHSJvuNc4AfR3YJ6RsTHG";
        public ActionResult Index()
        {
            var transactions = from odr in handle.GetOderRequests()
                                       join pay in handle.GetPayments() on odr.transId equals pay.transId
                                       select new Transaction
                                       {
                                           OfferRequest = odr,
                                           Payment = pay
                                       };
            return View(transactions);
        }
        public ActionResult resultPayment()
        {
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            string signature = crypto.signSHA256(param, serectKey);
            if (signature != Request.QueryString["signature"])
            {
                ViewBag.message = "Invalid Request";
                return View();
            }
            if (Request["statusCode"] == "0")
            {
                ViewBag.message = "Payment Success";
            }
            else
            {
                ViewBag.message = "Payment Failed";
            }
            return View();
        }
    }
}
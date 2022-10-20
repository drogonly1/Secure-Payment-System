using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPP_APISERVER.Models;

namespace SPP_APISERVER.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        GetDatabase getDatabase = new GetDatabase();
        public ActionResult Index()
        {
            return View();
        }

        // GET: Transaction/Details/5
        
        public ActionResult GetTransactions()
        {
            IEnumerable<InfoTransaction> transactions = getDatabase.GetTransaction().ToList();
            return View(transactions);
        }
        public ActionResult GetOderRequests()
        {
            IEnumerable<OderRequest> oderRequests = getDatabase.GetOderRequest().ToList();
            return View(oderRequests);
        }
        public ActionResult GetPaymentRequests()
        {
            IEnumerable<PaymentRequest> getPaymentRequests = getDatabase.GetPaymentRequest();
            return View(getPaymentRequests);
        }
        public ActionResult GetOderPaymentReceipts()
        {
            IEnumerable<OrderPaymentReceipt> getOderPaymentReceipts = getDatabase.GetoderPaymentReceipt();
            return View(getOderPaymentReceipts);
        }
        public ActionResult GetConfirmOderRequests()
        {
            IEnumerable<ConfirmOderRequest> getConfirmOderRequests = getDatabase.GetConfirmOderRequest();
            return View(getConfirmOderRequests);
        }
    }
}

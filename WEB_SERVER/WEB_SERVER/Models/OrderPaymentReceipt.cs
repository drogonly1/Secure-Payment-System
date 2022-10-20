using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB_SERVER.Models
{
    public class OrderPaymentReceipt
    {
        public string transId { get; set; }
        public string amount { get; set; }
        public string statusCode { get; set; }
        public string responseTime { get; set; }
        public string signature { get; set; }
    }
}
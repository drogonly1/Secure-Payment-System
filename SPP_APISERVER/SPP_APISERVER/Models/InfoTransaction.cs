using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPP_APISERVER.Models
{
    public class InfoTransaction
    {
        public OderRequest OderRequest { get; set; }
        public PaymentRequest PaymentRequest { get; set; }
        public OrderPaymentReceipt OderPaymentReceipt { get; set; }
    }
}
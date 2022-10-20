using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB_SERVER.Models
{
    public class Transaction
    {
        public OderRequest OfferRequest { get; set; }
        public Payment Payment { get; set; }
    }
}
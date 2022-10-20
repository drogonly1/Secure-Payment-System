using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPP_APISERVER.Models
{
    public class ConfirmRequest
    {
        public string transId { get; set; }
        public string statusCode { get; set; }
        public string payUrl { get; set; }
        public string signature { get; set; }
    }
}
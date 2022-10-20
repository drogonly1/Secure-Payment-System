using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPP_APISERVER.Models
{
    public class UserPayment
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public long amount { get; set; }
    }
}
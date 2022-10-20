using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPP_APISERVER.Models
{
    public class GetDatabase
    {
        readonly TTPDatabaseDataContext db = new TTPDatabaseDataContext();
        OderRequest oderRequest;
        Merchant merchant;
        Buyer buyer;
        List<OderRequest> OderRequests;
        List<PaymentRequest> PaymentRequests;
        List<ConfirmOderRequest> ConfirmOderRequests;
        List<OrderPaymentReceipt> oderPaymentReceipts;
        IEnumerable<InfoTransaction> infoTransactions;
        public IEnumerable<OderRequest> GetOderRequest()
        {
            OderRequests = db.OderRequests.ToList();
            return OderRequests;
        }
        public IEnumerable<PaymentRequest> GetPaymentRequest()
        {
            PaymentRequests = db.PaymentRequests.ToList();
            return PaymentRequests;
        }
        public IEnumerable<ConfirmOderRequest> GetConfirmOderRequest()
        {
            ConfirmOderRequests = db.ConfirmOderRequests.ToList();
            return ConfirmOderRequests;
        }
        public IEnumerable<OrderPaymentReceipt> GetoderPaymentReceipt()
        {
            oderPaymentReceipts = db.OrderPaymentReceipts.ToList();
            return oderPaymentReceipts;
        }
        public IEnumerable<InfoTransaction> GetTransaction()
        {
            infoTransactions =from odr in GetOderRequest()
                               join pmr in GetPaymentRequest() on odr.transId equals pmr.transId into odr_pmr
                               from pmr in odr_pmr
                               join cor in GetoderPaymentReceipt() on pmr.transId equals cor.transId into pmr_cor
                               from cor in pmr_cor
                               //join cor in GetoderPaymentReceipt() on op.transId equals cor.transId
                               select new InfoTransaction
                                 {
                                    OderRequest = odr,
                                    PaymentRequest = pmr,
                                    OderPaymentReceipt = cor
                                 };
            return infoTransactions;
        }
        // Take one object
        public Buyer GetBuyer(string Username)
        {
            return db.Buyers.FirstOrDefault(x => x.Username.Trim() == Username.Trim());
        }
        public OderRequest GetOderRequest(string transId)
        {
            oderRequest = db.OderRequests.FirstOrDefault(x => x.transId == transId);
            return oderRequest;
        }
        public Merchant GetMerchantWithTransId(string transId)
        {
            try
            {
                OderRequest oder = GetOderRequest(transId);
                merchant = db.Merchants.FirstOrDefault(x => x.shopId.Trim() == oder.shopId.Trim());
                return merchant;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Merchant GetMerchant(string shopId)
        {
            try
            {
                merchant = db.Merchants.FirstOrDefault(x => x.shopId.Trim() == shopId.Trim());
                return merchant;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
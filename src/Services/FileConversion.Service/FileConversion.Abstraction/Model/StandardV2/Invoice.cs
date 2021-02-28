using System;

namespace FileConversion.Abstraction.Model.StandardV2
{
    public class Invoice
    {
        public VendorPayment VendorPayment {get;set;}
        public string VoucherNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public decimal? GrossAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? PaidNetAmount { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace FileConversion.Abstraction.Model.StandardV2
{
    public class VendorPayment : IStandardModel
    {
        public string Number { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Amount { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public Address Address { get; set; }
        
        #region Flatten Address
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressCity { get; set; }
        public string AddressStateProvince { get; set; }
        public string AddressCountry { get; set; }
        public string AddressZipCode { get; set; }
        #endregion

        public string[] FreeFormAddressLines { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }
    }
}

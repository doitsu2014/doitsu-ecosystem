using System.Collections.Generic;

namespace FileConversion.Abstraction.Model.StandardV2
{
    public class PackagingDocument : IStandardModel
    {
        public string ShippingCode { get; set; }
        public string OrderCode { get; set; }
        public string RawProductInformation { get; set; }
        public List<PackagingProductDocument> Products { get; set; }
        public string Feedback { get; set; }
        public string Note { get; set; }
    }
}
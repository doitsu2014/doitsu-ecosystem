using System.Collections.Generic;

namespace Blazor.FileConversion.Client.Models
{
    public class PackagingDocument
    {
        public string ShippingCode { get; set; }
        public string OrderCode { get; set; }
        public List<PackagingProductDocument> Products { get; set; }
        public string Feedback { get; set; }
        public string Note { get; set; }
    }
}
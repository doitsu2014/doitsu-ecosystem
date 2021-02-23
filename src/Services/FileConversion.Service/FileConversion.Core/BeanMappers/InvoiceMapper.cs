using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface;
using Optional;
using System.Collections.Generic;
using System.Linq;

namespace FileConversion.Core.BeanMappers
{
    public class InvoiceMapper : IBeanMapper
    {
        public Option<IEnumerable<object>, string> Map(IEnumerable<object> data)
        {
            return data.SomeNotNull().WithException("Data is null")
                .Map(d => d.Cast<Invoice>())
                .Map(d => d.GroupBy(x => new { x.VendorPayment.VendorId, x.VendorPayment.Number })
                    .Select(x =>
                    {
                        var vendorPayment = x.FirstOrDefault().VendorPayment;
                        vendorPayment.Invoices = x.ToList();
                        return vendorPayment;
                    })
                    .Cast<object>()
                );
        }
    }
}
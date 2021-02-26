using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface;
using System.Collections.Generic;
using LanguageExt;
using System.Linq;
using Shared.Abstraction.Models.Types;
using static Shared.Validations.GenericValidator;

namespace FileConversion.Core.BeanMappers
{
    public class InvoiceMapper : IBeanMapper
    {
        public Either<Error, IEnumerable<object>> Map(IEnumerable<object> data)
        {
            return ShouldNotNull(data)
                .Map(d => d.Cast<Invoice>()
                    .GroupBy(d => new {d.VendorPayment.VendorId, d.VendorPayment.Number})
                    .Select(g =>
                    {
                        var vendorPayment = g.FirstOrDefault().VendorPayment;
                        vendorPayment.Invoices = g.ToList();
                        return vendorPayment;
                    })
                    .Cast<object>());
        }
    }
}
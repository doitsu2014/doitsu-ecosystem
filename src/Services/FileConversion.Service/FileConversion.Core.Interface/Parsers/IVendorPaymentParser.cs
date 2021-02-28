using FileConversion.Abstraction.Model.StandardV2;

namespace FileConversion.Core.Interface.Parsers
{
    public interface IVendorPaymentParser : IParser<VendorPayment> { }

    public interface ICustomVendorPaymentParser : IVendorPaymentParser, ICustomParser { }
}

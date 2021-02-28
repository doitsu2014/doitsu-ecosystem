using FileConversion.Abstraction.Model.StandardV2;

namespace FileConversion.Core.Interface.Parsers
{
    public interface IVendorMasterParser : IParser<VendorMaster> { }

    public interface ICustomVendorMasterParser : IVendorMasterParser, ICustomParser { }
}

using System.Collections.Generic;
using LanguageExt;
using Shared.Abstraction.Models.Types;

namespace FileConversion.Core.Interface
{
    public interface IDynamicBeanMapperService
    {
        Validation<Error, IEnumerable<object>> MapFromSource(IEnumerable<object> data, string sourceCode);
    }
}

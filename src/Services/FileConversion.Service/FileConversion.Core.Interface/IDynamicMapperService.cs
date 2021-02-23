using Optional;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileConversion.Core.Interface
{
    public interface IDynamicBeanMapperService
    {
        Option<IEnumerable<object>, string> MapFromSource(IEnumerable<object> data, string sourceCode);
    }
}

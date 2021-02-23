using Optional;
using System.Collections.Generic;

namespace FileConversion.Core.Interface
{
    public interface IBeanMapper
    {
        Option<IEnumerable<object>, string> Map(IEnumerable<object> data);
    }
}

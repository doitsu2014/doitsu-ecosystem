using Optional;
using System.Threading.Tasks;

namespace FileConversion.Core.Interface
{
    public interface IBeanMapperService
    {
        IBeanMapper GetBeanMapper(string className);
    }
}

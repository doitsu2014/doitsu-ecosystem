using ACOMSaaS.NetCore.EFCore.Abstractions.Interface;
using ACOMSaaS.NetCore.EFCore.Abstractions.Model;

namespace FileConversion.Data.Services
{
    public interface IFileConversionEntityService<T> : IEntityService<FileConversionContext, T> where T : Entity
    {
    }
}

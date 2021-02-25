using Shared.EntityFrameworkCore;
using Shared.EntityFrameworkCore.Interfaces;

namespace FileConversion.Core.Interface
{
    public interface IFileConversionRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class 
    {
        
    }
}
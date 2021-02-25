using FileConversion.Core.Interface;
using Microsoft.Extensions.Logging;
using Shared.EntityFrameworkCore;

namespace FileConversion.Infrastructure.Repositories
{
    public class FileConversionRepository<TEntity> : BaseRepository<FileConversionContext, TEntity>, IFileConversionRepository<TEntity>
        where TEntity : class 
    {
        public FileConversionRepository(ILogger<FileConversionRepository<TEntity>> logger,
            FileConversionContext context) : base(logger, context)
        {
        }
    }
}
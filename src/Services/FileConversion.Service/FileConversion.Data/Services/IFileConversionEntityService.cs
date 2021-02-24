
namespace FileConversion.Data.Services
{
    public interface IFileConversionEntityService<T> : IEntityService<FileConversionContext, T> where T : Entity
    {
    }
}

namespace FileConversion.Data.Services
{
    public class FileConversionEntityService<T> : EntityService<FileConversionContext, T>, IFileConversionEntityService<T> where T : Entity
    {
        public FileConversionEntityService(FileConversionContext context) : base(context) { }
    }
}

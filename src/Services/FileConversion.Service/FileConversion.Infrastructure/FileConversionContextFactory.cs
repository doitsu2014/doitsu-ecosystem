using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FileConversion.Infrastructure
{
    public class FileConversionContextFactory : IDesignTimeDbContextFactory<FileConversionContext>
    {
        public FileConversionContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FileConversionContext>();
            optionsBuilder.UseNpgsql("Host=103.114.104.24;Database=test_db;Username=postgres;Password=zaQ@1234");
            return new FileConversionContext(optionsBuilder.Options);
        }
    }
}
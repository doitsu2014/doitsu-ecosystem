using System.Text;
using FileConversion.Abstraction.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileConversion.Infrastructure
{
    public class FileConversionContext : DbContext
    {
        public DbSet<InputMapping> InputMappings { get; set; }
        public DbSet<MapperSourceText> MapperSourceTexts { get; set; }
        public DbSet<OutputMapping> OutputMappings { get; set; }

        public FileConversionContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileConversionContext).Assembly);
        }
    }
}
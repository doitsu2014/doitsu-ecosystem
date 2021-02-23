using ACOMSaaS.NetCore.EFCore.Abstractions;
using ACOMSaaS.NetCore.EFCore.Abstractions.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FileConversion.Data
{
    public class FileConversionContext : BaseDbContext
    {
        private ILogger<FileConversionContext> _logger;
        protected ILogger<FileConversionContext> Logger
        {
            get
            {
                if (_logger == null)
                {
                    var loggerFactory = this.GetInfrastructure().GetService<ILoggerFactory>();
                    _logger = loggerFactory.CreateLogger<FileConversionContext>();
                }

                return _logger;
            }
        }

        public FileConversionContext(DbContextOptions options, IEnumerable<IEntityChangeHandler> handlers) : base(options, handlers)
        {
        }
    }
}

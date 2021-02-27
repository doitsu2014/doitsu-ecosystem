using FileConversion.Core.BeanMappers;
using FileConversion.Core.Interface;
using FileConversion.Core.Interface.Parsers;
using FileConversion.Core.LoaderServices;
using FileConversion.Core.Parsers;
using FileConversion.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FileConversion.Core
{
    public static class Extensions
    {
        public static IServiceCollection RegisterDefaultParserDependencies(this IServiceCollection services)
        {
            services.AddScoped<IBeanMapperService, BeanMapperService>();
            services.AddScoped<IBeanMapper, InvoiceMapper>();
            services.AddScoped<IBeanMapper, PackagingProductDocumentMapper>();
            services.AddSingleton<ITransformer, ExcelTransformer>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IParserFactory, ParserFactory>();
            services.AddScoped<ICustomParser, AccentWireVendorPaymentParser>();
            return services;
        }
    }
}

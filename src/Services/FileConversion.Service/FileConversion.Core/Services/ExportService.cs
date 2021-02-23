using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeanIO;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface;
using FileConversion.Data.Services;
using Microsoft.EntityFrameworkCore;
using Optional;
using Optional.Async;

namespace FileConversion.Core.Services
{
    public class ExportService : IExportService
    {
        private readonly IFileConversionEntityService<OutputMapping> _outputMappingEntityService;
        public ExportService(IFileConversionEntityService<OutputMapping> outputMappingEntityService)
        {
            _outputMappingEntityService = outputMappingEntityService;
        }

        /// <summary>
        /// 1. Resolve a converter by mapping ID, to convert data => PaymentOutputModel
        /// 2. Standardized code to convert PaymentOutputModel to byte[] in here, in standard format in wiki
        /// </summary>
        /// <param name="mappingId">mapping id</param>
        /// <param name="data">list mapping object</param>
        /// <returns></returns>
        public async Task<Option<byte[], string>> ExportAsync(string inputType, ImmutableList<object> data)
        {
            return (await (inputType, data)
                .Some()
                .WithException(string.Empty)
                .FlatMapAsync(async d =>
                {
                    if (!Enum.TryParse(d.inputType, true, out InputType inputTypeEnum) || inputTypeEnum == InputType.NotSupported)
                    {
                        return Option.None<string, string>(
                            $"Not supported input type: {inputTypeEnum.ToString()}");
                    }

                    var outputMapping = await _outputMappingEntityService.Set().FirstOrDefaultAsync(x=> x.Key == inputTypeEnum);
                    if (outputMapping == null)
                    {
                        return Option.None<string, string>(
                          $"Not supported output mapping: {inputTypeEnum.ToString()}");
                    }

                    using (var xmlConfigStream = outputMapping.XmlConfiguration.ToStream())
                    {
                        var factory = StreamFactory.NewInstance();
                        factory.Load(xmlConfigStream);

                        var text = new StringWriter();
                        var writer = factory.CreateWriter(Constants.DefaultExportStream, text);

                        // write header default values
                        for (var headerI = 0; headerI < outputMapping.NumberOfHeader; ++headerI)
                        {
                            writer.Write(Constants.DefaultExportHeaderRecord + headerI.ToString(), null);
                        }

                        foreach (var item in d.data)
                        {
                            writer.Write(item);
                        }

                        // write footer default values
                        for (var footerI = 0; footerI < outputMapping.NumberOfFooter; ++footerI)
                        {
                            writer.Write(Constants.DefaultExportFooterRecord + footerI.ToString(), null);
                        }

                        writer.Flush();
                        return Option.Some<string, string>(text.ToString());
                    }
                }))
                .Map(d =>
                {
                    return Encoding.UTF8.GetBytes(d);
                });
        }

    }
}

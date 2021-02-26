using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BeanIO;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using FileConversion.Core.Interface;
using LanguageExt;
using Shared.Abstraction.Models.Types;
using static LanguageExt.Prelude;
using static Shared.Validations.GenericValidator;
using static Shared.Validations.StringValidator;

namespace FileConversion.Core.Services
{
    public class ExportService : IExportService
    {
        private readonly IFileConversionRepository<OutputMapping> _outputMappingRepository;

        public ExportService(IFileConversionRepository<OutputMapping> outputMappingRepository)
        {
            _outputMappingRepository = outputMappingRepository;
        }

        /// <summary>
        /// 1. Resolve a converter by mapping ID, to convert data => PaymentOutputModel
        /// 2. Standardized code to convert PaymentOutputModel to byte[] in here, in standard format in wiki
        /// </summary>
        /// <param name="mappingId">mapping id</param>
        /// <param name="data">list mapping object</param>
        /// <returns></returns>
        public async Task<Either<Error, byte[]>> ExportAsync(string inputType, ImmutableList<object> data)
        {
            return await (ShouldNotNullOrEmpty(inputType), ShouldNotNullOrEmpty(data))
                .Apply((inpt, d) => (inpt, d))
                .MatchAsync(async d =>
                {
                    if (!Enum.TryParse(d.inpt, true, out InputType inputTypeEnum) ||
                        inputTypeEnum == InputType.NotSupported)
                    {
                        return Left<Error, byte[]>(
                            $"Not supported input type: {inputTypeEnum.ToString()}");
                    }
                    return (await _outputMappingRepository.GetAsync(inputTypeEnum))
                        .Match(outputMapping =>
                        {
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

                                foreach (var item in d.d)
                                {
                                    writer.Write(item);
                                }

                                // write footer default values
                                for (var footerI = 0; footerI < outputMapping.NumberOfFooter; ++footerI)
                                {
                                    writer.Write(Constants.DefaultExportFooterRecord + footerI.ToString(), null);
                                }

                                writer.Flush();

                                var result = Encoding.UTF8.GetBytes(text.ToString());
                                return Right<Error, byte[]>(result);
                            }
                        }, () => Left<Error, byte[]>($"Not supported output mapping: {inputTypeEnum.ToString()}"));
                }, errors => errors.Join());
        }
    }
}
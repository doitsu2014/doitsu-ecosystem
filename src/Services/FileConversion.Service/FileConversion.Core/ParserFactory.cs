using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface;
using FileConversion.Core.Interface.Parsers;
using FileConversion.Core.Parsers;
using System;
using LanguageExt;
using Shared.Abstraction.Models.Types;
using static Shared.Validations.GenericValidator;
using static Shared.Validations.StringValidator;
using static LanguageExt.Prelude;

namespace FileConversion.Core
{
    public class ParserFactory : IParserFactory
    {
        private readonly IEnumerable<ICustomParser> _customParsers;
        private readonly IEnumerable<ITransformer> _fileLoaderServices;
        private readonly IFileConversionRepository<InputMapping> _inputMappingRepository;
        private readonly IFileConversionRepository<MapperSourceText> _mstEntityRepository;
        private readonly IBeanMapperService _beanMapperFactory;

        public ParserFactory(IEnumerable<ICustomParser> customParsers,
            IEnumerable<ITransformer> fileLoaderServices,
            IFileConversionRepository<InputMapping> inputMappingRepository,
            IFileConversionRepository<MapperSourceText> mstEntityRepository,
            IBeanMapperService beanMapperFactory)
        {
            _customParsers = customParsers;
            _fileLoaderServices = fileLoaderServices;
            _inputMappingRepository = inputMappingRepository;
            _mstEntityRepository = mstEntityRepository;
            _beanMapperFactory = beanMapperFactory;
        }

        private InputType MapInputType<T>() where T : IStandardModel
        {
            var type = typeof(T);
            if (type == typeof(VendorPayment))
                return InputType.VendorPayment;
            else if (type == typeof(VendorMaster))
                return InputType.VendorMaster;
            else if (type == typeof(PosPay))
                return InputType.PosPay;

            throw new NotSupportedException($"Invalid model type: {type.FullName}");
        }

        public async Task<Validation<Error, IParser<T>>> GetParserAsync<T>(string key) where T : IStandardModel
        {
            return await ShouldNotNullOrEmpty(key)
                .MatchAsync(async k =>
                {
                    // Get custom parser
                    var parser = _customParsers?.OfType<ICustomParser<T>>().FirstOrDefault(p => p.Key == k);
                    if (parser != null)
                    {
                        return Success<Error, IParser<T>>(parser);
                    }
                    else
                    {
                        var inputType = MapInputType<T>();
                        // Get beanio parser
                        var inputMapping = (await _inputMappingRepository
                                .ListAsync(q => q.Where(e => e.Key == k && e.InputType == inputType)))
                            .FirstOrDefault();

                        if (inputMapping == null)
                            return Fail<Error, IParser<T>>($"No mapping by key: {k} & type: {inputType.ToString()}");
                        else if (inputMapping.XmlConfiguration.IsNullOrEmpty())
                            return Fail<Error, IParser<T>>(
                                $"No xml configuration by key: {k} & type: {inputType.ToString()}");

                        return (await _mstEntityRepository.GetAsync(inputMapping.MapperSourceTextId))
                            .Match(mapperSrcText => Success<Error, IParser<T>>(new BeanParser<T>(
                                    inputMapping.XmlConfiguration,
                                    _fileLoaderServices.SingleOrDefault(fl => fl.Type == inputMapping.StreamType),
                                    _beanMapperFactory.GetBeanMapper(inputMapping.Mapper),
                                    mapperSrcText)),
                                () => Fail<Error, IParser<T>>(
                                    $"Mapper source text '{inputMapping.MapperSourceTextId}' does not exist."));
                    }
                }, errors => errors.Join());
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Data.Services;
using FileConversion.Core.Interface;
using FileConversion.Core.Interface.Parsers;
using FileConversion.Core.Parsers;
using Optional;
using Optional.Async;
using System;
using Microsoft.EntityFrameworkCore;

namespace FileConversion.Core
{
    public class ParserFactory : IParserFactory
    {
        private readonly IEnumerable<ICustomParser> _customParsers;
        private readonly IEnumerable<ITransformer> _fileLoaderServices;
        private readonly IFileConversionEntityService<InputMapping> _inputMappingEntityService;
        private readonly IFileConversionEntityService<MapperSourceText> _mstEntityService;
        private readonly IBeanMapperService _beanMapperFactory;

        public ParserFactory(IEnumerable<ICustomParser> customParsers,
            IEnumerable<ITransformer> fileLoaderServices,
            IFileConversionEntityService<InputMapping> inputMappingEntityService,
            IFileConversionEntityService<MapperSourceText> mstEntityService,
            IBeanMapperService beanMapperFactory)
        {
            _customParsers = customParsers;
            _fileLoaderServices = fileLoaderServices;
            _inputMappingEntityService = inputMappingEntityService;
            _mstEntityService = mstEntityService;
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

        public async Task<Option<IParser<T>, string>> GetParserAsync<T>(string key) where T : IStandardModel
        {
            return await key.SomeNotNull().WithException("Key is null")
                .FlatMapAsync(async k =>
                {
                    // Get customer parser
                    var parser = _customParsers?.OfType<ICustomParser<T>>().FirstOrDefault(p => p.Key == key);
                    if (parser != null)
                        return Option.Some<IParser<T>, string>(parser);

                    // Get beanio parser
                    var inputType = MapInputType<T>();
                    var inputMapping = await 
                        _inputMappingEntityService
                        .Set().FirstOrDefaultAsync(x => x.Key == key && x.InputType == inputType);

                    if (inputMapping == null)
                        return Option.None<IParser<T>, string>($"No mapping by key: {k} & type: {inputType.ToString()}");
                    else if (inputMapping.XmlConfiguration.IsNullOrEmpty())
                        return Option.None<IParser<T>, string>($"No xml configuration by key: {k} & type: {inputType.ToString()}");

                    var mapperSrcText = await _mstEntityService.Set().FirstOrDefaultAsync(x=> x.Id == inputMapping.MapperSourceTextId);

                    return Option.Some<IParser<T>, string>(new BeanParser<T>(
                        inputMapping.XmlConfiguration,
                        _fileLoaderServices.SingleOrDefault(fl => fl.Type == inputMapping.StreamType),
                        _beanMapperFactory.GetBeanMapper(inputMapping.Mapper),
                        mapperSrcText));;
                });
        }
    }
}

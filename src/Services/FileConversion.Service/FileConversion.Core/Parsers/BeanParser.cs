using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using ACOMSaaS.NetCore.Abstractions;
using BeanIO;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface;
using FileConversion.Core.Interface.Parsers;
using FileConversion.Core.Services;
using Optional;

namespace FileConversion.Core.Parsers
{
    public class BeanParser<T> : IParser<T> where T : IStandardModel
    {
        private readonly string _xmlConfiguration;
        private readonly ITransformer _fileLoaderService;
        private readonly IBeanMapper _beanMapper;
        private readonly MapperSourceText _mapperSrcText;

        public BeanParser(string xmlConfiguration, ITransformer fileLoaderService, IBeanMapper beanMapper, MapperSourceText mapperSrcText)
        {
            _xmlConfiguration = xmlConfiguration;
            _fileLoaderService = fileLoaderService;
            _beanMapper = beanMapper;
            _mapperSrcText = mapperSrcText;
        }

        private Option<ImmutableList<T>, string> CastToExpectedType(IEnumerable<object> src)
        {
            if (!src.All(mE => mE.GetType() == typeof(T)))
            {
                var firstEle = src.FirstOrDefault();
                var expectedType = firstEle != null ? firstEle.GetType().ToString() : "null";
                return Option
                     .None<ImmutableList<T>, string>($"{typeof(T).GetType().ToString()} expected, but got {expectedType}.");
            }
            var casted = src.Cast<T>().ToImmutableList();
            return Option.Some<ImmutableList<T>, string>(casted);
        }

        public Option<ImmutableList<T>, string> Parse(byte[] content)
        {
            return content
                .SomeNotNull()
                .WithException("Content is null")
                .FlatMap(c =>
                {
                    if (_fileLoaderService != null)
                        content = _fileLoaderService.Transform(content);

                    using (var xmlConfigStream = new MemoryStream(_xmlConfiguration.Map(Encoding.UTF8.GetBytes)))
                    using (var contentStream = new MemoryStream(content))
                    {
                        var factory = StreamFactory.NewInstance();
                        factory.Load(xmlConfigStream);
                        var reader = factory.CreateReader(Constants.DefaultImportStream, new StreamReader(contentStream));
                        var listResult = new List<object>();
                        object record = null;

                        while ((record = reader.Read()) != null)
                        {
                            var castedRecord = record;
                            listResult.Add(castedRecord);
                        }

                        if (_beanMapper != null)
                        {
                            return _beanMapper
                               .Map(listResult.Cast<object>())
                               .FlatMap(m => CastToExpectedType(m));
                        }
                        else if (_mapperSrcText != null && _mapperSrcText.SourceText.IsNotNullOrEmpty())
                        {
                            var dynamicMapperService = new DynamicBeanMapperService();
                            return dynamicMapperService
                                .MapFromSource(listResult.Cast<object>(), _mapperSrcText.SourceText)
                                .FlatMap(m => CastToExpectedType(m));
                        }
                        else
                        {
                            return Option.Some<ImmutableList<T>, string>(listResult.Cast<T>().ToImmutableList());
                        }
                    }
                });
        }
    }
}

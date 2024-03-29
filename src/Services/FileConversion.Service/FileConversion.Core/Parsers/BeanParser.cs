﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using BeanIO;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface;
using FileConversion.Core.Interface.Parsers;
using FileConversion.Core.Services;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.CodeAnalysis;
using Shared.Abstraction.Models.Types;
using static LanguageExt.Prelude;
using static Shared.Validations.GenericValidator;

namespace FileConversion.Core.Parsers
{
    public class BeanParser<T> : IParser<T> where T : IStandardModel
    {
        private readonly string _xmlConfiguration;
        private readonly ITransformer _fileLoaderService;
        private readonly IBeanMapper _beanMapper;
        private readonly MapperSourceText _mapperSrcText;

        public BeanParser(string xmlConfiguration, ITransformer fileLoaderService, IBeanMapper beanMapper,
            Option<MapperSourceText> mapperSrcText)
        {
            _xmlConfiguration = xmlConfiguration;
            _fileLoaderService = fileLoaderService;
            _beanMapper = beanMapper;
            if (mapperSrcText.IsSome)
                _mapperSrcText = mapperSrcText.ValueUnsafe();
        }

        private Either<Error, ImmutableList<T>> CastToExpectedType(IEnumerable<object> src)
        {
            if (!src.All(mE => mE.GetType() == typeof(T)))
            {
                var firstEle = src.FirstOrDefault();
                var expectedType = firstEle != null ? firstEle.GetType().ToString() : "null";
                return Left<Error, ImmutableList<T>>(
                    $"{typeof(T).GetType().ToString()} expected, but got {expectedType}.");
            }

            var casted = src.Cast<T>().ToImmutableList();
            return Right<Error, ImmutableList<T>>(casted);
        }

        public Either<Error, ImmutableList<T>> Parse(byte[] content)
        {
            return ShouldNotNullOrEmpty(content)
                .ToEither()
                .Match(c =>
                {
                    if (_fileLoaderService != null)
                        content = _fileLoaderService.Transform(content);

                    using (var xmlConfigStream = new MemoryStream(Encoding.UTF8.GetBytes(_xmlConfiguration)))
                    using (var contentStream = new MemoryStream(content))
                    {
                        var factory = StreamFactory.NewInstance();
                        factory.Load(xmlConfigStream);
                        var reader = factory.CreateReader(Constants.DefaultImportStream,
                            new StreamReader(contentStream));
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
                                .Match(CastToExpectedType, error => error);
                        }
                        else if (_mapperSrcText != null && _mapperSrcText.SourceText.IsNotNullOrEmpty())
                        {
                            var dynamicMapperService = new DynamicBeanMapperService();
                            return dynamicMapperService
                                .MapFromSource(listResult.Cast<object>(), _mapperSrcText.SourceText)
                                .Match(CastToExpectedType, error => error);
                        }
                        else
                        {
                            return Right<Error, ImmutableList<T>>(listResult.Cast<T>().ToImmutableList());
                        }
                    }
                }, errors => errors.Join());
        }
    }
}
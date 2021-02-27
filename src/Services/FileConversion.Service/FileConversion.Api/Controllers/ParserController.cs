using FileConversion.Abstraction;
using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Shared.Abstraction.Models.Types;
using Shared.Extensions;
using static LanguageExt.Prelude;
using static Shared.Validations.GenericValidator;
using static Shared.Validations.StringValidator;

namespace FileConversion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        private readonly IParserFactory _parserFactory;
        private readonly IExportService _exportService;

        public ParserController(IParserFactory parserFactory, IExportService exportService)
        {
            _parserFactory = parserFactory;
            _exportService = exportService;
        }

        [HttpPost("{key}/{inputType}")]
        [Authorize(PolicyConstants.PolicyFileConversionParse)]
        public async Task<IActionResult> Post([FromForm] IFormFile file, [FromRoute] string key,
            [FromRoute] string inputType)
        {
            async Task<Either<Error, ImmutableList<object>>> Parse<T>(string k, IFormFile f) where T : IStandardModel
            {
                return await (await _parserFactory.GetParserAsync<T>(k))
                    .Bind(p => p.Parse(LoadFileContent(f)))
                    .AsTask()
                    .MapT(p => p.Select(e => e as object).ToImmutableList());
            }

            return await (ShouldNotNull(file), ShouldNotNullOrEmpty(key), ShouldNotNullOrEmpty(inputType))
                .Apply((f, k, t) => (f, k, t))
                .MatchAsync(async d =>
                {
                    if (!Enum.TryParse(d.t, true, out InputType inputTypeEnum) ||
                        inputTypeEnum == InputType.NotSupported)
                    {
                        return Left<Error, string>(
                            $"Not supported input type: {inputTypeEnum.ToString()}");
                    }

                    var parsedResult = inputTypeEnum switch
                    {
                        InputType.VendorPayment => await Parse<VendorPayment>(d.k, d.f),
                        InputType.PosPay => await Parse<PosPay>(d.k, d.f),
                        InputType.VendorMaster => await Parse<VendorMaster>(d.k, d.f),
                        InputType.PackagingDocument => await Parse<PackagingDocument>(d.k, d.f),
                        _ => Left<Error, ImmutableList<object>>(
                            $"Not supported input type: {inputTypeEnum.ToString()}"),
                    };

                    return await parsedResult
                        .MatchAsync(async pd => (await _exportService.ExportAsync(d.t, pd)).Map(Encoding.UTF8.GetString)
                            , Left<Error, string>);
                }, errors => Left<Error, string>(errors.Join()))
                .ToActionResultAsync();
        }

        private byte[] LoadFileContent(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
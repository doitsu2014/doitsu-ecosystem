using FileConversion.Abstraction;
using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Optional;
using Optional.Async;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileConversion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = Security.Scheme)]
    //[Authorize(Security.PolicyFileConversion)]
    [AllowAnonymous]
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
        public async Task<IActionResult> Post([FromForm] IFormFile file, [FromRoute] string key, [FromRoute] string inputType)
        {
            async Task<Option<ImmutableList<object>, string>> Parse<T>(string k, IFormFile f) where T : IStandardModel
            {
                return (await _parserFactory.GetParserAsync<T>(key))
                    .FlatMap(p => p.Parse(LoadFileContent(f)))
                    .Map(p => p.Select(e => e as object).ToImmutableList());
            }

            return (await (key, file, inputType).SomeNotNull().WithException(string.Empty)
                .Filter(d => !string.IsNullOrEmpty(d.key), "Null or empty key")
                .Filter(d => file != null && file.Length > 0, "No file or empty file was submitted")
                .FlatMapAsync(async d => {
                    if (!Enum.TryParse(d.inputType, true, out InputType value) || value == InputType.NotSupported)
                    {
                        return Option.None<byte[], string>(
                            $"Not supported input type: {value.ToString()}");
                    }

                    var parsedResult = value == InputType.VendorPayment
                            ? await Parse<VendorPayment>(d.key, d.file)
                            : value == InputType.PosPay
                                ? await Parse<PosPay>(d.key, d.file)
                                : await Parse<VendorMaster>(d.key, d.file);

                    return await parsedResult.FlatMapAsync(async pd => await _exportService.ExportAsync(d.inputType, pd));
                }))
                .Match(res => File(res, "application/octet-stream", Constants.DefaultExportFileName), err => (IActionResult)BadRequest(err));
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

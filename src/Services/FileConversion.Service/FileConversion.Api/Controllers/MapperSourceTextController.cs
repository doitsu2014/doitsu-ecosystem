using System.Threading.Tasks;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using FileConversion.Core.Interface;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstraction.Models.Types;
using Shared.Extensions;
using static Shared.Validations.GenericValidator;
using static LanguageExt.Prelude;

namespace FileConversion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapperSourceTextController : ControllerBase
    {
        private readonly IFileConversionRepository<MapperSourceText> _mapperSourceTextRepository;

        public MapperSourceTextController(IFileConversionRepository<MapperSourceText> mapperSourceTextRepository)
        {
            _mapperSourceTextRepository = mapperSourceTextRepository;
        }

        [HttpGet]
        [Authorize(PolicyConstants.PolicyFileConversionRead)]
        public async Task<IActionResult> GetAllMapperSourceTexts()
        {
            return Ok(await _mapperSourceTextRepository.ListAllAsync());
        }

        [HttpGet("{key}")]
        [Authorize(PolicyConstants.PolicyFileConversionRead)]
        public async Task<IActionResult> FindMapperSourceText([FromRoute] int key)
        {
            return await _mapperSourceTextRepository.GetAsync(key)
                .ToActionResultAsync();
        }

        private Task<Validation<Error, MapperSourceText>> MapperSourceTextMustExist(MapperSourceText mapperSourceText)
            => _mapperSourceTextRepository.AnyAsync(e => e.Id == mapperSourceText.Id)
                .Map(exist => exist
                    ? Fail<Error, MapperSourceText>(
                        $"Mapper source text {mapperSourceText.Id} does exist")
                    : Success<Error, MapperSourceText>(mapperSourceText));

        private Task<Validation<Error, MapperSourceText>> MapperSourceTextMustNotExist(
            MapperSourceText mapperSourceText)
            => _mapperSourceTextRepository.AnyAsync(e => e.Id == mapperSourceText.Id)
                .Map(exist => !exist
                    ? Fail<Error, MapperSourceText>(
                        $"Mapper source text {mapperSourceText.Id} does not exist")
                    : Success<Error, MapperSourceText>(mapperSourceText));

        private Task<Validation<Error, MapperSourceText>> ValidateCreateMapperSourceText(
            MapperSourceText mapperSourceText)
            => ShouldNotNull(mapperSourceText)
                .AsTask()
                .BindT(MapperSourceTextMustExist);

        private Task<Validation<Error, MapperSourceText>> ValidateEditMapperSourceText(
            MapperSourceText mapperSourceText)
            => ShouldNotNull(mapperSourceText)
                .AsTask()
                .BindT(MapperSourceTextMustNotExist);

        [HttpPost]
        [Authorize(PolicyConstants.PolicyFileConversionWrite)]
        public async Task<IActionResult> Post([FromBody] MapperSourceText data)
        {
            return await ValidateCreateMapperSourceText(data)
                .MapT(async d =>
                {
                    await _mapperSourceTextRepository.AddAsync(d);
                    return d;
                })
                .ToActionResultAsync();
        }

        [HttpPut]
        [Authorize(PolicyConstants.PolicyFileConversionWrite)]
        public async Task<IActionResult> Put([FromBody] MapperSourceText data)
        {
            return await ValidateEditMapperSourceText(data)
                .MapT(async d =>
                {
                    await _mapperSourceTextRepository.UpdateAsync(d);
                    return d;
                })
                .ToActionResultAsync();
        }

        [HttpDelete("{key}")]
        [Authorize(PolicyConstants.PolicyFileConversionWrite)]
        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            return await (await _mapperSourceTextRepository.GetAsync(key))
                .ToValidation(Error.New($"Mapper source text {key} does not exist"))
                .AsTask()
                .MapT(async d =>
                {
                    await _mapperSourceTextRepository.DeleteAsync(d);
                    return $"Remove Source mapper id {key} successfully";
                })
                .ToActionResultAsync();
        }
    }
}
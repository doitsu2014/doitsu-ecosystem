using System.Threading.Tasks;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using FileConversion.Core.Interface;
using FileConversion.Infrastructure;
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
    public class InputMappingController : ControllerBase
    {
        private readonly IFileConversionRepository<InputMapping> _inputMappingRepository;

        public InputMappingController(IFileConversionRepository<InputMapping> inputMappingRepository)
        {
            _inputMappingRepository = inputMappingRepository;
        }

        [HttpGet]
        [Authorize(PolicyConstants.PolicyFileConversionRead)]
        public async Task<IActionResult> GetAllInputMappings()
        {
            return Ok(await _inputMappingRepository.ListAllAsync());
        }

        [HttpGet("{key}/{inputType}")]
        [Authorize(PolicyConstants.PolicyFileConversionRead)]
        public async Task<IActionResult> FindInputMapping([FromRoute] string key, [FromRoute] InputType inputType)
        {
            return await _inputMappingRepository.SingleOrDefaultAsync(x =>
                    x.Key == key && x.InputType == inputType)
                .ToActionResultAsync();
        }

        private async Task<Validation<Error, InputMapping>> InputMappingMustNotExist(InputMapping inputMapping)
            => await _inputMappingRepository
                .AnyAsync(e => e.Key == inputMapping.Key && e.InputType == inputMapping.InputType)
                .Map(exist => exist
                    ? Fail<Error, InputMapping>(
                        $"Input mapping key {inputMapping.Key} and type {inputMapping.InputType} does exist")
                    : Success<Error, InputMapping>(inputMapping));

        private async Task<Validation<Error, InputMapping>> InputMappingMustExist(InputMapping inputMapping)
            => await _inputMappingRepository
                .AnyAsync(e => e.Key == inputMapping.Key && e.InputType == inputMapping.InputType)
                .Map(exist => !exist
                    ? Fail<Error, InputMapping>(
                        $"Input mapping key {inputMapping.Key} and type {inputMapping.InputType} does not exist")
                    : Success<Error, InputMapping>(inputMapping));

        private async Task<Validation<Error, InputMapping>> ValidateCreateInputMapping(InputMapping inputMapping)
            => await ShouldNotNull(inputMapping)
                .AsTask()
                .BindT(InputMappingMustNotExist);

        private async Task<Validation<Error, InputMapping>> ValidateEditInputMapping(InputMapping inputMapping)
            => await ShouldNotNull(inputMapping)
                .AsTask()
                .BindT(InputMappingMustExist);

        [HttpPost]
        [Authorize(PolicyConstants.PolicyFileConversionWrite)]
        public async Task<IActionResult> Post([FromBody] InputMapping data)
        {
            return await ValidateCreateInputMapping(data)
                .MapT(async d =>
                {
                    await _inputMappingRepository.AddAsync(d);
                    return d;
                })
                .ToActionResultAsync();
        }

        [HttpPut]
        [Authorize(PolicyConstants.PolicyFileConversionWrite)]
        public async Task<IActionResult> Put([FromBody] InputMapping data)
        {
            return (await ValidateEditInputMapping(data)
                .MapT(async d =>
                {
                    await _inputMappingRepository.UpdateAsync(d);
                    return d;
                })
                .ToActionResultAsync());
        }

        [HttpDelete("{key}/{inputType}")]
        [Authorize(PolicyConstants.PolicyFileConversionWrite)]
        public async Task<IActionResult> Delete([FromRoute] string key, [FromRoute] InputType inputType)
        {
            return (await (await _inputMappingRepository.SingleOrDefaultAsync(e =>
                    e.Key == key && e.InputType == inputType))
                .ToValidation(Error.New($"Input mapping key {key} and type {inputType} does not exist"))
                .AsTask()
                .MapT(async im =>
                {
                    await _inputMappingRepository.DeleteAsync(im);
                    return $"Remove input mapping key {key} and type {inputType} successfully";
                })
                .ToActionResultAsync());
        }
    }
}
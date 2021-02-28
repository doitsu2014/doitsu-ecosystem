using System.Threading;
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
    public class OutputMappingController : ControllerBase
    {
        private readonly IFileConversionRepository<OutputMapping> _outputMappingRepository;

        public OutputMappingController(IFileConversionRepository<OutputMapping> outputMappingRepository)
        {
            _outputMappingRepository = outputMappingRepository;
        }

        [HttpGet]
        [Authorize(PolicyConstants.PolicyFileConversionRead)]
        public async Task<IActionResult> GetAllOutputMappings()
        {
            return Ok(await _outputMappingRepository.ListAllAsync());
        }

        [HttpGet("{key}")]
        [Authorize(PolicyConstants.PolicyFileConversionRead)]
        public async Task<IActionResult> FindOutputMapping([FromRoute] InputType key)
        {
            return await _outputMappingRepository.GetAsync(key)
                .ToActionResultAsync();
        }

        private Task<Validation<Error, OutputMapping>> OutputMappingMustExist(OutputMapping outputMapping)
            => _outputMappingRepository.AnyAsync(e => e.Id == outputMapping.Id)
                .Map(exist => exist
                    ? Fail<Error, OutputMapping>(
                        $"Output mapping {outputMapping.Id} does exist")
                    : Success<Error, OutputMapping>(outputMapping));

        private Task<Validation<Error, OutputMapping>> OutputMappingMustNotExist(OutputMapping outputMapping)
            => _outputMappingRepository.AnyAsync(e => e.Id == outputMapping.Id)
                .Map(exist => !exist
                    ? Fail<Error, OutputMapping>(
                        $"Output mapping {outputMapping.Id} does not exist")
                    : Success<Error, OutputMapping>(outputMapping));

        private Task<Validation<Error, OutputMapping>> ValidateCreateOutputMapping(
            OutputMapping mapperSourceText)
            => ShouldNotNull(mapperSourceText)
                .AsTask()
                .BindT(OutputMappingMustExist);

        private Task<Validation<Error, OutputMapping>> ValidateEditOutputMapping(
            OutputMapping mapperSourceText)
            => ShouldNotNull(mapperSourceText)
                .AsTask()
                .BindT(OutputMappingMustNotExist);


        [HttpPost]
        [Authorize(PolicyConstants.PolicyFileConversionWrite)]
        public async Task<IActionResult> Post([FromBody] OutputMapping data)
        {
            return await ValidateCreateOutputMapping(data)
                .MapT(async d =>
                {
                    await _outputMappingRepository.AddAsync(d);
                    return d;
                })
                .ToActionResultAsync();
        }

        [HttpPut]
        [Authorize(PolicyConstants.PolicyFileConversionWrite)]
        public async Task<IActionResult> Put([FromBody] OutputMapping data)
        {
            return await ValidateEditOutputMapping(data)
                .MapT(async d =>
                {
                    await _outputMappingRepository.UpdateAsync(d);
                    return d;
                })
                .ToActionResultAsync();
        }

        [HttpDelete("{key}")]
        [Authorize(PolicyConstants.PolicyFileConversionWrite)]
        public async Task<IActionResult> Delete([FromRoute] InputType key)
        {
            return await (await _outputMappingRepository.GetAsync(key))
                .ToValidation(Error.New($"Output mapping {key} does not exist"))
                .AsTask()
                .MapT(async d =>
                {
                    await _outputMappingRepository.DeleteAsync(d);
                    return $"Remove output mapping {key} successfully";
                })
                .ToActionResultAsync();
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using FileConversion.Data;
using FileConversion.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Optional;
using Optional.Async;

namespace FileConversion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Security.Scheme)]
    [Authorize(Security.PolicyFileConversion)]
    public class OutputMappingController : ControllerBase
    {
        private readonly IFileConversionEntityService<OutputMapping> _outputMappingEntityService;
        private readonly FileConversionContext _db;
        public OutputMappingController(IFileConversionEntityService<OutputMapping> outputMappingEntityService, FileConversionContext db)
        {
            _outputMappingEntityService = outputMappingEntityService;
            _db = db;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllOutputMappings()
        {
            return Ok(await _outputMappingEntityService.ReadManyNoTracked().ToListAsync());
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> FindOutputMapping([FromRoute]InputType key)
        {
            return Ok(await _outputMappingEntityService.Set().FirstOrDefaultAsync(x => x.Key == key));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]OutputMapping data)
        {
            return (await data.SomeNotNull()
                    .WithException("Output Mapping data is empty")
                    .FilterAsync(async d => !(await _outputMappingEntityService.Set().AnyAsync(im => im.Key == data.Key)), $"Output Mapping key {data.Key} does exist")
                    .MapAsync(async d =>
                    {
                        var OutputMapping = await _db.AddAsync(d);
                        await _db.SaveChangesAsync(cancellationToken: CancellationToken.None);
                        return OutputMapping.Entity;
                    }))
                    .Match<IActionResult>(
                        res => Ok(res),
                        error => BadRequest(error)
                    );
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]OutputMapping data)
        {
            return (await data.SomeNotNull()
                    .WithException("Output Mapping data is empty")
                    .FilterAsync(async d => await _outputMappingEntityService.Set().AnyAsync(im => im.Key == data.Key), $"Output Mapping Key {data.Key} does not exist")
                    .MapAsync(async d =>
                    {
                        var OutputMapping = _db.Update(d);
                        await _db.SaveChangesAsync(cancellationToken: CancellationToken.None);
                        return OutputMapping.Entity;
                    }))
                    .Match<IActionResult>(
                        res => Ok(res),
                        error => BadRequest(error)
                    );
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromRoute]InputType key)
        {
            return (await key.SomeNotNull()
                    .WithException("Deleted key is empty")
                    .FilterAsync(async d => (await _outputMappingEntityService.Set().AnyAsync(im => im.Key == key)), $"Output Mapping key {key} does not exist")
                    .MapAsync(async d =>
                    {
                        var OutputMapping = await _outputMappingEntityService.Set().FirstOrDefaultAsync(im => im.Key == key);
                        _db.Remove(OutputMapping);
                        await _db.SaveChangesAsync(cancellationToken: CancellationToken.None);
                        return $"Remove Output Mapping Key {key} successfully";
                    }))
                    .Match<IActionResult>(
                        res => Ok(res),
                        error => BadRequest(error)
                    );
        }
    }
}
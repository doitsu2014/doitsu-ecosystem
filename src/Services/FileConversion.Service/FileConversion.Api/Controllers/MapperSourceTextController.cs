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
    public class MapperSourceTextController : ControllerBase
    {
        private readonly IFileConversionEntityService<MapperSourceText> _mapperSourceTextEntityService;
        private readonly FileConversionContext _db;

        public MapperSourceTextController(IFileConversionEntityService<MapperSourceText> mapperSourceTextEntityService, FileConversionContext db)
        {
            _mapperSourceTextEntityService = mapperSourceTextEntityService;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMapperSourceTexts()
        {
            return Ok(await _mapperSourceTextEntityService.ReadManyNoTracked().ToListAsync());
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> FindMapperSourceText([FromRoute]int key)
        {
            return Ok(await _mapperSourceTextEntityService.Set().FirstOrDefaultAsync(x => x.Id == key));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]MapperSourceText data)
        {
            return (await data.SomeNotNull()
                    .WithException("Source mapper data is empty")
                    .FilterAsync(async d => !(await _mapperSourceTextEntityService.Set().AnyAsync(im => im.Id == data.Id)), $"Source mapper key {data.Id} does exist")
                    .MapAsync(async d =>
                    {
                        var MapperSourceText = await _db.AddAsync(d);
                        await _db.SaveChangesAsync(cancellationToken: CancellationToken.None);
                        return MapperSourceText.Entity;
                    }))
                    .Match<IActionResult>(
                        res => Ok(res),
                        error => BadRequest(error)
                    );
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]MapperSourceText data)
        {
            return (await data.SomeNotNull()
                    .WithException("Source mapper data is empty")
                    .FilterAsync(async d => await _mapperSourceTextEntityService.Set().AnyAsync(im => im.Id == data.Id), $"Source mapper id {data.Id} does not exist")
                    .MapAsync(async d =>
                    {
                        var MapperSourceText = _db.Update(d);
                        await _db.SaveChangesAsync(cancellationToken: CancellationToken.None);
                        return MapperSourceText.Entity;
                    }))
                    .Match<IActionResult>(
                        res => Ok(res),
                        error => BadRequest(error)
                    );
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromRoute]int key)
        {
            return (await key.SomeNotNull()
                    .WithException("Deleted key is empty")
                    .FilterAsync(async d => (await _mapperSourceTextEntityService.Set().AnyAsync(im => im.Id == key)), $"Source mapper key {key} does not exist")
                    .MapAsync(async d =>
                    {
                        var MapperSourceText = await _mapperSourceTextEntityService.Set().FirstOrDefaultAsync(im => im.Id == key);
                        _db.Remove(MapperSourceText);
                        await _db.SaveChangesAsync(cancellationToken: CancellationToken.None);
                        return $"Remove Source mapper id {key} successfully";
                    }))
                    .Match<IActionResult>(
                        res => Ok(res),
                        error => BadRequest(error)
                    );
        }
    }
}
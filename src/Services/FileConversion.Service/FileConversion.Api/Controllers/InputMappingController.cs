using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using FileConversion.Core.Interface;
using FileConversion.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileConversion.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Security.Scheme)]
    [Authorize(Security.PolicyFileConversion)]
    public class InputMappingController : ControllerBase
    {
        private readonly IFileConversionRepository<InputMapping> _inputMappingRepository;
        private readonly FileConversionContext _db;

        public InputMappingController(IFileConversionRepository<InputMapping> inputMappingRepository,
            FileConversionContext db)
        {
            _inputMappingRepository = inputMappingRepository;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInputMappings()
        {
            return Ok(await _inputMappingRepository.ListAllAsync());
        }

        [HttpGet("{key}/{inputType}")]
        public async Task<IActionResult> FindInputMapping([FromRoute] string key, [FromRoute] InputType inputType)
        {
            return Ok(await _inputMappingRepository.ListAsync(q =>
                q.Where(x => x.Key == key && x.InputType == inputType)));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] InputMapping data)
        {
            return (await data.SomeNotNull()
                    .WithException("Input mapping data is empty")
                    .FilterAsync(
                        async d => !(await _inputMappingRepository
                            .AnyAsync(im => im.Key == data.Key && im.InputType == data.InputType)),
                        $"Input mapping key {data.Key} and input type {data.InputType} does exist")
                    .MapAsync(async d =>
                    {
                        var inputMapping = await _db.AddAsync(d);
                        await _db.SaveChangesAsync(cancellationToken: CancellationToken.None);
                        return inputMapping.Entity;
                    }))
                .Match<IActionResult>(
                    res => Ok(res),
                    error => BadRequest(error)
                );
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] InputMapping data)
        {
            return (await data.SomeNotNull()
                    .WithException("Input mapping data is empty")
                    .FilterAsync(
                        async d => (await _inputMappingRepository.AnyAsync(im => im.Key == data.Key && im.InputType == data.InputType)),
                        $"Input mapping key {data.Key} and input type {data.InputType} does not exist")
                    .MapAsync(async d =>
                    {
                        var inputMapping = _db.Update(d);
                        await _db.SaveChangesAsync(cancellationToken: CancellationToken.None);
                        return inputMapping.Entity;
                    }))
                .Match<IActionResult>(
                    res => Ok(res),
                    error => BadRequest(error)
                );
        }

        [HttpDelete("{key}/{inputType}")]
        public async Task<IActionResult> Delete([FromRoute] string key, [FromRoute] InputType inputType)
        {
            return (await key.SomeNotNull()
                    .WithException("Deleted key is empty")
                    .FilterAsync(
                        async d => (await _inputMappingRepository
                            .AnyAsync(im => im.Key == key && im.InputType == inputType)),
                        $"Input mapping key {key} and input type {inputType} does not exist")
                    .MapAsync(async d =>
                    {
                        var inputMapping = await _inputMappingRepository
                            .SingleOrDefaultAsync(im => im.Key == key && im.InputType == inputType);
                        
                        _db.Remove(inputMapping);
                        await _db.SaveChangesAsync(cancellationToken: CancellationToken.None);
                        
                        return $"Remove input mapping key {key} and input type {inputType} successfully";
                    }))
                .Match<IActionResult>(
                    res => Ok(res),
                    error => BadRequest(error)
                );
        }
    }
}
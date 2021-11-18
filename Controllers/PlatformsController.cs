using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController: ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBus;

        public PlatformsController(
            IPlatformRepo repository,
            IMapper mapper, ICommandDataClient commandDataClient, 
            IMessageBusClient messageBus)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var platforms = await _repository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = "GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var platform = await _repository.GetPlatformByIdAsync(id);

            if (platform is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlatformCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = _mapper.Map<Platform>(createDto);

            await _repository.CreatePlatformAsync(model);
            await _repository.SaveChangesAsync();

            var platformDto = _mapper.Map<PlatformReadDto>(model);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($"-->cound not send synchronusly: {e.Message}");
            }

            try
            {
                var platformPublishDto = _mapper.Map<PlatformPublishedDto>(platformDto);
                platformPublishDto.Event = "Platform_Published";
                _ = _messageBus.PublishNewPlatform(platformPublishDto);

            }
            catch (Exception e)
            {
                Console.WriteLine($"-->cound not send asynchronously: {e.Message}");
            }

            return CreatedAtRoute(nameof(GetById), new {platformDto.Id}, platformDto);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MessageApi.DomainObjects;
using MessageApi.Model;
using MessageApi.Repositories;
using MessageApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessageApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DutyMessageController : ControllerBase
    {
        private readonly DutyMessageRepository _repository;
        private readonly MappingService _mappingService;
        private readonly SubmissionService _submissionService;

        public DutyMessageController(
            DutyMessageRepository repository, 
            MappingService mappingService,
            SubmissionService submissionService)
        {
            _repository = repository;
            _mappingService = mappingService;
            _submissionService = submissionService;
        }

        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(List<DutyMessageDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetMessagesByCustomer(Guid customerId)
        {
            try
            {
                var result = _mappingService.Map<DutyMessage, DutyMessageDto>(await _repository.GetByCustomerId(customerId));
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("{customerId}")]
        [ProducesResponseType(typeof(List<DutyMessageDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Add(Guid customerId, [FromBody]List<DutyMessageDto> messages)
        {
            try
            {
                foreach (var msg in messages)
                    msg.CustomerId = customerId;

                await _repository.Add(customerId, _mappingService.Map<DutyMessageDto, DutyMessage>(messages));
                return Ok(messages);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("{customerId}")]
        [ProducesResponseType(typeof(List<DutyMessageDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Submit(Guid customerId, [FromBody]List<Guid> messageIds)
        {
            try
            {
                await _submissionService.Submit(customerId, messageIds);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
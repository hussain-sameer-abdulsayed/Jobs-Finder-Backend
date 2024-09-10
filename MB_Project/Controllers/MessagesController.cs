using AutoMapper;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS.MessageDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MB_Project.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepo _messageRepo;
        private readonly IMapper _mapper;
        public MessagesController(IMessageRepo messageRepo, IMapper mapper)
        {
            _messageRepo = messageRepo;
            _mapper = mapper;
        }

        [HttpGet("{chatRoomId}")]
        public async Task<IActionResult> GetMessages(string chatRoomId)
        {
            try
            {
                var messages = await _messageRepo.GetMessagesByRoomId(chatRoomId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("message/{id}")]
        public async Task<IActionResult> GetMessageById(string id)
        {
            try
            {
                var message = await _messageRepo.GetMessageByIdAsync(id);
                if (message == null)
                {
                    return BadRequest("Message Not Found");
                }
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MB_Project.IRepos;
using MB_Project.Models;
using MongoDB.Driver.Core.Servers;
using Microsoft.Net.Http.Headers;

namespace MB_Project.Controllers
{
    [Authorize]
    [Route("api/chatroom")]
    [ApiController]
    public class ChatRoomController : ControllerBase
    {
        private readonly IRoomRepo _roomRepo;
        private readonly IJWTManagerRepo _jwtRepo;

        public ChatRoomController(IRoomRepo roomRepo, IJWTManagerRepo jwtRepo)
        {
            _roomRepo = roomRepo;
            _jwtRepo = jwtRepo;
        }

        [HttpPost("{otherUserId}")]
        public async Task<IActionResult> CreateChatRoom(string otherUserId)
        {
            if (string.IsNullOrEmpty(otherUserId))
            {
                
                return BadRequest("Invalid data");
            }
            Console.WriteLine("otherUserId");
            Console.WriteLine(otherUserId);
            // Get the authenticated user's ID from the token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("userId");
            Console.WriteLine(userId);


            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }
            // This checks if there is any order between two users, used for create chatRoom if order exist then create chatRoom if doesn't do not create
            var isThereAnyOrder = await _roomRepo.CheckOrderBetweenTwoUsers(userId, otherUserId);
            if (!isThereAnyOrder)
            {
                return BadRequest("There is no order between you.");
            }


            var chatRoom = new ChatRoom
            {
                Id = Guid.NewGuid().ToString(),
                BuyerId = userId,
                SellerId = otherUserId,
                CreatedAt = DateTime.UtcNow,

            };


            await _roomRepo.AddChatRoomAsync(chatRoom);
            return Ok(chatRoom);  
            /*
            var chatRoom = await _roomRepo.FindChatRoomAsync(userId, otherUserId);
            if (chatRoom == null)
            {
                return NotFound();
            }
            return Ok(chatRoom);
            */
            /*
            if (chatRoom != null) { 
                return Ok(chatRoom);
            }
            
           
            
            chatRoom = new ChatRoom
            {
                Id = Guid.NewGuid().ToString(),
                BuyerId = userId,
                SellerId = otherUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _roomRepo.AddChatRoomAsync(chatRoom);

            return Ok(chatRoom);
            */
        }


        [HttpGet("{otherUserId}")]
        public async Task<IActionResult> FindChatRoom(string otherUserId)
        {
            if (string.IsNullOrEmpty(otherUserId))
            {
                return BadRequest("Invalid data");
            }

            // Get the authenticated user's ID from the token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var chatRoom = await _roomRepo.FindChatRoomAsync(userId, otherUserId);

            if (chatRoom != null)
            {
                return Ok(chatRoom);
            }

            return NotFound("There is no cahtRoom between you");
        }


        [HttpGet()]
        public async Task<IActionResult> GetUserChatRooms()
        {
            // Get the authenticated user's ID from the token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("userId:", userId);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            try
            {
                var chatRooms = await _roomRepo.GetChatRoomsByUserId(userId);
                Console.WriteLine("chatRooms.Count: ", chatRooms.Count);
                return Ok(chatRooms);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

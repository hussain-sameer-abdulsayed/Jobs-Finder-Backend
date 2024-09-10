using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models.DTOS.MessageDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MB_Project.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageRepo _messageRepo;
        private readonly IRoomRepo _roomRepo;
        private readonly MB_ProjectContext _context;
        private readonly IJWTManagerRepo _jwtManagerRepo;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IMessageRepo messageRepo, IRoomRepo roomRepo, MB_ProjectContext context, IJWTManagerRepo jwtManagerRepo, ILogger<ChatHub> logger)
        {
            _messageRepo = messageRepo;
            _roomRepo = roomRepo;
            _context = context;
            _jwtManagerRepo = jwtManagerRepo;
            _logger = logger;
        }

        public async Task SendMessage(string chatRoomId, string userId, string message)
        {
            _logger.LogInformation($"SendMessage invoked with chatRoomId: {chatRoomId}, userId: {userId}, message: {message}");

            if (string.IsNullOrEmpty(chatRoomId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
            {
                _logger.LogError("Invalid parameters in SendMessage");
                throw new ArgumentException("Invalid parameters");
            }

            var chatMessage = new Message
            {
                ChatRoomId = chatRoomId,
                SenderId = userId,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            var createdMessage = await _messageRepo.CreateChatMessage(chatMessage.ChatRoomId, chatMessage.SenderId, chatMessage.Content);
            await _roomRepo.AddMessageToChatRoom(chatRoomId, createdMessage.Id);

            await Clients.Group(chatRoomId).SendAsync("ReceiveMessage", userId, message);

            var chatRoom = await _roomRepo.GetChatRoomById(chatRoomId);

            var otherUserId = chatRoom.BuyerId == userId ? chatRoom.SellerId : chatRoom.BuyerId;
            if (!Context.ConnectionId.Contains(otherUserId))
            {
                await Clients.User(otherUserId).SendAsync("NewMessageNotification", chatRoomId, message);
            }
        }

        public async Task JoinRoom(string chatRoomId)
        {
            _logger.LogInformation($"JoinRoom invoked with chatRoomId: {chatRoomId}");

            if (string.IsNullOrEmpty(chatRoomId))
            {
                _logger.LogError("Invalid chatRoomId in JoinRoom");
                throw new ArgumentException("Invalid chatRoomId");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId);

            var messages = await _messageRepo.GetMessagesByRoomId(chatRoomId);
            await Clients.Caller.SendAsync("LoadMessages", messages);
        }

        public async Task LeaveRoom(string chatRoomId)
        {
            _logger.LogInformation($"LeaveRoom invoked with chatRoomId: {chatRoomId}");

            if (string.IsNullOrEmpty(chatRoomId))
            {
                _logger.LogError("Invalid chatRoomId in LeaveRoom");
                throw new ArgumentNullException(nameof(chatRoomId));
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId);
        }

        public async Task Typing(string chatRoomId, string userId)
        {
            _logger.LogInformation($"Typing invoked with chatRoomId: {chatRoomId}, userId: {userId}");

            if (string.IsNullOrEmpty(chatRoomId) || string.IsNullOrEmpty(userId))
            {
                _logger.LogError("Invalid parameters in Typing");
                throw new ArgumentException("Invalid parameters");
            }

            await Clients.Group(chatRoomId).SendAsync("Typing", userId);
        }

        public async Task StopTyping(string chatRoomId, string userId)
        {
            _logger.LogInformation($"StopTyping invoked with chatRoomId: {chatRoomId}, userId: {userId}");

            if (string.IsNullOrEmpty(chatRoomId) || string.IsNullOrEmpty(userId))
            {
                _logger.LogError("Invalid parameters in StopTyping");
                throw new ArgumentException("Invalid parameters");
            }

            await Clients.Group(chatRoomId).SendAsync("StopTyping", userId);
        }
    }
}






/*using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models.DTOS.MessageDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MB_Project.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageRepo _messageRepo;
        private readonly IRoomRepo _roomRepo;
        private readonly MB_ProjectContext _context;
        private readonly IJWTManagerRepo _jwtManagerRepo;

        public ChatHub(IMessageRepo messageRepo, IRoomRepo roomRepo, MB_ProjectContext context, IJWTManagerRepo jwtManagerRepo)
        {
            _messageRepo = messageRepo;
            _roomRepo = roomRepo;
            _context = context;
            _jwtManagerRepo = jwtManagerRepo;
        }



        public async Task SendMessage(string chatRoomId, string userId, string message)
        {
            if (string.IsNullOrEmpty(chatRoomId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Invalid parameters");
            }
            
            var chatMessage = new Message
            {
                ChatRoomId = chatRoomId,
                SenderId = userId,
                Content = message,
                Timestamp = DateTime.UtcNow
            };
            
            //CreateMessageDto messageDto = new CreateMessageDto()
            //{
              //  RoomId = chatRoomId,
              //  UserId = userId,
              //  Content = message
            //};
            
            var createdMessage = await _messageRepo.CreateChatMessage(chatMessage.ChatRoomId, chatMessage.SenderId, chatMessage.Content);
            await _roomRepo.AddMessageToChatRoom(chatRoomId, createdMessage.Id);
            
            //var createdMessage = await _messageRepo.CreateChatMessage(messageDto);
            //await _roomRepo.AddMessageToChatRoom(createdMessage);
            
            await Clients.Group(chatRoomId).SendAsync("ReceiveMessage", userId, message);

            var chatRoom = await _roomRepo.GetChatRoomById(chatRoomId);

            var otherUserId = chatRoom.BuyerId == userId ? chatRoom.SellerId : chatRoom.BuyerId;
            if (!Context.ConnectionId.Contains(otherUserId))
            {
                await Clients.User(otherUserId).SendAsync("NewMessageNotification", chatRoomId, message);
            }
        }


        public async Task JoinRoom(string chatRoomId)
        {
            // Notify the client to fetch messages
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId);
 
            // we are using an api to fetch athe messages for now, the following lines work as well but an api is more controllable
            var messages = await _messageRepo.GetMessagesByRoomId(chatRoomId);
            await Clients.Caller.SendAsync("LoadMessages", messages);
        }

        public async Task LeaveRoom(string chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId);
        }

        public async Task Typing(string chatRoomId, string userId)
        {
            await Clients.Group(chatRoomId).SendAsync("Typing", userId);
        }

        public async Task StopTyping(string chatRoomId, string userId)
        {
            await Clients.Group(chatRoomId).SendAsync("StopTyping", userId);
        }
    }
}
*/














































/*
using Azure.Core;
using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS.UserDto;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Plugins;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MB_Project.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageRepo _messageRepo;
        private static readonly ConcurrentDictionary<string, ChatRoom> _chatRooms = new ConcurrentDictionary<string, ChatRoom>();
        private readonly IRoomRepo _roomRepo;
        private readonly IJWTManagerRepo _jwtManagerRepo;
        private readonly MB_ProjectContext _context;
        private readonly IUserRepo _userRepo;
        public ChatHub(IMessageRepo messageRepo, IJWTManagerRepo jWTManagerRepo, IRoomRepo roomRepo, MB_ProjectContext context, IUserRepo userRepo)
        {
            _messageRepo = messageRepo;
            _jwtManagerRepo = jWTManagerRepo;
            _roomRepo = roomRepo;
            _context = context;
            _userRepo = userRepo;
        }

        public async Task JoinSpecificChatRoom(string username, string chatroom )
        {
            try
            {
                var sender = await GetUser();
                //var receiver = await GetReceiver();
                if (sender == null ) 
                {
                    return;
                }

                // Get or create the chat room
                var room = await _roomRepo.GetRoomByUserAsync(sender.Id, chatroom);
                if (room is null)
                {
                    ChatRoom chatRoom = new ChatRoom()
                    {
                        User1Id = sender.Id,
                        User2Id = chatroom // here must be ReceiverId rather than chatRoom
                        //User2Id = receiver.Id,
                    };
                    await _roomRepo.CreateRoomAsync(chatRoom);
                    room = chatRoom;
                    // Verify insertion
                    var insertedRoom = await _roomRepo.GetRoomByUserAsync(sender.Id, chatroom);
                    if (insertedRoom != null)
                    {
                        Console.WriteLine($"Chat room successfully inserted with Id: {insertedRoom.Id}");
                    }
                    else
                    {
                        Console.WriteLine("Failed to insert chat room.");
                        return;
                    }
                }
                

                // add get user to print jis userName when send a message

                // Add connection to group
                await Groups.AddToGroupAsync(Context.ConnectionId, room.Id);
                _chatRooms[Context.ConnectionId] = room;
                await Clients.Group(room.Id).SendAsync("ReceiveMessage", "admin", $"{sender.Name} has joined {room.Id}");

                // Fetch past messages
                await FetchMessages(room.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        // here must send ==> message and inside the message send the the roomId
        public async Task SendMessage(string msg)
        {
            if (_chatRooms.TryGetValue(Context.ConnectionId, out ChatRoom cr))
            {
                var sender = await GetUser();
                if (sender == null)
                {
                    return;
                }
                var message = new Message
                {
                    Content = msg,
                    SenderId = sender.Id,
                    RoomId = cr.Id
                };

                await _messageRepo.SendMessage(message);
                await Clients.Group(cr.Id).SendAsync("ReceiveSpecificMessage", sender.Name, msg);
            }
        }

        public async Task FetchMessages(string chatRoomId)
        {
            try
            {
                var messages = await _messageRepo.GetMessages(chatRoomId);
                if (messages == null || messages.Count == 0)
                {
                    return;
                }

                // Send all messages to the client
                await Clients.Caller.SendAsync("ReceiveMessages", messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching messages: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }


        private async Task<ViewUserDto> GetUser()
        {
            // Ensure the token is present in the query string
            string token = Context.GetHttpContext()?.Request.Query["token"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            // Get sender ID from token
            string senderId = _jwtManagerRepo.GetUserId(token);
            if (senderId is null)
            {
                return null;
            }

            return await _userRepo.GetUserById(senderId);
        }
        
        private async Task<ViewUserDto> GetReceiver()
        {
            string receiverId = Context.GetHttpContext()?.Request.Query["receiverId"].ToString();
            if (string.IsNullOrEmpty(receiverId))
            {
                return null;
            }
            return await _userRepo.GetUserById(receiverId);
        }

    }
}
*/

















/*
 
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using System.Linq;

public class ChatHub : Hub
{
    private readonly IRoomRepo _roomRepo; // Inject the repository for room management
    private readonly IMessageRepo _messageRepo; // Inject the repository for message management
    private readonly IUserRepo _userRepo; // Inject the repository for user management

    public ChatHub(IRoomRepo roomRepo, IMessageRepo messageRepo, IUserRepo userRepo)
    {
        _roomRepo = roomRepo;
        _messageRepo = messageRepo;
        _userRepo = userRepo;
    }

    public async Task SendMessage(string senderId, string recipientId, string messageContent)
    {
        var room = await _roomRepo.GetRoomByUsersAsync(senderId, recipientId);
        if (room == null)
        {
            room = new Room
            {
                Id = Guid.NewGuid().ToString(),
                User1Id = senderId,
                User2Id = recipientId,
                CreatedAt = DateTime.UtcNow,
                Messages = new List<Message>()
            };
            await _roomRepo.CreateRoomAsync(room);
        }

        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            RoomId = room.Id,
            SenderId = senderId,
            Content = messageContent,
            Timestamp = DateTime.UtcNow
        };

        await _messageRepo.CreateMessageAsync(message);
        room.Messages.Add(message);

        await Clients.Group(room.Id).SendAsync("ReceiveMessage", message);
    }

    public async Task JoinRoom(string senderId, string recipientId)
    {
        var room = await _roomRepo.GetRoomByUsersAsync(senderId, recipientId);
        if (room == null)
        {
            room = new Room
            {
                Id = Guid.NewGuid().ToString(),
                User1Id = senderId,
                User2Id = recipientId,
                CreatedAt = DateTime.UtcNow,
                Messages = new List<Message>()
            };
            await _roomRepo.CreateRoomAsync(room);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, room.Id);
    }

    public async Task LeaveRoom(string senderId, string recipientId)
    {
        var room = await _roomRepo.GetRoomByUsersAsync(senderId, recipientId);
        if (room != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Id);
        }
    }
}
*/


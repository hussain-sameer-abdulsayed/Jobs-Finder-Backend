/*
using MB_Project.IRepos;
using MB_Project.Models;
using MongoDB.Driver;

namespace MB_Project.Repos
{
    public class MessageRepo : IMessageRepo
    {
        private readonly IMongoCollection<Message> _messages;
        public MessageRepo(IMongoDatabase database)
        {
            _messages = database.GetCollection<Message>("Messages");
        }


        public async Task<List<Message>> GetMessages(string userId)
        {
            var messages = await _messages.Find(m => m.ReceiverId == userId).ToListAsync();
            foreach(var msg in messages)
            {
                msg.IsRead = true;
            }
            return messages;
        }

        public async Task<Message> SendMessage(Message message)
        {
            await _messages.InsertOneAsync(message);
            return message;
        }
    }
}
*/
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS.MessageDto;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MB_Project.Repos
{
    public class MessageRepo : IMessageRepo
    {
        private readonly IMongoCollection<Message> _chatMessages;

        public MessageRepo(IMongoDatabase database)
        {
            _chatMessages = database.GetCollection<Message>("ChatMessages");
        }
        

        // add Dto for create message
        public async Task<Message> CreateChatMessage(string chatRoomId, string senderId, string content)
        {
            var chatMessage = new Message
            {
                ChatRoomId = chatRoomId,
                SenderId = senderId,
                Content = content,
                Timestamp = DateTime.UtcNow
            };
            await _chatMessages.InsertOneAsync(chatMessage);
            return chatMessage;
        }
        /*
        public async Task<Message> CreateChatMessage(CreateMessageDto messageDto)
        {
            var chatMessage = new Message
            {
                ChatRoomId = messageDto.RoomId,
                SenderId = messageDto.UserId,
                Content = messageDto.Content,
                Timestamp = DateTime.UtcNow
            };
            await _chatMessages.InsertOneAsync(chatMessage);
            return chatMessage;
        }
        */
        // return messages by roomId
        public async Task<List<Message>> GetMessagesByRoomId(string roomId)
        {
            return await _chatMessages.Find(message => message.ChatRoomId == roomId).ToListAsync();
        }

        public async Task MarkMessageAsRead(string messageId)
        {
            var update = Builders<Message>.Update.Set(message => message.Read, true);
            await _chatMessages.UpdateOneAsync(message => message.Id == messageId, update);
        }

        public async Task<Message> GetMessageByIdAsync(string messageId)
        {
            return await _chatMessages.Find(m=> m.Id == messageId).FirstOrDefaultAsync();
        }
    }
}

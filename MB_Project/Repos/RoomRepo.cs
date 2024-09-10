using MB_Project.Data;
using MB_Project.IRepos;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace MB_Project.Repos
{
    public class RoomRepo : IRoomRepo
    {
        private readonly IMongoCollection<ChatRoom> _chatRooms;
        private readonly MB_ProjectContext _context;

        public RoomRepo(IMongoDatabase database, MB_ProjectContext context)
        {
            _chatRooms = database.GetCollection<ChatRoom>("ChatRooms");
            _context = context;
        }

        public async Task<ChatRoom> CreateChatRoom(ChatRoom chatRoom)
        {
            await _chatRooms.InsertOneAsync(chatRoom);
            return chatRoom;
        }

        // get all user chatRooms by userId
        public async Task<List<ChatRoom>> GetChatRoomsByUserId(string userId)
        {
            return await _chatRooms.Find(room => room.BuyerId == userId || room.SellerId == userId).ToListAsync();
        }

        // get chatRoom by id
        public async Task<ChatRoom> GetChatRoomById(string id)
        {
            return await _chatRooms.Find(room => room.Id == id).FirstOrDefaultAsync();
        }
        
        public async Task AddMessageToChatRoom(string roomId, string messageId)
        {
            var update = Builders<ChatRoom>.Update.AddToSet(room => room.Messages, messageId);
            await _chatRooms.UpdateOneAsync(room => room.Id == roomId, update);
        }
        /*
        public async Task AddMessageToChatRoom(Message message)
        {
            var update = Builders<ChatRoom>.Update.AddToSet(room => room.Messages, message);
            await _chatRooms.UpdateOneAsync(room => room.Id == message.ChatRoomId, update);
        }
        */

        public async Task<ChatRoom> FindChatRoomAsync(string userId, string otherUserId)
    {
        var filter = Builders<ChatRoom>.Filter.Or(
            Builders<ChatRoom>.Filter.And(
                Builders<ChatRoom>.Filter.Eq(cr => cr.SellerId, userId),
                Builders<ChatRoom>.Filter.Eq(cr => cr.BuyerId, otherUserId)
            ),
            Builders<ChatRoom>.Filter.And(
                Builders<ChatRoom>.Filter.Eq(cr => cr.SellerId, otherUserId),
                Builders<ChatRoom>.Filter.Eq(cr => cr.BuyerId, userId)
            )
        );

        return await _chatRooms.Find(filter).FirstOrDefaultAsync();
    }

        public async Task AddChatRoomAsync(ChatRoom chatRoom)
    {
        await _chatRooms.InsertOneAsync(chatRoom);
    }

        // This checks if there is any order between two users, used for create chatRoom if order exist then create chatRoom if doesn't do not create
        public async Task<bool> CheckOrderBetweenTwoUsers(string userId, string otherUserId)
        {
            // Fetch all posts created by userId (freelancer)
            var userPosts = await _context.Posts
                .Where(p => p.FreelancerId == userId)
                .Select(p => p.Id)
                .ToListAsync();

            // Fetch all posts created by otherUserId (freelancer)
            var otherUserPosts = await _context.Posts
                .Where(p => p.FreelancerId == otherUserId)
                .Select(p => p.Id)
                .ToListAsync();

            // Check if there are any orders where userId is the customer and otherUserId is the freelancer
            var hasOrderByUser = await _context.Orders
                .AnyAsync(o => o.UserId == userId && otherUserPosts.Contains(o.WorkId.Value));

            // Check if there are any orders where otherUserId is the customer and userId is the freelancer
            var hasOrderByOtherUser = await _context.Orders
                .AnyAsync(o => o.UserId == otherUserId && userPosts.Contains(o.WorkId.Value));

            return hasOrderByUser || hasOrderByOtherUser;
        }

    }
}

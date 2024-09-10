namespace MB_Project.IRepos
{
    public interface IRoomRepo
    {
        Task<ChatRoom> CreateChatRoom(ChatRoom chatRoom);
        Task<List<ChatRoom>> GetChatRoomsByUserId(string userId);
        Task<ChatRoom> GetChatRoomById(string id);
        Task AddMessageToChatRoom(string roomId, string messageId);
        //Task AddMessageToChatRoom(Message message);
        Task<ChatRoom> FindChatRoomAsync(string userId, string otherUserId);
        Task AddChatRoomAsync(ChatRoom chatRoom);
        Task<bool> CheckOrderBetweenTwoUsers(string userId, string otherUserId);

    }
}

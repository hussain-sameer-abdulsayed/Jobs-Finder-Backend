using MB_Project.Models;
using MB_Project.Models.DTOS.MessageDto;

namespace MB_Project.IRepos
{
    public interface IMessageRepo
    {
        Task<List<Message>> GetMessagesByRoomId(string roomId);

        Task<Message> CreateChatMessage(string chatRoomId, string senderId, string content);
        //Task<Message> CreateChatMessage(CreateMessageDto messageDto);
        Task MarkMessageAsRead(string messageId);
        Task<Message> GetMessageByIdAsync(string messageId);
    }
}

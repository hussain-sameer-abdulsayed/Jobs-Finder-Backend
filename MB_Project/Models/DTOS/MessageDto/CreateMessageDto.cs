using MongoDB.Bson.Serialization.Attributes;

namespace MB_Project.Models.DTOS.MessageDto
{
    public class CreateMessageDto
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;
        public string RoomId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}

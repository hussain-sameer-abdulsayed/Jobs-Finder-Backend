/*
namespace MB_Project.Models
{
    public class ChatRoom
    {
        public string Username { get; set; } = string.Empty;
        public string ChatRoomId { get; set; } = string.Empty;
    }
}
*/

using MB_Project.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ChatRoom
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; }

    [BsonElement("BuyerId")]
    public string BuyerId { get; set; }

    [BsonElement("SellerId")]
    public string SellerId { get; set; }

    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("Messages")]
    public List<string> Messages { get; set; } = new List<string>(); // List of ChatMessage IDs
}

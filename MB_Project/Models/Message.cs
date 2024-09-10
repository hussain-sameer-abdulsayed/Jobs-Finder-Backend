/*
using MongoDB.Bson.Serialization.Attributes;

namespace MB_Project.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } // MongoDB's ObjectId as string
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
*/
/*
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace MB_Project.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string ChatRoomId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
*/
using MB_Project.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("ChatRoomId")]
    public string ChatRoomId { get; set; }

    [BsonElement("SenderId")]
    public string SenderId { get; set; }

    [BsonElement("Content")]
    public string Content { get; set; }

    [BsonElement("Read")]
    public bool Read { get; set; }

    [BsonElement("Timestamp")]
    public DateTime Timestamp { get; set; }
}

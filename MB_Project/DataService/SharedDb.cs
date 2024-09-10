using MB_Project.Models;
using System.Collections.Concurrent;

namespace MB_Project.DataBase
{
    public class SharedDb
    {
        private readonly ConcurrentDictionary<string, ChatRoom> _chatrooms = new(); 
        public ConcurrentDictionary<string, ChatRoom> chatrooms => _chatrooms;
    }
}

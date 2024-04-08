using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace MB_Project.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string? ProfileImage { get; set; }
        public string Bio { get; set; }
        public string? RefreeshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public List<Post>? Posts { get; set; } // one to many
        public List<Review>? Reviews { get; set; }
        public List<Order>? Orders { get; set; }
        
    }
}


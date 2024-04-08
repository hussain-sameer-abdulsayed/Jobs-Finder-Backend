using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Post>? Posts { get; set; } // many to many with ==> posts
        public bool ShowOnMain { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models
{
    public class PostImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int? PostId { get; set; }
        [ForeignKey("PostId")]
        public Post? Post { get; set; }
    }
}

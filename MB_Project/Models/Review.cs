using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Content { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }  // 1 - 5
        public int? PostId { get; set; }
        [ForeignKey("PostId")]
        public Post? Post { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}

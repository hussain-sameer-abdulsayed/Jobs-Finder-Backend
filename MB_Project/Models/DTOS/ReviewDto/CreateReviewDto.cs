using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.ReviewDto
{
    public class CreateReviewDto
    {
        [MaxLength(500)]
        public string Content { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }  // 1 - 5
        public int? PostId { get; set; }
        public string? UserId { get; set; }

    }
}

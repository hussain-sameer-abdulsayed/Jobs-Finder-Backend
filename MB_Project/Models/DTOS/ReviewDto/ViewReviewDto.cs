using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.ReviewDto
{
    public class ViewReviewDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }  // 1 - 5
        public int? WorkId { get; set; }
        public string? UserId { get; set; }

    }
}

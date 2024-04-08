using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.PostImageDto
{
    public class ViewPostImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int? PostId { get; set; }
        [ForeignKey("PostId")]
        public Post? Post { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.PostImageDto
{
    public class CreatePostImageDto
    {
        public List<IFormFile>? secondaryImages { get; set; }
        public int? WorkId { get; set; }
    }
}

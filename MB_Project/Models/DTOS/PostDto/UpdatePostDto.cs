using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.PostDto
{
    public class UpdatePostDto
    {
        [MaxLength(100)]
        public string? Title { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public IFormFile? MainImage { get; set; }
        [Range(0, 10000000)]
        public float? BasePrice { get; set; }
        public List<int>? CategoriesIds { get; set; } // many to many with ==> category
    }
}

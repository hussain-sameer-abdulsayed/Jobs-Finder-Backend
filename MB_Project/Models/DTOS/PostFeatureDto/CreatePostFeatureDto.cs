using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.PostFeatureDto
{
    public class CreatePostFeatureDto
    {
        public int? PostId { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        [Range(0, 10000000)]
        public float Price { get; set; }
    }
}

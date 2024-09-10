using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models
{
    public class PostFeature
    {
        public int Id { get; set; }
        public int? WorkId { get; set; }
        [ForeignKey("WorkId")]
        public Post? Post { get; set; }
        public string Title { get; set; }
        public float Price { get; set; }
        public List<Order>? Orders { get; set; }

    }
}

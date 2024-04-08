using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models
{
    public class PostFeature
    {
        public int Id { get; set; }
        public int? PostId { get; set; }
        [ForeignKey("PostId")]
        public Post? Post { get; set; }
        public string Title { get; set; }
        public float Price { get; set; }
        public List<Order>? Orders { get; set; }

    }
}

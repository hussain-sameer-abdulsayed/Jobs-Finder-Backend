using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models
{
    public class PostCategory
    {
        public int WorkId { get; set; }
        public Post Work { get; set; }
        [ForeignKey("WorkId")]
        public int CategoryId { get; set; }
    }
}

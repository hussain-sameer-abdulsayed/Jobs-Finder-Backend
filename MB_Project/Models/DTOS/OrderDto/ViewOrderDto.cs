using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.OrderDto
{
    public class ViewOrderDto
    {
        /*
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? PostId { get; set; }
        public float TotalPrice { get; set; }
        //public OrderrrrStatus? Status { get; set; }
        public string? OrderStatus { get; set; }
        public List<PostFeature>? PostFeatures { get; set; }
        */

        public int Id { get; set; }
        public string? userId { get; set; }
        public Post? Work { get; set; }
        public float? TotalPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public List<PostFeature>? PostFeatures { get; set; }
    }
}

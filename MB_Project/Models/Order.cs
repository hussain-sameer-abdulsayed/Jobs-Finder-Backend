using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models
{
    /*
    public enum OrderStatus
    {
        None = 0,
        PENDING,
        ACCEPTED,
        APPROVED,
        COMPLETED,
        DELIVERED,
        CANCELLED
    }
    */
   

    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }//from this you get Orders
        public int? PostId { get; set; }
        [ForeignKey("PostId")]
        public Post? Post { get; set; }
        public float? TotalPrice { get; set; }
        //public OrderrrrStatus? Status { get; set; }
        public string? OrderStatus { get; set; } = "PENDING";
        public List<PostFeature>? PostFeatures { get; set; }

    }
}

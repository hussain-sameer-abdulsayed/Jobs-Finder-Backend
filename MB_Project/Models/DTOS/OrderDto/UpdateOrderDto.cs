using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.OrderDto
{
    public class UpdateOrderDto
    {
        public int? WorkId { get; set; }
        public float TotalPrice { get; set; }
        //public OrderStatus? Status { get; set; } 

        //public List<PostFeature>? PostFeatures { get; set; } ======>> i find this ====>> post.postFeatures
    }
}

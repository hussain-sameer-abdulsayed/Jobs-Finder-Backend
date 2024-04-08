using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.OrderDto
{
    public class CreateOrderDto
    {
        public string? UserId { get; set; }
        public int PostId { get; set; }
        public List<int>? PostFeatureIds { get; set; }  
    } 
}

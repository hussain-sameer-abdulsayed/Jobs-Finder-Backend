using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.OrderDto
{
    public class CreateOrderDto
    {
        //public string? UserId { get; set; }
        public int WorkId { get; set; }
        public List<int>? FeaturesIds { get; set; }  
    } 
}

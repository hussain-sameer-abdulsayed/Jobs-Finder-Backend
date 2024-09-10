using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.PostFeatureDto
{
    public class ViewPostFeatureDto
    {
        public int Id { get; set; }
        public int? WorkId { get; set; }
        public string Title { get; set; }
        public float Price { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace MB_Project.Models.DTOS.CategoryDto
{
    public class UpdateCategoryDto
    {
        [MaxLength(100)]
        public string Name { get; set; }
        public bool ShowOnMain { get; set; }
    }
}

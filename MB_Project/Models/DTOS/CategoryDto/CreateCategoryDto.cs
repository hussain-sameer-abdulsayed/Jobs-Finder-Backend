using System.ComponentModel.DataAnnotations;

namespace MB_Project.Models.DTOS.CategoryDto
{
    public class CreateCategoryDto
    {
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s.\-']{2,}$", ErrorMessage = "Please Enter Only Letters")]
        public string Name { get; set; }
    }
}

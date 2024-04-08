using System.ComponentModel.DataAnnotations;

namespace MB_Project.Models.DTOS.UserDto
{
    public class UpdateUserDto
    {
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s.\-']{2,}$", ErrorMessage = "Please Enter Only Letters")]
        public string? Name { get; set; }
        //public string Email { get; set; }
        //public string Password { get; set; }
        [MaxLength(500)]
        public string? Bio { get; set; }
        public IFormFile? profileImage { get; set; }
    }
}



using System.ComponentModel.DataAnnotations;

namespace MB_Project.Models.DTOS.UserDto
{
    public class CreateUserDto
    {
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s.\-']{2,}$", ErrorMessage = "Please Enter Only Letters")]
        public string Name { get; set; }
        [MaxLength(50)]
        public string? UserName { get; set; }
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile? profileImage { get; set; }
        //public string? StrProfilePicture { get; set; } // to pass the name to User Image
        [MaxLength(500)]
        public string Bio { get; set; }
        public bool? isFreelancer { get; set; }
        //public CreateUserDto() 
        //{
        //    if (string.IsNullOrEmpty(UserName))
        //    {
        //        UserName = Email;
        //    }
        //}
    }
}

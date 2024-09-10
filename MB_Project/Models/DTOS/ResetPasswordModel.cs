using System.ComponentModel.DataAnnotations;

namespace MB_Project.Models.DTOS
{
    public class ResetPasswordModel
    {
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Token { get; set; }
    }
}

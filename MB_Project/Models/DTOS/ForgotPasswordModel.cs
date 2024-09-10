using System.ComponentModel.DataAnnotations;

namespace MB_Project.Models.DTOS
{
    public class ForgotPasswordModel
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}

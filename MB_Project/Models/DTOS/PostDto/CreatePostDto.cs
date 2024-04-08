using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.PostDto
{
    public class CreatePostDto
    {
        [MaxLength(100)]
        //[RegularExpression(@"^[a-zA-Z\s.\-']{2,}$", ErrorMessage = "Please Enter Only Letters")]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public IFormFile? MainImage { get; set; }
        [Range(0, 10000000,ErrorMessage ="Please Enter A Valid Number")]
        public float BasePrice { get; set; }
        public string? FreelancerId { get; set; } // user id  maybe this id send from js
        public List<int>? CategoriesIds { get; set; }
        public List<IFormFile>? SecondaryPicturesUrl { get; set; }

    }
}

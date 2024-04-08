using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models.DTOS.PostDto
{
    public class ViewPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? MainImage { get; set; }
        public float BasePrice { get; set; }
        public string? FreelancerId { get; set; } // user id 
        public List<Category>? Categories { get; set; } // many to many with ==> category
        public List<PostImage>? SecondaryImages { get; set; } // secondory images
        public List<string> images { get; set; }
        public List<Review>? Reviews { get; set; } // reviews
        public List<PostFeature>? PostFeatures { get; set; }
    }
}

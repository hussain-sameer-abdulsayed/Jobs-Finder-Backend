using System.ComponentModel.DataAnnotations.Schema;

namespace MB_Project.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? MainImage { get; set; }
        public float? BasePrice { get; set; }
        public string? FreelancerId { get; set; } // from this you get projects 
        [ForeignKey("FreelancerId")]
        public User? User { get; set; }
        public List<Category>? Categories { get; set; } // many to many with ==> category
        public List<PostImage>? SecondaryImages { get; set; } // secondory images
        public List<string>? images { get; set; } // because SecondaryImages take postimage so i create this to toke links
        public List<Review>? Reviews { get; set; } // reviews
        public List<PostFeature>? PostFeatures { get; set; }
        public List<Order>? Orders { get; set; }

    }
}

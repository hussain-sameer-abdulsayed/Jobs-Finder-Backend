namespace MB_Project.Models.DTOS.CategoryDto
{
    public class ViewCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Post>? Posts { get; set; } // many to many with ==> posts
        public bool ShowOnMain { get; set; }
    }
}

namespace MB_Project.Models.DTOS
{
    public class AddPostImages
    {
        public int WorkId { get; set; }
        public List<IFormFile> SecondaryImages { get; set; }
    }
}

using MB_Project.Models;

namespace MB_Project.IRepos
{
    public interface IPostImageRepo
    {
        Task<IEnumerable<PostImage>> GetPostImages(int postId);
        //Task<bool> Create(PostImage postImage);
        Task<bool> UpdatePostImages(int postId, IFormFile File);
        Task<bool> DeletePostImage(int postId, string imageUrl);
        Task<bool> Create(PostImage postImages);
        Task<bool> Delete(int id);
        Task<List<string>> GetPostSecondaryImagesUniqueFileNames(int PostId);
        Task<List<string>> GetPostSecondaryImages(List<string> uniqueFileName);
        Task<string> SecndarySaveUploadedFile(IFormFile file);
    }
}

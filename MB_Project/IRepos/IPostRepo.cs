using MB_Project.Models;
using MB_Project.Models.DTOS.PostDto;

namespace MB_Project.IRepos
{
    public interface IPostRepo
    {
        Task<IEnumerable<Post>> GetPosts();
        Task<string> GetUniqueFileNameForPostMainImage(int postId);
        Task<string> GetPostMainImage(string uniqueFileName);
        Task<bool> DeletePostMainImage(int PostId);
        Task<IEnumerable<Post>> GetUserPosts(string userId);
        Task<IEnumerable<Post>> SearchPosts(string name);
        Task<bool> CreatePostMainImage(int PostId,string uniqueFileName);
        Task<string> SaveUploadedFile(IFormFile file);
        Task<string> SecndarySaveUploadedFile(IFormFile file);
        Task<Post> GetPostsById(int postId);
        Task<Post> GetPostBySellerId(string userId);
        Task<List<Post>> GetPostsByCategoryId(int CategoryId);
        Task<Post> CreatePost(Post post);
        Task<bool> UpdatePost(int postId, UpdatePostDto postDto);
        Task<bool> AddPostCategory(List<PostCategory> postCategories);
        Task<bool> DeletePost(int postId);
        Task<PostImage> GetPostImage(int id);
        Task<List<PostImage>> GetPostImagesObj(int postId);
        Task<List<string>> GetPostSecondaryImagesUniqueFileNames(int PostId);
        Task<List<string>> GetPostSecondaryImages(List<string> uniqueFileName);
        Task<bool> Create(PostImage postImage);
        Task<bool> Update(int id, PostImage postImage);
        Task<bool> Delete(int id);
        void DeleteFile(string uniqueFileName);
        Task<double> GetPostRating(int postId);
    }
}

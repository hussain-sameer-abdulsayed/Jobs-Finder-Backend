using MB_Project.Models;

namespace MB_Project.IRepos
{
    public interface IReviewRepo
    {
        Task<IEnumerable<Review>> GetUserReviews(string UserId);
        Task<IEnumerable<Review>> GetPostReviews(int PostId);
        Task<Review> GetReviewById(int id);
        Task<bool> CreateReview(Review review);
        Task<bool> UpdateReview(int id,Review review);
        Task<bool> DeleteReview(int id);
    }
}

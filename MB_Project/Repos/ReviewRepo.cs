using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace MB_Project.Repos
{
    public class ReviewRepo : IReviewRepo
    {
        private readonly MB_ProjectContext _context;

        public ReviewRepo(MB_ProjectContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateReview(Review review)
        {
            try
            {
                await _context.Reviews.AddAsync(review);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteReview(int id)
        {
            try
            {
                var obj = await _context.Reviews.FindAsync(id);
                if (obj == null)
                {
                    return false;
                }
                _context.Attach(obj);
                _context.Reviews.Remove(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch 
            { 
                return false; 
            }
        }

        public async Task<IEnumerable<Review>> GetPostReviews(int PostId)
        {
            try
            {
                var post = await _context.Reviews.Where(x => x.PostId == PostId).ToListAsync();
                if (post.Count() == 0)
                {
                    return Enumerable.Empty<Review>();
                }
                return (IEnumerable<Review>)post;
            }
            catch
            {
                return Enumerable.Empty<Review>();
            }
        }

        public async Task<Review> GetReviewById(int id)
        {
            try
            {
                var obj = await _context.Reviews.Where(x=>x.Id == id).FirstOrDefaultAsync();
                if (obj == null)
                {
                    return null;
                }
                return obj;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<Review>> GetUserReviews(string UserId)
        {
            try
            {
                var post = await _context.Reviews.Where(x => x.UserId == UserId && x.PostId != null).ToListAsync();
                if (!post.Any())
                {
                    return Enumerable.Empty<Review>();
                }
                return (IEnumerable<Review>)post;
            }
            catch
            {
                return Enumerable.Empty<Review>();
            }
        }

        public async Task<bool> UpdateReview(int id, Review review)
        {
            try
            {
                var obj = await _context.Reviews.FindAsync(id);
                if (obj == null)
                {
                    return false;
                }
                _context.Attach(obj);
                obj.Content = review.Content;
                obj.Rating = review.Rating;
                await _context.SaveChangesAsync();
                return true;
            }
            catch 
            { 
                return false; 
            }
        }
    }
}

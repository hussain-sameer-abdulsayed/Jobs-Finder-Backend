using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MB_Project.Repos
{
    public class PostFeatureRepo : IPostFeatureRepo
    {
        private readonly MB_ProjectContext _context;

        public PostFeatureRepo(MB_ProjectContext context)
        {
            _context = context;
        }
        public async Task<bool> Create(List<PostFeature> postFeature)
        {
            try
            {
                _context.AttachRange(postFeature);
                await _context.PostFeatures.AddRangeAsync(postFeature);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> Delete(int PostFeatureId)
        {
            try
            {
                var obj = await _context.PostFeatures.FindAsync(PostFeatureId);
                if (obj == null)
                {
                    return false;
                }
                _context.Attach(obj);
                _context.PostFeatures.Remove(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<PostFeature> GetPostFeatureById(int id)
        {
            try
            {
                var obj = await _context.PostFeatures.FindAsync(id);
                return obj;
            }
            catch 
            {
                return null;
            }
        }
        public async Task<IEnumerable<PostFeature>> GetPostFeatures(int Postid)
        {
            try
            {
                var obj = await _context.PostFeatures.Where(x => x.WorkId == Postid).ToListAsync();
                if (obj.Count() == 0)
                {
                    return Enumerable.Empty<PostFeature>();
                }
                return (IEnumerable<PostFeature>)obj;
            }
            catch
            {
                return Enumerable.Empty<PostFeature>();
            }
        }
        public async Task<bool> Update(int id, PostFeature postFeature)
        {
            try
            {
                var obj = await _context.PostFeatures.FindAsync(id);
                if (obj == null)
                {
                    return false;
                }
                _context.Attach(obj);
                obj.Title = postFeature.Title;
                obj.Price = postFeature.Price;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }




        /*
        public async Task<IEnumerable<PostFeature>> GetAllPostFeatures()
        {
            try
            {
                var postFeatures = await _context.PostFeatures.ToListAsync();
                return postFeatures;
            }
            catch { return Enumerable.Empty<PostFeature>(); }
        }
        */
    }
}

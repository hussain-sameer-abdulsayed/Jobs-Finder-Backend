using MB_Project.Models;

namespace MB_Project.IRepos
{
    public interface IPostFeatureRepo
    {
        Task<IEnumerable<PostFeature>> GetPostFeatures(int Postid);
        Task<PostFeature> GetPostFeatureById(int id);
        Task<bool> Create(List<PostFeature> postFeature);
        Task<bool> Update(int id,PostFeature postFeature);
        Task<bool> Delete(int PostFeatureId);




        //Task<IEnumerable<PostFeature>> GetAllPostFeatures();
    }
}

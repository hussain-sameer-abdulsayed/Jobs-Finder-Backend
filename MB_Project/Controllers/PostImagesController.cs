using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS;
using MB_Project.Models.DTOS.PostImageDto;
using MB_Project.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MB_Project.Controllers
{
    [Route("api/workimages")]
    [ApiController]
    public class PostImagesController : ControllerBase
    {
        private readonly IPostRepo _postRepo;
        private readonly IPostImageRepo _postImageRepo;
        private readonly ITransactionRepo _transactionRepo;

        public PostImagesController(IPostRepo postRepo, IPostImageRepo postImageRepo, ITransactionRepo transactionRepo)
        {
            _postRepo = postRepo;
            _postImageRepo = postImageRepo;
            _transactionRepo = transactionRepo;
        }


        [AllowAnonymous]
        [HttpGet("work/{workId}")]
        public async Task<IActionResult> GetPostImages(int workId)
        {
            try
            {
                /*
                var uniqueFileNames = await _postRepo.GetPostSecondaryImagesUniqueFileNames(workId);
                var images = await _postRepo.GetPostSecondaryImages(uniqueFileNames);
                if(images == null)
                {
                    return NotFound();
                }
                return Ok(images);
                */
                var postImages = await _postRepo.GetPostImagesObj(workId);
                if (postImages == null) 
                { 
                    return NotFound();
                }
                return Ok(postImages);
            }
            catch
            {
                return BadRequest();
            }
        }


        //[Authorize(Roles ="ADMIN , SELLER")]
        [HttpPost()]
        public async Task<IActionResult> CreatePostImages([FromForm]AddPostImages addPostImages)
        {
            try
            {
                _transactionRepo.BeginTransaction();
                var images = new List<PostImage>();
                foreach (var item in addPostImages.SecondaryImages)
                {
                    var SecndaryUniqueFileName = await _postImageRepo.SecndarySaveUploadedFile(item);
                    var img = new PostImage()
                    {
                        WorkId = addPostImages.WorkId,
                        ImageUrl = SecndaryUniqueFileName
                    };
                    var chkimg = await _postImageRepo.Create(img);
                    if (chkimg == false)
                    {
                        _postRepo.DeleteFile(SecndaryUniqueFileName);
                        _transactionRepo.RollBackTransaction();
                        return BadRequest("img was not created,so post also not created");
                    }
                }
                _transactionRepo.CommitTransaction();
                return Ok("img Created");
            }
            catch
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest();
            }
        }


        [HttpDelete("{workId}")]
        public async Task<IActionResult> DeleteWorkImage(int workId,[FromBody]DeletePostImage data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _transactionRepo.BeginTransaction();
                var chk = await _postImageRepo.DeletePostImage(workId, data.ImageUrl);
                if (!chk)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest();
                }
                _transactionRepo.CommitTransaction();
                return Ok("image deleted");
            }
            catch
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest();
            }
        }
    }
}

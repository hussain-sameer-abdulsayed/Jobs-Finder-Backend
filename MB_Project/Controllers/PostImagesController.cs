using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS.PostImageDto;
using MB_Project.Repos;
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
        [HttpPost()]
        public async Task<IActionResult> CreatePostImages(CreatePostImageDto postImageDto)
        {
            try
            {
                _transactionRepo.BeginTransaction();
                var images = new List<PostImage>();
                foreach (var item in postImageDto.secondaryImages)
                {
                    var SecndaryUniqueFileName = await _postImageRepo.SecndarySaveUploadedFile(item);
                    var img = new PostImage()
                    {
                        PostId = postImageDto.workId,
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

    }
}

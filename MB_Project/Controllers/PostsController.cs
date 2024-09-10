using AutoMapper;
using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS;
using MB_Project.Models.DTOS.PostDto;
using MB_Project.Models.DTOS.UserDto;
using MB_Project.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Drawing;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MB_Project.Controllers
{
    [Route("api/works")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostRepo _postRepo;
        private readonly IPostImageRepo _postImageRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IUserRepo _userRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly MB_ProjectContext _context;
        private readonly IJWTManagerRepo _jwtManagerRepo;

        public PostsController(IMapper mapper, IPostRepo postRepo, IPostImageRepo postImageRepo, ICategoryRepo categoryRepo, IUserRepo userRepo, ITransactionRepo transactionRepo, MB_ProjectContext context, IJWTManagerRepo jwtManagerRepo)
        {
            _mapper = mapper;
            _postRepo = postRepo;
            _postImageRepo = postImageRepo;
            _categoryRepo = categoryRepo;
            _userRepo = userRepo;
            _transactionRepo = transactionRepo;
            _context = context;
            _jwtManagerRepo = jwtManagerRepo;
        }


        [AllowAnonymous]
        // GET: api/<PostController>
        [HttpGet()]
        public async Task<IActionResult> GetAllWorks()
        {
            try
            {
                var posts = await _postRepo.GetPosts();
                if (posts.Count() == 0)
                {
                    return NotFound("No posts exist");
                }
                var DtoList = new List<ViewPostDto>();
                foreach (var item in posts)
                {
                    DtoList.Add(_mapper.Map<ViewPostDto>(item));
                }
                return Ok(DtoList);
            }
            catch (Exception ex)
            {
                return BadRequest($"No posts exist:{ex.Message}");
            }
        }
        // GET api/<PostController>/5


        [AllowAnonymous]
        [HttpGet("{workId}")]
        public async Task<IActionResult> GetWorkById(int workId)
        {
            try
            {
                var post = await _postRepo.GetPostsById(workId);
                if (post == null)
                {
                    return NotFound("Work does not exist");
                }
                var Dto = _mapper.Map<ViewPostDto>(post);
                return Ok(Dto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Work does not exist:{ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("post/{postName}")]
        public async Task<IActionResult> WorkSearch(string postName)
        {
            try
            {
                var posts = await _postRepo.SearchPosts(postName);
                if (posts.Count() == 0)
                {
                    return NotFound("No work found");
                }
                var DtoList = new List<ViewPostDto>();
                foreach (var item in posts)
                {
                    DtoList.Add(_mapper.Map<ViewPostDto>(item));
                }
                return Ok(DtoList);
            }
            catch (Exception ex)
            {
                return BadRequest($"No post found:{ex.Message}");
            }
        }


        [AllowAnonymous]
        [HttpGet("freelancer/{freelancerId}")]
        public async Task<IActionResult> GetFreelancerWorks(string freelancerId)
        {
            try
            {
                var Dtolist = new List<ViewPostDto>();
                var obj = await _postRepo.GetUserPosts(freelancerId);
                if (obj == null)
                {
                    return NotFound("not found");
                }
                foreach (var item in obj)
                {
                    Dtolist.Add(_mapper.Map<ViewPostDto>(item));
                }
                return Ok(Dtolist);
            }
            catch
            {
                return BadRequest();
            }
        }


        [AllowAnonymous]
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetWorksByCategoryId(int categoryId)
        {
            try
            {
                var Posts = await _postRepo.GetPostsByCategoryId(categoryId);
                if (Posts.Count() == 0)
                {
                    return NotFound("no posts");
                }
                var Dtolist = new List<ViewPostDto>();
                foreach (var Post in Posts)
                {
                    Dtolist.Add(_mapper.Map<ViewPostDto>(Post));
                }
                return Ok(Dtolist);
            }
            catch
            {
                return BadRequest();
            }
        }

        
        //[Authorize(Roles = "SELLER")]
        // POST api/<PostController>
        [HttpPost()]
        public async Task<IActionResult> CreateWork([FromForm] CreatePostDto postDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {   
                /*
                if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValue))
                {
                    return Unauthorized();
                }
                string token = authorizationHeaderValue.ToString();
                
                var contentType = Request.ContentType;
                // Check if the Content-Type is "multipart/form-data"
                if (contentType != null && contentType.Contains("multipart/form-data"))
                {
                    // Handle form data with file uploads
                    // Access form fields from postDto as needed
                }
                
                if(!await _jwtManagerRepo.isValidToken(token))
                {
                    return Unauthorized();
                }
                */
                

                _transactionRepo.BeginTransaction();
                //var chk = await _userRepo.GetUserRoles(postDto.FreelancerId);
                //if (!chk.Contains("SELLER"))
                //{
                //    return Forbid();
                //}

                string accessToken = Request.Headers[HeaderNames.Authorization];
                var token = accessToken.Substring(7);
                var FreelancerId = _jwtManagerRepo.GetUserId(token);
                if(FreelancerId == null)
                {
                    return BadRequest("No freelancerId in the request header");
                }
                
                string uniqueFileName = await _postRepo.SaveUploadedFile(postDto.MainImage);
                if (uniqueFileName == "size")
                {
                    _postRepo.DeleteFile(uniqueFileName);
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("The size of the image must be less than 5MB");
                }
                var post = _mapper.Map<Post>(postDto);
                post.MainImage = uniqueFileName;
                /*
                var Catlist = new List<Category>();
                foreach (var cat in postDto.CategoryId)
                {
                    Catlist.Add(await _categoryRepo.GetCategoryById(cat));
                }
                post.Categories = Catlist;
                */

                post.FreelancerId = FreelancerId;
                var postCreated = await _postRepo.CreatePost(post);
                if (postCreated == null) // 0 means fail
                {
                    _postRepo.DeleteFile(uniqueFileName);
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("Post was not created");
                }
                var PstCat = new List<PostCategory>();
                foreach (var cat in postDto.CategoriesIds)
                {
                    var postCategory = new PostCategory
                    {
                        CategoryId = cat,
                        WorkId = postCreated.Id
                    };
                    PstCat.Add(postCategory);
                }
                var chkPostCategories = await _postRepo.AddPostCategory(PstCat);
                if (!chkPostCategories)
                {
                    _postRepo.DeleteFile(uniqueFileName);
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("post was not created");
                }
                /*
                foreach (var item in postDto.SecondaryPicturesUrl)
                {
                    var SecndaryUniqueFileName = await _postRepo.SecndarySaveUploadedFile(item);
                    var img = new PostImage()
                    {
                        PostId = post.Id,
                        ImageUrl = SecndaryUniqueFileName
                    };
                    var chkimg = await _postRepo.Create(img);
                    if (chkimg == false)
                    {
                        _postRepo.DeleteFile(uniqueFileName);
                        _transactionRepo.RollBackTransaction();
                        return NotFound("img was not created,so post also not created");
                    }
                }
                */
                _transactionRepo.CommitTransaction();
                return Ok(postCreated);
            }
            catch (Exception ex)
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest($"post was not created:{ex.Message}");
            }
        }
        
        
        //[Authorize(Roles = "ADMIN , SELLER")]
        // PUT api/<PostController>/5
        [HttpPut("{workId}")]
        public async Task<IActionResult> UpdateWork(int workId, [FromForm] UpdatePostDto postDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _transactionRepo.BeginTransaction();
                var postUpdated = await _postRepo.UpdatePost(workId, postDto);
                if (!postUpdated)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("Work was not updated");
                }
                _transactionRepo.CommitTransaction();
                return Ok("Work updated");
            }
            catch (Exception ex)
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest($"Work was not updated:{ex.Message}");
            }
        }
        
        
        //[Authorize(Roles = "ADMIN , SELLER")]
        // DELETE api/<PostController>/5
        [HttpDelete("{workId}")]
        public async Task<IActionResult> DeleteWork(int workId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _transactionRepo.BeginTransaction();
                var imageDeleted = await _postRepo.DeletePostMainImage(workId);
                if (!imageDeleted)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("Image was not deleted, so work was not deleted!");
                }
                var postDeleted = await _postRepo.DeletePost(workId);
                if (!postDeleted)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("Work was not deleted");
                }
                _transactionRepo.CommitTransaction();
                return Ok("Work deleted");
            }
            catch (Exception ex)
            { 
                _transactionRepo.RollBackTransaction();
                return BadRequest($"Work was not deleted:{ex.Message}");
            }
        }





        




        /*
        [Authorize]
        [HttpPut("UpdatePostMainImage")]
        public async Task<IActionResult> UpdatePostMainImage([FromForm]int PostId,IFormFile File)
        {
            try
            {
                string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images/post");
                var oldUniqueFileName = await _postRepo.GetUniqueFileNameForPostMainImage(PostId);
                if (oldUniqueFileName == null)
                {
                    return NotFound("not found");
                }
                var chkDelete = await _postRepo.DeletePostMainImage(Directory, oldUniqueFileName, PostId);
                if (chkDelete == false)
                {
                    return NotFound("error with deletion");
                }
                var NewUniqueFileName = await SaveUploadedFile(File);
                if (NewUniqueFileName == "size") 
                {
                    return BadRequest("The size of the image must be less than 5MB");
                }
                var chk = await _postRepo.CreatePostMainImage(PostId, NewUniqueFileName);
                if (chk == false)
                {
                    return NotFound("image was not updated");
                }
                return Ok("image updated");
            }
            catch
            {
                return BadRequest("image was not updated");
            }

        }
        */
        /*
        [HttpGet("GetPostMainImage")]
        public async Task<IActionResult> GetMainImage(int PostId)
        {
            try
            {
                var uniqueFileName = await _postRepo.GetUniqueFileNameForPostMainImage(PostId);
                if(uniqueFileName == null)
                {
                    return NotFound("not found");
                }
                var imageUrl = await _postRepo.GetPostMainImage(uniqueFileName);
                return Ok(imageUrl);
            }
            catch 
            { 
                return BadRequest(); 
            }
        }
        */
        /*
        [HttpGet("GetPostSecondaryImages")]
        public async Task<IActionResult> GetPostSecondaryImages(int PostId)
        {
            try
            {
                var PostSecondaryUrls = await _postRepo.GetPostSecondaryImagesUniqueFileNames(PostId);
                if(PostSecondaryUrls.Count() == 0)
                {
                    return NotFound("not found");
                }
                var PostSecondaryImagesLinks = await _postRepo.GetPostSecondaryImages(PostSecondaryUrls);
                return Ok(PostSecondaryImagesLinks);
            }
            catch { return BadRequest(); }
        }
        
        */
        /*
        [HttpDelete("DeletePostMainImage")]
        public async Task<IActionResult> DeletePostMainImage(int PostId) 
        {
            try
            {
                string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images/post");
                var uniqueFileName = await _postRepo.GetUniqueFileNameForPostMainImage(PostId);
                if(uniqueFileName == null)
                {
                    return NotFound("not found");
                }
                var chkDelete = await _postRepo.DeletePostMainImage(Directory, uniqueFileName,PostId);
                if(chkDelete == false)
                {
                    return NotFound("error with deletion");
                }
                return Ok("image deleted");
            }
            catch
            {
                return BadRequest();
            }
        }
        */
        /*
        [HttpPost("CreatePostMainImage")]
        public async Task<IActionResult> CreatePostMainImage([FromForm]int PostId,IFormFile File)
        {
            try
            {
                var uniqueFileName = await SaveUploadedFile(File);
                var chk = await _postRepo.CreatePostMainImage(PostId, uniqueFileName);
                if(chk == false)
                {
                    return NotFound("image was not added");
                }
                return Ok("image added");
            }
            catch { return BadRequest(); }
        }
        */
    }
}

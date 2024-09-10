using AutoMapper;
using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS.ReviewDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MB_Project.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IReviewRepo _reviewRepo;
        private readonly ITransactionRepo _transactionRepo;
        public ReviewsController(IMapper mapper, IReviewRepo reviewRepo, ITransactionRepo transactionRepo)
        {
            _mapper = mapper;
            _reviewRepo = reviewRepo;
            _transactionRepo = transactionRepo;
        }




        [AllowAnonymous]
        // GET: api/<ReviewController>
        [HttpGet("PostReviews/{PostId}")]
        public async Task<IActionResult> GetAllPostReviews(int PostId)
        {
            try
            {
                var Dtolist = new List<ViewReviewDto>();
                //var post = _context.Posts.Include(x => x.Reviews).Where(c=>c.Id== PostId).ToListAsync();
                var posts = await _reviewRepo.GetPostReviews(PostId);
                if(!posts.Any())
                {
                    return NotFound("not found");
                }
                foreach(var item in posts)
                {
                    Dtolist.Add(_mapper.Map<ViewReviewDto>(item));
                }
                return Ok(Dtolist);
            }
            catch
            {
                return BadRequest();
            }
        }


        //[Authorize]
        // GET api/<ReviewController>/5
        [HttpGet("UserReviews/{UserId}")]
        public async Task<IActionResult> GetUserReviews(string UserId)
        {
            try
            {
                var DtoList = new List<ViewReviewDto>();
                var reviews = await _reviewRepo.GetUserReviews(UserId);
                if (!reviews.Any())
                {
                    return NotFound("not found");
                }
                foreach (var item in reviews)
                {
                    DtoList.Add(_mapper.Map<ViewReviewDto>(item));
                }
                return Ok(DtoList);
            }
            catch
            {
                return BadRequest();
            }
        }


        [AllowAnonymous]
        [HttpGet("{ReviewId}")]
        public async Task<IActionResult> GetReview(int ReviewId)
        {
            try
            {
                var obj = await _reviewRepo.GetReviewById(ReviewId);
                if (obj == null)
                {
                    return NotFound("not found");
                }
                var Dto = _mapper.Map<ViewReviewDto>(obj);
                return Ok(Dto);
            }
            catch
            {
                return BadRequest();
            }
        }
        

        //[Authorize]
        // POST api/<ReviewController>
        [HttpPost()]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _transactionRepo.BeginTransaction();
                var obj = _mapper.Map<Review>(reviewDto);
                var reviewCreated = await _reviewRepo.CreateReview(obj);
                if(!reviewCreated)
                {
                    _transactionRepo.RollBackTransaction();
                    return NotFound("Review was not posted");
                }
                _transactionRepo.CommitTransaction();
                return Ok("Review was posted");
            }
            catch (Exception ex)
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest($"Review was posted:{ex.Message}");
            }
        }


        //[Authorize]
        // PUT api/<ReviewController>/5
        [HttpPut("{ReviewId}")]
        public async Task<IActionResult> UpdateReview(int ReviewId, [FromBody] UpdateReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _transactionRepo.BeginTransaction();
                var Review = _mapper.Map<Review>(reviewDto);
                var reviewUpdated = await _reviewRepo.UpdateReview(ReviewId, Review);
                if(!reviewUpdated)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("Review was not updated");
                }
                _transactionRepo.CommitTransaction();
                return Ok("Review was updated");
            }
            catch (Exception ex)
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest($"Review was not updated:{ex.Message}");
            }
        }


        //[Authorize]
        // DELETE api/<ReviewController>/5
        [HttpDelete("{ReviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _transactionRepo.BeginTransaction();
                var reviewDeleted = await _reviewRepo.DeleteReview(reviewId);
                if (!reviewDeleted)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("Review was not deleted");
                }
                _transactionRepo.CommitTransaction();
                return Ok("Review was deleted");
            }
            catch (Exception ex)
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest($"Review was not deleted:{ex.Message}");
            }
        }
    }
}

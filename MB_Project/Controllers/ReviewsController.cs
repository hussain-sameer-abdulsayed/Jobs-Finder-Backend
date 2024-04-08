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
        //[Authorize]
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


        //[AllowAnonymous]
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _transactionRepo.BeginTransaction();
                var obj = _mapper.Map<Review>(reviewDto);
                var chk = await _reviewRepo.CreateReview(obj);
                if(chk == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return NotFound("not found");
                }
                _transactionRepo.CommitTransaction();
                return Ok();
            }
            catch
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest();
            }
        }


        //[Authorize]
        // PUT api/<ReviewController>/5
        [HttpPut("{ReviewId}")]
        public async Task<IActionResult> UpdateReview(int ReviewId, [FromBody] UpdateReviewDto reviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _transactionRepo.BeginTransaction();
                var Rev = _mapper.Map<Review>(reviewDto);
                var chk = await _reviewRepo.UpdateReview(ReviewId, Rev);
                if(chk == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return NotFound("not found");
                }
                _transactionRepo.CommitTransaction();
                return Ok();
            }
            catch
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest();
            }
        }


        //[Authorize]
        // DELETE api/<ReviewController>/5
        [HttpDelete("{ReviewId}")]
        public async Task<IActionResult> Delete(int ReviewId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _transactionRepo.BeginTransaction();
                var chk = await _reviewRepo.DeleteReview(ReviewId);
                if (chk == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return NotFound("not found");
                }
                _transactionRepo.CommitTransaction();
                return Ok();
            }
            catch
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest();
            }
        }
    }
}

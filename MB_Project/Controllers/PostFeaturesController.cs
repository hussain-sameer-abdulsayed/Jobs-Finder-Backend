using AutoMapper;
using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS.PostFeatureDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MB_Project.Controllers
{
    [Route("api/workfeatures")]
    [ApiController]
    public class PostFeaturesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostFeatureRepo _postFeatureRepo;
        private readonly ITransactionRepo _transactionRepo;

        public PostFeaturesController(IMapper mapper, IPostFeatureRepo postFeatureRepo, ITransactionRepo transactionRepo)
        {
            _mapper = mapper;
            _postFeatureRepo = postFeatureRepo;
            _transactionRepo = transactionRepo;
        }
        //[Authorize]
        // GET api/<PostFeatureController>/5
        [HttpGet("{PostId}")]
        public async Task<IActionResult> GetPostFeatures(int PostId)
        {
            try
            {
                var Dtolist = new List<ViewPostFeatureDto>();
                var obj = await _postFeatureRepo.GetPostFeatures(PostId);
                if (obj.Count() == 0)
                {
                    return NotFound("not found");
                }
                foreach (var item in obj)
                {
                    Dtolist.Add(_mapper.Map<ViewPostFeatureDto>(item));
                }
                return Ok(Dtolist);
            }
            catch
            {
                return BadRequest();
            }
        }


        //[Authorize]
        // POST api/<PostFeatureController>
        [HttpPost()]
        public async Task<IActionResult> CreatePostFeature([FromBody] CreatePostFeatureDto postFeatureDto)
        {
            
            try
            {
                _transactionRepo.BeginTransaction();
                // check if post exist or not ==> add IpostRepo 
                //var post = 
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var obj = _mapper.Map<PostFeature>(postFeatureDto);
                var chk = await _postFeatureRepo.Create(obj);
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
        // PUT api/<PostFeatureController>/5
        [HttpPut("{PostFeatureId}")]
        public async Task<IActionResult> UpdatePostFeature(int PostFeatureId, [FromBody] UpdatePostFeatureDto postFeatureDto)
        {
            
            try
            {
                _transactionRepo.BeginTransaction();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var obj = _mapper.Map<PostFeature>(postFeatureDto);
                var chk = await _postFeatureRepo.Update(PostFeatureId, obj);
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
        // DELETE api/<PostFeatureController>/5
        [HttpDelete("{PostFeatureId}")]
        public async Task<IActionResult> DeletePostFeature(int PostFeatureId)
        {
            
            try
            {
                _transactionRepo.BeginTransaction();
                var chk = await _postFeatureRepo.Delete(PostFeatureId);
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








        /*
       [Authorize]
       [HttpGet("GetAllPostFeatures")]
       public async Task<IActionResult> GetAll()
       {
           try
           {
               var dto = new List<ViewPostFeatureDto>();
               var objlist = await _postFeatureRepo.GetAllPostFeatures();
               if (objlist.Count() == 0)
               {
                   return NotFound("no post features");
               }
               foreach(var item in objlist)
               {
                   dto.Add(_mapper.Map<ViewPostFeatureDto>(item));
               }
               return Ok(dto);
           }
           catch
           {
               return BadRequest();
           }
       }
       */
        /*
        [Authorize]
        // GET: api/<PostFeatureController>
        [HttpGet("GetPostFeatureById/{PostFeatureId}")]
        public async Task<IActionResult> GetPostFeatureById(int PostFeatureId)
        {
            try
            {
                var obj = await _postFeatureRepo.GetPostFeatureById(PostFeatureId);
                if (obj == null)
                {
                    return NotFound("not found");
                }
                var Dto = _mapper.Map<ViewPostFeatureDto>(obj);
                return Ok(Dto);
            }
            catch
            {
                return BadRequest();
            }
        }
        */
    }
}

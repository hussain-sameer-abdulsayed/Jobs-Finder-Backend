using AutoMapper;
using Mapster;
using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS.CategoryDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MB_Project.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepo _categoryRepo;
        private readonly ITransactionRepo _transactionRepo;

        public CategoriesController(IMapper mapper, ICategoryRepo categoryRepo, ITransactionRepo transactionRepo)
        {
            _mapper = mapper;
            _categoryRepo = categoryRepo;
            _transactionRepo = transactionRepo;
        }

        // GET: api/<CategoryController>
        [HttpGet()]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var DtoList = new List<ViewCategoryDto>();
                var obj = await _categoryRepo.GetAllCategories();
                if (obj.Count() == 0) 
                {
                    return NotFound();
                }
                foreach (var item in obj)
                {
                    DtoList.Add(_mapper.Map<ViewCategoryDto>(item));
                }
                return Ok(DtoList);
            }
            catch
            {
                return BadRequest();
            }
        }


        
        // GET api/<CategoryController>/5
        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            try
            {
                var obj = await _categoryRepo.GetCategoryById(categoryId);
                if (obj == null)
                {
                    return NotFound();
                }
                var Dto = _mapper.Map<ViewCategoryDto>(obj);
                return Ok(Dto);
            }
            catch
            {
                return BadRequest();
            }
        }


        
        [HttpGet("category/{categoryName}")]
        public async Task<IActionResult> GetCategoryByName(string categoryName)
        {
            try
            {
                var obj = await _categoryRepo.GetCategoryByName(categoryName);
                if (obj == null)
                {
                    return NotFound();
                }
                return Ok(obj);
            }
            catch
            {
                return BadRequest();
            }
        }


        //[Authorize(Roles ="ADMIN")]
        // POST api/<CategoryController>
        [HttpPost()]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            
            try
            {
                _transactionRepo.BeginTransaction();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var category = _mapper.Map<Category>(categoryDto);
                var chk = await _categoryRepo.CreateCategory(category);
                if(chk == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return NotFound();
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


        //[Authorize(Roles = "ADMIN")]
        // PUT api/<CategoryController>/5
        [HttpPut("{Categoryid}")]
        public async Task<IActionResult> UpdateCategory(int Categoryid, [FromBody] UpdateCategoryDto categoryDto)
        {
            try
            {
                _transactionRepo.BeginTransaction();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var category = _mapper.Map<Category>(categoryDto);
                var chk = await _categoryRepo.UpdateCategory(Categoryid,category);
                if(chk == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return NotFound();
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


        //[Authorize(Roles = "ADMIN")]
        // DELETE api/<CategoryController>/5
        [HttpDelete("{CategoryId}")]
        public async Task<IActionResult> DeleteCategory(int CategoryId)
        {
            
            try
            {
                _transactionRepo.BeginTransaction();
                var chk = await _categoryRepo.DeleteCategoryById(CategoryId);
                if(chk == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return NotFound();
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

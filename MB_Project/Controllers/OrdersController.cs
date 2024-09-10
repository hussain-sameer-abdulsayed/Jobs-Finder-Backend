using AutoMapper;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS;
using MB_Project.Models.DTOS.OrderDto;
using MB_Project.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MB_Project.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IMapper _mapper;
        private readonly IPostRepo _postRepo;
        private readonly IPostFeatureRepo _postFeatureRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IJWTManagerRepo _jwtManagerRepo;
        public OrdersController(IOrderRepo orderRepo, IMapper mapper, IPostRepo postRepo, IPostFeatureRepo postFeatureRepo, ITransactionRepo transactionRepo, IJWTManagerRepo jwtManagerRepo)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _postRepo = postRepo;
            _postFeatureRepo = postFeatureRepo;
            _transactionRepo = transactionRepo;
            _jwtManagerRepo = jwtManagerRepo;
        }
        private bool IsValidOrderStatus(string userInput)
        {
            string[] validStatuses = { "PENDING", "ACCEPTED", "APPROVED", "COMPLETED", "DELIVERED", "CANCELLED" };
            return validStatuses.Contains(userInput);
        }

        // add update order
        



        //[Authorize(Roles = "ADMIN")]
        // GET: api/<OrderController>
        [HttpGet()]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var DtoList = new List<ViewOrderDto>();
                var orders = await _orderRepo.GetAllOrders();
                if(orders.Count() == 0)
                {
                    return NotFound("not found");
                }
                foreach (var order in orders)
                {
                    var post = await _postRepo.GetPostsById((int)order.WorkId);
                    var neworder = new ViewOrderDto
                    {
                        Id = order.Id,
                        Work = post,
                        Status = order.OrderStatus,
                        Description = post.Description,
                        userId = order.UserId,
                        CreatedAt = DateTime.Now,
                        TotalPrice = order.TotalPrice,
                    };
                    DtoList.Add(neworder);
                        //DtoList.Add(_mapper.Map<ViewOrderDto>(order));
                }
                return Ok(DtoList);
            }
            catch
            {
                return BadRequest();
            }
        }


        //[Authorize(Roles ="ADMIN , SELLER")]
        [HttpGet("freelancer")]
        public async Task<IActionResult> GetFreelancerProjects()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string accessToken = Request.Headers[HeaderNames.Authorization];
                var token = accessToken.Substring(7);
                var FreelancerId = _jwtManagerRepo.GetUserId(token);
                if (FreelancerId == null)
                {
                    return BadRequest("no userId");
                }
                var projects = await _orderRepo.GetSellerProjects(FreelancerId);
                if (projects.Count()==0) 
                {
                    return NotFound();
                }
                var DtoList = new List<ViewOrderDto>();
                foreach (var project in projects)
                {
                    var post = await _postRepo.GetPostsById((int)project.WorkId);
                    var neworder = new ViewOrderDto
                    {
                        Id = project.Id,
                        Work = post,
                        Status = project.OrderStatus,
                        Description = post.Description,
                        userId = project.UserId,
                        CreatedAt = DateTime.Now,
                        TotalPrice = project.TotalPrice
                    };
                    DtoList.Add(neworder);

                }
                return Ok(DtoList);
            }
            catch
            {
                return BadRequest();
            }
        }


        //[Authorize]
        // GET api/<OrderController>/5
        [HttpGet("user")]
        public async Task<IActionResult> GetUserOrders()
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string accessToken = Request.Headers[HeaderNames.Authorization];
                var token = accessToken.Substring(7);
                var UserId = _jwtManagerRepo.GetUserId(token);
                if (UserId == null)
                {
                    return BadRequest("no userId");
                }
                
 
                var DtoList = new List<ViewOrderDto>();
                var orders = await _orderRepo.GetUserOrders(UserId);
                if(orders.Count() == 0)
                {
                    return NotFound("not found");
                }
                foreach (var order in orders)
                {
                    var post = await _postRepo.GetPostsById((int)order.WorkId);
                    var neworder = new ViewOrderDto
                    {
                        Id = order.Id,
                        Work = post,
                        Status = order.OrderStatus,
                        Description = post.Description,
                        userId = order.UserId,
                        CreatedAt = DateTime.Now,
                        TotalPrice = order.TotalPrice
                    };
                    DtoList.Add(neworder);
                    //DtoList.Add(_mapper.Map<ViewOrderDto>(order));
                }
                return Ok(DtoList);
                Post work = new Post
                {
                    Id = 500,
                    FreelancerId = "bc43fdd2-ba0d-4e84-9009-d72e1bddf92f",
                    Title = "Test",
                    Description = "Test", 
                    MainImage = "Test",
                    BasePrice = 1000
                };
                var Test = new TestRoderModel
                {
                    Id = 1,
                    TotalPrice = 10,
                    Description = "Test",
                    FreelancerId = "bc43fdd2-ba0d-4e84-9009-d72e1bddf92f",
                    CreatedAt = DateTime.Now,
                    Status = "PENDING",
                    post = work
                };
                
                return Ok(Test);
            }
            catch 
            { 
                return BadRequest(); 
            }
        }
        

        //[Authorize]
        // POST api/<OrderController>
        [HttpPost()]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            
            try
            {
                _transactionRepo.BeginTransaction();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string accessToken = Request.Headers[HeaderNames.Authorization];
                var token = accessToken.Substring(7);
                var UserId = _jwtManagerRepo.GetUserId(token);
                if (UserId == null)
                {
                    return BadRequest("no userId");
                }


                var order = _mapper.Map<Order>(orderDto);
                order.UserId = UserId;
                var work = await _postRepo.GetPostsById(orderDto.WorkId);
                order.WorkId = work.Id;
                //order.OrderStatus = "PENDING";
                float pstFeat = 0;
                if(orderDto.FeaturesIds != null)
                {
                    var postFeatures = new List<PostFeature>();
                    foreach (var feature in orderDto.FeaturesIds)
                    {
                        var postfeature = await _postFeatureRepo.GetPostFeatureById(feature);
                        postFeatures.Add(postfeature);
                        pstFeat += postfeature.Price;
                    }
                    order.PostFeatures = postFeatures;
                }
                order.TotalPrice = work.BasePrice + pstFeat;
                var chk = await _orderRepo.CreateOrder(order);
                if(chk == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return NotFound("Order was not created");
                }
                _transactionRepo.CommitTransaction();
                return Ok("Order created");
            }
            catch 
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest(); 
            }
        }


        //[Authorize(Roles = "ADMIN , SELLER")]
        [HttpPut("{OrderId}/{Status}")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] string OrderStatus, int OrderId)
        {

            try
            {
                _transactionRepo.BeginTransaction();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var chkInput = IsValidOrderStatus(OrderStatus.ToUpper());
                if (!chkInput)
                {
                    return BadRequest("Syntax Error in Order Status");
                }
                var chk = await _orderRepo.UpdateOrderStatus(OrderId, OrderStatus.ToUpper());
                if (chk == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("Order Status was not Updated");
                }
                _transactionRepo.CommitTransaction();
                return Ok("Order Status Updated");
            }
            catch
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest("Order Status was not Updated");
            }
        }


        //[Authorize(Roles ="ADMIN")]
        // DELETE api/<OrderController>/5
        [HttpDelete("{OrderId}")]
        public async Task<IActionResult> DeleteOrder(int OrderId)
        {
            
            try
            {
                _transactionRepo.BeginTransaction();
                var chk = await _orderRepo.DeleteOrder(OrderId);
                if(chk == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return NotFound("order was not deleted");
                }
                _transactionRepo.CommitTransaction();
                return Ok("Order Deleted");
            }
            catch 
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest();
            }
        }


        /*
        //[Authorize(Roles = "ADMIN , SELLER")]
        [HttpGet("{PostId}")]
        public async Task<IActionResult> GetPostOrders(int PostId)
        {
            try
            {
                var DtoList = new List<ViewOrderDto>();
                var orders = await _orderRepo.GetPostOrders(PostId);
                if (orders.Count() == 0)
                {
                    return NotFound("not found");
                }
                foreach (var order in orders)
                {
                    DtoList.Add(_mapper.Map<ViewOrderDto>(order));
                }
                return Ok(DtoList);
            }
            catch
            {
                return BadRequest();
            }
        }


        //[Authorize]
        [HttpGet("{OrderId}")]
        public async Task<IActionResult> GetOrderById(int OrderId)
        {
            try
            {
                var order = await _orderRepo.GetOrderById(OrderId);
                if(order == null)
                {
                    return NotFound("not found");
                }
                var Dto = _mapper.Map<ViewOrderDto>(order);
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

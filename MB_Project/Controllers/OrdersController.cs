using AutoMapper;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS.OrderDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public OrdersController(IOrderRepo orderRepo, IMapper mapper, IPostRepo postRepo, IPostFeatureRepo postFeatureRepo, ITransactionRepo transactionRepo)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _postRepo = postRepo;
            _postFeatureRepo = postFeatureRepo;
            _transactionRepo = transactionRepo;
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
                        DtoList.Add(_mapper.Map<ViewOrderDto>(order));
                }
                return Ok(DtoList);
            }
            catch
            {
                return BadRequest();
            }
        }


        [HttpPut("{OrderId}/{Status}")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody]string OrderStatus,int OrderId)
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
                if(chk == false)
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

        [HttpGet("user/projects/{freelancerId}")]
        public async Task<IActionResult> GetFreelancerProjects(string freelancerId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var projects = await _orderRepo.GetSellerProjects(freelancerId);
                if (projects.Count()==0) 
                {
                    return NotFound();
                }
                return Ok(projects);
            }
            catch
            {
                return BadRequest();
            }
        }

        //[Authorize]
        // GET api/<OrderController>/5
        [HttpGet("user")]
        public async Task<IActionResult> GetUserOrders(string UserId)
        {
            try
            {
                var DtoList = new List<ViewOrderDto>();
                var orders = await _orderRepo.GetUserOrders(UserId);
                if(orders.Count() == 0)
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
                var order = _mapper.Map<Order>(orderDto);
                var post = await _postRepo.GetPostsById(orderDto.PostId);
                order.PostId = post.Id;
                //order.OrderStatus = "PENDING";
                float pstFeat = 0;
                if(orderDto.PostFeatureIds != null)
                {
                    var postFeatures = new List<PostFeature>();
                    foreach (var feature in orderDto.PostFeatureIds)
                    {
                        var postfeature = await _postFeatureRepo.GetPostFeatureById(feature);
                        postFeatures.Add(postfeature);
                        pstFeat += postfeature.Price;
                    }
                    order.PostFeatures = postFeatures;
                }
                order.TotalPrice = post.BasePrice + pstFeat;
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
    }
}

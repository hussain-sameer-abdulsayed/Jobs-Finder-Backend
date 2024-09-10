using Microsoft.AspNetCore.Mvc;

namespace MB_Project.Controllers
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers
{
    public class ReservationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

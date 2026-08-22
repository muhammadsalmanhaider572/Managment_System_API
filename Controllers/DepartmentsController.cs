using Microsoft.AspNetCore.Mvc;

namespace Managment_System_API.Controllers
{
    public class DepartmentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

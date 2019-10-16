using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebSite.Controllers
{
    public class AdminController : Controller
    {
        [Route("admin")]
        public async Task<IActionResult> Index() => View("~/Views/Admin/Index.cshtml");

        [Route("admin/post/create")]
        public async Task<IActionResult> CreatePost() => View("~/Views/Admin/NewPost.cshtml");

        [Route("admin/vacancy/create")]
        public async Task<IActionResult> CreateVacancy() => View("~/Views/Admin/NewVacancy.cshtml");
    }
}
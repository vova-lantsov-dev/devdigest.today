using Microsoft.AspNetCore.Mvc;

namespace WebSite.Controllers;

public class AdminController : Controller
{
    [Route("admin")]
    public Task<IActionResult> Index() => Task.FromResult<IActionResult>(View("~/Views/Admin/Index.cshtml"));

    [Route("admin/post/create")]
    public Task<IActionResult> CreatePost() => Task.FromResult<IActionResult>(View("~/Views/Admin/NewPost.cshtml"));
}
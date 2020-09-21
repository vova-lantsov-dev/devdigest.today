using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebSite.ViewModels;

namespace WebSite.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("info")]
        public ActionResult Info(int? code)
        {
            ViewBag.Title = "Error";

            var statusCode = code ?? HttpContext.Response.StatusCode;
            
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode,
                Description = GetDescription(statusCode)
            };
            
            return View(model);
        }

        private static string GetDescription(int code)
        {
            switch (code)
            {
                case (int) HttpStatusCode.NotFound:
                    return "Page not found";
                default:
                    return "System error";
            }
        }
    }
}
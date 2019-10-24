using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Managers;
using Core.Managers.Crosspost;
using Core.ViewModels;
using Core.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebSite.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        public ErrorController()
        {
        }

        [Route("info")]
        public ActionResult Info()
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = HttpContext.Response.StatusCode,
                Description = GetDescription(HttpContext.Response.StatusCode)
            };
            
            return View(model);
        }

        private static string GetDescription(int code)
        {
            if (code == (int) HttpStatusCode.NotFound) 
                return "Page not found";

            return "System error";
        }
    }
}
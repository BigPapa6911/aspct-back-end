using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aspcts_backend.Controllers
{
    [Route("[controller]")]
    public class InterventionPlansController : Controller
    {
        private readonly ILogger<InterventionPlansController> _logger;

        public InterventionPlansController(ILogger<InterventionPlansController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}
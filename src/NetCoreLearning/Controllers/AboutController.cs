using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NetCoreLearning.Controllers
{
    //[Route("attribute/[controller]/[action]")]    
    //[Route("test")]
    [Route("[controller]")]
    public class AboutController : Controller
    {
        // GET: /<controller>/
        [Route("[action]")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Country")]
        public ContentResult Country()
        {
            return Content("Taiwan");
        }

        [Route("NotTest")]
        public ContentResult Test()
        {
            return Content("This is actually a test!");
        }
    }
}

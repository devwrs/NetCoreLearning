using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

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

        [Route("[action]")]
        public async Task<IActionResult> AsyncRequestTest()
        {
            //int result = await SimplyWaitTask(2000).ConfigureAwait(false);
            //int result = SimplyWaitTask(2000);
            //return Content($"waiting {result} milliseconds to complete the request.");
            int result = await AddComputationAsync(1, 2).ConfigureAwait(false);
            return Content($"1 + 2 = {result}");
        }

        private int SimplyWaitTask(int milliSeconds)
        {
            //var result = Task.Delay(milliSeconds).ConfigureAwait(false);                     
            //await result;
            Task.Delay(milliSeconds).Wait();
            return milliSeconds;            
        }

        private async Task<int> AddComputationAsync(int x, int y)
        {
            var task = Task.Run(() => x + y);            
            return await task.ConfigureAwait(false);
        }
    }
}

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using app1.Telemetry;

namespace app1.Controllers
{
    [ApiController]
    [Route("/api")]
    public class ApiController : ControllerBase
    {

        private static readonly Random _rnd;
        private static HttpClient client = new HttpClient();
        private readonly ILogger<ApiController> _logger;

        static ApiController()
        {
            _rnd = new Random();
        }

        public ApiController(ILogger<ApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var tasks = new[]
            {
                CallApp2(),
                CallApp2(),
                CallApp2(),
            };
                
            await Foo();
            await Task.WhenAll(tasks);
            
            return "123";
        }

        private static async Task Foo()
        {
            using var activity = TracerSingleton.Tracer.StartActivity("foo");

            if (activity == null)
            {
                Console.WriteLine("ACTIVITY IS NULL - REQUEST NOT SAMPLED");
            }

            var sleep = _rnd.Next(100, 1000);
            await Task.Delay(sleep);
            activity?.AddEvent(new ActivityEvent($"first sleep done in {sleep}"));
            await Bar();
        }

        private static async Task Bar()
        {
            using var activity = TracerSingleton.Tracer.StartActivity("bar");
            await Task.Delay(_rnd.Next(100, 300));
        }

        private static Task CallApp2()
        {
            return client.GetAsync(new Uri("http://localhost:5002/api"));
        }
    }
}

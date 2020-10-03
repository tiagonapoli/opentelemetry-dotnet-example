using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using app2.Telemetry;

namespace app2.Controllers
{
    [ApiController]
    [Route("/api")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            if (new Random().Next(0, 10) < 4)
            {
                throw new Exception("wololo");
            }
            
            await Baz();
            return "123";
        }
        
        private static async Task Baz()
        {
            using var activity = TracerSingleton.Tracer.StartActivity("baz");

            if (activity == null)
            {
                Console.WriteLine("ACTIVITY IS NULL - REQUEST NOT SAMPLED");
            }

            var sleep = new Random().Next(10, 50);
            await Task.Delay(sleep);
            activity?.AddEvent(new ActivityEvent($"sleep done in {sleep}"));
        }
    }
}

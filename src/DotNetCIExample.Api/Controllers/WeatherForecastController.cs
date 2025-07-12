using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace DotNetCIExample.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        public WeatherForecastController()
        {
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = GetSecureRandomInt(-20, 55),
                Summary = Summaries[GetSecureRandomInt(0, Summaries.Length)]
            })
            .ToArray();
        }

        private static int GetSecureRandomInt(int minValue, int maxValue)
        {
            return RandomNumberGenerator.GetInt32(minValue, maxValue);

        }
      
    }
}

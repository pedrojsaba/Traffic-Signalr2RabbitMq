using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Traffic.Application;
using Traffic.Application.Dtos;

namespace Traffic.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrafficController : ControllerBase
    {
        private readonly ITrafficApplicationService _trafficApplicationService;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        public TrafficController(ITrafficApplicationService transactionApplicationService)
        {
            _trafficApplicationService = transactionApplicationService;
        }

        // POST api/transfers
        [HttpPost]
        public async Task<IActionResult> PerformTrafficTransfer([FromBody] PerformTrafficTransferRequestDto dto)
        {
            PerformTrafficTransferResponseDto response = await _trafficApplicationService.PerformTransfer(dto);
            return Ok(response);
        }
    }
}
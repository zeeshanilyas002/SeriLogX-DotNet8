using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace SeriLogX.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogTestController : ControllerBase
    {
        private readonly ILogger<LogTestController> _logger;

        public LogTestController(ILogger<LogTestController> logger)
        {
            _logger = logger;
        }

        [HttpGet("test")]
        public IActionResult TestLog()
        {
            _logger.LogInformation("This is an info message.");
            _logger.LogError("This is an error message.");
            _logger.LogWarning("This is a warning.");

            return Ok("Logs written.");
        }
    }
}

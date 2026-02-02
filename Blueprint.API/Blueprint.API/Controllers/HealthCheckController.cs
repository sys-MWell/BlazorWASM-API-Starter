using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace Blueprint.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Basic self-check
            var health = new
            {
                status = "Healthy",
                api = "Blueprint.API",
                time = DateTime.UtcNow,
                portOpen = await IsPortOpen("127.0.0.1", HttpContext.Connection.LocalPort),
            };

            return Ok(health);
        }

        private async Task<bool> IsPortOpen(string host, int port)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                var timeoutTask = Task.Delay(1000);
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                return completedTask == connectTask && client.Connected;
            }
            catch
            {
                return false;
            }
        }
    }
}
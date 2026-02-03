using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;

namespace Blueprint.API.Controllers
{
    /// <summary>
    /// Provides a simple health check endpoint to verify API readiness.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        /// <summary>
        /// Returns the current health status of the API including port availability.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> with health details.</returns>
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

        /// <summary>
        /// Checks whether a TCP port is open on the specified host.
        /// </summary>
        /// <param name="host">The host to check.</param>
        /// <param name="port">The port number to verify.</param>
        /// <returns>True if the port is open; otherwise, false.</returns>
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Web.Models.Models
{
    /// <summary>
    /// Represents the health check status of an API.
    /// </summary>
    public class HealthCheck
    {
        /// <summary>
        /// Gets or sets the status of the health check.
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Gets or sets the name or URL of the API being checked.
        /// </summary>
        public required string Api { get; set; }

        /// <summary>
        /// Gets or sets the time when the health check was performed.
        /// </summary>
        public required DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the port is open.
        /// </summary>
        public required bool PortOpen { get; set; }
    }
}

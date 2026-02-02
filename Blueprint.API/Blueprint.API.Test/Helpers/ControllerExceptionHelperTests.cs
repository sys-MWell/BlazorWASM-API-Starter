using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Blueprint.API.Helpers;

namespace Blueprint.API.Test.Helpers
{
    /// <summary>
    /// Tests for <see cref="Blueprint.API.Helpers.ControllerExceptionHelper"/> request handling.
    /// </summary>
    [TestClass]
    public sealed class ControllerExceptionHelperTests
    {
        private sealed class TestLogger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) => new Noop();
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
            private sealed class Noop : IDisposable { public void Dispose() { } }
        }

        /// <summary>
        /// Returns Ok for non-empty enumerable results.
        /// </summary>
        [TestMethod]
        public async Task HandleRequestAsync_ReturnsOk_ForNonEmpty()
        {
            var res = await ControllerExceptionHelper.HandleRequestAsync(async () => new[] { 1 }, new TestLogger());
            Assert.IsInstanceOfType(res, typeof(OkObjectResult));
        }

        /// <summary>
        /// Returns NotFound for empty enumerable results like int[0].
        /// </summary>
        [TestMethod]
        public async Task HandleRequestAsync_ReturnsNotFound_ForEmptyEnumerable()
        {
            var res = await ControllerExceptionHelper.HandleRequestAsync(async () => Array.Empty<int>(), new TestLogger());
            Assert.IsInstanceOfType(res, typeof(NotFoundObjectResult));
        }

        /// <summary>
        /// Returns NotFound for null results.
        /// </summary>
        [TestMethod]
        public async Task HandleRequestAsync_ReturnsNotFound_ForNull()
        {
            var res = await ControllerExceptionHelper.HandleRequestAsync<object?>(async () => null, new TestLogger());
            Assert.IsInstanceOfType(res, typeof(NotFoundObjectResult));
        }

        /// <summary>
        /// Returns 500 for unhandled exceptions.
        /// </summary>
        [TestMethod]
        public async Task HandleRequestAsync_HandlesGeneralException()
        {
            var res = await ControllerExceptionHelper.HandleRequestAsync<object>(async () => throw new InvalidOperationException(), new TestLogger());
            var obj = res as ObjectResult;
            Assert.IsNotNull(obj);
            Assert.AreEqual(500, obj!.StatusCode);
        }
    }
}

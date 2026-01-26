using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace App.Web.Test.Utils
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = _handler(request);
            return Task.FromResult(response);
        }

        public static HttpClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> handler)
            => new HttpClient(new FakeHttpMessageHandler(handler));

        public static HttpResponseMessage Json(HttpStatusCode status, string json)
            => new HttpResponseMessage(status) { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };

        public static HttpResponseMessage Text(HttpStatusCode status, string text)
            => new HttpResponseMessage(status) { Content = new StringContent(text, System.Text.Encoding.UTF8, "text/plain") };
    }
}

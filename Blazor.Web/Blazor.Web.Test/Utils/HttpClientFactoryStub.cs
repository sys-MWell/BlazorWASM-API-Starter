using System.Net.Http;
using Microsoft.Extensions.Http;

namespace App.Web.Test.Utils
{
    public sealed class HttpClientFactoryStub : IHttpClientFactory
    {
        private readonly HttpClient _client;
        public HttpClientFactoryStub(HttpClient client) => _client = client;
        public HttpClient CreateClient(string name) => _client;
        public static IHttpClientFactory FromHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
            => new HttpClientFactoryStub(new HttpClient(new FakeHttpMessageHandler(handler)));
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kevsoft.WLED.Tests
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, string> _mockedResponses
            = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public void AppendResponse(string uri, string body)
        {
            _mockedResponses.Add(uri, body);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_mockedResponses.TryGetValue(request.RequestUri.AbsoluteUri, out var body))
            {
                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(body, Encoding.UTF8, "application/json")
                    });
            }
            
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}

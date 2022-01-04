using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kevsoft.WLED;

namespace WledJsonTester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            WLedClient client = new WLedClient("http://192.168.178.213/");
            var test = await client.GetState();

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
                if (test != null)
                {
                    Console.WriteLine(test); 
                    Console.WriteLine();
                    //Console.WriteLine(client.CreateJson(test.Result));

                    Console.WriteLine("Sending state");
                    test.On = true;
                    await client.PostJson(test);
                }
            }
        }
    }

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
            if (request.RequestUri != null && _mockedResponses.TryGetValue(request.RequestUri.AbsoluteUri, out var body))
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

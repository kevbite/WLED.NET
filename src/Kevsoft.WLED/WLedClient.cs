using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kevsoft.WLED
{
    public class WLedClient
    {
        private readonly HttpClient _client;

        public WLedClient(HttpMessageHandler httpMessageHandler, string baseUri)
        {
            _client = new HttpClient(httpMessageHandler)
            {
                BaseAddress = new Uri(baseUri, UriKind.Absolute)
            };

            // Add the keep-alive flag to the header
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        }

        public WLedClient(string baseUri) : this(new HttpClientHandler(), baseUri)
        {

        }

        public async Task<WLedRootResponse?> Get()
        {
            var message = await _client.GetAsync("json");

            message.EnsureSuccessStatusCode();

            var response = await JsonSerializer.DeserializeAsync<WLedRootResponse>(await message.Content.ReadAsStreamAsync());

            return response;
        }

        public async Task<State?> GetState()
        {
            var message = await _client.GetAsync("json/state");

            message.EnsureSuccessStatusCode();

            var response = await JsonSerializer.DeserializeAsync<State>(await message.Content.ReadAsStreamAsync());

            return response;
        }

        public async Task<Information?> GetInformation()
        {
            var message = await _client.GetAsync("json/info");

            message.EnsureSuccessStatusCode();

            var response = await JsonSerializer.DeserializeAsync<Information>(await message.Content.ReadAsStreamAsync());

            return response;
        }

        public async Task<string[]?> GetEffects()
        {
            var message = await _client.GetAsync("json/eff");

            message.EnsureSuccessStatusCode();

            var response = await JsonSerializer.DeserializeAsync<string[]>(await message.Content.ReadAsStreamAsync());

            return response;
        }

        public async Task<string[]?> GetPalettes()
        {
            var message = await _client.GetAsync("json/pal");

            message.EnsureSuccessStatusCode();

            var response = await JsonSerializer.DeserializeAsync<string[]>(await message.Content.ReadAsStreamAsync());

            return response;
        }

        public async Task PostJson(object jsonObject)
        {
            // Create a JSON string from the input parameter
            var stateString = JsonSerializer.Serialize(jsonObject);

            // POST the JSON string to the WLED device and ensure that it was successful
            using (var content = new StringContentWithoutCharset(stateString, "application/json"))
            {
                var result = await _client.PostAsync("/json", content);
                result.EnsureSuccessStatusCode();
            }
        }
    }

    public class StringContentWithoutCharset : StringContent
    {
        public StringContentWithoutCharset(string content) : base(content)
        {
        }

        /// <summary>
        /// Remove the charset property from header because WLED can't deal with it
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        public StringContentWithoutCharset(string content, Encoding encoding) : base(content, encoding)
        {
            Headers.ContentType.CharSet = "";
        }

        /// <summary>
        /// Remove the charset property from header because WLED can't deal with it
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <param name="mediaType"></param>
        public StringContentWithoutCharset(string content, Encoding encoding, string mediaType) : base(content, encoding, mediaType)
        {
            Headers.ContentType.CharSet = "";
        }

        /// <summary>
        /// Remove the charset property from header because WLED can't deal with it
        /// </summary>
        /// <param name="content"></param>
        /// <param name="mediaType"></param>
        public StringContentWithoutCharset(string content, string mediaType) : base(content, Encoding.UTF8, mediaType)
        {
            Headers.ContentType.CharSet = "";
        }
    }
}

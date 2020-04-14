using Newtonsoft.Json;
using ProductScraper.Services.Helpers;
using ProductScraper.Services.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ProductScraper.Services.Implementations
{
    public class HttpHandlerService : IHttpHandlerService
    {
        private readonly HttpClient httpClient;
        public HttpHandlerService()
        {
            httpClient = new HttpClient();
        }
        public HttpContent CreateContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                MemoryStream ms = new MemoryStream();
                SerializeHelper.SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }
        public async Task<T> HandleGetRequest<T>(string url)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            using HttpResponseMessage response = await httpClient
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, new CancellationToken())
                .ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            else
            {
                dynamic body = await response.Content.ReadAsStringAsync();
                T result = JsonConvert.DeserializeObject<T>(body as string);
                return result;
            }
        }
        public async Task HandlePostRequest(string url, object content)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            using HttpContent httpContent = CreateContent(content);
            request.Content = httpContent;

            using HttpResponseMessage response = await httpClient
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, new CancellationToken())
                .ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
    }
}

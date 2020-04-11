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
        private HttpClient httpClient;
        private CancellationToken cancellationToken;
        public HttpHandlerService()
        {
            httpClient = new HttpClient();
        }
        public HttpContent CreateContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeHelper.SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }
        public async Task<T> HandleGetRequest<T>(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await httpClient
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception(await response.Content.ReadAsStringAsync());
            else
            {
                dynamic body = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(body as string);
                return result;
            }
        }
        public async Task HandlePostRequest(string url, object content)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            using var httpContent = CreateContent(content);
            request.Content = httpContent;

            using var response = await httpClient
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception(await response.Content.ReadAsStringAsync());
        }
    }
}

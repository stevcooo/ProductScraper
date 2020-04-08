using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ProductScraper.Services.Helpers
{
    public static class HttpContentHelper
    {
        public static HttpContent Create(object content)
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
    }
}

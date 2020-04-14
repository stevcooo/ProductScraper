using System.Net.Http;
using System.Threading.Tasks;

namespace ProductScraper.Services.Interfaces
{
    public interface IHttpHandlerService
    {
        HttpContent CreateContent(object content);
        Task<T> HandleGetRequest<T>(string url);
        Task HandlePostRequest(string url, object content);
    }
}

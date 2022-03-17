using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services
{
    public class BasketService : IBasketService
    {
        public readonly HttpClient _httpClient;

        public BasketService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BasketModel> GetBasket(string userName)
        {
            var response = await _httpClient.GetAsync($"/api/Basket/{userName}");
            return await response.ReadContentAs<BasketModel>();
        }
    }
}

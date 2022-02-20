using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        public readonly IDistributedCache _redishCache;

        public BasketRepository(IDistributedCache redishCache)
        {
            _redishCache = redishCache;
        }

        public async Task<ShoppingCart> GetBasket(string username)
        {
          var basket = await _redishCache.GetStringAsync(username);
            if (string.IsNullOrEmpty(basket))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redishCache.SetStringAsync(basket.UserName,JsonConvert.SerializeObject(basket));
            return await GetBasket(basket.UserName);
        }

        public async Task DeleteBasket(string username)
        {
            await _redishCache.RemoveAsync(username);
        }
    }
}

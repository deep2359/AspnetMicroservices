using Basket.API.Entities;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public interface IBasketRepository
    {
        public Task<ShoppingCart> GetBasket(string username);
        public Task<ShoppingCart> UpdateBasket(ShoppingCart basket);
        public Task DeleteBasket(string username);
    }
}

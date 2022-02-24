using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {

        public readonly IBasketRepository _repo;
        private readonly DiscountGrpcServices _discountGrpcServices;

        public BasketController(IBasketRepository repo, DiscountGrpcServices discountGrpcService)
        {
            _repo = repo;
            _discountGrpcServices = discountGrpcService;
        }


        [HttpGet("{username}",Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
        {
            var busket =  await _repo.GetBasket(username);
            return Ok(busket ?? new ShoppingCart(username));
        }


        
        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket( [FromBody] ShoppingCart basket)
        {
            //TODO: Connect with Discount Grpc.
            //Calculate the latest price of Product in the shopping cart
            //Consume Grpc class
            foreach(var item in basket.Items)
            {
               var coupun = await _discountGrpcServices.GetDiscount(item.ProductName);
                item.Price -= coupun.Amount;
            }
         
            return Ok(await _repo.UpdateBasket(basket));
        }


        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void),(int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            await _repo.DeleteBasket(userName);
           return Ok();
        }

        
        
    }
}

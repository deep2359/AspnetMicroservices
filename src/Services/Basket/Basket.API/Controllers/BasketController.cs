            using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(IBasketRepository repo, DiscountGrpcServices discountGrpcService, 
            IMapper map, IPublishEndpoint publishEndpoint)
        {
            _repo = repo;
            _discountGrpcServices = discountGrpcService;
            _mapper = map;
            _publishEndpoint = publishEndpoint;
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


        //so that we can call it using method name CheckoutBasket
        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<ActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            //Get existing basket with total price
            //Create BasketCheckoutEvent - Set TotalPrice on basketCheckout eventMessage with bascket
            //Send checkout event to rabbitmq
            //Remove the basket (delete data from redis since I will put this data to MSSQL in ordering API)


            //Get existing basket with total price
            var basket = await _repo.GetBasket(basketCheckout.UserName);
            if (basket == null)
                return BadRequest();

            //Create BasketCheckoutEvent
            //for doing this we need to map BasketCheckoutEvent with basket
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);

            //Set TotalPrice on basketCheckout eventMessage 
            //I will show it explicitly
            eventMessage.TotalPrice = basket.TotalPrice;

            //eventMessage is the event or object that I am going to publish to rabbitmq
            //publishing an event to rabbitmq is very easy using masstransit
            //in order to publish a message to rabbitmq an important object is IPublishEndpoint
            //I am going to inject IPublishEndpoint

            //Send checkout event to rabbitmq
            await _publishEndpoint.Publish(eventMessage);

            //Remove the basket
            await _repo.DeleteBasket(basketCheckout.UserName);

            //if successfull return this method
            return Accepted();

        }

    }
}

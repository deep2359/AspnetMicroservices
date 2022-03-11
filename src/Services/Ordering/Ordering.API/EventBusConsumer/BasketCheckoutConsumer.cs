using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using System.Threading.Tasks;

namespace Ordering.API.EventBusConsumer
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
    {
        public readonly IMapper _mapper;
        public readonly IMediator _mediator;
        public readonly ILogger<BasketCheckoutConsumer> _logger;

        public BasketCheckoutConsumer(IMapper mmapper, IMediator mediator,
            ILogger<BasketCheckoutConsumer> logger)
        {
            _mapper = mmapper;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            var command = _mapper.Map<CheckoutOrderCommand>(context.Message);
            var result = await _mediator.Send(command);
            _logger.LogInformation("BasketcheckoutEvent Consumed successfully. Created Order Id :" +
                " {newOrderId}", result);
            }
    }
}
    
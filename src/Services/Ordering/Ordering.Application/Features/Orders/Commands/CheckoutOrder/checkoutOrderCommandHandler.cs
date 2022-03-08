using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    internal class checkoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        public readonly IOrderRepository _repo;
        public readonly IMapper _mapper;
        public readonly IEmailService _emailService;
        public readonly ILogger<checkoutOrderCommandHandler> _logger;

        public checkoutOrderCommandHandler(IOrderRepository repo, IMapper mapper
            , IEmailService emailService, ILogger<checkoutOrderCommandHandler> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(request);
            var newOrder = await _repo.AddAsync(orderEntity);
            _logger.LogInformation($"Order {newOrder.Id} is successfully created");

            await SendMail(newOrder);
            return newOrder.Id;
        }

        private async Task SendMail(Order newOrder)
        {
            var email = new Email
            {
                To = "deep2359@gmail.com",
                Body = "Order was created",
                Subject = "Order was created"
            };

            try
            {
              await  _emailService.SendEmail(email);

            }
            catch (Exception ex)
            {

                _logger.LogError($"Order {newOrder.Id} failed due to an error with the mail service" +
                    $" {ex.Message}");
            }

        }
    }
}

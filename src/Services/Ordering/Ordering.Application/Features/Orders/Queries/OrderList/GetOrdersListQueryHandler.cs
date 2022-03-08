using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Queries.OrderList
{
    public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrdersVm>>
    {
        public readonly IOrderRepository _repo;
        public readonly IMapper _mapper;

        public GetOrdersListQueryHandler(IOrderRepository repo, IMapper mapper )
        {
            _repo=repo;
            _mapper=mapper;
        }

        public async Task<List<OrdersVm>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
        {
          var orderList=  await _repo.GetOrdersByUserName(request._UserName);
           return _mapper.Map<List<OrdersVm>>(orderList);
        }
    }
}

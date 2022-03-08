using MediatR;
using System.Collections.Generic;

namespace Ordering.Application.Features.Orders.Queries.OrderList
{
    public class GetOrdersListQuery : IRequest<List<OrdersVm>>
    {
        public string _UserName;
        public GetOrdersListQuery(string userName)
        {
            _UserName = userName;

        }
    }
}

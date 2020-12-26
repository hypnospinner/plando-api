using System.Collections.Generic;
using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Plando.Common;
using Plando.Models.Orders;

using Plando.Models;

namespace Plando.Queries.Orders
{
    public class GetAllOrders : PaginatedQuery, IQuery<IEnumerable<Order>> { }

    public class GetAllOrdersHandler : HandlerWithApplicationContext, IQueryHandler<GetAllOrders, IEnumerable<Order>>
    {
        public GetAllOrdersHandler(ApplicationContext context) : base(context) { }
        public async Task<IEnumerable<Order>> HandleAsync(GetAllOrders query)
        {
            return await _context.GetOrdersAsync(
                query.Page,
                query.PerPage);
        }
    }
}
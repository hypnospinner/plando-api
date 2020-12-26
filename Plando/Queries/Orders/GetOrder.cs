using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.Models;
using Plando.Models.Orders;
using Plando.Models.Users;
using static Plando.Common.TypedException;

namespace Plando.Queries.Orders
{
    public class GetOrder : IQuery<Order>
    {
        public GetOrder(int orderId)
            => OrderId = orderId;

        public int OrderId { get; set; }
        public int? UserId { get; set; } = null;
    }

    public class GetOrderHandler : HandlerWithApplicationContext, IQueryHandler<GetOrder, Order>
    {
        public GetOrderHandler(ApplicationContext context) : base(context) { }

        public async Task<Order> HandleAsync(GetOrder query)
        {
            if (query.UserId is null)
                throw BusinessLogicException("Cannot get order without user info");

            var user = await _context.Users
                .Include(x => x.Identity)
                .Include(x => x.Laundry)
                    .ThenInclude(x => x.Orders)
                .Include(x => x.Orders)
                .SingleOrDefaultAsync(x => x.Id == query.UserId);

            switch (user.Identity.Role)
            {
                case UserRole.Client:
                    var hasOrder = user.Orders.Any(x => x.Id == query.OrderId);
                    if (!hasOrder)
                        throw BusinessLogicException($"Cannot return order {query.OrderId} to user {user.Id}");

                    return await _context.GetOrderAsync(query.OrderId);
                case UserRole.Manager:
                    if (!user.Laundry.Orders.Any(x => x.Id == query.OrderId))
                        throw BusinessLogicException($"Cannot return order {query.OrderId} to manager {user.Id}");

                    return await _context.GetOrderAsync(query.OrderId);

                default: return await _context.GetOrderAsync(query.OrderId);
            }
        }
    }
}
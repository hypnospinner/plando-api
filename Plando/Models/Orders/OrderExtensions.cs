using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Plando.Common;

namespace Plando.Models.Orders
{
    public static class OrdersExtensions
    {
        public static async Task<Order> GetOrderAsync(this ApplicationContext context, int id)
        {
            var orderCreatedEvent = await context.OrderCreatedEvents
                .Include(x => x.OrderPutInProgressEvent)
                .Include(x => x.OrderCancelledEvent)
                .Include(x => x.OrderPassedEvent)
                .Include(x => x.OrderPutInProgressEvent)
                .Include(x => x.ServiceAddedEvents)
                    .ThenInclude(x => x.ServiceCompletedEvent)
                .Include(x => x.ServiceAddedEvents)
                    .ThenInclude(x => x.ServiceRemovedEvent)
                .Include(x => x.ServiceAddedEvents)
                    .ThenInclude(x => x.Service)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (orderCreatedEvent is null)
                return null;

            var aggregates = new List<IAggregator<Order>>(new IAggregator<Order>[] {
                orderCreatedEvent,
                orderCreatedEvent.OrderCancelledEvent,
                orderCreatedEvent.OrderFinishedEvent,
                orderCreatedEvent.OrderPassedEvent,
                orderCreatedEvent.OrderPutInProgressEvent})
                .Concat(orderCreatedEvent.ServiceAddedEvents.Aggregate(
                    new List<IAggregator<Order>>(), (list, next) =>
                    {
                        list.AddRange(new IAggregator<Order>[] {
                            next,
                            next.ServiceCompletedEvent,
                            next.ServiceRemovedEvent,});

                        return list;
                    }));

            return aggregates
                .Where(x => x is not null)
                .Aggregate();
        }

        public static async Task<IEnumerable<Order>> GetOrdersAsync(this ApplicationContext context, int page = 0, int perPage = 20)
        {
            var orderCreatedEvents = await context.OrderCreatedEvents
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .Skip(page * perPage)
                .Take(perPage)
                .ToListAsync();

            var tasks = orderCreatedEvents
                .Select(x => context.GetOrderAsync(x.Id))
                .ToArray();

            Task.WaitAll(tasks);

            return tasks.Select(x => x.Result);
        }
    }
}
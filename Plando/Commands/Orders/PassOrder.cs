using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using static Plando.Common.TypedException;
using Plando.Models;
using Plando.Models.Orders;

namespace Plando.Commands.Orders
{
    public class PassOrder : ICommand
    {
        public PassOrder(int orderId)
            => OrderId = orderId;

        public int? ClientId { get; set; } = null;
        public int OrderId { get; private set; }
    }

    public class PassOrderHandler : HandlerWithApplicationContext, ICommandHandler<PassOrder>
    {
        public PassOrderHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(PassOrder command)
        {
            if (command.ClientId is null)
                throw BusinessLogicException($"Cannot pass order without client info");

            var orderCreatedEvent = await _context.OrderCreatedEvents
                .Include(x => x.OrderFinishedEvent)
                .Include(x => x.OrderCancelledEvent)
                .Include(x => x.OrderPassedEvent)
                .Include(x => x.OrderPutInProgressEvent)
                .SingleOrDefaultAsync(x => x.Id == command.OrderId);

            if (orderCreatedEvent.ClientId != command.ClientId)
                throw BusinessLogicException($"Client {command.ClientId} cannot pass order {command.OrderId}");

            if (orderCreatedEvent.OrderPutInProgressEvent is null)
                throw BusinessLogicException($"Cannot pass order {command.OrderId} as it's not put in progress");

            if (orderCreatedEvent.OrderFinishedEvent is null)
                throw BusinessLogicException($"Cannot pass order {command.OrderId} as it's not finished");

            if (orderCreatedEvent.OrderPassedEvent is not null)
                throw BusinessLogicException($"Cannot pass order {command.OrderId} as it has been already passed");

            if (orderCreatedEvent.OrderCancelledEvent is not null)
                throw BusinessLogicException($"Cannot pass order {command.OrderId} as it has been cancelled");

            _context.OrderPassedEvents.Add(new OrderPassedEvent { OrderId = command.OrderId });
            await _context.SaveChangesAsync();
        }
    }
}
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using static Plando.Common.TypedException;
using Plando.Models;
using Plando.Models.Orders;
using Plando.Models.Users;

namespace Plando.Commands.Orders
{
    public class CancelOrder : ICommand
    {
        public CancelOrder(int orderId)
            => OrderId = orderId;

        public int? ClientId { get; set; } = null;
        public int OrderId { get; private set; }
    }

    public class CancelOrderHandler : HandlerWithApplicationContext, ICommandHandler<CancelOrder>
    {
        public CancelOrderHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(CancelOrder command)
        {
            if (command.ClientId is null)
                throw BusinessLogicException($"Cannot cancel order {command.OrderId} without client info");

            var orderCreatedEvent = await _context.OrderCreatedEvents
                .Include(x => x.OrderFinishedEvent)
                .Include(x => x.OrderCancelledEvent)
                .Include(x => x.OrderPassedEvent)
                .Include(x => x.OrderPutInProgressEvent)
                .SingleOrDefaultAsync(x => x.Id == command.OrderId);

            if (orderCreatedEvent.ClientId != command.ClientId &&
                await _context.Identities.AnyAsync(x => x.Role == UserRole.Administrator && x.UserId == command.ClientId))
                throw BusinessLogicException($"Client {command.ClientId} cannot cancel order {command.OrderId}");

            if (orderCreatedEvent.ClientId != command.ClientId)
                throw BusinessLogicException($"Client {command.ClientId} cannot cancel order {command.OrderId}");

            if (orderCreatedEvent.OrderPutInProgressEvent is not null)
                throw BusinessLogicException($"Cannot cancel order {command.OrderId} as it has been already put in progress");

            if (orderCreatedEvent.OrderFinishedEvent is not null)
                throw BusinessLogicException($"Cannot cancel order {command.OrderId} as it has been already finished");

            if (orderCreatedEvent.OrderPassedEvent is not null)
                throw BusinessLogicException($"Cannot cancel order {command.OrderId} as it has been already passed");

            if (orderCreatedEvent.OrderCancelledEvent is not null)
                throw BusinessLogicException($"Cannot cancel order {command.OrderId} as it has been already cancelled");

            _context.OrderCancelledEvents.Add(new OrderCancelledEvent { OrderId = command.OrderId });
            await _context.SaveChangesAsync();
        }
    }
}
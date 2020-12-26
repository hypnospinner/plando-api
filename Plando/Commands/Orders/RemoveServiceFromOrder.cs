using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.Models;
using Plando.Models.Orders;
using Plando.Models.Users;
using static Plando.Common.TypedException;

namespace Plando.Commands.Orders
{
    public class RemoveServiceFromOrder : ICommand
    {
        public RemoveServiceFromOrder(int serviceId, int orderId)
        {
            ServiceId = serviceId;
            OrderId = orderId;
        }

        public int? ClientId { get; set; } = null;
        public int ServiceId { get; private set; }
        public int OrderId { get; private set; }
    }

    public class RemoveServiceFromOrderHandler : HandlerWithApplicationContext, ICommandHandler<RemoveServiceFromOrder>
    {
        public RemoveServiceFromOrderHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(RemoveServiceFromOrder command)
        {
            if (command.ClientId is null)
                throw BusinessLogicException($"Cannot remove service from order {command.OrderId} without client info");

            var orderCreatedEvent = await _context.OrderCreatedEvents
                .Include(x => x.OrderFinishedEvent)
                .Include(x => x.OrderCancelledEvent)
                .Include(x => x.OrderPassedEvent)
                .Include(x => x.OrderPutInProgressEvent)
                .Include(x => x.ServiceAddedEvents)
                    .ThenInclude(x => x.ServiceRemovedEvent)
                .Include(x => x.ServiceAddedEvents)
                    .ThenInclude(x => x.ServiceCompletedEvent)
                .SingleOrDefaultAsync(x => x.Id == command.OrderId);

            if (orderCreatedEvent.ClientId != command.ClientId)
                throw BusinessLogicException($"Client {command.ClientId} cannot remove service from order {command.OrderId}");

            if (orderCreatedEvent.OrderCancelledEvent is not null)
                throw BusinessLogicException($"Cannot remove service from cancelled order {command.OrderId}");

            if (orderCreatedEvent.OrderPassedEvent is not null)
                throw BusinessLogicException($"Cannot remove service from order {command.OrderId} as it has been already passed");

            if (orderCreatedEvent.OrderFinishedEvent is not null)
                throw BusinessLogicException($"Cannot remove service from order {command.OrderId} as it has been already finished");

            if (orderCreatedEvent.OrderPutInProgressEvent is not null)
                throw BusinessLogicException($"Cannot remove service from order {command.OrderId} as it has been put in progress");

            var serviceAddedEvent = orderCreatedEvent.ServiceAddedEvents
                .SingleOrDefault(x => x.ServiceId == command.ServiceId && x.ServiceRemovedEvent is null);

            if (serviceAddedEvent.ServiceRemovedEvent is not null)
                throw BusinessLogicException($"Cannot remove service {command.ServiceId} as it has been already removed");

            if (serviceAddedEvent.ServiceCompletedEvent is not null)
                throw BusinessLogicException($"Cannot remove service {command.ServiceId} as it has been already completed");

            var serviceRemovedEvent = new ServiceRemovedEvent
            {
                ServiceAddedEventId = serviceAddedEvent.Id,
                OrderId = command.OrderId,
                ServiceId = command.ServiceId
            };

            _context.ServiceRemovedEvents.Add(serviceRemovedEvent);
            await _context.SaveChangesAsync();
        }
    }
}
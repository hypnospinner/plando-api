using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.Models;
using Plando.Models.Orders;
using static Plando.Common.TypedException;

namespace Plando.Commands.Orders
{
    public class CompleteServiceInOrder : ICommand
    {
        public CompleteServiceInOrder(int serviceId, int orderId)
        {
            ServiceId = serviceId;
            OrderId = orderId;
        }

        public int? ManagerId { get; set; } = null;
        public int ServiceId { get; private set; }
        public int OrderId { get; private set; }
    }

    public class CompleteServiceInOrderHandler : HandlerWithApplicationContext, ICommandHandler<CompleteServiceInOrder>
    {
        public CompleteServiceInOrderHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(CompleteServiceInOrder command)
        {
            if (command.ManagerId is null)
                throw BusinessLogicException($"Cannot complete service {command.ServiceId} for order {command.OrderId} without manager info");

            var orderCreatedEvent = await _context.OrderCreatedEvents
                .Include(x => x.OrderFinishedEvent)
                .Include(x => x.OrderCancelledEvent)
                .Include(x => x.OrderPassedEvent)
                .Include(x => x.OrderPutInProgressEvent)
                .Include(x => x.ServiceAddedEvents)
                    .ThenInclude(x => x.ServiceRemovedEvent)
                .Include(x => x.ServiceAddedEvents)
                    .ThenInclude(x => x.ServiceCompletedEvent)
                .Include(x => x.Laundry)
                    .ThenInclude(x => x.Managers)
                .SingleOrDefaultAsync(x => x.Id == command.OrderId);

            if (!orderCreatedEvent.Laundry.Managers.Any(x => x.Id == command.ManagerId))
                throw BusinessLogicException($"Manager {command.ManagerId} cannot complete service {command.ServiceId} for order {command.OrderId}");

            if (orderCreatedEvent.OrderCancelledEvent is not null)
                throw BusinessLogicException($"Cannot complete service in cancelled order {command.OrderId}");

            if (orderCreatedEvent.OrderPassedEvent is not null)
                throw BusinessLogicException($"Cannot complete service in order {command.OrderId} as it has been already passed");

            if (orderCreatedEvent.OrderFinishedEvent is not null)
                throw BusinessLogicException($"Cannot complete service in order {command.OrderId} as it has been already finished");

            if (orderCreatedEvent.OrderPutInProgressEvent is not null)
                throw BusinessLogicException($"Cannot complete service in order {command.OrderId} as it has been put in progress");

            var serviceAddedEvent = orderCreatedEvent.ServiceAddedEvents
                .SingleOrDefault(x => x.ServiceId == command.ServiceId);

            if (serviceAddedEvent.ServiceRemovedEvent is not null)
                throw BusinessLogicException($"Cannot complete service {command.ServiceId} as it has been already removed");

            if (serviceAddedEvent.ServiceCompletedEvent is not null)
                throw BusinessLogicException($"Cannot complete service {command.ServiceId} as it has been already completed");

            var serviceCompletedEvent = new ServiceCompletedEvent { 
                ServiceAddedEventId = serviceAddedEvent.Id,
                ServiceId = command.ServiceId,
                OrderId = command.OrderId  };

            _context.ServiceCompletedEvents.Add(serviceCompletedEvent);
            await _context.SaveChangesAsync();
        }
    }
}
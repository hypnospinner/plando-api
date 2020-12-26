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
    public class FinishOrder : ICommand
    {
        public FinishOrder(int orderId)
            => OrderId = orderId;

        public int? ManagerId { get; set; } = null;
        public int OrderId { get; private set; }
    }

    public class FinishOrderHandler : HandlerWithApplicationContext, ICommandHandler<FinishOrder>
    {
        public FinishOrderHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(FinishOrder command)
        {
            if (command.ManagerId is null)
                throw BusinessLogicException($"Cannot finish order in progress without manager info");

            var orderCreatedEvent = await _context.OrderCreatedEvents
                .Include(x => x.OrderFinishedEvent)
                .Include(x => x.OrderCancelledEvent)
                .Include(x => x.OrderPassedEvent)
                .Include(x => x.Laundry)
                    .ThenInclude(x => x.Managers)
                .Include(x => x.ServiceAddedEvents)
                    .ThenInclude(x => x.ServiceCompletedEvent)
                .SingleOrDefaultAsync(x => x.Id == command.OrderId);

            if (!orderCreatedEvent.Laundry.Managers.Any(x => x.Id == command.ManagerId))
                throw BusinessLogicException($"Manager {command.ManagerId} cannot finish order {command.OrderId}");

            if (orderCreatedEvent.OrderPutInProgressEvent is null)
                throw BusinessLogicException($"Cannot finish order {command.OrderId} as it hasn't been put in progress");

            if (orderCreatedEvent.OrderFinishedEvent is not null)
                throw BusinessLogicException($"Cannot finish order {command.OrderId} as it has been already finished");

            if (orderCreatedEvent.OrderPassedEvent is not null)
                throw BusinessLogicException($"Cannot pass order {command.OrderId} as it has been already passed");

            if (orderCreatedEvent.OrderCancelledEvent is not null)
                throw BusinessLogicException($"Cannot pass order {command.OrderId} as it has been cancelled");

            foreach (var @event in orderCreatedEvent.ServiceAddedEvents)
                if (@event.ServiceCompletedEvent is null)
                    throw BusinessLogicException($"Cannot finish order {command.OrderId} as service {@event.ServiceId} is not completed");

            _context.OrderFinishedEvents.Add(new OrderFinishedEvent { OrderId = command.OrderId });
            await _context.SaveChangesAsync();
        }
    }
}
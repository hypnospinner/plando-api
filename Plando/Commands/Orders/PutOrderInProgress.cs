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
    public class PutOrderInProgress : ICommand
    {
        public PutOrderInProgress(int orderId)
            => OrderId = orderId;

        public int? ManagerId { get; set; } = null;
        public int OrderId { get; private set; }
    }

    public class PutOrderInProgressHandler : HandlerWithApplicationContext, ICommandHandler<PutOrderInProgress>
    {
        public PutOrderInProgressHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(PutOrderInProgress command)
        {
            if (command.ManagerId is null)
                throw BusinessLogicException($"Cannot put order in progress without manager info");

            var orderCreatedEvent = await _context.OrderCreatedEvents
                .Include(x => x.OrderCancelledEvent)
                .Include(x => x.OrderPutInProgressEvent)
                .Include(x => x.OrderFinishedEvent)
                .Include(x => x.OrderPassedEvent)
                .Include(x => x.Laundry)
                    .ThenInclude(x => x.Managers)
                .SingleOrDefaultAsync(x => x.Id == command.OrderId);

            if (!orderCreatedEvent.Laundry.Managers.Any(x => x.Id == command.ManagerId))
                throw BusinessLogicException($"Manager {command.ManagerId} cannot put order {command.OrderId} in progress");

            if (!orderCreatedEvent.Laundry.Managers.Any(x => x.Id == command.ManagerId))
                throw BusinessLogicException($"Manager {command.ManagerId} cannot put order {command.OrderId} in progress");

            if (orderCreatedEvent.OrderPassedEvent is not null)
                throw BusinessLogicException($"Cannot put order {command.OrderId} in progress as it has been already passed");

            if (orderCreatedEvent.OrderFinishedEvent is not null)
                throw BusinessLogicException($"Cannot put order {command.OrderId} in progress as it has been already finished");

            if (orderCreatedEvent.OrderCancelledEvent is not null)
                throw BusinessLogicException($"Cannot put cancelled order {command.OrderId} in progress");

            if (orderCreatedEvent.OrderPutInProgressEvent is not null)
                throw BusinessLogicException($"Cannot put order {command.OrderId} in progress as it has been already put in progress");

            _context.OrderPutInProgressEvents.Add(new OrderPutInProgressEvent { OrderId = command.OrderId });
            await _context.SaveChangesAsync();
        }
    }
}
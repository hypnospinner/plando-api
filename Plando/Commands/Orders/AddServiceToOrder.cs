using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using static Plando.Common.TypedException;
using Plando.Models;
using Plando.Models.Orders;
using System.Linq;
using Plando.Models.Users;

namespace Plando.Commands.Orders
{
    public class AddServiceToOrder : ICommand
    {
        public AddServiceToOrder(int serviceId, int orderId)
        {
            ServiceId = serviceId;
            OrderId = orderId;
        }

        public int? ClientId { get; set; } = null;
        public int ServiceId { get; private set; }
        public int OrderId { get; private set; }
    }

    public class AddServiceToOrderHandler : HandlerWithApplicationContext, ICommandHandler<AddServiceToOrder>
    {
        public AddServiceToOrderHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(AddServiceToOrder command)
        {
            if (command.ClientId is null)
                throw BusinessLogicException($"Cannot add service to order {command.OrderId} without client info");

            var orderCreatedEvent = await _context.OrderCreatedEvents
                .Include(x => x.OrderCancelledEvent)
                .Include(x => x.OrderFinishedEvent)
                .Include(x => x.OrderPassedEvent)
                .Include(x => x.ServiceAddedEvents)
                    .ThenInclude(x => x.ServiceRemovedEvent)
                .Include(x => x.Laundry)
                    .ThenInclude(x => x.Services)
                .SingleOrDefaultAsync(x => x.Id == command.OrderId);

            if (orderCreatedEvent is null)
                throw BusinessLogicException($"Order {command.OrderId} does not exist");

            if (orderCreatedEvent.ClientId != command.ClientId &&
                await _context.Identities.AnyAsync(x => x.Role == UserRole.Administrator && x.UserId == command.ClientId))
                throw BusinessLogicException($"Client {command.ClientId} cannot add service to order {command.OrderId}");

            if (!orderCreatedEvent.Laundry.Services
                .Any(x => x.ServiceId == command.ServiceId))
                throw BusinessLogicException($"Cannot add service {command.ServiceId} to order {command.OrderId} as this service is unavailable in laundry {orderCreatedEvent.LaundryId}");

            if (orderCreatedEvent is null)
                throw BusinessLogicException($"Cannot add service to order {command.OrderId} as this order does not exist");

            if (orderCreatedEvent.OrderCancelledEvent is not null)
                throw BusinessLogicException($"Cannot add service to cancelled order {command.OrderId}");

            if (orderCreatedEvent.OrderPassedEvent is not null)
                throw BusinessLogicException($"Cannot add service to passed order {command.OrderId}");

            if (orderCreatedEvent.OrderFinishedEvent is not null)
                throw BusinessLogicException($"Cannot add service to passed order {command.OrderId}");

            if (orderCreatedEvent.OrderPutInProgressEvent is not null)
                throw BusinessLogicException($"Cannot add service to order {command.OrderId} as it has been already pu in progress");

            _context.ServiceAddedEvents.Add(new ServiceAddedEvent
            {
                ServiceId = command.ServiceId,
                OrderId = command.OrderId
            });
            await _context.SaveChangesAsync();
        }
    }
}
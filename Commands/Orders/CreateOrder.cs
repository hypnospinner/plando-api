using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Plando.Models;
using Plando.Common;
using Plando.Models.Orders;

namespace Plando.Commands.Orders
{
    public class CreateOrder : ICommand
    {
        public CreateOrder(int clientId, int laundryId, string title)
        {
            ClientId = clientId;
            LaundryId = laundryId;
            Title = title;
        }

        public int ClientId { get; private set; }
        public int LaundryId { get; private set; }
        public string Title { get; private set; }
    }

    public class CreateOrderHandler : HandlerWithApplicationContext, ICommandHandler<CreateOrder>
    {
        public CreateOrderHandler(ApplicationContext context) : base(context) { }
        public async Task HandleAsync(CreateOrder command)
        {
            _context.OrderCreatedEvents.Add(new OrderCreatedEvent
            {
                ClientId = command.ClientId,
                LaundryId = command.LaundryId,
                Title = command.Title
            });
            await _context.SaveChangesAsync();
        }
    }
}
using System.Linq;
using Plando.Common;

namespace Plando.Models.Orders
{
    public class ServiceRemovedEvent : EventBase, IAggregator<Order>
    {
        public int ServiceId { get; set; }
        public int OrderId { get; set; }
        public OrderCreatedEvent OrderCreatedEvent { get; set; }
        public int ServiceAddedEventId { get; set; }
        public ServiceAddedEvent ServiceAddedEvent { get; set; }

        public Order Push(Order aggregate)
        {
            if (aggregate is null)
                return null;

            aggregate.Services = aggregate.Services
                .Where(x => x.Id != ServiceId)
                .ToList();

            aggregate.Price = aggregate.Services.Sum(x => x.Price);

            return aggregate;
        }
    }
}
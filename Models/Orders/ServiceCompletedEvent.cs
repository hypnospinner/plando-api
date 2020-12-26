using System.Linq;
using Plando.Common;

namespace Plando.Models.Orders
{
    public class ServiceCompletedEvent : EventBase, IAggregator<Order>
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

            var service = aggregate.Services
                .FirstOrDefault(x => x.Id == ServiceId);

            service.Done = true;

            return aggregate;
        }
    }
}
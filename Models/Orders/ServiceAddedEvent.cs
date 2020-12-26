using System;
using Plando.Common;
using Plando.Models.Services;

namespace Plando.Models.Orders
{
    public class ServiceAddedEvent : EventBase, IAggregator<Order>
    {
        public int ServiceId { get; set; }
        public int OrderId { get; set; }
        public OrderCreatedEvent OrderCreatedEvent { get; set; }
        public ServiceRemovedEvent ServiceRemovedEvent { get; set; }
        public ServiceCompletedEvent ServiceCompletedEvent { get; set; }
        public Service Service { get; set; }
        public Order Push(Order aggregate)
        {
            if (aggregate is null)
                return null;

            aggregate.Services.Add(new ServiceInOrder
            {
                Id = ServiceId,
                Done = false,
                Name = Service.Title,
                Price = Service.Price
            });

            aggregate.Price += Service.Price;

            return aggregate;
        }
    }
}
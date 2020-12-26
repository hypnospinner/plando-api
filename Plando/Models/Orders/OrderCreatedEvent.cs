using System.Collections.Generic;
using Plando.Common;
using Plando.Models.Laundries;
using Plando.Models.Users;

namespace Plando.Models.Orders
{
    public class OrderCreatedEvent : EventBase, IAggregator<Order>
    {
        public int ClientId { get; set; }
        public int LaundryId { get; set; }
        public string Title { get; set; }
        public OrderPassedEvent OrderPassedEvent { get; set; }
        public OrderFinishedEvent OrderFinishedEvent { get; set; }
        public OrderCancelledEvent OrderCancelledEvent { get; set; }
        public OrderPutInProgressEvent OrderPutInProgressEvent { get; set; }
        public ICollection<ServiceAddedEvent> ServiceAddedEvents { get; set; }
        public User Client { get; set; }
        public Laundry Laundry { get; set; }
        public Order Push(Order aggregate)
        {
            aggregate.Id = Id;
            aggregate.ClientId = ClientId;
            aggregate.LaundryId = LaundryId;
            aggregate.Title = Title;
            aggregate.Status = OrderStatus.NEW;
            aggregate.Price = 0.0m;
            aggregate.Services = new List<ServiceInOrder>();

            return aggregate;
        }
    }
}
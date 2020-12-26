using System.Collections.Generic;

namespace Plando.Models.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public int LaundryId { get; set; }
        public int ClientId { get; set; }
        public string Title { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Price { get; set; }
        public IList<ServiceInOrder> Services { get; set; }
    }

    public class ServiceInOrder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Done { get; set; }
        public decimal Price { get; set; }
    }
}
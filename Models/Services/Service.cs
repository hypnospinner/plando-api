using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Plando.Models.Orders;

namespace Plando.Models.Services
{
    public class Service
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public ICollection<LaundryService> Laundries { get; set; }
        public ICollection<ServiceAddedEvent> ServiceAddedEvents { get; set; }
    }
}
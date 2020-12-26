using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Plando.Models.Orders;
using Plando.Models.Services;
using Plando.Models.Users;

namespace Plando.Models.Laundries
{
    public class Laundry
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; }
        public ICollection<OrderCreatedEvent> Orders { get; set; }
        public ICollection<LaundryService> Services { get; set; }
        public ICollection<User> Managers { get; set; }
    }
}
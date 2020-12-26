using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Plando.Models.Laundries;
using Plando.Models.Orders;

namespace Plando.Models.Users
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Identity Identity { get; set; }
        public Laundry Laundry { get; set; }
        public ICollection<OrderCreatedEvent> Orders { get; set; }
        public int? LaundryId { get; set; } = null;
    }
}
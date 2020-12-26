using Plando.Models.Laundries;

namespace Plando.Models.Services
{
    public class LaundryService
    {
        public int ServiceId { get; set; }
        public int LaundryId { get; set; }
        public Service Service { get; set; }
        public Laundry Laundry { get; set; }
    }
}
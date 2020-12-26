using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.Models;
using Plando.Models.Services;

namespace Plando.Commands.Services
{
    public class MakeServiceAvailableInLaundry : LaundryService, ICommand
    {
        public MakeServiceAvailableInLaundry(int laundryId, int serviceId)
        {
            LaundryId = laundryId;
            ServiceId = serviceId;
        }
    }

    public class MakeServiceAvailableInLaundryHandler : HandlerWithApplicationContext, ICommandHandler<MakeServiceAvailableInLaundry>
    {
        public MakeServiceAvailableInLaundryHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(MakeServiceAvailableInLaundry command)
        {
            if (!await _context.LaundryServices
                .AnyAsync(x => x.LaundryId == command.LaundryId && x.ServiceId == command.ServiceId))
            {
                _context.LaundryServices.Add(command as LaundryService);
                await _context.SaveChangesAsync();
            }
        }
    }
}
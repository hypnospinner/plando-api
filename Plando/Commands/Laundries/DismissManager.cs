using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.Models;
using Plando.Models.Users;
using static Plando.Common.TypedException;

namespace Plando.Commands.Laundries
{
    public class DismissManager : ICommand
    {
        public DismissManager(int managerId)
        {
            ManagerId = managerId;
        }

        public int ManagerId { get; set; }
    }

    public class DismissManagerHandler : HandlerWithApplicationContext, ICommandHandler<DismissManager>
    {
        public DismissManagerHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(DismissManager command)
        {
            var user = await _context.Users
                .Include(x => x.Identity)
                .SingleOrDefaultAsync(x => x.Id == command.ManagerId);

            if (user.Identity.Role != UserRole.Manager)
                throw BusinessLogicException($"Cannot dismiss user {command.ManagerId} as one is not manager");

            if (user.LaundryId is null)
                throw BusinessLogicException($"Cannot dismiss manager {command.ManagerId} as one is not employed");

            user.LaundryId = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.Models;
using Plando.Models.Users;
using static Plando.Common.TypedException;

namespace Plando.Commands.Laundries
{
    public class EmployManager : ICommand
    {
        public EmployManager(int managerId, int laundryId)
        {
            ManagerId = managerId;
            LaundryId = laundryId;
        }

        public int ManagerId { get; set; }
        public int LaundryId { get; set; }
    }

    public class EmployManagerHandler : HandlerWithApplicationContext, ICommandHandler<EmployManager>
    {
        public EmployManagerHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(EmployManager command)
        {
            var laundry = await _context.Laundries
                .Include(x => x.Managers)
                    .ThenInclude(x => x.Laundry)
                .SingleOrDefaultAsync(x => x.Id == command.LaundryId);

            if (laundry is null)
                throw BusinessLogicException($"Cannot employ {command.ManagerId} to laundry which does not exist");

            if (laundry.Managers
                .Select(x => x.Id)
                .Contains(command.ManagerId))
                throw BusinessLogicException($"Cannot add manager {command.ManagerId} to laundry {command.LaundryId} as one is already employed in it");

            var user = await _context.Users
                .Include(x => x.Identity)
                .SingleOrDefaultAsync(x => x.Id == command.ManagerId);

            if (user.Identity.Role != UserRole.Manager)
                throw BusinessLogicException($"Cannot employ non-manager user {command.ManagerId}");

            user.LaundryId = command.LaundryId;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
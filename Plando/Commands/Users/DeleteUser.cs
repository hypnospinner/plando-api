using System;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.Models;
using Plando.Models.Users;
using static Plando.Common.TypedException;

namespace Plando.Commands.Users
{
    public class DeleteUser : ICommand
    {
        public int Id { get; set; }
    }

    public class DeleteUserHandler : HandlerWithApplicationContext, ICommandHandler<DeleteUser>
    {
        public DeleteUserHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(DeleteUser command)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == command.Id);

            if (user is null)
                throw BusinessLogicException($"Cannot delete user {command.Id} as one does not exist");

            var identity = await _context
                .Identities.FirstOrDefaultAsync(x => x.Email == user.Email);

            // we can't delete administrator using API
            if (identity.Role is UserRole.Administrator)
                throw new Exception("Cannot delete administrator");

            _context.Users.Remove(user);
            _context.Identities.Remove(identity);

            await _context.SaveChangesAsync();
        }
    }
}
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.Models;
using Plando.Models.Users;
using static Plando.Common.TypedException;


namespace Plando.Commands.Auth
{
    public class RegisterManager : RegisterUser
    {
        public RegisterManager(
            string firstName,
            string lastName,
            string email,
            string password,
            int? laundryId = null) : base(firstName, lastName, email, password)
            => LaundryId = laundryId;

        public int? LaundryId { get; set; }
    }

    public class RegisterManagerHandler : HandlerWithApplicationContext, ICommandHandler<RegisterManager>
    {
        public RegisterManagerHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(RegisterManager command)
        {
            var user = new User
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                Identity = new Identity
                {
                    Email = command.Email,
                    Password = command.Password,
                    Role = UserRole.Manager,
                }
            };

            if (command.LaundryId is not null)
            {
                if (await _context.Laundries.AnyAsync(x => x.Id == command.LaundryId))
                {
                    user.LaundryId = command.LaundryId;
                }
                else
                {
                    var entity = _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    throw BusinessLogicException(
                        $"Cannot auto-employ new manager {entity.Entity.Id} to laundry {command.LaundryId} as it does not exist; Manager saved in db");
                }

            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();


        }
    }
}
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Plando.Common;
using Plando.Models;
using Plando.Models.Users;

namespace Plando.Commands.Auth
{
    public class RegisterUser : ICommand
    {
        public RegisterUser(string firstName, string lastName, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterUserHandler : HandlerWithApplicationContext, ICommandHandler<RegisterUser>
    {
        public RegisterUserHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(RegisterUser command)
        {
            _context.Users.Add(new User()
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                Identity = new Identity()
                {
                    Email = command.Email,
                    Password = command.Password,
                    Role = UserRole.Client
                }
            });

            await _context.SaveChangesAsync();
        }
    }
}
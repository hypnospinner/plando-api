using System;
using System.Threading.Tasks;
using Convey.Auth;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.Models;

namespace Plando.Commands.Auth
{
    public class AuthenticateUser : ICommand
    {
        public AuthenticateUser(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; } = null;
    }

    public class AuthenticateUserHandler :
        HandlerWithApplicationContext,
        ICommandHandler<AuthenticateUser>
    {
        private readonly IJwtHandler _jwtHandler;
        public AuthenticateUserHandler(
            ApplicationContext context,
            IJwtHandler jwtHandler) : base(context)
            => _jwtHandler = jwtHandler;

        public async Task HandleAsync(AuthenticateUser command)
        {
            var identity = await _context.Identities
                .SingleOrDefaultAsync(x => x.Email == command.Email);

            if (identity is null)
                throw new Exception($"No user with email: {command.Email} found");

            if ((identity.Email, identity.Password) != (command.Email, command.Password))
                throw new Exception($"Invalid password for {identity.Email}");

            var token = _jwtHandler.CreateToken(
                userId: identity.UserId.ToString(),
                role: identity.Role.ToString());

            command.Token = token.AccessToken;
        }
    }
}
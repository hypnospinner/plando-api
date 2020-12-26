using Convey.WebApi;
using Convey.WebApi.CQRS;
using Plando.Commands.Auth;
using Plando.Models.Users;

namespace Plando.Router
{
    internal static class AuthRouter
    {
        public static IDispatcherEndpointsBuilder AddAuthRouter(this IDispatcherEndpointsBuilder endpoints)
            => endpoints
                .Post<AuthenticateUser>(
                    path: "auth/login",
                    afterDispatch: (command, context) =>
                        command.Token is null ?
                        context.Response.Forbidden() :
                        context.Response.Ok(command.Token))
                .Post<RegisterUser>(
                    path: "auth/register"
                )
                .Post<RegisterManager>(
                    path: "auth/register/manager",
                    auth: true,
                    roles: UserRole.Administrator.ToString()
                );
    }
}
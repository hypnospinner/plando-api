using System.Collections.Generic;
using System.Threading.Tasks;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Plando.Commands.Users;
using Plando.DTOs;
using Plando.Models.Users;
using Plando.Queries.Users;
using Plando.Utils;

namespace Plando.Router
{
    internal static class UsersRouter
    {
        public static IDispatcherEndpointsBuilder AddUserRouter(this IDispatcherEndpointsBuilder endpoints)
            => endpoints
                // delete one user by id
                .Delete<DeleteUser>(
                    path: "users/{Id}",
                    afterDispatch: async (command, context) =>
                    {
                        context.Response.StatusCode = 204;
                        await context.Response.WriteJsonAsync(new
                        {
                            id = command.Id
                        });
                    },
                    auth: true,
                    roles: UserRole.Administrator.ToString())
                // get all users (with pagination)
                .Get<GetAllUsers, IEnumerable<UserDTO>>(
                    path: "users",
                    auth: true,
                    roles: UserRole.Administrator.ToString())
                // get profile of authorized user
                .Get<GetProfile, UserDTO>(
                    path: "profile",
                    beforeDispatch: (command, context) =>
                    {
                        command.Id = context.GetUserId();

                        return Task.CompletedTask;
                    },
                    auth: true);
    }
}
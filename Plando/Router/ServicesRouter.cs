using Convey.WebApi.CQRS;
using Plando.Commands.Services;
using Plando.Models.Users;

namespace Plando.Router
{
    public static class ServicesRouter
    {
        internal static IDispatcherEndpointsBuilder AddServicesRouter(this IDispatcherEndpointsBuilder endpoints)
            => endpoints
                .Post<AddService>(
                    path: "service/add",
                    auth: true,
                    roles: UserRole.Administrator.ToString())
                .Post<MakeServiceAvailableInLaundry>(
                    path: "service/enable",
                    auth: true,
                    roles: $"{UserRole.Administrator},{UserRole.Manager}"
                );
    }
}
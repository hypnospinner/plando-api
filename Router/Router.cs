using Convey.WebApi.CQRS;
using Microsoft.AspNetCore.Builder;

namespace Plando.Router
{
    public static class PlandoRouter
    {
        public static IApplicationBuilder UseRoutes(this IApplicationBuilder app) =>
            app.UseDispatcherEndpoints(endpoints => endpoints
                .AddUserRouter()
                .AddOrdersRouter()
                .AddServicesRouter()
                .AddLaundriesRouter()
                .AddAuthRouter());

    }
}
using System;
using Convey;
using Convey.Auth;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.Docs.Swagger;
using Convey.WebApi;
using Convey.WebApi.Swagger;
using Convey.WebApi.Exceptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plando.Models;
using Plando.Router;
using Plando.Common;

namespace Plando
{
    public static class Program
    {
        private const string PROD = "production";
        public static void Main(string[] args)
        {
            GetWebHostBuilder(args)
                .Build()
                .CreateDatabaseIfNotExists()
                .Run();
        }

        public static IWebHostBuilder GetWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => services
                    .AddDatabase(context.Configuration)
                    .AddConvey()
                    .AddCommandHandlers()
                    .AddQueryHandlers()
                    .AddEventHandlers()
                    .AddInMemoryCommandDispatcher()
                    .AddInMemoryQueryDispatcher()
                    .AddInMemoryEventDispatcher()
                    .AddWebApi()
                    .AddErrorHandler<ExceptionToResponseMapper>()
                    .AddSwaggerDocs()
                    .AddWebApiSwaggerDocs()
                    .AddJwt()
                    .Build())
                .Configure(app => app
                    .UseAuthentication()
                    .UseConvey()
                    .UseRoutes()
                    .UseSwaggerDocs());
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var env = (Environment.GetEnvironmentVariable("ENVIRONMENT"));

            if (env is PROD)
            {
                var dbConfig = new ApplicationContextConfig();
                configuration.Bind("mssqldb", dbConfig);

                return services.AddDbContext<ApplicationContext>(
                    options => options.UseSqlServer(dbConfig.ConnectionString));
            }
            else
            {
                var connectionString = configuration.GetSection("db")["connectionString"];

                return services.AddDbContext<ApplicationContext>(
                    options => options.UseSqlite(connectionString));
            }
        }

        private static IWebHost CreateDatabaseIfNotExists(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationContext>();
                    ApplicationContextSeed.Initialize(context);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            }

            return host;
        }
    }
}

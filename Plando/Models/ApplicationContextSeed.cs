using System.Collections.Generic;
using System.Linq;
using Bogus;
using Plando.Models.Laundries;
using Plando.Models.Services;
using Plando.Models.Users;

namespace Plando.Models
{
    public static class ApplicationContextSeed
    {
        public static void Initialize(ApplicationContext context)
        {
            context.Database.EnsureCreated();
            context
                .SeedAdministrators()
                .SeedLaundries();
        }

        private static ApplicationContext SeedAdministrators(this ApplicationContext context)
        {
            if (!context.Users.Any())
            {
                var user = new User
                {
                    Email = "admin@plando.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    Identity = new Identity
                    {
                        Email = "admin@plando.com",
                        Password = "ASDqwe!@#",
                        Role = UserRole.Administrator,
                    }
                };

                context.Users.Add(user);

                context.SaveChanges();
            }

            return context;
        }

        private static ApplicationContext SeedLaundries(this ApplicationContext context)
        {
            if (!context.Laundries.Any())
            {
                var managers = new List<User>(new User[] {
                    FakeData.GenerateManager(),
                    FakeData.GenerateManager()
                });

                var clients = FakeData.GenerateClients();

                var faker = new Faker();

                var laundry = new Laundry() { Address = faker.Address.FullAddress() };

                var services = FakeData.GetServices();
                context.Services.AddRange(services);
                context.SaveChanges();

                context.Laundries.Add(laundry);
                context.SaveChanges();

                var id = context.Laundries.FirstOrDefault().Id;

                var laundryServices = context.Services
                    .Select(x => new LaundryService() { ServiceId = x.Id, LaundryId = id });

                context.LaundryServices.AddRange(laundryServices);

                foreach (var manager in managers)
                    manager.LaundryId = id;

                context.Users.AddRange(managers);
                context.Users.AddRange(clients);
                context.SaveChanges();
            }

            return context;
        }

        private static class FakeData
        {
            private static readonly List<Service> _services = new List<Service>(
                new Service[] {
                    new Service { Id = 1, Title = "Cleaning", Price = 300 },
                    new Service { Id = 2, Title = "Washing", Price = 200 },
                    new Service { Id = 3, Title = "Dry Cleaning", Price = 750 },
                }
            );

            private static readonly Faker<User> _managerFaker = new Faker<User>()
                // .RuleFor(x => x.Id, _ => _userIds++)
                .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(x => x.Identity, (f, u) => new Identity
                {
                    // UserId = u.Id,
                    Role = UserRole.Manager,
                    Email = u.Email,
                    Password = f.Internet.Password(
                        length: 10,
                        memorable: true)
                });

            private static readonly Faker<User> _clientFaker = new Faker<User>()
                // .RuleFor(x => x.Id, _ => _userIds++)
                .RuleFor(x => x.FirstName, f => f.Name.FirstName())
                .RuleFor(x => x.LastName, f => f.Name.LastName())
                .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(x => x.Identity, (f, u) => new Identity
                {
                    // UserId = u.Id,
                    Role = UserRole.Client,
                    Email = u.Email,
                    Password = f.Internet.Password(
                        length: 10,
                        memorable: true)
                });

            private static readonly Faker<Laundry> _laundryFaker = new Faker<Laundry>()
                .RuleFor(x => x.Address, f => f.Address.FullAddress())
                .RuleFor(x => x.Managers, (f, t) =>
                {
                    var managers = _managerFaker.Generate(f.Random.Number(1, 3)).ToList();
                    foreach (var manager in managers)
                        manager.LaundryId = t.Id;

                    return managers;
                })
                .RuleFor(x => x.Services, (f, t) => new List<LaundryService>(
                    _services
                        .Select(x => new LaundryService { ServiceId = x.Id, LaundryId = t.Id })
                ));

            public static User GenerateManager() => _managerFaker.Generate();

            public static IEnumerable<Laundry> GenerateLaundries(int count = 3)
                => _laundryFaker.Generate(3);

            public static IEnumerable<Service> GetServices()
                => _services;

            public static IEnumerable<User> GenerateClients(int count = 3)
                => _clientFaker.Generate(count).ToList();
        }
    }
}
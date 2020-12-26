using Microsoft.EntityFrameworkCore;
using Plando.Models.Laundries;
using Plando.Models.Orders;
using Plando.Models.Services;
using Plando.Models.Users;

namespace Plando.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1 created <-> 1 passed
            modelBuilder
                .Entity<OrderCreatedEvent>()
                .HasOne<OrderPassedEvent>(x => x.OrderPassedEvent)
                .WithOne(x => x.OrderCreatedEvent)
                .HasForeignKey<OrderPassedEvent>(x => x.OrderId)
                .IsRequired(false);

            // 1 created <-> 1 put in progress
            modelBuilder
                .Entity<OrderCreatedEvent>()
                .HasOne<OrderPutInProgressEvent>(x => x.OrderPutInProgressEvent)
                .WithOne(x => x.OrderCreatedEvent)
                .HasForeignKey<OrderPutInProgressEvent>(x => x.OrderId)
                .IsRequired(false);

            // 1 created <-> 1 finished
            modelBuilder
                .Entity<OrderCreatedEvent>()
                .HasOne<OrderFinishedEvent>(x => x.OrderFinishedEvent)
                .WithOne(x => x.OrderCreatedEvent)
                .HasForeignKey<OrderFinishedEvent>(x => x.OrderId)
                .IsRequired(false);

            // 1 created <-> 1 cancelled
            modelBuilder
                .Entity<OrderCreatedEvent>()
                .HasOne<OrderCancelledEvent>(x => x.OrderCancelledEvent)
                .WithOne(x => x.OrderCreatedEvent)
                .HasForeignKey<OrderCancelledEvent>(x => x.OrderId)
                .IsRequired(false);

            // * service added events <-> 1 order created event
            modelBuilder
                .Entity<ServiceAddedEvent>()
                .HasOne<OrderCreatedEvent>(x => x.OrderCreatedEvent)
                .WithMany(x => x.ServiceAddedEvents)
                .HasForeignKey(x => x.OrderId);

            // * laundries <-> * services
            modelBuilder
                .Entity<LaundryService>()
                .HasKey(x => new { x.ServiceId, x.LaundryId });

            modelBuilder
                .Entity<LaundryService>()
                .HasOne<Laundry>(x => x.Laundry)
                .WithMany(x => x.Services)
                .HasForeignKey(x => x.LaundryId)
                .IsRequired(false);

            modelBuilder
                .Entity<LaundryService>()
                .HasOne<Service>(x => x.Service)
                .WithMany(x => x.Laundries)
                .HasForeignKey(x => x.ServiceId)
                .IsRequired();

            // * order created events <-> 1 laundry
            modelBuilder
                .Entity<OrderCreatedEvent>()
                .HasOne<Laundry>(x => x.Laundry)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.LaundryId)
                .IsRequired();

            // * service added event <-> 1 service
            modelBuilder
                .Entity<ServiceAddedEvent>()
                .HasOne<Service>(x => x.Service)
                .WithMany(x => x.ServiceAddedEvents)
                .HasForeignKey(x => x.ServiceId)
                .IsRequired();

            // 1 service added event <-> 1 servive removed event
            modelBuilder
               .Entity<ServiceAddedEvent>()
               .HasOne<ServiceRemovedEvent>(x => x.ServiceRemovedEvent)
               .WithOne(x => x.ServiceAddedEvent)
               .HasForeignKey<ServiceRemovedEvent>(x => x.ServiceAddedEventId)
               .IsRequired(false);

            // 1 service added event <-> 1 servive completed event
            modelBuilder
               .Entity<ServiceAddedEvent>()
               .HasOne<ServiceCompletedEvent>(x => x.ServiceCompletedEvent)
               .WithOne(x => x.ServiceAddedEvent)
               .HasForeignKey<ServiceCompletedEvent>(x => x.ServiceAddedEventId)
               .IsRequired(false);

            // 1 user <-> 1 identity 
            modelBuilder
                .Entity<User>()
                .HasOne<Identity>(x => x.Identity)
                .WithOne(x => x.User)
                .HasForeignKey<Identity>(x => x.UserId);

            // * users <-> 0/1 laundry
            modelBuilder
                .Entity<User>()
                .HasOne<Laundry>(x => x.Laundry)
                .WithMany(x => x.Managers)
                .HasForeignKey(x => x.LaundryId);

            // * order created events <-> 1 user 
            modelBuilder
                .Entity<OrderCreatedEvent>()
                .HasOne<User>(x => x.Client)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.ClientId);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Identity> Identities { get; set; }
        public DbSet<Laundry> Laundries { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<LaundryService> LaundryServices { get; set; }
        public DbSet<ServiceAddedEvent> ServiceAddedEvents { get; set; }
        public DbSet<ServiceRemovedEvent> ServiceRemovedEvents { get; set; }
        public DbSet<ServiceCompletedEvent> ServiceCompletedEvents { get; set; }
        public DbSet<OrderCreatedEvent> OrderCreatedEvents { get; set; }
        public DbSet<OrderPassedEvent> OrderPassedEvents { get; set; }
        public DbSet<OrderFinishedEvent> OrderFinishedEvents { get; set; }
        public DbSet<OrderCancelledEvent> OrderCancelledEvents { get; set; }
        public DbSet<OrderPutInProgressEvent> OrderPutInProgressEvents { get; set; }
    }
}
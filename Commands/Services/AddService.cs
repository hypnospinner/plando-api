using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Plando.Common;
using Plando.Models;
using Plando.Models.Services;

namespace Plando.Commands.Services
{
    public class AddService : Service, ICommand
    {
        public AddService(string title, decimal price)
        {
            Title = title;
            Price = price;
        }
    }

    public class AddServiceHandler : HandlerWithApplicationContext, ICommandHandler<AddService>
    {
        public AddServiceHandler(ApplicationContext context) : base(context) { }

        public async Task HandleAsync(AddService command)
        {
            _context.Services.Add(command as Service);
            await _context.SaveChangesAsync();
        }
    }
}
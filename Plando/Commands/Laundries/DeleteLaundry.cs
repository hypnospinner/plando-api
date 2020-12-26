using System;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.Models;

namespace Plando.Commands.Laundries
{
    public class DeleteLaundry : ICommand
    {
        public DeleteLaundry(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }

    public class DeleteLaundryHandler : HandlerWithApplicationContext, ICommandHandler<DeleteLaundry>
    {
        public DeleteLaundryHandler(ApplicationContext context) : base(context) { }
        public async Task HandleAsync(DeleteLaundry command)
        {
            var laundry = await _context.Laundries
                .SingleOrDefaultAsync(x => x.Id == command.Id);

            if (laundry is null)
                throw new Exception("Cannot delete this laundry: it does not exist");

            _context.Laundries.Remove(laundry);
            await _context.SaveChangesAsync();
        }
    }
}
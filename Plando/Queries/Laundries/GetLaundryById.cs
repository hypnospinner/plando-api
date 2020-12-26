using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Plando.Common;
using Plando.DTOs;
using Plando.Models;

namespace Plando.Queries.Laundries
{
    public class GetLaundryById : IQuery<LaundryDTO>
    {
        public int Id { get; set; }
    }

    public class GetLaundryByIdHandler : HandlerWithApplicationContext, IQueryHandler<GetLaundryById, LaundryDTO>
    {
        public GetLaundryByIdHandler(ApplicationContext context) : base(context) { }
        public async Task<LaundryDTO> HandleAsync(GetLaundryById query)
        {
            var laundry = await _context.Laundries.FindAsync(query.Id);
            return new LaundryDTO(laundry);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Microsoft.EntityFrameworkCore;
using Plando.Common;
using Plando.DTOs;
using Plando.Models;

namespace Plando.Queries.Laundries
{
    public class GetAllLaundries : PaginatedQuery, IQuery<IEnumerable<LaundryDTO>> { }

    public class GetAllLaundriesHandler : HandlerWithApplicationContext, IQueryHandler<GetAllLaundries, IEnumerable<LaundryDTO>>
    {
        public GetAllLaundriesHandler(ApplicationContext context) : base(context) { }
        public async Task<IEnumerable<LaundryDTO>> HandleAsync(GetAllLaundries query)
        {
            var laundries = await _context.Laundries
                .Skip(query.Page * query.PerPage)
                .Take(query.PerPage)
                .ToListAsync();

            return laundries
                .Select(x => new LaundryDTO(x))
                .AsEnumerable();
        }


    }
}
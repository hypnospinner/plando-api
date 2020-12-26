using System.Threading.Tasks;
using Convey.CQRS.Queries;
using Plando.Common;
using Plando.DTOs;
using Plando.Models;

namespace Plando.Queries.Users
{
    public class GetProfile : IQuery<UserDTO>
    {
        public int Id { get; set; }
    }

    public class GetUserByIdHandler : HandlerWithApplicationContext, IQueryHandler<GetProfile, UserDTO>
    {
        public GetUserByIdHandler(ApplicationContext context) : base(context) { }

        public async Task<UserDTO> HandleAsync(GetProfile query)
        {
            var user = await _context.Users.FindAsync(query.Id);

            return new UserDTO(user);
        }
    }
}
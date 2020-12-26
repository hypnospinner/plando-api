using Plando.Models;

namespace Plando.Common
{
    public abstract class HandlerWithApplicationContext
    {
        protected readonly ApplicationContext _context;

        protected HandlerWithApplicationContext(ApplicationContext context)
            => _context = context;
    }
}
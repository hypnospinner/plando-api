using System;
using Microsoft.AspNetCore.Http;
using static Plando.Common.TypedException;

namespace Plando.Utils
{
    public static class GetUserFromContextHelper
    {
        public static int GetUserId(this HttpContext context)
        {
            if (context.User == null || context.User.Identity == null)
                throw ApiException("No user information provided in HTTP request");

            if (Int32.TryParse(context.User.Identity.Name, out var id))
            {
                return id;
            }

            throw ApiException("Failed to parse user id. User id must be valid integer");
        }
    }
}
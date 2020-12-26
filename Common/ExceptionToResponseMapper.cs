using System;
using System.Net;
using Convey.WebApi.Exceptions;

namespace Plando.Common
{
    public class ExceptionToResponseMapper : IExceptionToResponseMapper
    {
        public ExceptionResponse Map(Exception exception) =>
            exception is TypedException ? new ExceptionResponse(new
            {
                code = (exception as TypedException).ExceptionType.ToString(),
                message = exception.Message
            }, HttpStatusCode.BadRequest) : new ExceptionResponse(new
            {
                code = "UNTYPED ERROR",
                message = exception.Message
            }, HttpStatusCode.BadRequest);
    }
}
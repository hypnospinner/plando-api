using System;

namespace Plando.Common
{
    public enum ExceptionType
    {
        BUSINESS,
        INFRASTRUCTURE,
        API
    }

    public class TypedException : Exception
    {
        public TypedException(string message, ExceptionType exceptionType) : base(message)
            => ExceptionType = exceptionType;

        public ExceptionType ExceptionType { get; set; }

        public static TypedException BusinessLogicException(string message)
            => new TypedException(message, ExceptionType.BUSINESS);

        public static TypedException InfrastructureException(string message)
            => new TypedException(message, ExceptionType.INFRASTRUCTURE);

        public static TypedException ApiException(string message)
            => new TypedException(message, ExceptionType.API);
    }
}
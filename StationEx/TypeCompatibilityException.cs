namespace StationEx
{
    using System;

    internal sealed class TypeCompatibilityException : Exception
    {
        public TypeCompatibilityException()
        {

        }

        public TypeCompatibilityException(string message)
            : base(message)
        {

        }

        public TypeCompatibilityException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}

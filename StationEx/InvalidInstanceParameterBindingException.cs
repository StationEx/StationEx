namespace StationEx
{
    using System;
    using Mono.Cecil;

    internal sealed class InvalidInstanceParameterBindingException : Exception
    {
        public InvalidInstanceParameterBindingException()
        {

        }

        public InvalidInstanceParameterBindingException(string message)
            : base(message)
        {

        }

        public InvalidInstanceParameterBindingException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public InvalidInstanceParameterBindingException(MethodDefinition sourceMethod, MethodDefinition targetMethod)
            : base($"Method '{sourceMethod.FullName}' attempts instance parameter binding with method '{targetMethod.FullName}' but target has no {{this}} parameter.")
        {

        }
    }
}

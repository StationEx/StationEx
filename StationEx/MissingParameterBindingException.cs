namespace StationEx
{
    using System;
    using Mono.Cecil;

    internal sealed class MissingParameterBindingException : Exception
    {
        public MissingParameterBindingException()
        {

        }

        public MissingParameterBindingException(MethodDefinition method, ParameterDefinition parameter)
            : base($"Method '{method.FullName}' is missing parameter binding for parameter '{parameter.Name}'")
        {

        }

        public MissingParameterBindingException(string message)
            : base(message)
        {

        }

        public MissingParameterBindingException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}

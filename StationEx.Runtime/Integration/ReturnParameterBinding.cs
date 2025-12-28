namespace StationEx.Runtime.Integration
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    internal sealed class ReturnParameterBinding : Attribute
    {
        private readonly bool isModifiable;

        public bool IsModifiable
        {
            get
            {
                return this.isModifiable;
            }
        }

        public ReturnParameterBinding(bool isModifiable)
        {
            this.isModifiable = isModifiable;
        }
    }
}

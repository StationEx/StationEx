namespace StationEx.Runtime.Integration
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class IntegrationAttribute : Attribute
    {
        private readonly IntegrationType type;
        private readonly string assembly;
        private readonly string method;

        public IntegrationType Type
        {
            get
            {
                return this.type;
            }
        }

        public string Assembly
        {
            get
            {
                return this.assembly;
            }
        }

        public string Method
        {
            get
            {
                return this.method;
            }
        }

        public IntegrationAttribute(IntegrationType type, string assembly, string method)
        {
            this.type = type;
            this.assembly = assembly;
            this.method = method;
        }
    }
}

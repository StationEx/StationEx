namespace StationEx.Runtime.Integration
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class IntegrationAttribute : Attribute
    {
        private readonly IntegrationType type;
        private readonly string module;
        private readonly string method;

        public IntegrationType Type
        {
            get
            {
                return this.type;
            }
        }

        public string Module
        {
            get
            {
                return this.module;
            }
        }

        public string Method
        {
            get
            {
                return this.method;
            }
        }

        public IntegrationAttribute(IntegrationType type, string module, string method)
        {
            this.type = type;
            this.module = module;
            this.method = method;
        }
    }
}

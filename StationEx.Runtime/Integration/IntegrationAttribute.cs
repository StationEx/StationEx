namespace StationEx.Runtime.Integration
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class IntegrationAttribute : Attribute
    {
        private readonly IntegrationMode mode;
        private readonly string module;
        private readonly string type;
        private readonly string method;

        public IntegrationMode Mode
        {
            get
            {
                return this.mode;
            }
        }

        public string Module
        {
            get
            {
                return this.module;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }
        }

        public string Method
        {
            get
            {
                return this.method;
            }
        }

        public IntegrationAttribute(IntegrationMode mode, string module, string type, string method)
        {
            this.mode = mode;
            this.module = module;
            this.type = type;
            this.method = method;
        }
    }
}

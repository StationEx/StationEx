namespace StationEx.Runtime.Integration
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal sealed class TypeAdapterAttribute : Attribute
    {
        private readonly string module;
        private readonly string type;

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

        public TypeAdapterAttribute(string module, string type)
        {
            this.module = module;
            this.type = type;
        }
    }
}

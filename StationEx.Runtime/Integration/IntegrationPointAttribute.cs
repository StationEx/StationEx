namespace StationEx.Runtime.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class IntegrationPointAttribute : Attribute
    {
        private readonly IntegrationPointType type;

        public IntegrationPointType Type
        {
            get
            {
                return this.type;
            }
        }

        public IntegrationPointAttribute(IntegrationPointType type)
        {
            this.type = type;
        }
    }
}

namespace StationEx.Runtime.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    internal sealed class ParameterBindingAttribute : Attribute
    {
        private readonly string name;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public ParameterBindingAttribute(string name)
        {
            this.name = name;
        }
    }
}

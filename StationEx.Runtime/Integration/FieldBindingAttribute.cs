namespace StationEx.Runtime.Integration
{
    using System;

    internal sealed class FieldBindingAttribute : Attribute
    {
        private readonly string name;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public FieldBindingAttribute(string name)
        {
            this.name = name;
        }
    }
}

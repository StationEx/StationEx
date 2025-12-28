namespace StationEx.Runtime.Integration
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal sealed class PropertyBindingAttribute : Attribute
    {
        private readonly string propertyName;

        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        public PropertyBindingAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }
    }
}

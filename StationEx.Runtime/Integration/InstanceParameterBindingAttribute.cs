namespace StationEx.Runtime.Integration
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    internal sealed class InstanceParameterBindingAttribute : Attribute
    {
        public InstanceParameterBindingAttribute()
        {

        }
    }
}

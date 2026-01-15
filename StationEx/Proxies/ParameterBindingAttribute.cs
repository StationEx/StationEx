namespace StationEx.Proxies
{
    internal sealed class ParameterBindingAttribute
    {
        public string Name
        {
            get;
            init;
        }

        public ParameterBindingAttribute(string name)
        {
            this.Name = name;
        }
    }
}

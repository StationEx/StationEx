namespace StationEx.Analysis
{
    internal sealed class PropertyBindingDescription
    {
        public string TargetPropertyName
        {
            get;
            init;
        }

        public PropertyBindingDescription(string targetPropertyName)
        {
            this.TargetPropertyName = targetPropertyName;
        }
    }
}

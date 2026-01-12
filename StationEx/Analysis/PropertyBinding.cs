namespace StationEx.Analysis
{
    using Mono.Cecil;

    internal sealed class PropertyBinding
    {
        public PropertyDefinition SourceProperty
        {
            get;
            init;
        }

        public PropertyDefinition TargetProperty
        {
            get;
            init;
        }

        public PropertyBinding(PropertyDefinition sourceProperty, PropertyDefinition targetProperty)
        {
            this.SourceProperty = sourceProperty;
            this.TargetProperty = targetProperty;
        }
    }
}

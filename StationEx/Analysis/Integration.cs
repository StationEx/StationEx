namespace StationEx.Analysis
{
    using Mono.Cecil;

    internal sealed class Integration
    {
        public IntegrationType Type
        {
            get;
            init;
        }

        public MethodDefinition Source
        {
            get;
            init;
        }

        public MethodDefinition Target
        {
            get;
            init;
        }

        public Integration(IntegrationType type, MethodDefinition source, MethodDefinition target)
        {
            this.Type = type;
            this.Source = source;
            this.Target = target;
        }
    }
}

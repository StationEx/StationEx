namespace StationEx
{
    using Mono.Cecil;

    internal sealed class IntegrationIntent
    {
        public MethodDefinition Source
        {
            get;
            init;
        }

        public int Type
        {
            get;
            init;
        }

        public string TargetModuleName
        {
            get;
            init;
        }

        public string TargetMethodName
        {
            get;
            init;
        }

        public IntegrationIntent(MethodDefinition source, int type, string targetAssemblyName, string targetMethodName)
        {
            this.Source = source;
            this.Type = type;
            this.TargetModuleName = targetAssemblyName;
            this.TargetMethodName = targetMethodName;
        }
    }
}

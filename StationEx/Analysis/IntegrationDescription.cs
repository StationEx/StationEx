namespace StationEx.Analysis
{
    internal sealed class IntegrationDescription
    {
        public int Mode
        {
            get;
            init;
        }

        public string TargetModuleName
        {
            get;
            init;
        }

        public string TargetTypeName
        {
            get;
            init;
        }

        public string TargetMethodName
        {
            get;
            init;
        }

        public IntegrationDescription(int mode, string targetModuleName, string targetTypeName, string targetMethodName)
        {
            this.Mode = mode;
            this.TargetModuleName = targetModuleName;
            this.TargetTypeName = targetTypeName;
            this.TargetMethodName = targetMethodName;
        }
    }
}

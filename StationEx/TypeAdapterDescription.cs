namespace StationEx
{
    internal sealed class TypeAdapterDescription
    {
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

        public TypeAdapterDescription(string targetModuleName, string targetTypeName)
        {
            this.TargetModuleName = targetModuleName;
            this.TargetTypeName = targetTypeName;
        }
    }
}

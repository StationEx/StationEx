namespace StationEx.Analysis
{
    using Mono.Cecil;

    internal sealed class TypeAdapter
    {
        public TypeDefinition SourceType
        {
            get;
            init;
        }

        public TypeDefinition TargetType
        {
            get;
            init;
        }

        public TypeAdapter(TypeDefinition sourceType, TypeDefinition targetType)
        {
            this.SourceType = sourceType;
            this.TargetType = targetType;
        }
    }
}

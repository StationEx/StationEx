namespace StationEx.Analysis
{
    using System.Diagnostics.CodeAnalysis;
    using Mono.Cecil;
    using Mono.Cecil.Rocks;

    internal static class ConstructorHelper
    {
        public static bool TryGetDefaultConstructor(TypeDefinition type, [NotNullWhen(true)] out MethodDefinition? constructor)
        {
            foreach (MethodDefinition method in type.GetConstructors())
            {
                if (method.HasParameters)
                {
                    continue;
                }

                constructor = method;
                return true;
            }

            constructor = null;
            return false;
        }
    }
}

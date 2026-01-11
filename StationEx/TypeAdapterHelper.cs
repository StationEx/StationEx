namespace StationEx
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Mono.Cecil;

    internal static class TypeAdapterHelper
    {
        private static bool IsTypeAdapterAttribute(CustomAttribute attribute)
        {
            return
                attribute.AttributeType.Module.Name == "StationEx.Runtime.dll" &&
                attribute.AttributeType.FullName == "StationEx.Runtime.Integration.TypeAdapterAttribute" &&
                attribute.ConstructorArguments.Count == 2 &&
                attribute.ConstructorArguments[0].Type.FullName == "System.String" &&
                attribute.ConstructorArguments[1].Type.FullName == "System.String";
        }

        private static bool TryGetTypeAdapterDescription(TypeDefinition type, [NotNullWhen(true)] out TypeAdapterDescription? description)
        {
            if (type.IsClass || type.IsValueType)
            {
                foreach (CustomAttribute attribute in type.CustomAttributes)
                {
                    if (IsTypeAdapterAttribute(attribute))
                    {
                        description = new TypeAdapterDescription(
                            (string)attribute.ConstructorArguments[0].Value,
                            (string)attribute.ConstructorArguments[1].Value);

                        return true;
                    }
                }
            }

            description = null;
            return false;
        }

        private static bool TryGetTypeAdapter(TypeDefinition sourceType, TypeAdapterDescription description, AssemblyDefinition targetAssembly, [NotNullWhen(true)] out TypeAdapter? adapter)
        {
            ModuleDefinition? targetModule = targetAssembly.Modules.SingleOrDefault(module => module.Name == description.TargetModuleName);
            if (targetModule is null)
            {
                adapter = null;
                return false;
            }

            TypeDefinition? targetType = targetModule.Types.SingleOrDefault(type => type.FullName == description.TargetTypeName);
            if (targetType is null)
            {
                adapter = null;
                return false;
            }

            adapter = new TypeAdapter(sourceType, targetType);
            return true;
        }

        public static List<TypeAdapter> GetTypeAdapters(IEnumerable<Integration> integrations, AssemblyDefinition target)
        {
            List<TypeAdapter> adapters = new List<TypeAdapter>();

            foreach (Integration integration in integrations)
            {
                foreach (ParameterDefinition sourceParameter in integration.Source.Parameters)
                {
                    TypeDefinition sourceType = sourceParameter.ParameterType.Resolve();
                    if (TryGetTypeAdapterDescription(sourceType, out TypeAdapterDescription? description))
                    {
                        if (TryGetTypeAdapter(sourceType, description, target, out TypeAdapter? adapter))
                        {
                            adapters.Add(adapter);
                        }
                    }
                }
            }

            return adapters;
        }
    }
}

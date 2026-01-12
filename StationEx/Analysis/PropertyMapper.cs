namespace StationEx.Analysis
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Mono.Cecil;

    internal static class PropertyMapper
    {
        private static bool TryMapInheritedProperty(PropertyDefinition sourceProperty, TypeDefinition targetType, string targetPropertyName, [NotNullWhen(true)] out PropertyDefinition? targetProperty)
        {
            while (targetType is not null)
            {
                targetProperty = targetType.Properties.SingleOrDefault(property => property.Name == targetPropertyName);
                if (targetProperty is not null)
                {
                    return true;
                }

                targetType = targetType.BaseType.Resolve();
            }

            targetProperty = null;
            return false;
        }

        private static bool TryMapInterfaceProperty(PropertyDefinition sourceProperty, TypeDefinition targetType, string targetPropertyName, [NotNullWhen(true)] out PropertyDefinition? targetProperty)
        {
            foreach (InterfaceImplementation implementation in targetType.Interfaces)
            {
                TypeDefinition type = implementation.InterfaceType.Resolve();

                targetProperty = type.Properties.SingleOrDefault(property => property.Name == targetPropertyName);
                if (targetProperty is not null)
                {
                    return true;
                }
            }

            targetProperty = null;
            return false;
        }

        public static bool TryMapProperty(PropertyDefinition sourceProperty, TypeDefinition targetType, string targetPropertyName, [NotNullWhen(true)] out PropertyDefinition? targetProperty)
        {
            if (TryMapInheritedProperty(sourceProperty, targetType, targetPropertyName, out targetProperty))
            {
                return true;
            }

            if (TryMapInterfaceProperty(sourceProperty, targetType, targetPropertyName, out targetProperty))
            {
                return true;
            }

            targetProperty = null;
            return false;
        }
    }
}

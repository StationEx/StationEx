namespace StationEx.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Mono.Cecil;
    using StationEx.Constants;

    internal static class PropertyBindingHelper
    {
        private static bool IsPropertyBindingAttribute(CustomAttribute attribute)
        {
            return
                attribute.AttributeType.Module.Name == ModuleNames.StationExRuntime &&
                attribute.AttributeType.FullName == TypeNames.StationExRuntimePropertyBindingAttributeFull &&
                attribute.ConstructorArguments.Count == 1 &&
                attribute.ConstructorArguments[0].Type.FullName == typeof(String).FullName;
        }

        private static bool TryGetPropertyBindingDescription(PropertyDefinition property, [NotNullWhen(true)] out PropertyBindingDescription? description)
        {
            foreach (CustomAttribute attribute in property.CustomAttributes)
            {
                if (IsPropertyBindingAttribute(attribute))
                {
                    description = new PropertyBindingDescription(
                        (string)attribute.ConstructorArguments[0].Value);

                    return true;
                }
            }

            description = null;
            return false;
        }

        private static bool TryGetPropertyBinding(PropertyDefinition sourceProperty, PropertyBindingDescription description, TypeDefinition targetType, [NotNullWhen(true)] out PropertyBinding? binding)
        {
            PropertyDefinition? targetProperty = targetType.Properties.SingleOrDefault(property => property.FullName == description.TargetPropertyName);
            if (targetProperty is null)
            {
                binding = null;
                return false;
            }

            binding = new PropertyBinding(sourceProperty, targetProperty);
            return true;
        }

        public static List<PropertyBinding> GetPropertyBindings(TypeDefinition sourceType, TypeDefinition targetType)
        {
            List<PropertyBinding> bindings = new List<PropertyBinding>();

            foreach (PropertyDefinition sourceProperty in sourceType.Properties)
            {
                foreach (CustomAttribute sourceAttribute in sourceProperty.CustomAttributes)
                {
                    if (IsPropertyBindingAttribute(sourceAttribute))
                    {
                        if (TryGetPropertyBindingDescription(sourceProperty, out PropertyBindingDescription? description))
                        {
                            if (TryGetPropertyBinding(sourceProperty, description, targetType, out PropertyBinding? binding))
                            {
                                bindings.Add(binding);
                            }
                        }
                    }
                }
            }

            return bindings;
        }
    }
}

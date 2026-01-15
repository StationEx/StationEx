namespace StationEx.Proxies.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Mono.Cecil;
    using StationEx.Constants;

    internal static class ParameterDefinitionExtensions
    {
        public static bool IsInstanceBinding(this ParameterDefinition self)
        {
            foreach (CustomAttribute attribute in self.CustomAttributes)
            {
                if (attribute.AttributeType.FullName == TypeNames.StationExRuntimeInstanceBindingAttributeFull)
                {
                    Debug.Assert(!attribute.HasProperties, "BUG CHECK: Attribute does not have the expected number of properties.");

                    return true;
                }
            }

            return false;
        }

        public static bool TryGetBinding(this ParameterDefinition self, [NotNullWhen(true)] out ParameterBindingAttribute? result)
        {
            foreach (CustomAttribute attribute in self.CustomAttributes)
            {
                if (attribute.AttributeType.FullName == TypeNames.StationExRuntimePropertyBindingAttributeFull)
                {
                    // For now we'll thoroughly check the structure of the type, but we should do better.
                    // TODO: Emit reference types from the runtime without making it a dependency of the compiler.
                    Debug.Assert(attribute.HasProperties, "BUG CHECK: Attribute does not have properties when expected.");
                    Debug.Assert(attribute.Properties.Count == 1, "BUG CHECK: Attribute does not have the expected number of properties.");
                    Debug.Assert(attribute.Properties[0].Name == "Name", "BUG CHECK: Attribute does not have the expected property name at position 0.");
                    Debug.Assert(attribute.Properties[0].Argument.Type.FullName == typeof(String).FullName, "BUG CHECK: Attribute property at position 0 is not of the expected type.");

                    result = new ParameterBindingAttribute((string)attribute.Properties[0].Argument.Value);
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}

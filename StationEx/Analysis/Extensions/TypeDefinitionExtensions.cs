namespace StationEx.Analysis.Extensions
{
    using System;
    using System.Diagnostics;
    using Mono.Cecil;
    using StationEx.Constants;

    internal static class TypeDefinitionExtensions
    {
        public static bool IsAdapter(this TypeDefinition self)
        {
            foreach (CustomAttribute attribute in self.CustomAttributes)
            {
                if (attribute.AttributeType.FullName == TypeNames.StationExRuntimeTypeAdapterAttributeFull)
                {
                    return true;
                }
            }

            return false;
        }

        public static TypeDefinition GetAdaptedType(this TypeDefinition self)
        {
            string? typeName = null;
            foreach (CustomAttribute attribute in self.CustomAttributes)
            {
                if (attribute.AttributeType.FullName != TypeNames.StationExRuntimeTypeAdapterAttributeFull)
                {
                    continue;
                }

                foreach (CustomAttributeNamedArgument property in attribute.Properties)
                {
                    if (property.Name != "Type")
                    {
                        continue;
                    }

                    typeName = (string)property.Argument.Value;
                    break;
                }
            }

            if (typeName is null)
            {
                throw new InvalidOperationException("Unable to get adapted type for a type that is not a type adapter.");
            }

            TypeReference? type;
            if (!self.Module.TryGetTypeReference(typeName, out type))
            {
                throw new InvalidOperationException("Unable to get adapted type for a type that is not within the same module as the type adapter.");
            }

            return type.Resolve();
        }

        public static MethodDefinition GetConverter(this TypeDefinition self)
        {
            TypeDefinition adaptedType = self.GetAdaptedType();

            TypeReference typeAdapterReference;
            if (!self.Module.TryGetTypeReference(TypeNames.StationExTypeAdapterFull, out typeAdapterReference))
            {
                throw new InvalidOperationException("Unable to get the converter because the containing type was not found.");
            }

            TypeDefinition typeAdapterType = typeAdapterReference.Resolve();
            foreach (MethodDefinition converter in typeAdapterType.Methods)
            {
                Debug.Assert(converter.Parameters.Count == 1, $"Type adapter converter '{converter.FullName}' had an unexpected number of parameters.");

                if (converter.Parameters[0].ParameterType.FullName == self.FullName)
                {
                    return converter;
                }
            }

            throw new InvalidOperationException("Unable to get the converter because it was not found.");
        }
    }
}

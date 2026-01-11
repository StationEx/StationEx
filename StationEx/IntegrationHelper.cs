namespace StationEx
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Mono.Cecil;

    internal static class IntegrationHelper
    {
        private static bool IsIntegrationAttribute(CustomAttribute attribute)
        {
            return
                attribute.AttributeType.Module.Name == "StationEx.Runtime.dll" &&
                attribute.AttributeType.FullName == "StationEx.Runtime.Integration.IntegrationAttribute" &&
                attribute.ConstructorArguments.Count == 4 &&
                attribute.ConstructorArguments[0].Type.FullName == "StationEx.Runtime.Integration.IntegrationMode" &&
                attribute.ConstructorArguments[1].Type.FullName == "System.String" &&
                attribute.ConstructorArguments[2].Type.FullName == "System.String" &&
                attribute.ConstructorArguments[3].Type.FullName == "System.String";
        }

        private static bool TryGetIntegrationDescription(MethodDefinition method, [NotNullWhen(true)] out IntegrationDescription? description)
        {
            if (method.IsStatic)
            {
                foreach (CustomAttribute attribute in method.CustomAttributes)
                {
                    if (IsIntegrationAttribute(attribute))
                    {
                        description = new IntegrationDescription(
                            (int)attribute.ConstructorArguments[0].Value,
                            (string)attribute.ConstructorArguments[1].Value,
                            (string)attribute.ConstructorArguments[2].Value,
                            (string)attribute.ConstructorArguments[3].Value);

                        return true;
                    }
                }
            }

            description = null;
            return false;
        }

        private static bool TryGetIntegration(MethodDefinition sourceMethod, IntegrationDescription description, AssemblyDefinition targetAssembly, [NotNullWhen(true)] out Integration? integration)
        {
            ModuleDefinition? targetModule = targetAssembly.Modules.SingleOrDefault(module => module.Name == description.TargetModuleName);
            if (targetModule is null)
            {
                integration = null;
                return false;
            }

            TypeDefinition? targetType = targetModule.Types.SingleOrDefault(type => type.FullName == description.TargetTypeName);
            if (targetType is null)
            {
                integration = null;
                return false;
            }

            MethodDefinition? targetMethod = targetType.Methods.SingleOrDefault(method => method.FullName == description.TargetMethodName);
            if (targetMethod is null)
            {
                integration = null;
                return false;
            }

            integration = new Integration((IntegrationType)description.Mode, sourceMethod, targetMethod);
            return true;
        }

        public static List<Integration> GetIntegrations(AssemblyDefinition assembly, AssemblyDefinition target)
        {
            List<Integration> integrations = new List<Integration>();

            foreach (ModuleDefinition sourceModule in assembly.Modules)
            {
                foreach (TypeDefinition sourceType in sourceModule.Types)
                {
                    foreach (MethodDefinition sourceMethod in sourceType.Methods)
                    {
                        if (TryGetIntegrationDescription(sourceMethod, out IntegrationDescription? description))
                        {
                            if (TryGetIntegration(sourceMethod, description, target, out Integration? integration))
                            {
                                integrations.Add(integration);
                            }
                        }
                    }
                }
            }

            return integrations;
        }
    }
}

namespace StationEx
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Mono.Cecil;
    using Mono.Cecil.Rocks;
    using Mono.CompilerServices.SymbolWriter;

    internal static class AssemblyPatch
    {
        private static bool IsValidIntegrationAttribute(CustomAttribute attribute)
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
                    if (IsValidIntegrationAttribute(attribute))
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

        private static List<Integration> GetIntegrations(AssemblyDefinition assembly, AssemblyDefinition target)
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

        private static void DeleteRuntimeReference(AssemblyDefinition target)
        {
            AssemblyNameReference? stationExRuntimeReference = target.MainModule.AssemblyReferences.SingleOrDefault(reference => reference.Name == "StationEx.Runtime");
            if (stationExRuntimeReference is not null)
            {
                target.MainModule.AssemblyReferences.Remove(stationExRuntimeReference);
            }
        }

        private static void DeleteStaticIntegrations(AssemblyDefinition target)
        {
            TypeDefinition? integrationCoreType = target.MainModule.Types.SingleOrDefault(type => type.FullName == "StationEx.Generated.IntegrationCore");
            if (integrationCoreType is not null)
            {
                target.MainModule.Types.Remove(integrationCoreType);
            }
        }

        private static void ApplyRuntimeReference(AssemblyDefinition target)
        {
            AssemblyNameReference reference = new AssemblyNameReference("StationEx.Runtime", new Version("1.0.0.0"));
            target.MainModule.AssemblyReferences.Add(reference);
        }

        private static void CopyStaticIntegrationHandlers(AssemblyDefinition target, IEnumerable<Integration> integrations)
        {
            TypeDefinition integrationCoreType = new TypeDefinition("StationEx.Generated", "IntegrationCore", TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Sealed);

            foreach (Integration sourceIntegration in integrations)
            {
                MethodDefinition targetHandler = new MethodDefinition(
                    $"StationEx.Generated.{sourceIntegration.Source.Name}",
                    MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                    sourceIntegration.Source.ReturnType);

                foreach (ParameterDefinition sourceHandlerParameter in sourceIntegration.Source.Parameters)
                {
                    ParameterDefinition targetHandlerParameter = new ParameterDefinition(
                        sourceHandlerParameter.Name,
                        sourceHandlerParameter.Attributes,
                        target.MainModule.ImportReference(sourceHandlerParameter.ParameterType));

                    targetHandler.Parameters.Add(targetHandlerParameter);
                }

                targetHandler.Body = sourceIntegration.Source.Body;
                integrationCoreType.Methods.Add(targetHandler);
            }

            target.MainModule.Types.Add(integrationCoreType);
        }

        private static void ApplyStaticIntegrations(AssemblyDefinition source, AssemblyDefinition target, Integration integration)
        {

        }

        private static void ApplyStaticIntegrations(AssemblyDefinition source, AssemblyDefinition target)
        {
            List<Integration> integrations = GetIntegrations(source, target);
            CopyStaticIntegrationHandlers(target, integrations);

            foreach (Integration integration in integrations)
            {
                ApplyStaticIntegrations(source, target, integration);
            }
        }

        public static void ApplyRuntimeIntegration(AssemblyDefinition source, AssemblyDefinition target)
        {
            DeleteStaticIntegrations(target);
            DeleteRuntimeReference(target);

            ApplyRuntimeReference(target);
            ApplyStaticIntegrations(source, target);
        }
    }
}

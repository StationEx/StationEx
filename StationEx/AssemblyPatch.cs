namespace StationEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;
    using StationEx.Analysis;
    using StationEx.Constants;

    internal static class AssemblyPatch
    {
        private static void DeleteRuntimeReference(AssemblyDefinition target)
        {
            AssemblyNameReference? stationExRuntimeReference = target.MainModule.AssemblyReferences.SingleOrDefault(reference => reference.Name == AssemblyNames.StationExRuntime);
            if (stationExRuntimeReference is not null)
            {
                target.MainModule.AssemblyReferences.Remove(stationExRuntimeReference);
            }
        }

        private static void DeleteStaticTypeAdapters(AssemblyDefinition target)
        {
            TypeDefinition? integrationAdapterType = target.MainModule.Types.SingleOrDefault(type => type.FullName == TypeNames.StationExTypeAdapterFull);
            if (integrationAdapterType is not null)
            {
                target.MainModule.Types.Remove(integrationAdapterType);
            }
        }

        private static void DeleteStaticIntegrations(AssemblyDefinition target)
        {
            TypeDefinition? integrationCoreType = target.MainModule.Types.SingleOrDefault(type => type.FullName == TypeNames.StationExIntegrationCoreFull);
            if (integrationCoreType is not null)
            {
                target.MainModule.Types.Remove(integrationCoreType);
            }
        }

        private static void ApplyRuntimeReference(AssemblyDefinition target)
        {
            AssemblyNameReference reference = new AssemblyNameReference(AssemblyNames.StationExRuntime, Versions.CurrentStationExRuntime);
            target.MainModule.AssemblyReferences.Add(reference);
        }

        private static void CopyStaticTypeAdapters(AssemblyDefinition target, IEnumerable<TypeAdapter> adapters)
        {
            TypeDefinition integrationCoreType = new TypeDefinition(Namespaces.StationExGenerated, TypeNames.StationExTypeAdapter, TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Sealed);
            foreach (TypeAdapter adapter in adapters)
            {
                throw new NotImplementedException();
            }
        }

        private static void CopyStaticIntegrationHandlers(AssemblyDefinition target, IEnumerable<Integration> integrations)
        {
            TypeDefinition integrationCoreType = new TypeDefinition(Namespaces.StationExGenerated, TypeNames.StationExIntegrationCore, TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Sealed);

            foreach (Integration sourceIntegration in integrations)
            {
                MethodDefinition targetHandler = new MethodDefinition(
                    $"{Namespaces.StationExGenerated}.{sourceIntegration.Source.Name}",
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
            throw new NotImplementedException();
        }

        private static void ApplyStaticIntegrations(AssemblyDefinition source, AssemblyDefinition target)
        {
            DeleteStaticTypeAdapters(target);

            List<Integration> integrations = IntegrationHelper.GetIntegrations(source, target);
            List<TypeAdapter> adapters = TypeAdapterHelper.GetTypeAdapters(integrations, target);

            CopyStaticTypeAdapters(source, adapters);
            CopyStaticIntegrationHandlers(target, integrations);

            foreach (Integration integration in integrations)
            {
                ApplyStaticIntegrations(source, target, integration);
            }
        }

        public static void ApplyRuntimeIntegration(AssemblyDefinition source, AssemblyDefinition target)
        {
            ApplyRuntimeReference(target);
            ApplyStaticIntegrations(source, target);
        }

        public static void DeleteRuntimeIntegration(AssemblyDefinition target)
        {
            DeleteStaticIntegrations(target);
            DeleteRuntimeReference(target);
        }
    }
}

namespace StationEx
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using StationEx.Analysis;
    using StationEx.Constants;

    internal static class RuntimeCompiler
    {
        private static void DeleteRuntimeReference(AssemblyDefinition target)
        {
            AssemblyNameReference? stationExRuntimeReference = target.MainModule.AssemblyReferences.SingleOrDefault(reference => reference.Name == AssemblyNames.StationExRuntime);
            if (stationExRuntimeReference is not null)
            {
                target.MainModule.AssemblyReferences.Remove(stationExRuntimeReference);
            }
        }

        private static void DeleteTypeAdapters(AssemblyDefinition target)
        {
            TypeDefinition? integrationAdapterType = target.MainModule.Types.SingleOrDefault(type => type.FullName == TypeNames.StationExTypeAdapterFull);
            if (integrationAdapterType is not null)
            {
                target.MainModule.Types.Remove(integrationAdapterType);
            }
        }

        private static void DeleteIntegrationCore(AssemblyDefinition target)
        {
            TypeDefinition? integrationCoreType = target.MainModule.Types.SingleOrDefault(type => type.FullName == TypeNames.StationExIntegrationCoreFull);
            if (integrationCoreType is not null)
            {
                target.MainModule.Types.Remove(integrationCoreType);
            }
        }

        private static void DeleteStaticIntegrations(AssemblyDefinition target)
        {
            DeleteIntegrationCore(target);
            DeleteTypeAdapters(target);
        }

        private static void CreateRuntimeReference(AssemblyDefinition target)
        {
            AssemblyNameReference reference = new AssemblyNameReference(AssemblyNames.StationExRuntime, Versions.CurrentStationExRuntime);
            target.MainModule.AssemblyReferences.Add(reference);
        }
        
        private static void CreateIntegrationCore(AssemblyDefinition target)
        {
            TypeDefinition integrationCoreType = new TypeDefinition(
                Namespaces.StationExGenerated, TypeNames.StationExIntegrationCore,
                TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Sealed);

            target.MainModule.Types.Add(integrationCoreType);
        }

        private static MethodDefinition GenerateAdapterConverter(AssemblyDefinition target, TypeAdapter adapter)
        {
            TypeReference adapterConverterReturnType = target.MainModule.ImportReference(adapter.SourceType);
            MethodDefinition adapterConverterMethod = new MethodDefinition(
                $"Convert{adapter.TargetType.Name}To{adapter.SourceType.Name}",
                MethodAttributes.Public | MethodAttributes.Static,
                adapterConverterReturnType);

            ParameterDefinition targetTypeParameter = new ParameterDefinition("value", ParameterAttributes.None, adapter.TargetType);
            adapterConverterMethod.Parameters.Add(targetTypeParameter);

            ILProcessor processor = adapterConverterMethod.Body.GetILProcessor();

            MethodDefinition? sourceTypeConstructor;
            if (!ConstructorHelper.TryGetDefaultConstructor(adapter.SourceType, out sourceTypeConstructor))
            {
                throw new TypeCompatibilityException($"The adapter source type '{adapter.SourceType.FullName}' does not have a default constructor.");
            }

            processor.Emit(OpCodes.Newobj, target.MainModule.ImportReference(sourceTypeConstructor));

            IEnumerable<PropertyBinding> propertyBindings = PropertyBindingHelper.GetPropertyBindings(adapter.SourceType, adapter.TargetType);
            foreach (PropertyBinding propertyBinding in propertyBindings)
            {
                processor.Emit(OpCodes.Dup);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Callvirt, target.MainModule.ImportReference(propertyBinding.TargetProperty.GetMethod));
                processor.Emit(OpCodes.Callvirt, target.MainModule.ImportReference(propertyBinding.SourceProperty.SetMethod));
            }

            processor.Emit(OpCodes.Ret);

            return adapterConverterMethod;
        }

        private static void CreateTypeAdapters(AssemblyDefinition target, IEnumerable<TypeAdapter> adapters)
        {
            TypeDefinition typeAdapterType = new TypeDefinition(Namespaces.StationExGenerated, TypeNames.StationExTypeAdapter, TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Sealed);

            foreach (TypeAdapter adapter in adapters)
            {
                MethodDefinition adapterConverterMethod = GenerateAdapterConverter(target, adapter);
                typeAdapterType.Methods.Add(adapterConverterMethod);
            }

            target.MainModule.Types.Add(typeAdapterType);
        }

        private static MethodDefinition CreateStaticIntegrationHandler(AssemblyDefinition target, Integration integration)
        {
            MethodDefinition targetHandler = new MethodDefinition(
                $"{Namespaces.StationExGenerated}.{integration.Source.Name}",
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                integration.Source.ReturnType);

            foreach (ParameterDefinition sourceHandlerParameter in integration.Source.Parameters)
            {
                ParameterDefinition targetHandlerParameter = new ParameterDefinition(
                    sourceHandlerParameter.Name,
                    sourceHandlerParameter.Attributes,
                    target.MainModule.ImportReference(sourceHandlerParameter.ParameterType));

                targetHandler.Parameters.Add(targetHandlerParameter);
            }

            targetHandler.Body = integration.Source.Body;
            return targetHandler;
        }

        private static void CreateBeforeCallToHandler(AssemblyDefinition source, AssemblyDefinition target, MethodDefinition handler)
        {

        }

        private static void CreateStaticIntegration(AssemblyDefinition source, AssemblyDefinition target, Integration integration)
        {
            MethodDefinition handler = CreateStaticIntegrationHandler(target, integration);

            switch (integration.Type)
            {
                case IntegrationType.Before:
                    CreateBeforeCallToHandler(source, target, handler);
                    break;

                case IntegrationType.After:
                    throw new NotImplementedException();

                case IntegrationType.Replace:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentException("The integration type is not valid.", nameof(integration));
            }
        }

        private static void CreateStaticIntegrations(AssemblyDefinition source, AssemblyDefinition target)
        {
            List<Integration> integrations = IntegrationHelper.GetIntegrations(source, target);
            List<TypeAdapter> adapters = TypeAdapterHelper.GetTypeAdapters(integrations, target);

            CreateTypeAdapters(target, adapters);

            CreateIntegrationCore(target);
            foreach (Integration integration in integrations)
            {
                CreateStaticIntegration(source, target, integration);
            }
        }

        public static void Install(AssemblyDefinition source, AssemblyDefinition target)
        {
            CreateRuntimeReference(target);
            CreateStaticIntegrations(source, target);
        }

        public static void Uninstall(AssemblyDefinition target)
        {
            DeleteStaticIntegrations(target);
            DeleteRuntimeReference(target);
        }
    }
}

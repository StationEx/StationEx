namespace StationEx
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using StationEx.Analysis;
    using StationEx.Analysis.Extensions;
    using StationEx.Constants;
    using StationEx.Proxies;
    using StationEx.Proxies.Extensions;

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

        private static IntegrationHandler CreateStaticIntegrationHandler(AssemblyDefinition target, Integration integration)
        {
            MethodDefinition integrationMethod = new MethodDefinition(
                $"{Namespaces.StationExGenerated}.{integration.Source.Name}",
                MethodAttributes.Public | MethodAttributes.Static,
                integration.Source.ReturnType);

            int[] integrationToTargetParameterMap = new int[integration.Source.Parameters.Count];
            for (int i = 0; i < integration.Source.Parameters.Count; ++i)
            {
                ParameterDefinition sourceParameter = integration.Source.Parameters[i];
                if (sourceParameter.IsInstanceBinding())
                {
                    if (!integration.Target.HasThis)
                    {
                        throw new InvalidInstanceParameterBindingException(integration.Source, integration.Target);
                    }

                    integrationToTargetParameterMap[i] = 0;
                    continue;
                }

                ParameterBindingAttribute? sourceParameterBinding;
                if (!sourceParameter.TryGetBinding(out sourceParameterBinding))
                {
                    throw new MissingParameterBindingException(integration.Source, sourceParameter);
                }

                bool isTargetParameterFound = false;
                for (int k = 0; k < integration.Target.Parameters.Count; ++k)
                {
                    if (integration.Target.Parameters[k].Name == sourceParameterBinding.Name)
                    {
                        isTargetParameterFound = true;
                        integrationToTargetParameterMap[i] = k;
                        break;
                    }
                }

                if (!isTargetParameterFound)
                {
                    // TODO: Throw an exception to indicate we are missing the target of a parameter binding
                }

                ParameterDefinition integrationParameter = new ParameterDefinition(
                    sourceParameter.Name,
                    sourceParameter.Attributes,
                    target.MainModule.ImportReference(sourceParameter.ParameterType));

                integrationMethod.Parameters.Add(integrationParameter);
            }

            Debug.Assert(integrationMethod.Parameters.Count == integration.Source.Parameters.Count, "BUG CHECK: The integration source method and integration method have a different number of parameters.");

            integrationMethod.Body = integration.Source.Body;

            return new IntegrationHandler(integrationMethod, integrationToTargetParameterMap);
        }

        private static void CreateStaticBeforeIntegrationToHandler(Integration integration, IntegrationHandler handler)
        {
            throw new NotImplementedException();
        }

        private static void CreateStaticAfterIntegrationToHandler(Integration integration, IntegrationHandler handler)
        {
            throw new NotImplementedException();
        }

        private static void CreateStaticIntegration(AssemblyDefinition source, AssemblyDefinition target, Integration integration)
        {
            IntegrationHandler handler = CreateStaticIntegrationHandler(target, integration);

            switch (integration.Type)
            {
                case IntegrationType.Before:
                    CreateStaticBeforeIntegrationToHandler(integration, handler);
                    break;

                case IntegrationType.After:
                    CreateStaticAfterIntegrationToHandler(integration, handler);
                    break;

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

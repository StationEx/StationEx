namespace StationEx
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Mono.Cecil;

    internal sealed class AssemblyPatch
    {
        private const string IntegrationAttributeTypeName = "StationEx.Runtime.Integration.IntegrationAttribute";

        private static bool TryGetIntegrationIntent(MethodDefinition methodDefinition, [NotNullWhen(true)] out IntegrationIntent? intent)
        {
            if (methodDefinition.IsStatic)
            {
                foreach (CustomAttribute customAttribute in methodDefinition.CustomAttributes)
                {
                    if (customAttribute.AttributeType.FullName == IntegrationAttributeTypeName)
                    {
                        intent = new IntegrationIntent(
                            methodDefinition,
                            (int)customAttribute.ConstructorArguments[0].Value,
                            (string)customAttribute.ConstructorArguments[1].Value,
                            (string)customAttribute.ConstructorArguments[2].Value);

                        return true;
                    }
                }
            }

            intent = null;
            return false;
        }

        private static List<IntegrationIntent> GetIntegrationIntents(AssemblyDefinition assembly, AssemblyDefinition target)
        {
            List<IntegrationIntent> intents = new List<IntegrationIntent>();

            foreach (ModuleDefinition moduleDefinition in assembly.Modules)
            {
                foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
                {
                    foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                    {
                        IntegrationIntent? intent;
                        if (!TryGetIntegrationIntent(methodDefinition, out intent))
                        {
                            continue;
                        }

                        if (intent.TargetModuleName != target.MainModule.Name)
                        {
                            continue;
                        }

                        intents.Add(intent);
                    }
                }
            }

            return intents;
        }

        public static void ApplyStaticIntegrations(AssemblyDefinition source, AssemblyDefinition target)
        {
            List<IntegrationIntent> intents = GetIntegrationIntents(source, target);
            foreach (IntegrationIntent intent in intents)
            {

            }
        }
    }
}

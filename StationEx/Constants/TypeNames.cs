namespace StationEx.Constants
{
    internal static class TypeNames
    {
        public const string StationExTypeAdapter = "TypeAdapter";
        public const string StationExTypeAdapterFull = $"{Namespaces.StationExGenerated}.{TypeNames.StationExTypeAdapter}";

        public const string StationExIntegrationCore = "IntegrationCore";
        public const string StationExIntegrationCoreFull = $"{Namespaces.StationExGenerated}.{TypeNames.StationExIntegrationCore}";

        public const string StationExRuntimeTypeAdapterAttribute = "TypeAdapterAttribute";
        public const string StationExRuntimeTypeAdapterAttributeFull = $"{Namespaces.StationExRuntimeIntegrations}.{TypeNames.StationExRuntimeTypeAdapterAttribute}";

        public const string StationExRuntimeInstanceBindingAttribute = "InstanceBindingAttribute";
        public const string StationExRuntimeInstanceBindingAttributeFull = $"{Namespaces.StationExRuntimeIntegrations}.{TypeNames.StationExRuntimeInstanceBindingAttribute}";

        public const string StationExRuntimePropertyBindingAttribute = "PropertyBindingAttribute";
        public const string StationExRuntimePropertyBindingAttributeFull = $"{Namespaces.StationExRuntimeIntegrations}.{TypeNames.StationExRuntimePropertyBindingAttribute}";

        public const string StationExRuntimeIntegrationAttribute = "IntegrationAttribute";
        public const string StationExRuntimeIntegrationAttributeFull = $"{Namespaces.StationExRuntimeIntegrations}.{TypeNames.StationExRuntimeIntegrationAttribute}";

        public const string StationExRuntimeIntegrationMode = "IntegrationMode";
        public const string StationExRuntimeIntegrationModeFull = $"{Namespaces.StationExRuntimeIntegrations}.{TypeNames.StationExRuntimeIntegrationMode}";
    }
}

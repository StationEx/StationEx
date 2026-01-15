namespace StationEx.Analysis
{
    using Mono.Cecil;

    internal sealed class IntegrationHandler
    {
        public MethodDefinition Target
        {
            get;
            init;
        }

        public int[] ParameterMap
        {
            get;
            init;
        }

        public IntegrationHandler(MethodDefinition target, int[] parameterMap)
        {
            this.Target = target;
            this.ParameterMap = parameterMap;
        }
    }
}

namespace StationEx
{
    using Mono.Cecil;

    internal static class Program
    {
        private static readonly ReaderParameters AssemblyReaderParameters = new ReaderParameters()
        {
            AssemblyResolver = new DefaultAssemblyResolver()
        };

        private static void Main(string[] parameters)
        {
            AssemblyDefinition source = AssemblyDefinition.ReadAssembly(parameters[0], AssemblyReaderParameters);
            AssemblyDefinition target = AssemblyDefinition.ReadAssembly(parameters[1], AssemblyReaderParameters);

            AssemblyPatch.ApplyStaticIntegrations(source, target);
        }
    }
}

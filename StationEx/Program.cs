namespace StationEx
{
    using System;
    using System.IO;
    using Mono.Cecil;

    internal static class Program
    {
        private static readonly DefaultAssemblyResolver SourceAssemblyResolver = new DefaultAssemblyResolver();
        private static readonly DefaultAssemblyResolver TargetAssemblyResolver = new DefaultAssemblyResolver();

        private static readonly ReaderParameters SourceReaderOptions = new ReaderParameters()
        {
            AssemblyResolver = SourceAssemblyResolver
        };

        private static readonly ReaderParameters TargetReaderOptions = new ReaderParameters()
        {
            AssemblyResolver = TargetAssemblyResolver
        };

        private static void Main(string[] parameters)
        {
            SourceAssemblyResolver.AddSearchDirectory(Path.GetDirectoryName(parameters[0]));
            TargetAssemblyResolver.AddSearchDirectory(Path.GetDirectoryName(parameters[1]));

            AssemblyDefinition source = AssemblyDefinition.ReadAssembly(parameters[0], SourceReaderOptions);
            AssemblyDefinition target = AssemblyDefinition.ReadAssembly(parameters[1], TargetReaderOptions);

            AssemblyPatch.DeleteRuntimeIntegration(target);
            AssemblyPatch.ApplyRuntimeIntegration(source, target);

            target.Write(parameters[2]);
        }
    }
}

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
            string? sourceDirectoryName = Path.GetDirectoryName(parameters[0]);
            string? targetDirectoryName = Path.GetDirectoryName(parameters[1]);
            string? outputDirectoryName = Path.GetDirectoryName(parameters[2]);

            if (sourceDirectoryName is null ||
                targetDirectoryName is null ||
                outputDirectoryName is null)
            {
                return;
            }

            SourceAssemblyResolver.AddSearchDirectory(sourceDirectoryName);

            TargetAssemblyResolver.AddSearchDirectory(sourceDirectoryName);
            TargetAssemblyResolver.AddSearchDirectory(targetDirectoryName);

            AssemblyDefinition source = AssemblyDefinition.ReadAssembly(parameters[0], SourceReaderOptions);
            AssemblyDefinition target = AssemblyDefinition.ReadAssembly(parameters[1], TargetReaderOptions);

            RuntimeCompiler.Uninstall(target);
            RuntimeCompiler.Install(source, target);

            foreach (string filePath in Directory.GetFiles(sourceDirectoryName))
            {
                string? inputFileName = Path.GetFileName(filePath);
                if (inputFileName is null || !inputFileName.StartsWith("StationEx", StringComparison.Ordinal))
                {
                    continue;
                }

                string? outputFileName = Path.Combine(outputDirectoryName, inputFileName);

                if (File.Exists(outputFileName))
                {
                    File.Delete(outputFileName);
                }

                File.Copy(filePath, outputFileName);
            }

            target.Write(parameters[2]);
        }
    }
}

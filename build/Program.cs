using Microsoft.Build.Utilities;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

public static partial class Program
{
    private const string PackageId = "jnm2.ReferenceAssemblies.net35";
    private const string FrameworkName = ".NETFramework";
    private const string FrameworkVersion = "v3.5";
    private const string RelativeNupkgDestination = @"build\" + FrameworkName + @"\" + FrameworkVersion;
    private const string ArtifactsDir = "artifacts";

    public static void Main()
    {
        SetCurrentDirectoryToRoot();

        // https://github.com/microsoft/msbuild/blob/9c33693c492a0cb99474dcb703bfd0947056d8a9/src/Tasks/GetReferenceAssemblyPaths.cs#L236-L241
        var referenceAssemblyPaths = ToolLocationHelper.GetPathToReferenceAssemblies(
            FrameworkName,
            FrameworkVersion,
            targetFrameworkProfile: "",
            targetFrameworkRootPath: null,
            targetFrameworkFallbackSearchPaths: null);

        var redistListsByRelativePath = CombineRedistLists(referenceAssemblyPaths);

        var builder = new PackageBuilder($@"src\{PackageId}.nuspec", basePath: "src", propertyProvider: null, includeEmptyDirectories: false);

        AddRedistFilesToBuilder(referenceAssemblyPaths, redistListsByRelativePath, builder, out var excludedAssemblyNames);

        SaveCombinedRedistLists(redistListsByRelativePath, excludedAssemblyNames);

        builder.AddFiles("src", source: @"**\*", destination: "", exclude: "*.nuspec");

        Directory.CreateDirectory(ArtifactsDir);

        using (var stream = File.Create(Path.Combine(ArtifactsDir, $"{builder.Id}.{builder.Version}.nupkg")))
            builder.Save(stream);

        Console.WriteLine("Build succeeded.");
    }

    private static void SetCurrentDirectoryToRoot()
    {
        var current = Directory.GetCurrentDirectory();

        while (!Directory.Exists(Path.Combine(current, "src")))
        {
            current = Path.GetDirectoryName(current);

            if (string.IsNullOrEmpty(current))
                throw new NotImplementedException("Cannot find the src directory.");
        }

        Directory.SetCurrentDirectory(current);
    }

    private static IReadOnlyDictionary<string, RedistList> CombineRedistLists(IList<string> referenceAssemblyPaths)
    {
        var redistListByRelativePath = new Dictionary<string, RedistList>(StringComparer.OrdinalIgnoreCase);

        foreach (var rootPath in referenceAssemblyPaths)
        {
            foreach (var list in ReadRedistLists(rootPath))
            {
                if (redistListByRelativePath.TryGetValue(list.RelativePath, out var existingList))
                    existingList.CombineDocument(list.Document);
                else
                    redistListByRelativePath.Add(list.RelativePath, list);
            }
        }

        return redistListByRelativePath;
    }


    private static IReadOnlyCollection<RedistList> ReadRedistLists(string rootPath)
    {
        return new[] { "RedistList", "SubsetList" }
            .SelectMany(folderName =>
            {
                try
                {
                    return Directory.GetFiles(Path.Combine(rootPath, folderName), "*.xml");
                }
                catch (DirectoryNotFoundException)
                {
                    return Enumerable.Empty<string>();
                }
            })
            .Select(listPath => new RedistList(
                MakeRelative(listPath, rootPath),
                XDocument.Load(listPath)))
            .ToList();
    }

    private static string MakeRelative(string fullPath, string relativeParentPath)
    {
        relativeParentPath = relativeParentPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        if (relativeParentPath.Last() != Path.DirectorySeparatorChar)
            relativeParentPath += Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(relativeParentPath, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("The given path must be inside the parent path.");

        return fullPath.Substring(relativeParentPath.Length);
    }

    private static void SaveCombinedRedistLists(IReadOnlyDictionary<string, RedistList> redistListsByRelativePath, IReadOnlyCollection<string> excludedAssemblyNames)
    {
        foreach (var listByRelativePath in redistListsByRelativePath)
        {
            var combinedListPath = Path.Combine("src", RelativeNupkgDestination, listByRelativePath.Key);
            Directory.CreateDirectory(Path.GetDirectoryName(combinedListPath));

            listByRelativePath.Value.RemoveAssemblies(excludedAssemblyNames);
            listByRelativePath.Value.Document.Save(combinedListPath);
        }
    }

    private static void AddRedistFilesToBuilder(
        IList<string> referenceAssemblyPaths,
        IReadOnlyDictionary<string, RedistList> redistListsByRelativePath,
        PackageBuilder builder,
        out IReadOnlyCollection<string> excludedAssemblyNames)
    {
        var assemblyNames = redistListsByRelativePath.Values
            .SelectMany(list => list.AssemblyNames)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var usedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var rootPath in referenceAssemblyPaths)
        {
            foreach (var filePath in Directory.GetFiles(rootPath))
            {
                if (HasAnyExtension(filePath, ".dll", ".xml")
                    && usedFileNames.Add(Path.GetFileName(filePath))
                    && assemblyNames.Contains(Path.GetFileNameWithoutExtension(filePath)))
                {
                    var destination = HasAnyExtension(filePath, ".dll")
                        ? Path.Combine(RelativeNupkgDestination, AssemblyName.GetAssemblyName(filePath).Name + ".dll")
                        : RelativeNupkgDestination;

                    builder.AddFiles(rootPath, source: filePath, destination);
                }
            }
        }

        // Assemblies that are missing are .exe files and .dlls that are not in the root folder. They are excluded by
        // design because they don't show up in Visual Studio's Add References dialog.
        assemblyNames.ExceptWith(usedFileNames
            .Where(name => HasAnyExtension(name, ".dll"))
            .Select(Path.GetFileNameWithoutExtension));

        excludedAssemblyNames = assemblyNames;
    }

    private static bool HasAnyExtension(string path, params string[] extensions)
    {
        return extensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
    }
}

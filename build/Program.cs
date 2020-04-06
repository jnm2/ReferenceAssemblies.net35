using Microsoft.Build.Utilities;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

public static partial class Program
{
    public static void Main()
    {
        SetCurrentDirectoryToRoot();

        Build(
            packageIdSuffix: "net35",
            targetFrameworkIdentifier: ".NETFramework",
            targetFrameworkVersion: "v3.5",
            useVisualStudioTargetFrameworkRootPath: false,
            artifactsDir: "artifacts");
    }

    private static void Build(
        string packageIdSuffix,
        string targetFrameworkIdentifier,
        string targetFrameworkVersion,
        bool useVisualStudioTargetFrameworkRootPath,
        string artifactsDir)
    {
        // https://github.com/microsoft/msbuild/blob/9c33693c492a0cb99474dcb703bfd0947056d8a9/src/Tasks/GetReferenceAssemblyPaths.cs#L236-L241
        var referenceAssemblyPaths = ToolLocationHelper.GetPathToReferenceAssemblies(
            targetFrameworkIdentifier,
            targetFrameworkVersion,
            targetFrameworkProfile: "",
            targetFrameworkRootPath: useVisualStudioTargetFrameworkRootPath
                ? GetVisualStudioTargetFrameworkRootPath()
                : null,
            targetFrameworkFallbackSearchPaths: null);

        var redistListsByRelativePath = CombineRedistLists(referenceAssemblyPaths);

        var builder = new PackageBuilder($@"src\jnm2.ReferenceAssemblies.{packageIdSuffix}.nuspec", basePath: "src", propertyProvider: null, includeEmptyDirectories: false);

        var relativeNupkgDestination = $@"build\{targetFrameworkIdentifier}\{targetFrameworkVersion}";

        AddRedistFilesToBuilder(relativeNupkgDestination, referenceAssemblyPaths, redistListsByRelativePath, builder, out var excludedAssemblyNames);

        SaveCombinedRedistLists(relativeNupkgDestination, redistListsByRelativePath, excludedAssemblyNames);

        builder.AddFiles("src", source: @"**\*", destination: "", exclude: "*.nuspec");

        Directory.CreateDirectory(artifactsDir);

        var nupkgPath = Path.Combine(artifactsDir, $"{builder.Id}.{builder.Version}.nupkg");

        using (var stream = File.Create(nupkgPath))
            builder.Save(stream);

        Console.WriteLine("Built successfully: " + nupkgPath);
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

    private static string GetVisualStudioTargetFrameworkRootPath()
    {
        using var process = new Process
        {
            StartInfo =
            {
                FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft Visual Studio\Installer\vswhere.exe"),
                Arguments = @"-find Common7\IDE\ReferenceAssemblies\Microsoft\Framework",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            }
        };

        process.Start();

        var lines = new List<string>();
        while (process.StandardOutput.ReadLine() is { } line)
            lines.Add(line);

        process.WaitForExit();
        if (process.ExitCode != 0)
            throw new NotImplementedException("vswhere exited with code " + process.ExitCode);

        if (lines.Count != 1)
            throw new NotImplementedException("Expected a single line. Lines:" + Environment.NewLine + string.Join(Environment.NewLine, lines));

        return lines[0];
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

    private static void SaveCombinedRedistLists(
        string relativeNupkgDestination,
        IReadOnlyDictionary<string, RedistList> redistListsByRelativePath,
        IReadOnlyCollection<string> excludedAssemblyNames)
    {
        foreach (var listByRelativePath in redistListsByRelativePath)
        {
            var combinedListPath = Path.Combine("src", relativeNupkgDestination, listByRelativePath.Key);
            Directory.CreateDirectory(Path.GetDirectoryName(combinedListPath));

            listByRelativePath.Value.RemoveAssemblies(excludedAssemblyNames);
            listByRelativePath.Value.Document.Save(combinedListPath);
        }
    }

    private static void AddRedistFilesToBuilder(
        string relativeNupkgDestination,
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
                        ? Path.Combine(relativeNupkgDestination, AssemblyName.GetAssemblyName(filePath).Name + ".dll")
                        : relativeNupkgDestination;

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

using System.Diagnostics;
using System.Text;
using AiProgrammer.CodeProcessing;

namespace AiProgrammer.IO;

public class ProjectContentReader : IProjectContentReader
{
    private readonly IClassExtractor _classExtractor;

    public ProjectContentReader(IClassExtractor classExtractor)
    {
        _classExtractor = classExtractor;
    }

    public static IReadOnlyList<string> GetFilesPaths()
    {
        return PrintFiles(GetRepoWorkingDirectory());
    }

    public string GetClassesWithoutMethodBodies(IReadOnlyList<FilePath> relativePaths)
    {
        StringBuilder stringBuilder = new StringBuilder();
        
        foreach (FilePath path in relativePaths)
        {
            FileContent fileContent = GetClassWithSelectedMethodBodies(path.Value, Array.Empty<string>());
            
            stringBuilder.AppendLine($"Below is a content stripped of method bodies of {fileContent.FilePath}");
            stringBuilder.AppendLine("```");
            stringBuilder.AppendLine(fileContent.Content);
            stringBuilder.AppendLine("```");
            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    private static string GetRepoWorkingDirectory()
    {
        const string envName = "GITHUB_WORKSPACE";
        string? path = Environment.GetEnvironmentVariable(envName);

        if (string.IsNullOrEmpty(path))
        {
            throw new Exception($"Environemnt variable '{envName}' is not set");
        }

        return path;
    }
    
    private static IReadOnlyList<string> PrintFiles(string rootPath)
    {
        List<string> files = new();
        PrintFilesRecursive(files, rootPath, rootPath);

        return files;
    }

    private static void PrintFilesRecursive(List<string> files, string path, string rootPath)
    {
        string directoryName = Path.GetFileName(path);
        if (directoryName == "bin" || directoryName == "obj" || directoryName.StartsWith("."))
        {
            return;
        }
        
        if (Directory.Exists(path))
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                PrintFilesRecursive(files, dir, rootPath);
            }

            foreach (string file in Directory.GetFiles(path))
            {
                string fileExtension = Path.GetExtension(file);

                if (fileExtension is ".dll" or ".exe" or ".obj" or ".pdb" or ".meta" or ".asset")
                {
                    continue;
                }

                string relativePath = Path.GetRelativePath(rootPath, file);
                files.Add(relativePath);
            }
        }
        else
        {
            Trace.WriteLine($"Invalid directory path: {path}");
        }
    }

    public string GetClassesWithSelectedMethodBodies(IReadOnlyList<string> filesAndMethods)
    {
        Dictionary<string, List<string>> methodsToShow = GetMethodsToShow(filesAndMethods);

        IReadOnlyList<FileContent> files = GetClassesWithMethods(methodsToShow).ToArray();

        StringBuilder stringBuilder = new();

        foreach (FileContent file in files)
        {
            stringBuilder.AppendLine($"Below is a content of {file.FilePath}");
            stringBuilder.AppendLine("```");
            stringBuilder.AppendLine(file.Content);
            stringBuilder.AppendLine("```");
            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }

    public string ReadFile(FilePath filePath)
    {
        string fullPath = Path.Combine(GetRepoWorkingDirectory(), filePath.Value);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Cannot read file content: it does not exist", filePath.Value);
        }

        return File.ReadAllText(fullPath);
    }

    public bool DoesFileExist(FilePath path)
    {
        string fullPath = Path.Combine(GetRepoWorkingDirectory(), path.Value);

        return File.Exists(fullPath);
    }

    private IEnumerable<FileContent> GetClassesWithMethods(Dictionary<string, List<string>> methodsToShow)
    {
        foreach (KeyValuePair<string, List<string>> methodToShow in methodsToShow)
        {
            yield return GetClassWithSelectedMethodBodies(methodToShow.Key, methodToShow.Value);
        }
    }

    private FileContent GetClassWithSelectedMethodBodies(string filePath, IReadOnlyList<string> methods)
    {
        string fullPath = Path.Combine(GetRepoWorkingDirectory(), filePath);

        string classContent = _classExtractor.GetClassContentWithSelectedMethodBodies(fullPath, methods);

        return new FileContent(filePath, classContent);
    }

    private static Dictionary<string, List<string>> GetMethodsToShow(IReadOnlyList<string> filesAndMethods)
    {
        Dictionary<string, List<string>> methodToShow = new();

        foreach (string fileAndMethod in filesAndMethods)
        {
            string[] parts = fileAndMethod.Split(' ');

            if (parts.Length != 2)
            {
                Console.WriteLine($"Invalid requested method format: '{fileAndMethod}'");
                continue;
            }

            string file = parts[0];

            if (string.IsNullOrEmpty(file))
            {
                Console.WriteLine($"Invalid requested method format: '{fileAndMethod}'");
                continue;
            }

            List<string>? methods = methodToShow.GetValueOrDefault(file);

            methods ??= new List<string>();

            string method = parts[1];

            if (string.IsNullOrEmpty(method))
            {
                Console.WriteLine($"Invalid requested method format: '{fileAndMethod}'");
                continue;
            }

            methods.Add(method);
            methodToShow[file] = methods;
        }

        return methodToShow;
    }

    public static async Task<string?> GetFileContent(string fileRelativePath)
    {
        string fullPath = Path.Combine(GetRepoWorkingDirectory(), fileRelativePath);

        if (!File.Exists(fullPath))
        {
            return null;
        }
        
        return await File.ReadAllTextAsync(fullPath);
    }
}

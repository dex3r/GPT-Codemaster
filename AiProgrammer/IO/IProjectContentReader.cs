namespace AiProgrammer.IO;

public interface IProjectContentReader
{
    string GetClassesWithoutMethodBodies(IReadOnlyList<FilePath> relativePaths);
    string GetClassesWithSelectedMethodBodies(IReadOnlyList<string> filesAndMethods);
    string ReadFile(FilePath filePath);
    bool DoesFileExist(FilePath path);
}

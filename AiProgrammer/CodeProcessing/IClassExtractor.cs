namespace AiProgrammer.CodeProcessing
{
    public interface IClassExtractor
    {
        string GetClassContentWithSelectedMethodBodies(string fullFilePath, IReadOnlyList<string> methods);
        string GetClassContentWithSelectedMethodBodies(TextReader streamReader, IReadOnlyList<string> methods);
    }
}
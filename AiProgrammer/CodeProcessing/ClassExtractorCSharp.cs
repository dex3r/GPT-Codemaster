using System.Text;

namespace AiProgrammer.CodeProcessing;

public class ClassExtractorCSharp : IClassExtractor
{
    public string GetClassContentWithSelectedMethodBodies(string fullFilePath, IReadOnlyList<string> methods)
    {
        return GetClassContentWithSelectedMethodBodies(File.OpenText(fullFilePath), methods);
    }

    //TODO: Longest method generated. Too long, refactor, preferably using AI itself.
    public string GetClassContentWithSelectedMethodBodies(TextReader streamReader, IReadOnlyList<string> methods)
    {
        StringBuilder stringBuilder = new StringBuilder();

        string line;
        int braceCount = 0;
        int insideMethodBraceCount = -1;

        int classStartBraceCount = 0;
        bool isInsideClass = false;
        bool isInsideMethod = false;
        bool nextLineIsNamespace = false;
        bool nextLineIsClass = false;
        bool waitForNamespaceEnd = false;
        bool waitForClassEnd = false;

        void PrintLine(string line)
        {
            stringBuilder.AppendLine(line);
        }

        while ((line = streamReader.ReadLine()) != null)
        {
            if (!line.Contains(";") && !line.Contains("\"")
                                    && (line.Contains(" namespace ") || line.Contains("\tnamespace ") || line.StartsWith("namespace ")))
            {
                PrintLine(line);
                nextLineIsNamespace = true;
                continue;
            }

            if (!line.Contains("\"") &&
                (line.Contains(" class ") || line.Contains("\tclass ")))
            {
                PrintLine(line);
                nextLineIsClass = true;
                isInsideClass = true;
                braceCount++;
                classStartBraceCount = braceCount;
                continue;
            }

            if (nextLineIsNamespace)
            {
                waitForNamespaceEnd = true;
                nextLineIsNamespace = false;
                PrintLine(line);
                continue;
            }

            if (nextLineIsClass)
            {
                waitForClassEnd = true;
                nextLineIsClass = false;
                PrintLine(line);
                continue;
            }

            if (line.Trim().EndsWith("{"))
            {
                braceCount++;

                if (isInsideMethod)
                {
                    insideMethodBraceCount++;
                }
            }

            if (braceCount <= classStartBraceCount)
            {
                PrintLine(line);

                if (!isInsideMethod && methods.Any(method => line.Contains(method)))
                {
                    isInsideMethod = true;
                    insideMethodBraceCount = 0;
                }
            }
            else if (isInsideMethod)
            {
                PrintLine(line);
            }

            if (line.Trim().EndsWith("}"))
            {
                if (braceCount == classStartBraceCount)
                {
                    isInsideClass = false;
                }
                
                braceCount--;

                if (isInsideMethod)
                {
                    insideMethodBraceCount--;

                    if (insideMethodBraceCount == 0)
                    {
                        isInsideMethod = false;
                    }
                }

                if (braceCount == 0)
                {
                    if (waitForClassEnd)
                    {
                        waitForClassEnd = false;
                    }
                    else if (waitForNamespaceEnd)
                    {
                        waitForNamespaceEnd = false;
                    }
                }
            }
        }

        return stringBuilder.ToString();
    }
}

using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AiProgrammer.CodeProcessing
{
    public class ClassExtractorCSharp : IClassExtractor
    {
        public string GetClassContentWithSelectedMethodBodies(string fullFilePath, IReadOnlyList<string> methods)
        {
            return GetClassContentWithSelectedMethodBodies(File.OpenText(fullFilePath), methods);
        }

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
                if (IsNamespaceDeclaration(line))
                {
                    HandleNamespaceDeclaration(line, ref nextLineIsNamespace);
                    continue;
                }

                if (IsClassDeclaration(line))
                {
                    HandleClassDeclaration(line, ref isInsideClass, ref braceCount, ref classStartBraceCount, ref nextLineIsClass);
                    continue;
                }

                if (nextLineIsNamespace)
                {
                    HandleNextLineNamespace(line, ref waitForNamespaceEnd, ref nextLineIsNamespace);
                    continue;
                }

                if (nextLineIsClass)
                {
                    HandleNextLineClass(line, ref waitForClassEnd, ref nextLineIsClass);
                    continue;
                }

                ProcessLine(line, ref braceCount, ref insideMethodBraceCount, ref isInsideClass, ref isInsideMethod, classStartBraceCount, methods);
            }

            return stringBuilder.ToString();
        }

        private bool IsNamespaceDeclaration(string line)
        {
            return !line.Contains(";") && !line.Contains("\"")
                && (line.Contains(" namespace ") || line.Contains("\tnamespace ") || line.StartsWith("namespace "));
        }

        private void HandleNamespaceDeclaration(string line, ref bool nextLineIsNamespace)
        {
            PrintLine(line);
            nextLineIsNamespace = true;
        }

        private bool IsClassDeclaration(string line)
        {
            return !line.Contains("\"") &&
                (line.Contains(" class ") || line.Contains("\tclass "));
        }

        private void HandleClassDeclaration(string line, ref bool isInsideClass, ref int braceCount, ref int classStartBraceCount, ref bool nextLineIsClass)
        {
            PrintLine(line);
            nextLineIsClass = true;
            isInsideClass = true;
            braceCount++;
            classStartBraceCount = braceCount;
        }

        private void HandleNextLineNamespace(string line, ref bool waitForNamespaceEnd, ref bool nextLineIsNamespace)
        {
            waitForNamespaceEnd = true;
            nextLineIsNamespace = false;
            PrintLine(line);
        }

        private void HandleNextLineClass(string line, ref bool waitForClassEnd, ref bool nextLineIsClass)
        {
            waitForClassEnd = true;
            nextLineIsClass = false;
            PrintLine(line);
        }

        private void ProcessLine(string line, ref int braceCount, ref int insideMethodBraceCount, ref bool isInsideClass, ref bool isInsideMethod, int classStartBraceCount, IReadOnlyList<string> methods)
        {
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
    }
}
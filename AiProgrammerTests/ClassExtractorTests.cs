using System.Collections.Generic;
using System.IO;
using AiProgrammer;
using AiProgrammer.CodeProcessing;
using NUnit.Framework;

namespace AiProgrammerTests;

public class ClassExtractorTests
{
    [Test]
    public void EmptyClassTest()
    {
        string content = """
public class EmptyClass
{
}
""";

        RunTest(content, content);
    }

    [Test]
    public void EmptyClassInNamespaceTest()
    {
        string content = """
using A;

namespace A;

public class EmptyClass
{
}
""";

        RunTest(content, content);
    }

    [Test]
    public void EmptyClassInNamespaceTest2()
    {
        string content = """
using A;

namespace A
{
    public class EmptyClass
    {
    }
}
""";

        RunTest(content, content);
    }

    [Test]
    public void ClassWithOneMethod()
    {
        string content = """
using A;

namespace A
{
    public class EmptyClass
    {
        public string SomeMethod(string param1, int param2){}
    }
}
""";

        RunTest(content, content);
    }

    [Test]
    public void ClassWithOneMethod2()
    {
        string content = """
using A;

namespace A
{
    public class EmptyClass
    {
        public string SomeMethod(string param1, int param2)
        {
        }
    }
}
""";

        string expected = """
using A;

namespace A
{
    public class EmptyClass
    {
        public string SomeMethod(string param1, int param2)
    }
}
""";

        RunTest(content, expected);
    }

    [Test]
    public void ClassWithOneMethodWithBody()
    {
        string content = """
using A;

namespace A
{
    public class EmptyClass
    {
        public string SomeMethod(string param1, int param2)
        {
            BlaBla();
            BlaBla2();
        }
    }
}
""";

        string expected = """
using A;

namespace A
{
    public class EmptyClass
    {
        public string SomeMethod(string param1, int param2)
    }
}
""";

        RunTest(content, expected);
    }

    [Test]
    public void ClassWithOneMethodWithBodyComplicated()
    {
        string content = """
using A;

namespace A
{
    public class EmptyClass
    {
        public string SomeMethod(string param1, int param2)
        {
            BlaBla();
            BlaBla2();

            if(A)
            {
                foreach(asdasd)
                {
                    DASDAS();
                }
            }
        }
    }
}
""";

        string expected = """
using A;

namespace A
{
    public class EmptyClass
    {
        public string SomeMethod(string param1, int param2)
    }
}
""";

        RunTest(content, expected);
    }

    [Test]
    public void ClassWithOneMethodWithExtract()
    {
        string content = """
using A;

namespace A
{
    public class EmptyClass
    {
        public string SomeMethod(string param1, int param2)
        {
            BlaBla();
            BlaBla2();

            if(A)
            {
                foreach(asdasd)
                {
                    DASDAS();
                }
            }
        }
    }
}
""";

        RunTest(content, content, "SomeMethod");
    }

    [Test]
    public void RealWorldExample()
    {
        string content = """"
using Octokit;

namespace AiProgrammer;

public class ProgrammerBrain
{
    private ProgrammerBrainInterface _programmerBrainInterface = new ProgrammerBrainInterface();
    
    public async Task SolveIssue(RequestContext context)
    {
        await PerformSolverSteps(context);
    }
}
"""";
        
        string result = """"
using Octokit;

namespace AiProgrammer;

public class ProgrammerBrain
{
    private ProgrammerBrainInterface _programmerBrainInterface = new ProgrammerBrainInterface();
    
    public async Task SolveIssue(RequestContext context)
}
"""";

        RunTest(content, result);
    }

    public void RunTest(string content, string expectedResult, params string[] methods)
    {
        methods ??= new string[0];

        content = content.ReplaceLineEndings("\n");
        expectedResult = expectedResult.ReplaceLineEndings("\n");
        
        string results = new ClassExtractorCSharp().GetClassContentWithSelectedMethodBodies(new StringReader(content), methods)
            .ReplaceLineEndings("\n").TrimEnd('\n');

        Assert.AreEqual(expectedResult, results);
    }
}

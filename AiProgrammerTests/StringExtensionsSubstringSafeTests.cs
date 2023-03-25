using AiProgrammer.Extensions;
using NUnit.Framework;

namespace AiProgrammerTests
{
    public class StringExtensionsSubstringSafeTests
    {
        [Theory]
        [TestCase(null, 0, 0, null)]
        [TestCase("", 0, 0, "")]
        [TestCase("Hello, World!", 0, 5, "Hello")]
        [TestCase("Hello, World!", 7, 5, "World")]
        [TestCase("Hello, World!", 13, 5, "")]
        [TestCase("Hello, World!", 5, 0, "")]
        [TestCase("Hello, World!", 5, 20, ", World!")]
        [TestCase("Hello, World!", 5, 7, ", World")]
        [TestCase("Hello, World!", 5, 8, ", World!")]
        [TestCase("Hello, World!", 5, 9, ", World!")]
        public void SubstringSafeTests(string source, int startIndex, int length, string expectedResult)
        {
            string? result = source.SubstringSafe(startIndex, length);
            
            Assert.AreEqual(expectedResult, result);
        }
    }
}

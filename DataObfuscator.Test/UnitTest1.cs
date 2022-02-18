using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DataObfuscator.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var lines = File.ReadAllLines("Test1.csv");

            for (int i = 0; i < lines.Length; i += 2)
            {
                var line1 = lines[i];
                var line2 = lines[i + 1];

                TestPair(line1, line2);
            }
        }
    }

    private void TestPair(string[] input, string[] expectedOutput)
    {
        var investor = new InvestorData
        {
            Id = input[0],
            Line1 = input[1],
            ...
            };

        var converted = RandomDataGenerateHelper.Transform(investor);



    }
}
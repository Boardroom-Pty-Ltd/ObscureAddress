using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace ObscureAddress.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            try
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "DataFiles\\";
                var lines = File.ReadAllLines(filePath + "Test1.csv");

                int failedCount = 0;
                for (int i = 0; i < lines.Length; i += 2)
                {
                    var line1 = lines[i];
                    var line2 = lines[i + 1];

                    var input = SplitCsvToFields(line1);
                    if (!input.Any(x => !string.IsNullOrWhiteSpace(x))) { i--; continue; }

                    var expectedOutput = SplitCsvToFields(line2);
                    var isTested = TestPair(input, expectedOutput);
                    if (!isTested) failedCount++;
                }

                if (failedCount > 0)
                    Assert.Fail(failedCount + " cases have been failed.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string[] SplitCsvToFields(string value)
        {
            string[] fields = new string[0];

            var parser = new TextFieldParser(new StringReader(value));
            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");
            parser.TrimWhiteSpace = false;
            while (!parser.EndOfData)
            {
                fields = parser.ReadFields();
            }
            parser.Close();

            return fields;
        }

        private bool TestPair(string[] input, string[] expectedOutput)
        {
            if (input.Length >= 10 && expectedOutput.Length >= 10)
            {
                var investor = new InvestorData
                {
                    Id = input[0],
                    Name = input[1],
                    Line1 = input[2],
                    Line2 = input[3],
                    Line3 = input[4],
                    Line4 = input[5],
                    Line5 = input[6],
                    Line6 = input[7],
                    Postcode = input[8],
                    Domicile = input[9]
                };

                var result = DataObfuscator.Transform(investor);
                var expectedResult = new InvestorData
                {
                    Id = expectedOutput[0],
                    Name = expectedOutput[1],
                    Line1 = expectedOutput[2],
                    Line2 = expectedOutput[3],
                    Line3 = expectedOutput[4],
                    Line4 = expectedOutput[5],
                    Line5 = expectedOutput[6],
                    Line6 = expectedOutput[7],
                    Postcode = expectedOutput[8],
                    Domicile = expectedOutput[9]
                };
                return expectedResult.Equals(result.Data);
            }

            return false;
        }
    }
}
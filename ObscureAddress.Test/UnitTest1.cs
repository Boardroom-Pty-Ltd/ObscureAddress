using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObscureAddress.Model;
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
                    Id = input[0].Trim(),
                    Name = input[1].Trim(),
                    Line1 = input[2].Trim(),
                    Line2 = input[3].Trim(),
                    Line3 = input[4].Trim(),
                    Line4 = input[5].Trim(),
                    Line5 = input[6].Trim(),
                    Line6 = input[7].Trim(),
                    Postcode = input[8].Trim(),
                    Domicile = input[9].Trim()
                };

                var result = DataObfuscator.Transform(investor);
                var expectedResult = new InvestorData
                {
                    Id = expectedOutput[0].Trim(),
                    Name = expectedOutput[1].Trim(),
                    Line1 = expectedOutput[2].Trim(),
                    Line2 = expectedOutput[3].Trim(),
                    Line3 = expectedOutput[4].Trim(),
                    Line4 = expectedOutput[5].Trim(),
                    Line5 = expectedOutput[6].Trim(),
                    Line6 = expectedOutput[7].Trim(),
                    Postcode = expectedOutput[8].Trim(),
                    Domicile = expectedOutput[9].Trim()
                };
                var finalResult = new InvestorData()
                {
                    Id = result.Data.Id.Trim(),
                    Name = result.Data.Name.Trim(),
                    Line1 = result.Data.Line1.Trim(),
                    Line2 = result.Data.Line2.Trim(),
                    Line3 = result.Data.Line3.Trim(),
                    Line4 = result.Data.Line4.Trim(),
                    Line5 = result.Data.Line5.Trim(),
                    Line6 = result.Data.Line6.Trim(),
                    Postcode = result.Data.Postcode.Trim(),
                    Domicile = result.Data.Domicile.Trim()
                };
                return expectedResult.Equals(finalResult);
            }

            return false;
        }
    }
}
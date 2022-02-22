using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObscureAddress.Model;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ObscureAddress.Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestInvestorData()
		{
			bool createOutputFile = false;

			try
			{
				var dataFilesDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "DataFiles");
				if (!dataFilesDirectory.Exists)
					dataFilesDirectory.Create();

				var csvFilePath = dataFilesDirectory.FullName + "\\investor_input.csv";
				var outputFilePath = dataFilesDirectory.FullName + "\\investor_output.csv";
				if (!File.Exists(csvFilePath.ToLower()))
					Assert.Fail("Directory not exists.");
				if (File.Exists(outputFilePath.ToLower()))
					File.Delete(outputFilePath);

				var lines = File.ReadAllLines(csvFilePath);
				if (lines.Length == 0)
					Assert.Fail("Input file is empty.");

				var stringBuilder = new StringBuilder();

				int failedCount = 0;
				for (int i = 0; i < lines.Length; i += 2)
				{
					try
					{
						var line1 = lines[i];
						var line2 = lines[i + 1];

						var input = SplitCsvToFields(line1);
						if (!input.Any(x => !string.IsNullOrWhiteSpace(x))) { i--; continue; }

						var expectedOutput = SplitCsvToFields(line2);
						var isTested = TestInvestorPair(input, expectedOutput, (createOutputFile ? stringBuilder : null));
						if (!isTested) failedCount++;
					}
					catch (Exception ex)
					{
						throw;
					}
				}

				if (createOutputFile)
				{
					File.WriteAllText(outputFilePath, stringBuilder.ToString());
				}

				if (failedCount > 0)
					Assert.Fail(failedCount + " cases have been failed.");
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		[TestMethod]
		public void TestCrsData()
		{
			try
			{
				var dataFilesDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "DataFiles");
				if (!dataFilesDirectory.Exists)
					dataFilesDirectory.Create();

				var csvFilePath = dataFilesDirectory.FullName + "\\crs_input.csv";
				if (!File.Exists(csvFilePath.ToLower()))
					Assert.Fail("Directory not exists.");

				var lines = File.ReadAllLines(csvFilePath);
				if (lines.Length == 0)
					Assert.Fail("Input file is empty.");

				int failedCount = 0;
				for (int i = 0; i < lines.Length; i += 2)
				{
					try
					{
						var line1 = lines[i];
						var line2 = lines[i + 1];

						var input = SplitCsvToFields(line1);
						if (!input.Any(x => !string.IsNullOrWhiteSpace(x))) { i--; continue; }

						var expectedOutput = SplitCsvToFields(line2);
						var isTested = TestCrsPair(input, expectedOutput);
						if (!isTested) failedCount++;
					}
					catch (Exception ex)
					{
						throw;
					}
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

		private bool TestInvestorPair(string[] input, string[] expectedOutput)
		{
			return TestInvestorPair(input, expectedOutput, null);
		}

		private bool TestInvestorPair(string[] input, string[] expectedOutput, StringBuilder stringBuilder = null)
		{
			if (input.Length >= 10 && expectedOutput.Length >= 10)
			{
				var investor = StringArrayToInvestor(input);
				var expectedResult = StringArrayToInvestor(expectedOutput);

				var result = DataObfuscator.Transform(investor);

				var finalResult = new InvestorData()
				{
					Id = result.Data.Id,
					Name = result.Data.Name,
					Line1 = result.Data.Line1,
					Line2 = result.Data.Line2,
					Line3 = result.Data.Line3,
					Line4 = result.Data.Line4,
					Line5 = result.Data.Line5,
					Line6 = result.Data.Line6,
					Postcode = result.Data.Postcode,
					Domicile = result.Data.Domicile,
					Type = result.Data.Type
				};

				if (stringBuilder != null)
				{
					var resultInput = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", input[0], "\"" + input[1] + "\"", "\"" + input[2] + "\"", "\"" + input[3] + "\"", "\"" + input[4] + "\"", "\"" + input[5] + "\"", "\"" + input[6] + "\"", "\"" + input[7] + "\"", "\"" + input[8] + "\"", "\"" + input[9] + "\"", "\"" + input[10] + "\"");
					var resultOutput = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", result.Data.Id, "\"" + result.Data.Name + "\"", "\"" + result.Data.Line1 + "\"", "\"" + result.Data.Line2 + "\"", "\"" + result.Data.Line3 + "\"", "\"" + result.Data.Line4 + "\"", "\"" + result.Data.Line5 + "\"", "\"" + result.Data.Line6 + "\"", "\"" + result.Data.Postcode + "\"", "\"" + result.Data.Domicile + "\"", "\"" + result.Data.Type + "\"");

					stringBuilder.AppendLine(resultInput);
					stringBuilder.AppendLine(resultOutput);
					stringBuilder.AppendLine("");
				}

				return expectedResult.Equals(finalResult);
			}

			return false;
		}

		private bool TestCrsPair(string[] input, string[] expectedOutput)
		{
			if (input.Length >= 15 && expectedOutput.Length >= 15)
			{
				var crsData = StringArrayToCrs(input);
				var expectedResult = StringArrayToCrs(expectedOutput);

				var result = DataObfuscator.Transform(crsData);

				var finalResult = new CrsData()
				{
					Id = result.Data.Id,
					EntityType = result.Data.EntityType,
					FirstName = result.Data.FirstName,
					MiddleName = result.Data.MiddleName,
					LastName = result.Data.LastName,
					RegisteredName = result.Data.RegisteredName,
					CorporationName = result.Data.CorporationName,
					Address1 = result.Data.Address1,
					Address2 = result.Data.Address2,
					Address3 = result.Data.Address3,
					Address4 = result.Data.Address4,
					Address5 = result.Data.Address5,
					GiinNumber = result.Data.GiinNumber,
					AbnNumber = result.Data.AbnNumber,
					AddressDomicile = result.Data.AddressDomicile
				};

				return expectedResult.Equals(finalResult);
			}

			return false;
		}

		private InvestorData StringArrayToInvestor(string[] input)
		{
			if (input.Length < 11) return new InvestorData();

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
				Domicile = input[9],
				Type = input[10]
			};

			return investor;
		}

		private CrsData StringArrayToCrs(string[] input)
		{
			if (input.Length < 15) return new CrsData();

			var crsData = new CrsData
			{
				Id = Convert.ToInt32(input[0]),
				EntityType = Convert.ToChar(input[1]),
				FirstName = input[2],
				MiddleName = input[3],
				LastName = input[4],
				RegisteredName = input[5],
				CorporationName = input[6],
				Address1 = input[7],
				Address2 = input[8],
				Address3 = input[9],
				Address4 = input[10],
				Address5 = input[11],
				GiinNumber = input[12],
				AbnNumber = input[13],
				AddressDomicile = input[14]
			};

			return crsData;
		}
	}
}
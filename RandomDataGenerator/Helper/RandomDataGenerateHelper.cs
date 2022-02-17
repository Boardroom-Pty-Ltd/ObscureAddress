using Newtonsoft.Json;
using RandomDataGenerator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RandomDataGenerator.Helper
{
    public static class RandomDataGenerateHelper
    {
        private static string[] Salutations = new string[] { "MR", "MRS", "MISS", "MS" };
        private static string[] CompanyAbbreviations = new string[] { "PTY LTD", "PTE LTD", "INC", "LIMITED," };
        private static string[] lstPOBOXes = new string[] { "GPO BOX", "PO BOX", "BOX" };

        public static ResponseModel Transform(string filePath, InvestorModel original)
        {
            var preExistingErrors = Validate(original);
            var result = TransformImpl(filePath, original);
            var finalErrors = Validate(result);

            var retval = new ResponseModel
            {
                PreExistingErrors = preExistingErrors,
                FinalErrors = finalErrors,
                Data = result
            };

            return retval;
        }

        private static InvestorModel TransformImpl(string filePath, InvestorModel investor)
        {
            var result = new InvestorModel();
            result = ReadLine(filePath, investor.Id.ToString());
            if (!string.IsNullOrEmpty(result.Id.ToString()))
                return result;
            else
                result = WriteLine(filePath, investor);

            return result;
        }

        private static InvestorModel ReadLine(string filePath, string key)
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                var line = lines.ToList().FirstOrDefault(x => x.StartsWith(key));
                if (line != null)
                {
                    line = line.Replace(key + ": ", string.Empty);
                    return JsonConvert.DeserializeObject<InvestorModel>(line);
                }
            }
            return new InvestorModel();
        }

        private static InvestorModel WriteLine(string filePath, InvestorModel investor)
        {
            var newInvestor = new InvestorModel();
            if (File.Exists(filePath))
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    newInvestor = TransformingData(investor);
                    sw.WriteLine(investor.Id.ToString() + ": {0}", JsonConvert.SerializeObject(newInvestor));
                }
            }
            return newInvestor;
        }

        private static InvestorModel TransformingData(InvestorModel investor)
        {
            var gender = Bogus.DataSets.Name.Gender.Male;
            var postCode = !string.IsNullOrEmpty(investor.Postcode) ? GenerateRandomNumber().ToString() : string.Empty;
            if (!string.IsNullOrEmpty(investor.Line1))
            {
                var valueArray = investor.Line1.Split(' ');
                var value = Salutations.FirstOrDefault(x => x == valueArray[0]);
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "MR")
                        gender = Bogus.DataSets.Name.Gender.Male;
                    else if (value == "MRS" || value == "MISS" || value == "MS")
                        gender = Bogus.DataSets.Name.Gender.Female;
                }
            }

            var newInvestor = new InvestorModel();
            newInvestor.Id = investor.Id;

            var endResultName = GenerateRandomValue(investor.Name, gender, investor.Type, null);
            var endResultLine1 = GenerateRandomValue(investor.Line1, gender, investor.Type, endResultName);
            var endResultLine2 = GenerateRandomValue(investor.Line2, gender, investor.Type, endResultLine1);
            var endResultLine3 = GenerateRandomValue(investor.Line3, gender, investor.Type, endResultLine2);
            var endResultLine4 = GenerateRandomValue(investor.Line4, gender, investor.Type, endResultLine3);
            var endResultLine5 = GenerateRandomValue(investor.Line5, gender, investor.Type, endResultLine4);

            newInvestor.Name = endResultName.Output + new string(' ', (25 - endResultName.Output.Length));
            newInvestor.Line1 = endResultLine1.Output + new string(' ', (30 - endResultLine1.Output.Length));
            newInvestor.Line2 = endResultLine2.Output + new string(' ', (30 - endResultLine2.Output.Length));
            newInvestor.Line3 = endResultLine3.Output + new string(' ', (30 - endResultLine3.Output.Length));
            newInvestor.Line4 = endResultLine4.Output + new string(' ', (30 - endResultLine4.Output.Length));
            newInvestor.Line5 = endResultLine5.Output + new string(' ', (30 - endResultLine5.Output.Length));
            newInvestor.Line6 = investor.Line6 + new string(' ', (30 - investor.Line6.Length));
            newInvestor.Domicile = investor.Domicile;
            newInvestor.Postcode = postCode;
            newInvestor.Type = investor.Type;

            return newInvestor;
        }

        private static EndResult GenerateRandomValue(string value, Bogus.DataSets.Name.Gender gender, string type, EndResult lastResult)
        {
        StartProcess:
            if (string.IsNullOrEmpty(value.Trim())) return new EndResult() { IsName = false, IsNextAddress = false, Output = string.Empty };

            var outputEndResult = new EndResult();
            if (lastResult == null || string.IsNullOrEmpty(lastResult?.Output) || lastResult?.IsNextAddress == false)
            {
                if (Regex.IsMatch(value, @"^\d"))
                {
                    lastResult.IsNextAddress = true;
                    goto StartProcess;
                }
                outputEndResult = GenerateName(value, gender, type);
            }
            else if (lastResult.IsNextAddress == true)
            {
                outputEndResult = GenerateAddress(value, type, lastResult);
            }

            return outputEndResult;
        }

        private static EndResult GenerateName(string value, Bogus.DataSets.Name.Gender gender, string type)
        {
            var endResult = new EndResult();

            if (string.IsNullOrEmpty(value.Trim()))
                return new EndResult();
            if (value.StartsWith("<") && (value.EndsWith(">") || value.TrimEnd().EndsWith(">,")))
                return new EndResult() { IsName = true, IsNextAddress = true, Output = value };
            if (CompanyAbbreviations.Contains(value))
                return new EndResult() { IsName = true, IsNextAddress = true, Output = value };

            var faker = new Bogus.Faker("en_AU");
            var returnString = string.Empty;
            var valueArray = value.Split(' ');
            valueArray.ToList().RemoveAll(x => x == string.Empty);
            if (valueArray.Length > 0)
            {
                var obj = valueArray[0];
                if (Salutations.Contains(obj) && (type == "I" || type == "J"))
                {
                    var nameArray = valueArray.ToList();
                    nameArray.Remove(obj);

                    var name = string.Empty;

                    for (int j = 0; j < nameArray.Count - 1; j++)
                    {
                        name += (!string.IsNullOrEmpty(name) ? " " : string.Empty) + faker.Name.FirstName(gender).ToUpper();
                    }

                    var result = obj.Trim() + " " + name + " " + faker.Name.LastName(gender).ToUpper();
                    returnString += result;
                }
                else if (CompanyAbbreviations.Contains(obj) && type == "C")
                {
                    var result = faker.Company.CompanyName() + " " + obj.Trim();
                    returnString += result;
                }
                else if (lstPOBOXes.Contains(obj))
                {
                    var containValue = lstPOBOXes.FirstOrDefault(x => x == obj.ToUpper());
                    var result = containValue + " " + GenerateRandomNumber();
                    returnString += result;
                }
                else if (type == "C")
                {
                    var result = faker.Company.CompanyName();
                    returnString += result;
                }
                else
                {
                    var name = string.Empty;

                    for (int j = 0; j < valueArray.Length; j++)
                    {
                        name += (!string.IsNullOrEmpty(name) ? " " : string.Empty) + faker.Name.FirstName(gender).ToUpper();
                    }

                    var result = string.Empty;
                    if (valueArray.Length > 1)
                        result = name + " " + faker.Name.LastName(gender).ToUpper();
                    else
                        result = name;

                    returnString += result;
                }
            }

            endResult.IsName = true;
            endResult.IsNextAddress = false;

            if (value.EndsWith(","))
            {
                returnString += ",";
                endResult.IsNextAddress = true;
            }
            else if (value.EndsWith(" ,"))
            {
                returnString += " ,";
                endResult.IsNextAddress = true;
            }
            else if (value.EndsWith(",."))
            {
                returnString += ",.";
                endResult.IsNextAddress = true;
            }
            else if (value.EndsWith("+"))
                returnString += "+";

            endResult.Output = returnString;
            return endResult;
        }

        private static EndResult GenerateAddress(string value, string type, EndResult lastEndResult)
        {
            if (string.IsNullOrEmpty(value.Trim()))
                return new EndResult() { IsName = false, IsNextAddress = true, Output = string.Empty };
            if (value.StartsWith("<") && (value.EndsWith(">") || value.TrimEnd().EndsWith(">,")))
                return new EndResult() { IsName = true, IsNextAddress = true, Output = value };
            if (value.ToUpper().StartsWith("LEVEL"))
                return new EndResult() { IsName = true, IsNextAddress = true, Output = value };

            var endResult = new EndResult();
            var returnString = string.Empty;
            var faker = new Bogus.Faker("en_AU");

            if (lstPOBOXes.Contains(value))
            {
                var containValue = lstPOBOXes.FirstOrDefault(x => x == value.ToUpper());
                var result = containValue + " " + GenerateRandomNumber();
                returnString += result;
            }
            else if (lastEndResult.IsName == true)
                returnString = faker.Address.StreetAddress();
            else
                returnString = faker.Address.City();

            endResult.IsName = false;
            endResult.IsNextAddress = true;
            if (value.EndsWith(","))
            {
                returnString += ",";
                endResult.IsNextAddress = true;
            }
            else if (value.EndsWith(" ,"))
            {
                returnString += " ,";
                endResult.IsNextAddress = true;
            }
            else if (value.EndsWith(",."))
            {
                returnString += ",.";
                endResult.IsNextAddress = true;
            }
            else if (value.EndsWith("+"))
                returnString += "+";

            endResult.Output = returnString;
            return endResult;
        }

        private static int GenerateRandomNumber()
        {
            int _min = 0000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        private static List<string> Validate(InvestorModel investor)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(investor.Id))
                errors.Add("Id must not be empty");
            if ((!string.IsNullOrEmpty(investor.Line1) && investor.Line1.Length != 30) ||
                (!string.IsNullOrEmpty(investor.Line2) && investor.Line2.Length != 30) ||
                (!string.IsNullOrEmpty(investor.Line3) && investor.Line3.Length != 30) ||
                (!string.IsNullOrEmpty(investor.Line4) && investor.Line4.Length != 30) ||
                (!string.IsNullOrEmpty(investor.Line5) && investor.Line5.Length != 30) ||
                (!string.IsNullOrEmpty(investor.Line6) && investor.Line6.Length != 30))
                errors.Add("Address lines must all be 30 char long");

            return errors;
        }
    }

    public class EndResult
    {
        public string Output { get; set; }
        public bool? IsName { get; set; }
        public bool? IsNextAddress { get; set; }

        public EndResult()
        {
            Output = string.Empty;
        }
    }
}
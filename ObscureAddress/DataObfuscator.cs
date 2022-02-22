// keep it static so it is stateless and thread-safe
using ObscureAddress.Model;
using ObscureAddress.Properties;
using ObscureAddress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObscureAddress
{
	public static class DataObfuscator
	{
		private static string[] stringSeparators = new string[] { "\r\n" };
		private static List<string> ignoreWords;
		private static List<string> maleNames;
		private static List<string> femaleNames;
		private static List<string> surNames;
		private static List<string> streetNames;
		private static List<string> townNames;
		private static List<string> countryNames;
		private static List<string> nouns;

		static DataObfuscator()
		{
			ignoreWords = Resources.IgnoreWords.ToUpper().Split(stringSeparators, StringSplitOptions.None).ToList();
			maleNames = Resources.MaleNames.ToUpper().Split(stringSeparators, StringSplitOptions.None).ToList();
			femaleNames = Resources.FemaleNames.ToUpper().Split(stringSeparators, StringSplitOptions.None).ToList();
			surNames = Resources.Surnames.ToUpper().Split(stringSeparators, StringSplitOptions.None).ToList();
			streetNames = Resources.StreetNames.ToUpper().Split(stringSeparators, StringSplitOptions.None).ToList();
			townNames = Resources.TownNames.ToUpper().Split(stringSeparators, StringSplitOptions.None).ToList();
			countryNames = Resources.CountryNames.ToUpper().Split(stringSeparators, StringSplitOptions.None).ToList();
			nouns = Resources.Nouns.ToUpper().Split(stringSeparators, StringSplitOptions.None).ToList();
		}

		public static Result<InvestorData> Transform(InvestorData original)
		{
			var preExistingErrors = ValidateInvestor(original);
			var result = TransformImpl(original);
			var finalErrors = ValidateInvestor(result);

			var retval = new Result<InvestorData>
			{
				PreExistingErrors = preExistingErrors,
				FinalErrors = finalErrors,
				Data = result
			};

			return retval;
		}

		private static InvestorData TransformImpl(InvestorData investor)
		{
			// this function needs to change investor information name(s) and address so that they are not recognizable
			// it must identify pieces of information such as first names, last names, company names, street names, street numbers, etc
			// and then randomly replace them with other names
			//
			// randomness must be repeatable - every time transformation runs for an investor, we need to get the same result
			// to achieve this, random generator must be seeded with investor Id
			//
			// length of all strings that make up an address (line1...6) must always be 30 characters
			// this means that as you replace strings, you need to use substitutes of the same length
			// I recommend to find list of common English names and surnames, list of known Australian town and street names
			// then you can run lookup by length and always replace strings with the same length
			// there will be situation when the word is unknown (it may be a company name, foreign names and localities, etc..)
			// in this case generate random word (remember that randomness is seeded by InvestorId)
			//
			// consider the following approach
			// keep list of the following: male names, female names, surnames, towns, streets, etc...
			// keep list of words that must not change: salutations (Mr, Ms, etc.), company abbreviations (Pty Ltd, Inc, etc..)
			// split original address in words
			// for each word identify what list it belongs to (use hashtable or dictionary for fast search)
			// try to find word of the same length in that list, or else generate random word
			// replace the word
			// run validation on the end result to make sure nothing is broken

			var newInvestor = new InvestorData { Id = investor.Id, Name = investor.Name, Postcode = investor.Postcode, Domicile = investor.Domicile, Type = investor.Type };

			var seedId = Convert.ToInt32(investor.Id.Replace("S", string.Empty));
			newInvestor.Line1 = DataTransform(seedId, investor.Line1, 30);
			newInvestor.Line2 = DataTransform(seedId, investor.Line2, 30);
			newInvestor.Line3 = DataTransform(seedId, investor.Line3, 30);
			newInvestor.Line4 = DataTransform(seedId, investor.Line4, 30);
			newInvestor.Line5 = DataTransform(seedId, investor.Line5, 30);
			newInvestor.Line6 = DataTransform(seedId, investor.Line6, 30);

			return newInvestor;
		}

		public static Result<CrsData> Transform(CrsData original)
		{
			var preExistingErrors = ValidateCrs(original);
			var result = TransformImpl(original);
			var finalErrors = ValidateCrs(result);

			var retval = new Result<CrsData>
			{
				PreExistingErrors = preExistingErrors,
				FinalErrors = finalErrors,
				Data = result
			};

			return retval;
		}

		private static CrsData TransformImpl(CrsData crsData)
		{
			var newCrs = new CrsData { Id = crsData.Id, EntityType = crsData.EntityType, AddressDomicile = crsData.AddressDomicile };

			newCrs.FirstName = DataTransform(newCrs.Id, crsData.FirstName.ToUpper());
			newCrs.MiddleName = DataTransform(newCrs.Id, crsData.MiddleName.ToUpper());
			newCrs.LastName = DataTransform(newCrs.Id, crsData.LastName.ToUpper());

			newCrs.RegisteredName = DataTransform(newCrs.Id, crsData.RegisteredName.ToUpper());
			newCrs.CorporationName = DataTransform(newCrs.Id, crsData.CorporationName.ToUpper());

			newCrs.Address1 = DataTransform(newCrs.Id, crsData.Address1.ToUpper());
			newCrs.Address2 = DataTransform(newCrs.Id, crsData.Address2.ToUpper());
			newCrs.Address3 = DataTransform(newCrs.Id, crsData.Address3.ToUpper());
			newCrs.Address4 = DataTransform(newCrs.Id, crsData.Address4.ToUpper());
			newCrs.Address5 = DataTransform(newCrs.Id, crsData.Address5.ToUpper());

			newCrs.GiinNumber = DataTransform(newCrs.Id, crsData.GiinNumber);
			newCrs.AbnNumber = DataTransform(newCrs.Id, crsData.AbnNumber);

			return newCrs;
		}

		private static string DataTransform(int id, string value)
		{
			return DataTransform(id, value, 0);
		}

		private static string DataTransform(int id, string value, int maxLength)
		{
			var seed = id.ToString().Replace("S", "");
			var random = new Random(Convert.ToInt32(seed));

			var tokens = SplitValue(value);
			var tokenValues = tokens.Select(x => Replace(x, random)).ToList();
			var finalValue = string.Join(string.Empty, tokenValues.Select(x => x.Value));
			return finalValue + (maxLength > 0 ? new string(' ', (maxLength - finalValue.Length)) : string.Empty);
		}

		private static List<Token> SplitValue(string str)
		{
			var tokenizer = new StringTokenizer(str);
			var words = new List<Token>();
			while (true)
			{
				var token = tokenizer.Next();
				if (token.Kind == TokenKind.Eof) break;
				words.Add(token);
			}
			return words;
		}

		private static Token Replace(Token token, Random random)
		{
			switch (token.Kind)
			{
				case TokenKind.WhiteSpace:
					return token;

				case TokenKind.Symbol:
					return token;

				case TokenKind.Number:
					return new Token()
					{
						Kind = token.Kind,
						Value = ReplaceNumber(token.Value, random)
					};

				case TokenKind.Word:
					return new Token()
					{
						Kind = token.Kind,
						Value = ReplaceWord(token.Value, random)
					};
			}

			return token;
		}

		private static string ReplaceNumber(string word, Random random)
		{
			const string chars = "0123456789";
			return new string(Enumerable.Repeat(chars, word.Length).Select(s => s[random.Next(s.Length)]).ToArray());
		}

		private static string ReplaceWord(string word, Random random)
		{
			if (ignoreWords.Contains(word))
				return word;

			if (maleNames.Contains(word))
				return FindReplacement(maleNames, word, random);

			if (femaleNames.Contains(word))
				return FindReplacement(femaleNames, word, random);

			if (surNames.Contains(word))
				return FindReplacement(surNames, word, random);

			if (countryNames.Contains(word))
				return word;  //return FindReplacement(countryNames, word, random);

			if (townNames.Contains(word))
				return FindReplacement(townNames, word, random);

			if (streetNames.Contains(word))
				return FindReplacement(streetNames, word, random);

			if (nouns.Contains(word))
				return FindReplacement(nouns, word, random);

			return FindReplacement(nouns, word, random);
			//return ReplaceUnknownWord(word, random);
		}

		private static string FindReplacement(List<string> list, string word, Random random)
		{
			var suitableByLengthList = list.Where(x => x.Length == word.Length).ToList();
			var randomSelection = RandomlySelectFromList(suitableByLengthList, random);
			return randomSelection;
		}

		private static string RandomlySelectFromList(List<string> list, Random random)
		{
			if (list.Count == 0) return string.Empty;
			return list[random.Next(list.Count)];
		}

		private static string ReplaceUnknownWord(string word, Random random)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			return new string(Enumerable.Repeat(chars, word.Length).Select(s => s[random.Next(s.Length)]).ToArray());
		}

		private static List<string> ValidateInvestor(InvestorData investor)
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
			{
				errors.Add("Address lines must all be 30 char long");
			}

			return errors;
		}

		private static List<string> ValidateCrs(CrsData crsData)
		{
			var errors = new List<string>();

			if (crsData.Id <= 0)
				errors.Add("Id must not be empty");

			if ((!string.IsNullOrEmpty(crsData.FirstName) && crsData.FirstName.Length > 50) ||
				(!string.IsNullOrEmpty(crsData.MiddleName) && crsData.MiddleName.Length > 50) ||
				(!string.IsNullOrEmpty(crsData.LastName) && crsData.LastName.Length > 50))
			{
				errors.Add("Name lines must all be 50 char long");
			}

			if ((!string.IsNullOrEmpty(crsData.RegisteredName) && crsData.RegisteredName.Length > 300) ||
				(!string.IsNullOrEmpty(crsData.CorporationName) && crsData.CorporationName.Length > 300))
			{
				errors.Add("Registration Name lines must all be 300 char long");
			}

			if ((!string.IsNullOrEmpty(crsData.Address1) && crsData.Address1.Length > 30) ||
				(!string.IsNullOrEmpty(crsData.Address2) && crsData.Address2.Length > 30) ||
				(!string.IsNullOrEmpty(crsData.Address3) && crsData.Address3.Length > 30) ||
				(!string.IsNullOrEmpty(crsData.Address4) && crsData.Address4.Length > 30) ||
				(!string.IsNullOrEmpty(crsData.Address5) && crsData.Address5.Length > 30))
			{
				errors.Add("Address lines must all be 30 char long");
			}

			return errors;
		}
	}
}
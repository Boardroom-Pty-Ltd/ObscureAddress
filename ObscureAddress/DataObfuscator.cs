// keep it static so it is stateless and thread-safe

using ObscureAddress.Helper;
using ObscureAddress.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class DataObfuscator
{
    private static string filePath = AppDomain.CurrentDomain.BaseDirectory + "Data\\";
    private static string femaleFileName = filePath + "FemaleNames.txt";
    private static string maleFileName = filePath + "MaleNames.txt";
    private static string surnameFileName = filePath + "Surnames.txt";
    private static string streetAddressFileName = filePath + "StreetAddresses.txt";
    private static string townFileName = filePath + "TownNames.txt";

    //static string[] ignoreWords = new string[] { "MR", "MRS", "MISS", "MS", "PTY LTD,", "PTY LTD", "PTE LTD,", "PTE LTD", "INC.", "INC", "PTY LIMITED,", "LIMITED,", "LIMITED", "CORP", "GPO BOX", "PO BOX", "BOX", "C/-" };
    private static string[] ignoreWords = new string[] { "MR", "MRS", "MISS", "MS", "PTY", "PTE", "LTD,", "LTD", "INC.", "INC", "LIMITED,", "LIMITED", "CORP", "GPO", "PO", "BOX", "C/-" };

    private static List<string> maleNames = new List<string>();
    private static List<string> femaleNames = new List<string>();
    private static List<string> surNames = new List<string>();
    private static List<string> streetAddresses = new List<string>();
    private static List<string> townNames = new List<string>();

    public static Result<InvestorData> Transform(InvestorData original)
    {
        if (File.Exists(femaleFileName))
            femaleNames = File.ReadAllLines(femaleFileName).ToList();
        if (File.Exists(maleFileName))
            maleNames = File.ReadAllLines(maleFileName).ToList();
        if (File.Exists(surnameFileName))
            surNames = File.ReadAllLines(surnameFileName).ToList();
        if (File.Exists(streetAddressFileName))
            streetAddresses = File.ReadAllLines(streetAddressFileName).ToList();
        if (File.Exists(townFileName))
            townNames = File.ReadAllLines(townFileName).ToList();

        var preExistingErrors = Validate(original);
        var result = TransformImpl(original);
        var finalErrors = Validate(result);

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

        var newInvestor = new InvestorData();
        newInvestor.Id = investor.Id;
        newInvestor.Name = investor.Name;
        newInvestor.Postcode = investor.Postcode;
        newInvestor.Domicile = investor.Domicile;

        var words = new string[] { };
        var seed = investor.Id.Replace("S", "");
        var random = new Random(Convert.ToInt32(seed));

        //Line1 Code
        words = SplitIntoWords(investor.Line1);

        for (int i = 0; i < words.Length; i++)
        {
            var stringTokenizer = new StringTokenizer(words[i]);
            //words[i] = Replace(stringTokenizer.Next(), random).Value;
            words[i] = Replace(stringTokenizer, random);
        }
        newInvestor.Line1 = string.Join(" ", words);
        newInvestor.Line1 = newInvestor.Line1 + new string(' ', (30 - newInvestor.Line1.Length));

        //Line2 Code
        words = SplitIntoWords(investor.Line2);

        for (int i = 0; i < words.Length; i++)
        {
            var stringTokenizer = new StringTokenizer(words[i]);
            words[i] = Replace(stringTokenizer, random);
        }
        newInvestor.Line2 = string.Join(" ", words);
        newInvestor.Line2 = newInvestor.Line2 + new string(' ', (30 - newInvestor.Line2.Length));

        //Line3 Code
        words = SplitIntoWords(investor.Line3);

        for (int i = 0; i < words.Length; i++)
        {
            var stringTokenizer = new StringTokenizer(words[i]);
            words[i] = Replace(stringTokenizer, random);
        }
        newInvestor.Line3 = string.Join(" ", words);
        newInvestor.Line3 = newInvestor.Line3 + new string(' ', (30 - newInvestor.Line3.Length));

        //Line4 Code
        words = SplitIntoWords(investor.Line4);

        for (int i = 0; i < words.Length; i++)
        {
            var stringTokenizer = new StringTokenizer(words[i]);
            words[i] = Replace(stringTokenizer, random);
        }
        newInvestor.Line4 = string.Join(" ", words);
        newInvestor.Line4 = newInvestor.Line4 + new string(' ', (30 - newInvestor.Line4.Length));

        //Line5 Code
        words = SplitIntoWords(investor.Line5);

        for (int i = 0; i < words.Length; i++)
        {
            var stringTokenizer = new StringTokenizer(words[i]);
            words[i] = Replace(stringTokenizer, random);
        }
        newInvestor.Line5 = string.Join(" ", words);
        newInvestor.Line5 = newInvestor.Line5 + new string(' ', (30 - newInvestor.Line5.Length));

        //Line6 Code
        words = SplitIntoWords(investor.Line6);

        for (int i = 0; i < words.Length; i++)
        {
            var stringTokenizer = new StringTokenizer(words[i]);
            words[i] = Replace(stringTokenizer, random);
        }
        newInvestor.Line6 = string.Join(" ", words);
        newInvestor.Line6 = newInvestor.Line6 + new string(' ', (30 - newInvestor.Line6.Length));

        return newInvestor;
    }

    private static string[] SplitIntoWords(string values)
    {
        return values.Split(' ');
    }

    private static string Replace(StringTokenizer stringTokenizer, Random random)
    {
        var word = string.Empty;
        Check:
        var token = stringTokenizer.Next();
        if (token.Kind != TokenKind.Eof)
        {
            word += Replace(token, random).Value;
            goto Check;
        }
        return word;
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

        if (streetAddresses.Contains(word))
            return FindReplacement(streetAddresses, word, random);

        if (townNames.Contains(word))
            return FindReplacement(townNames, word, random);

        return ReplaceUnknownWord(word, random);

        // foreach (var list in replaceLists)
        // {
        // }

        return string.Empty;
    }

    private static string FindReplacement(List<string> list, string word, Random random)
    {
        var suitableByLengthList = list.Where(x => x.Length == word.Length).ToList();
        var randomSelection = RandomlySelectFromList(suitableByLengthList, random);
        return randomSelection;
    }

    private static string RandomlySelectFromList(List<string> list, Random random)
    {
        //Pick any string from List data
        return list[random.Next(list.Count)];
    }

    private static string ReplaceUnknownWord(string word, Random random)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, word.Length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static List<string> Validate(InvestorData investor)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(investor.Id))
            errors.Add("Id must not be empty");

        if (investor.Line1.Length != 30 ||
            investor.Line2.Length != 30 ||
            investor.Line3.Length != 30 ||
            investor.Line4.Length != 30 ||
            investor.Line5.Length != 30 ||
            investor.Line6.Length != 30)
        {
            errors.Add("Address lines must all be 30 char long");
        }

        // etc - move validation to be added here

        return errors;
    }
}
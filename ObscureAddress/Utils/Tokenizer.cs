using System;
using System.Linq;

namespace ObscureAddress.Utils
{
    public enum TokenKind
    {
        Unknown,
        Word,
        Number,
        QuotedString,
        WhiteSpace,
        Symbol,
        Eol,
        Eof
    }

    public class TextPos
    {
        public int Lin { get; set; }
        public int Col { get; set; }
        public int Pos { get; set; }
    }

    public class Token
    {
        public Token()
        { }

        public Token(TokenKind kind, string value, TextPos start, TextPos end)
        {
            this.Kind = kind;
            this.Value = value;
            this.Start = start;
            this.End = end;
        }

        public TextPos Start { get; set; }
        public TextPos End { get; set; }

        public TokenKind Kind { get; set; }
        public string Value { get; set; }
        public bool Incomplete { get; set; }

        public override string ToString()
        {
            return "[" + this.Kind + "] " + this.Value;
        }
    }

    public class StringTokenizer
    {
        private const char Eof = (char)0;

        private int _line;
        private int _column;

        private char[] _symbolChars;

        private int _saveLine;
        private int _saveCol;
        private int _savePos;

        public StringTokenizer(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.Data = data;
            this.QuoteChar = '\"';
            this.ValidSymbolsInWord = new char[] { };

            Reset();
        }

        public bool IgnoreWhiteSpace { get; set; }
        public char QuoteChar { get; set; }
        public int Pos { get; private set; }
        public string Data { get; private set; }
        public char[] ValidSymbolsInWord { get; set; }

        public Token Next()
        {
        ReadToken:

            char ch = Test(0);

            if (ch == this.QuoteChar) return ReadString();

            switch (ch)
            {
                case Eof:
                    return CreateEmptyToken(TokenKind.Eof);

                case ' ':
                case '\t':
                    {
                        if (this.IgnoreWhiteSpace)
                        {
                            Consume();
                            goto ReadToken;
                        }
                        else
                            return ReadWhitespace();
                    }
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ReadNumber();

                case '\r':
                    {
                        StartRead();
                        Consume();
                        if (Test(0) == '\n')
                            Consume(); // on DOS/Windows we have \r\n for new line

                        _line++;
                        _column = 0;

                        return CreateToken(TokenKind.Eol);
                    }
                case '\n':
                    {
                        StartRead();
                        Consume();
                        _line++;
                        _column = 0;

                        return CreateToken(TokenKind.Eol);
                    }

                default:
                    {
                        if (Char.IsLetter(ch) || ch == '_')
                            return ReadWord();
                        else if (IsSymbol(ch))
                        {
                            StartRead();
                            Consume();
                            return CreateToken(TokenKind.Symbol);
                        }
                        else
                        {
                            StartRead();
                            Consume();
                            return CreateToken(TokenKind.Unknown);
                        }
                    }
            }
        }

        public void Reset()
        {
            this.IgnoreWhiteSpace = false;
            this._symbolChars = new char[] { '\'', '=', '+', '-', '/', ',', '.', '*', '~', '!', '@', '#', '$', '%', '^', '&', '(', ')', '{', '}', '[', ']', ':', ';', '<', '>', '?', '|', '\\' };

            _line = 0;
            _column = 0;
            Pos = 0;
        }

        private char Test(int count)
        {
            if (Pos + count >= Data.Length)
                return Eof;
            else
                return Data[Pos + count];
        }

        private char Consume()
        {
            char ret = Data[Pos];
            Pos++;
            _column++;

            return ret;
        }

        private Token CreateEmptyToken(TokenKind kind)
        {
            return new Token(kind, "",
                             new TextPos { Lin = _saveLine, Col = _saveCol, Pos = _savePos },
                             new TextPos { Lin = _saveLine, Col = _saveCol, Pos = _savePos });
        }

        private Token CreateToken(TokenKind kind)
        {
            string tokenData = Data.Substring(_savePos, Pos - _savePos);
            return new Token(kind, tokenData,
                             new TextPos { Lin = _saveLine, Col = _saveCol, Pos = _savePos },
                             new TextPos { Lin = _line, Col = _column - 1, Pos = Pos - 1 });
        }

        private void StartRead()
        {
            _saveLine = _line;
            _saveCol = _column;
            _savePos = Pos;
        }

        private Token ReadWhitespace()
        {
            StartRead();

            Consume(); // consume the looked-ahead whitespace char

            while (true)
            {
                char ch = Test(0);
                if (ch == '\t' || ch == ' ')
                    Consume();
                else
                    break;
            }

            return CreateToken(TokenKind.WhiteSpace);
        }

        private Token ReadNumber()
        {
            StartRead();

            bool hadDot = false;

            Consume(); // read first digit

            while (true)
            {
                char ch = Test(0);
                if (Char.IsDigit(ch))
                    Consume();
                else if (ch == '.' && !hadDot)
                {
                    hadDot = true;
                    Consume();
                }
                else
                    break;
            }

            return CreateToken(TokenKind.Number);
        }

        private Token ReadWord()
        {
            StartRead();

            Consume(); // consume first character of the word

            while (true)
            {
                char ch = Test(0);
                if (Char.IsLetterOrDigit(ch) || ch == '_' || ValidSymbolsInWord.Contains(ch))
                    Consume();
                else
                    break;
            }

            return CreateToken(TokenKind.Word);
        }

        private Token ReadString()
        {
            StartRead();

            Consume(); // read "

            bool incomplete = true;

            while (true)
            {
                char ch = Test(0);
                if (ch == Eof)
                    break;
                else if (ch == '\r')    // handle CR in strings
                {
                    Consume();
                    if (Test(0) == '\n')    // for DOS & windows
                        Consume();

                    _line++;
                    _column = 0;
                }
                else if (ch == '\n')    // new line in quoted string
                {
                    Consume();

                    _line++;
                    _column = 0;
                }
                else if (ch == this.QuoteChar)
                {
                    Consume();
                    if (Test(0) != this.QuoteChar)
                    {
                        // done reading, and this quotes does not have escape character
                        incomplete = false;
                        break;
                    }
                    else
                        Consume(); // consume second ", because first was just an escape
                }
                else
                    Consume();
            }

            var retval = CreateToken(TokenKind.QuotedString);
            retval.Incomplete = incomplete;
            return retval;
        }

        private bool IsSymbol(char c)
        {
            for (int i = 0; i < _symbolChars.Length; i++)
                if (_symbolChars[i] == c)
                    return true;

            return false;
        }
    }

    public class WordToken
    {
        public string Space { get; set; }
        public string Pre { get; set; }
        public string Word { get; set; }
        public string Post { get; set; }
    }

    public class WordTokenizer
    {
        private readonly string _data;
        private int _pos;

        public WordTokenizer(string data)
        {
            _data = data;
            _pos = 0;
        }

        public WordToken Next()
        {
            var l = _data.Length;
            string space = null;

            // get whitespace

            while (_pos < l && char.IsWhiteSpace(_data[_pos]))
            {
                space += _data[_pos];
                _pos++;
            }

            // get the token

            string token = null;

            while (_pos < l && !char.IsWhiteSpace(_data[_pos]))
            {
                token += _data[_pos];
                _pos++;
            }

            if (token == null) return null;

            // split the token

            string pre = null;
            string post = null;
            string word = null;

            int pr = 0;
            int po = token.Length - 1;

            for (int i = 0; i < token.Length; i++)
                if (char.IsLetterOrDigit(token[i]))
                {
                    pr = i;
                    break;
                }

            for (int i = token.Length - 1; i >= 0; i--)
                if (char.IsLetterOrDigit(token[i]))
                {
                    po = i;
                    break;
                }

            //if (pr != po)
            {
                if (pr > 0) pre = token.Substring(0, pr);
                if (po < token.Length - 1) post = token.Substring(po + 1);
                word = token.Substring(pr, po - pr + 1);
            }
            //else word = token;

            return new WordToken
            {
                Space = space,
                Pre = pre,
                Word = word,
                Post = post
            };
        }
    }
}
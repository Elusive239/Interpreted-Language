// https://github.com/tlaceby/guide-to-interpreters-series
// -----------------------------------------------------------
// ---------------          LEXER          -------------------
// ---  Responsible for producing tokens from the source   ---
// -----------------------------------------------------------

// Represents tokens that our language understands in parsing.

using System.Text;
using ITLang.Util;

namespace ITLang.Frontend
{
    // Represents a single token from the source-code.
    public class Lexer
    {
        private TokenListFactory tokenizer;

        public Lexer(ReadOnlySpan<char> sourceCode)
        {
            tokenizer = new TokenListFactory(sourceCode);
        }

        public static bool IsKeyWord(string word, out TokenType tk)
        {
            tk = word switch
            {
                "let" => TokenType.Let,
                "const" => TokenType.Const,
                "func" => TokenType.Fn,
                "if" => TokenType.If,
                "elif" => TokenType.Elif,
                "else" => TokenType.Else,
                "for" => TokenType.For,
                "while" => TokenType.While,
                _ => tk = TokenType.Number,
            };
            return tk != TokenType.Number;
        }

        private static char Escape(char escape)
        {
            return escape switch
            {
                'n' => '\n',
                'r' => '\r',
                't' => '\t',
                'v' => '\v',
                'f' => '\f',
                '0' => '\0',
                '\'' => '\'',
                '\"' => '\'',
                '\\' => '\\',
                _ => throw new Exception("How did we get here?")
            };
        }

        public bool IsCommentToken()
        {
            if (!tokenizer.At.Equals('#'))
                return IsStringToken();

            tokenizer.Shift();
            while (tokenizer.Has() && !tokenizer.At.Equals('\n') && !tokenizer.At.Equals('#'))
                tokenizer.Shift();
            tokenizer.Shift();

            return true;
        }

        public bool IsStringToken()
        {
            if (!tokenizer.At.Equals('\'') && !tokenizer.At.Equals('\"'))
                return IsGreaterOrLessOperatorToken();

            bool isQuote = tokenizer.Shift().Equals('\"');
            tokenizer.AddToken(isQuote ? '\"' : '\'', isQuote ? TokenType.Quote : TokenType.Apostrophe);
            StringBuilder stringBuilder = new StringBuilder();
            while (tokenizer.Has() && tokenizer.At != '\'' && tokenizer.At != '\"')
            {
                if (tokenizer.At.Equals('\\'))
                {
                    tokenizer.Shift();
                    stringBuilder.Append(Escape(tokenizer.Shift()));
                    continue;
                }
                stringBuilder.Append(tokenizer.Shift());
            }
            tokenizer.AddToken(stringBuilder.ToString(), TokenType.String);
            //End the string
            if ((tokenizer.At.Equals('\"') && !isQuote) || (tokenizer.At.Equals('\'') && isQuote))
            {
                return false;
                //throw new Exception($"String does not start with {tokenizer.At}.");
            }
            tokenizer.AddToken(tokenizer.Shift(), isQuote ? TokenType.Quote : TokenType.Apostrophe);

            return true;
        }

        public bool IsGreaterOrLessOperatorToken()
        {
            if (!tokenizer.At.Equals('>') && !tokenizer.At.Equals('<'))
                return IsEquivalentOrAssignmentOperatorToken();
            StringBuilder builder = new StringBuilder();
            builder.Append(tokenizer.Shift());
            if (tokenizer.Has() && tokenizer.At.Equals('='))
                builder.Append(tokenizer.Shift());
            tokenizer.AddToken(builder.ToString(), TokenType.BooleanOperator);
            return true;
        }

        public bool IsEquivalentOrAssignmentOperatorToken()
        {
            if (!tokenizer.At.Equals('='))
                return IsAndOrOperatorToken();
            tokenizer.Shift();
            if (tokenizer.Has() && tokenizer.At.Equals('='))
            {
                tokenizer.Shift();
                tokenizer.AddToken("==", TokenType.BooleanOperator);
            }
            else
                tokenizer.AddToken('=', TokenType.Equals);

            return true;
        }

        public bool IsAndOrOperatorToken()
        {
            if (!tokenizer.At.Equals('&') && !tokenizer.At.Equals('|'))
                return IsMathOrUnaryOperatorToken();
            bool andOr = tokenizer.Shift().Equals('&');
            if (andOr && tokenizer.At.Equals('&'))
                tokenizer.AddToken("&&", TokenType.BooleanOperator);
            else if (!andOr && tokenizer.At.Equals('|'))
                tokenizer.AddToken("||", TokenType.BooleanOperator);
            else
                return false;
                //throw new Exception("Expected boolean operator where there was none during lexing.");
            tokenizer.Shift();
            return true;
        }

        public bool IsMathOrUnaryOperatorToken()
        {
            if (!tokenizer.At.Equals('+') &&
                !tokenizer.At.Equals('-') &&
                !tokenizer.At.Equals('*') &&
                !tokenizer.At.Equals('/') &&
                !tokenizer.At.Equals('%'))
            {
                return IsSingleCharToken();
            }
            //This is one of the cases that I think we dont need a string builder, 
            //because its like just one string creation? i think?
            char tk = tokenizer.Shift();
            //don't wanna bother with the modulo unary operator.
            if (!tk.Equals('%') && tokenizer.At.Equals(tk))
            {
                tokenizer.AddToken(tk + "" + tokenizer.Shift(), TokenType.UnaryOperator);
            }
            else tokenizer.AddToken(tk, TokenType.BinaryOperator);
            return true;
        }

        public bool IsSingleCharToken()
        {
            if (!"(){}[];:,.".Contains(tokenizer.At))
                return IsNumberToken();
            char tk = tokenizer.Shift();

            TokenType type = tk switch
                {
                    '(' => TokenType.OpenParen,
                    ')' => TokenType.CloseParen,
                    '{' => TokenType.OpenBrace,
                    '}' => TokenType.CloseBrace,
                    '[' => TokenType.OpenBracket,
                    ']' => TokenType.CloseBracket,
                    ';' => TokenType.Semicolon,
                    ':' => TokenType.Colon,
                    ',' => TokenType.Comma,
                    '.' => TokenType.Dot,
                    _ => TokenType.Number,
                    //throw new Exception("How did we get here?")
                };
            tokenizer.AddToken(tk, type);
            return type != TokenType.Number;
        }

        public bool IsNumberToken()
        {
            if (!tokenizer.IsInt())
                return IsIdentifierToken();
            StringBuilder numBuilder = new StringBuilder();
            while (tokenizer.Has() && tokenizer.IsInt())
            {
                numBuilder.Append(tokenizer.Shift());
            }
            // append new numeric token.
            tokenizer.AddToken(numBuilder.ToString(), TokenType.Number);
            return true;
        }

        public bool IsIdentifierToken()
        {
            if (!tokenizer.IsAlpha())
                return IsSkippableToken();

            StringBuilder identBuilder = new StringBuilder();
            while (tokenizer.Has() && tokenizer.IsAlpha())
            {
                identBuilder.Append(tokenizer.Shift());
            }
            string finIdent = identBuilder.ToString();
            // CHECK FOR RESERVED KEYWORDS
            // If value is not undefined then the identifier is
            // a reconized keyword
            if (IsKeyWord(finIdent, out TokenType t))
            {
                tokenizer.AddToken(finIdent, t);
            }
            else
            {
                // Unreconized name must mean user defined symbol.
                tokenizer.AddToken(finIdent, TokenType.Identifier);
            }
            return true;
        }

        public bool IsSkippableToken()
        {
            if (!tokenizer.IsSkippable())
                return false;
            // Skip uneeded chars.
            while(tokenizer.IsSkippable())
                tokenizer.Shift();
            return true;
        }

        /*
        * Given a string representing source code: Produce tokens and handles
        * possible unidentified characters.
        *
        * - Returns a array of tokens.
        * - Does not modify the incoming string.
        */
        public Token[] Tokenize()
        {
            while (tokenizer.Has())
            {
                bool error = IsCommentToken();
                if(error is false)
                    throw new Exception($"Unrecognized character found in source: {tokenizer.At} {tokenizer}");
            }
            tokenizer.AddToken("EndOfFile", TokenType.EOF);
           

            //return new Token[] {new Token("", tp, 0)};
            return tokenizer.GetFinishedTokens();
        }
    }
}
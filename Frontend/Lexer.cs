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
        public static bool IsKeyWord(string word, out TokenType tk){
            tk = word switch {
                "let" => TokenType.Let,
                "const" => TokenType.Const,
                "func" => TokenType.Fn,
                "if" => TokenType.If,
                "elif" => TokenType.Elif,
                "else" => TokenType.Else,
                _ => tk = TokenType.Number,
            };
            return tk != TokenType.Number;
        }

        private static char Escape(char escape){
            return escape switch{
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

        /*
        * Given a string representing source code: Produce tokens and handles
        * possible unidentified characters.
        *
        * - Returns a array of tokens.
        * - Does not modify the incoming string.
        */
        public Token[] Tokenize(ReadOnlySpan<char> sourceCode)
        {
            TokenListFactory tokenizer = new TokenListFactory(sourceCode);

            while (tokenizer.Has())
            {
                switch (tokenizer.At)
                {
                    case '(':

                        tokenizer.AddToken(tokenizer.Shift(), TokenType.OpenParen);
                        break;
                    case ')':
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.CloseParen);
                        break;
                    case '{':
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.OpenBrace);
                        break;
                    case '}':
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.CloseBrace);
                        break;
                    case '[':
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.OpenBracket);
                        break;
                    case ']':
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.CloseBracket);
                        break;

                    case '#':
                        while (tokenizer.Has() && !tokenizer.At.Equals('\n'))
                        {
                            tokenizer.Shift();
                        }
                        tokenizer.Shift();
                        break;

                    case '\"':
                    case '\'':
                        bool isQuote = tokenizer.Shift().Equals('\'');
                        tokenizer.AddToken(isQuote ? '\"' : '\'', isQuote ? TokenType.Quote : TokenType.Apostrophe);
                        StringBuilder stringBuilder = new StringBuilder();
                        while (tokenizer.Has() && tokenizer.At != '\'' && tokenizer.At != '\"')
                        {
                            if (tokenizer.At.Equals('\\'))
                            {
                                tokenizer.Shift();
                                stringBuilder.Append (Escape(tokenizer.Shift()));
                                continue;
                            }

                            stringBuilder.Append(tokenizer.Shift());
                        }
                        tokenizer.AddToken(stringBuilder.ToString(), TokenType.String);
                        //End the string

                        if ((tokenizer.At.Equals('\"') && isQuote) || (tokenizer.At.Equals('\'') && !isQuote))
                        {
                            throw new Exception($"String does not start with {tokenizer.At}.");
                        }

                        tokenizer.AddToken(tokenizer.Shift(), isQuote ? TokenType.Quote : TokenType.Apostrophe);
                        break;

                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '%':
                    //This is one of the cases that I think we dont need a string builder, 
                    //because its like just one string creation? i think?
                        char tk = tokenizer.Shift();
                        //don't wanna bother with the modulo unary operator.
                        if (!tk.Equals('%') && tokenizer.At.Equals(tk))
                        {
                            tokenizer.AddToken( (tk + "" + tokenizer.Shift()), TokenType.UnaryOperator);
                        }
                        else tokenizer.AddToken(tk, TokenType.BinaryOperator);
                        break;
                    case '>':
                    case '<':
                    StringBuilder builder = new StringBuilder();
                    builder.Append(tokenizer.Shift());
                        if (tokenizer.Has() && tokenizer.At.Equals('='))
                            builder.Append(tokenizer.Shift());
                        tokenizer.AddToken(builder.ToString(), TokenType.BooleanOperator);
                        break;
                    case '=':
                        tokenizer.Shift();
                        if (tokenizer.Has() && tokenizer.At.Equals('='))
                        {
                            tokenizer.Shift();
                            tokenizer.AddToken("==", TokenType.BooleanOperator);
                        }
                        else
                        {
                            tokenizer.AddToken('=', TokenType.Equals);
                        }
                        break;
                    case '&':
                    case '|':
                        bool andOr = tokenizer.Shift().Equals('&');
                        if (andOr && tokenizer.At.Equals('&'))
                        {
                            tokenizer.AddToken("&&", TokenType.BooleanOperator);
                        }
                        else if (!andOr && tokenizer.At.Equals('|'))
                        {
                            tokenizer.AddToken("||", TokenType.BooleanOperator);
                        }
                        else throw new Exception("Expected boolean operator where there was none during lexing.");
                        tokenizer.Shift();
                        break;
                    case ';':
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.Semicolon);
                        break;
                    case ':':
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.Colon);
                        break;
                    case ',':
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.Comma);
                        break;
                    case '.':
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.Dot);
                        break;

                    default:
                        if (tokenizer.IsInt())
                        {
                            StringBuilder numBuilder = new StringBuilder();
                            while (tokenizer.Has() && tokenizer.IsInt())
                            {
                                numBuilder.Append (tokenizer.Shift());
                            }
                            // append new numeric token.
                            tokenizer.AddToken(numBuilder.ToString(), TokenType.Number);
                        }
                        else if (tokenizer.IsAlpha())
                        {
                            StringBuilder identBuilder = new StringBuilder();
                            while (tokenizer.Has() && tokenizer.IsAlpha())
                            {
                                identBuilder.Append (tokenizer.Shift());
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
                        }
                        else if (tokenizer.IsSkippable() || !tokenizer.Has())
                        {
                            // Skip uneeded chars.
                            if (tokenizer.Has())
                            {
                                tokenizer.Shift();
                            }
                            else
                            {
                                throw new Exception($"Unrecognized character found in source: {tokenizer.At} {tokenizer}");
                            }
                        }
                        break;
                }
            }
            tokenizer.AddToken("EndOfFile", TokenType.EOF);
            return tokenizer.GetFinishedTokens();
        }
    }
}
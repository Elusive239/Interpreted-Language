// https://github.com/tlaceby/guide-to-interpreters-series
// -----------------------------------------------------------
// ---------------          LEXER          -------------------
// ---  Responsible for producing tokens from the source   ---
// -----------------------------------------------------------

// Represents tokens that our language understands in parsing.

using ITLang.Util;

namespace ITLang.Frontend
{
    // Represents a single token from the source-code.
    public static class Lexer
    {
        public static Dictionary<string, TokenType> KEYWORDS = new Dictionary<string, TokenType>{
            {"let", TokenType.Let},
            {"const", TokenType.Const},
            {"func", TokenType.Fn},
            {"if", TokenType.If},
            {"elif", TokenType.Elif},
            {"else", TokenType.Else}
        };

        private static Dictionary<string, string> ESCAPES = new Dictionary<string, string>{
            {"n", "\n"},
            {"r", "\r"},
            {"t", "\t"},
            {"v", "\v"},
            {"f", "\f"},
            {"0", "\0"},
            {"\"", "\""},
            {"\'", "\'"},
            {"\\", "\\"}
        };

        /*
        * Given a string representing source code: Produce tokens and handles
        * possible unidentified characters.
        *
        * - Returns a array of tokens.
        * - Does not modify the incoming string.
        */
        public static Token[] Tokenize(string sourceCode)
        {
            TokenListFactory tokenizer = new TokenListFactory(sourceCode);

            while (tokenizer.Has())
            {
                switch (tokenizer.At())
                {
                    case "(":

                        tokenizer.AddToken(tokenizer.Shift(), TokenType.OpenParen);
                        break;
                    case ")":
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.CloseParen);
                        break;
                    case "{":
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.OpenBrace);
                        break;
                    case "}":
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.CloseBrace);
                        break;
                    case "[":
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.OpenBracket);
                        break;
                    case "]":
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.CloseBracket);
                        break;

                    case "#":
                        while (tokenizer.Has() && !tokenizer.At().Equals("\n"))
                        {
                            tokenizer.Shift();
                        }
                        tokenizer.Shift();
                        break;

                    case "\"":
                    case "\'":
                        bool isQuote = tokenizer.Shift().Equals("\"");
                        tokenizer.AddToken(isQuote ? "\"" : "\'", isQuote ? TokenType.Quote : TokenType.Apostrophe);
                        string value = "";
                        while (tokenizer.Has() && tokenizer.At() != "\"" && tokenizer.At() != "\'")
                        {
                            if (tokenizer.At().Equals("\\"))
                            {
                                tokenizer.Shift();
                                value += ESCAPES[tokenizer.Shift()];
                                continue;
                            }

                            value += tokenizer.Shift();
                        }
                        tokenizer.AddToken(value, TokenType.String);
                        //End the string

                        if ((tokenizer.At().Equals("\"") && !isQuote) || (tokenizer.At().Equals("\'") && isQuote))
                        {
                            throw new Exception($"String does not start with {tokenizer.At()}.");
                        }

                        tokenizer.AddToken(tokenizer.Shift(), isQuote ? TokenType.Quote : TokenType.Apostrophe);
                        break;

                    case "+":
                    case "-":
                    case "*":
                    case "/":
                    case "%":
                        string tk = tokenizer.Shift();
                        //don't wanna bother with the modulo unary operator.
                        if (!tk.Equals("%") && tokenizer.At().Equals(tk))
                        {
                            tokenizer.AddToken(tk + tokenizer.Shift(), TokenType.UnaryOperator);
                        }
                        else tokenizer.AddToken(tk, TokenType.BinaryOperator);
                        break;
                    case ">":
                    case "<":
                        string str = tokenizer.Shift();
                        if (tokenizer.Has() && tokenizer.At().Equals("="))
                            str += tokenizer.Shift();
                        tokenizer.AddToken(str, TokenType.BooleanOperator);
                        break;
                    case "=":
                        tokenizer.Shift();
                        if (tokenizer.Has() && tokenizer.At().Equals("="))
                        {
                            tokenizer.Shift();
                            tokenizer.AddToken("==", TokenType.BooleanOperator);
                        }
                        else
                        {
                            tokenizer.AddToken("=", TokenType.Equals);
                        }
                        break;
                    case "&":
                    case "|":
                        bool andOr = tokenizer.Shift().Equals("&");
                        if (andOr && tokenizer.At().Equals("&"))
                        {
                            tokenizer.AddToken("&&", TokenType.BooleanOperator);
                        }
                        else if (!andOr && tokenizer.At().Equals("|"))
                        {
                            tokenizer.AddToken("||", TokenType.BooleanOperator);
                        }
                        else throw new Exception("Expected boolean operator where there was none during lexing.");
                        tokenizer.Shift();
                        break;
                    case ";":
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.Semicolon);
                        break;
                    case ":":
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.Colon);
                        break;
                    case ",":
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.Comma);
                        break;
                    case ".":
                        tokenizer.AddToken(tokenizer.Shift(), TokenType.Dot);
                        break;

                    default:
                        if (tokenizer.IsInt())
                        {
                            string num = "";
                            while (tokenizer.Has() && tokenizer.IsInt())
                            {
                                num += tokenizer.Shift();
                            }
                            // append new numeric token.
                            tokenizer.AddToken(num, TokenType.Number);
                        }
                        else if (tokenizer.IsAlpha())
                        {
                            string ident = "";
                            while (tokenizer.Has() && tokenizer.IsAlpha())
                            {
                                ident += tokenizer.Shift();
                            }

                            // CHECK FOR RESERVED KEYWORDS
                            // If value is not undefined then the identifier is
                            // a reconized keyword
                            if (KEYWORDS.ContainsKey(ident))
                            {
                                tokenizer.AddToken(ident, KEYWORDS[ident]);
                            }
                            else
                            {
                                // Unreconized name must mean user defined symbol.
                                tokenizer.AddToken(ident, TokenType.Identifier);
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
                                throw new Exception($"Unrecognized character found in source: {tokenizer.At()} {tokenizer}");
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
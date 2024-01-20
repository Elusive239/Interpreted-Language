using ITLang.Frontend;
namespace ITLang.Util{
    public class TokenListFactory : List<char>{
        private int lineCount, currentIndex;
        private List<Token> tokens;
        public string At() => Has() ? this[currentIndex] + "" : "";
        public int LineCount => lineCount;

        /*
        *   Takes the source code and adds the individual 
        *   chars into the factory (a list).
        *   Initializes a list of tokens, 
        *   sets the line count to 1, 
        *   and sets the current index to 0.
        */
        public TokenListFactory(string sourceCode){
            foreach(char c in sourceCode){
                this.Add(c);
            }

            tokens = new List<Token>();
            lineCount = 1;
            currentIndex = 0;
        }

        /*
        *   Return whether the character is a valid integer -> [0-9]
        */
        public bool IsInt() {
            char c = this[currentIndex];
            const char upper = '9', lower = '0';
            return c >= lower && c <= upper;
        }

        /*
        *   Returns true if the character is whitespace like -> [\s, \t, \n]
        */
        public bool IsSkippable() {
            bool isSpace = At().Equals(" "),
                isNewLine = At().Equals("\n"),
                isTab = At().Equals("\t"),
                isCariageReturn = At().Equals("\r");

            if(isNewLine){
                lineCount++;
            }
            
            return  isSpace || isNewLine  || isTab || isCariageReturn;
        }

        /*
        *   Returns whether the character passed in is either:
        *   a) alphabetic -> [a-zA-Z]
        *   b) and underscore -> _
        */
        public bool IsAlpha() {
            return At().ToUpper() != At().ToLower() || At() == "_";
        }

        /*
        *   Returns a token of a given type and value
        */
        public void AddToken(string value = "", TokenType type = TokenType.Number){
            tokens.Add(new Token(value, type, LineCount));
        }

        /*
        *   Returns the current token and 
        *   shifts the current index right 1.
        */
        public string Shift(){
            string c = At();
            currentIndex ++;
            return c;
        }

        /*
        *   Removes the end of file token.
        */
        public void RemoveEOF() => tokens.Remove(tokens[^1 ]);
        
        /*
        *   Checks if there are any tokens left to parse.
        */
        public bool Has() => this.currentIndex < this.Count ;

        /*
        *   Returns the tokens as an array.
        */
        public Token[] GetFinishedTokens() => tokens.ToArray();
    }
}
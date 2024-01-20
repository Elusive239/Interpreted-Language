using ITLang.Frontend;
namespace ITLang.Util{
    public class TokenStack : List<Token>{
        /*
        *   The index of the current token.
        */
        private int currentIndex = 0;

        /*
        *   Returns the index of the current token 
        */
        public int CurrentToken => currentIndex;

        /*
        *   Returns the lineNumber stored in the current token.
        */
        public int LineNumber => At().lineNum;

        /*
        *   A list of tokens to be iterated over.
        */
        public TokenStack(Token[] tokens){
            foreach(Token token in tokens){
                Add(token);
            }
            currentIndex = 0;
        }

        /*
        *   Returns the token at the CurrentToken int, 
        *   optional offset lets you access previous and 
        *   future tokens, if you need to.
        */
        public Token At(int offset = 0) => this[currentIndex + offset];
        
        /*
        *   Returns the current character 
        *   and moves the current index right 1.
        */
        public Token Eat() {
            Token fstToken = this[currentIndex];
            currentIndex++;
            return fstToken;
        }

        /*
        *   If the source code is the same why reparse everthing?
        *   sets the current index to 0 so we can parse tokens from the begining.
        *   never call this yourself.
        */
        public void Restart(){
            currentIndex = 0;
        }

        /*
        *   If At() has the wrong token type we throw an exception.
        *   Then it eats the current token and returns it.
        */
        public Token Expect(TokenType type, string err){
			if ( this.CurrentToken >= this.Count){
                throw new Exception($"Parser Error:\n {err} - Out of tokens at Line {LineNumber}");
            } if(At().type != type) {
				throw new Exception($"Parser Error:\n {err} {At()} - Expecting: {new Token("", At().type).Stringify()} at Line {LineNumber}");
			}
			return Eat();
		}

        /*
        *   If no the end of file, return false.
        */
        public bool Not_eof()  {
			if(this.Count <= currentIndex)
				return false;
			
			return this[currentIndex].type != TokenType.EOF;
		}
    }
}
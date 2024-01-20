namespace ITLang.Frontend
{
    // Reoresents a single token from the source-code.
    public class Token
    {   
        /*
        *   Contains the raw value as seen inside the source code as a C# string.
        */
        public string value; 

        /*
        *   The type of the tagged structure.
        */
        public TokenType type; 

        /*
        *   The line number this taken was created on.
        */
        public int lineNum;

        /*
        *   A parsed token. cant be a a full word, 
        *   like "if", or can be a single symbol "=".
        */
        public Token(string _value, TokenType _type, int _lineNum = 0)
        {
            this.value = _value;
            this.type = _type;
            this.lineNum = _lineNum;
        }

    }
}
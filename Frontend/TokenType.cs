namespace ITLang.Frontend
{
    public enum TokenType
    {
        // Literal Types
        Number,
        String, 
        Identifier,
        // Keywords
        Let,
        Const,
        Fn, // fn
        If, 
        Elif, 
        Else, 

        // Grouping * Operators
        BinaryOperator,
        BooleanOperator,
        UnaryOperator,
        Equals,
        Comma,
        Dot,
        Colon,
        Semicolon,
        OpenParen, // (
        CloseParen, // )
        OpenBrace, // {
        CloseBrace, // }
        OpenBracket, // [
        CloseBracket, //]
        Quote, // \" CUSTOM
        Apostrophe, // \' CUSTOM
        Hashtag, //#
        EOF // Signified the end of file
    }
}
namespace ITLang.Frontend
{
    /*
    *   Type of node to be used in parsing.
    */
    public enum NodeType{
        // STATEMENTS
        Program,
        VarDeclaration,
        FunctionDeclaration,
        // EXPRESSIONS
        AssignmentExpr,
        MemberExpr,
        CallExpr,
        IfStmt,    
        WhileStmt,
        ForStmt,
        BooleanExpr,    
        UnaryExpr,
        BinaryExpr,
        // Literals
        Property,
        ObjectLiteral,
        NumericLiteral,
        StringLiteral,
        Identifier
    }
}
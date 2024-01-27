// deno-lint-ignore-file no-empty-interface
// https://github.com/tlaceby/guide-to-interpreters-series

// -----------------------------------------------------------
// --------------          AST TYPES        ------------------
// ---     Defines the structure of our languages AST      ---
// -----------------------------------------------------------

namespace ITLang.Frontend{
    /*
    *   Statements do not result in a value at runtime.
    *   They contain one or more expressions internally 
    */

    public class Stmt{
        public NodeType kind;
    }

    /**
    *   Defines a block which contains many statements.
    *   - Only one program will be contained in a file.
    */
    public class Program : Stmt{
        public List<Stmt> body = new List<Stmt>();
        public Program(){
            kind = NodeType.Program;
        }
    }

    /*
    *   A variable declaration from the source code.
    *   Determines whether a variable is constant, 
    *   its identifier, 
    *   and its value (including a null value)
    */
    public class VarDeclaration : Stmt{
        public bool constant;
        public string identifier = "";
        public Expr? value = null;
        public VarDeclaration(){
            kind = NodeType.VarDeclaration;
        }
    }

    public class FunctionDeclaration : Stmt{
        public string[] parameters;
        public string name;
        public Stmt[] body;
        public FunctionDeclaration(){
            kind = NodeType.FunctionDeclaration;
            name = "";
            parameters = Array.Empty<string>();
            body = Array.Empty<Stmt>();
        }
    }

    /*
    *   Represents an if statement in the source code.
    */
    public class IfStmt : Stmt{
        public Expr? expr;
        public List<Stmt> body;
        public IfStmt? elseStmt;
        public IfStmt(Expr expr){
            kind = NodeType.IfStmt;
            this.expr = expr;
            body = new List<Stmt>(1);
        }
    }

    public class WhileLoopStmt : Stmt{
        public Expr? boolean;
        
        public List<Stmt>? body;
        public WhileLoopStmt(){
            kind = NodeType.WhileStmt;
        }
    } 

    public class ForLoopStmt : WhileLoopStmt{
        public Stmt? defVar;
        public Expr? unaryOperator;
        public ForLoopStmt(){
            kind = NodeType.ForStmt;
        }
    }


    /*
    *   Expressions will result in a value at runtime unlike Statements 
    */
    public class Expr : Stmt {}

    /*
    *   Takes an Expression that can be Evaluated into:
    *   assigne => an Identifier that alread exists and
    *   value => a value or expression like a string, number, or function.
    */
    public class AssignmentExpr : Expr{
        public Expr? assigne;
        public Expr? value;
        public AssignmentExpr(){
            kind = NodeType.AssignmentExpr;
            value = null;
            assigne = null;
        }
    }

    /*
    *   A operation with two sides seperated by a operator.
    *   Both sides can be ANY Complex Expression.
    *   - Supported Operators -> + | - | / | * | %
    */
    public class BinaryExpr : Expr{
        public Expr? left;
        public Expr? right;
        public string op;
        public BinaryExpr(){
            kind = NodeType.BinaryExpr;
            left = null;
            right = null;
            op = "";
        }
    }

    /*
    *   A operation with one side and an operator.
    *   - Supported Operators -> ++ | --
    */
    public class UnaryExpr : Expr{
        public Expr? value;
        public string op;
        public UnaryExpr(){
            kind = NodeType.UnaryExpr;
            value = null;
            op = "";
        }
    }

    /*
    *   A function expression to be evaluated.
    *   args => an array of expressions.
    *   caller => the thing that calls the function expression.
    */
    public class CallExpr : Expr{
        public Expr[]? args;
        public Expr? caller;
        public CallExpr(){
            kind = NodeType.CallExpr;
            caller = null;
            args = Array.Empty<Expr>();
        }
    }

    /*
    *   An expression that holds an object expression 
    *   and the desired property that object supposedly 
    *   contains.
    */
    public class MemberExpr : Expr{
        public Expr? obj;
        public Expr? property;
        public bool computed;

        public MemberExpr(){
            kind = NodeType.MemberExpr;
            obj = null;
            property = null;
        }
    }

    // LITERAL / PRIMARY EXPRESSION TYPES
    /*
    * Represents a user-defined variable or symbol in source.
    */
    public class Identifier : Expr{
        public string symbol;
        public Identifier(){
            kind = NodeType.Identifier;
            symbol = "";
        }

        public Identifier(string symbol){
            kind = NodeType.VarDeclaration;
            this.symbol = symbol;
        }
    }

    /*
    * Represents a numeric constant inside the soure code.
    */
    public class NumericLiteral : Expr{
        public float value;
        public NumericLiteral(float value = 0){
            kind = NodeType.NumericLiteral;
            this.value = value;
        }
    }

    /*
    *   Represents a C# boolean expression within the source code.
    */
    public class BooleanExpr : Expr{
        public bool value;
        public BooleanExpr(bool value = true){
            kind = NodeType.BooleanExpr;
            this.value = value;
        }
    }

    /*
    *   Represents a string constant inside the source code.
    */
    public class StringLiteral : Expr{
        public string value;
        public StringLiteral(string value = ""){
            kind = NodeType.StringLiteral;
            this.value = value;
        }
    }

    /*
    *   Represents a property to be used inside object literals.
    */
    public class Property : Expr{
        public string key;
        public Expr? value;
        public Property(){
            kind = NodeType.Property;
            key = "";
            value = null;
        }
    }

    /*
    *   Represents a user defined object and its collection of properties.
    */
    public class ObjectLiteral : Expr{
        public Property[] properties = Array.Empty<Property>();

        public ObjectLiteral(){
            kind = NodeType.ObjectLiteral;
        }
        
        public ObjectLiteral AddProperty(string ident, Expr? value){
            Property[] newArr = new Property[this.properties.Length+1];
            for(int i = 0; i < properties.Length; i++){
                newArr[i] = this.properties[i];
            }
            newArr[this.properties.Length] = new Property{
                key = ident, 
                value = value
            };
            this.properties = newArr;
            return this;
        }
    }
}
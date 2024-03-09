using ITLang.Frontend;
using ITLang.Util;
using static ITLang.Runtime.Eval.Expressions;
using static ITLang.Runtime.Eval.Statements;

namespace ITLang.Runtime
{
    public class Interpreter{
        public static RuntimeVal Evaluate(Stmt astNode, Enviornment env){
            if(astNode == null) 
                throw new Exception("astNode is null. How did this happen???");
            return astNode.kind switch
            {
                NodeType.NumericLiteral => new NumberVal(((NumericLiteral)astNode).value),
                NodeType.StringLiteral => new StringVal(((StringLiteral)astNode).value),
                NodeType.Identifier => Eval_Identifier((Identifier)astNode, env),
                NodeType.ObjectLiteral => Eval_Object_Expr((ObjectLiteral)astNode, env),
                NodeType.CallExpr => Eval_Call_Expr((CallExpr)astNode, env),
                NodeType.AssignmentExpr => Eval_Assignment((AssignmentExpr)astNode, env),
                NodeType.BinaryExpr => Eval_Binary_Expr((BinaryExpr)astNode, env),
                NodeType.Program => Eval_Program((Program)astNode, env),
                NodeType.VarDeclaration => Eval_Var_Declaration((VarDeclaration)astNode, env),
                NodeType.FunctionDeclaration => Eval_Function_Declaration((FunctionDeclaration)astNode, env),
                NodeType.MemberExpr => Eval_Member_Expr((MemberExpr)astNode, env),
                NodeType.BooleanExpr => new BooleanVal(((BooleanExpr)astNode).value),
                NodeType.IfStmt => Eval_If_Expr((IfStmt)astNode, env),
                NodeType.UnaryExpr => Eval_Unary_Expr((UnaryExpr)astNode, env ),
                NodeType.WhileStmt or NodeType.ForStmt => Eval_Loop((WhileLoopStmt) astNode, env),
                NodeType.ClassLiteral => Eval_Class_Expr((ClassLiteral)astNode, env),
                _ => throw new Exception($"This AST Node ({astNode.kind.Stringify()}) has not yet been setup for interpretation."),
            };
        }
    }
}
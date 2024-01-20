using ITLang.Frontend;
using static ITLang.Runtime.Values;
using static ITLang.Runtime.Interpreter;

namespace ITLang.Runtime.Eval
{
    public static class Statements{
        /*
        *   Evaluates a program AST Node.
        *   Should be the first AST Node evaluated.
        */
        public static RuntimeVal Eval_Program(Program program, Enviornment env) {
            RuntimeVal lastEvaluated = MK_NULL();
            if(program.body == null) return lastEvaluated;

            foreach (var statement in program.body) {
                lastEvaluated = Evaluate(statement, env);
            }
            return lastEvaluated;
        }

        /*
        *   Declares a variable in the passed in environment.
        *   if the declaration has no value, will initalize the 
        *   variable with a NullVal instead.
        */
        public static RuntimeVal Eval_Var_Declaration(VarDeclaration declaration, Enviornment env ) {
            RuntimeVal value = declaration.value != null
                ? Evaluate(declaration.value, env)
                : MK_NULL();

            return env.DeclareVar(declaration.identifier, value, declaration.constant);
        }

        /*
        *   Declares a function in the passed in environment.
        *   Function can be native or from source code.
        */
        public static RuntimeVal Eval_Function_Declaration(FunctionDeclaration declaration, Enviornment env) {
            // Create new function scope
            FunctionValue fn = new FunctionValue(
                declaration.name,
                declaration.parameters,
                env, 
                declaration.body
            );

            return env.DeclareVar(declaration.name, fn, true);
        }

        /*
        *   Evaluates basic if statement. 
        *   handles regular if statements and evaluates
        *   elif/else statements if they exit. 
        */
        public static RuntimeVal Eval_If_Expr(IfStmt ifExpr, Enviornment env)
        {   
            RuntimeVal returnValue = MK_NULL();
            var tmpEnv = new Enviornment(env);

            if(ifExpr == null){
                return returnValue;
            }

            BooleanVal isTrue = (BooleanVal) MK_BOOL();
            if(ifExpr.expr != null){
                isTrue = (BooleanVal) Evaluate(ifExpr.expr, tmpEnv);
            }
            if(isTrue.value){
                foreach(Stmt stmt in ifExpr.body){
                    returnValue = Evaluate(stmt, tmpEnv);
                }
            }else if(!isTrue.value && ifExpr.elseStmt != null){
                returnValue = Evaluate(ifExpr.elseStmt, tmpEnv);
            }

            return returnValue;
        }
    }
}